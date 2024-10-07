using Microsoft.Extensions.Hosting;

Environment.SetEnvironmentVariable("PLAYWRIGHT_BROWSERS_PATH", Environment.GetEnvironmentVariable("HOME_EXPANDED"));
Microsoft.Playwright.Program.Main(new[] { "install", "chromium", "--with-deps" });

var host = new HostBuilder()
    .ConfigureFunctionsWorkerDefaults()
    .Build();

host.Run();
