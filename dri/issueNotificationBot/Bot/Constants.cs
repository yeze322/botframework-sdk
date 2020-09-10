// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

namespace IssueNotificationBot
{
    public static class Constants
    {
        public const string MaintainerGitHubId = "mdrichardson";

        public const string GitHubUserStorageKey = "GitHubUsers";
        public const string TeamsIdToGitHubUserMapStorageKey = "TeamsIdToGitHubUser";

        public const string GitHubApiBaseUrl = "https://api.github.com";
        public const string GitHubApiAuthenticatedUserPath = "/user";
        public const string UserAgent = "IssueNotificationBot";

        public const int IssueNeedsResolutionHours = 24 * 30;
        public const bool ExcludeWeekendsFromExpireHours = true;

        public const string PassedExpirationMessage = "has passed";
        public const string NearingExpirationMessage = "is nearing";

        public const string NoActivityId = "NoActivityId";
        public const string PersonalConversationType = "personal";

        public const string HideIssueNotificationAction = "hideIssueNotification";

        public const string NoConversationResponse = "Sorry. I'm mostly a notification-only bot don't know how to respond to this.";
        public const string NoOAuthInGroupConversationsResponse = "Please sign in via the 1:1 conversation that I sent you previously. If you need me to send it again, please type \"login\" **in our 1:1 conversation**";

        public const string LoginCommand = "login";

        public const string TestRepo = "testRepoForIssueNotificationBot";
    }

    static public class MaintainerCommands
    {
        public const string CommandPrefix = "command:";
        public const string ShowCommands = CommandPrefix + "showCommands";

        public const string EnableMaintainerNotifications = CommandPrefix + "enableNotifications";
        public const string DisableMaintainerNotifications = CommandPrefix + "disableNotifications";

        public const string TestCards = CommandPrefix + "testCards";

        public const string ResendGrettings = CommandPrefix + "resendGreetings";

        public const string UpdateUserNotificationSettings = CommandPrefix + "updateNotificationSettings";

        public const string SetBroadcastMessage = CommandPrefix + "setBroadcastMessage";
        public const string ViewBroadcastMessage = CommandPrefix + "viewBroadcastMessage";
        public const string SendBroadcastMessage = CommandPrefix + "sendBroadcastMessage";
    }
}
