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
using System.Windows.Forms;
using Utility_Classes;
using Utilities.Config;
using System.Threading;
using Microsoft.Office.Interop.Excel;
using System.Drawing;
using Utilities.Object_Repository;
using log4net;
using log4net.Config;
using System.Reflection;
using System.IO;
using System.Diagnostics;
using Microsoft.Expression.Encoder.ScreenCapture;
using IETTVWebportal.Reusable_Functions;
using Sikuli4Net.sikuli_UTIL;
using Sikuli4Net.sikuli_REST;

namespace IETTVWebportal.Chrome
{
    [TestFixture]
    public class Chrome_QAManagementVerification
    {

        #region Variables

        // This is to configure logger mechanism for Utilities.Config
        private static ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        internal IWebDriver driver = null;

        IJavaScriptExecutor executor;

        IWait<IWebDriver> iWait;

        string driverName = "", driverPath, appURL;

        Utility_Functions uf = new Utility_Functions();                      // Instantiate object for Utility Functons

        Configuration cf = new Configuration();                             // Instantiate object for Configuration

        Object_Repository_Class OR = new Object_Repository_Class();        // Instantiate object for object repository

        ScreenCaptureJob job = null;

        Chrome_WebSetupTearDown st = new Chrome_WebSetupTearDown();        // Instantiate object for Chrome Setup Teardown


        #endregion

        #region Constructors

        public Chrome_QAManagementVerification(){}

        public Chrome_QAManagementVerification(IWebDriver driver, ILog log1, IJavaScriptExecutor Executor, IWait<IWebDriver> iWait)
        {
            this.driver = driver;
            log = log1;
            this.executor = Executor;
            this.iWait = iWait;
        }

        #endregion

        #region Setup

        [TestFixtureSetUp]
        public void FixtureSetUp()
        {
            try
            {

                log4net.Config.XmlConfigurator.Configure(new FileInfo(AppDomain.CurrentDomain.BaseDirectory + "WebPortal.config"));

                log.Info("Inside Fixture Setup of chrome - NonIETmemberRegistration Test" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

                uf.CreateOrReplaceVideoFolder();

                string baseDir = AppDomain.CurrentDomain.BaseDirectory;                         // Get path till Base Directory

                driverName = "webdriver.chrome.driver";                                        // Driver name for Chrome

                driverPath = baseDir + "/chromedriver.exe";                                    // Path for ChromeDriver

                System.Environment.SetEnvironmentVariable(driverName, driverPath);

                driver = new ChromeDriver();                                                  // Initialize chrome driver   

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


        #endregion 

        public Boolean IsQAPanelDisplayed()
        {
            return driver.FindElement((OR.GetElement("QnAManagement", "QAPanel", "TVWebPortalOR.xml"))).Displayed;
        }
     
        public Boolean IsChatboxEnabled()
        {

           return Convert.ToBoolean(driver.FindElement((OR.GetElement("QnAManagement", "ChatText", "TVWebPortalOR.xml"))).GetAttribute("disabled"));

        }

       

        public Boolean IsSendButtonEnabled()
        {
            Boolean btnStatus = (Boolean)executor.ExecuteScript(" return document.getElementById('btn_reply').disabled");

            log.Info("Status of element Checked :: " + btnStatus + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

            return btnStatus;
        }

        public void sendMessageToAdmin(String msg)
        {
            iWait.Until(ExpectedConditions.ElementIsVisible((OR.GetElement("QnAManagement", "ChatText", "TVWebPortalOR.xml"))));
            driver.FindElement((OR.GetElement("QnAManagement", "ChatText", "TVWebPortalOR.xml"))).SendKeys(msg);
            Thread.Sleep(1000);
             executor.ExecuteScript("arguments[0].click()",  driver.FindElement((OR.GetElement("QnAManagement", "ReplyButton", "TVWebPortalOR.xml"))));
        }

        public IWebElement panelChatBox()
        {
            iWait.Until(ExpectedConditions.ElementToBeClickable((OR.GetElement("QnAManagement", "ChatBoxPanel", "TVWebPortalOR.xml"))));

            return driver.FindElement((OR.GetElement("QnAManagement", "ChatBoxPanel", "TVWebPortalOR.xml")));
        }

       

        /// <summary>
        /// IsAdmin Parameter specify whose message we are verifying (admin or User)
        /// 
        /// </summary>
        public void VerifyMessage(String message, Boolean isAdmin)
        {
            uf.isJqueryActive(driver);
            Boolean flag = false;
             List<IWebElement> chatItem = null;

            if(isAdmin)
                chatItem = panelChatBox().FindElements((OR.GetElement("QnAManagement", "AdminMessage", "TVWebPortalOR.xml"))).ToList();
            else if(!isAdmin)
                chatItem = panelChatBox().FindElements((OR.GetElement("QnAManagement", "UserMessage", "TVWebPortalOR.xml"))).ToList();

            foreach (IWebElement currentChat in chatItem)
            {
                if (message.Equals(currentChat.FindElement((OR.GetElement("QnAManagement", "ChatItemText", "TVWebPortalOR.xml"))).Text.Trim()))
                {
                    flag = true;
                    break;
                }
            }

            Assert.AreEqual(true, flag); // If fails then chat is not available in chat box
        }

        public void VerifyUserMessageOnWebPortal(String msg)
        {
            VerifyMessage(msg, false);
            
        }

        public void VerifyAdminMessageOnWebPortal(String msg)
        {
            VerifyMessage(msg, true);
        }

    }
}
