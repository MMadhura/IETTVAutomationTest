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

using Microsoft.Expression.Encoder.ScreenCapture;
using IETTVWebportal.Reusable_Functions;


namespace IETTVWebportal.Chrome
{
    [TestFixture]
    public class Chrome_UserGeneratedContentVerification
    {
        // This is to configure logger mechanism for Utilities.Config
        private static ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        internal IWebDriver driver = null;

        IJavaScriptExecutor executor;

        IWait<IWebDriver> iWait;

        IWait<IWebDriver> vidUplaodWait;

        string driverName = "", driverPath, appURL;

        Utility_Functions uf = new Utility_Functions();                      // Instantiate object for Utility Functons

        Configuration cf = new Configuration();                             // Instantiate object for Configuration

        Object_Repository_Class OR = new Object_Repository_Class();        // Instantiate object for object repository

        ScreenCaptureJob job = null;

        Chrome_WebSetupTearDown st = new Chrome_WebSetupTearDown();        // Instantiate object for Chrome Setup Teardown



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

        public Chrome_UserGeneratedContentVerification(IWebDriver driver, ILog log1, IJavaScriptExecutor Executor, IWait<IWebDriver> iWait)
        {
            this.driver = driver;

            log = log1;

            this.executor = Executor;

            this.iWait = iWait;

        }

        public Chrome_UserGeneratedContentVerification()
        {


        }

        #region Resuable Functions

        //This function wait invisibility of overlay class
        public void OverlayWait()
        {
            iWait.Until(d => d.FindElement((OR.GetElement("SeriesManagement", "OverlayWait", "TVWebPortalOR.xml"))).GetAttribute("class").Contains("display-block"));
        }

        public void handlePromotionalPopup()
        {
            IWebElement promotionalPopup = driver.FindElement((OR.GetElement("SeriesManagement", "Promotionaldiv", "TVWebPortalOR.xml")));

            //iWait.Until(d => d.FindElement((OR.GetElement("SeriesManagement", "Promotionaldiv", "TVWebPortalOR.xml"))).GetCssValue("display").Equals("block"));

            String PromoPopup = promotionalPopup.GetCssValue("display").ToString();

            // Boolean promoPopupPresent = IspromoPopupPresent(driver, By.Id("div_Promotional"));
            if (PromoPopup.Equals("block"))
            {
                driver.FindElement((OR.GetElement("SeriesManagement", "PromotionalPopup", "TVWebPortalOR.xml"))).Click();
            }
        }

        public void HandleEmergencyPopUp()
        {
            //Handling pop up message
            IWebElement emergencyPopup = driver.FindElement((OR.GetElement("SeriesManagement", "EmergencyDiv", "TVWebPortalOR.xml")));

            string emergencyColor = emergencyPopup.GetCssValue("display").ToString();

            if (emergencyColor.Equals("block"))
            {

                driver.FindElement((OR.GetElement("SeriesManagement", "NewEmergencyPopup", "TVWebPortalOR.xml"))).Click();

                iWait.Until(ExpectedConditions.InvisibilityOfElementLocated((OR.GetElement("SeriesManagement", "EmergencyDiv", "TVWebPortalOR.xml"))));
            }
        }

        public void WelcomeMessage()
        {
            //Handling pop up message

            iWait.Until(ExpectedConditions.InvisibilityOfElementLocated((OR.GetElement("SeriesManagement", "OverlayWait", "TVWebPortalOR.xml"))));

            iWait.Until(ExpectedConditions.ElementIsVisible((OR.GetElement("VideoManagement", "LoginWelcomeMsg", "TVWebPortalOR.xml"))));

            driver.FindElement((OR.GetElement("VideoRequestByUser", "LoginWelcomeMessageOkButton", "TVWebPortalOR.xml"))).Click();

            //  driver.FindElement(By.CssSelector("div#loginWelcomeMessage > div > div.modal-content > div.modal-footer > div > button.btn.btn1.btn-success.ok_btn_size")).Click();

            iWait.Until(ExpectedConditions.InvisibilityOfElementLocated((OR.GetElement("VideoManagement", "LoginWelcomeMsg", "TVWebPortalOR.xml"))));
        }

        #endregion

        //This function do login process
        public void RedirectToLogin()
        {
            log.Info("Inside redirectToLogin ::::");

            iWait.Until(ExpectedConditions.ElementExists((OR.GetElement("NonIETMemberRegistration", "LoginLink", "TVWebPortalOR.xml"))));

            IWebElement loginLink = driver.FindElement((OR.GetElement("NonIETMemberRegistration", "LoginLink", "TVWebPortalOR.xml")));

            executor.ExecuteScript("arguments[0].click();", loginLink);

            log.Info("username    " + cf.readingXMLFile("WebPortal", "InstitutionUser", "instUserName", "Config.xml"));

            log.Info("Password    " + cf.readingXMLFile("WebPortal", "InstitutionUser", "instPassWord", "Config.xml"));

            iWait.Until(ExpectedConditions.ElementIsVisible((OR.GetElement("VideoRequestByUser", "UserNameTB", "TVWebPortalOR.xml"))));

            driver.FindElement((OR.GetElement("VideoRequestByUser", "UserName", "TVWebPortalOR.xml"))).SendKeys(cf.readingXMLFile("WebPortal", "InstitutionUser", "instUserName", "Config.xml"));
            Thread.Sleep(1000);

            driver.FindElement((OR.GetElement("VideoRequestByUser", "Password", "TVWebPortalOR.xml"))).SendKeys(cf.readingXMLFile("WebPortal", "InstitutionUser", "instPassWord", "Config.xml"));

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
            WelcomeMessage();

        }

