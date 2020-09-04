// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.Bot.Schema;
using Microsoft.Bot.Schema.Teams;

namespace IssueNotificationBot.Models
{
    public class TrackedUser
    {
        public NotificationSettings NotificationSettings = new NotificationSettings();
        public TeamsChannelAccount TeamsUserInfo;
        public GitHubUserResponse GitHubDetails;
        public ConversationReference ConversationReference;

        public TrackedUser(GitHubUserResponse gitHubUserResponse)
        {
            GitHubDetails = gitHubUserResponse;
        }
    }

    public class NotificationSettings
    {
        public bool Enabled = true;
        public TimePeriodNotification[] TimePeriodNotifications =
        {
            new TimePeriodNotification(
                24 * 3,
                "72h",
                24,
                6
                ),
            new TimePeriodNotification(
                24 * 30,
                "30d",
                24 * 3,
                24
                ),
            new TimePeriodNotification(
                24 * 90,
                "90d",
                24 * 7,
                24 * 3
                )
        };
    }

    public class TimePeriodNotification
    {
        public string Name;
        public bool Enabled = true;
        public int ExpireHours;
        public int NotifyPriorToExpiryHours;
        public int NotificationFrequency;

        public TimePeriodNotification(int _expireHours, string name, int notifyPriorToExpiryHours, int notificationFrequencyHours, bool enabled = true)
        {
            ExpireHours = _expireHours;
            Name = name;
            NotifyPriorToExpiryHours = notifyPriorToExpiryHours;
            NotificationFrequency = notificationFrequencyHours;
            Enabled = enabled;
        }
    }
}
