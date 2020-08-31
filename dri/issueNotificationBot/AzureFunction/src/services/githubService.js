const axios = require('axios');
const BotService = require('./botService');

module.exports = class GitHubService {

    constructor({ repositories, source }, context) {
        this.repositories = repositories;
        this.source = source;
        this.context = context;
    }

    async process() {
        try {
            let issues = await Promise.all(this.repositories.map(async repository => await this.getIssues(repository)));
            issues = issues.filter(issueArray => issueArray.length > 0).flat().flat();
            
            const groupedIssues = this.groupIssues(issues);
            
            const bot = new BotService(this.context);
            await bot.sendData(groupedIssues);

        } catch ({ message }) {
            this.context.error(message)
        }
    }

    async getIssues({ org, repo, labels, ignoreLabels = [] }) {

        const config = {
            method: 'POST',
            url: 'https://api.github.com/graphql',
            headers: { Authorization: `Bearer ${process.env.GitHubToken}`, 'Content-Type': 'application/json'}
        };

        if (!!labels) {
            const issues = await labels.reduce(async (result, label) => {
                const query = this.getQuery(`repo:${org}/${repo} is:open is:issue label:${label} ${ignoreLabels.map(ignore => `-label:'${ignore}'`).join(' ')}`);
                const response = await axios({...config, data: query });
                if (response.data && response.data.errors && response.data.errors.length > 0) {
                    this.context.error(JSON.stringify(response.data.errors, null, 2));
                    return;
                }
                const { data: { data: { search: { edges: issues }}}} = response;
                if (issues.length > 0) {
                    result.push(issues);
                }

                return result;
            }, []);
            return issues;
        } else {
            const query = this.getQuery(`repo:${org}/${repo} is:issue ${ignoreLabels.map(ignore => `-label:${ignore}`).join(' ')}`);
            const { data: { data: { search: { edges: issues }}}} = await axios({...config, data: query });
            return issues;
        }
    }

    // Go here for help creating/editing query: https://developer.github.com/v4/explorer/
    getQuery(search) {
        return { 
            query: 
            `{
                search(query: "${search}", type: ISSUE, first: 100) {
                  edges {
                    node {
                      ... on Issue {
                        number
                        repository {
                          name
                          url
                        }
                        title
                        labels(first: 100) {
                          nodes {
                            name
                            updatedAt
                            url
                          }
                        }
                        assignees(first: 100) {
                          nodes {
                            login
                            name
                            url
                          }
                        }
                        author {
                          login
                            url
                        }
                        body
                        createdAt,
                        url
                      }
                    }
                  }
                }
              }`
        }
    }

    groupIssues(issues) {
        const reportedNotReplied = [];
        const reportedAndReplied = [];
        const reportedNotRepliedNoAssignee = [];
        const reportedAndRepliedNoAssignee = [];

        for (const issue of issues) {
            if (this.issueReportedNotReplied(issue)) {
                reportedNotReplied.push(issue);
            } else if (this.issueReportedAndReplied(issue)) {
                reportedAndReplied.push(issue);
            } else if (this.issueReportedNotRepliedNoAssignee(issue)) {
                reportedNotRepliedNoAssignee.push(issue);
            } else if (this.issueReportedAndRepliedNoAssignee(issue)) {
                reportedAndRepliedNoAssignee.push(issue);
            } else {
                this.context.error(`Unable to group issue:\n${ JSON.stringify(issue, null, 2) }`);
            };
        }

        return {
            reportedNotReplied,
            reportedAndReplied,
            reportedNotRepliedNoAssignee,
            reportedAndRepliedNoAssignee
        }
    }

    issueReportedNotReplied(issue) {
        try {
            const labels = issue.node.labels.nodes;
            const labelValues = labels.map(label => label.name.toLowerCase());
            const assignees = issue.node.assignees.nodes;
            return labelValues.includes('customer-reported') && !labelValues.includes('customer-replied-to') && assignees.length > 0;
        } catch (err) {
            this.context.error(err);
        }
    }

    issueReportedAndReplied(issue) {
        try {
            const labels = issue.node.labels.nodes;
            const labelValues = labels.map(label => label.name.toLowerCase());
            const assignees = issue.node.assignees.nodes;
            return labelValues.includes('customer-reported') && labelValues.includes('customer-replied-to') && assignees.length > 0;
        } catch (err) {
            this.context.error(err);
        }
    }

    issueReportedNotRepliedNoAssignee(issue) {
        try {
            const labels = issue.node.labels.nodes;
            const labelValues = labels.map(label => label.name.toLowerCase());
            const assignees = issue.node.assignees.nodes;
            return labelValues.includes('customer-reported') && !labelValues.includes('customer-replied-to') && assignees.length === 0;
        } catch (err) {
            this.context.error(err);
        }
    }

    issueReportedAndRepliedNoAssignee(issue) {
        try {
            const labels = issue.node.labels.nodes;
            const labelValues = labels.map(label => label.name.toLowerCase());
            const assignees = issue.node.assignees.nodes;
            return labelValues.includes('customer-reported') && labelValues.includes('customer-replied-to') && assignees.length === 0;
        } catch (err) {
            this.context.error(err);
        }
    }
}
