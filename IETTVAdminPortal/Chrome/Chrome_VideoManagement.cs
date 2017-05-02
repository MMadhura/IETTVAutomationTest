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
using System.Globalization;

namespace IETTVAdminPortal.Chrome
{

    [TestFixture]
    public class Chrome_VideoManagement
    {

        // This is to configure logger mechanism for Utilities.Config
        private static ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        #region variable declaration and object initialisation

        IJavaScriptExecutor executor;

        String videoTitleUsedInRecentVideoSection = "";

        string driverName = "", driverPath, appURL;

        internal IWebDriver driver = null;

        IWait<IWebDriver> iWait = null;

        String abstractContent = "A paragraph from the Ancient Greek paragraphos.";

        String guid_Admin, videoID_Admin, video_AccessCode;

        public string videoName;

        IWebElement CopyrightuploadButton, SuccessBannerMessage, OkButton, chooseFileButton;

        //Instantiating Utilities function class

        Utility_Functions uf = new Utility_Functions();                                         // Instantiate object for Utility Functons

        Configuration cf = new Configuration();                                                 // Instantiate object for Configuration

        Object_Repository_Class OR = new Object_Repository_Class();

        Chrome_AdminSetupTearDown st = new Chrome_AdminSetupTearDown();

        Chrome_VideoManagementVerification videoResult;

        // Chrome_VideoSearchResult 

        #endregion

        public Chrome_VideoManagement(IWebDriver driver, ILog log1, IJavaScriptExecutor Executor, IWait<IWebDriver> iWait)
        {
            this.driver = driver;

            log = log1;

            this.executor = Executor;

            this.iWait = iWait;

        }


        public Chrome_VideoManagement()
        {

        }


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

        #region Resuable elements

        public IWebElement adminDropdown()
        {
            iWait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy((OR.GetElement("VideoRequestByUser", "AdminMenu", "TVAdminPortalOR.xml"))));

            return driver.FindElement((OR.GetElement("VideoRequestByUser", "AdminMenu", "TVAdminPortalOR.xml")));
        }

        public IWebElement videoManagementLink()
        {
            iWait.Until(ExpectedConditions.ElementToBeClickable((OR.GetElement("VideoManagement", "VidManagementLink", "TVAdminPortalOR.xml"))));

            return driver.FindElement((OR.GetElement("VideoManagement", "VidManagementLink", "TVAdminPortalOR.xml")));
        }

        public IWebElement addNewButton()
        {
            iWait.Until(ExpectedConditions.ElementToBeClickable((OR.GetElement("VideoManagement", "AddNewBTN", "TVAdminPortalOR.xml"))));

            return driver.FindElement((OR.GetElement("VideoManagement", "AddNewBTN", "TVAdminPortalOR.xml")));
        }

        public IWebElement videoTitleField()
        {
            iWait.Until(ExpectedConditions.ElementExists((OR.GetElement("VideoRequestByUser", "VideoTitle", "TVAdminPortalOR.xml"))));

            return driver.FindElement((OR.GetElement("VideoRequestByUser", "VideoTitle", "TVAdminPortalOR.xml")));
        }

        public IWebElement videoGuidField()
        {
            iWait.Until(ExpectedConditions.ElementExists((OR.GetElement("VideoRequestByUser", "GuAdminId", "TVAdminPortalOR.xml"))));

            return driver.FindElement((OR.GetElement("VideoRequestByUser", "GuAdminId", "TVAdminPortalOR.xml")));
        }

        public IWebElement videoIdField()
        {
            iWait.Until(ExpectedConditions.ElementExists((OR.GetElement("VideoRequestByUser", "GuAdminId", "TVAdminPortalOR.xml"))));

            return driver.FindElement((OR.GetElement("VideoRequestByUser", "GuAdminId", "TVAdminPortalOR.xml")));
        }

        public IWebElement videoDescription()
        {
            iWait.Until(ExpectedConditions.ElementExists((OR.GetElement("VideoManagement", "ShortDesc", "TVAdminPortalOR.xml"))));

            return driver.FindElement((OR.GetElement("VideoManagement", "ShortDesc", "TVAdminPortalOR.xml")));
        }

        public IWebElement videoType()
        {
            iWait.Until(ExpectedConditions.ElementExists((OR.GetElement("VideoRequestByUser", "VideoTypeDDL", "TVAdminPortalOR.xml"))));

            return driver.FindElement((OR.GetElement("VideoRequestByUser", "VideoTypeDDL", "TVAdminPortalOR.xml")));
        }

        public IWebElement channelTab()
        {

            return driver.FindElement(OR.GetElement("EventManagement", "ChannellistTab", "TVAdminPortalOR.xml"));
        }

        public IWebElement channelDefaultDropDown()
        {
            iWait.Until(ExpectedConditions.ElementIsVisible((OR.GetElement("VideoManagement", "DefaultChannelDDL", "TVAdminPortalOR.xml"))));

            return driver.FindElement((OR.GetElement("VideoManagement", "DefaultChannelDDL", "TVAdminPortalOR.xml")));
        }

        public IWebElement pricingTab()
        {
            return driver.FindElement((OR.GetElement("VideoManagement", "PricingTab", "TVAdminPortalOR.xml")));
        }

        public IWebElement tabPermission()
        {
            return driver.FindElement((OR.GetElement("VideoManagement", "PermissionTab", "TVAdminPortalOR.xml")));
        }

        public IWebElement chkDisplayPolling()
        {
            iWait.Until(ExpectedConditions.ElementToBeClickable((OR.GetElement("VideoManagement", "DisplayPolling", "TVAdminPortalOR.xml"))));

            return driver.FindElement((OR.GetElement("VideoManagement", "DisplayPolling", "TVAdminPortalOR.xml")));
        }

        public IWebElement chkDisplayQA()
        {
            iWait.Until(ExpectedConditions.ElementToBeClickable((OR.GetElement("VideoManagement", "DisplayQA", "TVAdminPortalOR.xml"))));

            return driver.FindElement((OR.GetElement("VideoManagement", "DisplayQA", "TVAdminPortalOR.xml")));
        }

        public IWebElement freeRadioButton()
        {
            iWait.Until(ExpectedConditions.ElementToBeClickable((OR.GetElement("VideoManagement", "FreeRadioBTN", "TVAdminPortalOR.xml"))));

            return driver.FindElement((OR.GetElement("VideoManagement", "FreeRadioBTN", "TVAdminPortalOR.xml")));
        }

        public IWebElement subscriptionRadioButton()
        {
            iWait.Until(ExpectedConditions.ElementToBeClickable((OR.GetElement("VideoManagement", "SubscriptionRadioBTN", "TVAdminPortalOR.xml"))));

            return driver.FindElement((OR.GetElement("VideoManagement", "SubscriptionRadioBTN", "TVAdminPortalOR.xml")));
        }

        public IWebElement premiumRadioButton()
        {
            iWait.Until(ExpectedConditions.ElementToBeClickable((OR.GetElement("VideoManagement", "PremiumRadioBTN", "TVAdminPortalOR.xml"))));

            return driver.FindElement((OR.GetElement("VideoManagement", "PremiumRadioBTN", "TVAdminPortalOR.xml")));
        }

        public IWebElement eventPricedRadioButton()
        {
            iWait.Until(ExpectedConditions.ElementToBeClickable((OR.GetElement("VideoManagement", "EventPricedRadioBtn", "TVAdminPortalOR.xml"))));

            return driver.FindElement((OR.GetElement("VideoManagement", "EventPricedRadioBtn", "TVAdminPortalOR.xml")));
        }

        public IWebElement videoPrice()
        {
            iWait.Until(ExpectedConditions.ElementIsVisible((OR.GetElement("VideoManagement", "VideoPrice", "TVAdminPortalOR.xml"))));

            return driver.FindElement((OR.GetElement("VideoManagement", "VideoPrice", "TVAdminPortalOR.xml")));
        }

        public IWebElement promoTab()
        {
            return driver.FindElement((OR.GetElement("VideoManagement", "PromoTab", "TVAdminPortalOR.xml")));
        }

        public IWebElement showCountDown()
        {
            iWait.Until(ExpectedConditions.ElementIsVisible((OR.GetElement("VideoManagement", "ShowCountDown", "TVAdminPortalOR.xml"))));

            return driver.FindElement((OR.GetElement("VideoManagement", "ShowCountDown", "TVAdminPortalOR.xml")));
        }


        public IWebElement copyrightTab()
        {
            iWait.Until(ExpectedConditions.ElementToBeClickable((OR.GetElement("VideoManagement", "CopyrightTab", "TVAdminPortalOR.xml"))));

            return driver.FindElement((OR.GetElement("VideoManagement", "CopyrightTab", "TVAdminPortalOR.xml")));
        }

        public IWebElement notesField()
        {
            return driver.FindElement((OR.GetElement("VideoManagement", "NotesField", "TVAdminPortalOR.xml")));
        }

        public IWebElement copyrightUploadButton()
        {
            return driver.FindElement((OR.GetElement("VideoManagement", "CopyrightUpldBTN", "TVAdminPortalOR.xml")));
        }

        public IWebElement keywordsTabButton()
        {
            iWait.Until(ExpectedConditions.ElementToBeClickable((OR.GetElement("VideoManagement", "KeyworrdsTabBTN", "TVAdminPortalOR.xml"))));

            return driver.FindElement((OR.GetElement("VideoManagement", "KeyworrdsTabBTN", "TVAdminPortalOR.xml")));
        }

        public IWebElement keywordsTextField()
        {
            iWait.Until(ExpectedConditions.ElementExists((OR.GetElement("VideoManagement", "KeywordsText", "TVAdminPortalOR.xml"))));

            return driver.FindElement((OR.GetElement("VideoManagement", "KeywordsText", "TVAdminPortalOR.xml")));
        }

        public IWebElement addTagButton()
        {
            iWait.Until(ExpectedConditions.ElementExists((OR.GetElement("VideoManagement", "AddTagBTN", "TVAdminPortalOR.xml"))));

            return driver.FindElement((OR.GetElement("VideoManagement", "AddTagBTN", "TVAdminPortalOR.xml")));
        }

        public IWebElement attachmentTab()
        {
            return driver.FindElement((OR.GetElement("VideoManagement", "AttachementTab", "TVAdminPortalOR.xml")));
        }

        public IWebElement attachmentUploadButton()
        {
            return driver.FindElement((OR.GetElement("VideoManagement", "AttachementUpldBTN", "TVAdminPortalOR.xml")));
        }

        public IWebElement sponsorTextField()
        {
            return driver.FindElement((OR.GetElement("VideoManagement", "SponsorTXT", "TVAdminPortalOR.xml")));
        }

        public IWebElement sponsorUploadButton()
        {
            return driver.FindElement((OR.GetElement("VideoManagement", "SponsorUpldBTN", "TVAdminPortalOR.xml")));
        }

        public IWebElement speakerTab()
        {
            return driver.FindElement((OR.GetElement("VideoManagement", "SpeakerTab", "TVAdminPortalOR.xml")));
        }

        public IWebElement createSpeakerTab()
        {
            iWait.Until(ExpectedConditions.ElementExists((OR.GetElement("VideoManagement", "CreateSpeakerTab", "TVAdminPortalOR.xml"))));

            return driver.FindElement((OR.GetElement("VideoManagement", "CreateSpeakerTab", "TVAdminPortalOR.xml")));
        }

        public IWebElement speakerTitle()
        {
            iWait.Until(ExpectedConditions.ElementExists((OR.GetElement("VideoManagement", "SpeakerTitle", "TVAdminPortalOR.xml"))));

            return driver.FindElement((OR.GetElement("VideoManagement", "SpeakerTitle", "TVAdminPortalOR.xml")));
        }

        public IWebElement speakerFirstName()
        {
            return driver.FindElement((OR.GetElement("VideoManagement", "SpeakerFirstName", "TVAdminPortalOR.xml")));
        }

        public IWebElement speakerLastName()
        {
            return driver.FindElement((OR.GetElement("VideoManagement", "SpeakerLastName", "TVAdminPortalOR.xml")));
        }

        public IWebElement speakerAddButton()
        {
            return driver.FindElement((OR.GetElement("VideoManagement", "SpeakerAddBTN", "TVAdminPortalOR.xml")));
        }

        public IWebElement uploadVideoTab()
        {
            return driver.FindElement((OR.GetElement("VideoManagement", "UploadVidTab", "TVAdminPortalOR.xml")));
        }

        public IWebElement uploadVideoButton()
        {
            return driver.FindElement((OR.GetElement("VideoManagement", "UploadVidBTN", "TVAdminPortalOR.xml")));
        }

        public IWebElement videoPreviewButton()
        {
            return driver.FindElement((OR.GetElement("VideoManagement", "VidPreviewBTN", "TVAdminPortalOR.xml")));
        }

        public IWebElement publishTab()
        {
            return driver.FindElement((OR.GetElement("VideoManagement", "PublishTab", "TVAdminPortalOR.xml")));
        }

