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

namespace IETTVWebportal.Firefox
{
    [TestFixture]
    class Firefox_ForgottenPasswordVerification
    {
        internal IWebDriver driver;

        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        
        string driverName = "", appURL;

        IJavaScriptExecutor executor;

        IWait<IWebDriver> iWait;

        Utility_Functions uf = new Utility_Functions();                                         // Instantiate object for Utility Functons

        Configuration cf = new Configuration();                                                // Instantiate object for Configuration

        Object_Repository_Class or = new Object_Repository_Class();                           // Instantiate object for object repository

        Firefox_WebSetupTearDown st = new Firefox_WebSetupTearDown();                        // Instantiate object for Firefox Setup Teardown

        [TestFixtureSetUp]
        public void FixtureSetUp()
        {
            log4net.Config.XmlConfigurator.Configure(new FileInfo(AppDomain.CurrentDomain.BaseDirectory + "WebPortal.config"));

            log.Info("Inside Fixture Setup of Firefox - BottomBar Verification Test" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());
  
            uf.CreateOrReplaceVideoFolder();
            
            driver = new FirefoxDriver();                                                       // Initialize Firefox driver

            iWait = new WebDriverWait(driver, TimeSpan.FromSeconds(60));

            executor = (IJavaScriptExecutor)driver;
        }

        [SetUp]
        public void SetUp()
        {
            appURL = st.Firefox_Setup(driver, log, executor);                                   // Calling Firefox Setup
        }

        [Test]
        public void TVWeb_001_VerifyForgottenPassword()
        {

            try
            {
                log.Info("Verify Forgotten Password Test Started" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

                IWebElement element, forgotPass, loginText, btnCancel, forgotUsername, btnSubmit, resetPassword;

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

                element = wait.Until<IWebElement>((d) =>
                {
                    return d.FindElement(By.Id(or.readingXMLFile("ForgottenPassword", "userName", "TVWebPortalOR.xml")));
                });

                // Clicking on Cancel button and verify if login page is displayed

                btnCancel = driver.FindElement(By.CssSelector(or.readingXMLFile("ForgottenPassword", "Cancel", "TVWebPortalOR.xml")));

                element = wait.Until<IWebElement>((d) =>
                {
                    return d.FindElement(By.CssSelector(or.readingXMLFile("ForgottenPassword", "Cancel", "TVWebPortalOR.xml")));
                });

                btnCancel.Click();

                element = wait.Until<IWebElement>((d) =>
                {
                    return d.FindElement(By.Id(or.readingXMLFile("ForgottenPassword", "aftCancelUsername", "TVWebPortalOR.xml")));
                });

                String loginPageTitle = driver.Url.ToString();

                log.Info("Login Page URL:" + loginPageTitle + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

                Assert.AreEqual(appURL + "/view/LoginNew.html", loginPageTitle);

                Thread.Sleep(2000);

                forgotPass = driver.FindElement(By.ClassName(or.readingXMLFile("ForgottenPassword", "checkBox", "TVWebPortalOR.xml"))).FindElement(By.TagName("div")).FindElement(By.TagName("a"));

                // Clicking on Forgot Password link 

                executor = (IJavaScriptExecutor)driver;
                executor.ExecuteScript("arguments[0].click();", forgotPass);                     

                element = wait.Until<IWebElement>((d) =>
                {
                    return d.FindElement(By.Id(or.readingXMLFile("ForgottenPassword", "userName", "TVWebPortalOR.xml")));
                });

                //  User name of individual user

                forgotUsername = driver.FindElement(By.Id(or.readingXMLFile("ForgottenPassword", "userName", "TVWebPortalOR.xml")));

                // Click on Submit without entering username

                btnSubmit = driver.FindElement(By.Id(or.readingXMLFile("ForgottenPassword", "Submit", "TVWebPortalOR.xml")));                   
                btnSubmit.Click();

                uf.isJqueryActive(driver);

                // Get style attribute value

                String userBorder = forgotUsername.GetAttribute("style").ToString();

                log.Info("userBorder:=" + userBorder + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());


                if (driverName.Equals("webdriver.ie.driver"))
                {
                    // For IE Browser
                    Assert.AreEqual("padding: 2px 1px; border: 1px solid rgb(197, 0, 0);", userBorder);  

                }
                else
                {
                    Assert.AreEqual("border: 1px solid rgb(197, 0, 0); padding: 2px 1px;", userBorder);
                }

                //  Forgot Username (Need to take this value from test data)

                forgotUsername.SendKeys("rucha23");                                                     

                btnSubmit = driver.FindElement(By.Id(or.readingXMLFile("ForgottenPassword", "Submit", "TVWebPortalOR.xml")));
                btnSubmit.Click();

                Thread.Sleep(2000);

                resetPassword = driver.FindElement(By.Id(or.readingXMLFile("ForgottenPassword", "btnYes", "TVWebPortalOR.xml")));
                resetPassword.Click();

                element = wait.Until<IWebElement>((d) =>
                {
                    return d.FindElement(By.Id(or.readingXMLFile("ForgottenPassword", "txtEmailAddress", "TVWebPortalOR.xml")));
                });

                //// Request username / password reset form

                // Registered email address of Individual User

                IWebElement txtRegEmail = driver.FindElement(By.Id(or.readingXMLFile("ForgottenPassword", "txtEmailAddress", "TVWebPortalOR.xml")));            
                txtRegEmail.SendKeys("rucha23@yopmail.com");  // (Need to take this value from test data)

                // Surname

                IWebElement txtSurname = driver.FindElement(By.Id(or.readingXMLFile("ForgottenPassword", "txtSurname", "TVWebPortalOR.xml")));                  
                txtSurname.SendKeys("Kasar");       // (Need to take this value from test data)

                // Forgotten my password checkbox
                
                IWebElement checkForgotPass = driver.FindElement(By.Id(or.readingXMLFile("ForgottenPassword", "txtForgotPass", "TVWebPortalOR.xml")));      
                checkForgotPass.Click();     // (Need to take this value from test data)   

                Thread.Sleep(1000);

                // Forgotten my username checkbox

                IWebElement checkForgotUsername = driver.FindElement(By.Id(or.readingXMLFile("ForgottenPassword", "chkForgotUsername", "TVWebPortalOR.xml")));  
                checkForgotUsername.Click();    // (Need to take this value from test data)

                Thread.Sleep(1000);

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

        [TestFixtureTearDown]
        public void TearDown()
        {
            st.Firefox_TearDown(driver, log);                                                   // Calling Firefox Teardown
        }
    }

}
