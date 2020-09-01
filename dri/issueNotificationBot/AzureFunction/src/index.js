const { GitHubService } = require('./services');
const { GitHub } = require('./config');

module.exports = async function(context) {
  context.log('Starting Processes');
  const gitHub = new GitHubService(GitHub, context);
  await gitHub.processIssues();

  if (context.bindings.atEightAM) {
    await gitHub.processPRs();
  }

  context.log('Finished Processes');
};
