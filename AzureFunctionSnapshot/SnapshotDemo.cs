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

            _logger.LogInformation("Processing snapshot request.");

            var response = req.CreateResponse(HttpStatusCode.OK);
            response.Headers.Add("Content-Type", "image/png");

            using var playwright = await Playwright.CreateAsync();
            await using var browser = await playwright.Chromium.LaunchAsync(new() { Headless = true });
            var page = await browser.NewPageAsync();

            await page.GotoAsync("https://example.com");

            response.WriteBytes(await page.ScreenshotAsync());

            return response;
        }
    }
}
