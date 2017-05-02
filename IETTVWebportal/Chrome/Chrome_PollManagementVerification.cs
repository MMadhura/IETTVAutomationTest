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
    public class Chrome_PollManagementVerification
    {

        #region Global Variable Declaration

        // This is to configure logger mechanism for Utilities.Config
        private  ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

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

        #region SetUp

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

                driver = new ChromeDriver(baseDir + "/DLLs", new ChromeOptions(), TimeSpan.FromSeconds(120));         // Initialize chrome driver   

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

        #region Constructors

        public Chrome_PollManagementVerification()
        {

        }

        public Chrome_PollManagementVerification(IWebDriver driver, log4net.ILog log1, IJavaScriptExecutor Executor, IWait<IWebDriver> iWait)
        {
            this.driver = driver;

            log = log1;

            this.executor = Executor;

            this.iWait = iWait;

        }

        #endregion


        #region Resuable Elements

        public IWebElement txtSearch()
        {
            iWait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy((OR.GetElement("SeriesManagement", "SearchTB", "TVWebPortalOR.xml"))));
            return driver.FindElement((OR.GetElement("SeriesManagement", "SearchTB", "TVWebPortalOR.xml")));
        }

        public IWebElement btnSearch()
        {
            iWait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy((OR.GetElement("VideoLandingPage", "SearchIcon", "TVWebPortalOR.xml"))));
            return driver.FindElement((OR.GetElement("VideoLandingPage", "SearchIcon", "TVWebPortalOR.xml")));
        }

        public IWebElement resultPanel()
        {
            iWait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy((OR.GetElement("PollManagement", "ResultContainer", "TVWebPortalOR.xml"))));
            return driver.FindElement((OR.GetElement("PollManagement", "ResultContainer", "TVWebPortalOR.xml")));
        }

        public IWebElement lblResponse()
        {
            iWait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy((OR.GetElement("PollManagement", "PollTotalResponses", "TVWebPortalOR.xml"))));
            return driver.FindElement((OR.GetElement("PollManagement", "PollTotalResponses", "TVWebPortalOR.xml")));
        }

        public IWebElement lblAnswer(int optionNumber)
        {
            return driver.FindElement(OR.GetElement("PollManagement", "LblAnswer", "TVWebPortalOR.xml",optionNumber));
        }

        public IWebElement lblPercentResult(int optionNumber)
        {
            return driver.FindElement(OR.GetElement("PollManagement", "PollResultBorder", "TVWebPortalOR.xml",optionNumber));
        }


        #endregion


       

        public void Search(String videoName, String searchBy, Boolean isVideo)
        {

            uf.isJqueryActive(driver);
            
            Boolean flag = false;

            txtSearch().SendKeys(videoName);

            btnSearch().Click();

            uf.isJqueryActive(driver);
        }

        public void HandlingEmergencyMessage()
        {
              //Handling pop up message
            iWait.Until(ExpectedConditions.ElementIsVisible((OR.GetElement("SeriesManagement", "EmergencyDiv", "TVWebPortalOR.xml"))));

            driver.FindElement((OR.GetElement("SeriesManagement", "EmergencyPopup", "TVWebPortalOR.xml"))).Click();

            iWait.Until(ExpectedConditions.InvisibilityOfElementLocated((OR.GetElement("SeriesManagement", "EmergencyDiv", "TVWebPortalOR.xml"))));

            uf.isJqueryActive(driver);

        }

        public void ClickOnVideoOREvent(String element, Boolean isVideo)
        {
             //verifying the search result
            IList<IWebElement> videoSearchList = (IList<IWebElement>)driver.FindElement((OR.GetElement("BuyVideoVerification", "SearchResult", "TVWebPortalOR.xml"))).FindElements((OR.GetElement("VideoLandingPage", "SearchResultRecord", "TVWebPortalOR.xml")));


            if (isVideo)
            {

                //gettting the search result details
                foreach (IWebElement currentSearchRecord in videoSearchList)
                {
                    IWebElement searchResultDetails = currentSearchRecord.FindElement((OR.GetElement("PollManagement", "VidDesc", "TVWebPortalOR.xml"))).
                                                      FindElement(By.TagName("span")).FindElement(By.TagName("a"));


                    searchResultDetails.Click();

                    // String webvideoTitle = searchresultDetails.Text.Trim();

                    ////getting video Title from search result
                    //if (webvideoTitle.Equals(videoName))
                    //{
                    //    flag = true;

                    //    String videoID_Web = searchresultDetails.GetAttribute("data-videono");

                    //    String guid_Web = searchresultDetails.GetAttribute("data-videoid");

                    //    cf.writingIntoXML("AdminPortal", "VideoManagement", "VideoID", videoID_Web, "SysConfig.xml");

                    //    IWebElement serachResultLink = driver.FindElement((OR.GetElement("VideoLandingPage", "SearchResultLink", "TVWebPortalOR.xml")));

                    //    log.Info("Web Video Title:" + webvideoTitle + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

                    //    serachResultLink.Click();

                    //    uf.isJqueryActive(driver);

                    //    break;
                    //}
                }
            }
            else if (!isVideo)
            {
                driver.FindElement(By.LinkText(element)).Click();
                uf.isJqueryActive(driver);
            }

        }

        public void ClickOnVideo(String videoName)
        {   
            ClickOnVideoOREvent(videoName, true);
        }

        public void ClickOnEvent(String eventName)
        {
            ClickOnVideoOREvent(eventName, false);
        }

        public Boolean IsPollPanelDisplayed()
        {
            return driver.FindElement((OR.GetElement("VideoManagement", "QnADisplayed", "TVWebPortalOR.xml"))).Displayed;
        }

        public void VerifyingPollDetails(Dictionary<String, String> pollDetails)
        {

            Assert.AreEqual(pollDetails["question"], driver.FindElement((OR.GetElement("PollManagement", "QuestionDiv", "TVWebPortalOR.xml"))).Text.Trim());


            for (int i = 1; i < pollDetails.Count; i++)
            {
                Assert.AreEqual(pollDetails["option" + i], driver.FindElement(OR.GetElement("PollManagement", "PollQuestionText", "TVWebPortalOR.xml",(i - 1))).Text.Trim());
            }
               

        }


        public void SelectOption(int optionNumber)
        {

            optionNumber--;

            driver.FindElement(OR.GetElement("PollManagement", "PollOption", "TVWebPortalOR.xml",optionNumber)).Click();

            driver.FindElement((OR.GetElement("PollManagement", "PollSubmitBTN", "TVWebPortalOR.xml"))).Click();
        }

        public void HandlingWarningMessage(String message)
        {
            iWait.Until(ExpectedConditions.ElementIsVisible((OR.GetElement("VideoRequestByUser", "BannerMessageOkButton", "TVWebPortalOR.xml"))));

            Assert.AreEqual(message, driver.FindElement((OR.GetElement("PollManagement", "Warningmsg", "TVWebPortalOR.xml"))).Text.Trim());

            driver.FindElement((OR.GetElement("VideoRequestByUser", "BannerMessageOkButton", "TVWebPortalOR.xml"))).Click();

            iWait.Until(ExpectedConditions.InvisibilityOfElementLocated((OR.GetElement("VideoRequestByUser", "BannerMessageOkButton", "TVWebPortalOR.xml"))));
        }

        public void ClickOnLoginLink()
        {
            //clicking on Login button
            iWait.Until(ExpectedConditions.ElementToBeClickable((OR.GetElement("NonIETMemberRegistration", "LoginLink", "TVWebPortalOR.xml"))));
            //element = driver.FindElement(By.LinkText("Please Log in"));
            IWebElement lnkLogin = driver.FindElement((OR.GetElement("NonIETMemberRegistration", "LoginLink", "TVWebPortalOR.xml")));
            executor.ExecuteScript("arguments[0].click();", lnkLogin);
        }

        public void Login(String username, String password)
        {

           //Waiting untill please wait text is displayed
            iWait.Until(ExpectedConditions.InvisibilityOfElementLocated((OR.GetElement("PollManagement", "PleaseWaitLoader", "TVWebPortalOR.xml"))));

            iWait.Until(ExpectedConditions.ElementExists((OR.GetElement("NonIETMemberRegistration", "UserName", "TVWebPortalOR.xml"))));

            // Entering Username
            driver.FindElement((OR.GetElement("NonIETMemberRegistration", "UserName", "TVWebPortalOR.xml"))).SendKeys(username);

            //Entering password
            driver.FindElement((OR.GetElement("NonIETMemberRegistration", "Password", "TVWebPortalOR.xml"))).SendKeys(password);

            Thread.Sleep(1000);

            // Clicking on Login button
            driver.FindElement((OR.GetElement("NonIETMemberRegistration", "LoginButton", "TVWebPortalOR.xml"))).Click();


            log.Info("already logged in count  : " + driver.FindElements((OR.GetElement("VideoRequestByUser", "YesButton", "TVWebPortalOR.xml"))).Count);

            Console.WriteLine("already logged in count  : " + driver.FindElements((OR.GetElement("VideoRequestByUser", "YesButton", "TVWebPortalOR.xml"))).Count);

            Thread.Sleep(2000);

            //DefaultWait<IWebDriver> wait = new DefaultWait<IWebDriver>(driver);
            //wait.Timeout = TimeSpan.FromSeconds(5);
            //wait.PollingInterval = TimeSpan.FromSeconds(1);
            //wait.IgnoreExceptionTypes(typeof(NoSuchElementException));


            //wait.Until(d => d.FindElements((OR.GetElement("VideoRequestByUser", "YesButton", "TVWebPortalOR.xml"))).Count != 0);
              Console.WriteLine("already logged in count  : " + driver.FindElements((OR.GetElement("VideoRequestByUser", "YesButton", "TVWebPortalOR.xml"))).Count);


            //if user is already logged in
            if (driver.FindElements((OR.GetElement("VideoRequestByUser", "YesButton", "TVWebPortalOR.xml"))).Count > 0)
            {
                driver.FindElement((OR.GetElement("VideoRequestByUser", "YesButton", "TVWebPortalOR.xml"))).Click();
            }


        }

        public void Logout()
        {
            IWebElement lblMYIET = driver.FindElement((OR.GetElement("NonIETMemberRegistration", "MyIETDropDown", "TVWebPortalOR.xml")));                                       //Clicking myIET dropdown 
            executor.ExecuteScript("arguments[0].click();", lblMYIET);

            IWebElement lblLogout = driver.FindElement((OR.GetElement("NonIETMemberRegistration", "LogoutLink", "TVWebPortalOR.xml")));                                     //clicking on log out link
            executor.ExecuteScript("arguments[0].click();", lblLogout);

        }


        public void HandlingWelcomePopup()
        {


            iWait.Until(ExpectedConditions.ElementIsVisible((OR.GetElement("VideoManagement", "LoginWelcomeMsg", "TVWebPortalOR.xml"))));
            //iWait.Until(ExpectedConditions.ElementIsVisible((OR.GetElement("NonIETMemberRegistration", "button.ok_btn_size", "TVWebPortalOR.xml"))));  // Waiting for Popup window to appear after clicking on accept button

            IList<IWebElement> btnOK = driver.FindElements((OR.GetElement("NonIETMemberRegistration", "button.ok_btn_size", "TVWebPortalOR.xml")));

           // btnOK[0].Click();

            executor.ExecuteScript("arguments[0].click();", btnOK.ElementAt(0));

            iWait.Until(ExpectedConditions.InvisibilityOfElementLocated((OR.GetElement("VideoManagement", "LoginWelcomeMsg", "TVWebPortalOR.xml"))));
        }

        public void HandlingSuccessMessage(String message)
        {
            iWait.Until(ExpectedConditions.ElementIsVisible((OR.GetElement("VideoRequestByUser", "BannerMessageOkButton", "TVWebPortalOR.xml"))));

            Assert.AreEqual(message, driver.FindElement(OR.GetElement("ReportAbuse", "SuccessMsg", "TVWebPortalOR.xml")).Text.Trim());

            driver.FindElement((OR.GetElement("VideoRequestByUser", "BannerMessageOkButton", "TVWebPortalOR.xml"))).Click();

            //iWait.Until(ExpectedConditions.InvisibilityOfElementLocated((OR.GetElement("VideoRequestByUser", "BannerMessageOkButton", "TVWebPortalOR.xml"))));

            iWait.Until(ExpectedConditions.InvisibilityOfElementLocated((OR.GetElement("SeriesManagement", "OverlayWait", "TVWebPortalOR.xml"))));
        }

        public Dictionary<String, String> StoreCurrentResult()
        {
            Dictionary<String, String> result = new Dictionary<string, string>();

            result.Add("totalResponse", lblResponse().Text.Trim().Split(' ')[0]);

            result.Add("option1Response", lblAnswer(0).FindElement(By.TagName("span")).Text.Trim().ToCharArray()[1].ToString());

            result.Add("option2Response", lblAnswer(1).FindElement(By.TagName("span")).Text.Trim().ToCharArray()[1].ToString());

            result.Add("option1PercentResult", lblPercentResult(0).FindElement(By.TagName("div")).Text.Trim());

            result.Add("option2PercentResult", lblPercentResult(1).FindElement(By.TagName("div")).Text.Trim());

            return result;

        }

        public Dictionary<String, String> StoreCurrentResult(int totalOption)
        {
            Dictionary<String, String> result = new Dictionary<string, string>();

            result.Add("totalResponse", lblResponse().Text.Trim().Split(' ')[0]);

            for (int i = 0; i < totalOption; i++)
            {
                result.Add("option" + (i + 1) + "Response", lblAnswer(i).FindElement(By.TagName("span")).Text.Trim().ToCharArray()[1].ToString());
            }

            for (int i = 0; i < totalOption; i++)
            {
                result.Add("option" + (i + 1) + "PercentResponse", lblPercentResult(i).FindElement(By.TagName("div")).Text.Trim());
            }

           
            return result;

        }


           

    }
}
