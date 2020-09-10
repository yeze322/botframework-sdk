// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using IssueNotificationBot.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;

namespace IssueNotificationBot.Services
{
    public class GitHubDataProcessor
    {
        internal readonly UserStorage UserStorage;
        internal Dictionary<string, TrackedUser> TrackedUsers = new Dictionary<string, TrackedUser>();
        internal readonly NotificationHelper NotificationHelper;
        internal readonly IConfiguration Configuration;
        internal readonly ILogger Logger;
        public GitHubDataProcessor(UserStorage userStorage, NotificationHelper notificationHelper, IConfiguration configuration, ILogger<GitHubIssueProcessor> logger)
        {
            UserStorage = userStorage;
            NotificationHelper = notificationHelper;
            Configuration = configuration;
            Logger = logger;
        }

        internal DateTime GetExpiration(GitHubPR pr, TimePeriodNotification timePeriod, DateTime now)
        {
            var adjustedExpiration = pr.CreatedAt.AddHours(timePeriod.ExpireHours);
            return GetExpiration(adjustedExpiration, pr.CreatedAt, timePeriod, now);
        }
        internal DateTime GetExpiration(GitHubIssue issue, TimePeriodNotification timePeriod, DateTime now)
        {
            var adjustedExpiration = issue.CreatedAt.AddHours(timePeriod.ExpireHours);
            return GetExpiration(adjustedExpiration, issue.CreatedAt, timePeriod, now);
        }
        private DateTime GetExpiration(DateTime adjustedExpiration, DateTime createdAt, TimePeriodNotification timePeriod, DateTime now)
        {
            // 30d and 90d periods always include weekends.
            if (timePeriod.ExpireHours != 72) return adjustedExpiration;

            // Adjust the expiration time to not include weekends.
            if (Constants.ExcludeWeekendsFromExpireHours)
            {
                var weekendDays = 0;
                for (DateTime date = createdAt; date.Date <= now.Date; date = date.AddDays(1))
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
    }
}
