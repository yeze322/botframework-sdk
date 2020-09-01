// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using IssueNotificationBot.Models;
using IssueNotificationBot.Services;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Teams;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace IssueNotificationBot
{
    public class SignInBot<T> : TeamsActivityHandler where T : Dialog
    {
        protected readonly BotState ConversationState;
        protected readonly Dialog Dialog;
        protected readonly ILogger Logger;
        protected readonly BotState UserState;
        protected readonly NotificationHelper NotificationHelper;
        protected readonly UserStorage UserStorage;
        protected TrackedUser Maintainer;

        public SignInBot(ConversationState conversationState, UserState userState, T dialog, ILogger<SignInBot<T>> logger, UserStorage userStorage, NotificationHelper notificationHelper)
        {
            ConversationState = conversationState;
            UserState = userState;
            Dialog = dialog;
            Logger = logger;
            NotificationHelper = notificationHelper;
            UserStorage = userStorage;
        }

        public override async Task OnTurnAsync(ITurnContext turnContext, CancellationToken cancellationToken = default)
        {
            await base.OnTurnAsync(turnContext, cancellationToken);

            // Save any state changes that might have occurred during the turn.
            await ConversationState.SaveChangesAsync(turnContext, false, cancellationToken);
            await UserState.SaveChangesAsync(turnContext, false, cancellationToken);
        }

        protected override async Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            var handled = false;

            turnContext.Activity.RemoveMentionText(turnContext.Activity.Recipient.Id);

            //If the user sends the Login Command and we don't have their info, send them through the auth dialog
            if (string.Equals(turnContext.Activity.Text, Constants.LoginCommand, StringComparison.InvariantCultureIgnoreCase) ||
                !await UserStorage.HaveUserDetails(turnContext.Activity.From.Id))
            {
                // We don't want to send the OAuth card to a group conversation
                if (turnContext.Activity.Conversation.ConversationType == Constants.PersonalConversationType)
                {
                    Logger.LogInformation($"Running SignInDialog for {turnContext.Activity.From.Name}");

                    await Dialog.RunAsync(turnContext, ConversationState.CreateProperty<DialogState>(nameof(DialogState)), cancellationToken);
                }
                else
                {
                    Logger.LogInformation($"{turnContext.Activity.From.Name} sent the bot a message from a Group channel and we don't have their information.");

                    await turnContext.SendActivityAsync(Constants.NoOAuthInGroupConversationsResponse);
                }

                handled = true;
            }

            // Handle maintainer commands
            if (Maintainer == null)
            {
                Maintainer = await UserStorage.GetTrackedUserFromGitHubUserId(Constants.MaintainerGitHubId);
            }

            // Check if Maintainer name matches incoming. ID seems to change between bots.
            if (!handled && turnContext.Activity.From.Name == Maintainer?.TeamsUserInfo.Name)
            {
                if (string.Equals(turnContext.Activity.Text, Constants.EnableMaintainerNotificationsCommand, StringComparison.InvariantCultureIgnoreCase))
                {
                    Logger.LogInformation("Enabling Maintainer Notifications");
                    NotificationHelper.NotifyMaintainer = true;
                    await turnContext.SendActivityAsync("Maintainer Notifications Enabled");
                }
                else if (string.Equals(turnContext.Activity.Text, Constants.DisableMaintainerNotificationsCommand, StringComparison.InvariantCultureIgnoreCase))
                {
                    Logger.LogInformation("Disabling Maintainer Notifications");
                    NotificationHelper.NotifyMaintainer = false;
                    await turnContext.SendActivityAsync("Maintainer Notifications Disabled");
                }
                else if (string.Equals(turnContext.Activity.Text, Constants.MaintainerTestCards, StringComparison.InvariantCultureIgnoreCase))
                {
                    await SendMaintainerTestCards(turnContext);
                }
                else if (string.Equals(turnContext.Activity.Text, Constants.MaintainerResendGreetings, StringComparison.InvariantCultureIgnoreCase))
                {
                    await ResendGreetings(turnContext, cancellationToken);
                }

                handled = true;
            }

            // Check for and catch Adaptive Card Submit actions
            if (!handled)
            {
                handled = await HandleAdaptiveCardSubmitActions(turnContext, cancellationToken);
            }

            // Bot doesn't know how to handle anything else
            if (!handled)
            {
                Logger.LogInformation($"Unable to handle message: {turnContext.Activity.Text} from: {turnContext.Activity.From.Name}");
                await turnContext.SendActivityAsync(Constants.NoConversationResponse);
            }
        }

        protected override async Task OnTeamsSigninVerifyStateAsync(ITurnContext<IInvokeActivity> turnContext, CancellationToken cancellationToken)
        {
            Logger.LogInformation("Running dialog with signin/verifystate from an Invoke Activity.");

            // The OAuth Prompt needs to see the Invoke Activity in order to complete the login process.

            // Run the Dialog with the new Invoke Activity.
            await Dialog.RunAsync(turnContext, ConversationState.CreateProperty<DialogState>(nameof(DialogState)), cancellationToken);
        }

        private async Task<bool> HandleAdaptiveCardSubmitActions(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            var handled = false;

            // It's highly likely this is an Adaptive Card Submit action if Activity.Value is populated and Activity.Text is not
            if (turnContext.Activity.Value != null && string.IsNullOrEmpty(turnContext.Activity.Text))
            {
                try
                {
                    var data = ((JObject)turnContext.Activity.Value).ToObject<AdaptiveCardIssueSubmitAction>();
                    Logger.LogInformation($"Received AdaptiveCard data: {data.action}");
                    switch (data.action)
                    {
                        case Constants.HideIssueNotificationAction:
                            await turnContext.DeleteActivityAsync(turnContext.Activity.ReplyToId, cancellationToken);
                            break;
                    }
                    handled = true;
                }
                catch(Exception e)
                {
                    Logger.LogError($"Unable to cast to AdaptiveCardSubmitAction.\n{e.Message}");
                }
            }
            return handled;
        }

        private async Task SendMaintainerTestCards(ITurnContext<IMessageActivity> turnContext)
        {
            var welcomeCard = TemplateCardHelper.GetUserWelcomeCard(
                        Maintainer.GitHubDetails.Avatar_url,
                        Maintainer.GitHubDetails.Login,
                        Maintainer.GitHubDetails.Name,
                        Maintainer);

            var fakeIssue = new
            {
                Title = "Test Title",
                CreatedAt = new DateTime(),
                Body = "Test Body",
                Url = "www.contoso.com",
                Number = 999
            };

            var issueCard = TemplateCardHelper.GetPersonalIssueCard(
                JsonConvert.DeserializeObject<GitHubIssue>(JsonConvert.SerializeObject(fakeIssue)),
                "TEST EXIRE MESSAGE",
                new DateTime(),
                "TEST ACTION",
                Maintainer);

            await turnContext.SendActivitiesAsync(new IActivity[] { MessageFactory.Attachment(welcomeCard), MessageFactory.Attachment(issueCard) });
        }

        private async Task ResendGreetings(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            Logger.LogInformation("Resending Greetings");

            var members = await TeamsInfo.GetTeamMembersAsync(turnContext);
            foreach (var member in members)
            {
                try
                {
                    if (!await UserStorage.HaveUserDetails(member.Id))
                    {
                        await GreetNewTeamMember(member, turnContext, cancellationToken);
                    }
                    else
                    {
                        Logger.LogInformation($"Already have login info for: { member.Name }");
                    }
                }
                // Users that block the bot throw Forbidden errors. We'll catch all exceptions in case
                // unforseen errors occur; we want to message as many members as possible.
                catch(Exception e)
                {
                    Logger.LogError(new EventId(1), e, $"Something went wrong when greeting { member.Name }");
                }
            }
        }

        internal async Task GreetNewTeamMember(ChannelAccount member, ITurnContext turnContext, CancellationToken cancellationToken)
        {
            Logger.LogInformation($"Greeting new member: { member.Name }");

            var conversationReference = turnContext.Activity.GetConversationReference();
            conversationReference.User = member;

            // TODO: Eventually, it would be nice to begin the SignInDialog here, proactively. However, I keep getting NotFound errors
            // When SignInDialog calls GetToken
            await NotificationHelper.CreatePersonalConversationAsync(conversationReference, async (turnContext2, cancellationToken2) =>
            {
                var activity = MessageFactory.Text($"Hello! I am {Constants.UserAgent} and I can notify you about your GitHub issues in the Bot Framework repositories that are about to \"expire\".\n" +
                        "An \"expired\" issue is one with the `customer-reported` tag, and is nearing or past:\n" +
                        "* 72 hours with no `customer-replied` tag\n" +
                        "* 30 days and still open\n" +
                        "* 90 days and still open\n\n" +
                        "To get started, type \"login\" so that I can get your GitHub information.");
                activity.TeamsNotifyUser();
                await turnContext2.SendActivityAsync(activity, cancellationToken2);
            }, cancellationToken);
        }
    }
}
