using IssueNotificationBot.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace IssueNotificationBot.Services
{
    public class GitHubPRProcessor : GitHubDataProcessor
    {
        public GitHubPRProcessor(UserStorage userStorage, NotificationHelper notificationHelper, IConfiguration configuration, ILogger<GitHubIssueProcessor> logger)
            : base(userStorage, notificationHelper, configuration, logger)
        { }

        public async Task ProcessPRs(Dictionary<string, GitHubPRReviewer> prs)
        {
            Logger.LogInformation("Processing data");
            TrackedUsers = await UserStorage.GetGitHubUsers();

            await SendNotificationsForStalePRs(prs);
        }

        public async Task SendNotificationsForStalePRs(Dictionary<string, GitHubPRReviewer> prs)
        {
            var now = DateTime.Now;

            foreach(var user in TrackedUsers)
            {
                // If we're tracking the user and they have PRs they're a reviewer for
                if (prs.TryGetValue(user.Value.GitHubDetails.Login, out GitHubPRReviewer reviewer))
                {
                    var notificationSetting = user.Value.NotificationSettings.TimePeriodNotifications.Single(setting => setting.Name == "PR Notification");

                    if (!user.Value.NotificationSettings.AllEnabled || !notificationSetting.Enabled)
                    {
                        continue;
                    }

                    var cardTemplate = new PRCardTemplate
                    {
                        SinglePRs = new List<GitHubPRForCardTemplate>(),
                        GroupPRs = new List<GitHubPRForCardTemplate>(),
                        PRQueryUrl = $"https://github.com/search?q=is%3Aopen+review-requested%3A{user.Value.GitHubDetails.Login}+sort%3Acreated-asc+repo%3AMicrosoft%2Fbotbuilder-azure+repo%3AMicrosoft%2Fbotbuilder-cognitiveservices+repo%3AMicrosoft%2Fbotbuilder-dotnet+repo%3AMicrosoft%2Fbotbuilder-java+repo%3AMicrosoft%2Fbotbuilder-js+repo%3AMicrosoft%2Fbotbuilder-python+repo%3AMicrosoft%2Fbotbuilder-samples+repo%3AMicrosoft%2Fbotbuilder-tools+repo%3AMicrosoft%2Fbotbuilder-v3+repo%3AMicrosoft%2Fbotframework-emulator+repo%3AMicrosoft%2Fbotframework-directlinejs+repo%3AMicrosoft%2Fbotframework-solutions+repo%3AMicrosoft%2Fbotframework-services+repo%3AMicrosoft%2Fbotframework-sdk+repo%3AMicrosoft%2Fbotframework-composer+repo%3AMicrosoft%2Fbotframework-cli+repo%3AMicrosoft%2Fbotframework-webchat+repo%3AMicrosoftDocs%2Fbot-docs+is%3Apr&type=Issues"
                    };

                    // Highlight > 3 day old PRs in Adaptive Card
                    foreach(var pr in reviewer.Single.OrderBy(pr => pr.CreatedAt))
                    {
                        var expires = GetExpiration(pr, notificationSetting, now);
                        if (now > expires)
                        {
                            pr.Highlight = true;
                        }

                        var prTemplate = ConvertPRtoTemplate(pr);

                        cardTemplate.SinglePRs.Add(prTemplate);
                    }
                    foreach (var pr in reviewer.Group.OrderBy(pr => pr.CreatedAt))
                    {
                        var expires = GetExpiration(pr, notificationSetting, now);
                        if (now > expires)
                        {
                            pr.Highlight = true;
                        }

                        var prTemplate = ConvertPRtoTemplate(pr);

                        cardTemplate.GroupPRs.Add(prTemplate);
                    }

                    await NotificationHelper.SendPRNotificationToUserAsync(user.Value, cardTemplate);
                }
            }
        }

        private GitHubPRForCardTemplate ConvertPRtoTemplate(GitHubPR pr)
        {
            var group = !string.IsNullOrEmpty(pr.ReviewingForGroup) ? pr.ReviewingForGroup : "";
            const string groupReplacePattern = "(bf-)|(bb-)";
            group = Regex.Replace(group, groupReplacePattern, "", RegexOptions.IgnoreCase);

            var repo = pr.Repository.Name;
            const string repoReplacePattern = "(botframework-)|(botbuilder-)";
            repo = Regex.Replace(repo, repoReplacePattern, "", RegexOptions.IgnoreCase);

            return new GitHubPRForCardTemplate
            {
                CreatedAt = pr.CreatedAt,
                Group = group,
                Highlight = pr.Highlight,
                Repository = repo,
                Title = pr.Title,
                Url = pr.Url.ToString()
            };
        }
    }
}
