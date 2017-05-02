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
using Microsoft.Expression.Encoder.ScreenCapture;
using IETTVWebportal.Reusable_Functions;

namespace IETTVWebportal.Chrome
{
    [TestFixture]
    class Chrome_ForgottenPasswordVerification
    {
        internal IWebDriver driver = null;

        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        IWebElement element = null;

        string driverName = "", driverPath, appURL;

        IWait<IWebDriver> iWait = null;

        IJavaScriptExecutor executor = null;

        Utility_Functions uf = new Utility_Functions();                                         // Instantiate object for Utility Functons

        Configuration cf = new Configuration();                                                 // Instantiate object for Configuration

        Object_Repository_Class or = new Object_Repository_Class();                             // Instantiate object for object repository

        Chrome_WebSetupTearDown st = new Chrome_WebSetupTearDown();

        Chrome_VideoManagementVerification videoResult;
        // Instantiate object for Chrome Setup Teardown

        [TestFixtureSetUp]
        public void FixtureSetUp()
        {
            log4net.Config.XmlConfigurator.Configure(new FileInfo(AppDomain.CurrentDomain.BaseDirectory + "WebPortal.config"));

            log.Info("Inside Fixture Setup of chrome - NonIETmemberRegistration Test" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

            uf.CreateOrReplaceVideoFolder();

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
            appURL = st.Chrome_Setup(driver, log, executor);                                    // Calling Chrome Setup
        }

        [Test]
        public void TVWeb_001_VerifyForgottenPassword()
        {

            try
            {
                log.Info("Verify Forgotten Password Test Started" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

                IWebElement forgotPass, loginText, btnCancel, forgotUsername, btnSubmit, resetPassword;

                IJavaScriptExecutor executor;

                WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(60));

                wait.Until(ExpectedConditions.ElementExists(By.ClassName(or.readingXMLFile("ForgottenPassword", "overLaySpinner", "TVWebPortalOR.xml"))));

                loginText = driver.FindElement(By.Id(or.readingXMLFile("ForgottenPassword", "login", "TVWebPortalOR.xml"))).FindElement(By.TagName("a"));

                executor = (IJavaScriptExecutor)driver;
                executor.ExecuteScript("arguments[0].click();", loginText);

                element = wait.Until<IWebElement>((d) =>
                {
                    return d.FindElement(By.ClassName(or.readingXMLFile("ForgottenPassword", "checkBox", "TVWebPortalOR.xml"))).FindElement(By.TagName("div"));
                });

                Thread.Sleep(2000);

                forgotPass = driver.FindElement(By.ClassName(or.readingXMLFile("ForgottenPassword", "checkBox", "TVWebPortalOR.xml"))).FindElement(By.TagName("div")).FindElement(By.TagName("a"));

                executor = (IJavaScriptExecutor)driver;
                executor.ExecuteScript("arguments[0].click();", forgotPass);

                //element = wait.Until<IWebElement>((d) =>
                //{
                //    return d.FindElement(By.Id(or.readingXMLFile("ForgottenPassword", "userName", "TVWebPortalOR.xml")));
                //});

                //// Clicking on Cancel button and verify if login page is displayed

                //btnCancel = driver.FindElement(By.CssSelector(or.readingXMLFile("ForgottenPassword", "Cancel", "TVWebPortalOR.xml")));

                //element = wait.Until<IWebElement>((d) =>
                //{
                //    return d.FindElement(By.CssSelector(or.readingXMLFile("ForgottenPassword", "Cancel", "TVWebPortalOR.xml")));
                //});

                //btnCancel.Click();

                //element = wait.Until<IWebElement>((d) =>
                //{
                //    return d.FindElement(By.Id(or.readingXMLFile("ForgottenPassword", "aftCancelUsername", "TVWebPortalOR.xml")));
                //});

                //String loginPageTitle = driver.Url.ToString();

                //log.Info("Login Page URL:" + loginPageTitle + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

                //Assert.AreEqual(appURL + "/view/LoginNew.html", loginPageTitle);

                //Thread.Sleep(2000);

                //forgotPass = driver.FindElement(By.ClassName(or.readingXMLFile("ForgottenPassword", "checkBox", "TVWebPortalOR.xml"))).FindElement(By.TagName("div")).FindElement(By.TagName("a"));

                //// Clicking on Forgot Password link 

                //executor = (IJavaScriptExecutor)driver;
                //executor.ExecuteScript("arguments[0].click();", forgotPass);                     

                //element = wait.Until<IWebElement>((d) =>
                //{
                //    return d.FindElement(By.Id(or.readingXMLFile("ForgottenPassword", "userName", "TVWebPortalOR.xml")));
                //});

                //  User name of individual user

                forgotUsername = driver.FindElement(By.Id(or.readingXMLFile("ForgottenPassword", "userName", "TVWebPortalOR.xml")));

                // Click on Submit without entering username

                //btnSubmit = driver.FindElement(By.Id(or.readingXMLFile("ForgottenPassword", "Submit", "TVWebPortalOR.xml")));            
                //btnSubmit.Click();

                //uf.isJqueryActive(driver);

                //// Get style attribute value
                //String userBorder = forgotUsername.GetAttribute("style").ToString();

                //log.Info("userBorder:=" + userBorder + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());


                //if (driverName.Equals("webdriver.ie.driver"))
                //{
                //    // For IE Browser
                //    Assert.AreEqual("padding: 2px 1px; border: 1px solid rgb(197, 0, 0);", userBorder);  

                //}
                //else
                //{
                //    Assert.AreEqual("border: 1px solid rgb(197, 0, 0); padding: 2px 1px;", userBorder);
                //}

                //  Forgot Username (Need to take this value from test data)

                forgotUsername.SendKeys("Madhura.Mungekar@yopmail.com");

                btnSubmit = driver.FindElement(By.Id(or.readingXMLFile("ForgottenPassword", "Submit", "TVWebPortalOR.xml")));
                btnSubmit.Click();

                Thread.Sleep(2000);

                resetPassword = driver.FindElement(By.Id(or.readingXMLFile("ForgottenPassword", "btnYes", "TVWebPortalOR.xml")));
                resetPassword.Click();

                Thread.Sleep(2000);
                //iWait.Until(ExpectedConditions.ElementExists(By.ClassName(or.readingXMLFile("VideoLandingPage", "InfoMessage", "TVWebPortalOR.xml"))));

                iWait.Until(ExpectedConditions.ElementIsVisible((or.GetElement("VideoLandingPage", "InfoMessage", "TVWebPortalOR.xml"))));

                driver.FindElement((or.GetElement("ReportAbuse", "InfoMessageButton", "TVWebPortalOR.xml"))).Click();

                //element = wait.Until<IWebElement>((d) =>
                //{
                //    return d.FindElement(By.Id(or.readingXMLFile("ForgottenPassword", "txtEmailAddress", "TVWebPortalOR.xml")));
                //});

                //// Request username / password reset form

                ////new added
                //check from yopmail

                //check from yopmail


                // Registered email address of Individual User

                //IWebElement txtRegEmail = driver.FindElement(By.Id(or.readingXMLFile("ForgottenPassword", "txtEmailAddress", "TVWebPortalOR.xml")));          
                //txtRegEmail.SendKeys("rucha23@yopmail.com");     // (Need to take this value from test data)

                //// Surname

                //IWebElement txtSurname = driver.FindElement(By.Id(or.readingXMLFile("ForgottenPassword", "txtSurname", "TVWebPortalOR.xml")));               
                //txtSurname.SendKeys("Kasar");  // (Need to take this value from test data)

                //// Forgotten my password checkbox

                //IWebElement checkForgotPass = driver.FindElement(By.Id(or.readingXMLFile("ForgottenPassword", "txtForgotPass", "TVWebPortalOR.xml")));      
                //checkForgotPass.Click();  // (Need to take this value from test data)   

                //Thread.Sleep(1000);

                //// Forgotten my username checkbox

                //IWebElement checkForgotUsername = driver.FindElement(By.Id(or.readingXMLFile("ForgottenPassword", "chkForgotUsername", "TVWebPortalOR.xml")));  
                //checkForgotUsername.Click(); // (Need to take this value from test data)

                //Thread.Sleep(1000);

                //IWebElement btnContinue = driver.FindElement(By.Name("submit"));                // Submit button
                //btnContinue.Click();                                                            // Clicking on continue button without entering captcha details

                //string getBrowserURL = driver.Url.ToString();
                //MessageBox.Show(getBrowserURL);


            }
            catch (Exception e)
            {
                log.Error(e.Message + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());
                Assert.AreEqual(true, false);
            }

        }

        public void handlePromotionalPopup()
        {
            IWebElement promotionalPopup = driver.FindElement((or.GetElement("SeriesManagement", "Promotionaldiv", "TVWebPortalOR.xml")));

            String PromoPopup = promotionalPopup.GetCssValue("display").ToString();

            if (PromoPopup.Equals("block"))
            {
                iWait.Until(ExpectedConditions.ElementIsVisible(or.GetElement("SeriesManagement", "Promotionaldiv", "TVWebPortalOR.xml")));

                driver.FindElement((or.GetElement("SeriesManagement", "PromotionalPopup", "TVWebPortalOR.xml"))).Click();

                iWait.Until(ExpectedConditions.InvisibilityOfElementLocated((or.GetElement("SeriesManagement", "EmergencyDiv", "TVWebPortalOR.xml"))));

            }
        }

        public void handleEmergencyPopUp()
        {
            //Handling pop up message
            IWebElement emergencyPopup = driver.FindElement((or.GetElement("SeriesManagement", "EmergencyDiv", "TVWebPortalOR.xml")));

            string emergencyColor = emergencyPopup.GetCssValue("display").ToString();

            if (emergencyColor.Equals("block"))
            {
                iWait.Until(ExpectedConditions.ElementIsVisible(or.GetElement("SeriesManagement", "EmergencyDiv", "TVWebPortalOR.xml")));

                driver.FindElement((or.GetElement("SeriesManagement", "NewEmergencyPopup", "TVWebPortalOR.xml"))).Click();

                iWait.Until(ExpectedConditions.InvisibilityOfElementLocated((or.GetElement("SeriesManagement", "EmergencyDiv", "TVWebPortalOR.xml"))));
            }

        }

        [Test]
        public void TVWeb_002_VerifyPasswordFromEmail()
        {
            try
            {
                log.Info("Verify Forgotten Password Test Started" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

                IWebElement forgotPass, loginText, forgotUsername, btnSubmit, resetPassword;

                //
                //
                handlePromotionalPopup();

                handleEmergencyPopUp();

                driver.FindElement(By.Id("Button1")).Click();

                //IList<IWebElement> videoSearchList = (IList<IWebElement>)driver.FindElements(By.CssSelector("

               // driver.FindElement(By.CssSelector("div.newCustomDrop ul li:nth-child(1) a")).Click();

                Actions action = new Actions(driver);
                IWebElement we = driver.FindElement(By.Id("Button1"));
                action.MoveToElement(we).Perform();


                Thread.Sleep(8000);

                IJavaScriptExecutor executor;

                WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(60));

                wait.Until(ExpectedConditions.ElementExists(By.ClassName(or.readingXMLFile("ForgottenPassword", "overLaySpinner", "TVWebPortalOR.xml"))));

                loginText = driver.FindElement(By.Id(or.readingXMLFile("ForgottenPassword", "login", "TVWebPortalOR.xml"))).FindElement(By.TagName("a"));

                executor = (IJavaScriptExecutor)driver;
                executor.ExecuteScript("arguments[0].click();", loginText);

                element = wait.Until<IWebElement>((d) =>
                {
                    return d.FindElement(By.ClassName(or.readingXMLFile("ForgottenPassword", "checkBox", "TVWebPortalOR.xml"))).FindElement(By.TagName("div"));
                });

                Thread.Sleep(2000);

                forgotPass = driver.FindElement(By.ClassName(or.readingXMLFile("ForgottenPassword", "checkBox", "TVWebPortalOR.xml"))).FindElement(By.TagName("div")).FindElement(By.TagName("a"));

                executor = (IJavaScriptExecutor)driver;
                executor.ExecuteScript("arguments[0].click();", forgotPass);

                forgotUsername = driver.FindElement(By.Id(or.readingXMLFile("ForgottenPassword", "userName", "TVWebPortalOR.xml")));

                forgotUsername.SendKeys("Madhura.Mungekar@yopmail.com");

                string username = forgotUsername.Text.ToString();

                cf.writingIntoXML("Admin", "ForgotPassword", "username", "Madhura.Mungekar@yopmail.com", "TestCopy.xml");

                btnSubmit = driver.FindElement(By.Id(or.readingXMLFile("ForgottenPassword", "Submit", "TVWebPortalOR.xml")));
                btnSubmit.Click();

                Thread.Sleep(2000);

                resetPassword = driver.FindElement(By.Id(or.readingXMLFile("ForgottenPassword", "btnYes", "TVWebPortalOR.xml")));
                resetPassword.Click();

                Thread.Sleep(2000);
                //iWait.Until(ExpectedConditions.ElementExists(By.ClassName(or.readingXMLFile("VideoLandingPage", "InfoMessage", "TVWebPortalOR.xml"))));

                iWait.Until(ExpectedConditions.ElementIsVisible((or.GetElement("VideoLandingPage", "InfoMessage", "TVWebPortalOR.xml"))));

                driver.FindElement((or.GetElement("ReportAbuse", "InfoMessageButton", "TVWebPortalOR.xml"))).Click();

                #region getting password from mail
                //code for getting password from mail
                uf.OpenNewTab(driver);

                //yopmail
                String appURL = cf.readingXMLFile("WebPortal", "Yopmail", "startURL", "Config.xml");

                driver.Navigate().GoToUrl(appURL);

                driver.Manage().Window.Maximize();

                iWait.Until(ExpectedConditions.ElementIsVisible(By.Id("login")));

                IWebElement YopUsername = driver.FindElement(By.Id("login"));
                YopUsername.SendKeys("Madhura.Mungekar@yopmail.com");

                IWebElement submitBtn = driver.FindElement(By.CssSelector("input.sbut"));
                submitBtn.Click();

                Thread.Sleep(150000);
               // 150000

                IWebElement mailFrame = driver.FindElement(By.Id("ifmail"));
                driver.SwitchTo().Frame(mailFrame);

                IWebElement mailText = driver.FindElement(By.Id("mailmillieu"));

                string[] passwordText = mailText.Text.Split(':');

                string finalPassword = passwordText[2].Substring(1, 8);

                #endregion

                cf.writingIntoXML("Admin", "ForgotPassword", "password", finalPassword, "TestCopy.xml");

                String browsertype = uf.getRunningBrowser(driver, driver.FindElement(By.TagName("html")));

                uf.SwitchToWebTab(driver, browsertype);

                uf.NavigateWebPortal(cf, driver);

                log.Info("redirectToLogin ::::");

                iWait.Until(ExpectedConditions.ElementExists((or.GetElement("NonIETMemberRegistration", "LoginLink", "TVWebPortalOR.xml"))));

                IWebElement loginLink = driver.FindElement((or.GetElement("NonIETMemberRegistration", "LoginLink", "TVWebPortalOR.xml")));

                executor.ExecuteScript("arguments[0].click();", loginLink);

                driver.FindElement((or.GetElement("VideoRequestByUser", "UserName", "TVWebPortalOR.xml"))).SendKeys("Madhura.Mungekar@yopmail.com");
                Thread.Sleep(1000);

                driver.FindElement((or.GetElement("VideoRequestByUser", "Password", "TVWebPortalOR.xml"))).SendKeys(cf.readingXMLFile("Admin", "ForgotPassword", "password", "TestCopy.xml"));
                iWait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy((or.GetElement("VideoRequestByUser", "Login", "TVWebPortalOR.xml"))));

                driver.FindElement((or.GetElement("VideoRequestByUser", "Login", "TVWebPortalOR.xml"))).Click();

                Thread.Sleep(2000);

                log.Info("already logged in count  : " + driver.FindElements((or.GetElement("VideoRequestByUser", "YesButton", "TVWebPortalOR.xml"))).Count);

                //if user is already logged in
                if (driver.FindElements((or.GetElement("VideoRequestByUser", "YesButton", "TVWebPortalOR.xml"))).Count > 0)
                {
                    driver.FindElement((or.GetElement("VideoRequestByUser", "YesButton", "TVWebPortalOR.xml"))).Click();
                }

                //Handling pop up message
                welcomeMessage();

                driver.SwitchTo().DefaultContent();
            }
            catch (Exception e)
            {
                log.Error(e.Message + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());
                Assert.AreEqual(true, false);
            }
        }

        public void welcomeMessage()
        {
            //Handling pop up message
            iWait.Until(ExpectedConditions.ElementIsVisible((or.GetElement("VideoManagement", "LoginWelcomeMsg", "TVWebPortalOR.xml"))));

            driver.FindElement((or.GetElement("VideoManagement", "WelcomeMsgCSS", "TVWebPortalOR.xml"))).Click();

            iWait.Until(ExpectedConditions.InvisibilityOfElementLocated((or.GetElement("VideoManagement", "LoginWelcomeMsg", "TVWebPortalOR.xml"))));
        }

        [TestFixtureTearDown]
        public void TearDown()
        {
            st.Chrome_TearDown(driver, log);                                                    // Calling Chrome Teardown
        }
    }

}
