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
using IETTVAdminportal.Reusable;
using IETTVWebportal.Chrome;
using log4net;
using log4net.Config;
using System.Reflection;
using System.IO;
using System.Diagnostics;
using Utilities;
using Sikuli4Net.sikuli_UTIL;
using Sikuli4Net.sikuli_REST;
using NSoup.Nodes;

namespace IETTVAdminPortal.Chrome
{

    [TestFixture]
    class Chrome_QAManagement
    {

        // This is to configure logger mechanism for Utilities.Config
        private static ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        #region variable declaration and object initialisation

        IJavaScriptExecutor executor;               

        string driverName = "", driverPath, appURL;

        internal IWebDriver driver = null;

        IWait<IWebDriver> iWait = null;

        //Instantiating Utilities function class

        Utility_Functions uf = new Utility_Functions();                                         // Instantiate object for Utility Functons

        Configuration cf = new Configuration();                                                 // Instantiate object for Configuration

        Object_Repository_Class OR = new Object_Repository_Class();

        Chrome_AdminSetupTearDown st = new Chrome_AdminSetupTearDown();

        Chrome_VideoManagementVerification videoResult;

        Chrome_VideoManagement objAdminVideoManagement = null;
        Chrome_PollManagement objAdminPollManagement = null;
        Chrome_PollManagementVerification objWebPollManagement = null;
        Chrome_QAManagementVerification objWebQAManagement = null;
        Chrome_EventManagement objAdminEventManagement = null;
        Chrome_EventManagementVerification objWebEventtManagement = null;

        #endregion

        #region Constructors

         public Chrome_QAManagement(IWebDriver driver, ILog log1, IJavaScriptExecutor Executor, IWait<IWebDriver> iWait)
        {
            this.driver = driver;
            log = log1;
            this.executor = Executor;
            this.iWait = iWait;
        }

         public Chrome_QAManagement(){ }

        #endregion

        #region Setup


