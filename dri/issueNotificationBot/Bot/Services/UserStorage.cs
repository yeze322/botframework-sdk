using IssueNotificationBot.Models;
using Microsoft.Azure.Cosmos;
using Microsoft.Bot.Builder;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace IssueNotificationBot.Services
{
    public class UserStorage
    {
        protected readonly IStorage Db;
        protected readonly ILogger Logger;

        public UserStorage(IStorage db, ILogger<UserStorage> logger)
        {
            Db = db;
            Logger = logger;
        }

        public async Task AddGitHubUser(TrackedUser user)
        {
            Logger.LogInformation($"Storing GitHub User: {user.GitHubDetails.Name}");

            var users = await GetGitHubUsers();
            users.TryAdd(user.GitHubDetails.Login, user);
            await OverwriteGitHubUsersDatabase(users);
        }

        public async Task RemoveGitHubUser(string gitHubUserLogin)
        {
            Logger.LogInformation($"Removing GitHub User: {gitHubUserLogin}");

            var users = await GetGitHubUsers();
            users.Remove(gitHubUserLogin);
            await OverwriteGitHubUsersDatabase(users);
        }

        public async Task<Dictionary<string, TrackedUser>> GetGitHubUsers()
        {
            return await GetUsersDb<TrackedUser>(Constants.GitHubUserStorageKey);
        }

        public async Task RemoveUser(string teamsUserId)
        {
            var userMap = await GetTeamsUserToGitHubMap(teamsUserId);

            await RemoveFromTeamsUserToGitHubUserMap(userMap);
            await RemoveGitHubUser(userMap.GitHubUserLogin);
        }

        public async Task AddTeamsUserToGitHubUserMap(TeamsUserToGitHubMap user)
        {
            Logger.LogInformation($"Adding Teams User: {user.TeamsUserId}/{user.GitHubUserLogin} to GitHubUsersMap");

            var users = await GetTeamsUsers();
            users.TryAdd(user.TeamsUserId, user);
            await OverwriteTeamsUsersDatabase(users);
        }

        public async Task RemoveFromTeamsUserToGitHubUserMap(TeamsUserToGitHubMap user)
        {
            Logger.LogInformation($"Removing Teams User: {user.TeamsUserId}/{user.GitHubUserLogin} to GitHubUsersMap");

            var users = await GetTeamsUsers();
            users.Remove(user.TeamsUserId);
            await OverwriteTeamsUsersDatabase(users);
        }

        public async Task<TeamsUserToGitHubMap> GetTeamsUserToGitHubMap(string teamsUserId)
        {
            var teamsUsers = await GetTeamsUsers();
            if (teamsUsers.TryGetValue(teamsUserId, out TeamsUserToGitHubMap user))
            {
                return user;
            }
            return null;
        }

        public async Task<Dictionary<string, TeamsUserToGitHubMap>> GetTeamsUsers()
        {
            return await GetUsersDb<TeamsUserToGitHubMap>(Constants.TeamsIdToGitHubUserMapStorageKey);
        }

        public async Task<bool> HaveUserDetails(string teamsUserId)
        {
            var teamsUsers = await GetTeamsUsers();
            if (!teamsUsers.TryGetValue(teamsUserId, out TeamsUserToGitHubMap user))
            {
                return false;
            }
            var gitHubUsers = await GetGitHubUsers();
            return gitHubUsers.ContainsKey(user.GitHubUserLogin);
        }

        public async Task<TrackedUser> GetTrackedUserFromGitHubUserId(string gitHubUserId)
        {
            var users = await GetGitHubUsers();
            if (users.TryGetValue(gitHubUserId, out TrackedUser trackedUser))
            {
                return trackedUser;
            }
            return null;
        }

        public async Task<TrackedUser> GetTrackedUserFromTeamsUserId(string teamsUserId)
        {
            var gitHubUserId = (await GetTeamsUserToGitHubMap(teamsUserId))?.GitHubUserLogin;
            return await GetTrackedUserFromGitHubUserId(gitHubUserId);
        }

        public async Task OverwriteNotificationSettingsForAllUsers(NotificationSettings toOverwrite = null)
        {
            toOverwrite ??= new NotificationSettings();

            var users = await GetGitHubUsers();

            foreach(var user in users)
            {
                user.Value.NotificationSettings = toOverwrite;
            }

            await OverwriteGitHubUsersDatabase(users);
        }

        private async Task OverwriteGitHubUsersDatabase(object users)
        {
            await Db.WriteAsync(new Dictionary<string, object>() { { Constants.GitHubUserStorageKey, users } });
        }

        private async Task OverwriteTeamsUsersDatabase(object users)
        {
            await Db.WriteAsync(new Dictionary<string, object>() { { Constants.TeamsIdToGitHubUserMapStorageKey, users } });
        }

        private async Task<Dictionary<string, T>> GetUsersDb<T>(string key)
        {
            try
            {
                var document = await Db.ReadAsync<Dictionary<string, T>>(new string[] { key });
                return document.TryGetValue(key, out Dictionary<string, T> users) ? users : new Dictionary<string, T>();
            }
            catch (CosmosException e) when (e.StatusCode == HttpStatusCode.NotFound)
            {
                // NotFound *should* only indicate that the Container hasn't been created yet.
                Logger.LogWarning($"404 when reading Cosmos Container: {key} DB.");
                return new Dictionary<string, T>();
            }
        }
    }
}
