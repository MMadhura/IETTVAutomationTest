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
using Microsoft.Expression.Encoder.ScreenCapture;
using Utilities.Config;
using Utilities.Object_Repository;
using log4net;
using System.Drawing;
using System.Diagnostics;

namespace IETTVWebportal.Reusable_Functions
{
    [TestFixture]
    public class Firefox_WebSetupTearDown
    {
        ScreenCaptureJob job = new ScreenCaptureJob();

        Utility_Functions uf = new Utility_Functions();                                         // Instantiate object for Utility Functons

        Configuration cf = new Configuration();                                                 // Instantiate object for Configuration

        int screenHeight, screenWidth;

        string appURL;

        FirefoxProfile profile;

        internal IWebDriver driver = null;

        public IWebDriver Firefox_PlayVideo_Fixture()
        {
            profile = new FirefoxProfile();


            profile.AddExtension(@"C:\Project\IETTVWebPortal\Firebug\firebug-2.0.9.xpi");
            profile.AddExtension(@"C:\Project\IETTVWebPortal\Firebug\netExport-0.9b7.xpi");
            profile.AddExtension(@"C:\Project\IETTVWebPortal\Firebug\fireStarter-0.1a6.xpi");

            profile.EnableNativeEvents = true;


            // Firebug Preferences

            profile.SetPreference("extensions.firebug.DBG_NETEXPORT", true);
            profile.SetPreference("extensions.firebug.script.enableSites", true);
            profile.SetPreference("extensions.firebug.net.persistent", true);
            profile.SetPreference("extensions.firebug.net.enableSites", true);
            profile.SetPreference("extensions.firebug.previousPlacement", 1);
            profile.SetPreference("extensions.firebug.console.enableSites", true);
            profile.SetPreference("extensions.firebug.consoles.enableSite", true);
            profile.SetPreference("extensions.firebug.addonBarOpened", true);
            profile.SetPreference("extensions.firebug.currentVersion", "2.0.9");
            profile.SetPreference("extensions.firebug.DBG_STARTER", true);
            profile.SetPreference("extensions.firebug.allPagesActivation", "on");
            profile.SetPreference("extensions.firebug.onByDefault", true);
            profile.SetPreference("extensions.firebug.defaultPanelName", "net");

            // Net Export Preferences

            profile.SetPreference("extensions.firebug.netexport.sendToConfirmation", false);
            profile.SetPreference("extensions.firebug.netexport.defaultLogDir", "C:\\Project\\IETTVWebPortal\\Firebug");
            profile.SetPreference("extensions.firebug.netexport.alwaysEnableAutoExport", true);
            profile.SetPreference("extensions.firebug.netexport.autoExportToFile", true);
            profile.SetPreference("extensions.firebug.netexport.saveFiles", true);
            profile.SetPreference("extensions.firebug.netexport.autoExportToServer", false);
            profile.SetPreference("extensions.firebug.netexport.Automation", true);
            profile.SetPreference("extensions.firebug.netexport.showPreview", true);  // preview.
            profile.SetPreference("extensions.firebug.netexport.timeout", 60000);
            profile.SetPreference("extensions.firebug.netexport.pageLoadedTimeout", 1500);
            profile.SetPreference("extensions.firebug.net.defaultPersist", true);

            driver = new FirefoxDriver(profile);

            return driver;
        }

        public string Firefox_Setup(IWebDriver driver, ILog log, IJavaScriptExecutor executor)
        {

            try
            {
                job = new ScreenCaptureJob();

                string testname = NUnit.Framework.TestContext.CurrentContext.Test.FullName;

                uf.ScreenCap(job, testname, driver);

                screenHeight = uf.getScreenHeight(driver);                                          // Get screen Height

                screenWidth = uf.getScreenWidth(driver);                                            // Get Screen Width

                log.Info("Screen Height:" + screenHeight + "Screen Width:" + screenWidth + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

                driver.Manage().Window.Position = new System.Drawing.Point(0, 0);

                driver.Manage().Window.Size = new Size(screenWidth, screenHeight);

                driver.Manage().Cookies.DeleteAllCookies();

                appURL = cf.readingXMLFile("WebPortal", "Login", "startURL", "Config.xml");         // Read application URL
                                                           
                driver.Navigate().GoToUrl(appURL);                                                  // Navigate to URL

                Thread.Sleep(2000);
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
