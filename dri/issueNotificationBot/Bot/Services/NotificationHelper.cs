// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using IssueNotificationBot.Models;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Integration.AspNet.Core;
using Microsoft.Bot.Builder.Teams;
using Microsoft.Bot.Connector;
using Microsoft.Bot.Connector.Authentication;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace IssueNotificationBot.Services
{
    public class NotificationHelper
    {
        private readonly IBotFrameworkHttpAdapter Adapter;
        private readonly IConfiguration Configuration;
        public readonly ConcurrentDictionary<int, MappedIssue> IssueActivityMap = new ConcurrentDictionary<int, MappedIssue>();
        private readonly ILogger Logger;
        public bool NotifyMaintainer = true;
        private readonly UserStorage UserStorage;

        public NotificationHelper(IBotFrameworkHttpAdapter adapter, IConfiguration configuration, ILogger<NotificationHelper> logger, UserStorage userStorage)
        {
            Adapter = adapter;
            Configuration = configuration;
            Logger = logger;
            UserStorage = userStorage;

            // If the channel is the Emulator, and authentication is not in use,
            // the AppId will be null.  We generate a random AppId for this case only.
            // This is not required for production, since the AppId will have a value.
            if (string.IsNullOrEmpty(Configuration["MicrosoftAppId"]))
            {
                Configuration["MicrosoftAppId"] = Guid.NewGuid().ToString(); //if no AppId, use a random Guid
            }

            // Notify the maintainer of this bot of any errors via Teams.
            // We need to do this here and not in AdapterWithErrorHandler to avoid circular dependencies.
            var originalOnTurnError = (adapter as AdapterWithErrorHandler)?.OnTurnError;

            (adapter as AdapterWithErrorHandler)!.OnTurnError = async (turnContext, exception) =>
            {
                if (NotifyMaintainer)
                {
                    var maintainer = await userStorage.GetTrackedUserFromGitHubUserId(Constants.MaintainerGitHubId);
                    if (maintainer != null && turnContext.Activity.From.Name != maintainer.TeamsUserInfo.Name)
                    {
                        var errorMessage = $"Error occurred for {turnContext?.Activity?.From?.Name}:\n{exception.Message}\n{exception.StackTrace}\n{turnContext?.Activity}";
                        Logger.LogError(errorMessage);
                        await SendProactiveNotificationToUserAsync(maintainer, MessageFactory.Text(errorMessage));
                    }

                    await turnContext.SendActivityAsync("I've notified the maintainer of this bot about this error.");
                }

                await originalOnTurnError(turnContext, exception);
            };
        }

        public async Task<ResourceResponse> SendProactiveNotificationToUserAsync(TrackedUser user, IActivity activity, CancellationToken cancellationToken = default)
        {
            var tcs = new TaskCompletionSource<ResourceResponse>();
            await CreatePersonalConversationAsync(user.ConversationReference, async (turnContext, cancellationToken2) =>
            {
               var activityId = await turnContext.SendActivityAsync(activity, cancellationToken2);
               tcs.SetResult(activityId);
            }, cancellationToken);

            return tcs.Task.GetAwaiter().GetResult();
        }

        public async Task SendIssueNotificationToUserAsync(TrackedUser user, GitHubIssue issue, string nearingOrExpiredMessage, DateTime expires, string action, CancellationToken cancellationToken = default)
        {
            Logger.LogInformation($"Sending Issue notification to {user.TeamsUserInfo.Name} for {issue.Number}");

            var maintainer = await UserStorage.GetTrackedUserFromGitHubUserId(Constants.MaintainerGitHubId);

            var card = TemplateCardHelper.GetPersonalIssueCard(issue, nearingOrExpiredMessage, expires, action, maintainer);

            var activity = MessageFactory.Attachment(card);
            activity.TeamsNotifyUser();

            var activityId = await SendProactiveNotificationToUserAsync(user, activity, cancellationToken);
            StoreIssueCardActivityId(activityId.Id, issue.Number, user.TeamsUserInfo.Id);
        }

        public async Task SendPRNotificationToUserAsync(TrackedUser user, PRCardTemplate prs, CancellationToken cancellationToken = default)
        {
            Logger.LogInformation($"Sending PR notification to {user.TeamsUserInfo.Name} with {prs.SinglePRs.Count} single and {prs.GroupPRs.Count} group");

            var maintainer = await UserStorage.GetTrackedUserFromGitHubUserId(Constants.MaintainerGitHubId);

            var card = TemplateCardHelper.GetPersonalPRCard(prs, maintainer);

            var activity = MessageFactory.Attachment(card);
            activity.TeamsNotifyUser();

            await SendProactiveNotificationToUserAsync(user, activity, cancellationToken);
        }

        public async Task CreatePersonalConversationAsync(ConversationReference conversationReference, BotCallbackHandler callback, CancellationToken cancellationToken)
        {
            Logger.LogInformation($"Creating personal conversation for {conversationReference.User.Name}");

            var serviceUrl = conversationReference.ServiceUrl;
            var credentials = new MicrosoftAppCredentials(Configuration["MicrosoftAppId"], Configuration["MicrosoftAppPassword"]);

            var conversationParameters = new ConversationParameters
            {
                IsGroup = false,
                Members = new ChannelAccount[]
                {
                        conversationReference.User
                },
                TenantId = conversationReference.Conversation.TenantId,
                Bot = conversationReference.Bot
            };

            AppCredentials.TrustServiceUrl(serviceUrl);

            await ((BotFrameworkAdapter)Adapter).CreateConversationAsync(
                Channels.Msteams,
                serviceUrl,
                credentials,
                conversationParameters,
                async (turnContext1, cancellationToken1) =>
                {
                    Logger.LogInformation($"Continuing conversation for {conversationReference.User.Name}");

                    var conversationReference2 = turnContext1.Activity.GetConversationReference();
                    conversationReference2.User = conversationReference.User;
                    await ((BotFrameworkAdapter)Adapter).ContinueConversationAsync(
                        Configuration["MicrosoftAppId"],
                        conversationReference2,
                        async (turnContext2, cancellationToken2) => await callback(turnContext2, cancellationToken2),
                        cancellationToken1);
                },
                cancellationToken);
        }

        private void StoreIssueCardActivityId(string activityId, int issueNumber, string teamsUserId)
        {
            Logger.LogInformation($"Storing IssueCard {issueNumber} for activityId {activityId}");
            var newMappedActivity = new MappedIssue(activityId, teamsUserId);
            IssueActivityMap.AddOrUpdate(issueNumber, newMappedActivity, (_, oldValue) =>
            {
                oldValue.Users[teamsUserId] = new MappedActivityUser(activityId);
                return oldValue;
            });
        }

        public MappedIssue GetMappedIssue(int issueNumber)
        {
            if (IssueActivityMap.TryGetValue(issueNumber, out MappedIssue mappedIssue))
            {
                return mappedIssue;
            }
            return null;
        }

        public MappedActivityUser GetMappedActivityFromIssueAndUser(int issueNumber, string teamsUserId)
        {
            var mappedIssue = GetMappedIssue(issueNumber);
            if (mappedIssue != null && mappedIssue.Users.TryGetValue(teamsUserId, out MappedActivityUser mappedUser))
            {
                return mappedUser;
            }
            return null;
        }

        public void HideActivity(int issueNumber, string teamsUserId)
        {
            if (IssueActivityMap.TryGetValue(issueNumber, out MappedIssue mappedActivity))
            {
                Logger.LogInformation($"Hiding issue {issueNumber} for user {teamsUserId}");

                if (mappedActivity.Users.ContainsKey(teamsUserId))
                {
                    mappedActivity.Users[teamsUserId].Hidden = true;
                }
            }
        }
    }

    public class MappedIssue
    {
        public readonly ConcurrentDictionary<string, MappedActivityUser> Users = new ConcurrentDictionary<string, MappedActivityUser>();

        public MappedIssue(string activityId, string teamsUserId)
        {
            Users[teamsUserId] = new MappedActivityUser(activityId);
        }
    }

    public class MappedActivityUser
    {
        public bool Hidden;
        public readonly string ActivityId;
        public readonly DateTime SentAt = DateTime.Now;

        public MappedActivityUser(string activityId)
        {
            ActivityId = activityId;
        }
    }
}
