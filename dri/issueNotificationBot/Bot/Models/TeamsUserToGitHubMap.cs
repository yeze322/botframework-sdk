// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

namespace IssueNotificationBot.Models
{
    public class TeamsUserToGitHubMap
    {
        public readonly string TeamsUserId;
        public readonly string GitHubUserLogin;

        public TeamsUserToGitHubMap(string teamsUserId, string gitHubUserLogin)
        {
            TeamsUserId = teamsUserId;
            GitHubUserLogin = gitHubUserLogin;
        }
    }
}
