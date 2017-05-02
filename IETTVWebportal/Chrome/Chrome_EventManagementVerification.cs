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
    public class Chrome_EventManagementVerification
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

        public Chrome_EventManagementVerification() { }

        public Chrome_EventManagementVerification(IWebDriver driver, ILog log1, IJavaScriptExecutor Executor, IWait<IWebDriver> iWait)
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

                Console.WriteLine("Inside Fixture Setup of chrome - NonIETmemberRegistration Test" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

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

        #region Reusable Elements

        public IWebElement pnlVideoSchedule()
        {
            iWait.Until(ExpectedConditions.ElementIsVisible((OR.GetElement("EventManagement", "VideoSchedule", "TVWebPortalOR.xml"))));
            return driver.FindElement((OR.GetElement("EventManagement", "VideoSchedule", "TVWebPortalOR.xml")));
        }



        #endregion

        public String GetVideoStatus(String videoName)
        {
            Console.WriteLine("Inside GetVideoStatus:::: ");

            String status = null;
            uf.scrollDown(driver);

            List<IWebElement> videoList = pnlVideoSchedule().FindElements((OR.GetElement("EventManagement", "EventScheduleContainer", "TVWebPortalOR.xml"))).ToList();
            Boolean flag = false;

            foreach (IWebElement video in videoList)
            {
                iWait.Until(ExpectedConditions.ElementExists((OR.GetElement("EventManagement", "ItemDesc", "TVWebPortalOR.xml"))));
                IWebElement videoDescription = video.FindElement((OR.GetElement("EventManagement", "ItemDesc", "TVWebPortalOR.xml"))).FindElement(By.TagName("a"));

                if (videoName.Equals(videoDescription.Text.Trim()))
                {
                    flag = true;
                    status = video.FindElement((OR.GetElement("EventManagement", "VidStatusSpan", "TVWebPortalOR.xml"))).Text.Trim();
                    break;
                }
            }

            Assert.AreEqual(true, flag); // if fails then video is not present in Shedule section.
            return status;
        }

        public void SearchEvent(String eventSearchByName)
        {
            Console.WriteLine("Inside SearchEvent :::::");

            iWait.Until(ExpectedConditions.InvisibilityOfElementLocated((OR.GetElement("SeriesManagement", "OverlayWait", "TVAdminPortalOR.xml"))));

            iWait.Until(ExpectedConditions.ElementExists((OR.GetElement("VideoLandingPage", "SearchTB", "TVWebPortalOR.xml"))));

            //search the required event
            IWebElement SearchTextField = driver.FindElement((OR.GetElement("VideoLandingPage", "SearchTB", "TVWebPortalOR.xml")));
            SearchTextField.SendKeys(eventSearchByName);

            Thread.Sleep(4000);

            iWait.Until(ExpectedConditions.ElementToBeClickable((OR.GetElement("VideoLandingPage", "SearchIcon", "TVWebPortalOR.xml"))));

            //Click on searchIcon
            IWebElement SearchIcon = driver.FindElement((OR.GetElement("VideoLandingPage", "SearchIcon", "TVWebPortalOR.xml")));

            SearchIcon.Click();

            Thread.Sleep(4000);

            SearchTextField.Clear();
        }

        public void handlePromotionalPopup()
        {
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

        public void ClickAndVerifyEventName(String eventName)
        {
            Console.WriteLine("Inside ClickAndVerifyEventName :::::");

            Boolean flag = false;
            iWait.Until(ExpectedConditions.ElementIsVisible((OR.GetElement("SeriesManagement", "SearchedWord", "TVWebPortalOR.xml"))));

            IList<IWebElement> eventSearchList = (IList<IWebElement>)driver.FindElement((OR.GetElement("BuyVideoVerification", "SearchResult", "TVWebPortalOR.xml"))).FindElements((OR.GetElement("VideoLandingPage", "SearchResultRecord", "TVWebPortalOR.xml")));

            //gettting the search result details
            foreach (IWebElement currentSearchRecord in eventSearchList)
            {
                IWebElement searchresultDetails = currentSearchRecord.FindElement((OR.GetElement("SeriesManagement", "SearchedWord", "TVWebPortalOR.xml")));
                String webEventTitle = searchresultDetails.Text.Trim();

                //getting Event Title from search result
                if (webEventTitle.Equals(eventName))
                {
                    flag = true;
                    searchresultDetails.Click();

                    uf.isJqueryActive(driver);

                    Assert.AreEqual(eventName, driver.FindElement((OR.GetElement("EventManagement", "EventTitle", "TVWebPortalOR.xml"))).Text.Trim());
                }
            }
        }

        //verify the image 
        public void VerifyEventLogo(String eventLogoName)
        {
            //verify FreePromoImage image
            Console.WriteLine("Inside VerifyEventLogo :::::");

            String logoDownloadImageName = "actualEventLogoImage.jpg";

            String imageLinkUrl = driver.FindElement((OR.GetElement("EventManagement", "EventLogo", "TVWebPortalOR.xml"))).GetAttribute("src");

            string downloadPath = Environment.CurrentDirectory + "\\Upload\\DownloadedImages\\";

            uf.download(imageLinkUrl, downloadPath, logoDownloadImageName);

            string promoImagePath = Environment.CurrentDirectory + "\\Upload\\Images\\" + eventLogoName;

            Assert.AreEqual(true, uf.imageComparision(new Bitmap(downloadPath + logoDownloadImageName), new Bitmap(promoImagePath)));
        }

        public void ClickAndVerifyEventAcessCode(String eventAccessCode)
        {
            Console.WriteLine("Inside ClickAndVerifyEventAcessCode :::::");

            uf.scrollUp(driver);

            Thread.Sleep(8000);
            List<IWebElement> eventOverlay = driver.FindElement((OR.GetElement("EventManagement", "PremiumVideoText", "TVWebPortalOR.xml"))).FindElements(By.TagName("div")).ToList();
            IWebElement delegAccessCode = eventOverlay[4].FindElement(By.TagName("a"));
            Console.WriteLine("deletgate text::: " + delegAccessCode.Text);

            Assert.AreEqual(true, driver.FindElement((OR.GetElement("EventManagement", "PremiumVideoText", "TVWebPortalOR.xml"))).Displayed);
            delegAccessCode.Click();

            EventLogin();

            ClickYesButton();

            Thread.Sleep(5000);

            welcomeMessage();

            Thread.Sleep(8000);

            List<IWebElement> eventOverlay1 = driver.FindElement((OR.GetElement("EventManagement", "PremiumVideoText", "TVWebPortalOR.xml"))).FindElements(By.TagName("div")).ToList();
            IWebElement delegAccessCode1 = eventOverlay1[2].FindElement(By.TagName("a"));
            delegAccessCode1.Click();


            iWait.Until(ExpectedConditions.ElementIsVisible((OR.GetElement("EventManagement", "EventModal", "TVWebPortalOR.xml"))));
            //driver.FindElement((OR.GetElement("EventManagement", "Delegate", "TVWebPortalOR.xml"))).SendKeys("automationUser");
            driver.FindElement((OR.GetElement("EventManagement", "EventAccessCode", "TVWebPortalOR.xml"))).SendKeys(eventAccessCode);

            //click on submit button
            driver.FindElement((OR.GetElement("EventManagement", "EventAccessCodeBTN", "TVWebPortalOR.xml"))).Click();
            uf.isJqueryActive(driver);
            iWait.Until(ExpectedConditions.InvisibilityOfElementLocated((OR.GetElement("EventManagement", "ModalContent", "TVWebPortalOR.xml"))));

            Assert.AreEqual(true, driver.FindElement((OR.GetElement("EventManagement", "VidPlayBTN", "TVWebPortalOR.xml"))).Displayed);

            //Assert.AreEqual(false, driver.FindElement((OR.GetElement("EventManagement", "PremiumVideoText", "TVWebPortalOR.xml"))).Displayed);
        }

        public void EventLogin()
        {
            iWait.Until(ExpectedConditions.ElementIsVisible((OR.GetElement("EventManagement", "confirmmsg", "TVWebPortalOR.xml"))));

            driver.FindElement((OR.GetElement("EventManagement", "eventLogin", "TVWebPortalOR.xml"))).Click();

            string userName = cf.readingXMLFile("WebPortal", "CorporateUser", "corpUserName", "Config.xml");
            string userPassword = cf.readingXMLFile("WebPortal", "CorporateUser", "corpPassWord", "Config.xml");

            log.Info("username    " + userName);

            log.Info("Password    " + userPassword);

            iWait.Until(ExpectedConditions.ElementIsVisible(OR.GetElement("VideoRequestByUser", "UserNameTB", "TVWebPortalOR.xml")));

            driver.FindElement(OR.GetElement("VideoRequestByUser", "UserName", "TVWebPortalOR.xml")).SendKeys(userName);
            Thread.Sleep(1000);

            driver.FindElement(OR.GetElement("VideoRequestByUser", "Password", "TVWebPortalOR.xml")).SendKeys(userPassword);

            iWait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy(OR.GetElement("VideoRequestByUser", "Login", "TVWebPortalOR.xml")));

            driver.FindElement(OR.GetElement("VideoRequestByUser", "Login", "TVWebPortalOR.xml")).Click();
        }

        public void welcomeMessage()
        {
            //Handling pop up message
            iWait.Until(ExpectedConditions.ElementIsVisible((OR.GetElement("VideoManagement", "LoginWelcomeMsg", "TVWebPortalOR.xml"))));

            driver.FindElement((OR.GetElement("VideoManagement", "WelcomeMsgCSS", "TVWebPortalOR.xml"))).Click();

            iWait.Until(ExpectedConditions.InvisibilityOfElementLocated((OR.GetElement("VideoManagement", "LoginWelcomeMsg", "TVWebPortalOR.xml"))));
        }

        public void ClickYesButton()
        {

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
                //driver.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromSeconds(10));
                Thread.Sleep(1000);
                driver.FindElement(element);
                return true;
            }
            catch (Exception)
            {

                return false;
            }
        }

        public void VerifyPremiumEvent()
        {
            Console.WriteLine("Inside verify PremiumVideo :::::");

            //  Assert.AreEqual(true,driver.FindElement((OR.GetElement("EventManagement", "PremiumVideoText", "TVWebPortalOR.xml"))).Displayed);

            Assert.AreEqual(true, driver.FindElement((OR.GetElement("VideoManagement", "LockBtn", "TVWebPortalOR.xml"))).Displayed);



        }

        public void VerifyFreeEvent()
        {
            Console.WriteLine("Inside verify FreeVideo :::::");

            Assert.AreEqual(true, driver.FindElement((OR.GetElement("EventManagement", "VidPlayBTN", "TVWebPortalOR.xml"))).Displayed);
        }

        public void VerifyPromoImage(String promoDetails)
        {
            //verify FreePromoImage image
            Console.WriteLine("Inside verifyPromoImage :::::");

            String promoDownloadImageName = "actualEventPromoImage.jpg";

            int integer = driver.FindElement((OR.GetElement("VideoManagement", "BackgroundImg", "TVWebPortalOR.xml"))).GetCssValue("background-image").Substring(5).IndexOf("?");
            String imageLinkUrl = driver.FindElement((OR.GetElement("VideoManagement", "BackgroundImg", "TVWebPortalOR.xml"))).GetCssValue("background-image").Substring(5, integer);

            string downloadPath = Environment.CurrentDirectory + "\\Upload\\DownloadedImages\\";

            uf.download(imageLinkUrl, downloadPath, promoDownloadImageName);

            string promoImagePath = Environment.CurrentDirectory + "\\Upload\\Images\\" + promoDetails;

            Assert.AreEqual(true, uf.imageComparision(new Bitmap(downloadPath + promoDownloadImageName), new Bitmap(promoImagePath)));

        }

        public void VerifyDefaultImage()
        {

            Console.WriteLine("Inside  VerifyDefaultImage :::::");

            String defaultDownloadImageName = "actualEventDefaultImage.jpg";

            iWait.Until(ExpectedConditions.ElementIsVisible((OR.GetElement("VideoManagement", "BackgroundImg", "TVWebPortalOR.xml"))));
            //String imageLinkUrl = driver.FindElement((OR.GetElement("VideoManagement", "BackgroundImg", "TVWebPortalOR.xml"))).GetAttribute("src");
            int integer = driver.FindElement((OR.GetElement("VideoManagement", "BackgroundImg", "TVWebPortalOR.xml"))).GetCssValue("background-image").Substring(5).IndexOf("?");
            String imageLinkUrl = driver.FindElement((OR.GetElement("VideoManagement", "BackgroundImg", "TVWebPortalOR.xml"))).GetCssValue("background-image").Substring(5, integer);
            string downloadPath = Environment.CurrentDirectory + "\\Upload\\DownloadedImages\\";

            uf.download(imageLinkUrl, downloadPath, defaultDownloadImageName);

            string defaultImageServerPath = cf.readingXMLFile("AdminPortal", "Video_Management", "WebImagesServerPath", "Config.xml");

            Console.WriteLine("defaultImageServerPath  " + defaultImageServerPath);

            String defaultImageName = cf.readingXMLFile("AdminPortal", "Event_Management", "EventDefaultLogo", "Config.xml");

            Console.WriteLine("defaultImageName" + defaultImageName);

            Assert.AreEqual(true, uf.imageComparision(new Bitmap(downloadPath + defaultDownloadImageName), new Bitmap(defaultImageServerPath + defaultImageName)));

        }

        public void VerifyCountDownDetails()
        {
            Thread.Sleep(1000);
            Console.WriteLine("Inside VerifyCountDownDetails::::::::::");

            Assert.AreEqual(true, driver.FindElement((OR.GetElement("VideoManagement", "CountDown", "TVWebPortalOR.xml"))).Displayed);

            String countDownTemp = driver.FindElement((OR.GetElement("VideoManagement", "CountDown", "TVWebPortalOR.xml"))).FindElement(By.TagName("span")).Text.Trim();

            Assert.AreEqual("1d", countDownTemp.Split(' ')[0]);
        }

        // This function verify Sponsor on video landing page
        public void VerifySponsorDetails(Dictionary<String, String> adminSponsorDetails)
        {
            Console.WriteLine("VerifySponsorDetails::::");

            iWait.Until(ExpectedConditions.ElementExists((OR.GetElement("SeriesManagement", "Sponserdiv", "TVWebPortalOR.xml"))));

            var elem = driver.FindElement((OR.GetElement("SeriesManagement", "Sponserdiv", "TVWebPortalOR.xml")));
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(true);", elem);

            //1.verify sponsor image

            Console.WriteLine("verify sponsor image:::::");

            String sponsorSystemImageName = adminSponsorDetails["imageName"];

            String sponsorDownloadImageName = "actualSponsorImage.jpg";

            string imageLinkUrl = driver.FindElement((OR.GetElement("SeriesManagement", "ImageLinkUrl", "TVWebPortalOR.xml"))).GetAttribute("src");

            string downloadPath = Environment.CurrentDirectory + "\\Upload\\DownloadedImages\\";

            uf.download(imageLinkUrl, downloadPath, sponsorDownloadImageName);

            string sponsorImagePath = Environment.CurrentDirectory + "\\Upload\\Images\\" + adminSponsorDetails["imageName"];

            Assert.AreEqual(true, uf.imageComparision(new Bitmap(downloadPath + sponsorDownloadImageName), new Bitmap(sponsorImagePath)));

            //2.verify sponsor image url STATUS
            Console.WriteLine("verify sponsor image url Status:::::");

            String str = "'";

            string sponsorRedirectUrl = driver.FindElement((OR.GetElement("SeriesManagement", "SponsorRedirectUrl", "TVWebPortalOR.xml"))).GetAttribute("onclick");

            String url = sponsorRedirectUrl.Split(Convert.ToChar(str))[1];

            Console.WriteLine("urlaftersplit :: " + sponsorRedirectUrl.Split(Convert.ToChar(str))[1]);

            Assert.AreEqual("OK", uf.getStatusCode(new Uri(url)));

        }

        public void SelectVideoFromVideoSheduleSection(String videoName)
        {
            Console.WriteLine("SelectVideoFromVideoSheduleSection::::");

            uf.scrollDown(driver);

            Thread.Sleep(2000);

            List<IWebElement> videoList = pnlVideoSchedule().FindElements((OR.GetElement("EventManagement", "EventScheduleContainer", "TVWebPortalOR.xml"))).ToList();

            Console.WriteLine("Video List Size:" + videoList.Count);

            Boolean flag = false;
            //item-description
            foreach (IWebElement video in videoList)
            {
                iWait.Until(ExpectedConditions.ElementExists((OR.GetElement("EventManagement", "ItemDesc", "TVWebPortalOR.xml"))));

                IWebElement videoDescription = video.FindElement((OR.GetElement("EventManagement", "ItemDesc", "TVWebPortalOR.xml"))).FindElement(By.TagName("a"));

                Console.WriteLine("Video Name:" + videoName);

                Console.WriteLine("Video Description:" + videoDescription.Text.Trim());

                if (videoName.Equals(videoDescription.Text.Trim()))
                {
                    flag = true;

                    videoDescription.Click();
                    break;
                }
            }

            Assert.AreEqual(true, flag); // if fails then video is not present in Shedule section.
        }

        public void GetEventVideoStatus(String videoName, string videoStatusAdmin)
        {
            Console.WriteLine("Inside GetEventVideoStatus::");
            uf.scrollDown(driver);
            Thread.Sleep(3000);

            List<IWebElement> videoList = pnlVideoSchedule().FindElements((OR.GetElement("EventManagement", "EventScheduleContainer", "TVWebPortalOR.xml"))).ToList();
            Boolean flag = false;
            //item-description
            foreach (IWebElement video in videoList)
            {

                IWebElement videoDescription = video.FindElement((OR.GetElement("EventManagement", "ItemDesc", "TVWebPortalOR.xml"))).FindElement(By.TagName("a"));

                if (videoName.Equals(videoDescription.Text.Trim()))
                {
                    flag = true;

                    if (videoStatusAdmin.Equals("Set as Reminder"))
                    {
                        String videoStatusWeb = video.FindElement((OR.GetElement("EventManagement", "SetReminder", "TVWebPortalOR.xml"))).Text.Trim();
                        Console.WriteLine("video status from webportal::: " + videoStatusWeb);
                        Assert.AreEqual(videoStatusAdmin, videoStatusWeb);
                    }
                    else
                    {
                        iWait.Until(ExpectedConditions.ElementExists((OR.GetElement("EventManagement", "VideoStatus", "TVWebPortalOR.xml"))));
                        String videoStatusWeb = video.FindElement((OR.GetElement("EventManagement", "VideoStatus", "TVWebPortalOR.xml"))).FindElement(By.TagName("span")).Text.Trim();
                        Console.WriteLine("video status from webportal::: " + videoStatusWeb);
                        Assert.AreEqual(videoStatusAdmin, videoStatusWeb);
                    }
                    break;
                }
            }

            Assert.AreEqual(true, flag); // if fails then video is not present in Shedule section.
        }

        public void SearchAndClickVideoSchedule(String videoEventDayAdmin)
        {
            Console.WriteLine("SearchAndClickVideoSchedule::::");
            Boolean flag = false;
            List<IWebElement> scheduleDay = driver.FindElement((OR.GetElement("EventManagement", "VideoScheduleTabs", "TVWebPortalOR.xml"))).FindElements(By.TagName("li")).ToList();

            scheduleDay.RemoveAt(0);

            foreach (IWebElement temp in scheduleDay)
            {
                String dayWeb = temp.FindElement(By.TagName("a")).Text.Trim();
                Console.WriteLine("Day from webportal  :" + dayWeb);
                if (dayWeb.Equals(videoEventDayAdmin))
                {
                    flag = true;
                    Thread.Sleep(3000);
                    temp.FindElement(By.TagName("a")).Click();
                    uf.isJqueryActive(driver);
                    break;
                }

            }
        }

        public void VerifyEventURL(String eventURLAdmin)
        {
            log.Info("Inside VerifyEventURL :::::");

            String actualEventURLContent = driver.FindElement((OR.GetElement("EventManagement", "EventWebsite", "TVWebPortalOR.xml"))).Text.Trim();
            Console.WriteLine("actualEventURLContent : " + actualEventURLContent);

            Assert.AreEqual(eventURLAdmin, actualEventURLContent);

            log.Info("VerifyEventURL Status:::::");

            Assert.AreEqual("OK", uf.getStatusCode(new Uri(actualEventURLContent)));

        }


    }
}

