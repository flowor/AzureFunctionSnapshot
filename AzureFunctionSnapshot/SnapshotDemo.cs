using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Playwright;
using System.Net;

namespace AzureFunctionSnapshot
{
    public class SnapshotDemo
    {
        private readonly ILogger _logger;

        public SnapshotDemo(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<SnapshotDemo>();
        }

        [Function(nameof(SnapshotDemo))]
        public async Task<HttpResponseData> TakeSnapshotAsync([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequestData req)
        {
            Environment.SetEnvironmentVariable("PLAYWRIGHT_BROWSERS_PATH", Environment.GetEnvironmentVariable("HOME_EXPANDED"));
            Microsoft.Playwright.Program.Main(new[] { "install", "chromium", "--with-deps" });

            var query = System.Web.HttpUtility.ParseQueryString(req.Url.Query);


            _logger.LogInformation($"Processing snapshot request. {query["url"]}");



            var url = string.IsNullOrWhiteSpace(query["url"]) ? "https://example.com" : query["url"];


            var response = req.CreateResponse(HttpStatusCode.OK);
            response.Headers.Add("Content-Type", "image/png");

            using var playwright = await Playwright.CreateAsync();
            await using var browser = await playwright.Chromium.LaunchAsync(new() { Headless = true });
            var page = await browser.NewPageAsync();

            await page.GotoAsync(url);

            response.WriteBytes(await page.ScreenshotAsync());

            return response;
        }
    }
}
