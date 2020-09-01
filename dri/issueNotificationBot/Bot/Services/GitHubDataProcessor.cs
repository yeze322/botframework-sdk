// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using IssueNotificationBot.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IssueNotificationBot.Services
{
    public class GitHubDataProcessor
    {
        private readonly UserStorage UserStorage;
        private Dictionary<string, TrackedUser> TrackedUsers;
        private readonly NotificationHelper NotificationHelper;
        private readonly IConfiguration Configuration;
        private readonly ILogger Logger;
        public GitHubDataProcessor(UserStorage userStorage, NotificationHelper notificationHelper, IConfiguration configuration, ILogger<GitHubDataProcessor> logger)
        {
            UserStorage = userStorage;
            NotificationHelper = notificationHelper;
            Configuration = configuration;
            Logger = logger;
        }

        public async Task ProcessData(GitHubIssues data)
        {
            Logger.LogInformation("Processing data");
            TrackedUsers = await UserStorage.GetGitHubUsers();

            await ProcessReportedNotReplied(data.ReportedNotReplied);
        }

        private async Task ProcessReportedNotReplied(GitHubIssue[] issues)
        {
            Logger.LogInformation("Processing ProcessReportedNotReplied");
            foreach (var issue in issues)
            {
                foreach (var assignee in GetAssigneeUserData(issue))
                {
                    await NotifyAssigneeAsNecessary(assignee, issue);
                }
            }
        }

        private List<TrackedUser> GetAssigneeUserData(GitHubIssue issue)
        {
            var users = new List<TrackedUser>();
            foreach (var assignee in issue.Assignees)
            {
                if (TrackedUsers.TryGetValue(assignee.Login, out TrackedUser user))
                {
                    users.Add(user);
                }
            }
            return users;
        }

        private async Task NotifyAssigneeAsNecessary(TrackedUser user, GitHubIssue issue)
        {
            var now = DateTime.UtcNow;
            if (user.NotificationSettings.Enabled)
            {
                // Check each time period from largest to smallest
                foreach (TimePeriodNotification timePeriod in user.NotificationSettings.TimePeriodNotifications.OrderByDescending(item => item.ExpireHours).ToList())
                {
                    // Stop checking if we've already sent the notification
                    if (NotificationHelper.IssueActivityMap.TryGetValue(issue.Number, out MappedIssue mappedActivity) && mappedActivity.Users.TryGetValue(user.TeamsUserInfo.Id, out MappedActivityUser mappedUser))
                    {
                        return;
                    }

                    // Adjust the message we send to the user
                    var action = "Respond";
                    if (IssueExpiredNeedsResolve(timePeriod))
                    {
                        action = "Resolve";
                    }

                    string nearingOrExpiredMessage = null;
                    var expires = GetExpiration(issue, timePeriod, now);

                    // TESTING ONLY: This allows us to set up a separate repository and some issues to test that the bot works.
                    // Here, we can manually set the issue created time to mock an expired issue.
                    if (Configuration?["EnableTestMode"] == "true" && issue.Repository.Name == Constants.TestRepo)
                    {
                        expires = new DateTime(2020, 1, 1);
                    }

                    if (IssueExpiredNeedsResponse(expires, now))
                    {
                        nearingOrExpiredMessage = Constants.PassedExpirationMessage;
                    }
                    else if (IssueNearingExpirationNeedsResponse(timePeriod, expires, now))
                    {
                        nearingOrExpiredMessage = Constants.NearingExpirationMessage;
                    }

                    if (!string.IsNullOrEmpty(nearingOrExpiredMessage) && !UserNotifiedWithinWindow(timePeriod, now, issue, user.TeamsUserInfo.Id))
                    {
                        await NotificationHelper.SendIssueNotificationToUserAsync(user, issue, nearingOrExpiredMessage, expires, action);
                    }
                }
            }
        }

        private DateTime GetExpiration(GitHubIssue issue, TimePeriodNotification timePeriod, DateTime now)
        {
            var adjustedExpiration = issue.CreatedAt.AddHours(timePeriod.ExpireHours);

            // 30d and 90d periods always include weekends.
            if (timePeriod.ExpireHours != 72) return adjustedExpiration;

            // Adjust the expiration time to not include weekends.
            if (Constants.ExcludeWeekendsFromExpireHours)
            {
                var weekendDays = 0;
                for (DateTime date = issue.CreatedAt; date.Date <= now.Date; date = date.AddDays(1))
                {
                    if ((date.DayOfWeek == DayOfWeek.Saturday) || (date.DayOfWeek == DayOfWeek.Sunday))
                    {
                        weekendDays++;
                    }
                }

                adjustedExpiration.AddDays(weekendDays);
            }

            return adjustedExpiration;
        }

        private bool IssueExpiredNeedsResolve(TimePeriodNotification timePeriod)
        {
            return timePeriod.ExpireHours > Constants.IssueNeedsResolutionHours;
        }

        private bool IssueExpiredNeedsResponse(DateTime expires, DateTime now)
        {
            return now >= expires;
        }

        private bool IssueNearingExpirationNeedsResponse(TimePeriodNotification timePeriod, DateTime expires, DateTime now)
        {
            return now >= expires.AddHours(-timePeriod.NotifyPriorToExpiryHours);
        }

        private bool UserNotifiedWithinWindow(TimePeriodNotification timePeriod, DateTime now, GitHubIssue issue, string teamsUserId)
        {
            var mappedActivity = NotificationHelper.GetMappedIssue(issue.Number);
            if (mappedActivity != null && mappedActivity.Users.TryGetValue(teamsUserId, out MappedActivityUser mappedUser))
            {
                return mappedUser.SentAt.AddHours(timePeriod.NotificationFrequency) <= now;
            }
            return false;
        }
    }
}