        public IWebElement finalPublishFromDate()
        {
            iWait.Until(ExpectedConditions.ElementToBeClickable((OR.GetElement("VideoManagement", "FinalPublishFromDate", "TVAdminPortalOR.xml"))));

            return driver.FindElement((OR.GetElement("VideoManagement", "FinalPublishFromDate", "TVAdminPortalOR.xml")));
        }

        public IWebElement finalPublishFromTime()
        {
            iWait.Until(ExpectedConditions.ElementToBeClickable((OR.GetElement("VideoManagement", "FinalPublishFromTime", "TVAdminPortalOR.xml"))));

            return driver.FindElement((OR.GetElement("VideoManagement", "FinalPublishFromTime", "TVAdminPortalOR.xml")));
        }

        public IWebElement videoPublishRadioButton()
        {
            return driver.FindElement((OR.GetElement("VideoManagement", "VidPublishRadioBTN", "TVAdminPortalOR.xml")));
        }

        public IWebElement videoPublishButton()
        {
            iWait.Until(ExpectedConditions.ElementToBeClickable(((OR.GetElement("VideoManagement", "PublishVidBTN", "TVAdminPortalOR.xml")))));

            return driver.FindElement((OR.GetElement("VideoManagement", "PublishVidBTN", "TVAdminPortalOR.xml")));
        }

        public IWebElement finalPublishExpiryDate()
        {
            iWait.Until(ExpectedConditions.ElementToBeClickable((OR.GetElement("VideoManagement", "FinalPublishExpDT", "TVAdminPortalOR.xml"))));

            return driver.FindElement((OR.GetElement("VideoManagement", "FinalPublishExpDT", "TVAdminPortalOR.xml")));
        }

        public IWebElement finalPublishExpiryTime()
        {
            iWait.Until(ExpectedConditions.ElementToBeClickable((OR.GetElement("VideoManagement", "FinalPublishExpTime", "TVAdminPortalOR.xml"))));

            return driver.FindElement((OR.GetElement("VideoManagement", "FinalPublishExpTime", "TVAdminPortalOR.xml")));
        }

        public IWebElement recordPublishFromDate()
        {
            iWait.Until(ExpectedConditions.ElementToBeClickable((OR.GetElement("VideoManagement", "RecordPublishFromDT", "TVAdminPortalOR.xml"))));

            return driver.FindElement((OR.GetElement("VideoManagement", "RecordPublishFromDT", "TVAdminPortalOR.xml")));
        }

        public IWebElement recordPublishFromTime()
        {
            iWait.Until(ExpectedConditions.ElementToBeClickable((OR.GetElement("VideoManagement", "RecrodPublishFromTime", "TVAdminPortalOR.xml"))));

            return driver.FindElement((OR.GetElement("VideoManagement", "RecrodPublishFromTime", "TVAdminPortalOR.xml")));
        }

        public IWebElement uploadFtpRadioButton()
        {
            iWait.Until(ExpectedConditions.ElementToBeClickable((OR.GetElement("VideoManagement", "UploadFTPRadioBTN", "TVAdminPortalOR.xml"))));

            return driver.FindElement((OR.GetElement("VideoManagement", "UploadFTPRadioBTN", "TVAdminPortalOR.xml")));
        }

        public IWebElement hideRecordRadioButton()
        {
            iWait.Until(ExpectedConditions.ElementExists((OR.GetElement("VideoManagement", "HideRecordRadioBTN", "TVAdminPortalOR.xml"))));

            return driver.FindElement((OR.GetElement("VideoManagement", "HideRecordRadioBTN", "TVAdminPortalOR.xml")));
        }

        public IWebElement countdownFromDate()
        {
            Thread.Sleep(3000);

            iWait.Until(ExpectedConditions.ElementToBeClickable((OR.GetElement("VideoManagement", "CountDownFromDate", "TVAdminPortalOR.xml"))));

            return driver.FindElement((OR.GetElement("VideoManagement", "CountDownFromDate", "TVAdminPortalOR.xml")));
        }

        public IWebElement countdownFromTime()
        {
            iWait.Until(ExpectedConditions.ElementToBeClickable((OR.GetElement("VideoManagement", "CountDownFromTime", "TVAdminPortalOR.xml"))));

            return driver.FindElement((OR.GetElement("VideoManagement", "CountDownFromTime", "TVAdminPortalOR.xml")));
        }

        public IWebElement advanceTab()
        {
            Thread.Sleep(3000);

            iWait.Until(ExpectedConditions.ElementIsVisible((OR.GetElement("VideoManagement", "AdvanceTab", "TVAdminPortalOR.xml"))));

            return driver.FindElement((OR.GetElement("VideoManagement", "AdvanceTab", "TVAdminPortalOR.xml"))).FindElement(By.TagName("a"));
        }

        public IWebElement rdoLiveVideo()
        {
            iWait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy((OR.GetElement("VideoManagement", "RadioLiveVid", "TVAdminPortalOR.xml"))));

