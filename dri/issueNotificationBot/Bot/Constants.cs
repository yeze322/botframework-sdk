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
        public const string MaintainerCommandPrefix = "command:";
        public const string EnableMaintainerNotificationsCommand = MaintainerCommandPrefix + "enableNotifications";
        public const string DisableMaintainerNotificationsCommand = MaintainerCommandPrefix + "disableNotifications";
        public const string MaintainerTestCards = MaintainerCommandPrefix + "sendCards";
        public const string MaintainerResendGreetings = MaintainerCommandPrefix + "resendGreetings";
        public const string UpdateUserNotificationSettings = MaintainerCommandPrefix + "updateNotificationSettings";

        public const string TestRepo = "testRepoForIssueNotificationBot";
    }
}
