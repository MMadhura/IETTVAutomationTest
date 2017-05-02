using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Data.SqlClient;
using OpenQA.Selenium;
using log4net;
using OpenQA.Selenium.Firefox;
using Utilities.Config;
using Utility_Classes;
using System.Diagnostics;
using System.Drawing;
using NUnit.Framework;
using Microsoft.Expression.Encoder.ScreenCapture;

namespace IETTVAdminportal.Reusable
{
    class Firefox_AdminSetupTearDown
    {
        ScreenCaptureJob job = new ScreenCaptureJob();                                          // Instantiate object for Screen capture
       
        AdminAuth au = new AdminAuth();                                                         // Instantiate object for Authentication

        Configuration cf = new Configuration();                                                 // Instantiate object for Configuration

        Utility_Functions uf = new Utility_Functions();                                         // Instantiate object for Utility Functons  

        int screenHeight, screenWidth;

        string recordEvidence = "";

        string appURL;
        
        public string Firefox_Setup(IWebDriver driver, ILog log, IJavaScriptExecutor executor)
        {
            try
            {
                List<String> lstRecordEvidence = cf.readSysConfigFile("WebPortal", "Evidence", "SysConfig.xml");

                recordEvidence = lstRecordEvidence.ElementAt(1).ToString();

                if (recordEvidence == "Yes")
                {
                    job = new ScreenCaptureJob();

                    string testname = NUnit.Framework.TestContext.CurrentContext.Test.FullName;

                    uf.ScreenCap(job, testname, driver);
                }                

                screenHeight = uf.getScreenHeight(driver);

                screenWidth = uf.getScreenWidth(driver);

                log.Info("Screen Height:" + screenHeight + "Screen Width:" + screenWidth + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

                driver.Manage().Timeouts().SetPageLoadTimeout(TimeSpan.FromSeconds(120));

                driver.Manage().Window.Position = new System.Drawing.Point(0, 0);

                driver.Manage().Window.Size = new Size(screenWidth, screenHeight);

                driver.Manage().Cookies.DeleteAllCookies();

                appURL = cf.readingXMLFile("AdminPortal", "Login", "startURL", "Config.xml");  //Getting the Aplication URL

                driver.Navigate().GoToUrl(appURL);

            }
            catch (Exception e)
            {
                log.Error("Error occurred in Setup" + e.Message + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

                Assert.AreEqual(true, false);
            }

            return appURL;
        }

        public void Firefox_TearDown(IWebDriver driver, ILog log)
        {
            try
            {
                driver.Quit();

                if (recordEvidence == "Yes")
                {
                    this.job.Stop();

                    this.job = null;
                }

                log.Info("Test Completed" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

                log.Info("\n \n --------------------------------------------------------------------------------------------------------------------------------");

            }
            catch (Exception e)
            {
                log.Error(e.Message + "\n" + e.StackTrace + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

                log.Info("\n \n --------------------------------------------------------------------------------------------------------------------------------");

                this.job.Stop();

                Assert.AreEqual(true, false);
            }
        }
    }
}
