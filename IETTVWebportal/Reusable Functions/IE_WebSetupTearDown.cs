using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using System.Threading;
using OpenQA.Selenium.Support.UI;
using Utility_Classes;
using log4net;
using System.Diagnostics;
using Microsoft.Expression.Encoder.ScreenCapture;
using Utilities.Config;
using System.Drawing;

namespace IETTVWebportal.Reusable_Functions
{
    [TestFixture]
    public class IE_WebSetupTearDown
    {

        ScreenCaptureJob job = new ScreenCaptureJob();

        Utility_Functions uf = new Utility_Functions();                                         // Instantiate object for Utility Functons

        Configuration cf = new Configuration();                                                 // Instantiate object for Configuration

        int screenHeight, screenWidth;

        string appURL;

        internal IWebDriver driver = null;

        public string IE_Setup(IWebDriver driver, ILog log, IJavaScriptExecutor executor)
        {
            try
            {
                job = new ScreenCaptureJob();

                string testname = NUnit.Framework.TestContext.CurrentContext.Test.FullName;

                uf.ScreenCap(job, testname, driver);

                screenHeight = uf.getScreenHeight(driver);

                screenWidth = uf.getScreenWidth(driver);

                log.Info("Screen Height:" + screenHeight + "Screen Width:" + screenWidth + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

                driver.Manage().Window.Position = new System.Drawing.Point(0, 0);

                driver.Manage().Window.Size = new Size(screenWidth, screenHeight);

                log.Info("Running browser:" + uf.getRunningBrowser(driver, driver.FindElement(By.TagName("html"))).ToString() + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

                driver.Manage().Cookies.DeleteAllCookies();

                appURL = cf.readingXMLFile("WebPortal", "Login", "startURL", "Config.xml");

                driver.Navigate().GoToUrl(appURL);         

            }
            catch (Exception e)
            {
                log.Error("Error occurred in Setup" + e.Message + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

                Assert.AreEqual(true, false);
            }

            return appURL;
        }


        public void IE_TearDown(IWebDriver driver, ILog log)
        {
            try
            {
                driver.Quit();

                this.job.Stop();

                this.job = null;

                log.Info("Test Completed" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

                log.Info("\n \n --------------------------------------------------------------------------------------------------------------------------------");
            }
            catch (Exception e)
            {
                log.Error(e.Message + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

                log.Info("\n \n --------------------------------------------------------------------------------------------------------------------------------");

                Assert.AreEqual(true, false);

                this.job.Stop();
            }
        }

    }
}
