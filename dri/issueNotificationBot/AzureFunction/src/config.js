const globallyTrackedLabels = [
  'customer-reported'
]

const repositories = [
  { org: "Microsoft", repo: "botbuilder-azure", labels: globallyTrackedLabels },
  { org: "Microsoft", repo: "botbuilder-cognitiveservices", labels: globallyTrackedLabels },
  { org: "Microsoft", repo: "botbuilder-dotnet", labels: globallyTrackedLabels },
  { org: "Microsoft", repo: "botbuilder-java", labels: globallyTrackedLabels },
  { org: "Microsoft", repo: "botbuilder-js", labels: globallyTrackedLabels },
  { org: "Microsoft", repo: "botbuilder-python", labels: globallyTrackedLabels },
  { org: "Microsoft", repo: "botbuilder-samples", labels: globallyTrackedLabels },
  { org: "Microsoft", repo: "botbuilder-tools", labels: globallyTrackedLabels },
  { org: "Microsoft", repo: "botbuilder-v3", labels: globallyTrackedLabels },
  { org: "Microsoft", repo: "botframework-emulator", labels: globallyTrackedLabels },
  { org: "Microsoft", repo: "botframework-directlinejs", labels: globallyTrackedLabels },
  { org: "Microsoft", repo: "botframework-solutions", labels: globallyTrackedLabels },
  { org: "Microsoft", repo: "botframework-services", labels: globallyTrackedLabels },
  { org: "Microsoft", repo: "botframework-sdk", labels: globallyTrackedLabels },
  { org: "Microsoft", repo: "botframework-composer", labels: globallyTrackedLabels },
  { org: "Microsoft", repo: "botframework-cli", labels: globallyTrackedLabels },
  { org: "Microsoft", repo: "botframework-webchat", labels: globallyTrackedLabels },
  { org: "MicrosoftDocs", repo: "bot-docs", labels: globallyTrackedLabels },
]

if (process.env.UseTestRepo === 'true') repositories.push({ org: "mdrichardson", repo: "testRepoForIssueNotificationBot", labels: globallyTrackedLabels });

module.exports.GitHub = {
  repositories,
  source: 'GitHub'
}
