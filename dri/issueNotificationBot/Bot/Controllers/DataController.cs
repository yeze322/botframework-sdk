// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using IssueNotificationBot.Models;
using IssueNotificationBot.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Bot.Connector;
using Microsoft.Bot.Connector.Authentication;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using Microsoft.Rest.Serialization;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Threading.Tasks;

namespace IssueNotificationBot
{
    // This ASP Controller handles data sent from the Azure Function that polls the GitHub issues.
    // It requires that the Azure Function uses the bot AppId and AppPassword
    [Route("api/data")]
    [ApiController]
    public class DataController : ControllerBase
    {
        private readonly IConfiguration Configuration;
        private readonly GitHubDataProcessor GitHubDataProcessor;
        private readonly ILogger Logger;

        public DataController(IConfiguration configuration, GitHubDataProcessor gitHubDataProcessor, ILogger<DataController> logger)
        {
            Configuration = configuration;
            GitHubDataProcessor = gitHubDataProcessor;
            Logger = logger;
        }

        [HttpPost]
        public async Task<HttpStatusCode> PostAsync()
        {
            Logger.LogInformation("Received post on /api/data");
            using var reader = new System.IO.StreamReader(Request.Body);
            var json = await reader.ReadToEndAsync().ConfigureAwait(true);

            try
            {
                var gitHubData = SafeJsonConvert.DeserializeObject<GitHubServiceData>(json);

                var credentials = new SimpleCredentialProvider(Configuration["MicrosoftAppId"], Configuration["MicrosoftAppPassword"]);
                Request.Headers.TryGetValue("Authorization", out StringValues authHeader);
                var result = await JwtTokenValidation.ValidateAuthHeader(authHeader, credentials, new SimpleChannelProvider(), Channels.Directline);

                if (result.IsAuthenticated)
                {
                    await GitHubDataProcessor.ProcessData(gitHubData);
                    Logger.LogInformation("Finished processing post on /api/data");
                    return HttpStatusCode.OK;
                }

                return HttpStatusCode.Forbidden;
            }
            catch (JsonException)
            {
                Logger.LogError("Received invalid data on /api/data");
            }
            catch (Exception e)
            {
                Logger.LogError($"Something went wrong in /api/data controller: {e.Message}");
            }
            return HttpStatusCode.BadRequest;
        }
    }
}
