using Microsoft.Playwright;
using Newtonsoft.Json;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public class LambdaTestSetup
{
    protected IPlaywright playwright;
    protected IBrowser browser;
    protected IBrowserContext context;
    protected IPage page;

    [SetUp]
    public async Task Setup()
    {
        playwright = await Playwright.CreateAsync();

        string user = Environment.GetEnvironmentVariable("LT_USERNAME") ?? "YOUR_LAMBDATEST_USERNAME";
        string accessKey = Environment.GetEnvironmentVariable("LT_ACCESS_KEY") ?? "YOUR_LAMBDATEST_ACCESS_KEY";

        string testName = TestContext.CurrentContext.Test.MethodName;
        string buildName = Environment.GetEnvironmentVariable("LT_BUILD_NAME") ?? "YOUR_LAMBDATEST_ACCESS_KEY";

        var capabilities = new Dictionary<string, object?>
        {
            { "browserName", "pw-chromium" },
            { "browserVersion", "latest" },
            { "LT:Options", new Dictionary<string, string?>
                {
                    { "name", testName },
                    { "build", buildName },
                    { "platform", "Windows 11" },
                    { "user", user },
                    { "accessKey", accessKey }
                }
            }
        };

        string capabilitiesJson = JsonConvert.SerializeObject(capabilities);
        string cdpUrl = "wss://cdp.lambdatest.com/playwright?capabilities=" + Uri.EscapeDataString(capabilitiesJson);

        browser = await playwright.Chromium.ConnectAsync(cdpUrl);  // Assigning to class variable

        context = await browser.NewContextAsync();  // Creating browser context
        page = await context.NewPageAsync();  // Correctly creating a new page
    }

    [TearDown]
    public async Task Teardown()
    {
        await page.CloseAsync();
        await context.CloseAsync();
        await browser.CloseAsync();
        playwright.Dispose();
    }
}
