using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Data.SqlClient;
//using java.math;
using OpenQA.Selenium;
using log4net;
using Utilities.Config;
using Utility_Classes;
using System.Diagnostics;
using NUnit.Framework;
using System.Drawing;
using System.Threading;

namespace IETTVAdminportal.Reusable
{
    class IE_AdminSetupTearDown
    {
        AdminAuth au = new AdminAuth();                                                         // Instantiate object for Authentication

        Configuration cf = new Configuration();                                                 // Instantiate object for Configuration

        Utility_Functions uf = new Utility_Functions();                                         // Instantiate object for Utility Functons       

        int screenHeight, screenWidth;

        string appURL;
        
        public string IE_Setup(IWebDriver driver, ILog log, IJavaScriptExecutor executor)

        {
            try
            {
                List<string> globList = cf.readSysConfigFile("Global", "Automation", "SysConfig.xml");

                if (uf.getRunningBrowser(driver, driver.FindElement(By.TagName("html"))).ToString().Equals("IE11"))
                {

                    if (uf.checkIE11RegistryPresence().Equals("true"))                                   // Check if Registry Entry is present for IE 11 browser
                    {
                        log.Info("Registry Created successfully / Present for IE 11" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());
                    }
                    else
                    {
                        log.Info("Registry couldn't be created. Test may not run properly in IE 11. Please contact administrator" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());
                    }

                }

                if (globList.ElementAt(0).ToString().ToLower().Equals("yes"))                           // This is to check if AutoIt setting is set to 'Yes'
                {
                    Boolean statLogin = au.authLogin("IE");

                    log.Info("Login Status:" + statLogin + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());
                }

                screenHeight = uf.getScreenHeight(driver);

                screenWidth = uf.getScreenWidth(driver);

                log.Info("Screen Height:" + screenHeight + "Screen Width:" + screenWidth + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

                driver.Manage().Window.Position = new System.Drawing.Point(0, 0);

                driver.Manage().Window.Size = new Size(screenWidth, screenHeight);

                driver.Manage().Cookies.DeleteAllCookies();

                appURL = cf.readingXMLFile("AdminPortal", "Login", "startURL", "Config.xml");

                driver.Navigate().GoToUrl(appURL);

                Thread.Sleep(9000);

            }
            catch (Exception e)
            {
                log.Error("Error occurred in Setup" + e.Message + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

                Assert.AreEqual(true, false);
            }

            return null;
        }

        public void IE_TearDown(IWebDriver driver, ILog log)
        {
            try
            {
                driver.Quit();

                log.Info("Test Completed" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

                log.Info("\n \n --------------------------------------------------------------------------------------------------------------------------------");

            }
            catch (Exception e)
            {
                log.Error(e.Message + "\n" + e.StackTrace + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

                log.Info("\n \n --------------------------------------------------------------------------------------------------------------------------------");

                Assert.AreEqual(true, false);
            }
        }
    }
}
