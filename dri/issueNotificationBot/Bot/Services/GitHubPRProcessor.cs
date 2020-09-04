using IssueNotificationBot.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
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
            var expirePeriod = new TimePeriodNotification
                (
                    72,
                    "PR Notification",
                    0,
                    23
                );

            var now = DateTime.Now;

            foreach(var user in TrackedUsers)
            {
                // If we're tracking the user and they have PRs they're a reviewer for
                if (prs.TryGetValue(user.Value.GitHubDetails.Login, out GitHubPRReviewer reviewer))
                {
                    var tempCardObject = new
                    {
                        SinglePRs = from createdAt in reviewer.Single
                                    orderby createdAt descending
                                    select createdAt,
                        GroupPRs = from createdAt in reviewer.Group
                                   orderby createdAt descending
                                   select createdAt
                    };

                    var cardTemplate = new PRCardTemplate();

                    // Highlight > 3 day old PRs in Adaptive Card
                    foreach(var pr in tempCardObject.SinglePRs)
                    {
                        var expires = GetExpiration(pr, expirePeriod, now);
                        if (now > expires)
                        {
                            pr.Highlight = true;
                        }

                        var prTemplate = ConvertPRtoTemplate(pr);

                        cardTemplate.SinglePRs.Append(prTemplate);
                    }
                    foreach (var pr in tempCardObject.GroupPRs)
                    {
                        var expires = GetExpiration(pr, expirePeriod, now);
                        if (now > expires)
                        {
                            pr.Highlight = true;
                        }

                        var prTemplate = ConvertPRtoTemplate(pr);

                        cardTemplate.GroupPRs.Append(prTemplate);
                    }

                    await NotificationHelper.SendPRNotificationToUserAsync(user.Value, cardTemplate);
                }
            }
        }

        private GitHubPRForCardTemplate ConvertPRtoTemplate(GitHubPR pr)
        {
            return new GitHubPRForCardTemplate
            {
                CreatedAt = pr.CreatedAt,
                Group = !string.IsNullOrEmpty(pr.ReviewingForGroup) ? pr.ReviewingForGroup : "",
                Highlight = pr.Highlight,
                Respository = pr.Repository.Name,
                Title = pr.Title,
                Url = pr.Url.ToString()
            };
        }
    }
}