         [TestFixtureSetUp]
         public void FixtureSetUp()
         {
             try
             {
                 log4net.Config.XmlConfigurator.Configure(new FileInfo(AppDomain.CurrentDomain.BaseDirectory + "AdminPortal.config"));     //To configure logger funtionality

                 log.Info("Base Directory Admin :: " + AppDomain.CurrentDomain.BaseDirectory);

                 uf.CreateOrReplaceVideoFolder();

                 string baseDir = AppDomain.CurrentDomain.BaseDirectory;

                 driverName = "webdriver.chrome.driver"; // Driver name for Chrome

                 driverPath = baseDir + "/chromedriver.exe"; // Path for ChromeDriver

                 System.Environment.SetEnvironmentVariable(driverName, driverPath);

                 //Event firing
                 driver = new ChromeDriver(baseDir + "/DLLs", new ChromeOptions(), TimeSpan.FromSeconds(120)); // Initialize chrome driver 
                 EventFire ef = new EventFire(driver);
                 driver = ef; 

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

             videoResult = new Chrome_VideoManagementVerification(driver, log, executor, iWait);            // Creating a object for calling IETTVWebPortal project

             appURL = st.Chrome_Setup(driver, log, executor);                                               // Calling Chrome Setup  
         }

        #endregion

        #region Reusable Elements

          
        public IWebElement optQAManagement()
        {
            iWait.Until(ExpectedConditions.ElementToBeClickable((OR.GetElement("QnAManagement", "QnAManagementLink", "TVAdminPortalOR.xml"))));

            return driver.FindElement((OR.GetElement("QnAManagement", "QnAManagementLink", "TVAdminPortalOR.xml")));
        }

        public IWebElement txtChatBoxContainer()
        {
            iWait.Until(ExpectedConditions.ElementToBeClickable((OR.GetElement("QnAManagement", "TxtChatBox", "TVAdminPortalOR.xml"))));
            return driver.FindElement((OR.GetElement("QnAManagement", "TxtChatBox", "TVAdminPortalOR.xml")));
        }

        public IWebElement txtChat()
        {
            iWait.Until(ExpectedConditions.ElementToBeClickable((OR.GetElement("QnAManagement", "TxtMessage", "TVAdminPortalOR.xml"))));
            return driver.FindElement((OR.GetElement("QnAManagement", "TxtMessage", "TVAdminPortalOR.xml")));

        }

       
           public IWebElement ddlSearchBy()
        {
            iWait.Until(ExpectedConditions.ElementIsVisible((OR.GetElement("QnAManagement", "SearchDropdown", "TVAdminPortalOR.xml"))));
            return driver.FindElement((OR.GetElement("QnAManagement", "SearchDropdown", "TVAdminPortalOR.xml")));

        }

         public IWebElement txtSearch()
           {
               iWait.Until(ExpectedConditions.ElementToBeClickable((OR.GetElement("QnAManagement", "SearchText", "TVAdminPortalOR.xml"))));
               return driver.FindElement((OR.GetElement("QnAManagement", "SearchText", "TVAdminPortalOR.xml")));
           }

           public IWebElement btnSearch()
           {
               iWait.Until(ExpectedConditions.ElementToBeClickable((OR.GetElement("QnAManagement", "SearchButton", "TVAdminPortalOR.xml"))));
               return driver.FindElement((OR.GetElement("QnAManagement", "SearchButton", "TVAdminPortalOR.xml")));
           }
        
        public IWebElement btnReply()
        {
            iWait.Until(ExpectedConditions.ElementToBeClickable((OR.GetElement("QnAManagement", "ReplyButton", "TVAdminPortalOR.xml"))));
            return driver.FindElement((OR.GetElement("QnAManagement", "ReplyButton", "TVAdminPortalOR.xml")));
        }

         #endregion


        #region Reusable Function

         public String CreateFreeLiveVideoWithQA()
         {
             return objAdminPollManagement.CreateFreeLiveVideo(true);
         }

         public void RedirectToQAManagement()
         {
             log.Info("inside redirectToQAManagement " + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

             //clicking on Admin dropdown   
             objAdminVideoManagement.adminDropdown().Click();

             Thread.Sleep(2000);

             //Clicking on video Management Link

             optQAManagement().Click();
         }

         public void SearchVideoUnderQA(String videoName)
         {
             SearchUnderQA(videoName, true);
         }

         public void SearchEventUnderQA(String eventName)
         {
             SearchUnderQA(eventName, false);
         }


         public void SearchUnderQA(String searchText, Boolean isVideo)
         {


             iWait.Until(ExpectedConditions.ElementIsVisible((OR.GetElement("QnAManagement", "EventRadioButton", "TVAdminPortalOR.xml"))));

             if (!isVideo)
                 driver.FindElement((OR.GetElement("QnAManagement", "EventRadioButton", "TVAdminPortalOR.xml"))).Click();
             else if (isVideo)
                 driver.FindElement((OR.GetElement("QnAManagement", "VideoRadioButton", "TVAdminPortalOR.xml"))).Click();

             uf.isJqueryActive(driver);

             new SelectElement(ddlSearchBy()).SelectByText("Title");

             txtSearch().SendKeys(searchText);

             btnSearch().Click();
         }



      

         public void SendMessageToUser(String msg)
         {
             txtChat().SendKeys(msg);
             btnReply().Click();
         }

         public void VerifyUserMessageOnAdmin(String msg)
         {
             objWebQAManagement.VerifyMessage(msg, false);
         }

         public void VerifyAdminMessageOnAdmin(String msg)
         {
             objWebQAManagement.VerifyMessage(msg, true);

         }


         

         #endregion


         // This function will return status of element checked or not
         public Boolean IsElementChecked()
         {

             Boolean checkboxStatus = (Boolean)executor.ExecuteScript(" return document.getElementById('ContentPlaceHolder1_ucAdvancedInformation_chkDisplayQAndA').checked");

             log.Info("Status of element Checked :: " + checkboxStatus + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

             return checkboxStatus;

         }

         [Test]
         public void TVAdmin_001_QAFuncForVideo() 
         {
             try
             {
                 log.Info("TVAdmin_001_QAFuncForVideo Test Started" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

                 objAdminVideoManagement = new Chrome_VideoManagement(driver, log, executor, iWait);
                 objAdminPollManagement = new Chrome_PollManagement(driver, log, executor, iWait);
                 objWebPollManagement = new Chrome_PollManagementVerification(driver, log, executor, iWait);
                 objWebQAManagement = new Chrome_QAManagementVerification(driver, log, executor, iWait);


                 #region Create Live Free Video With QA enable

                 String videoName = CreateFreeLiveVideoWithQA();

                 #endregion

                 RedirectToQAManagement();

                 SearchVideoUnderQA(videoName);

                 #region Selecting Video from Search result

                 iWait.Until(ExpectedConditions.ElementIsVisible((OR.GetElement("QnAManagement", "SearchResult", "TVAdminPortalOR.xml"))));
                 driver.FindElement((OR.GetElement("QnAManagement", "SelectVidRadioBtn", "TVAdminPortalOR.xml"))).Click();
                 driver.FindElement((OR.GetElement("QnAManagement", "SelectVideoPlcHolder", "TVAdminPortalOR.xml"))).Click();
                 iWait.Until(ExpectedConditions.InvisibilityOfElementLocated((OR.GetElement("SeriesManagement", "OverlayWait", "TVAdminPortalOR.xml"))));

                 #endregion 

                 iWait.Until(ExpectedConditions.ElementIsVisible((OR.GetElement("QnAManagement", "TxtChatBox", "TVAdminPortalOR.xml"))));
                 
                 // Verifying Video Name
                 Assert.AreEqual(driver.FindElement((OR.GetElement("QnAManagement", "TitleName", "TVAdminPortalOR.xml"))).Text.Trim(), videoName);

                 #region Open new tab and navigate to web portal

                 uf.OpenNewTab(driver);

                 log.Info("count ::: " + driver.WindowHandles.Count);

                 String browsertype = uf.getRunningBrowser(driver, driver.FindElement(By.TagName("html")));

                 uf.SwitchToWebTab(driver, browsertype);

                 uf.NavigateWebPortal(cf, driver);

                 uf.isJqueryActive(driver);

                 iWait.Until(ExpectedConditions.InvisibilityOfElementLocated((OR.GetElement("SeriesManagement", "OverlayWait", "TVAdminPortalOR.xml"))));

                 #endregion

                 #region Search video and Verify the presence of QA Panel

                 // Search the video on Web Portal
                 objWebPollManagement.Search(videoName, null, true);
                 uf.isJqueryActive(driver);
                 objWebPollManagement.HandlingEmergencyMessage();
                 objWebPollManagement.ClickOnVideo(videoName);
                 uf.isJqueryActive(driver);

                 // Verify that poll panel is displayed
                 Assert.AreEqual(true, objWebQAManagement.IsQAPanelDisplayed());

                 Thread.Sleep(1000);

                 Assert.AreEqual(false, objWebQAManagement.IsChatboxEnabled());

                 //Assert.AreEqual(true, objWebQAManagement.IsSendButtonEnabled());


                 #endregion
      
                 #region Switch to Admin and Stop the chat

                 uf.SwitchToAdminTab(driver, browsertype);

                 // Clicknig on Stop button
                 driver.FindElement((OR.GetElement("QnAManagement", "StopStartButton", "TVAdminPortalOR.xml"))).Click();

                 Thread.Sleep(2000);         

                 executor.ExecuteScript("arguments[0].click()", driver.FindElement((OR.GetElement("QnAManagement", "SuccessMsgOkButton", "TVAdminPortalOR.xml"))));

                 iWait.Until(d => d.FindElement((OR.GetElement("SeriesManagement", "OverlayWait", "TVAdminPortalOR.xml"))).GetAttribute("class").Trim().Equals("overlay"));

                 // Verify the Text of Start button
                 Assert.AreEqual("Start",driver.FindElement((OR.GetElement("QnAManagement", "StopStartButton", "TVAdminPortalOR.xml"))).Text.Trim());

                 #endregion

                 #region Switch to Web portal and Verify the Status of QA panel

                 uf.SwitchToWebTab(driver, browsertype);

                 uf.isJqueryActive(driver);

                 Thread.Sleep(1000);

                 Assert.AreEqual(true, objWebQAManagement.IsChatboxEnabled());

                // Assert.AreEqual(false, objWebQAManagement.IsSendButtonEnabled());


                 #endregion

                 #region Switch to Admin and Start the chat

                 uf.SwitchToAdminTab(driver, browsertype);

                 // Clicknig on start button
                 driver.FindElement((OR.GetElement("QnAManagement", "StopStartButton", "TVAdminPortalOR.xml"))).Click();

                 Thread.Sleep(2000);

                 //iWait.Until(d => d.FindElement(By.Id("Sucess_Message")).Text.Trim().Equals("Q&A started successfully."));

                 //Assert.AreEqual("Q&A started successfully.", driver.FindElement(By.Id("Sucess_Message")).Text.Trim());

                 Thread.Sleep(1000);

                 executor.ExecuteScript("arguments[0].click()", driver.FindElement((OR.GetElement("QnAManagement", "SuccessMsgOkButton", "TVAdminPortalOR.xml"))));

                 iWait.Until(d => d.FindElement((OR.GetElement("SeriesManagement", "OverlayWait", "TVAdminPortalOR.xml"))).GetAttribute("class").Trim().Equals("overlay"));

                   // Verify the Text of stop button
                 Assert.AreEqual("Stop",driver.FindElement((OR.GetElement("QnAManagement", "StopStartButton", "TVAdminPortalOR.xml"))).Text.Trim());


                 #endregion

                 #region Switch to Web portal and Verify the Status of QA panel

                 uf.SwitchToWebTab(driver, browsertype);

                 uf.isJqueryActive(driver);

                 Thread.Sleep(1000);

                 Assert.AreEqual(false, objWebQAManagement.IsChatboxEnabled());

                // Assert.AreEqual(true, objWebQAManagement.IsSendButtonEnabled());


                 #endregion

                 String GUID = uf.getGuid();

                 int subconuter = GUID.Length / 2;

                 String userMessage = cf.readingXMLFile("AdminPortal", "QAManagement", "userMessageForVideo", "Config.xml") + " " + GUID.Substring(0, subconuter - 1) + " " + GUID.Substring(subconuter, subconuter);

                 #region Sending message from User to admin

                 objWebPollManagement.ClickOnLoginLink();

                 objWebPollManagement.Login(cf.readingXMLFile("WebPortal", "CorporateUser", "corpUserName", "Config.xml"), cf.readingXMLFile("WebPortal", "CorporateUser", "corpPassWord", "Config.xml"));

                 uf.isJqueryActive(driver);

                 objWebPollManagement.HandlingWelcomePopup();

                 objWebQAManagement.sendMessageToAdmin(userMessage);

                 #endregion

                 #region Verify User message on Web Portal 

                 objWebQAManagement.VerifyUserMessageOnWebPortal(userMessage);

                 #endregion

                 #region Verify User message on Admin Portal

                 uf.SwitchToAdminTab(driver, browsertype);

                 VerifyUserMessageOnAdmin(userMessage);

                 #endregion

                 #region Sending message from admin to User

                 String adminMessage = cf.readingXMLFile("AdminPortal", "QAManagement", "adminMessageForVideo", "Config.xml") + " " + GUID.Substring(0, subconuter - 1) + " " + GUID.Substring(subconuter, subconuter); 

                 SendMessageToUser(adminMessage);

                 #endregion

                 #region Verify Admin message on Admin Portal

                 VerifyAdminMessageOnAdmin(adminMessage);

                 #endregion

                 #region Verify User message on Web Portal

                 uf.SwitchToWebTab(driver, browsertype);

                 objWebQAManagement.VerifyAdminMessageOnWebPortal(adminMessage);

                 #endregion

                 objWebPollManagement.Logout();

             }
             catch (Exception e)
             {
                 log.Error(e.Message + "\n" + e.StackTrace + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());
                 Console.WriteLine(e.Message + "\n" + e.StackTrace);
                 Assert.AreEqual(true, false);
             }
         }
     
    
         [Test]
         public void TVAdmin_004_QAFuncForEvent()
         {
             try
             {
                 log.Info("TVAdmin_004_QAFuncForEvent test started" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

                 objAdminVideoManagement = new Chrome_VideoManagement(driver, log, executor, iWait);
                 objAdminPollManagement = new Chrome_PollManagement(driver, log, executor, iWait);
                 objWebPollManagement = new Chrome_PollManagementVerification(driver, log, executor, iWait);
                 objWebQAManagement = new Chrome_QAManagementVerification(driver, log, executor, iWait);
                 objWebEventtManagement = new Chrome_EventManagementVerification(driver, log, executor, iWait);
                 objAdminEventManagement = new Chrome_EventManagement(driver, log, executor, iWait);

                 #region Create Event / Free Live Video  With QA enable

                 String videoName = CreateFreeLiveVideoWithQA();
                 String eventName = objAdminEventManagement.CreateEventWithQA(videoName);
                 log.Info("Event name  :::: " + eventName);
                 #endregion

                 RedirectToQAManagement();

                 SearchEventUnderQA(eventName);

                 #region Selecting Event from Search result

                 iWait.Until(ExpectedConditions.ElementIsVisible((OR.GetElement("QnAManagement", "SearchResult", "TVAdminPortalOR.xml"))));
                 driver.FindElement((OR.GetElement("QnAManagement", "SelectVidRadioBtn", "TVAdminPortalOR.xml"))).Click();
                 driver.FindElement((OR.GetElement("QnAManagement", "SelectVideoPlcHolder", "TVAdminPortalOR.xml"))).Click();
                 iWait.Until(ExpectedConditions.InvisibilityOfElementLocated((OR.GetElement("SeriesManagement", "OverlayWait", "TVAdminPortalOR.xml"))));

                 #endregion

                 iWait.Until(ExpectedConditions.ElementIsVisible((OR.GetElement("QnAManagement", "TxtChatBox", "TVAdminPortalOR.xml"))));

                 // Verifying Event Name
                 Assert.AreEqual(driver.FindElement((OR.GetElement("QnAManagement", "TitleName", "TVAdminPortalOR.xml"))).Text.Trim(), eventName);

                 #region Open new tab and navigate to web portal

                 uf.OpenNewTab(driver);

                 log.Info("count ::: " + driver.WindowHandles.Count);

                 String browsertype = uf.getRunningBrowser(driver, driver.FindElement(By.TagName("html")));

                 uf.SwitchToWebTab(driver, browsertype);

                 uf.NavigateWebPortal(cf, driver);

                 uf.isJqueryActive(driver);

                 iWait.Until(ExpectedConditions.InvisibilityOfElementLocated((OR.GetElement("SeriesManagement", "OverlayWait", "TVAdminPortalOR.xml"))));

                 #endregion

                 #region Search Event On Web Portal

                 // Search the video on Web Portal
                 objWebPollManagement.Search(eventName, null, false);
                 uf.isJqueryActive(driver);
                 objWebPollManagement.HandlingEmergencyMessage();
                 objWebPollManagement.ClickOnEvent(eventName);
                 uf.isJqueryActive(driver);
    
                #endregion

                 #region Click on Live Video present in Event

                 objWebEventtManagement.SelectVideoFromVideoSheduleSection(videoName);

                 uf.isJqueryActive(driver);

                 // Verify that poll panel is displayed
                 Assert.AreEqual(true, objWebQAManagement.IsQAPanelDisplayed());

                 Thread.Sleep(1000);

                 Assert.AreEqual(false, objWebQAManagement.IsChatboxEnabled()); // true Means QA panel is disable.

                 //Assert.AreEqual(true, objWebQAManagement.IsSendButtonEnabled());


                 #endregion

                 #region Switch to Admin and Stop the chat

                 uf.SwitchToAdminTab(driver, browsertype);

                 // Clicknig on Stop button
                 driver.FindElement((OR.GetElement("QnAManagement", "StopStartButton", "TVAdminPortalOR.xml"))).Click();

                 Thread.Sleep(2000);

                 //iWait.Until(d => d.FindElement(By.Id("Sucess_Message")).Text.Trim().Equals("Q&A has been closed successfully."));

                 // Assert.AreEqual("Q&A has been closed successfully.", driver.FindElement(By.Id("Sucess_Message")).Text.Trim());



                 executor.ExecuteScript("arguments[0].click()", driver.FindElement((OR.GetElement("QnAManagement", "SuccessMsgOkButton", "TVAdminPortalOR.xml"))));

                 iWait.Until(d => d.FindElement((OR.GetElement("SeriesManagement", "OverlayWait", "TVAdminPortalOR.xml"))).GetAttribute("class").Trim().Equals("overlay"));

                 // Verify the Text of Start button
                 Assert.AreEqual("Start", driver.FindElement((OR.GetElement("QnAManagement", "StopStartButton", "TVAdminPortalOR.xml"))).Text.Trim());

                 #endregion

                 #region Switch to Web portal and Verify the Status of QA panel

                 uf.SwitchToWebTab(driver, browsertype);

                 uf.isJqueryActive(driver);

                 Thread.Sleep(1000);

                 Assert.AreEqual(true, objWebQAManagement.IsChatboxEnabled());

                 // Assert.AreEqual(false, objWebQAManagement.IsSendButtonEnabled());


                 #endregion

                 #region Switch to Admin and Start the chat

                 uf.SwitchToAdminTab(driver, browsertype);

                 // Clicknig on start button
                 driver.FindElement((OR.GetElement("QnAManagement", "StopStartButton", "TVAdminPortalOR.xml"))).Click();

                 Thread.Sleep(2000);

                 //iWait.Until(d => d.FindElement(By.Id("Sucess_Message")).Text.Trim().Equals("Q&A started successfully."));

                 //Assert.AreEqual("Q&A started successfully.", driver.FindElement(By.Id("Sucess_Message")).Text.Trim());

                 Thread.Sleep(1000);

                 executor.ExecuteScript("arguments[0].click()", driver.FindElement((OR.GetElement("QnAManagement", "SuccessMsgOkButton", "TVAdminPortalOR.xml"))));

                 iWait.Until(d => d.FindElement((OR.GetElement("SeriesManagement", "OverlayWait", "TVAdminPortalOR.xml"))).GetAttribute("class").Trim().Equals("overlay"));

                 // Verify the Text of stop button
                 Assert.AreEqual("Stop", driver.FindElement((OR.GetElement("QnAManagement", "StopStartButton", "TVAdminPortalOR.xml"))).Text.Trim());


                 #endregion

                 #region Switch to Web portal and Verify the Status of QA panel

                 uf.SwitchToWebTab(driver, browsertype);

                 uf.isJqueryActive(driver);

                 Thread.Sleep(1000);

                 Assert.AreEqual(false, objWebQAManagement.IsChatboxEnabled());

                 // Assert.AreEqual(true, objWebQAManagement.IsSendButtonEnabled());


                 #endregion

                 String GUID = uf.getGuid();

                 int subconuter = GUID.Length / 2;

                 String userMessage = cf.readingXMLFile("AdminPortal", "QAManagement", "userMessageForEvent", "Config.xml") + " " + GUID.Substring(0, subconuter - 1) + " " + GUID.Substring(subconuter, subconuter);

                 #region Sending message from User to admin

                 objWebPollManagement.ClickOnLoginLink();

                 objWebPollManagement.Login(cf.readingXMLFile("WebPortal", "CorporateUser", "corpUserName", "Config.xml"), cf.readingXMLFile("WebPortal", "CorporateUser", "corpPassWord", "Config.xml"));

                 uf.isJqueryActive(driver);

                 objWebPollManagement.HandlingWelcomePopup();

                 objWebQAManagement.sendMessageToAdmin(userMessage);

                 #endregion

                 #region Verify User message on Web Portal

                 objWebQAManagement.VerifyUserMessageOnWebPortal(userMessage);

                 #endregion

                 #region Verify User message on Admin Portal

                 uf.SwitchToAdminTab(driver, browsertype);

                 VerifyUserMessageOnAdmin(userMessage);

                 #endregion

                 #region Sending message from admin to User

                 String adminMessage = cf.readingXMLFile("AdminPortal", "QAManagement", "adminMessageForEvent", "Config.xml") + " " + GUID.Substring(0, subconuter - 1) + " " + GUID.Substring(subconuter, subconuter);

                 SendMessageToUser(adminMessage);

                 #endregion

                 #region Verify Admin message on Admin Portal

                 VerifyAdminMessageOnAdmin(adminMessage);

                 #endregion

                 #region Verify User message on Web Portal

                 uf.SwitchToWebTab(driver, browsertype);

                 objWebQAManagement.VerifyAdminMessageOnWebPortal(adminMessage);

                 #endregion

                 objWebPollManagement.Logout();


             }
             catch (Exception e)
             {
                 log.Error(e.Message + "\n" + e.StackTrace + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());
                 Console.WriteLine(e.Message + "\n" + e.StackTrace);
                 Assert.AreEqual(true, false);
             }
         }

         [Test]
         public void TVAdmin_002_DisplayQAFuncForVideo()
         {
             try
             {
                 log.Info("TVAdmin_002_DisplayQAFuncForVideo Test Started" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

                 objAdminVideoManagement = new Chrome_VideoManagement(driver, log, executor, iWait);
                 objAdminPollManagement = new Chrome_PollManagement(driver, log, executor, iWait);
                 objWebPollManagement = new Chrome_PollManagementVerification(driver, log, executor, iWait);
                 objWebQAManagement = new Chrome_QAManagementVerification(driver, log, executor, iWait);


                 #region Create Live Free Video With QA enable

                 log.Info("\nCreate Live Free Video With QA enable" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());


                 //String videoName = cf.readingXMLFile("AdminPortal", "QAManagement", "videoName", "Config.xml");
                 String videoName = CreateFreeLiveVideoWithQA();

                 #endregion

                 #region Search Video on Admin-Video Management and Uncheck the Display QA

                 log.Info("Search Video on Admin-Video Management and Uncheck the Display QA" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

                 //Redirect to Video Management
                 objAdminPollManagement.RedirectToVideoManagement();

                 // Search the Video
                 objAdminPollManagement.txtSearchVideoManagement().SendKeys(videoName);

                 objAdminPollManagement.btnSearchVideoManagement().Click();

                 iWait.Until(ExpectedConditions.InvisibilityOfElementLocated((OR.GetElement("SeriesManagement", "OverlayWait", "TVAdminPortalOR.xml"))));

                 iWait.Until(ExpectedConditions.ElementIsVisible((OR.GetElement("QnAManagement", "VideoListSearch", "TVAdminPortalOR.xml"))));


                 //Thread.Sleep(1000);

                 #region Select Required Video from search result

                 //Using Nsoup here to parse the html table
                 Document doc = NSoup.NSoupClient.Parse(driver.PageSource);
                 NSoup.Select.Elements rowListNsoup = doc.GetElementById(OR.readingXMLFile("QnAManagement", "VideoListSearch", "TVAdminPortalOR.xml")).GetElementsByTag("tr");

                 int rowCounter = 0;
                 Boolean flag = false;
                 String videoTitle = null;

                 foreach (Element currentRow in rowListNsoup)
                 {
                     Attributes attr = currentRow.Attributes;

                     //Row that have class="GridRowStyle" or class="AltGridStyle"
                     if (attr["class"].Equals("GridRowStyle") || attr["class"].Equals("AltGridStyle"))
                     {
                         log.Info("Row Counter :: " + rowCounter);
                         videoTitle = driver.FindElement(OR.GetElement("QnAManagement", "VideoListResult", "TVAdminPortalOR.xml",rowCounter)).GetAttribute("title").Trim();


                         if (videoTitle.ToLower().Equals(videoName.ToLower()))
                         {
                             //Clicknig on Edit button
                             driver.FindElement(OR.GetElement("QnAManagement", "VideoEditButton", "TVAdminPortalOR.xml",rowCounter)).Click();

                             uf.isJqueryActive(driver);

                         }
                         rowCounter++;
                     }
                 }

                 #endregion

                 // Uncheck the Display QnA under Advance
                 objAdminVideoManagement.advanceTab().Click();

                 objAdminVideoManagement.tabPermission().Click();

                 if(IsElementChecked())
                     objAdminVideoManagement.chkDisplayQA().Click();

                 objAdminVideoManagement.publishTab().Click();

                 objAdminVideoManagement.videoPublishButton().Click();

                 iWait.Until(ExpectedConditions.ElementIsVisible((OR.GetElement("VideoManagement", "SuccessBTN", "TVAdminPortalOR.xml"))));

                 driver.FindElement((OR.GetElement("VideoManagement", "SuccessBTN", "TVAdminPortalOR.xml"))).Click();

                 iWait.Until(ExpectedConditions.InvisibilityOfElementLocated((OR.GetElement("VideoManagement", "SuccessBTN", "TVAdminPortalOR.xml"))));


                 #endregion

                 #region Open a new Tab and redirect to Web Portal

                 log.Info("Open a new Tab and redirect to Web Portal" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

                 uf.OpenNewTab(driver);

                 log.Info("count ::: " + driver.WindowHandles.Count);

                 String browsertype = uf.getRunningBrowser(driver, driver.FindElement(By.TagName("html")));

                 uf.SwitchToWebTab(driver, browsertype);

                 uf.NavigateWebPortal(cf, driver);

                 uf.isJqueryActive(driver);

                 iWait.Until(ExpectedConditions.InvisibilityOfElementLocated((OR.GetElement("SeriesManagement", "OverlayWait", "TVAdminPortalOR.xml"))));

                 #endregion

                 #region Search video and Verify the presence of QA Panel on Web Portal

                 log.Info("Search video and Verify the presence of QA Panel on Web Portal" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

                 // Search the video on Web Portal
                 objWebPollManagement.Search(videoName, null, true);
                 uf.isJqueryActive(driver);
                 objWebPollManagement.HandlingEmergencyMessage();
                 objWebPollManagement.ClickOnVideo(videoName);
                 uf.isJqueryActive(driver);

                 // Verify that poll panel is displayed
                 Assert.AreEqual(false, objWebQAManagement.IsQAPanelDisplayed());

                 #endregion

                 #region Switch to Admin Portal and Uncheck the Display QA

                 log.Info("Switch to Admin Portal and check the Display QA" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

                 uf.SwitchToAdminTab(driver, browsertype);

                 // Search the Video
                 objAdminPollManagement.txtSearchVideoManagement().SendKeys(videoName);

                 objAdminPollManagement.btnSearchVideoManagement().Click();

                 iWait.Until(ExpectedConditions.InvisibilityOfElementLocated((OR.GetElement("SeriesManagement", "OverlayWait", "TVAdminPortalOR.xml"))));

                 iWait.Until(ExpectedConditions.ElementIsVisible((OR.GetElement("QnAManagement", "VideoListSearch", "TVAdminPortalOR.xml"))));


                 //Thread.Sleep(1000);

                 #region Select Required Video from search result

                 //Using Nsoup here to parse the html table
                 doc = NSoup.NSoupClient.Parse(driver.PageSource);
                  rowListNsoup = doc.GetElementById(OR.readingXMLFile("QnAManagement", "VideoListSearch", "TVAdminPortalOR.xml")).GetElementsByTag("tr");

                  rowCounter = 0;
                  flag = false;
                  videoTitle = null;

                 foreach (Element currentRow in rowListNsoup)
                 {
                     Attributes attr = currentRow.Attributes;

                     //Row that have class="GridRowStyle" or class="AltGridStyle"
                     if (attr["class"].Equals("GridRowStyle") || attr["class"].Equals("AltGridStyle"))
                     {
                         log.Info("Row Counter :: " + rowCounter);
                         videoTitle = driver.FindElement(OR.GetElement("QnAManagement", "VideoListResult", "TVAdminPortalOR.xml",rowCounter)).GetAttribute("title").Trim();


                         if (videoTitle.ToLower().Equals(videoName.ToLower()))
                         {
                             //Clicknig on Edit button
                             driver.FindElement(OR.GetElement("QnAManagement", "VideoEditButton", "TVAdminPortalOR.xml",rowCounter)).Click();

                             uf.isJqueryActive(driver);

                         }
                         rowCounter++;
                     }
                 }

                 #endregion

                 // Uncheck the Display Polling under Advance
                 objAdminVideoManagement.advanceTab().Click();

                 objAdminVideoManagement.tabPermission().Click();

                 objAdminVideoManagement.chkDisplayQA().Click();

                 objAdminVideoManagement.publishTab().Click();

                 objAdminVideoManagement.videoPublishButton().Click();

                 iWait.Until(ExpectedConditions.ElementIsVisible((OR.GetElement("VideoManagement", "SuccessBTN", "TVAdminPortalOR.xml"))));

                 driver.FindElement((OR.GetElement("VideoManagement", "SuccessBTN", "TVAdminPortalOR.xml"))).Click();

                 iWait.Until(ExpectedConditions.InvisibilityOfElementLocated((OR.GetElement("VideoManagement", "SuccessBTN", "TVAdminPortalOR.xml"))));

                 #endregion

                 #region Switch to Web Portal and verify the QA panel

                 log.Info("Switch to Web Portal and verify the QA panel" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

                 uf.SwitchToWebTab(driver, browsertype);

                 driver.Navigate().Refresh();

                 uf.isJqueryActive(driver);

                 // Verify that QnA panel is displayed
                 Assert.AreEqual(true, objWebQAManagement.IsQAPanelDisplayed());


                 #endregion

                 log.Info("\nTVAdmin_002_DisplayQAFuncForVideo Test Completed" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());


             }
             catch (Exception e)
             {
                 log.Error(e.Message + "\n" + e.StackTrace + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());
                 Console.WriteLine(e.Message + "\n" + e.StackTrace);
                 Assert.AreEqual(true, false);
             }
         }


        /// <summary>
        /// For a time being we are using already created event.
        /// After creating Event Management module we will create a new event and then use the same.
        /// </summary>
         [Test]
         public void TVAdmin_003_DisplayQAFuncForEvent()
         {
             try
             {
                 log.Info("TVAdmin_003_DisplayQAFuncForEvent Test Started" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

                 objAdminVideoManagement = new Chrome_VideoManagement(driver, log, executor, iWait);
                 objAdminPollManagement = new Chrome_PollManagement(driver, log, executor, iWait);
                 objWebPollManagement = new Chrome_PollManagementVerification(driver, log, executor, iWait);
                 objWebQAManagement = new Chrome_QAManagementVerification(driver, log, executor, iWait);
                 objAdminEventManagement = new Chrome_EventManagement(driver, log, executor, iWait);

                 #region Create Free Event With QA enable

                 log.Info("\nCreate Free Event With QA enable" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

                 String videoName = CreateFreeLiveVideoWithQA();
                 String eventName = objAdminEventManagement.CreateEventWithQA(videoName);

                 #endregion

                 #region Search Event on Admin-Event Management and Uncheck the Display QA

                 log.Info("Search Event on Admin-Event Management and Uncheck the Display QA" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

                 //Redirect to Event Management
                 objAdminEventManagement.RedirectToEventManagement();

                 // Search the Event
                 objAdminEventManagement.SearchEvent(eventName, "Title");

                 iWait.Until(ExpectedConditions.InvisibilityOfElementLocated((OR.GetElement("SeriesManagement", "OverlayWait", "TVAdminPortalOR.xml"))));

                 Boolean flag =  objAdminEventManagement.SelectEventFromSearchResult(eventName);
                 Assert.AreEqual(true, flag); // Event is not present in Search result.
            

                  
                 // Uncheck the Display QnA under Permission Tab and publishing the Event
                 objAdminEventManagement.tabPermission().Click();

                 if ((Boolean)executor.ExecuteScript(" return document.getElementById('ContentPlaceHolder1_chkDisplayQA').checked"))
                     objAdminEventManagement.chkDisplayQA().Click();

                 
                 
                 objAdminEventManagement.btnPublish().Click();
                 iWait.Until(ExpectedConditions.ElementIsVisible((OR.GetElement("VideoManagement", "SuccessBTN", "TVAdminPortalOR.xml"))));
                 driver.FindElement((OR.GetElement("VideoManagement", "SuccessBTN", "TVAdminPortalOR.xml"))).Click();
                 iWait.Until(ExpectedConditions.InvisibilityOfElementLocated(OR.GetElement("VideoManagement", "SuccessBTN", "TVAdminPortalOR.xml")));


                 #endregion

                 #region Open a new Tab and redirect to Web Portal

                 log.Info("Open a new Tab and redirect to Web Portal" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

                 uf.OpenNewTab(driver);

                 log.Info("count ::: " + driver.WindowHandles.Count);

                 String browsertype = uf.getRunningBrowser(driver, driver.FindElement(By.TagName("html")));

                 uf.SwitchToWebTab(driver, browsertype);

                 uf.NavigateWebPortal(cf, driver);

                 uf.isJqueryActive(driver);

                 iWait.Until(ExpectedConditions.InvisibilityOfElementLocated((OR.GetElement("SeriesManagement", "OverlayWait", "TVAdminPortalOR.xml"))));

                 #endregion

                 #region Search Event and Verify the presence of QA Panel on Web Portal

                 log.Info("Search event and Verify the presence of QA Panel on Web Portal" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

                 // Search the video on Web Portal
                 objWebPollManagement.Search(eventName, null, true);
                 uf.isJqueryActive(driver);
                 objWebPollManagement.HandlingEmergencyMessage();
                 objWebPollManagement.ClickOnEvent(eventName);
                 uf.isJqueryActive(driver);

                 // Verify that poll panel is displayed
                 Assert.AreEqual(false, objWebQAManagement.IsQAPanelDisplayed());

                 #endregion

                 #region Switch to Admin Portal and Uncheck the Display QA

                 log.Info("Switch to Admin Portal and check the Display QA checkbox" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

                 uf.SwitchToAdminTab(driver, browsertype);

                 //Redirect to Event Management
                 objAdminEventManagement.RedirectToEventManagement();

                 // Search the Event
                 objAdminEventManagement.SearchEvent(eventName, "Title");

                 iWait.Until(ExpectedConditions.InvisibilityOfElementLocated((OR.GetElement("SeriesManagement", "OverlayWait", "TVAdminPortalOR.xml"))));

                 flag = objAdminEventManagement.SelectEventFromSearchResult(eventName);
                 Assert.AreEqual(true, flag); // Event is not present in Search result.

                 // Uncheck the Display QnA under Permission Tab and publishing the Event
                 objAdminEventManagement.tabPermission().Click();
                 objAdminEventManagement.chkDisplayQA().Click();
                 objAdminEventManagement.btnPublish().Click();
                 iWait.Until(ExpectedConditions.ElementIsVisible(OR.GetElement("VideoManagement", "SuccessBTN", "TVAdminPortalOR.xml")));
                 driver.FindElement(OR.GetElement("VideoManagement", "SuccessBTN", "TVAdminPortalOR.xml")).Click();
                 iWait.Until(ExpectedConditions.InvisibilityOfElementLocated(OR.GetElement("VideoManagement", "SuccessBTN", "TVAdminPortalOR.xml")));

                 #endregion

                 #region Switch to Web Portal and verify the QA panel

                 log.Info("Switch to Web Portal and verify the QA panel" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

                 uf.SwitchToWebTab(driver, browsertype);

                 driver.Navigate().Refresh();

                 uf.isJqueryActive(driver);

                 // Verify that poll panel is displayed
                 Assert.AreEqual(true, objWebQAManagement.IsQAPanelDisplayed());


                 #endregion

                 log.Info("\nTVAdmin_003_DisplayQAFuncForEvent Test Completed" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());


             }
             catch (Exception e)
             {
                 log.Error(e.Message + "\n" + e.StackTrace + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());
                 Console.WriteLine(e.Message + "\n" + e.StackTrace);
                 Assert.AreEqual(true, false);
             }
         }

         [TearDown]
         public void TearDown()
         {
             try
             {
                 if (TestContext.CurrentContext.Result.Status.ToString().Equals("Failed"))
                 {
                     st.Chrome_SetUpTearDowm(driver, log,true);
                     Thread.Sleep(1000);
                 }
                 st.Chrome_TearDown(driver, log);

             }
             catch (Exception e)
             {
                 log.Error(e.Message + "\n" + e.StackTrace);
                 Assert.AreEqual(true, false);
             }

         }
    }
}
