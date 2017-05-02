
using IETTVWebportal.Reusable_Functions;
using Microsoft.Expression.Encoder.ScreenCapture;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Utilities.Config;
using Utilities.Object_Repository;
using Utility_Classes;
using log4net;
using System.Threading;

namespace IETTVWebportal.Chrome
{
    [TestFixture]
    public class Chrome_AccountManagementVerification
    {
        private static ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        internal IWebDriver driver = null;

        IJavaScriptExecutor executor;

        IWait<IWebDriver> iWait;

        string driverName = "", driverPath, appURL;

        Utility_Functions uf = new Utility_Functions();                      // Instantiate object for Utility Functons

        Configuration cf = new Configuration();                             // Instantiate object for Configuration

        Object_Repository_Class OR = new Object_Repository_Class();        // Instantiate object for object repository

        ScreenCaptureJob job = null;

        Chrome_WebSetupTearDown st = new Chrome_WebSetupTearDown();

        [TestFixtureSetUp]
        public void FixtureSetUp()
        {
            try
            {

                log4net.Config.XmlConfigurator.Configure(new FileInfo(AppDomain.CurrentDomain.BaseDirectory + "WebPortal.config"));

                log.Info("Inside Fixture Setup of chrome - NonIETmemberRegistration Test" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

                uf.CreateOrReplaceVideoFolder();

                string baseDir = AppDomain.CurrentDomain.BaseDirectory;                                              // Get path till Base Directory

                driverName = "webdriver.chrome.driver";                                                             // Driver name for Chrome

                driverPath = baseDir + "/chromedriver.exe";                                                        // Path for ChromeDriver

                System.Environment.SetEnvironmentVariable(driverName, driverPath);

                driver = new ChromeDriver(baseDir + "/DLLs", new ChromeOptions(), TimeSpan.FromSeconds(120));     // Initialize chrome driver   

                executor = (IJavaScriptExecutor)driver;

                iWait = new WebDriverWait(driver, TimeSpan.FromSeconds(60));
            }
            catch (Exception e)
            {
                log.Error(e.Message + "\n" + e.StackTrace + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

                Assert.AreEqual(true, false);
            }
        }

        [SetUp]
        public void SetUp()
        {
            appURL = st.Chrome_Setup(driver, log, executor);                                // Calling Chrome Setup
        }

        public Chrome_AccountManagementVerification(IWebDriver driver, log4net.ILog log1, IJavaScriptExecutor Executor, IWait<IWebDriver> iWait)
        {
            this.driver = driver;

            log = log1;

            this.executor = Executor;

            this.iWait = iWait;

        }

        public void AccountLogin()
        {
            log.Info("inside AccountLogin " + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

            iWait.Until(ExpectedConditions.ElementExists((OR.GetElement("NonIETMemberRegistration", "LoginLink", "TVWebPortalOR.xml"))));

            IWebElement loginLink = driver.FindElement((OR.GetElement("NonIETMemberRegistration", "LoginLink", "TVWebPortalOR.xml")));

            executor.ExecuteScript("arguments[0].click();", loginLink);

            string userName = cf.readingXMLFile("Admin", "Account Management", "UserEmailAddress", "TestCopy.xml");
            //string userName = "madhura123@yopmail.com";
            string userPassword = cf.readingXMLFile("Admin", "Account Management", "Password", "TestCopy.xml");

            log.Info("username    " + userName);

            log.Info("Password    " + userPassword);

            iWait.Until(ExpectedConditions.ElementIsVisible(OR.GetElement("VideoRequestByUser", "UserNameTB", "TVWebPortalOR.xml")));

            driver.FindElement(OR.GetElement("VideoRequestByUser", "UserName", "TVWebPortalOR.xml")).SendKeys(userName);
            Thread.Sleep(1000);

            driver.FindElement(OR.GetElement("VideoRequestByUser", "Password", "TVWebPortalOR.xml")).SendKeys(userPassword);

            iWait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy(OR.GetElement("VideoRequestByUser", "Login", "TVWebPortalOR.xml")));

            driver.FindElement(OR.GetElement("VideoRequestByUser", "Login", "TVWebPortalOR.xml")).Click();

            Thread.Sleep(2000);

            ClickTermsandConditionsAcceptButton();

        }

        public void ClickTermsandConditionsAcceptButton()
        {
            log.Info("inside ClickTermsandConditionsAcceptButton " + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

            Boolean yesButtonPresent = IsElementPresent(driver, By.Id("Accept"));
            if (yesButtonPresent)
            {
                driver.FindElement(By.Id("Accept")).Click();
            }

        }

        public void ClickYesButton()
        {
            log.Info("inside ClickYesButton " + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

            Boolean yesButtonPresent = IsElementPresent(driver, By.Id("yesButtonId"));
            if (yesButtonPresent)
            {
                driver.FindElement(By.Id("yesButtonId")).Click();
            }

        }

        public static Boolean IsElementPresent(IWebDriver driver, By element)
        {
            try
            {
                Thread.Sleep(1000);
                driver.FindElement(element);
                return true;
            }
            catch (Exception)
            {

                return false;
            }
        }

        public void WelcomeMessage()
        {
            log.Info("inside WelcomeMessage " + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

            iWait.Until(ExpectedConditions.ElementIsVisible((OR.GetElement("VideoManagement", "LoginWelcomeMsg", "TVWebPortalOR.xml"))));

            driver.FindElement((OR.GetElement("VideoManagement", "WelcomeMsgCSS", "TVWebPortalOR.xml"))).Click();

            iWait.Until(ExpectedConditions.InvisibilityOfElementLocated((OR.GetElement("VideoManagement", "LoginWelcomeMsg", "TVWebPortalOR.xml"))));
        }

        public void ClickMyIETTVSubscription()
        {
            log.Info("inside ClickMyIETTVSubscription " + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

            uf.isJqueryActive(driver);

            Thread.Sleep(2000);

            iWait.Until(ExpectedConditions.ElementIsVisible((OR.GetElement("VideoRequestByUser", "MyIETDropDown", "TVWebPortalOR.xml"))));

            driver.FindElement((OR.GetElement("VideoRequestByUser", "MyIETDropDown", "TVWebPortalOR.xml"))).Click();

            IWebElement mYIETTVLink = driver.FindElement((OR.GetElement("AccountManagement", "MyIETTVLink", "TVWebPortalOR.xml")));

            executor.ExecuteScript("arguments[0].click();", mYIETTVLink);

            uf.isJqueryActive(driver);
        }

        public void VerifySuccessBannerMsg(String message)
        {
            log.Info("inside VerifySuccessBannerMsg " + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

            Console.WriteLine("Inside VerifySuccessBannerMessage:::::");
            IWait<IWebDriver> wait = new WebDriverWait(driver, TimeSpan.FromSeconds(180.0));
            wait.Until<IWebElement>(ExpectedConditions.ElementIsVisible((OR.GetElement("SeriesManagement", "SuccessOkButton", "TVAdminPortalOR.xml"))));
            iWait.Until<bool>(d => d.FindElement((OR.GetElement("SeriesManagement", "SucessMessage", "TVAdminPortalOR.xml"))).Text.Equals(message));
            NUnit.Framework.Assert.AreEqual(message, driver.FindElement((OR.GetElement("SeriesManagement", "SucessMessage", "TVAdminPortalOR.xml"))).Text.Trim());
            driver.FindElement((OR.GetElement("SeriesManagement", "SuccessOkButton", "TVAdminPortalOR.xml"))).Click();
            iWait.Until(ExpectedConditions.InvisibilityOfElementLocated((OR.GetElement("SeriesManagement", "SuccessOkButton", "TVAdminPortalOR.xml"))));
            // OverlayWait();

        }

        public void handlePromotionalPopup()
        {
            log.Info("inside handlePromotionalPopup " + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

            IWebElement promotionalPopup = driver.FindElement((OR.GetElement("SeriesManagement", "Promotionaldiv", "TVWebPortalOR.xml")));

            String PromoPopup = promotionalPopup.GetCssValue("display").ToString();

            if (PromoPopup.Equals("block"))
            {
                //iWait.Until(ExpectedConditions.ElementIsVisible(OR.GetElement("SeriesManagement", "Promotionaldiv", "TVWebPortalOR.xml")));

                driver.FindElement((OR.GetElement("SeriesManagement", "PromotionalPopup", "TVWebPortalOR.xml"))).Click();

                iWait.Until(ExpectedConditions.InvisibilityOfElementLocated(OR.GetElement("SeriesManagement", "Promotionaldiv", "TVWebPortalOR.xml")));
            }

        }

        public void HandleEmergencyPopUp()
        {
            log.Info("inside HandleEmergencyPopUp " + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

            IWebElement emergencyPopup = driver.FindElement((OR.GetElement("SeriesManagement", "EmergencyDiv", "TVWebPortalOR.xml")));

            string emergencyColor = emergencyPopup.GetCssValue("display").ToString();

            if (emergencyColor.Equals("block"))
            {
                iWait.Until(ExpectedConditions.ElementIsVisible(OR.GetElement("SeriesManagement", "EmergencyDiv", "TVWebPortalOR.xml")));

                driver.FindElement((OR.GetElement("SeriesManagement", "NewEmergencyPopup", "TVWebPortalOR.xml"))).Click();

                iWait.Until(ExpectedConditions.InvisibilityOfElementLocated((OR.GetElement("SeriesManagement", "EmergencyDiv", "TVWebPortalOR.xml"))));
            }

        }

        public void switchToWebPortal()
        {
            log.Info("inside switchToWebPortal " + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

            uf.OpenNewTab(driver);

            log.Info("count ::: " + driver.WindowHandles.Count);

            String browsertype = uf.getRunningBrowser(driver, driver.FindElement(By.TagName("html")));

            uf.SwitchToWebTab(driver, browsertype);

            uf.NavigateWebPortal(cf, driver);

        }

        public void Logout()
        {
            log.Info("inside Logout " + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

            Thread.Sleep(2000);

            iWait.Until(ExpectedConditions.ElementIsVisible((OR.GetElement("VideoRequestByUser", "MyIETDropDown", "TVWebPortalOR.xml"))));

            IWebElement myAccountDropDown = driver.FindElement((OR.GetElement("VideoRequestByUser", "MyIETDropDown", "TVWebPortalOR.xml")));

            executor.ExecuteScript("arguments[0].click()", myAccountDropDown);

            iWait.Until(ExpectedConditions.ElementIsVisible((OR.GetElement("BuyVideoVerification", "LogOutLink", "TVWebPortalOR.xml"))));
            driver.FindElement((OR.GetElement("BuyVideoVerification", "LogOutLink", "TVWebPortalOR.xml"))).FindElement(By.TagName("a")).Click();
        }

        [TestFixtureTearDown]
        public void TearDown()
        {
            st.Chrome_TearDown(driver, log);                        // Calling Chrome Teardown
        }
    }
}
