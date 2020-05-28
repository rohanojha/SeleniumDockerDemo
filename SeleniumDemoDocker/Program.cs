using System;
using System.IO;
using System.Reflection;
using System.Threading;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace SeleniumDemoDocker
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {

                var objDataReader = new ReadExcelNew();
                var testData = objDataReader.ReadExcelUlta(@"sample.xlsx", "URL");

                var testResultsFolder = "Results\\" + string.Format("{0:yyyy-MM-dd}", DateTime.Now) + "\\" + "Demo" + "\\" + string.Format("{0:hh-mm-ss}", DateTime.Now);
                Directory.CreateDirectory(testResultsFolder);

                var htmlRep = new HTMLReport1(testResultsFolder + "\\result");
                htmlRep.InitHtmlReport();
                htmlRep.AddHeader(new string[] { "Sr.No", "URL", "Expected Title", "Actual Title", "Result", "Notes" });


                for (int i = 1; i < testData.GetLength(0); i++)
                {
                    string result = "Fail";

                   //var driver = new ChromeDriver(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location));

                    string driverPath = "/opt/selenium/";
                    String driverExecutableFileName = "chromedriver";
                    ChromeOptions options = new ChromeOptions();
                    options.AddArguments("headless");
                    options.AddArguments("no-sandbox");
                    options.AddArgument("--whitelisted-ips");
                    options.AddArgument("--disable-extensions");
                    options.BinaryLocation = "/opt/google/chrome/chrome";
                    ChromeDriverService service = ChromeDriverService.CreateDefaultService(driverPath, driverExecutableFileName);
                    var driver = new ChromeDriver(service, options, TimeSpan.FromSeconds(30));
                    driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(40);



                    try
                    {
                        //driver.Manage().Window.Maximize();

                        driver.Navigate().GoToUrl(testData[i, 1]);

                        if (driver.Title.Contains(testData[i, 2])) ;
                        {
                            result = "Pass";
                        }
                        var ssFileName = testResultsFolder + "\\" + testData[i, 0] + "-screenshot.png";
                        ((ITakesScreenshot)driver).GetScreenshot().SaveAsFile(ssFileName, ScreenshotImageFormat.Png);


                        htmlRep.AddReportData(new string[] { testData[i, 0], testData[i, 1], testData[i, 2], driver.Title, result, "NA" });
                        driver.Close();
                        Thread.Sleep(1000);

                    }
                    catch (Exception ex)
                    {
                        htmlRep.AddReportData(new string[] { testData[i, 0], testData[i, 1], testData[i, 2], "NA", "Exception", ex.ToString() });
                        driver.Close();
                        Thread.Sleep(1000);
                    }

                }
                htmlRep.CloseReport();




            }
            catch (Exception ex)
            {

                Console.WriteLine("Exception occured : "+ex.ToString());
            }
        }
}
}
