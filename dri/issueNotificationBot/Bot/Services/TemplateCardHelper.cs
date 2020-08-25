using AdaptiveCards;
using AdaptiveCards.Templating;
using IssueNotificationBot.Models;
using Microsoft.Bot.Schema;
using Microsoft.Rest.Serialization;
using Newtonsoft.Json;
using System;
using System.IO;

namespace IssueNotificationBot.Services
{
    public static class TemplateCardHelper
    {
        public static Attachment GetUserWelcomeCard(string avatar_url, string login, string name, TrackedUser maintainer)
        {
            // User may not have their name set in their profile
            name ??= "GitHub User";

            var paths = new[] { ".", "Resources", "userWelcomeCard.json" };
            var adaptiveCardJson = File.ReadAllText(Path.Combine(paths));

            var templateCard = new AdaptiveCardTemplate(adaptiveCardJson);

            var templateData = SafeJsonConvert.SerializeObject(new
            {
                avatar_url,
                login,
                name,
                maintainerName = maintainer?.TeamsUserInfo?.Name,
                maintainerEmail = maintainer?.TeamsUserInfo?.Email
            });
            var cardJson = templateCard.Expand(templateData);

            return new Attachment()
            {
                ContentType = AdaptiveCard.ContentType,
                Content = JsonConvert.DeserializeObject<AdaptiveCard>(cardJson),
            };
        }

        public static Attachment GetPersonalIssueCard(GitHubIssueNode issue, string nearingOrExpiredMessage, DateTime expires, string action, TrackedUser maintainer)
        {
            var paths = new[] { ".", "Resources", "personalIssueCard.json" };
            var adaptiveCardJson = File.ReadAllText(Path.Combine(paths));

            var templateCard = new AdaptiveCardTemplate(adaptiveCardJson);

            var templateData = SafeJsonConvert.SerializeObject(new
            {
                nearingOrExpiredMessage,
                issueTitle = issue.Title,
                issueCreated = issue.CreatedAt,
                issueExpires = expires,
                body = issue.Body,
                issueUrl = issue.Url,
                issueNumber = issue.Number,
                action,
                maintainerName = maintainer?.TeamsUserInfo?.Name,
                maintainerEmail = maintainer?.TeamsUserInfo?.Email
            });
            var cardJson = templateCard.Expand(templateData);

            return new Attachment()
            {
                ContentType = AdaptiveCard.ContentType,
                Content = JsonConvert.DeserializeObject<AdaptiveCard>(cardJson),
            };
        }
    }
}
