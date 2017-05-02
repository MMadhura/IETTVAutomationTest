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
    public class Chrome_VideoManagementVerification
    {
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

        #region resuable function

        //This function wait invisibility of overlay class
        public void OverlayWait()
        {
            iWait.Until(d => d.FindElement(OR.GetElement("SeriesManagement", "OverlayWait", "TVWebPortalOR.xml")).GetAttribute("class").Contains("display-block"));
        }

        public void handlePromotionalPopup()
        {
            IWebElement promotionalPopup = driver.FindElement((OR.GetElement("SeriesManagement", "Promotionaldiv", "TVWebPortalOR.xml")));

            String PromoPopup = promotionalPopup.GetCssValue("display").ToString();

            if (PromoPopup.Equals("block"))
            {
                iWait.Until(ExpectedConditions.ElementIsVisible(OR.GetElement("SeriesManagement", "Promotionaldiv", "TVWebPortalOR.xml")));

                driver.FindElement((OR.GetElement("SeriesManagement", "PromotionalPopup", "TVWebPortalOR.xml"))).Click();

                iWait.Until(ExpectedConditions.InvisibilityOfElementLocated((OR.GetElement("SeriesManagement", "EmergencyDiv", "TVWebPortalOR.xml"))));

            }
        }

        public void handleEmergencyPopUp()
        {
            //Handling pop up message
            IWebElement emergencyPopup = driver.FindElement((OR.GetElement("SeriesManagement", "EmergencyDiv", "TVWebPortalOR.xml")));

            string emergencyColor = emergencyPopup.GetCssValue("display").ToString();

            if (emergencyColor.Equals("block"))
            {
                iWait.Until(ExpectedConditions.ElementIsVisible(OR.GetElement("SeriesManagement", "EmergencyDiv", "TVWebPortalOR.xml")));

                driver.FindElement((OR.GetElement("SeriesManagement", "NewEmergencyPopup", "TVWebPortalOR.xml"))).Click();

                iWait.Until(ExpectedConditions.InvisibilityOfElementLocated((OR.GetElement("SeriesManagement", "EmergencyDiv", "TVWebPortalOR.xml"))));
            }

        }

        public void welcomeMessage()
        {
            //Handling pop up message
            iWait.Until(ExpectedConditions.ElementIsVisible((OR.GetElement("VideoManagement", "LoginWelcomeMsg", "TVWebPortalOR.xml"))));

            driver.FindElement((OR.GetElement("VideoManagement", "WelcomeMsgCSS", "TVWebPortalOR.xml"))).Click();

            iWait.Until(ExpectedConditions.InvisibilityOfElementLocated((OR.GetElement("VideoManagement", "LoginWelcomeMsg", "TVWebPortalOR.xml"))));
        }

        #endregion

        public Chrome_VideoManagementVerification()
        {

        }

        public Chrome_VideoManagementVerification(IWebDriver driver, ILog log1, IJavaScriptExecutor Executor, IWait<IWebDriver> iWait)
        {
            this.driver = driver;

            log = log1;

            this.executor = Executor;

            this.iWait = iWait;

        }

        //This function do login process
        public void redirectToLogin()
        {
            log.Info("redirectToLogin ::::");

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
            welcomeMessage();

        }

        public static Boolean IspromoPopupPresent(IWebDriver driver, By element)
        {
            try
            {
                driver.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromSeconds(30));
                driver.FindElement(element);
                return true;
            }
            catch (Exception)
            {

                return false;
            }
        }

        public static Boolean IsEmergencyPopuppresent(IWebDriver driver, By element)
        {
            try
            {
                driver.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromSeconds(30));
                driver.FindElement(element);

                return true;
            }
            catch (Exception)
            {

                return false;
            }
        }

        //This function search the required video and verify the same and navigate to video landing page
        public void searchVideoVerification(string videoname, string guid_Admin)
        {
            log.Info("searchVideoVerification::::");

            Boolean flag = false;

            //wait till jquery gets completed
            uf.isJqueryActive(driver);

            handlePromotionalPopup();

            handleEmergencyPopUp();

            Thread.Sleep(5000);

            iWait.Until(ExpectedConditions.ElementExists((OR.GetElement("VideoLandingPage", "SearchTB", "TVWebPortalOR.xml"))));

            //search the required video
            IWebElement SearchTextField = driver.FindElement((OR.GetElement("VideoLandingPage", "SearchTB", "TVWebPortalOR.xml")));
            SearchTextField.SendKeys(videoname);

            iWait.Until(ExpectedConditions.ElementToBeClickable((OR.GetElement("VideoLandingPage", "SearchIcon", "TVWebPortalOR.xml"))));

            //Click on searchIcon
            IWebElement SearchIcon = driver.FindElement((OR.GetElement("VideoLandingPage", "SearchIcon", "TVWebPortalOR.xml")));
            SearchIcon.Click();
            Thread.Sleep(2000);

            //handleEmergencyPopUp();

            uf.isJqueryActive(driver);



            //verifying the search result
            IList<IWebElement> videoSearchList = (IList<IWebElement>)driver.FindElement((OR.GetElement("VideoLandingPage", "SearchResult", "TVWebPortalOR.xml"))).FindElements(OR.GetElement("VideoLandingPage", "SearchResultRecord", "TVWebPortalOR.xml"));


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


        // This function verify video name on video landing page
        public void verifyVideoName(string videoname)
        {
            log.Info("verifyVideoName::::");

            Assert.AreEqual(videoname, driver.FindElement((OR.GetElement("VideoManagement", "VidNameCSS", "TVWebPortalOR.xml"))).Text.Trim());
        }

        // This function verify AbstractContent on video landing page
        public void verifyAbstractContent(String abstractContent)
        {

            executor.ExecuteScript("window.scrollTo(0, document.body.scrollHeight)", "");
            executor.ExecuteScript("window.scrollTo(0, 400)", "");

            Thread.Sleep(3000);
            log.Info("verifyAbstractContent::::");

            Assert.AreEqual(abstractContent, driver.FindElement((OR.GetElement("VideoManagement", "VideoDesc", "TVWebPortalOR.xml"))).FindElement(By.TagName("p")).Text.Trim());


        }

        // This function verify ChannelName on video landing page
        public void verifyChannelName(String adminSelectedChannel)
        {
            log.Info("verifyChannelName::::");

            executor.ExecuteScript("window.scrollTo(0, document.body.scrollHeight)", "");
            executor.ExecuteScript("window.scrollTo(0, 400)", "");

            Thread.Sleep(3000);
            Assert.AreEqual(adminSelectedChannel, driver.FindElement((OR.GetElement("VideoManagement", "ChannelNameCSS", "TVWebPortalOR.xml"))).Text.Trim());
        }

        // This function verify KeywordName on video landing page
        public void verifyKeywordName(String adminKeyword)
        {
            log.Info("verifyKeywordName::::");

            executor.ExecuteScript("window.scrollTo(0, document.body.scrollHeight)", "");
            executor.ExecuteScript("window.scrollTo(0, 400)", "");

            Thread.Sleep(3000);
            Assert.AreEqual(adminKeyword, driver.FindElement((OR.GetElement("VideoManagement", "Keywordid_0", "TVWebPortalOR.xml"))).Text.Trim());
        }

        // This function verify Attachment name on video landing page
        public void verifyAttachment(String adminAttachment)
        {
            log.Info("verifyAttachment::::");
            executor.ExecuteScript("window.scrollTo(0, document.body.scrollHeight)", "");
            executor.ExecuteScript("window.scrollTo(0, 400)", "");

            Thread.Sleep(3000);
            Assert.AreEqual(adminAttachment, driver.FindElement((OR.GetElement("VideoManagement", "DownloadAttachment", "TVWebPortalOR.xml"))).Text.Trim());
        }

        // This function verify Sponsor on video landing page
        public void verifySponsorDetails(Dictionary<String, String> adminSponsorDetails)
        {
            log.Info("verifySponsorDetails::::");
            executor.ExecuteScript("window.scrollTo(0, document.body.scrollHeight)", "");
            executor.ExecuteScript("window.scrollTo(0, 400)", "");

            Thread.Sleep(3000);
            iWait.Until(ExpectedConditions.ElementExists(OR.GetElement("SeriesManagement", "Sponserdiv", "TVWebPortalOR.xml")));

            var elem = driver.FindElement(OR.GetElement("SeriesManagement", "Sponserdiv", "TVWebPortalOR.xml"));
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(true);", elem);

            //1.verify sponsor image

            log.Info("verify sponsor image:::::");

            String sponsorSystemImageName = adminSponsorDetails["imageName"];

            String sponsorDownloadImageName = "actualSponsorImage.jpg";

            string imageLinkUrl = driver.FindElement((OR.GetElement("SeriesManagement", "ImageLinkUrl", "TVWebPortalOR.xml"))).GetAttribute("src");

            string downloadPath = Environment.CurrentDirectory + "\\Upload\\DownloadedImages\\";

            uf.download(imageLinkUrl, downloadPath, sponsorDownloadImageName);

            string sponsorImagePath = Environment.CurrentDirectory + "\\Upload\\Images\\" + adminSponsorDetails["imageName"];

            Assert.AreEqual(true, uf.imageComparision(new Bitmap(downloadPath + sponsorDownloadImageName), new Bitmap(sponsorImagePath)));

            //2.verify sponsor image url STATUS
            log.Info("verify sponsor image url Status:::::");

            String str = "'";

            string sponsorRedirectUrl = driver.FindElement((OR.GetElement("SeriesManagement", "SponsorRedirectUrl", "TVWebPortalOR.xml"))).GetAttribute("onclick");

            String url = sponsorRedirectUrl.Split(Convert.ToChar(str))[1];

            log.Info("urlaftersplit :: " + sponsorRedirectUrl.Split(Convert.ToChar(str))[1]);

            Assert.AreEqual("OK", uf.getStatusCode(new Uri(url)));

        }

        // This function verify SpeakerDetails on video landing page
        public void verifySpeakerDetails(Dictionary<String, String> adminSpeakerDetails)
        {
            log.Info("Inside verifySpeakerDetails::::");

            Assert.AreEqual(adminSpeakerDetails["title"], driver.FindElement(OR.GetElement("VideoManagement", "SpkrCarousel", "TVWebPortalOR.xml")).FindElements(By.TagName("span"))[0].Text.Trim());

            Assert.AreEqual(adminSpeakerDetails["firstName"], driver.FindElement(OR.GetElement("VideoManagement", "SpkrCarousel", "TVWebPortalOR.xml")).FindElements(By.TagName("span"))[1].Text.Trim());

            Assert.AreEqual(adminSpeakerDetails["lastName"], driver.FindElement(OR.GetElement("VideoManagement", "SpkrCarousel", "TVWebPortalOR.xml")).FindElements(By.TagName("span"))[2].Text.Trim());

        }

        //This function search the hide record video and verify the same
        public void verifyingNoresultFound(string videoname)
        {
            log.Info("inside verifyNoresultfound function  " + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

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

            uf.isJqueryActive(driver);

            //verifying the video with no result found
            String NoResultFound = driver.FindElement((OR.GetElement("VideoLandingPage", "SpanResult", "TVWebPortalOR.xml"))).Text;

            Thread.Sleep(1000);
            Assert.AreEqual("No result found", NoResultFound);
        }

        // consider channel type as member
        public void verifySubscriptionVideo()
        {
            log.Info("Inside verify SubscriptionVideo :::::");

            //iWait.Until(ExpectedConditions.ElementIsVisible((OR.GetElement("VideoManagement", "SubscriptionVidCSS", "TVWebPortalOR.xml"))));
            iWait.Until(ExpectedConditions.ElementIsVisible((OR.GetElement("VideoManagement", "Subscriptionbtn", "TVWebPortalOR.xml"))));

            Assert.AreEqual(true, driver.FindElement((OR.GetElement("VideoManagement", "Subscriptionbtn", "TVWebPortalOR.xml"))).Text.Contains("Buy channel"));

            Assert.AreEqual(true, driver.FindElement((OR.GetElement("VideoManagement", "LockBtn", "TVWebPortalOR.xml"))).Displayed);
        }

        public void verifyPremiumVideo()
        {
            log.Info("Inside verify PremiumVideo :::::");

            Assert.AreEqual("Buy video", driver.FindElement((OR.GetElement("VideoManagement", "PremiumVidCSS", "TVWebPortalOR.xml"))).Text.Trim());

            Assert.AreEqual(true, driver.FindElement((OR.GetElement("VideoManagement", "LockBtn", "TVWebPortalOR.xml"))).Displayed);

        }

        public void verifyFreeVideo()
        {
            log.Info("Inside verify FreeVideo :::::");

            Assert.AreEqual(true, driver.FindElement((OR.GetElement("VideoLandingPage", "PlayButton", "TVWebPortalOR.xml"))).Displayed);
        }

        public void verifyPromoImage(String promoDetails)
        {
            //verify FreePromoImage image
            log.Info("Inside verifyPromoImage :::::");

            String promoDownloadImageName = "actualPromoImage.jpg";

            String imageLinkUrl = driver.FindElement((OR.GetElement("VideoManagement", "BackgroundImg", "TVWebPortalOR.xml"))).GetAttribute("src");

            string downloadPath = Environment.CurrentDirectory + "\\Upload\\DownloadedImages\\";

            uf.download(imageLinkUrl, downloadPath, promoDownloadImageName);

            string promoImagePath = Environment.CurrentDirectory + "\\Upload\\Images\\" + promoDetails;

            Assert.AreEqual(true, uf.imageComparision(new Bitmap(downloadPath + promoDownloadImageName), new Bitmap(promoImagePath)));

        }

        public void verifyDefaultImage()
        {

            log.Info("Inside  verifyDefaultImage :::::");

            String defaultDownloadImageName = "actualDefaultImage.jpg";

            iWait.Until(ExpectedConditions.ElementIsVisible((OR.GetElement("VideoManagement", "BackgroundImg", "TVWebPortalOR.xml"))));

            String imageLinkUrl = driver.FindElement((OR.GetElement("VideoManagement", "BackgroundImg", "TVWebPortalOR.xml"))).GetAttribute("src");

            string downloadPath = Environment.CurrentDirectory + "\\Upload\\DownloadedImages\\";

            uf.download(imageLinkUrl, downloadPath, defaultDownloadImageName);

            string defaultImageServerPath = cf.readingXMLFile("AdminPortal", "Video_Management", "WebImagesServerPath", "Config.xml");

            log.Info("defaultImageServerPath  " + defaultImageServerPath);

            String defaultImageName = cf.readingXMLFile("AdminPortal", "Video_Management", "PromoDefaultImageName", "Config.xml");

            log.Info("defaultImageName  " + defaultImageName);

            Assert.AreEqual(true, uf.imageComparision(new Bitmap(downloadPath + defaultDownloadImageName), new Bitmap(defaultImageServerPath + defaultImageName)));

        }

        public void verifyCountDownDetails()
        {
            log.Info("Inside verifyCountDownDetails::::::::::");

            Assert.AreEqual(true, driver.FindElement((OR.GetElement("VideoManagement", "CountDown", "TVWebPortalOR.xml"))).Displayed);

            String countDownTemp = driver.FindElement((OR.GetElement("VideoManagement", "CountDown", "TVWebPortalOR.xml"))).FindElement(By.TagName("span")).Text.Trim();

            Assert.AreEqual("1d", countDownTemp.Split(' ')[0]);
        }

        public void verifyCommentStatus()
        {
            Thread.Sleep(2000);

            log.Info("Inside verify CommentStatus:::::::::");

            iWait.Until(ExpectedConditions.ElementExists((OR.GetElement("VideoLandingPage", "Comment", "TVWebPortalOR.xml"))));

            var elem = driver.FindElement((OR.GetElement("VideoLandingPage", "Comment", "TVWebPortalOR.xml")));
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(true);", elem);
            Thread.Sleep(500);

            Assert.AreEqual(false, driver.FindElement((OR.GetElement("VideoLandingPage", "Comment", "TVWebPortalOR.xml"))).Enabled);

            Assert.AreEqual(false, driver.FindElement((OR.GetElement("VideoLandingPage", "SubmitButton", "TVWebPortalOR.xml"))).Enabled);

        }

        public void verifyLikeDislikeStatus()
        {
            log.Info("Inside verify LikeDislikeStatus:::::::::");

            Assert.AreEqual(false, driver.FindElement((OR.GetElement("VideoManagement", "VideoLikeBTN", "TVWebPortalOR.xml"))).Enabled);

            Assert.AreEqual(false, driver.FindElement((OR.GetElement("VideoManagement", "VideoDisLikeBTN", "TVWebPortalOR.xml"))).Enabled);

        }

        public void verifyQnADisplayed()
        {
            log.Info("Inside verify QnADisplayed:::::::::");

            Assert.AreEqual(true, driver.FindElement((OR.GetElement("VideoManagement", "QnADisplayed", "TVWebPortalOR.xml"))).Displayed);
        }

        public void verifyThumbnail(String thumbnailImageName)
        {

            log.Info("Inside verifyThumbnail :::::");

            String thumbnailDownloadImageName = "actualThumbnailImage.jpg";

            String imageLinkUrl = driver.FindElement((OR.GetElement("VideoManagement", "BackgroundImg", "TVWebPortalOR.xml"))).GetAttribute("src");

            string downloadPath = AppDomain.CurrentDomain.BaseDirectory + "\\Upload\\DownloadedImages\\";

            uf.download(imageLinkUrl, downloadPath, thumbnailDownloadImageName);

            string thumbnailImagePath = Environment.CurrentDirectory + "\\Upload\\Images\\" + thumbnailImageName;

            Assert.AreEqual(true, uf.imageComparision(new Bitmap(downloadPath + thumbnailDownloadImageName), new Bitmap(thumbnailImagePath)));
        }

        public void searchVideo(String videoSearchByName)
        {
            log.Info("Inside searchVideo :::::");

            iWait.Until(ExpectedConditions.ElementExists((OR.GetElement("VideoLandingPage", "SearchTB", "TVWebPortalOR.xml"))));

            //search the required video
            IWebElement SearchTextField = driver.FindElement((OR.GetElement("VideoLandingPage", "SearchTB", "TVWebPortalOR.xml")));
            SearchTextField.SendKeys(videoSearchByName);

            iWait.Until(ExpectedConditions.ElementToBeClickable((OR.GetElement("VideoLandingPage", "SearchIcon", "TVWebPortalOR.xml"))));

            //Click on searchIcon
            IWebElement SearchIcon = driver.FindElement((OR.GetElement("VideoLandingPage", "SearchIcon", "TVWebPortalOR.xml")));

            SearchIcon.Click();

            Thread.Sleep(2000);

            SearchTextField.Clear();
        }

        public void verifySearchedVideo(String searchText, String guid_Admin)
        {
            log.Info("Inside verifySearchedVideo :::::");

            Boolean flag = false;

            //verifying the search result
            IList<IWebElement> videoSearchList = (IList<IWebElement>)driver.FindElement((OR.GetElement("VideoLandingPage", "SearchResult", "TVWebPortalOR.xml"))).FindElements((OR.GetElement("VideoLandingPage", "SearchResultRecord", "TVWebPortalOR.xml")));


            //gettting the search result details
            foreach (IWebElement currentSearchrecord in videoSearchList)
            {
                IWebElement searchresultDetails = driver.FindElement((OR.GetElement("VideoLandingPage", "SearchResultDetails", "TVWebPortalOR.xml")));
                String webvideoTitle = searchresultDetails.Text.Trim();

                //getting video Title from search result
                if (webvideoTitle.Equals(searchText))
                {
                    flag = true;

                    String videoID_Web = searchresultDetails.GetAttribute("data-videono");

                    String guid_Web = searchresultDetails.GetAttribute("data-videoid");

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

        //verified the image 
        public void verifyCpdLogo()
        {

            log.Info("Inside verifyCpdLogo :::::");

            String defaultDownloadImageName = "actualCpdImage.jpg";

            iWait.Until(ExpectedConditions.ElementIsVisible((OR.GetElement("VideoManagement", "BackgroundImg", "TVWebPortalOR.xml"))));

            String imageLinkUrl = driver.FindElement((OR.GetElement("VideoManagement", "ImgLinkURL", "TVWebPortalOR.xml"))).GetAttribute("src");

            string downloadPath = Environment.CurrentDirectory + "\\Upload\\DownloadedImages\\";

            uf.download(imageLinkUrl, downloadPath, defaultDownloadImageName);

            string cpdLogoPath = cf.readingXMLFile("AdminPortal", "Video_Management", "WebImagesServerPath", "Config.xml");

            log.Info("cpdLogoPath  " + cpdLogoPath);

            String cpdLogoName = cf.readingXMLFile("AdminPortal", "Video_Management", "CpdImagename", "Config.xml");

            log.Info("cpdLogoName  " + cpdLogoName);
        }


        public void likeDislikeTest(String videoName)
        {
            log.Info("Inside likeDislikeTest :::::");

            //like button verification

            //uf.scrollUp(driver);

            iWait.Until(ExpectedConditions.ElementToBeClickable((OR.GetElement("VideoManagement", "VideoLikeBTN", "TVWebPortalOR.xml"))));

            int likeCurrentValue = Convert.ToInt32(driver.FindElement((OR.GetElement("VideoManagement", "LikeValue", "TVWebPortalOR.xml"))).Text.Trim());

            log.Info("likeCurrentValue  : " + likeCurrentValue);

            iWait.Until(ExpectedConditions.ElementToBeClickable((OR.GetElement("VideoManagement", "VideoLikeBTN", "TVWebPortalOR.xml"))));

            //var elem = driver.FindElement((OR.GetElement("VideoManagement", "VideoLikeBTN", "TVWebPortalOR.xml")));
            //((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(true);", elem);

            Thread.Sleep(4000);

            //driver.FindElement((OR.GetElement("VideoManagement", "VideoLikeBTN", "TVWebPortalOR.xml"))).Click();

           // driver.FindElement(By.CssSelector("div#like-dislike-btn div.like-unlike-pannel button#btnVideoLike")).Click();

            IWebElement LikeDislikeDiv = driver.FindElement(OR.GetElement("VideoManagement", "LikeDislikeDiv", "TVWebPortalOR.xml"));
            LikeDislikeDiv.FindElement(OR.GetElement("VideoManagement", "Likebtn", "TVWebPortalOR.xml")).Click();


            //IWebElement e = driver.FindElement(By.Id("like-dislike-btn"));

            //e.FindElement(By.Id("btnVideoLike")).Click();
                      
            //Thread.Sleep(10000);

            driver.FindElement((OR.GetElement("VideoManagement", "VideoLikeBTN", "TVWebPortalOR.xml"))).Click();

            int likeCurrentValue1 = Convert.ToInt32(driver.FindElement((OR.GetElement("VideoManagement", "LikeValue", "TVWebPortalOR.xml"))).Text.Trim());

            Thread.Sleep(5000);

            //Assert.AreEqual(likeCurrentValue + 1, likeCurrentValue1);

            //executor.ExecuteScript("window.scrollTo(0, document.body.scrollHeight)", "");
            //executor.ExecuteScript("window.scrollTo(0, 400)", "");

            //Thread.Sleep(5000);

            //Dislike button verification
            int disLikeCurrentValue = Convert.ToInt32(driver.FindElement((OR.GetElement("VideoManagement", "DislikeValue", "TVWebPortalOR.xml"))).Text.Trim());

            log.Info("disLikeCurrentValue  : " + disLikeCurrentValue);

            //Thread.Sleep(6000);
            iWait.Until(ExpectedConditions.ElementToBeClickable((OR.GetElement("VideoManagement", "VideoDisLikeBTN", "TVWebPortalOR.xml"))));

            driver.FindElement((OR.GetElement("VideoManagement", "VideoDisLikeBTN", "TVWebPortalOR.xml"))).Click();

            Thread.Sleep(2000);
            executor.ExecuteScript("window.scrollTo(0, document.body.scrollHeight)", "");
            executor.ExecuteScript("window.scrollTo(0, 400)", "");

            int disLikeCurrentValue1 = Convert.ToInt32(driver.FindElement((OR.GetElement("VideoManagement", "DislikeValue", "TVWebPortalOR.xml"))).Text.Trim());

            Thread.Sleep(5000);
            

            Assert.AreEqual(disLikeCurrentValue + 1, disLikeCurrentValue1);

            // Assert.AreEqual(likeCurrentValue, Convert.ToInt32(driver.FindElement((OR.GetElement("VideoManagement", "LikeValue", "TVWebPortalOR.xml"))).Text.Trim()));


        }


        public void verifyPromoVideo()
        {
            String promoVideoImageName = cf.readingXMLFile("AdminPortal", "Video_Management", "PromoVideoImageName", "Config.xml");

            String promoVideoPath = Environment.CurrentDirectory + "\\Upload\\Sikuli\\" + promoVideoImageName;

            Console.WriteLine("promoVideoPath   :" + promoVideoPath);

            Pattern ptnFreePromoVideo = new Pattern(promoVideoPath);

            Sikuli4Net.sikuli_REST.Screen s = new Sikuli4Net.sikuli_REST.Screen();

            Assert.AreEqual(true, s.Exists(ptnFreePromoVideo));
        }

        [Test]
        public void TVWeb_001_SearchTest()
        {
            try
            {
                log.Info("TVWeb_001_SearchTest  Started" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

                IList<IWebElement> searchDropdown;

                String videoName = cf.readingXMLFile("AdminPortal", "VideoManagement", "VideoName", "SysConfig.xml");

                String guid_Admin = cf.readingXMLFile("AdminPortal", "VideoManagement", "Guid", "SysConfig.xml");

                String videoNumber = cf.readingXMLFile("AdminPortal", "VideoManagement", "VideoID", "SysConfig.xml");

                String speakerName = cf.readingXMLFile("AdminPortal", "VideoManagement", "SpeakerName", "SysConfig.xml");

                String date = cf.readingXMLFile("AdminPortal", "VideoManagement", "CreatedDate", "SysConfig.xml");

                #region Search by All

                log.Info("searchVideoVerification::::" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

                Boolean flag = false;

                //wait till jquery gets completed
                uf.isJqueryActive(driver);

                handlePromotionalPopup();

                handleEmergencyPopUp();

                Thread.Sleep(3000);

                searchVideo(videoName);

                uf.isJqueryActive(driver);

                verifySearchedVideo(videoName, guid_Admin);

                #endregion

                #region Search by speaker

                driver.FindElement((OR.GetElement("VideoManagement", "SearchBTN", "TVWebPortalOR.xml"))).Click();

                searchDropdown = driver.FindElement(OR.GetElement("VideoManagement", "SearchDorpdown", "TVWebPortalOR.xml")).FindElements(By.TagName("li"));

                searchDropdown[1].FindElement(By.TagName("a")).Click();

                log.Info("speaker name from admin :" + speakerName + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

                searchVideo(speakerName);

                uf.isJqueryActive(driver);

                IWebElement serachResultLink = driver.FindElement((OR.GetElement("VideoLandingPage", "SearchResultLink", "TVWebPortalOR.xml")));

                serachResultLink.Click();

                uf.isJqueryActive(driver);

                IList<IWebElement> speakerList = (IList<IWebElement>)driver.FindElements(OR.GetElement("VideoManagement", "SpeakerItem", "TVWebPortalOR.xml"));

                Assert.AreNotEqual(0, speakerList.Count);

                int i = 0;

                flag = false;

                for (i = 0; i < speakerList.Count * 2; i++)
                {
                    int count = speakerList[i].FindElements((OR.GetElement("VideoManagement", "UserBlock", "TVWebPortalOR.xml"))).Count;

                    for (int j = 0; j < 2; j++)
                    {

                        String firstName = speakerList[i].FindElements((OR.GetElement("VideoManagement", "UserBlock", "TVWebPortalOR.xml")))[j].FindElement(OR.GetElement("VideoManagement", "SpeakerFnameInfo", "TVWebPortalOR.xml")).Text.Trim();

                        String lastName = speakerList[i].FindElements((OR.GetElement("VideoManagement", "UserBlock", "TVWebPortalOR.xml")))[j].FindElement(OR.GetElement("VideoManagement", "SpeakerLnameInfo", "TVWebPortalOR.xml")).Text.Trim();

                        if (firstName.ToLower().Equals(speakerName.ToLower()) || lastName.ToLower().Equals(speakerName.ToLower()))
                        {
                            flag = true;

                            break;
                        }
                    }

                    if (flag)
                        break;

                    log.Info("Speaker name from web portal  :" + speakerList[i].FindElement(OR.GetElement("VideoManagement", "SpeakerFnameInfo", "TVWebPortalOR.xml")).Text.Trim());

                    if (speakerList.Count > 1)
                    {
                        driver.FindElement((OR.GetElement("VideoManagement", "SpeakerList", "TVWebPortalOR.xml"))).Click();

                        iWait.Until(ExpectedConditions.InvisibilityOfElementLocated((OR.GetElement("VideoManagement", "Next", "TVWebPortalOR.xml"))));
                    }
                }

                Assert.AreEqual(true, flag);

                #endregion

                #region Search by Date

                driver.FindElement((OR.GetElement("VideoManagement", "SearchBTN", "TVWebPortalOR.xml"))).Click();

                searchDropdown[2].FindElement(By.TagName("a")).Click();

                String[] dateAfterSplit = date.Split('-');

                String month = uf.convertIntoMMM(Convert.ToInt32(dateAfterSplit[1]));

                String year = dateAfterSplit[2];

                log.Info("month from admin  :" + month);

                log.Info("Year from admin  :" + year);

                searchVideo(month + " " + year);

                uf.isJqueryActive(driver);

                serachResultLink = driver.FindElement((OR.GetElement("VideoLandingPage", "SearchResultLink", "TVWebPortalOR.xml")));

                serachResultLink.Click();

                uf.isJqueryActive(driver);

                String completeDateFromWeb = driver.FindElement((OR.GetElement("VideoManagement", "VideoDate", "TVWebPortalOR.xml"))).Text.Trim();

                String[] completeDateFromWebAfterSplit = completeDateFromWeb.Split(' ');

                String monthFromWeb = completeDateFromWebAfterSplit[1];

                String yearFromWeb = completeDateFromWebAfterSplit[2];

                log.Info("month from web  :" + monthFromWeb);

                log.Info("Year from web  :" + yearFromWeb);

                Assert.AreEqual(uf.convertIntommm(month), monthFromWeb);

                Assert.AreEqual(year, yearFromWeb);

                #endregion

                #region Search by video number

                driver.FindElement((OR.GetElement("VideoManagement", "SearchBTN", "TVWebPortalOR.xml"))).Click();

                searchDropdown[3].FindElement(By.TagName("a")).Click();

                log.Info("Video number to be search :" + videoNumber);

                searchVideo(videoNumber);

                uf.isJqueryActive(driver);

                IWebElement searchresultDetails = driver.FindElement((OR.GetElement("VideoLandingPage", "SearchResultDetails", "TVWebPortalOR.xml")));

                Assert.AreEqual(videoNumber, searchresultDetails.GetAttribute("data-videono"));

                #endregion

                log.Info("TVWeb_001_SearchTest  Completed" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

            }
            catch (Exception e)
            {
                log.Error(e.Message + "\n" + e.StackTrace + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

                Assert.AreEqual(true, false);
            }
        }



        [TestFixtureTearDown]
        public void TearDown()
        {
            st.Chrome_TearDown(driver, log);                        // Calling Chrome Teardown
        }

    }
}




