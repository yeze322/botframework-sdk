// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Newtonsoft.Json;
using System;

namespace IssueNotificationBot.Models
{
    public class GitHubServiceData
    {
        [JsonProperty(PropertyName = "reportedNotReplied")]
        public readonly GitHubIssueNode[] ReportedNotReplied;
        [JsonProperty(PropertyName = "reportedAndReplied")]
        public readonly GitHubIssueNode[] ReportedAndReplied;
        [JsonProperty(PropertyName = "reportedNotRepliedNoAssignee")]
        public readonly GitHubIssueNode[] ReportedNotRepliedNoAssignee;
        [JsonProperty(PropertyName = "reportedAndRepliedNoAssignee")]
        public readonly GitHubIssueNode[] ReportedAndRepliedNoAssignee;
    }

    public class GitHubIssueNode
    {
        [JsonProperty(PropertyName = "node")]
        private readonly GitHubIssue Node;
        public int Number { get { return Node.Number; } }
        public GitHubRepository Repository { get { return Node.Repository; } }
        public string Title { get { return Node.Title; } }
        public GitHubLabel[] Labels { get { return Node.Labels.Nodes; } }
        public GitHubAssignee[] Assignees { get { return Node.Assignees.Nodes; } }
        public GitHubAuthor Author { get { return Node.Author; } }
        public string Body { get { return Node.Body; } }
        public DateTime CreatedAt { get { return Node.CreatedAt; } }
        public Uri Url { get { return Node.Url; } }
    }

    public class GitHubIssue
    {
        [JsonProperty(PropertyName = "number")]
        public readonly int Number;
        [JsonProperty(PropertyName = "repository")]
        public readonly GitHubRepository Repository;
        [JsonProperty(PropertyName = "title")]
        public readonly string Title;
        [JsonProperty(PropertyName = "labels")]
        public readonly GitHubLabelsNodes Labels;
        [JsonProperty(PropertyName = "assignees")]
        public readonly GitHubAssigneesNodes Assignees;
        [JsonProperty(PropertyName = "author")]
        public readonly GitHubAuthor Author;
        [JsonProperty(PropertyName = "body")]
        public readonly string Body;
        [JsonProperty(PropertyName = "createdAt")]
        public readonly DateTime CreatedAt;
        [JsonProperty(PropertyName = "url")]
        public readonly Uri Url;
    }

    public class GitHubRepository
    {
        [JsonProperty(PropertyName = "name")]
        public readonly string Name;
        [JsonProperty(PropertyName = "url")]
        public readonly Uri Url;
    }

    public class GitHubLabelsNodes
    {
        [JsonProperty(PropertyName = "nodes")]
        public readonly GitHubLabel[] Nodes;
    }

    public class GitHubLabel
    {
        [JsonProperty(PropertyName = "name")]
        public readonly string Name;
        [JsonProperty(PropertyName = "updatedAt")]
        public readonly DateTime UpdatedAt;
        [JsonProperty(PropertyName = "url")]
        public readonly Uri Url;
    }

    public class GitHubAssigneesNodes
    {
        [JsonProperty(PropertyName = "nodes")]
        public readonly GitHubAssignee[] Nodes;
    }

    public class GitHubAssignee
    {
        [JsonProperty(PropertyName = "login")]
        public readonly string Login;
        [JsonProperty(PropertyName = "name")]
        public readonly string Name;
        [JsonProperty(PropertyName = "url")]
        public readonly Uri Url;
    }

    public class GitHubAuthor
    {
        [JsonProperty(PropertyName = "login")]
        public readonly string Login;
        [JsonProperty(PropertyName = "url")]
        public readonly Uri Url;
    }
}
