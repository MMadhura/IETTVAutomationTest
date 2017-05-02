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
using Utilities.Config;
using Utilities.Object_Repository;
using log4net;
using Microsoft.Expression.Encoder.ScreenCapture;
using System.Diagnostics;
using System.Drawing;

namespace IETTVWebportal.Reusable_Functions
{
   
    [TestFixture]
    public class Chrome_WebSetupTearDown
    {
             
        ScreenCaptureJob job = new ScreenCaptureJob();

        Utility_Functions uf = new Utility_Functions();                                         // Instantiate object for Utility Functons

        Configuration cf = new Configuration();                                                 // Instantiate object for Configuration

        Object_Repository_Class or = new Object_Repository_Class();                             // Instantiate object for object repository

        int screenHeight, screenWidth;

        string appURL;

        string recordEvidence = "";

        string keepScreenShots = "";

        public string Chrome_Setup(IWebDriver driver, ILog log, IJavaScriptExecutor executor)
        {
            try
            {
                List<String> lstRecordEvidence = cf.readSysConfigFile("WebPortal", "Evidence", "SysConfig.xml");

                recordEvidence = lstRecordEvidence.ElementAt(1).ToString().ToLower();

                keepScreenShots = lstRecordEvidence.ElementAt(2).ToString().ToLower();

                if (recordEvidence == "yes")
                {
                    job = new ScreenCaptureJob();

                    string testname = NUnit.Framework.TestContext.CurrentContext.Test.FullName;

                    uf.ScreenCap(job, testname, driver);
                }

                screenHeight = uf.getScreenHeight(driver);                                          // Get screen Height                                               

                screenWidth = uf.getScreenWidth(driver);

                log.Info("Screen Height:" + screenHeight + "Screen Width:" + screenWidth + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

                driver.Manage().Timeouts().SetPageLoadTimeout(TimeSpan.FromSeconds(120));

                driver.Manage().Window.Position = new System.Drawing.Point(0, 0);

                driver.Manage().Window.Size = new Size(screenWidth, screenHeight);

                driver.Manage().Cookies.DeleteAllCookies();

                appURL = cf.readingXMLFile("WebPortal", "Login", "startURL", "Config.xml");                

                driver.Navigate().GoToUrl(appURL);

                Thread.Sleep(2000);
            
            }
            catch (Exception e)
            {
                log.Error("Error occurred in Setup" + e.Message + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

                Assert.AreEqual(true, false);
            }

            return appURL;
        }

        public void Chrome_TearDown(IWebDriver driver, ILog log)
        {
            try
            {
                if (keepScreenShots == "yes")
                {
                    string testname = NUnit.Framework.TestContext.CurrentContext.Test.FullName;

                   // uf.TakeScreenshot(driver, "other", testname);

                    Thread.Sleep(2000);

                }

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
