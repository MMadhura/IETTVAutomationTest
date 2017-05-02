using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.IE;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using Utility_Classes;
using Utilities.Config;
using Utilities.Object_Repository;
using log4net;
using log4net.Config;
using System.Reflection;
using System.IO;
using System.Diagnostics;
using NSoup.Nodes;  // for nsoup documents
using NSoup.Select;  //for nsoup element
//using IETTVAdminPortal.Chrome;
using Microsoft.Expression.Encoder.ScreenCapture;
using IETTVWebportal.Reusable_Functions;


namespace IETTVWebportal.Chrome
{
    [TestFixture]
    public class Chrome_VideoRequestByUserVerification
    {
        // This is to configure logger mechanism for Utilities.Config
        private readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        #region Variable Decration and object initialistaion

        internal IWebDriver driver = null;

        IWait<IWebDriver> iWait = null;

        string videoName;

        string driverName = "", driverPath, appURL;

        IJavaScriptExecutor executor = null;

        Utility_Functions uf = new Utility_Functions();                          // Instantiate object for Utility Functons

        Configuration cf = new Configuration();                                  // Instantiate object for Configuration                               

        Object_Repository_Class OR = new Object_Repository_Class();             // Instantiate object for object repository

        IETTVWebportal.Reusable_Functions.Chrome_WebSetupTearDown st = new IETTVWebportal.Reusable_Functions.Chrome_WebSetupTearDown();             // Instantiate object for Chrome Setup Teardown

        #endregion


        #region constructor

        public Chrome_VideoRequestByUserVerification()
        {

        }

        public Chrome_VideoRequestByUserVerification(IWebDriver driver, ILog log1, IJavaScriptExecutor Executor, IWait<IWebDriver> iWait)
        {
            this.driver = driver;

            log = log1;

            this.executor = Executor;

            this.iWait = iWait;

        }
        #endregion

        #region SetUp

        [TestFixtureSetUp]
        public void FixtureSetUp()
        {
            log4net.Config.XmlConfigurator.Configure(new FileInfo(AppDomain.CurrentDomain.BaseDirectory + "WebPortal.config"));

            log.Info("Inside Fixture Setup of chrome - NonIETmemberRegistration Test" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

            //uf.CreateOrReplaceVideoFolder();

            string baseDir = AppDomain.CurrentDomain.BaseDirectory;                        // Get path till Base Directory

            driverName = "webdriver.chrome.driver";                                        // Driver name for Chrome

            driverPath = baseDir + "/chromedriver.exe";                                    // Path for ChromeDriver

            System.Environment.SetEnvironmentVariable(driverName, driverPath);

            driver = new ChromeDriver();                                                   // Initialize chrome driver   

            executor = (IJavaScriptExecutor)driver;

            iWait = new WebDriverWait(driver, TimeSpan.FromSeconds(60));

        }

        [SetUp]
        public void SetUp()
        {
            appURL = st.Chrome_Setup(driver, log, executor);                                // Calling Chrome Setup
        }

        #endregion

        #region Reusable Function

        //This function wait invisibility of overlay class
        public void OverlayWait()
        {
            iWait.Until(d => d.FindElement((OR.GetElement("SeriesManagement", "OverlayWait", "TVWebPortalOR.xml"))).GetAttribute("class").Contains("display-block"));
        }

        public void RedirectToLogin()
        {
            uf.isJqueryActive(driver);

            iWait.Until(ExpectedConditions.ElementExists((OR.GetElement("VideoRequestByUser", "LoginLink", "TVWebPortalOR.xml"))));

            IWebElement loginLink = driver.FindElement((OR.GetElement("VideoRequestByUser", "LoginLink", "TVWebPortalOR.xml")));

            executor.ExecuteScript("arguments[0].click();", loginLink);

            log.Info("username    " + cf.readingXMLFile("WebPortal", "InstitutionUser", "instUserName", "Config.xml"));

            log.Info("Password    " + cf.readingXMLFile("WebPortal", "InstitutionUser", "instPassWord", "Config.xml"));

            string userName = cf.readingXMLFile("WebPortal", "InstitutionUser", "instUserName", "Config.xml");
            string userPassword = cf.readingXMLFile("WebPortal", "InstitutionUser", "instPassWord", "Config.xml");


            iWait.Until(ExpectedConditions.ElementIsVisible((OR.GetElement("VideoRequestByUser", "UserNameTB", "TVWebPortalOR.xml"))));

            driver.FindElement((OR.GetElement("VideoRequestByUser", "UserName", "TVWebPortalOR.xml"))).SendKeys(userName);
            Thread.Sleep(1000);

            driver.FindElement((OR.GetElement("VideoRequestByUser", "Password", "TVWebPortalOR.xml"))).SendKeys(userPassword);

            iWait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy((OR.GetElement("VideoRequestByUser", "Login", "TVWebPortalOR.xml"))));

            driver.FindElement((OR.GetElement("VideoRequestByUser", "Login", "TVWebPortalOR.xml"))).Click();
            Thread.Sleep(2000);

            log.Info("already logged in count  : " + driver.FindElements((OR.GetElement("VideoRequestByUser", "YesButton", "TVWebPortalOR.xml"))).Count);
            //if user is already logged in
            if (driver.FindElements((OR.GetElement("VideoRequestByUser", "YesButton", "TVWebPortalOR.xml"))).Count > 0)
            {
                driver.FindElement((OR.GetElement("VideoRequestByUser", "YesButton", "TVWebPortalOR.xml"))).Click();
            }

            //Handling pop up message
            iWait.Until(ExpectedConditions.InvisibilityOfElementLocated((OR.GetElement("SeriesManagement", "OverlayWait", "TVWebPortalOR.xml"))));

            iWait.Until(ExpectedConditions.ElementIsVisible((OR.GetElement("VideoRequestByUser", "LoginWelcomeMessage", "TVWebPortalOR.xml"))));

            executor.ExecuteScript("arguments[0].click()", driver.FindElement((OR.GetElement("VideoRequestByUser", "LoginWelcomeMessageOkButton", "TVWebPortalOR.xml"))));

            iWait.Until(ExpectedConditions.InvisibilityOfElementLocated((OR.GetElement("VideoRequestByUser", "LoginWelcomeMessage", "TVWebPortalOR.xml"))));

        }

