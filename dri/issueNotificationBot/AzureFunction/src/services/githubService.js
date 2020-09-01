const axios = require('axios');
const BotService = require('./botService');
const { getIssueQuery, groupIssues } = require('../helpers/issueQueryHelper');
const { getPRQuery, groupPRs } = require('../helpers/prQueryHelper');
const { flattenNodesAndEdges } = require('../helpers/universalQueryHelpers');

module.exports = class GitHubService {
  constructor({ repositories, source }, context) {
    this.repositories = repositories;
    this.source = source;
    this.context = context;

    this.gitHubQueryConfig = {
      method: 'POST',
      url: 'https://api.github.com/graphql',
      headers: { Authorization: `Bearer ${ process.env.GitHubToken }`, 'Content-Type': 'application/json' }
    };

    this.bot = new BotService(this.context);
  }

  async processIssues() {
    try {
      let issues = await Promise.all(this.repositories.map(async repository => await this.getIssues(repository)));
      issues = issues.filter(issueArray => issueArray.length > 0).flat().flat();

      const groupedIssues = groupIssues(issues, this.context);
      await this.bot.sendIssues(groupedIssues);
    } catch ({ message }) {
      this.context.error(message);
    }
  }

  async getIssues({ org, repo, labels, ignoreLabels = [] }) {
    if (labels) {
      const issues = await labels.reduce(async (result, label) => {
        const query = getIssueQuery(`repo:${ org }/${ repo } is:open is:issue label:${ label } ${ ignoreLabels.map(ignore => `-label:'${ ignore }'`).join(' ') }`);
        const response = await axios({ ...this.gitHubQueryConfig, data: query });
        if (response.data && response.data.errors && response.data.errors.length > 0) {
          this.context.error(JSON.stringify(response.data.errors, null, 2));
          return;
        }
        const { data: { data: { search: { edges: issues } } } } = response;
        if (issues.length > 0) {
          result.push(issues);
        }

        return result;
      }, []);
      return issues;
    } else {
      const query = getIssueQuery(`repo:${ org }/${ repo } is:issue ${ ignoreLabels.map(ignore => `-label:${ ignore }`).join(' ') }`);
      const { data: { data: { search: { edges: issues } } } } = await axios({ ...this.gitHubQueryConfig, data: query });
      return issues;
    }
  }

  async processPRs() {
    try {
      let prs = await Promise.all(this.repositories.map(async repository => await this.getPRs(repository)));
      prs = prs.filter(prArray => prArray.length > 0).flat().flat();

      const groupedPRs = groupPRs(prs, this.context);

      await this.bot.sendPRs(groupedPRs);
    } catch ({ message }) {
      this.context.error(message);
    }
  }

  async getPRs({ org, repo, ignoreLabels = [] }) {
    const query = getPRQuery(`repo:${ org }/${ repo } is:pr is:open ${ ignoreLabels.map(ignore => `-label:${ ignore }`).join(' ') }`);
    const { data: { data: { search: { edges: prs } } } } = await axios({ ...this.gitHubQueryConfig, data: query });
    return flattenNodesAndEdges(prs);
  }
};
