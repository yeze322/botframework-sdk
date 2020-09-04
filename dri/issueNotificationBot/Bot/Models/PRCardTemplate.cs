using Newtonsoft.Json;
using System;

#nullable enable
namespace IssueNotificationBot.Models
{
    public class PRCardTemplate
    {
        [JsonProperty(PropertyName = "SinglePRs")]
        public GitHubPRForCardTemplate[] SinglePRs;
        [JsonProperty(PropertyName = "GroupPRs")]
        public GitHubPRForCardTemplate[] GroupPRs;
    }

    public class GitHubPRForCardTemplate
    {
        [JsonProperty(PropertyName = "title")]
        public string Title;
        [JsonProperty(PropertyName = "repository")]
        public string? Respository;
        [JsonProperty(PropertyName = "createdAt")]
        public DateTime CreatedAt;
        [JsonProperty(PropertyName = "url")]
        public string Url;
        [JsonProperty(PropertyName = "highlight")]
        public bool Highlight;
        [JsonProperty(PropertyName = "group")]
        public string Group;
    }
}