            return driver.FindElement((OR.GetElement("VideoManagement", "RadioLiveVid", "TVAdminPortalOR.xml")));
        }

        public IWebElement accessCodeTab()
        {
            return driver.FindElement((OR.GetElement("VideoManagement", "AccessCodeTab", "TVAdminPortalOR.xml")));
        }

        public IWebElement generateVideoAccessButton()
        {
            iWait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy((OR.GetElement("VideoManagement", "GenerateVideoAccessBtn", "TVAdminPortalOR.xml"))));

            return driver.FindElement((OR.GetElement("VideoManagement", "GenerateVideoAccessBtn", "TVAdminPortalOR.xml")));
        }

        public IWebElement videoAccessCodeField()
        {
            iWait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy((OR.GetElement("VideoManagement", "VideoAccessField", "TVAdminPortalOR.xml"))));

            return driver.FindElement((OR.GetElement("VideoManagement", "VideoAccessField", "TVAdminPortalOR.xml")));
        }

        public IWebElement eventCodeTab()
        {
            return driver.FindElement((OR.GetElement("VideoManagement", "EventCodeTab", "TVAdminPortalOR.xml")));
        }

        public IWebElement eventSearchTitle()
        {
            iWait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy((OR.GetElement("VideoManagement", "EventSearchTitle", "TVAdminPortalOR.xml"))));

            return driver.FindElement((OR.GetElement("VideoManagement", "EventSearchTitle", "TVAdminPortalOR.xml")));
        }

        public IWebElement eventSearchButton()
        {
            iWait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy((OR.GetElement("VideoManagement", "EventSearchBtn", "TVAdminPortalOR.xml"))));

            return driver.FindElement((OR.GetElement("VideoManagement", "EventSearchBtn", "TVAdminPortalOR.xml")));
        }

        public IWebElement eventSelectButton()
        {
            iWait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy((OR.GetElement("VideoManagement", "EventSelectBtn", "TVAdminPortalOR.xml"))));

            return driver.FindElement((OR.GetElement("VideoManagement", "EventSelectBtn", "TVAdminPortalOR.xml")));
        }

        public IWebElement eventLocationDropDown()
        {
            iWait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy((OR.GetElement("VideoManagement", "EventLocationDD", "TVAdminPortalOR.xml"))));

            return driver.FindElement((OR.GetElement("VideoManagement", "EventLocationDD", "TVAdminPortalOR.xml")));
        }

        public IWebElement eventVenueDropDown()
        {
            iWait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy((OR.GetElement("VideoManagement", "EventVenueDD", "TVAdminPortalOR.xml"))));

            return driver.FindElement((OR.GetElement("VideoManagement", "EventVenueDD", "TVAdminPortalOR.xml")));
        }

        public IWebElement eventRoomDropDown()
        {
            iWait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy((OR.GetElement("VideoManagement", "EventRoomDD", "TVAdminPortalOR.xml"))));

            return driver.FindElement((OR.GetElement("VideoManagement", "EventRoomDD", "TVAdminPortalOR.xml")));
        }

        public IWebElement eventToTimeField()
        {
            iWait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy((OR.GetElement("VideoManagement", "EventToTime", "TVAdminPortalOR.xml"))));

            return driver.FindElement((OR.GetElement("VideoManagement", "EventToTime", "TVAdminPortalOR.xml")));
        }
        #endregion


        #region Reusable Function

        public void OverlayWait()
        {
            iWait.Until(ExpectedConditions.InvisibilityOfElementLocated((OR.GetElement("SeriesManagement", "OverlayWait", "TVAdminPortalOR.xml"))));
        }

        public void verifySuccessBannerMessage(String message)
        {
            iWait.Until(ExpectedConditions.ElementIsVisible((OR.GetElement("SeriesManagement", "SuccessOkButton", "TVAdminPortalOR.xml"))));

            iWait.Until(d => d.FindElement((OR.GetElement("SeriesManagement", "SucessMessage", "TVAdminPortalOR.xml"))).Text.Equals(message));

            Assert.AreEqual(message, driver.FindElement((OR.GetElement("SeriesManagement", "SucessMessage", "TVAdminPortalOR.xml"))).Text.Trim());

            //Click on ok button banner message

            Thread.Sleep(2000);

            driver.FindElement((OR.GetElement("SeriesManagement", "SuccessOkButton", "TVAdminPortalOR.xml"))).Click();

            OverlayWait();

        }

        //selecting video management link from Admin Dropdown
        public void redirectToVideoManagement()
        {

            log.Info("inside redirectToVideoManagement " + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

            //clicking on Admin dropdown   
            adminDropdown().Click();

            Thread.Sleep(3000);

            //Clicking on video Management Link

            videoManagementLink().Click();

            //Click on Add New Button
            addNewButton().Click();
        }


        //This function fill the details on the basic Info tab
        public void basicInfoTab()
        {
            String videoID_Admin;

            log.Info("inside basicInfoTab " + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

            //getting GUID of the current video
            guid_Admin = videoGuidField().GetAttribute("value");

            log.Info("Guid_Admin:: " + guid_Admin);

            videoID_Admin = videoIdField().GetAttribute("value");

            log.Info("VideoID_Admin:: " + videoID_Admin);

            //getting the uique name for the video title
            //videoName = "vid" + uf.getShortGuid().Substring(0, 10);
            videoName = "vid" + uf.getShortGuid();
            log.Info("Video name  : " + videoName);

            //Store the video details in sysconfig.xml file
            cf.writingIntoXML("AdminPortal", "VideoManagement", "VideoName", videoName, "SysConfig.xml");
            cf.writingIntoXML("AdminPortal", "VideoManagement", "VideoID", videoID_Admin, "SysConfig.xml");
            cf.writingIntoXML("AdminPortal", "VideoManagement", "Guid", guid_Admin, "SysConfig.xml");

            //Enter data in Title field

            videoTitleField().SendKeys(videoName);

            videoTitleUsedInRecentVideoSection = videoName;

            //Enter data in ShortDescription field
            videoDescription().SendKeys("This field is for writing Description");

            int size = driver.FindElements(By.TagName("iframe")).Count();

            Console.WriteLine("Total Frames:" + size);

            Thread.Sleep(5000);

            //Enter data into abstract field
            iWait.Until(ExpectedConditions.ElementExists((OR.GetElement("VideoManagement", "Abstract", "TVAdminPortalOR.xml"))));

            IWebElement abstract_frame = driver.FindElement((OR.GetElement("VideoManagement", "Abstract", "TVAdminPortalOR.xml")));

            driver.SwitchTo().Frame(abstract_frame);

            IWebElement editor_body = driver.FindElement(By.TagName("body"));

            OpenQA.Selenium.Interactions.Actions action = new OpenQA.Selenium.Interactions.Actions(driver);
            action.SendKeys(editor_body, abstractContent).Build().Perform();
            driver.SwitchTo().DefaultContent();

            SelectElement VideoTypeSelector = new SelectElement(videoType());

            VideoTypeSelector.SelectByIndex(3);
            String selectedVideoType = VideoTypeSelector.SelectedOption.Text.Trim();

            String videoCreatedDate = driver.FindElement((OR.GetElement("VideoManagement", "VideoCreatedDT", "TVAdminPortalOR.xml"))).GetAttribute("value");
            log.Info("videoCreatedDate   :" + videoCreatedDate);

            cf.writingIntoXML("AdminPortal", "VideoManagement", "CreatedDate", videoCreatedDate, "SysConfig.xml");
        }

        //This function fill the details on the basic Info tab
        public String basicInfoTab(String initVideoName)
        {
            String videoID_Admin;

            log.Info("inside basicInfoTab " + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

            ////getting GUID of the current video
            guid_Admin = videoGuidField().GetAttribute("value");

            log.Info("Guid_Admin:: " + guid_Admin);

            videoID_Admin = videoIdField().GetAttribute("value");

            log.Info("VideoID_Admin:: " + videoID_Admin);

            //getting the uique name for the video title
            videoName = initVideoName + uf.getShortGuid();

            log.Info("Video name  : " + videoName);

            ////Store the video details in sysconfig.xml file
            cf.writingIntoXML("AdminPortal", "VideoManagement", "VideoName", videoName, "SysConfig.xml");
            //cf.writingIntoXML("AdminPortal", "VideoManagement", "VideoID", videoID_Admin, "SysConfig.xml");
            cf.writingIntoXML("AdminPortal", "VideoManagement", "Guid", guid_Admin, "SysConfig.xml");

            cf.writingIntoXML("Admin", "External Management", "VideoName", videoName, "TestCopy.xml");

            //Enter data in Title field

            videoTitleField().SendKeys(videoName);

            videoTitleUsedInRecentVideoSection = videoName;

            //Enter data in ShortDescription field
            videoDescription().SendKeys("This field is for writing Description");

            int size = driver.FindElements(By.TagName("iframe")).Count();

            Console.WriteLine("Total Frames:" + size);

            Thread.Sleep(5000);

            //Enter data into abstract field
            iWait.Until(ExpectedConditions.ElementExists((OR.GetElement("VideoManagement", "Abstract", "TVAdminPortalOR.xml"))));

            IWebElement abstract_frame = driver.FindElement((OR.GetElement("VideoManagement", "Abstract", "TVAdminPortalOR.xml")));

            driver.SwitchTo().Frame(abstract_frame);

            IWebElement editor_body = driver.FindElement(By.TagName("body"));

            OpenQA.Selenium.Interactions.Actions action = new OpenQA.Selenium.Interactions.Actions(driver);
            action.SendKeys(editor_body, abstractContent).Build().Perform();
            driver.SwitchTo().DefaultContent();

            SelectElement VideoTypeSelector = new SelectElement(videoType());

            VideoTypeSelector.SelectByIndex(3);
            String selectedVideoType = VideoTypeSelector.SelectedOption.Text.Trim();

            String videoCreatedDate = driver.FindElement((OR.GetElement("VideoManagement", "VideoCreatedDT", "TVAdminPortalOR.xml"))).GetAttribute("value");
            log.Info("videoCreatedDate   :" + videoCreatedDate);

            cf.writingIntoXML("AdminPortal", "VideoManagement", "CreatedDate", videoCreatedDate, "SysConfig.xml");

            return videoName;
        }


        //This function select the channel
        public String channelListTab()
        {
            log.Info("inside channelListTab " + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

            uf.scrollUp(driver);

            //Click on Channel tab
            channelTab().FindElement(By.TagName("a")).Click();

            //Selecting channel from the default Channel dropdown
            SelectElement DefaultChanneleSelector = new SelectElement(channelDefaultDropDown());

            //getting number of channels from default channel dropdown
            int NumberofChannels = DefaultChanneleSelector.Options.Count;
            log.Info("number of channels in default channel dropdown:: " + NumberofChannels);

            //check number of channel in the dropdown if it is zero then first create channel
            if (NumberofChannels == 0)
            {
                log.Info("No default channel Present:: ");
                System.Windows.Forms.Application.Exit();
                //Call Create Channel                
            }
            //String memberChannelName = cf.readingXMLFile("AdminPortal", "Channel_Management", "MemberChannelName", "Config.xml");

            String memberChannelName = cf.readingXMLFile("Admin", "Channel Management", "channelname", "TestCopy.xml");

            DefaultChanneleSelector.SelectByText(memberChannelName);

            String adminChannelName = DefaultChanneleSelector.SelectedOption.Text;

            log.Info("Selected channel name :" + adminChannelName);

            return adminChannelName;

        }

        //This function select the pricing type for the video
        public void pricingListTab(String pricingType)
        {
            log.Info("inside pricingListTab " + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

            //Click on Pricing tab
            pricingTab().FindElement(By.TagName("a")).Click();


            //Cliking on Pricing - 'free' radio button
            if (pricingType.Equals("Free"))
            {
                freeRadioButton().Click();
            }
            else if (pricingType.Equals("Subscription"))
            {
                subscriptionRadioButton().Click();

            }

            else if (pricingType.Equals("Premium"))
            {
                premiumRadioButton().Click();

                videoPrice().Clear();

                videoPrice().SendKeys("50.00");
            }

            else if (pricingType.Equals("Event Priced"))
            {
                eventPricedRadioButton().Click();

            }
        }

        //changes
        /// <summary>
        /// This function generates Video Access Code
        /// </summary>
        public void videoAccessCodeTab()
        {
            log.Info("inside accessCodeTab " + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

            //Click on Pricing tab
            accessCodeTab().FindElement(By.TagName("a")).Click();

            generateVideoAccessButton().Click();

            OverlayWait();

            video_AccessCode = videoAccessCodeField().GetAttribute("value");

            cf.writingIntoXML("Admin", "Video Management", "videoAccessCode", video_AccessCode, "TestCopy.xml");

        }

        public void eventTab()
        {
            log.Info("inside eventTab " + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

            eventCodeTab().FindElement(By.TagName("a")).Click();

            //to check
            string eventTitle = cf.readingXMLFile("Admin", "Video Management", "eventTitle", "TestCopy.xml");

            eventSearchTitle().SendKeys(cf.readingXMLFile("Admin", "Video Management", "eventTitle", "TestCopy.xml"));

            eventSearchButton().Click();

            eventSelectButton().Click();

            //Location dropdown
            SelectElement EventLocationDD = new SelectElement(eventLocationDropDown());
            EventLocationDD.SelectByIndex(1);

            //Venue Dropdown
            SelectElement EventVenueDD = new SelectElement(eventVenueDropDown());
            EventVenueDD.SelectByIndex(1);

            //Room DropDown
            SelectElement EventRoomDD = new SelectElement(eventRoomDropDown());
            EventRoomDD.SelectByIndex(1);
            
            //Event To Time
            String currentDate = DateTime.Now.AddMinutes(2).ToString("dd/MM/yyyy HHmm", CultureInfo.InvariantCulture);

            String[] dateTime = currentDate.Split(' ');

            eventToTimeField().SendKeys(dateTime[1].Trim());



        }
        //changes

        //This function fill the detials required in the Copyright Tab with 'Add copyright' details
        public void addcopyright()
        {
            log.Info("inside addcopyright " + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

            //Click on the CopyrightTab
            copyrightTab().FindElement(By.TagName("a")).Click();

            //reading copyright.txt file from xml
            String uploadCopyright = cf.readingXMLFile("AdminPortal", "Video_Management", "CopyrightUpload", "Config.xml");
            string uploadCopyrightPath = AppDomain.CurrentDomain.BaseDirectory + "\\Upload\\Documents\\" + uploadCopyright;

            iWait.Until(ExpectedConditions.ElementIsVisible((OR.GetElement("VideoManagement", "AttachmentUploadBTN", "TVAdminPortalOR.xml"))));    //choosefile button

            CopyrightuploadButton = driver.FindElement((OR.GetElement("VideoManagement", "CopyrightUplBTNCSS", "TVAdminPortalOR.xml")));

            //calling upload funtionalty from the Utility class to upload the required file
            uf.uploadfile(CopyrightuploadButton, uploadCopyrightPath);

            //Write the data in the Notes field
            notesField().SendKeys("This is notes field Automation_test");

            //Click on the upload button
            copyrightUploadButton().Click();

            verifySuccessBannerMessage("Record is Added Successfully.");

        }

        public void authorisationCopyright()
        {
            log.Info("inside authorisationCopyright " + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

            //Click on the CopyrightTab
            copyrightTab().FindElement(By.TagName("a")).Click();

            SelectElement copyrightdropdown = new SelectElement(driver.FindElement((OR.GetElement("VideoManagement", "CopyrightAuthorizedBy", "TVAdminPortalOR.xml"))));
            copyrightdropdown.SelectByIndex(2);

            OverlayWait();
            //iWait.Until(ExpectedConditions.ElementIsVisible(By.Id("ContentPlaceHolder1_ucVideosBasicInformationID_btnCopyrightAuthorisationAttachment")));    //choosefile buttons

            //reading copyright.txt file from xml
            String uploadCopyright = cf.readingXMLFile("AdminPortal", "Video_Management", "CopyrightUpload", "Config.xml");
            string uploadCopyrightPath = AppDomain.CurrentDomain.BaseDirectory + "\\Upload\\Documents\\" + uploadCopyright;

            iWait.Until(ExpectedConditions.ElementIsVisible((OR.GetElement("VideoManagement", "ChooseFileBTN", "TVAdminPortalOR.xml"))));    //choosefile button

            CopyrightuploadButton = driver.FindElement((OR.GetElement("VideoManagement", "CopyrightUplBTNCSS1", "TVAdminPortalOR.xml")));

            //calling upload funtionalty from the Utility class to upload the required file
            uf.uploadfile(CopyrightuploadButton, uploadCopyrightPath);

            //click on upload button
            driver.FindElement((OR.GetElement("VideoManagement", "ChooseFileBTN", "TVAdminPortalOR.xml"))).Click();

            verifySuccessBannerMessage("Record is Added Successfully.");

            //enter notes field
            driver.FindElement((OR.GetElement("VideoManagement", "AuthorizationNotes", "TVAdminPortalOR.xml"))).SendKeys("Automation test");

            //click allow publish
            driver.FindElement((OR.GetElement("VideoManagement", "ChkAllowPublish", "TVAdminPortalOR.xml"))).Click();

            uf.scrollUp(driver);
        }

        public String promotionalInformation(String promoType, String currentDate)
        {
            log.Info("inside addcopyright " + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

            promoTab().FindElement(By.TagName("a")).Click();

            if (promoType.Equals("Countdown"))
            {
                showCountDown().Click();

                DateTime enteredDate = Convert.ToDateTime(currentDate);

                //Getting sysytem current date and adding 1minutes in time as video upload time should be greater than system current time
                currentDate = enteredDate.AddMinutes(1).ToString("dd/MM/yyyy HHmm", CultureInfo.InvariantCulture);

                String[] dateTime = currentDate.Split(' ');

                log.Info("Countdown video date:: " + dateTime[0].Trim());

                log.Info("Countdown video Time :: " + dateTime[1].Trim());

                //Selecting todays date from date picker
                countdownFromDate().SendKeys(dateTime[0].Trim());

                Thread.Sleep(4000);

                //enter the time in the Final Time field
                countdownFromTime().Clear();

                countdownFromTime().SendKeys(dateTime[1].Trim());

                return null;

            }

            else if (promoType.Equals("Video"))
            {
                //promo video radio button

                iWait.Until(ExpectedConditions.ElementExists((OR.GetElement("VideoManagement", "PromoRadioBTN", "TVAdminPortalOR.xml"))));

                driver.FindElement((OR.GetElement("VideoManagement", "PromoRadioBTN", "TVAdminPortalOR.xml"))).Click();

                SelectElement promoDropdown = new SelectElement(driver.FindElement((OR.GetElement("VideoManagement", "PromotionalDD", "TVAdminPortalOR.xml"))));

                String promoVideoName = cf.readingXMLFile("AdminPortal", "VideoManagement", "PromoVideoName", "SysConfig.xml");

                //need to read from xml the recent created promo video
                promoDropdown.SelectByText(promoVideoName);

                return null;

            }

            else if (promoType.Equals("Image"))
            {
                //promo image radio button

                iWait.Until(ExpectedConditions.ElementExists((OR.GetElement("VideoManagement", "PromoImageBTN", "TVAdminPortalOR.xml"))));

                driver.FindElement((OR.GetElement("VideoManagement", "PromoImageBTN", "TVAdminPortalOR.xml"))).Click();

                //find the div sibling in order to get upload button
                IList<IWebElement> siblings = (IList<IWebElement>)driver.FindElements(By.XPath("//input[@id='promoImageUploadButton']/following-sibling::div"));

                chooseFileButton = siblings[0].FindElement(By.TagName("input"));

                //reading xml file to upload Video
                String uploadImage = cf.readingXMLFile("AdminPortal", "Channel_Management", "channelIcon", "Config.xml");
                string uploadImagePath = Environment.CurrentDirectory + "\\Upload\\Images\\" + uploadImage;
                uf.uploadfile(chooseFileButton, uploadImagePath);

                //upload button
                iWait.Until(ExpectedConditions.ElementExists((OR.GetElement("VideoManagement", "UploadImageBTN", "TVAdminPortalOR.xml"))));

                driver.FindElement((OR.GetElement("VideoManagement", "UploadImageBTN", "TVAdminPortalOR.xml"))).Click();

                verifySuccessBannerMessage("Record is Added Successfully.");

                return uploadImage;

            }

            else if (promoType.Equals("DefaultImage"))
            {
                //default image radio button

                iWait.Until(ExpectedConditions.ElementExists((OR.GetElement("VideoManagement", "DefaultImg", "TVAdminPortalOR.xml"))));

                driver.FindElement((OR.GetElement("VideoManagement", "DefaultImg", "TVAdminPortalOR.xml"))).Click();

                return null;
            }

            return null;


        }


        //This function Add the keywords to the video
        public String keywordsTab()
        {
            String keyword = "AutomationRave";

            log.Info("inside keywordsTab " + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

            //click on Keyword Tab
            keywordsTabButton().FindElement(By.TagName("a")).Click();

            //Waiting for loader
            iWait = new WebDriverWait(driver, TimeSpan.FromSeconds(60));
            iWait.Until(ExpectedConditions.ElementExists((OR.GetElement("VideoManagement", "Loading", "TVAdminPortalOR.xml"))));

            log.Info("Keyword loading is over");

            //Tag name to be enter
            keywordsTextField().SendKeys(keyword);

            //click on add tag button
            addTagButton().Click();

            return keyword;
        }

        //This function will attach a documents to the video
        public String uploadAttachmentTab()
        {
            log.Info("inside uploadAttachmentTab " + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

            //Click on the Attachment Tab
            attachmentTab().FindElement(By.TagName("a")).Click();


            //wait for upload button
            iWait.Until(ExpectedConditions.ElementToBeClickable((OR.GetElement("VideoManagement", "AttachementUpldBTN", "TVAdminPortalOR.xml"))));

            //Reading xml file to upload Attachment
            String uploadAttachment = cf.readingXMLFile("AdminPortal", "Video_Management", "AttachmentUpload", "Config.xml");
            string uploadAttachmenttPath = AppDomain.CurrentDomain.BaseDirectory + "\\Upload\\Documents\\" + uploadAttachment;

            log.Info("uploadAttachment: " + uploadAttachment);
            log.Info("uploadAttachmenttPath: " + uploadAttachmenttPath);

            chooseFileButton = driver.FindElement((OR.GetElement("VideoManagement", "ChooseFileBTNCSS", "TVAdminPortalOR.xml")));

            uf.uploadfile(chooseFileButton, uploadAttachmenttPath);

            //Click on upload button
            attachmentUploadButton().Click();

            verifySuccessBannerMessage("Record is added successfully.");

            return uploadAttachment;

        }

        public Dictionary<String, String> editSponsor()
        {
            log.Info("inside editSponsor " + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

            Dictionary<String, String> sponsorDetails = new Dictionary<String, String>();

            sponsorDetails.Add("url", "http://www.google.com");

            sponsorDetails.Add("imageName", cf.readingXMLFile("AdminPortal", "Channel_Management", "channelIcon", "Config.xml"));

            //Click on the Attachment Tab
            attachmentTab().FindElement(By.TagName("a")).Click();


            //wait for upload button
            iWait.Until(ExpectedConditions.ElementToBeClickable((OR.GetElement("VideoManagement", "AttachementUpldBTN", "TVAdminPortalOR.xml"))));

            sponsorTextField().SendKeys(sponsorDetails["url"]);

            string sponsorImagePath = AppDomain.CurrentDomain.BaseDirectory + "\\Upload\\Images\\" + sponsorDetails["imageName"];

            log.Info("uploadSponsorName: " + sponsorDetails["imageName"]);

            log.Info("uploadSponsorPath: " + sponsorImagePath);

            IWebElement sponsorChooseButton = driver.FindElement((OR.GetElement("VideoManagement", "SponsorChooseBTNCSS", "TVAdminPortalOR.xml")));

            uf.uploadfile(sponsorChooseButton, sponsorImagePath);

            //click on sponsor upload button
            sponsorUploadButton().Click();

            verifySuccessBannerMessage("Record is added successfully.");

            return sponsorDetails;
        }


        //This functioncreate speaker to the video
        public Dictionary<String, String> createSpeaker()
        {
            Dictionary<String, String> speakerDetails = new Dictionary<String, String>();

            speakerDetails.Add("title", "Mr");

            speakerDetails.Add("firstName", "Automation");

            speakerDetails.Add("lastName", "Rave");

            log.Info("inside createSpeakerTab " + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

            //click on speaker Tab
            speakerTab().FindElement(By.TagName("a")).Click();

            createSpeakerTab().Click();

            //title field
            speakerTitle().SendKeys(speakerDetails["title"]);

            //First name field
            speakerFirstName().SendKeys(speakerDetails["firstName"]);

            log.Info("Speaker first name  : " + speakerDetails["firstName"]);

            cf.writingIntoXML("AdminPortal", "VideoManagement", "SpeakerName", speakerDetails["firstName"], "SysConfig.xml");


            //last name field
            speakerLastName().SendKeys(speakerDetails["lastName"]);

            //Add button
            speakerAddButton().Click();

            verifySuccessBannerMessage("Speaker added successfully.");

            return speakerDetails;

        }

        //This function will upload the required video
        public void uploadBrowseVideo()
        {
            log.Info("inside uploadBrowseVideo " + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

            iWait.Until(ExpectedConditions.InvisibilityOfElementLocated((OR.GetElement("SeriesManagement", "SuccessOkButton", "TVAdminPortalOR.xml"))));

            //Click on uplaod video tab
            uploadVideoTab().FindElement(By.TagName("a")).Click();


            //find the div sibling in order to get upload button
            IList<IWebElement> siblings = (IList<IWebElement>)driver.FindElements(By.XPath("//input[@id='UploadBrowseButton']/following-sibling::div"));

            chooseFileButton = siblings[0].FindElement(By.TagName("input"));

            //reading xml file to upload Video
            String uploadvideo = cf.readingXMLFile("AdminPortal", "Video_Management", "VideoUpload", "Config.xml");
            string uploadvideoPath = AppDomain.CurrentDomain.BaseDirectory + "\\Upload\\Videos\\" + uploadvideo;
            uf.uploadfile(chooseFileButton, uploadvideoPath);

            //   Click on video upload button
            uploadVideoButton().Click();

            //wait till 10mins uploading of the video gets completed
            int j = 1;
            while (j <= 200)
            {
                iWait.Until(ExpectedConditions.ElementExists((OR.GetElement("VideoManagement", "UploadVidBTN", "TVAdminPortalOR.xml"))));

                IWebElement ele = driver.FindElement((OR.GetElement("VideoManagement", "UploadVidBTN", "TVAdminPortalOR.xml")));

                String value = ele.GetAttribute("value").Trim();

                if (value.Equals("Uploaded"))
                    break;

                Thread.Sleep(3000);
                j++;
            }

            verifySuccessBannerMessage("Video uploaded successfully.");


            int count = 0;
            String status = null; ;

            //checking the status of video for 5mins untill it gets Ready
            while (count < 60)
            {
                iWait.Until(ExpectedConditions.InvisibilityOfElementLocated((OR.GetElement("SeriesManagement", "SuccessOkButton", "TVAdminPortalOR.xml"))));

                status = driver.FindElement((OR.GetElement("VideoManagement", "StatusCSS", "TVAdminPortalOR.xml"))).Text;


                log.Info("status of the Video:::" + status);

                if (status.Equals("Status: READY"))
                    break;
                Thread.Sleep(5000);

                count = count + 1;

                //Click on the preview button
                executor.ExecuteScript("arguments[0].click();", videoPreviewButton());
            }

            Assert.AreEqual("Status: READY", status);

            Thread.Sleep(2000);
        }

        public void uploadFtpVideo()
        {
            log.Info("inside uploadFtpVideo " + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

            iWait.Until(ExpectedConditions.InvisibilityOfElementLocated((OR.GetElement("SeriesManagement", "SuccessOkButton", "TVAdminPortalOR.xml"))));

            //Click on uplaod video tab
            uploadVideoTab().FindElement(By.TagName("a")).Click();

            uploadFtpRadioButton().Click();


            //Ftp file name field  

            String ftpVideoName = cf.readingXMLFile("AdminPortal", "Video_Management", "FtpVideoUpload", "Config.xml");

            driver.FindElement((OR.GetElement("VideoManagement", "UploadVidFTPTXT", "TVAdminPortalOR.xml"))).SendKeys(ftpVideoName);

            driver.FindElement((OR.GetElement("VideoManagement", "UploadVidNew", "TVAdminPortalOR.xml"))).Click();

            //wait till 10mins uploading of the video gets completed
            int j = 1;

            while (j <= 200)
            {
                iWait.Until(ExpectedConditions.ElementIsVisible((OR.GetElement("VideoManagement", "UploadVidNew", "TVAdminPortalOR.xml"))));

                String value = driver.FindElement((OR.GetElement("VideoManagement", "UploadVidNew", "TVAdminPortalOR.xml"))).GetAttribute("value").Trim();
                if (value.Equals("Uploaded"))
                    break;

                Thread.Sleep(3000);
                j++;
            }

            verifySuccessBannerMessage("Video uploaded successfully.");

            int count = 0;
            String status = null; ;

            //checking the status of video for 5mins untill it gets Ready
            while (count < 60)
            {
                iWait.Until(ExpectedConditions.InvisibilityOfElementLocated((OR.GetElement("SeriesManagement", "SuccessOkButton", "TVAdminPortalOR.xml"))));

                status = driver.FindElement((OR.GetElement("VideoManagement", "StatusCSS", "TVAdminPortalOR.xml"))).Text;
                log.Info("status of the Video:::" + status);
                if (status.Equals("Status: READY"))
                    break;
                Thread.Sleep(5000);

                count = count + 1;

                //Click on the preview button
                executor.ExecuteScript("arguments[0].click();", videoPreviewButton());
            }

            Assert.AreEqual("Status: READY", status);
            Thread.Sleep(2000);

        }

        //This funtion will select  Record Date and Time and click on publish button
        public String recordPublishVideo()
        {
            String currentDate = null;

            log.Info("inside recordPublishVideo " + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

            iWait.Until(ExpectedConditions.InvisibilityOfElementLocated((OR.GetElement("SeriesManagement", "SuccessOkButton", "TVAdminPortalOR.xml"))));

            //Click on Publish tab
            publishTab().Click();

            //Getting sysytem current date and adding 2minutes in time as video upload time should be greater than system current time
            currentDate = DateTime.Now.AddMinutes(2).ToString("dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture);

            String countDownDate = DateTime.Now.AddMinutes(2).ToString("dd/MM/yyyy HHmm", CultureInfo.InvariantCulture);

            String[] dateTime = countDownDate.Split(' ');

            log.Info("Date to record video :: " + dateTime[0].Trim());

            log.Info("Time at record video will publish :: " + dateTime[1].Trim());

            //Selecting todays date from date picker
            recordPublishFromDate().SendKeys(dateTime[0].Trim());

            //enter the time in the Final Time field
            recordPublishFromTime().Clear();

            recordPublishFromTime().SendKeys(dateTime[1].Trim());

            return currentDate;
        }

        public String permissionTab()
        {
            iWait.Until(ExpectedConditions.ElementExists((OR.GetElement("VideoManagement", "PermissionTab", "TVAdminPortalOR.xml"))));

            driver.FindElement((OR.GetElement("VideoManagement", "PermissionTab", "TVAdminPortalOR.xml"))).Click();

            log.Info("inside permission tab");

            //Disable comment check box
            //driver.FindElement((OR.GetElement("VideoManagement", "CommentChkBox", "TVAdminPortalOR.xml"))).Click();

            //Disable Like/Dislike check box
            //driver.FindElement((OR.GetElement("VideoManagement", "LikeDislikeChkBox", "TVAdminPortalOR.xml"))).Click();

            //Display QnA check box
            //driver.FindElement((OR.GetElement("VideoManagement", "DisplayQA", "TVAdminPortalOR.xml"))).Click();

            //select cpd logo
            driver.FindElement((OR.GetElement("VideoManagement", "CPDLogo", "TVAdminPortalOR.xml"))).Click();


            //Thumbnail choose file button
            //IWebElement thumbnailButton = driver.FindElement((OR.GetElement("VideoManagement", "FileChooseBTN", "TVAdminPortalOR.xml")));

            String thumbnailImageName = cf.readingXMLFile("AdminPortal", "Channel_Management", "channelIcon", "Config.xml");

            //string thumbnailPath = AppDomain.CurrentDomain.BaseDirectory + "\\Upload\\Images\\" + thumbnailImageName;

            //uf.uploadfile(thumbnailButton, thumbnailPath);

            ////Click on upload button
            //driver.FindElement((OR.GetElement("VideoManagement", "FileUpldBTN", "TVAdminPortalOR.xml"))).Click();

            //verifySuccessBannerMessage("video thumbnail image uploaded successfully");

            return thumbnailImageName;
        }

        public void clickPublishbtn()
        {
            publishTab().FindElement(By.TagName("a")).Click();

            videoPublishRadioButton().Click();
            Thread.Sleep(2000);

            // click on the publish button
            executor.ExecuteScript("arguments[0].click();", videoPublishButton());

            //waiting for loader
            iWait = new WebDriverWait(driver, TimeSpan.FromSeconds(120));
            iWait.Until(ExpectedConditions.ElementExists((OR.GetElement("VideoManagement", "Loading", "TVAdminPortalOR.xml"))));
            log.Info("loading is over");


            //  iWait.Until(d => d.FindElement((OR.GetElement("VideoManagement", "DefaultSuccessLang", "TVAdminPortalOR.xml"))).Text.Equals("Final Video Published Successfully."));
            // SuccessBannerMessage = driver.FindElement((OR.GetElement("VideoManagement", "DefaultSuccessLang", "TVAdminPortalOR.xml")));

            iWait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy((OR.GetElement("VideoManagement", "CustomMsgCSS", "TVAdminPortalOR.xml"))));

            iWait.Until(ExpectedConditions.ElementIsVisible((OR.GetElement("VideoManagement", "SuccessBTN", "TVAdminPortalOR.xml"))));

            //Verifying Success banner message
            SuccessBannerMessage = driver.FindElement((OR.GetElement("VideoManagement", "DefaultSuccessLang", "TVAdminPortalOR.xml")));
            String Publish_Successful_Message = SuccessBannerMessage.Text;

            Assert.AreEqual("Record Published Successfully.", Publish_Successful_Message);

            //Click on ok button of banner message
            OkButton = driver.FindElement((OR.GetElement("VideoManagement", "SuccessBTN", "TVAdminPortalOR.xml")));
            executor.ExecuteScript("arguments[0].click();", OkButton);

            iWait.Until(ExpectedConditions.InvisibilityOfElementLocated((OR.GetElement("VideoManagement", "SuccessBTN", "TVAdminPortalOR.xml"))));
        }

        public void hideVideo()
        {
            publishTab().Click();

            Thread.Sleep(2000);

            if (!hideRecordRadioButton().Selected)
            {
                hideRecordRadioButton().Click();
            }

            Thread.Sleep(2000);

            videoPublishButton().Click();

            iWait.Until(ExpectedConditions.ElementIsVisible((OR.GetElement("VideoManagement", "SuccessBTN", "TVAdminPortalOR.xml"))));

            iWait.Until(d => d.FindElement((OR.GetElement("VideoManagement", "DefaultSuccessLang", "TVAdminPortalOR.xml"))).Text.Equals("Final Video Published Successfully."));

            Assert.AreEqual("Final Video Published Successfully.", driver.FindElement((OR.GetElement("VideoManagement", "DefaultSuccessLang", "TVAdminPortalOR.xml"))).Text);

            driver.FindElement((OR.GetElement("VideoManagement", "SuccessBTN", "TVAdminPortalOR.xml"))).Click();

            OverlayWait();
        }

        //This funtion will select Final  Date and Time and click on publish button
        public void finalPublishVideo(String videoType)
        {
            log.Info("inside finalPublishVideo " + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

            iWait.Until(ExpectedConditions.InvisibilityOfElementLocated((OR.GetElement("SeriesManagement", "SuccessOkButton", "TVAdminPortalOR.xml"))));

            //Click on Publish tab
            publishTab().FindElement(By.TagName("a")).Click();

            String currentDate = null;

            if (videoType.Equals("promo"))
            {
                //Getting sysytem current date and adding 2minutes in time as video upload time should be greater than system current time
                currentDate = DateTime.Now.AddDays(2).ToString("dd/MM/yyyy HHmm", CultureInfo.InvariantCulture);

            }
            else
            {

                //Getting sysytem current date and adding 2minutes in time as video upload time should be greater than system current time
                currentDate = DateTime.Now.AddMinutes(2).ToString("dd/MM/yyyy HHmm", CultureInfo.InvariantCulture);

            }

            String[] dateTime = currentDate.Split(' ');

            log.Info("Date to final publish video :: " + dateTime[0].Trim());

            log.Info("Time at video will final publish :: " + dateTime[1].Trim());

            //Selecting todays date from date picker
            Console.WriteLine("date3:   :" + DateTime.Now.ToString("dd/MM/yyyy HHmm", CultureInfo.InvariantCulture));
            finalPublishFromDate().SendKeys(dateTime[0].Trim());


            //enter the time in the Final Time field
            finalPublishFromTime().Clear();
            finalPublishFromTime().SendKeys(dateTime[1].Trim());

            //Clicking on video publish radio button to close date picker
            videoPublishRadioButton().Click();
            Thread.Sleep(2000);

            // click on the publish button
            executor.ExecuteScript("arguments[0].click();", videoPublishButton());

            //waiting for loader
            iWait = new WebDriverWait(driver, TimeSpan.FromSeconds(120));
            iWait.Until(ExpectedConditions.ElementExists((OR.GetElement("VideoManagement", "Loading", "TVAdminPortalOR.xml"))));
            log.Info("loading is over");


            //  iWait.Until(d => d.FindElement((OR.GetElement("VideoManagement", "DefaultSuccessLang", "TVAdminPortalOR.xml"))).Text.Equals("Final Video Published Successfully."));
            // SuccessBannerMessage = driver.FindElement((OR.GetElement("VideoManagement", "DefaultSuccessLang", "TVAdminPortalOR.xml")));

            iWait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy((OR.GetElement("VideoManagement", "CustomMsgCSS", "TVAdminPortalOR.xml"))));

            iWait.Until(ExpectedConditions.ElementIsVisible((OR.GetElement("VideoManagement", "SuccessBTN", "TVAdminPortalOR.xml"))));

            //Verifying Success banner message
            SuccessBannerMessage = driver.FindElement((OR.GetElement("VideoManagement", "DefaultSuccessLang", "TVAdminPortalOR.xml")));
            String Publish_Successful_Message = SuccessBannerMessage.Text;

            Assert.AreEqual("Final Video Published Successfully.", Publish_Successful_Message);

            //Click on ok button of banner message
            OkButton = driver.FindElement((OR.GetElement("VideoManagement", "SuccessBTN", "TVAdminPortalOR.xml")));
            executor.ExecuteScript("arguments[0].click();", OkButton);

            iWait.Until(ExpectedConditions.InvisibilityOfElementLocated((OR.GetElement("VideoManagement", "SuccessBTN", "TVAdminPortalOR.xml"))));

        }

        //This funtion will select Expiry Date and Time and click on publish button
        public void ExpiryPublishVideo()
        {
            log.Info("inside ExpiryPublishVideo " + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

            iWait.Until(ExpectedConditions.InvisibilityOfElementLocated((OR.GetElement("SeriesManagement", "SuccessOkButton", "TVAdminPortalOR.xml"))));

            //Click on Publish tab
            publishTab().Click();

            //Getting sysytem current date and adding 4 minutes in time as video upload time should be greater than system current time
            String currentDate = DateTime.Now.AddMinutes(4).ToString("dd/MM/yyyy HHmm");

            String[] dateTime = currentDate.Split(' ');

            log.Info("Expiry Date of video :: " + dateTime[0].Trim());

            log.Info("Expiry Time of video :: " + dateTime[1].Trim());


            //Selecting todays date from date picker
            finalPublishExpiryDate().SendKeys(dateTime[0].Trim());


            //enter the time in the Final Time field
            finalPublishExpiryTime().Clear();

            finalPublishExpiryTime().SendKeys(dateTime[1].Trim());
        }


        //This funtion verify the recent video created and hide the record for the same
        public void recentVideoVerification(String videoName)
        {
            IList<IWebElement> videoRowList;

            log.Info("inside recentVideoVerification " + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

            //Wait for Add New Button
            iWait.Until(ExpectedConditions.ElementToBeClickable((OR.GetElement("VideoManagement", "AddNewBTN", "TVAdminPortalOR.xml"))));

            //click on my video check box
            driver.FindElement((OR.GetElement("VideoManagement", "ChkMyVid", "TVAdminPortalOR.xml"))).Click();

            //OverlayWait();
            Thread.Sleep(3000);
            iWait.Until(ExpectedConditions.ElementExists((OR.GetElement("VideoManagement", "RecentVidListing", "TVAdminPortalOR.xml"))));

            try
            {
                Thread.Sleep(3000);
                IWebElement tblVideoListing = driver.FindElement((OR.GetElement("VideoManagement", "RecentVidListing", "TVAdminPortalOR.xml")));

                iWait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy(By.TagName("tr")));

                videoRowList = (IList<IWebElement>)tblVideoListing.FindElements(By.TagName("tr"));
            }
            catch (Exception e)
            {
                log.Info("Row detection failed at first instance for My video table");

                Thread.Sleep(2000);  // Row detection failed at first instance thus retrying after wait

                IWebElement tblVideoListing = driver.FindElement((OR.GetElement("VideoManagement", "RecentVidListing", "TVAdminPortalOR.xml")));

                iWait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy(By.TagName("tr")));

                videoRowList = (IList<IWebElement>)tblVideoListing.FindElements(By.TagName("tr"));
            }

            Boolean flag = false;

            uf.isJqueryActive(driver);

            int i = 0;

            foreach (IWebElement currentRow in videoRowList)
            {

                //Check Row that have class="GridRowStyle" or class="AltGridStyle"
                if (currentRow.GetAttribute("class").Equals("GridRowStyle") || currentRow.GetAttribute("class").Equals("AltGridStyle"))
                {

                    String columData = currentRow.FindElements(By.TagName("td"))[0].FindElement(By.TagName("a")).GetAttribute("title").Trim();

                    Console.WriteLine("Video Title from manage page::" + columData);

                    //OverlayWait();

                    if (columData.Equals(videoName))
                    {
                        flag = true;

                        //Assert to check presence of edit button
                        // Assert.AreEqual(true, currentRow.FindElements(By.TagName("td"))[7].FindElement(By.TagName("tr")).FindElement(OR.GetElement("VideoManagement", "EditBTN", "TVAdminPortalOR.xml", i)).GetAttribute("src").Contains("Edit.png"));

                        //Click on edit button
                        Thread.Sleep(3000);

                        IWebElement VideoEditButton = driver.FindElement(OR.GetElement("VideoManagement", "EditBTN", "TVAdminPortalOR.xml", i));
                        VideoEditButton.Click();
                        break;
                    }
                    i++;
                }
            }

            iWait.Until(ExpectedConditions.ElementExists((OR.GetElement("VideoManagement", "VidNumberTXT", "TVAdminPortalOR.xml"))));

            //Get video videoID field data
            videoID_Admin = driver.FindElement((OR.GetElement("VideoManagement", "VidNumberTXT", "TVAdminPortalOR.xml"))).GetAttribute("value");

            log.Info("VideoID_admin:: " + videoID_Admin);

            //Get GUID of the Video
            guid_Admin = driver.FindElement((OR.GetElement("VideoRequestByUser", "GuAdminId", "TVAdminPortalOR.xml"))).GetAttribute("value");

            log.Info("Guid_Admin:: " + guid_Admin);

            String StreamID_Admin = driver.FindElement((OR.GetElement("VideoManagement", "StreamGUId", "TVAdminPortalOR.xml"))).GetAttribute("value");

            log.Info("StreamID_Admin:: " + guid_Admin);

            publishTab().Click();

            //if (!hideRecordRadioButton().Selected)
            //{
            //    hideRecordRadioButton().Click();
            //}

            Thread.Sleep(2000);

            //videoPublishButton().Click();

            //iWait.Until(ExpectedConditions.ElementIsVisible((OR.GetElement("VideoManagement", "SuccessBTN", "TVAdminPortalOR.xml"))));

            //iWait.Until(d => d.FindElement((OR.GetElement("VideoManagement", "DefaultSuccessLang", "TVAdminPortalOR.xml"))).Text.Equals("Final Video Published Successfully."));

            //Assert.AreEqual("Final Video Published Successfully.", driver.FindElement((OR.GetElement("VideoManagement", "DefaultSuccessLang", "TVAdminPortalOR.xml"))).Text);

            //driver.FindElement((OR.GetElement("VideoManagement", "SuccessBTN", "TVAdminPortalOR.xml"))).Click();

            //OverlayWait();

        }

        #endregion

        /// <summary>
        /// this test creates free video,free promo image, free default image
        /// verifies free video,free promo image, free default image
        /// verifies different permissions
        /// verifies video search
        /// </summary>
        #region Free Video functionality
        [Test]
        public void TVAdmin_001_FreeVideoFunctionality()
        {
            try
            {
                log.Info("TVAdmin_001_CreateFreeVideo test Started" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

                redirectToVideoManagement();

                basicInfoTab();

                String adminSelectedChannel = channelListTab();

                pricingListTab("Free");

                promotionalInformation("DefaultImage", null);

                addcopyright();

                advanceTab().Click();

                String thumbnailImageName = permissionTab();

                String adminKeyword = keywordsTab();

                String adminAttachment = uploadAttachmentTab();

                Dictionary<String, String> adminSponsorDetails = editSponsor();

                Dictionary<String, String> adminSpeakerDetails = createSpeaker();

                uploadBrowseVideo();

                String currentDate = recordPublishVideo();

                driver.FindElement(OR.GetElement("VideoRequestByUser", "BasicInfoTab", "TVAdminPortalOR.xml")).FindElement(By.TagName("a")).Click();

                promotionalInformation("Countdown", currentDate);

                //record publish the video
                clickPublishbtn();

                //waiting for 2.5 minutes as video will be publish after 2minutes from current system time
                Thread.Sleep(150000);

                uf.OpenNewTab(driver);

                log.Info("count ::: " + driver.WindowHandles.Count);

                String browsertype = uf.getRunningBrowser(driver, driver.FindElement(By.TagName("html")));

                uf.SwitchToWebTab(driver, browsertype);

                uf.NavigateWebPortal(cf, driver);

                //calling from IETTVWebportal project to search created video on web portal and verifying the same
                videoResult.searchVideoVerification(videoName, guid_Admin);

                videoResult.verifyDefaultImage();

                uf.SwitchToAdminTab(driver, browsertype);

                recentVideoVerification(videoName);

                driver.FindElement(OR.GetElement("VideoRequestByUser", "BasicInfoTab", "TVAdminPortalOR.xml")).FindElement(By.TagName("a")).Click();

                //add promo image
                String promoDetails = promotionalInformation("Image", null);

                //recordPublishVideo();

                clickPublishbtn();

                Thread.Sleep(150000);

                uf.SwitchToWebTab(driver, browsertype);

                uf.NavigateWebPortal(cf, driver);

                //calling from IETTVWebportal project to search created video on web portal and verifying the same
                videoResult.searchVideoVerification(videoName, guid_Admin);

                videoResult.verifyPromoImage(promoDetails);

                uf.SwitchToAdminTab(driver, browsertype);

                recentVideoVerification(videoName);

                finalPublishVideo("normal");

                Thread.Sleep(150000);

                uf.SwitchToWebTab(driver, browsertype);

                uf.NavigateWebPortal(cf, driver);

                videoResult.searchVideoVerification(videoName, guid_Admin);
                //videoResult.searchVideoVerification("vid707ce7", "9eb7e7af-c9ff-4300-b195-fac726c87eac");

                videoResult.verifyVideoName(videoName);

                videoResult.verifyFreeVideo();

                videoResult.verifyCpdLogo();

                //videoResult.verifyPromoImage(promoDetails);

                //videoResult.verifyThumbnail(thumbnailImageName);

                videoResult.verifyAbstractContent(abstractContent);

                videoResult.verifyChannelName(adminSelectedChannel);

                videoResult.verifyKeywordName(adminKeyword);

                videoResult.verifyAttachment(adminAttachment);

                videoResult.verifySponsorDetails(adminSponsorDetails);

                videoResult.verifySpeakerDetails(adminSpeakerDetails);


                ////newwwwww test 

                //videoResult.verifyAbstractContent(abstractContent);

                //videoResult.verifyChannelName("Chan 1f0f12");

                //videoResult.verifyKeywordName("AutomationRave");


                videoResult.redirectToLogin();

                //uf.scrollUp(driver);

                videoResult.searchVideoVerification(videoName, guid_Admin);
                //videoResult.searchVideoVerification("vide420d1", "adeff681-8481-4ad8-a19a-f96a83f3810e");

                Thread.Sleep(4000);

                videoResult.likeDislikeTest(videoName);
                // videoResult.likeDislikeTest("vide420d1");

                //check hide video functionality
                uf.SwitchToAdminTab(driver, browsertype);

                recentVideoVerification(videoName);

                hideVideo();

                Thread.Sleep(10000);

                uf.SwitchToWebTab(driver, browsertype);

                uf.NavigateWebPortal(cf, driver);

                //Verify the video on web portal
                videoResult.verifyingNoresultFound(videoName);

                log.Info("TVAdmin_001_CreateFreeVideo test completed" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message + "\n" + e.StackTrace + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());
                log.Error(e.Message + "\n" + e.StackTrace + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());
                Assert.AreEqual(true, false);
            }
        }
        #endregion

        /// <summary>
        /// this test creates premium video 
        /// with  promo Image,default Image
        /// and verifies premium video 
        /// with  promo Image,default Image
        /// </summary>
        #region Premium Video
        [Test]
        public void TVAdmin_002_PremiumVideofunctionality()
        {
            log.Info("TVAdmin_003_CreatePremiumVideo test Started" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

            redirectToVideoManagement();

            basicInfoTab();

            String adminSelectedChannel = channelListTab();

            pricingListTab("Premium");

            promotionalInformation("DefaultImage", null);

            authorisationCopyright();

            String adminKeyword = keywordsTab();

            String adminAttachment = uploadAttachmentTab();

            Dictionary<String, String> adminSponsorDetails = editSponsor();

            Dictionary<String, String> adminSpeakerDetails = createSpeaker();

            uploadFtpVideo();

            String currentDate = recordPublishVideo();

            driver.FindElement(OR.GetElement("VideoRequestByUser", "BasicInfoTab", "TVAdminPortalOR.xml")).FindElement(By.TagName("a")).Click();

            promotionalInformation("Countdown", currentDate);

            clickPublishbtn();

            //waiting for 2.5 minutes as video will be publish after 2minutes from current system time
            Thread.Sleep(150000);

            uf.OpenNewTab(driver);

            log.Info("Window count ::: " + driver.WindowHandles.Count);

            String browsertype = uf.getRunningBrowser(driver, driver.FindElement(By.TagName("html")));

            uf.SwitchToWebTab(driver, browsertype);

            uf.NavigateWebPortal(cf, driver);

            //calling from IETTVWebportal project to search created video on web portal and verifying the same
            videoResult.searchVideoVerification(videoName, guid_Admin);

            //videoResult.verifyCountDownDetails();

            videoResult.verifyDefaultImage();

            uf.SwitchToAdminTab(driver, browsertype);

            recentVideoVerification(videoName);

            driver.FindElement(OR.GetElement("VideoRequestByUser", "BasicInfoTab", "TVAdminPortalOR.xml")).FindElement(By.TagName("a")).Click();

            String promoDetails = promotionalInformation("Image", null);

            clickPublishbtn();

            Thread.Sleep(150000);

            log.Info("Window count ::: " + driver.WindowHandles.Count);

            uf.getRunningBrowser(driver, driver.FindElement(By.TagName("html")));

            uf.SwitchToWebTab(driver, browsertype);

            uf.NavigateWebPortal(cf, driver);

            //calling from IETTVWebportal project to search created video on web portal and verifying the same
            videoResult.searchVideoVerification(videoName, guid_Admin);

            videoResult.verifyPromoImage(promoDetails);

            uf.SwitchToAdminTab(driver, browsertype);

            recentVideoVerification(videoName);

            finalPublishVideo("normal");

            Thread.Sleep(150000);

            log.Info("Window count ::: " + driver.WindowHandles.Count);

            uf.getRunningBrowser(driver, driver.FindElement(By.TagName("html")));

            uf.SwitchToWebTab(driver, browsertype);

            uf.NavigateWebPortal(cf, driver);

            //calling from IETTVWebportal project to search created video on web portal and verifying the same
            videoResult.searchVideoVerification(videoName, guid_Admin);

            videoResult.verifyVideoName(videoName);

            videoResult.verifyPremiumVideo();

            videoResult.verifySpeakerDetails(adminSpeakerDetails);

            videoResult.verifyChannelName(adminSelectedChannel);

            videoResult.verifyAbstractContent(abstractContent);

            videoResult.verifyKeywordName(adminKeyword);

            videoResult.verifyAttachment(adminAttachment);

            videoResult.verifySponsorDetails(adminSponsorDetails);



            log.Info("TVAdmin_003_CreatePremiumVideo test completed" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());


        }

        #endregion

        /// <summary>
        /// this test creates subscription video 
        /// with  promo Image,default Image
        /// and verifies subscription video 
        /// with  promo Image,default Image
        /// </summary>
        #region Subscription Video Functionality
        [Test]
        public void TVAdmin_003_SubscriptionVideoFunctionality()
        {
            try
            {
                log.Info("TVAdmin_004_CreateSubscriptionVideo test Started" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

                redirectToVideoManagement();

                basicInfoTab();

                String adminSelectedChannel = channelListTab();

                pricingListTab("Subscription");

                promotionalInformation("DefaultImage", null);

                addcopyright();

                String adminKeyword = keywordsTab();

                String adminAttachment = uploadAttachmentTab();

                Dictionary<String, String> adminSponsorDetails = editSponsor();

                Dictionary<String, String> adminSpeakerDetails = createSpeaker();

                uploadBrowseVideo();

                String currentDate = recordPublishVideo();

                driver.FindElement(OR.GetElement("VideoRequestByUser", "BasicInfoTab", "TVAdminPortalOR.xml")).FindElement(By.TagName("a")).Click();

                promotionalInformation("Countdown", currentDate);

                clickPublishbtn();

                //waiting for 2.5 minutes as video will be publish after 2minutes from current system time
                Thread.Sleep(150000);

                uf.OpenNewTab(driver);

                log.Info("count ::: " + driver.WindowHandles.Count);

                String browsertype = uf.getRunningBrowser(driver, driver.FindElement(By.TagName("html")));

                uf.SwitchToWebTab(driver, browsertype);

                uf.NavigateWebPortal(cf, driver);

                //calling from IETTVWebportal project to search created video on web portal and verifying the same
                videoResult.searchVideoVerification(videoName, guid_Admin);

                videoResult.verifyDefaultImage();

                uf.SwitchToAdminTab(driver, browsertype);

                recentVideoVerification(videoName);

                driver.FindElement(OR.GetElement("VideoRequestByUser", "BasicInfoTab", "TVAdminPortalOR.xml")).FindElement(By.TagName("a")).Click();

                //add promo image
                String promoDetails = promotionalInformation("Image", null);

                clickPublishbtn();

                Thread.Sleep(150000);

                log.Info("count ::: " + driver.WindowHandles.Count);

                uf.getRunningBrowser(driver, driver.FindElement(By.TagName("html")));

                uf.SwitchToWebTab(driver, browsertype);

                uf.NavigateWebPortal(cf, driver);

                //calling from IETTVWebportal project to search created video on web portal and verifying the same
                videoResult.searchVideoVerification(videoName, guid_Admin);

                videoResult.verifyPromoImage(promoDetails);

                uf.SwitchToAdminTab(driver, browsertype);

                recentVideoVerification(videoName);

                finalPublishVideo("normal");

                Thread.Sleep(150000);

                log.Info("count ::: " + driver.WindowHandles.Count);

                uf.getRunningBrowser(driver, driver.FindElement(By.TagName("html")));

                uf.SwitchToWebTab(driver, browsertype);

                uf.NavigateWebPortal(cf, driver);

                //calling from IETTVWebportal project to search created video on web portal and verifying the same
                videoResult.searchVideoVerification(videoName, guid_Admin);

                videoResult.verifyVideoName(videoName);

                videoResult.verifySubscriptionVideo();

                videoResult.verifySpeakerDetails(adminSpeakerDetails);

                Thread.Sleep(4000);

                videoResult.verifyAbstractContent(abstractContent);

                videoResult.verifyChannelName(adminSelectedChannel);

                videoResult.verifyKeywordName(adminKeyword);

                videoResult.verifyAttachment(adminAttachment);

                videoResult.verifySponsorDetails(adminSponsorDetails);

                log.Info("TVAdmin_004_CreateSubscriptionVideo test completed" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());


            }

            catch (Exception e)
            {
                log.Error(e.Message + "\n" + e.StackTrace + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

                Assert.AreEqual(true, false);

            }
        }
        #endregion

        #region existing code

        //#region Normal video

        ///// <summary>
        ///// This test verifies Add Copyright,
        ///// Free video,
        ///// verifyAbstractContent,
        ///// verifyChannelName,
        ///// verifyKeywordName,
        ///// verifyAttachment,
        ///// verifySponsorDetails,
        ///// verifySpeakerDetails,
        ///// like/dislike
        ///// </summary>
        //[Test]
        //public void TVAdmin_001_CreateFreeVideo()
        //{
        //    try
        //    {
        //        log.Info("TVAdmin_001_CreateFreeVideo test Started" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

        //        redirectToVideoManagement();

        //        basicInfoTab();

        //        String adminSelectedChannel = channelListTab();

        //        pricingListTab("Free");

        //        addcopyright();

        //        String adminKeyword = keywordsTab();

        //        String adminAttachment = uploadAttachmentTab();

        //        Dictionary<String, String> adminSponsorDetails = editSponsor();

        //        Dictionary<String, String> adminSpeakerDetails = createSpeaker();

        //        uploadBrowseVideo();

        //        finalPublishVideo("normal");

        //        //waiting for 2.5 minutes as video will be publish after 2minutes from current system time
        //        Thread.Sleep(150000);

        //        uf.OpenNewTab(driver);

        //        log.Info("count ::: " + driver.WindowHandles.Count);

        //        String browsertype = uf.getRunningBrowser(driver, driver.FindElement(By.TagName("html")));

        //        uf.SwitchToWebTab(driver, browsertype);

        //        uf.NavigateWebPortal(cf, driver);

        //        //calling from IETTVWebportal project to search created video on web portal and verifying the same
        //        videoResult.searchVideoVerification(videoName, guid_Admin);

        //        videoResult.verifyVideoName(videoName);

        //        videoResult.verifyFreeVideo();

        //        videoResult.verifyAbstractContent(abstractContent);

        //        videoResult.verifyChannelName(adminSelectedChannel);

        //        videoResult.verifyKeywordName(adminKeyword);

        //        videoResult.verifyAttachment(adminAttachment);

        //        videoResult.verifySponsorDetails(adminSponsorDetails);

        //        videoResult.verifySpeakerDetails(adminSpeakerDetails);

        //        videoResult.redirectToLogin();

        //        videoResult.likeDislikeTest(videoName);

        //        log.Info("TVAdmin_001_CreateFreeVideo test completed" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());
        //    }



        //    catch (Exception e)
        //    {
        //        Console.WriteLine(e.Message + "\n" + e.StackTrace + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());
        //        log.Error(e.Message + "\n" + e.StackTrace + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());
        //        Assert.AreEqual(true, false);
        //    }
        //}


        ///// <summary>
        /////This Test perform hide operation from recently created video 
        ///// </summary>
        ///// 
        //[Test]
        //public void TVAdmin_002_HideFreeVideo()
        //{
        //    try
        //    {
        //        log.Info("TVAdmin_002_HideFreeVideo test Started" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

        //        redirectToVideoManagement();

        //        String initVidName = "AutoHide";

        //        String recentVideoName = basicInfoTab(initVidName);

        //        String adminSelectedChannel = channelListTab();

        //        pricingListTab("Free");

        //        addcopyright();

        //        uploadBrowseVideo();

        //        finalPublishVideo("normal");

        //        //waiting for 2.5 minutes as video will be publish after 2minutes from current system time
        //        Thread.Sleep(150000);

        //        uf.OpenNewTab(driver);

        //        log.Info("count ::: " + driver.WindowHandles.Count);

        //        String browsertype = uf.getRunningBrowser(driver, driver.FindElement(By.TagName("html")));

        //        uf.SwitchToWebTab(driver, browsertype);

        //        uf.NavigateWebPortal(cf, driver);

        //        //calling from IETTVWebportal project to search created video on web portal and verifying the same
        //        videoResult.searchVideoVerification(videoName, guid_Admin);

        //        uf.SwitchToAdminTab(driver, browsertype);

        //        Thread.Sleep(2000);
        //        uf.scrollUp(driver);

        //        Console.WriteLine("recent name:  " + recentVideoName);

        //        recentVideoVerification(recentVideoName);

        //        browsertype = uf.getRunningBrowser(driver, driver.FindElement(By.TagName("html")));

        //        uf.SwitchToWebTab(driver, browsertype);

        //        uf.NavigateWebPortal(cf, driver);

        //        //Verify the video on web portal
        //        videoResult.verifyingNoresultFound(recentVideoName);

        //        log.Info("TVAdmin_002_HideFreeVideo test Completed" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

        //    }


        //    catch (Exception e)
        //    {
        //        Console.WriteLine(e.Message + "\n" + e.StackTrace + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

        //        log.Error(e.Message + "\n" + e.StackTrace + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

        //        Assert.AreEqual(true, false);

        //    }


        //}

        ///// <summary>
        ///// This test verifies Copyright authorisation,
        ///// Premium video,Ftp video upload
        ///// verifyAbstractContent,
        ///// verifyChannelName,
        ///// verifyKeywordName,
        ///// verifyAttachment,
        ///// verifySponsorDetails,
        ///// verifySpeakerDetails
        ///// </summary>
        //[Test]
        //public void TVAdmin_003_CreatePremiumVideo()
        //{
        //    try
        //    {

        //        log.Info("TVAdmin_003_CreatePremiumVideo test Started" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

        //        redirectToVideoManagement();

        //        basicInfoTab();

        //        String adminSelectedChannel = channelListTab();

        //        pricingListTab("Premium");

        //        authorisationCopyright();

        //        String adminKeyword = keywordsTab();

        //        String adminAttachment = uploadAttachmentTab();

        //        Dictionary<String, String> adminSponsorDetails = editSponsor();

        //        Dictionary<String, String> adminSpeakerDetails = createSpeaker();

        //        uploadFtpVideo();

        //        finalPublishVideo("normal");

        //        //waiting for 2.5 minutes as video will be publish after 2minutes from current system time
        //        Thread.Sleep(150000);

        //        uf.OpenNewTab(driver);

        //        log.Info("Window count ::: " + driver.WindowHandles.Count);

        //        String browsertype = uf.getRunningBrowser(driver, driver.FindElement(By.TagName("html")));

        //        uf.SwitchToWebTab(driver, browsertype);

        //        uf.NavigateWebPortal(cf, driver);

        //        //calling from IETTVWebportal project to search created video on web portal and verifying the same
        //        videoResult.searchVideoVerification(videoName, guid_Admin);

        //        videoResult.verifyVideoName(videoName);

        //        videoResult.verifyPremiumVideo();

        //        videoResult.verifyAbstractContent(abstractContent);

        //        videoResult.verifyChannelName(adminSelectedChannel);

        //        videoResult.verifyKeywordName(adminKeyword);

        //        videoResult.verifyAttachment(adminAttachment);

        //        videoResult.verifySponsorDetails(adminSponsorDetails);

        //        videoResult.verifySpeakerDetails(adminSpeakerDetails);


        //        log.Info("TVAdmin_003_CreatePremiumVideo test completed" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());


        //    }

        //    catch (Exception e)
        //    {
        //        log.Error(e.Message + "\n" + e.StackTrace + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());
        //        Assert.AreEqual(true, false);

        //    }
        //}

        ///// <summary>
        ///// This test verifies Add copyright,
        ///// Subscription video(covered channel type as member)
        ///// verifyAbstractContent,
        ///// verifyChannelName,
        ///// verifyKeywordName,
        ///// verifyAttachment,
        ///// verifySponsorDetails,
        ///// verifySpeakerDetails
        ///// </summary>
        //[Test]
        //public void TVAdmin_004_CreateSubscriptionVideo()
        //{
        //    try
        //    {

        //        log.Info("TVAdmin_004_CreateSubscriptionVideo test Started" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

        //        redirectToVideoManagement();

        //        basicInfoTab();

        //        String adminSelectedChannel = channelListTab();

        //        pricingListTab("Subscription");

        //        addcopyright();

        //        String adminKeyword = keywordsTab();

        //        String adminAttachment = uploadAttachmentTab();

        //        Dictionary<String, String> adminSponsorDetails = editSponsor();

        //        Dictionary<String, String> adminSpeakerDetails = createSpeaker();

        //        uploadBrowseVideo();

        //        finalPublishVideo("normal");

        //        //waiting for 2.5 minutes as video will be publish after 2minutes from current system time
        //        Thread.Sleep(150000);

        //        uf.OpenNewTab(driver);

        //        log.Info("count ::: " + driver.WindowHandles.Count);

        //        String browsertype = uf.getRunningBrowser(driver, driver.FindElement(By.TagName("html")));

        //        uf.SwitchToWebTab(driver, browsertype);

        //        uf.NavigateWebPortal(cf, driver);

        //        //calling from IETTVWebportal project to search created video on web portal and verifying the same
        //        videoResult.searchVideoVerification(videoName, guid_Admin);

        //        videoResult.verifyVideoName(videoName);

        //        videoResult.verifySubscriptionVideo();

        //        videoResult.verifySpeakerDetails(adminSpeakerDetails);

        //        Thread.Sleep(4000);

        //        videoResult.verifyAbstractContent(abstractContent);

        //        videoResult.verifyChannelName(adminSelectedChannel);

        //        videoResult.verifyKeywordName(adminKeyword);

        //        videoResult.verifyAttachment(adminAttachment);

        //        videoResult.verifySponsorDetails(adminSponsorDetails);



        //        log.Info("TVAdmin_004_CreateSubscriptionVideo test completed" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());


        //    }

        //    catch (Exception e)
        //    {
        //        log.Error(e.Message + "\n" + e.StackTrace + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

        //        Assert.AreEqual(true, false);

        //    }
        //}

        //#endregion


        //#region Free promo

        ////need to see if it can be implemented

        ////[Test]//cannot test
        ////public void TVAdmin_006_CreateFreePromoVideo()
        ////{
        ////    try
        ////    {

        ////        log.Info("TVAdmin_006_CreateFreePromoVideo test Started" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

        ////        redirectToVideoManagement();

        ////        basicInfoTab();

        ////        channelListTab();

        ////        pricingListTab("Free");

        ////        promotionalInformation("Video");

        ////        copyrightListTab();

        ////        uploadVideoTab();

        ////        finalPublishVideo("normal");

        ////        log.Info("TVAdmin_006_CreateFreePromoVideo test completed" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());


        ////    }

        ////    catch (Exception e)
        ////    {
        ////        log.Info(e.Message + "\n" + e.StackTrace);
        ////        Assert.AreEqual(true, false);

        ////    }
        ////}

        ///// <summary>
        ///// This test verifies Add copyright,
        ///// free video,
        ///// Record publish
        ///// promo image
        ///// </summary>
        //[Test]
        //public void TVAdmin_005_CreateFreePromoImage()
        //{
        //    try
        //    {

        //        log.Info("TVAdmin_005_CreateFreePromoImage test Started" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

        //        redirectToVideoManagement();

        //        basicInfoTab();

        //        channelListTab();

        //        pricingListTab("Free");

        //        String promoDetails = promotionalInformation("Image", null);

        //        addcopyright();

        //        uploadBrowseVideo();

        //        recordPublishVideo();

        //        finalPublishVideo("promo");

        //        //waiting for 2.5 minutes as video will be publish after 2minutes from current system time
        //        Thread.Sleep(150000);

        //        uf.OpenNewTab(driver);

        //        log.Info("count ::: " + driver.WindowHandles.Count);

        //        String browsertype = uf.getRunningBrowser(driver, driver.FindElement(By.TagName("html")));

        //        uf.SwitchToWebTab(driver, browsertype);

        //        uf.NavigateWebPortal(cf, driver);

        //        //calling from IETTVWebportal project to search created video on web portal and verifying the same
        //        videoResult.searchVideoVerification(videoName, guid_Admin);

        //        videoResult.verifyVideoName(videoName);

        //        videoResult.verifyFreeVideo();

        //        videoResult.verifyPromoImage(promoDetails);

        //        log.Info("TVAdmin_005_CreateFreePromoImage test completed" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

        //    }

        //    catch (Exception e)
        //    {
        //        log.Error(e.Message + "\n" + e.StackTrace + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());
        //        Assert.AreEqual(true, false);

        //    }
        //}


        ///// <summary>
        ///// This test verifies Add copyright,
        ///// free video,
        ///// Record publish
        ///// DefaultImage,
        ///// CountDown
        ///// </summary>
        //[Test]
        //public void TVAdmin_006_CreateFreeDefaultImage()
        //{
        //    try
        //    {

        //        log.Info("TVAdmin_006_CreateFreeDefaultImage test Started" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

        //        redirectToVideoManagement();

        //        basicInfoTab();

        //        channelListTab();

        //        pricingListTab("Free");

        //        promotionalInformation("DefaultImage", null);

        //        addcopyright();

        //        uploadBrowseVideo();

        //        String currentDate = recordPublishVideo();

        //        driver.FindElement(OR.GetElement("VideoRequestByUser", "BasicInfoTab", "TVAdminPortalOR.xml")).FindElement(By.TagName("a")).Click();

        //        promotionalInformation("Countdown", currentDate);

        //        finalPublishVideo("promo");

        //        //waiting for 4 minutes as video will be publish after 2minutes from current system time
        //        Thread.Sleep(240000);

        //        uf.OpenNewTab(driver);

        //        log.Info("count ::: " + driver.WindowHandles.Count);

        //        String browsertype = uf.getRunningBrowser(driver, driver.FindElement(By.TagName("html")));

        //        uf.SwitchToWebTab(driver, browsertype);

        //        uf.NavigateWebPortal(cf, driver);

        //        //calling from IETTVWebportal project to search created video on web portal and verifying the same
        //        videoResult.searchVideoVerification(videoName, guid_Admin);

        //        videoResult.verifyCountDownDetails();

        //        uf.scrollUp(driver);

        //        videoResult.verifyVideoName(videoName);

        //        //uf.scrollUp(driver);

        //        videoResult.verifyFreeVideo();

        //        //uf.scrollUp(driver);
        //        //Thread.Sleep(2000);

        //        videoResult.verifyDefaultImage();


        //        log.Info("TVAdmin_006_CreateFreeDefaultImage test completed" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

        //    }

        //    catch (Exception e)
        //    {
        //        log.Error(e.Message + "\n" + e.StackTrace + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());
        //        Assert.AreEqual(true, false);

        //    }
        //}

        //#endregion


        //#region Premium promo

        ////need to see if it can be implemented

        ////[Test]//cannot test
        ////public void TVAdmin_009_CreatePremiumPromoVideo()
        ////{
        ////    try
        ////    {

        ////        log.Info("TVAdmin_009_CreatePremiumPromoVideo test Started" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

        ////        redirectToVideoManagement();

        ////        basicInfoTab();

        ////        channelListTab();

        ////        pricingListTab("Premium");

        ////        promotionalInformation("Video");

        ////        copyrightListTab();

        ////        uploadBrowseVideo();

        ////        recordPublishVideo();

        ////        finalPublishVideo("promo");

        ////        log.Info("TVAdmin_009_CreatePremiumPromoVideo test completed" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

        ////    }

        ////    catch (Exception e)
        ////    {
        ////        log.Info(e.Message + "\n" + e.StackTrace);
        ////        Assert.AreEqual(true, false);

        ////    }
        ////}

        ///// <summary>
        ///// This test verifies Add copyright,
        ///// Premium video,
        ///// Record publish
        ///// promo image
        ///// </summary>
        //[Test]
        //public void TVAdmin_007_CreatePremiumPromoImage()
        //{
        //    try
        //    {

        //        log.Info("TVAdmin_007_CreatePremiumPromoImage test Started" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

        //        redirectToVideoManagement();

        //        basicInfoTab();

        //        channelListTab();

        //        pricingListTab("Premium");

        //        String promoDetails = promotionalInformation("Image", null);

        //        addcopyright();

        //        uploadBrowseVideo();

        //        recordPublishVideo();

        //        finalPublishVideo("promo");

        //        //waiting for 2.5 minutes as video will be publish after 2minutes from current system time
        //        Thread.Sleep(150000);

        //        uf.OpenNewTab(driver);

        //        log.Info("count ::: " + driver.WindowHandles.Count);

        //        String browsertype = uf.getRunningBrowser(driver, driver.FindElement(By.TagName("html")));

        //        uf.SwitchToWebTab(driver, browsertype);

        //        uf.NavigateWebPortal(cf, driver);

        //        //calling from IETTVWebportal project to search created video on web portal and verifying the same
        //        videoResult.searchVideoVerification(videoName, guid_Admin);

        //        videoResult.verifyVideoName(videoName);

        //        videoResult.verifyPremiumVideo();

        //        videoResult.verifyPromoImage(promoDetails);

        //        log.Info("TVAdmin_007_CreatePremiumPromoImage test completed" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

        //    }

        //    catch (Exception e)
        //    {
        //        log.Error(e.Message + "\n" + e.StackTrace + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

        //        Assert.AreEqual(true, false);

        //    }
        //}

        ///// <summary>
        ///// This test verifies Add copyright,
        ///// Premium video,
        ///// Record publish
        ///// DefaultImage,
        ///// CountDown
        ///// </summary>
        //[Test]
        //public void TVAdmin_008_CreatePremiumDefaultImage()
        //{
        //    try
        //    {

        //        log.Info("TVAdmin_008_CreatePremiumDefaultImage test Started" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

        //        redirectToVideoManagement();

        //        basicInfoTab();

        //        channelListTab();

        //        pricingListTab("Premium");

        //        promotionalInformation("DefaultImage", null);

        //        addcopyright();

        //        uploadBrowseVideo();

        //        String currentDate = recordPublishVideo();

        //        driver.FindElement(OR.GetElement("VideoRequestByUser", "BasicInfoTab", "TVAdminPortalOR.xml")).FindElement(By.TagName("a")).Click();

        //        promotionalInformation("Countdown", currentDate);

        //        finalPublishVideo("promo");

        //        //waiting for 4 minutes as video will be publish after 2minutes from current system time
        //        Thread.Sleep(240000);

        //        uf.OpenNewTab(driver);

        //        log.Info("count ::: " + driver.WindowHandles.Count);

        //        String browsertype = uf.getRunningBrowser(driver, driver.FindElement(By.TagName("html")));

        //        uf.SwitchToWebTab(driver, browsertype);

        //        uf.NavigateWebPortal(cf, driver);

        //        //calling from IETTVWebportal project to search created video on web portal and verifying the same
        //        videoResult.searchVideoVerification(videoName, guid_Admin);

        //        videoResult.verifyVideoName(videoName);

        //        videoResult.verifyPremiumVideo();

        //        videoResult.verifyDefaultImage();

        //        videoResult.verifyCountDownDetails();

        //        log.Info("TVAdmin_008_CreatePremiumDefaultImage test completed" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

        //    }

        //    catch (Exception e)
        //    {
        //        log.Error(e.Message + "\n" + e.StackTrace + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());
        //        Assert.AreEqual(true, false);

        //    }
        //}

        //#endregion


        //#region Subscription Promo


        ////[Test]
        ////public void TVAdmin_012_CreateSubscriptionPromoVideo()
        ////{
        ////    try
        ////    {

        ////        log.Info("TVAdmin_012_CreateSubscriptionPromoVideo test Started" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

        ////        redirectToVideoManagement();

        ////        basicInfoTab();

        ////        channelListTab();

        ////        pricingListTab("Premium");

        ////        promotionalInformation("Video");

        ////        addcopyright();

        ////        uploadBrowseVideo();

        ////        recordPublishVideo();

        ////        finalPublishVideo("promo");

        ////        log.Info("TVAdmin_012_CreateSubscriptionPromoVideo test completed" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

        ////    }

        ////    catch (Exception e)
        ////    {
        ////        log.Info(e.Message + "\n" + e.StackTrace);
        ////        Assert.AreEqual(true, false);

        ////    }
        ////}


        ///// <summary>
        /////  This test verifies Add copyright,
        ///// Subscription video,
        ///// Record publish
        ///// promo image
        ///// </summary>
        //[Test]
        //public void TVAdmin_009_CreateSubscriptionPromoImage()
        //{
        //    try
        //    {

        //        log.Info("TVAdmin_009_CreateSubscriptionPromoImage test Started" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

        //        redirectToVideoManagement();

        //        basicInfoTab();

        //        channelListTab();

        //        pricingListTab("Subscription");

        //        String promoDetails = promotionalInformation("Image", null);

        //        addcopyright();

        //        uploadBrowseVideo();

        //        recordPublishVideo();

        //        finalPublishVideo("promo");

        //        //waiting for 2.5 minutes as video will be publish after 2minutes from current system time
        //        Thread.Sleep(150000);

        //        uf.OpenNewTab(driver);

        //        log.Info("count ::: " + driver.WindowHandles.Count);

        //        String browsertype = uf.getRunningBrowser(driver, driver.FindElement(By.TagName("html")));

        //        uf.SwitchToWebTab(driver, browsertype);

        //        uf.NavigateWebPortal(cf, driver);

        //        //calling from IETTVWebportal project to search created video on web portal and verifying the same
        //        videoResult.searchVideoVerification(videoName, guid_Admin);

        //        videoResult.verifyVideoName(videoName);

        //        //## consider channel type - member
        //        videoResult.verifySubscriptionVideo();

        //        videoResult.verifyPromoImage(promoDetails);

        //        log.Info("TVAdmin_009_CreateSubscriptionPromoImage test completed" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

        //    }

        //    catch (Exception e)
        //    {
        //        log.Error(e.Message + "\n" + e.StackTrace + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

        //        Assert.AreEqual(true, false);

        //    }
        //}



        ///// <summary>
        ///// This test verifies Add copyright,
        ///// Subscription video,
        ///// Record publish
        ///// DefaultImage,
        ///// CountDown
        ///// </summary>
        //[Test]
        //public void TVAdmin_010_CreateSubscriptionDefaultImage()
        //{

        //    try
        //    {

        //        log.Info("TVAdmin_010_CreateSubscriptionDefaultImage test Started" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

        //        redirectToVideoManagement();

        //        basicInfoTab();

        //        channelListTab();

        //        pricingListTab("Subscription");

        //        promotionalInformation("DefaultImage", null);

        //        addcopyright();

        //        uploadBrowseVideo();

        //        String currentDate = recordPublishVideo();

        //        driver.FindElement(OR.GetElement("VideoRequestByUser", "BasicInfoTab", "TVAdminPortalOR.xml")).FindElement(By.TagName("a")).Click();

        //        promotionalInformation("Countdown", currentDate);

        //        finalPublishVideo("promo");

        //        //waiting for 4 minutes as video will be publish after 2minutes from current system time
        //        Thread.Sleep(240000);

        //        uf.OpenNewTab(driver);

        //        log.Info("count ::: " + driver.WindowHandles.Count);

        //        String browsertype = uf.getRunningBrowser(driver, driver.FindElement(By.TagName("html")));

        //        uf.SwitchToWebTab(driver, browsertype);

        //        uf.NavigateWebPortal(cf, driver);

        //        //calling from IETTVWebportal project to search created video on web portal and verifying the same
        //        videoResult.searchVideoVerification(videoName, guid_Admin);

        //        videoResult.verifyVideoName(videoName);

        //        videoResult.verifySubscriptionVideo();

        //        videoResult.verifyDefaultImage();

        //        videoResult.verifyCountDownDetails();

        //        log.Info("TVAdmin_010_CreateSubscriptionDefaultImage test completed" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

        //    }

        //    catch (Exception e)
        //    {
        //        log.Error(e.Message + "\n" + e.StackTrace + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

        //        Assert.AreEqual(true, false);

        //    }
        //}

        //#endregion


        ///// <summary>
        ///// This test verifies Comment,
        ///// Like,
        ///// Qna,
        ///// Cpd Logo
        ///// Thumbnail.
        ///// </summary>
        //[Test]
        //public void TVAdmin_011_CreateFreeVidWithDiffPermissions()
        //{

        //    try
        //    {

        //        log.Info("TVAdmin_011_CreateFreeVidWithDiffPermissions test Started" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

        //        redirectToVideoManagement();

        //        basicInfoTab();

        //        channelListTab();

        //        pricingListTab("Free");

        //        addcopyright();

        //        advanceTab().Click();

        //        //Permission tab

        //        iWait.Until(ExpectedConditions.ElementExists((OR.GetElement("VideoManagement", "PermissionTab", "TVAdminPortalOR.xml"))));

        //        driver.FindElement((OR.GetElement("VideoManagement", "PermissionTab", "TVAdminPortalOR.xml"))).Click();

        //        log.Info("inside permission tab");

        //        //Disable comment check box
        //        driver.FindElement((OR.GetElement("VideoManagement", "CommentChkBox", "TVAdminPortalOR.xml"))).Click();

        //        //Disable Like/Dislike check box
        //        driver.FindElement((OR.GetElement("VideoManagement", "LikeDislikeChkBox", "TVAdminPortalOR.xml"))).Click();

        //        //Display QnA check box
        //        //driver.FindElement((OR.GetElement("VideoManagement", "DisplayQA", "TVAdminPortalOR.xml"))).Click();

        //        //select cpd logo
        //        driver.FindElement((OR.GetElement("VideoManagement", "CPDLogo", "TVAdminPortalOR.xml"))).Click();


        //        //Thumbnail choose file button
        //        IWebElement thumbnailButton = driver.FindElement((OR.GetElement("VideoManagement", "FileChooseBTN", "TVAdminPortalOR.xml")));

        //        String thumbnailImageName = cf.readingXMLFile("AdminPortal", "Channel_Management", "channelIcon", "Config.xml");

        //        string thumbnailPath = Environment.CurrentDirectory + "\\Upload\\Images\\" + thumbnailImageName;

        //        uf.uploadfile(thumbnailButton, thumbnailPath);

        //        //Click on upload button
        //        driver.FindElement((OR.GetElement("VideoManagement", "FileUpldBTN", "TVAdminPortalOR.xml"))).Click();

        //        verifySuccessBannerMessage("video thumbnail image uploaded successfully");

        //        uploadBrowseVideo();

        //        finalPublishVideo("normal");

        //        //waiting for 2.5 minutes as video will be publish after 2minutes from current system time
        //        Thread.Sleep(150000);

        //        uf.OpenNewTab(driver);

        //        log.Info("count ::: " + driver.WindowHandles.Count);

        //        String browsertype = uf.getRunningBrowser(driver, driver.FindElement(By.TagName("html")));

        //        uf.SwitchToWebTab(driver, browsertype);

        //        uf.NavigateWebPortal(cf, driver);

        //        //calling from IETTVWebportal project to search created video on web portal and verifying the same
        //        videoResult.searchVideoVerification(videoName, guid_Admin);

        //        videoResult.verifyVideoName(videoName);

        //        videoResult.redirectToLogin();

        //        videoResult.verifyLikeDislikeStatus();

        //        videoResult.verifyCommentStatus();

        //        //videoResult.verifyQnADisplayed();

        //        videoResult.verifyCpdLogo();

        //        videoResult.verifyThumbnail(thumbnailImageName);

        //        log.Info("TVAdmin_011_CreateFreeVidWithDiffPermissions test completed" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

        //    }

        //    catch (Exception e)
        //    {
        //        log.Error(e.Message + "\n" + e.StackTrace + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());
        //        Console.WriteLine(e.Message + "\n" + e.StackTrace + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());
        //        Assert.AreEqual(true, false);

        //    }
        //}

        #endregion

        [TearDown]
        public void TearDown()
        {
            try
            {
                if (TestContext.CurrentContext.Result.Status.ToString().Equals("Failed"))
                {
                    st.Chrome_SetUpTearDowm(driver, log, true);
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