        #endregion

        public string TestViewCreateRequestVideo()
        {
            try
            {
                String currentDate = DateTime.Now.ToString("dd/MM/yyyy");

                log.Info("TestViewCreateRequestVideo  started" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

                uf.isJqueryActive(driver);

                //RedirectToLogin();

                Console.Write("user successfully logged in");

                //wait till jquery gets completed
                uf.isJqueryActive(driver);

                Thread.Sleep(2000);

                iWait.Until(ExpectedConditions.ElementIsVisible((OR.GetElement("VideoRequestByUser", "MyIETDropDown", "TVWebPortalOR.xml"))));

                driver.FindElement((OR.GetElement("VideoRequestByUser", "MyIETDropDown", "TVWebPortalOR.xml"))).Click();

                IWebElement vidReqLink = driver.FindElement((OR.GetElement("VideoRequestByUser", "VideoRequest", "TVWebPortalOR.xml")));

                executor.ExecuteScript("arguments[0].click();", vidReqLink);

                uf.isJqueryActive(driver);

                //Waiting for My Request tab link
                iWait.Until(ExpectedConditions.ElementIsVisible((OR.GetElement("VideoRequestByUser", "NewRequestButton", "TVWebPortalOR.xml"))));

                //Click on Create New Request Link
                driver.FindElement((OR.GetElement("VideoRequestByUser", "NewRequestButton", "TVWebPortalOR.xml"))).Click();

                driver.FindElement((OR.GetElement("VideoRequestByUser", "ContactNoTB", "TVWebPortalOR.xml"))).SendKeys("09766091717");

                //getting the uique name for the video title
                videoName = "vid" + uf.getShortGuid();

                driver.FindElement((OR.GetElement("VideoRequestByUser", "VidTitle", "TVWebPortalOR.xml"))).SendKeys(videoName);

                driver.FindElement((OR.GetElement("VideoRequestByUser", "ShortDescTB", "TVWebPortalOR.xml"))).SendKeys("Test Short Desc");

                driver.FindElement((OR.GetElement("VideoRequestByUser", "Abstract", "TVWebPortalOR.xml"))).SendKeys("Test Abstract");


                //Constraint : There should be atleast one channel present in drop down.
                //Selecting random primary channel
                SelectElement primaryChannel = new SelectElement(driver.FindElement((OR.GetElement("VideoRequestByUser", "DefaultChannelDDL", "TVWebPortalOR.xml"))));
                primaryChannel.SelectByIndex(1);

                string eventName = "autoEvent" + uf.getShortGuid();

                driver.FindElement((OR.GetElement("VideoRequestByUser", "EventNameTXT", "TVWebPortalOR.xml"))).SendKeys(eventName);

                driver.FindElement((OR.GetElement("VideoRequestByUser", "LocNameTXT", "TVWebPortalOR.xml"))).SendKeys("Test Location");

                driver.FindElement((OR.GetElement("VideoRequestByUser", "VenueTxt", "TVWebPortalOR.xml"))).SendKeys("Test Venue");

                String[] dateTime = currentDate.Split(' ');

                //Selecting todays date from date picker
                driver.FindElement((OR.GetElement("VideoRequestByUser", "EventDateTXT", "TVWebPortalOR.xml"))).SendKeys(currentDate);

                //Clicking on Save Button
                driver.FindElement((OR.GetElement("VideoRequestByUser", "SaveBTN", "TVWebPortalOR.xml"))).Click();
                Thread.Sleep(2000);
                iWait.Until(ExpectedConditions.ElementIsVisible((OR.GetElement("VideoRequestByUser", "Overlay", "TVWebPortalOR.xml"))));

                driver.FindElement((OR.GetElement("VideoRequestByUser", "BannerMessageOkButton", "TVWebPortalOR.xml"))).Click();

                return videoName + "-" + eventName;
            }
            catch (Exception e)
            {
                log.Error(e.Message + "\n" + e.StackTrace + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());
                Console.WriteLine(e.Message + "\n" + e.StackTrace + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());
                Assert.AreEqual(true, false);

                return videoName;
            }
        }

        public string TestRejectRequestVideo()
        {
            try
            {
                log.Info("TestRejectRequestVideo  started" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

                uf.isJqueryActive(driver);

                //RedirectToLogin();

                Console.Write("user successfully logged in");

                //wait till jquery gets completed
                uf.isJqueryActive(driver);

                iWait.Until(ExpectedConditions.ElementIsVisible((OR.GetElement("VideoRequestByUser", "MyIETDropDown", "TVWebPortalOR.xml"))));

                driver.FindElement((OR.GetElement("VideoRequestByUser", "MyIETDropDown", "TVWebPortalOR.xml"))).Click();

                IWebElement vidReqLink = driver.FindElement((OR.GetElement("VideoRequestByUser", "VideoRequest", "TVWebPortalOR.xml")));

                executor.ExecuteScript("arguments[0].click();", vidReqLink);

                uf.isJqueryActive(driver);

                //Waiting for My Request tab link
                iWait.Until(ExpectedConditions.ElementIsVisible((OR.GetElement("VideoRequestByUser", "NewRequestButton", "TVWebPortalOR.xml"))));

                //Click on Create New Request Link
                driver.FindElement((OR.GetElement("VideoRequestByUser", "NewRequestButton", "TVWebPortalOR.xml"))).Click();

                driver.FindElement((OR.GetElement("VideoRequestByUser", "ContactNoTB", "TVWebPortalOR.xml"))).SendKeys("09766091717");

                //getting the uique name for the video title
                videoName = "vid" + uf.getShortGuid();

                driver.FindElement((OR.GetElement("VideoRequestByUser", "VidTitle", "TVWebPortalOR.xml"))).SendKeys(videoName);

                driver.FindElement((OR.GetElement("VideoRequestByUser", "ShortDescTB", "TVWebPortalOR.xml"))).SendKeys("Test Short Desc");

                driver.FindElement((OR.GetElement("VideoRequestByUser", "Abstract", "TVWebPortalOR.xml"))).SendKeys("Test Abstract");


                //Constraint : There should be atleast one channel present in drop down.
                //Selecting random primary channel
                SelectElement primaryChannel = new SelectElement(driver.FindElement((OR.GetElement("VideoRequestByUser", "DefaultChannelDDL", "TVWebPortalOR.xml"))));
                primaryChannel.SelectByIndex(1);

                //Clicking on Save Button
                driver.FindElement((OR.GetElement("VideoRequestByUser", "SaveBTN", "TVWebPortalOR.xml"))).Click();

                iWait.Until(ExpectedConditions.ElementIsVisible((OR.GetElement("VideoRequestByUser", "BannerMessageOkButton", "TVWebPortalOR.xml"))));

                executor.ExecuteScript("arguments[0].click()", driver.FindElement((OR.GetElement("VideoRequestByUser", "BannerMessageOkButton", "TVWebPortalOR.xml"))));

                return videoName;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message + "\n" + e.StackTrace + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());
                log.Error(e.Message + "\n" + e.StackTrace + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

                Assert.AreEqual(true, false);

                return videoName;
            }
        }

        [TestFixtureTearDown]
        public void TearDown()
        {
            //st.Chrome_TearDown(driver, log);                        // Calling Chrome Teardown
        }

    }

}