        public void RedirectToUserGeneratedContent()
        {
            log.Info("Inside redirectToUserGeneratedContent");

            Thread.Sleep(2000);

            iWait.Until(ExpectedConditions.ElementIsVisible((OR.GetElement("VideoRequestByUser", "MyIETDropDown", "TVWebPortalOR.xml"))));

            IWebElement myAccountDropDown = driver.FindElement((OR.GetElement("VideoRequestByUser", "MyIETDropDown", "TVWebPortalOR.xml")));

            executor.ExecuteScript("arguments[0].click()", myAccountDropDown);

            iWait.Until(ExpectedConditions.ElementIsVisible((OR.GetElement("UserGeneratedContent", "VideoContent", "TVWebPortalOR.xml"))));
            driver.FindElement((OR.GetElement("UserGeneratedContent", "VideoContent", "TVWebPortalOR.xml"))).FindElement(By.TagName("a")).Click();
        }


        //This function search the required video and verify the same and navigate to video landing page
        public void SearchVideoVerification(string videoname, string guid_Admin)
        {
            log.Info("searchVideoVerification::::");

            Boolean flag = false;

            //wait till jquery gets completed
            uf.isJqueryActive(driver);

            iWait.Until(ExpectedConditions.ElementExists((OR.GetElement("VideoLandingPage", "SearchTB", "TVWebPortalOR.xml"))));

            //search the required video
            IWebElement SearchTextField = driver.FindElement((OR.GetElement("VideoLandingPage", "SearchTB", "TVWebPortalOR.xml")));
            SearchTextField.SendKeys(videoname);

            iWait.Until(ExpectedConditions.ElementToBeClickable((OR.GetElement("VideoLandingPage", "SearchIcon", "TVWebPortalOR.xml"))));

            //Click on searchIcon
            IWebElement SearchIcon = driver.FindElement((OR.GetElement("VideoLandingPage", "SearchIcon", "TVWebPortalOR.xml")));
            SearchIcon.Click();
            Thread.Sleep(2000);

            handlePromotionalPopup();

            HandleEmergencyPopUp();

            uf.isJqueryActive(driver);

            //verifying the search result
            IList<IWebElement> videoSearchList = (IList<IWebElement>)driver.FindElement((OR.GetElement("VideoLandingPage", "SearchResult", "TVWebPortalOR.xml"))).FindElements((OR.GetElement("VideoLandingPage", "SearchResultRecord", "TVWebPortalOR.xml")));

            //gettting the search result details
            foreach (IWebElement currentSearchrecord in videoSearchList)
            {
                IWebElement searchresultDetails = driver.FindElement((OR.GetElement("VideoLandingPage", "SearchResultDetails", "TVWebPortalOR.xml")));
                String webvideoTitle = searchresultDetails.Text.Trim();

                //getting video Title from search result
                if (webvideoTitle.Equals(videoname))
                {
                    flag = true;

                    String videoID_Web = searchresultDetails.GetAttribute("data-videono");

                    String guid_Web = searchresultDetails.GetAttribute("data-videoid");

                    cf.writingIntoXML("AdminPortal", "VideoManagement", "VideoID", videoID_Web, "SysConfig.xml");

                    //verifying the Video Guid match on webportal with admin portal 
                    Assert.AreEqual(guid_Admin, guid_Web);

                    IWebElement serachResultLink = driver.FindElement((OR.GetElement("VideoLandingPage", "SearchResultLink", "TVWebPortalOR.xml")));

                    log.Info("Web Video Title:" + webvideoTitle + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

                    serachResultLink.Click();

                    uf.isJqueryActive(driver);

                    break;
                }
            }
        }


        public void VerifyBannerMessage(String message)
        {
            vidUplaodWait = new WebDriverWait(driver, TimeSpan.FromMinutes(10));

            //wait untill 10minutes for banner message to be get displayed 
            vidUplaodWait.Until(ExpectedConditions.ElementIsVisible((OR.GetElement("VideoLandingPage", "InfoMessage", "TVWebPortalOR.xml"))));

            log.Info("Message:::" + driver.FindElement((OR.GetElement("VideoLandingPage", "InfoMsg", "TVWebPortalOR.xml"))).Text.Trim());

            Assert.AreEqual(message.Trim(), driver.FindElement((OR.GetElement("VideoLandingPage", "InfoMsg", "TVWebPortalOR.xml"))).Text.Trim());

            driver.FindElement((OR.GetElement("VideoLandingPage", "InfoMessage", "TVWebPortalOR.xml"))).Click();

            iWait.Until(ExpectedConditions.InvisibilityOfElementLocated((OR.GetElement("VideoLandingPage", "InfoMessage", "TVWebPortalOR.xml"))));

        }

        public String GenerateContent()
        {
            try
            {
                log.Info("Inside generateContent");

                uf.isJqueryActive(driver);

                //RedirectToLogin();

                //Thread.Sleep(2000);

                RedirectToUserGeneratedContent();


                #region fill the details

                log.Info("filling details");

                iWait.Until(ExpectedConditions.ElementIsVisible((OR.GetElement("UserGeneratedContent", "UserVideoTitle", "TVWebPortalOR.xml"))));
                String videoTitle = "vid" + uf.getShortGuid();
                driver.FindElement((OR.GetElement("UserGeneratedContent", "UserVideoTitle", "TVWebPortalOR.xml"))).SendKeys(videoTitle);

                driver.FindElement((OR.GetElement("UserGeneratedContent", "UserVideoDesc", "TVWebPortalOR.xml"))).SendKeys("automation test");

                driver.FindElement((OR.GetElement("UserGeneratedContent", "UserContact", "TVWebPortalOR.xml"))).SendKeys("9869458458");

                #endregion

                //getting video file from xml
                String uploadvideo = cf.readingXMLFile("AdminPortal", "Video_Management", "VideoUpload", "Config.xml");
                string uploadvideoPath = Environment.CurrentDirectory + "\\Upload\\Videos\\" + uploadvideo;

                iWait.Until(ExpectedConditions.ElementIsVisible((OR.GetElement("UserGeneratedContent", "BrowseButton", "TVWebPortalOR.xml"))));    //browse button

                IWebElement videoBrowseButton = driver.FindElement((OR.GetElement("UserGeneratedContent", "VideoBrowseButton", "TVWebPortalOR.xml")));

                //calling upload funtionalty from the Utility class to upload the required file
                uf.uploadfile(videoBrowseButton, uploadvideoPath);

                driver.FindElement((OR.GetElement("UserGeneratedContent", "UploadButton", "TVWebPortalOR.xml"))).Click();

                VerifyBannerMessage(" Content is uploaded successfully.OK");

                return videoTitle;

            }
            catch (Exception e)
            {
                log.Info("error" + e.Message + e.StackTrace);
                return null;
            }


        }

        public void VerifyStatus(String videoName,String action)
        {

            iWait.Until(ExpectedConditions.InvisibilityOfElementLocated((OR.GetElement("SeriesManagement", "OverlayWait", "TVWebPortalOR.xml"))));

            iWait.Until(ExpectedConditions.ElementIsVisible((OR.GetElement("UserGeneratedContent", "StatusButton", "TVWebPortalOR.xml"))));

            //click on status link
            driver.FindElement((OR.GetElement("UserGeneratedContent", "StatusButton", "TVWebPortalOR.xml"))).Click();

            iWait.Until(ExpectedConditions.ElementExists((OR.GetElement("UserGeneratedContent", "VideoUpload", "TVWebPortalOR.xml"))));

            IWebElement tblVideoListing = driver.FindElement((OR.GetElement("UserGeneratedContent", "VideoUpload", "TVWebPortalOR.xml")));

            iWait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy(By.TagName("tr")));

            //IList<IWebElement> userContentRowList = (IList<IWebElement>)tblVideoListing.FindElements(By.TagName("tr"));

            IList<IWebElement> userContentRowList = (IList<IWebElement>)tblVideoListing.FindElements(By.CssSelector("table#UploadedVideo > tbody > tr"));

            Boolean flag = false;

            int i = 0;
            int counter = 0;
            foreach (IWebElement currentRow in userContentRowList)
            {

                if (counter >= 1)
                {

                    String columData = currentRow.FindElements((OR.GetElement("UserGeneratedContent", "StatusColumnData", "TVWebPortalOR.xml")))[0].Text.Trim();


                    if (columData.Equals(videoName))
                    {
                        flag = true;

                        if(action.Equals("reject"))
                            Assert.AreEqual("Rejected",currentRow.FindElements((OR.GetElement("UserGeneratedContent", "StatusColumnData", "TVWebPortalOR.xml")))[3].Text.Trim());
                        else
                            Assert.AreEqual("Approved", currentRow.FindElements((OR.GetElement("UserGeneratedContent", "StatusColumnData", "TVWebPortalOR.xml")))[3].Text.Trim());
                        break;
                    }
                    i++;
                }
                else
                    counter++;
            }
        }

        public void LogOut()
        {
            log.Info("Inside Logout");

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
