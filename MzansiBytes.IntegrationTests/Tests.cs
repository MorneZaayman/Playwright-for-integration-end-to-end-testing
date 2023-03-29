using Microsoft.Playwright.NUnit;
using NUnit.Framework;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace MzansiBytes.IntegrationTests
{
    [TestFixture]
    public class Tests : PageTest
    {
        private Process _apiProcess;
        private Process _wasmProcess;

        private string solutionDirectory = Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.Parent.FullName;

        private void DeleteDatabase()
        {
            if (File.Exists(@$"{solutionDirectory}{Path.DirectorySeparatorChar}MzansiBytes.IntegrationTest.db"))
            {
                WaitForFile(@$"{solutionDirectory}{Path.DirectorySeparatorChar}MzansiBytes.API{Path.DirectorySeparatorChar}MzansiBytes.IntegrationTest.db");               
                File.Delete(@$"{solutionDirectory}{Path.DirectorySeparatorChar}MzansiBytes.API{Path.DirectorySeparatorChar}MzansiBytes.IntegrationTest.db");                       
            }

            static bool IsFileReady(string filename)
            {
                // If the file can be opened for exclusive access it means that the file
                // is no longer locked by another process.
                try
                {
                    using (FileStream inputStream = File.Open(filename, FileMode.Open, FileAccess.Read, FileShare.None))
                        return inputStream.Length > 0;
                }
                catch (Exception)
                {
                    return false;
                }
            }

            static void WaitForFile(string filename)
            {
                //This will lock the execution until the file is ready
                while (!IsFileReady(filename)) { }
            }
        }

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            DeleteDatabase();

            _apiProcess = Process.Start(new ProcessStartInfo
            {
                FileName = "dotnet",
                Arguments = "run --configuration Release --launch-profile IntegrationTest",
                WorkingDirectory = @$"{solutionDirectory}{Path.DirectorySeparatorChar}MzansiBytes.API"
            });

            _wasmProcess = Process.Start(new ProcessStartInfo
            {
                FileName = "dotnet",
                Arguments = "run --configuration Release --launch-profile IntegrationTest",
                WorkingDirectory = @$"{solutionDirectory}{Path.DirectorySeparatorChar}MzansiBytes.WASM"
            });
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            _apiProcess?.Kill();
            _wasmProcess?.Kill();

            DeleteDatabase();
        }

        [Test]
        public async Task HomepageHasHelloWorldOnPage()
        {
            _ = await Page.GotoAsync("http://localhost:5171");

            await Expect(Page.Locator("//*[@id=\"app\"]/div/main/article/h1"))
                .ToContainTextAsync("Hello, world!");

            await Expect(Page.Locator("//*[@id=\"app\"]/div/main/article"))
                .ToContainTextAsync("Welcome to your new app.");
        }

        [Test]
        public async Task FetchdataPageHasWeatherForecastData()
        {
            _ = await Page.GotoAsync("http://localhost:5171/fetchdata");

            await Expect(Page.Locator("//*[@id=\"app\"]/div/main/article/h1"))
                .ToContainTextAsync("Weather forecast");

            await Expect(Page.Locator("//*[@id=\"app\"]/div/main/article"))
                .ToContainTextAsync("This component demonstrates fetching data from the server.");

            await Expect(Page.Locator("//*[@id=\"app\"]/div/main/article/table/tbody/tr[3]/td[2]"))
                .ToContainTextAsync("48");

            await Expect(Page.Locator("//*[@id=\"app\"]/div/main/article/table/tbody/tr[3]/td[3]"))
                .ToContainTextAsync("118");

            await Expect(Page.Locator("//*[@id=\"app\"]/div/main/article/table/tbody/tr[3]/td[4]"))
                .ToContainTextAsync("Hot");
        }
    }
}
