using IETTVAdminportal.Reusable;
using IETTVWebportal.Chrome;
using log4net;
using log4net.Config;
using NSoup;
using NSoup.Nodes;
using NSoup.Select;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using Sikuli4Net.sikuli_UTIL;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Reflection;
using Utilities.Config;
using Utilities.Object_Repository;
using Utility_Classes;
using System.Globalization;

namespace IETTVAdminPortal.Chrome
{

    [TestFixture]
    public class Chrome_EventManagement
    {

        #region varaible declaration
        private string appURL;
        private Configuration cf;
        internal IWebDriver driver;
        private string driverName;
        private string driverPath;
        private string eventDescriptionContent;
        private string eventURLContent;
        private IJavaScriptExecutor executor;
        private IWait<IWebDriver> iWait;
        private APILauncher launcher;
        private static ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private Chrome_EventManagementVerification objWebEventManagement;
        private Chrome_VideoManagementVerification objWebVideoManagement;
        private Object_Repository_Class OR;
        private Chrome_AdminSetupTearDown st;
        private Utility_Functions uf;
        private Chrome_VideoManagement objAdminVideoManagement = null;
        String event_Title , speaker_AccessCode;
        public string createdSpeakerName;
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
                driver = new ChromeDriver(baseDir + "/DLLs", new ChromeOptions(), TimeSpan.FromSeconds(240)); // Initialize chrome driver 
                EventFire ef = new EventFire(driver);
                driver = ef;

                executor = (IJavaScriptExecutor)driver;

                iWait = new WebDriverWait(driver, TimeSpan.FromSeconds(60));

            }
            catch (Exception exception)
            {
                log.Error(string.Concat(new object[] { exception.Message, "\n", exception.StackTrace, " at line:", new StackTrace(true).GetFrame(0).GetFileLineNumber() }));
                NUnit.Framework.Assert.AreEqual(true, false);
            }
        }

        [SetUp]
        public void SetUp()
        {
            objAdminVideoManagement = new Chrome_VideoManagement(driver, log, executor, iWait);
            objWebVideoManagement = new Chrome_VideoManagementVerification(driver, log, executor, iWait);
            objWebEventManagement = new Chrome_EventManagementVerification(driver, log, executor, iWait);
            appURL = st.Chrome_Setup(driver, log, executor);
        }

        #endregion

        public Chrome_EventManagement()
        {
            driverName = "";
            driver = null;
            iWait = null;
            uf = new Utility_Functions();
            cf = new Configuration();
            OR = new Object_Repository_Class();
            st = new Chrome_AdminSetupTearDown();
            launcher = new APILauncher(true);
            eventDescriptionContent = "Event description content";
            eventURLContent = cf.readingXMLFile("AdminPortal", "Event_Management", "EventURL", "Config.xml");
        }

        public Chrome_EventManagement(IWebDriver driver, ILog log1, IJavaScriptExecutor Executor, IWait<IWebDriver> iWait)
        {
            driverName = "";
            uf = new Utility_Functions();
            cf = new Configuration();
            OR = new Object_Repository_Class();
            st = new Chrome_AdminSetupTearDown();
            launcher = new APILauncher(true);
            eventDescriptionContent = "Event description content";
            eventURLContent = "http://www.google.com";
            this.driver = driver;
            log = log1;
            executor = Executor;
            this.iWait = iWait;
        }

        #region Reusable Elements

        public IWebElement btnAddGrid()
        {
            iWait.Until<IWebElement>(ExpectedConditions.ElementIsVisible((OR.GetElement("EventManagement", "AddGridBTN", "TVAdminPortalOR.xml"))));
            return driver.FindElement((OR.GetElement("EventManagement", "AddGridBTN", "TVAdminPortalOR.xml")));
        }

        public IWebElement btnAddLocation()
        {
            iWait.Until<IWebElement>(ExpectedConditions.ElementIsVisible((OR.GetElement("EventManagement", "AddLocBTN", "TVAdminPortalOR.xml"))));
            return driver.FindElement((OR.GetElement("EventManagement", "AddLocBTN", "TVAdminPortalOR.xml")));
        }

        public IWebElement btnAddNewEvent()
        {
            iWait.Until<IWebElement>(ExpectedConditions.ElementIsVisible((OR.GetElement("VideoManagement", "AddNewBTN", "TVAdminPortalOR.xml"))));
            return driver.FindElement((OR.GetElement("VideoManagement", "AddNewBTN", "TVAdminPortalOR.xml")));
        }

        public IWebElement btnAddRoom()
        {
            iWait.Until<IWebElement>(ExpectedConditions.ElementIsVisible((OR.GetElement("EventManagement", "AddRoomBTN", "TVAdminPortalOR.xml"))));
            return driver.FindElement((OR.GetElement("EventManagement", "AddRoomBTN", "TVAdminPortalOR.xml")));
        }

        public IWebElement btnAddVenue()
        {
            iWait.Until<IWebElement>(ExpectedConditions.ElementIsVisible((OR.GetElement("EventManagement", "AddVenueBTN", "TVAdminPortalOR.xml"))));
            return driver.FindElement((OR.GetElement("EventManagement", "AddVenueBTN", "TVAdminPortalOR.xml")));
        }

        public IWebElement btnEventSave()
        {
            // iWait.Until(ExpectedConditions.ElementIsVisible((OR.GetElement("EventManagement", "EventSaveBTN", "TVAdminPortalOR.xml"))));

            iWait.Until(ExpectedConditions.ElementToBeClickable((OR.GetElement("EventManagement", "EventSaveBTN", "TVAdminPortalOR.xml"))));
            return driver.FindElement((OR.GetElement("EventManagement", "EventSaveBTN", "TVAdminPortalOR.xml")));
        }

        public IWebElement btnPublish()
        {
            iWait.Until<IWebElement>(ExpectedConditions.ElementToBeClickable((OR.GetElement("EventManagement", "PublishBTN", "TVAdminPortalOR.xml"))));
            return driver.FindElement((OR.GetElement("EventManagement", "PublishBTN", "TVAdminPortalOR.xml")));
        }

        public IWebElement btnSave()
        {
            iWait.Until<IWebElement>(ExpectedConditions.ElementIsVisible((OR.GetElement("EventManagement", "SaveBTN", "TVAdminPortalOR.xml"))));
            return driver.FindElement((OR.GetElement("EventManagement", "SaveBTN", "TVAdminPortalOR.xml")));
        }

        public IWebElement btnSaveAddNewLocationPopup()
        {
            iWait.Until<IWebElement>(ExpectedConditions.ElementIsVisible((OR.GetElement("EventManagement", "LocationSaveBTN", "TVAdminPortalOR.xml"))));
            return driver.FindElement((OR.GetElement("EventManagement", "LocationSaveBTN", "TVAdminPortalOR.xml")));
        }

        public IWebElement btnSaveAddRoomPopup()
        {
            iWait.Until<IWebElement>(ExpectedConditions.ElementIsVisible((OR.GetElement("EventManagement", "AddRoomPopupBTN", "TVAdminPortalOR.xml"))));
            return driver.FindElement((OR.GetElement("EventManagement", "AddRoomPopupBTN", "TVAdminPortalOR.xml")));
        }

        public IWebElement btnSearch()
        {
            iWait.Until<IWebElement>(ExpectedConditions.ElementToBeClickable((OR.GetElement("SeriesManagement", "SearchButton", "TVAdminPortalOR.xml"))));
            return driver.FindElement((OR.GetElement("SeriesManagement", "SearchButton", "TVAdminPortalOR.xml")));
        }

        public IWebElement btnSearchVideo()
        {
            iWait.Until<IWebElement>(ExpectedConditions.ElementIsVisible((OR.GetElement("SeriesManagement", "SearchButton", "TVAdminPortalOR.xml"))));
            return driver.FindElement((OR.GetElement("SeriesManagement", "SearchButton", "TVAdminPortalOR.xml")));
        }

        public IWebElement btnSelectVideo(int rowCounter)
        {
            iWait.Until<IWebElement>(ExpectedConditions.ElementIsVisible(OR.GetElement("EventManagement", "SelectBTN", "TVAdminPortalOR.xml", rowCounter)));
            return driver.FindElement(OR.GetElement("EventManagement", "SelectBTN", "TVAdminPortalOR.xml", rowCounter));
        }

        public IWebElement btnSendEmail()
        {
            iWait.Until<IWebElement>(ExpectedConditions.ElementIsVisible((OR.GetElement("EventManagement", "SendMailBTN", "TVAdminPortalOR.xml"))));
            return driver.FindElement((OR.GetElement("EventManagement", "SendMailBTN", "TVAdminPortalOR.xml")));
        }

        public IWebElement btnSponsorUpload()
        {
            iWait.Until<IWebElement>(ExpectedConditions.ElementIsVisible((OR.GetElement("SeriesManagement", "SponsorUploadButton", "TVAdminPortalOR.xml"))));
            return driver.FindElement((OR.GetElement("SeriesManagement", "SponsorUploadButton", "TVAdminPortalOR.xml")));
        }

        public IWebElement chkDisplayQA()
        {
            iWait.Until<IWebElement>(ExpectedConditions.ElementToBeClickable((OR.GetElement("EventManagement", "DisplayQAChk", "TVAdminPortalOR.xml"))));
            return driver.FindElement((OR.GetElement("EventManagement", "DisplayQAChk", "TVAdminPortalOR.xml")));
        }

        public IWebElement chkShowCountdown()
        {
            iWait.Until<IWebElement>(ExpectedConditions.ElementIsVisible((OR.GetElement("EventManagement", "IsShowCountDownChk", "TVAdminPortalOR.xml"))));
            return driver.FindElement((OR.GetElement("EventManagement", "IsShowCountDownChk", "TVAdminPortalOR.xml")));
        }

        public IWebElement ContentPlaceHolder1_btnvenueSave()
        {
            iWait.Until<IWebElement>(ExpectedConditions.ElementIsVisible((OR.GetElement("EventManagement", "SaveVenueBTN", "TVAdminPortalOR.xml"))));
            return driver.FindElement((OR.GetElement("EventManagement", "SaveVenueBTN", "TVAdminPortalOR.xml")));
        }

        public IWebElement ddlAdmin()
        {


            iWait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy((OR.GetElement("VideoRequestByUser", "AdminMenu", "TVAdminPortalOR.xml"))));
            return driver.FindElement((OR.GetElement("VideoRequestByUser", "AdminMenu", "TVAdminPortalOR.xml")));


        }

        public IWebElement ddlCountryAddNewLocationPopup()
        {
            iWait.Until<IWebElement>(ExpectedConditions.ElementIsVisible((OR.GetElement("EventManagement", "LocCountryDDL", "TVAdminPortalOR.xml"))));
            return driver.FindElement((OR.GetElement("EventManagement", "LocCountryDDL", "TVAdminPortalOR.xml")));
        }

        public IWebElement ddlLocationAddVideoPopup()
        {
            iWait.Until<IWebElement>(ExpectedConditions.ElementIsVisible((OR.GetElement("EventManagement", "AddLocationVid", "TVAdminPortalOR.xml"))));
            return driver.FindElement((OR.GetElement("EventManagement", "AddLocationVid", "TVAdminPortalOR.xml")));
        }

        public IWebElement ddlLocationlist()
        {
            iWait.Until<IWebElement>(ExpectedConditions.ElementIsVisible((OR.GetElement("EventManagement", "LocListDDL", "TVAdminPortalOR.xml"))));
            return driver.FindElement((OR.GetElement("EventManagement", "LocListDDL", "TVAdminPortalOR.xml")));
        }

        public IWebElement ddlPromoVideo()
        {
            iWait.Until<IWebElement>(ExpectedConditions.ElementIsVisible((OR.GetElement("EventManagement", "PromoVidDDL", "TVAdminPortalOR.xml"))));
            return driver.FindElement((OR.GetElement("EventManagement", "PromoVidDDL", "TVAdminPortalOR.xml")));
        }

        public IWebElement ddlRoomAddVideoPopup()
        {
            iWait.Until<IWebElement>(ExpectedConditions.ElementExists((OR.GetElement("EventManagement", "RoomListDDL", "TVAdminPortalOR.xml"))));
            return driver.FindElement((OR.GetElement("EventManagement", "RoomListDDL", "TVAdminPortalOR.xml")));
        }

        public IWebElement ddlSearchBy()
        {
            iWait.Until<IWebElement>(ExpectedConditions.ElementToBeClickable((OR.GetElement("EventManagement", "SearchTypeDDL", "TVAdminPortalOR.xml"))));
            return driver.FindElement((OR.GetElement("EventManagement", "SearchTypeDDL", "TVAdminPortalOR.xml")));
        }

        public IWebElement ddlSelectVideo()
        {
            iWait.Until<IWebElement>(ExpectedConditions.ElementIsVisible((OR.GetElement("EventManagement", "SearchTypeDDL", "TVAdminPortalOR.xml"))));
            return driver.FindElement((OR.GetElement("EventManagement", "SearchTypeDDL", "TVAdminPortalOR.xml")));
        }

        public IWebElement ddlVenueAddVideoPopup()
        {
            iWait.Until<IWebElement>(ExpectedConditions.ElementExists((OR.GetElement("EventManagement", "VideoListDDL", "TVAdminPortalOR.xml"))));
            return driver.FindElement((OR.GetElement("EventManagement", "VideoListDDL", "TVAdminPortalOR.xml")));
        }

        public IWebElement lnkEventMangementInDropdown()
        {
            iWait.Until<IWebElement>(ExpectedConditions.ElementToBeClickable((OR.GetElement("EventManagement", "EventManagementLink", "TVAdminPortalOR.xml"))));
            return driver.FindElement((OR.GetElement("EventManagement", "EventManagementLink", "TVAdminPortalOR.xml")));
        }

        public IWebElement rdbEventPriceFree()
        {
            iWait.Until<IWebElement>(ExpectedConditions.ElementIsVisible((OR.GetElement("EventManagement", "EventPriceFreeRdo", "TVAdminPortalOR.xml"))));
            return driver.FindElement((OR.GetElement("EventManagement", "EventPriceFreeRdo", "TVAdminPortalOR.xml")));
        }

        public IWebElement rdbEventPricePremium()
        {
            iWait.Until<IWebElement>(ExpectedConditions.ElementIsVisible((OR.GetElement("EventManagement", "EventPricePaid", "TVAdminPortalOR.xml"))));
            return driver.FindElement((OR.GetElement("EventManagement", "EventPricePaid", "TVAdminPortalOR.xml")));
        }

        public IWebElement rdbPromoDefaultImage()
        {
            iWait.Until<IWebElement>(ExpectedConditions.ElementIsVisible((OR.GetElement("EventManagement", "PromoDefaultImg", "TVAdminPortalOR.xml"))));
            return driver.FindElement((OR.GetElement("EventManagement", "PromoDefaultImg", "TVAdminPortalOR.xml")));
        }

        public IWebElement rdbPromoImage()
        {
            iWait.Until<IWebElement>(ExpectedConditions.ElementIsVisible((OR.GetElement("EventManagement", "PromoImg", "TVAdminPortalOR.xml"))));
            return driver.FindElement((OR.GetElement("EventManagement", "PromoImg", "TVAdminPortalOR.xml")));
        }

        public IWebElement rdbPromoVideo()
        {
            iWait.Until<IWebElement>(ExpectedConditions.ElementIsVisible((OR.GetElement("EventManagement", "PromoVid", "TVAdminPortalOR.xml"))));
            return driver.FindElement((OR.GetElement("EventManagement", "PromoVid", "TVAdminPortalOR.xml")));
        }

        public IWebElement popupAddLocation()
        {
            iWait.Until<IWebElement>(ExpectedConditions.ElementIsVisible((OR.GetElement("EventManagement", "AddLocation", "TVAdminPortalOR.xml"))));
            return driver.FindElement((OR.GetElement("EventManagement", "AddLocation", "TVAdminPortalOR.xml")));
        }

        public IWebElement popupAddRoom()
        {
            iWait.Until<IWebElement>(ExpectedConditions.ElementIsVisible((OR.GetElement("EventManagement", "AddRoom", "TVAdminPortalOR.xml"))));
            return driver.FindElement((OR.GetElement("EventManagement", "AddRoom", "TVAdminPortalOR.xml")));
        }

        public IWebElement popupAddVenue()
        {
            iWait.Until<IWebElement>(ExpectedConditions.ElementIsVisible((OR.GetElement("EventManagement", "AddVenue", "TVAdminPortalOR.xml"))));
            return driver.FindElement((OR.GetElement("EventManagement", "AddVenue", "TVAdminPortalOR.xml")));
        }

        public IWebElement popupAddVideo()
        {
            iWait.Until<IWebElement>(ExpectedConditions.ElementIsVisible((OR.GetElement("EventManagement", "ViewVideoModel", "TVAdminPortalOR.xml"))));
            return driver.FindElement((OR.GetElement("EventManagement", "ViewVideoModel", "TVAdminPortalOR.xml")));
        }

        public IWebElement tabBasicInformation()
        {
            iWait.Until<IWebElement>(ExpectedConditions.ElementToBeClickable((OR.GetElement("EventManagement", "BasicInfoLink", "TVAdminPortalOR.xml"))));
            return driver.FindElement((OR.GetElement("EventManagement", "BasicInfoLink", "TVAdminPortalOR.xml")));
        }

        public IWebElement tabEventAccessCode()
        {
            iWait.Until<IWebElement>(ExpectedConditions.ElementToBeClickable((OR.GetElement("EventManagement", "EventAccessCodeLink", "TVAdminPortalOR.xml"))));
            return driver.FindElement((OR.GetElement("EventManagement", "EventAccessCodeLink", "TVAdminPortalOR.xml")));
        }

        public IWebElement tabEventPrice()
        {
            iWait.Until<IWebElement>(ExpectedConditions.ElementIsVisible((OR.GetElement("EventManagement", "EventPrice", "TVAdminPortalOR.xml"))));
            return driver.FindElement((OR.GetElement("EventManagement", "EventPrice", "TVAdminPortalOR.xml"))).FindElement(By.TagName("a"));
        }

        public IWebElement tabEventVideoAndTimetable()
        {
            iWait.Until<IWebElement>(ExpectedConditions.ElementToBeClickable((OR.GetElement("EventManagement", "EventVidLink", "TVAdminPortalOR.xml"))));
            return driver.FindElement((OR.GetElement("EventManagement", "EventVidLink", "TVAdminPortalOR.xml")));
        }

        public IWebElement tableEventSearchResult()
        {
            iWait.Until<IWebElement>(ExpectedConditions.ElementIsVisible((OR.GetElement("PollManagement", "VideoListSearch", "TVAdminPortalOR.xml"))));
            return driver.FindElement((OR.GetElement("PollManagement", "VideoListSearch", "TVAdminPortalOR.xml")));
        }

        public IWebElement tableVideoSearchResult()
        {
            iWait.Until<IWebElement>(ExpectedConditions.ElementIsVisible((OR.GetElement("PollManagement", "VideoListSearch", "TVAdminPortalOR.xml"))));
            return driver.FindElement((OR.GetElement("PollManagement", "VideoListSearch", "TVAdminPortalOR.xml")));
        }

        public IWebElement tabLocation()
        {
            iWait.Until<IWebElement>(ExpectedConditions.ElementIsVisible((OR.GetElement("EventManagement", "LocationsLink", "TVAdminPortalOR.xml"))));
            return driver.FindElement((OR.GetElement("EventManagement", "LocationsLink", "TVAdminPortalOR.xml")));
        }

        public IWebElement tabPermission()
        {
            iWait.Until<IWebElement>(ExpectedConditions.ElementToBeClickable((OR.GetElement("EventManagement", "PermissionsLink", "TVAdminPortalOR.xml"))));
            return driver.FindElement((OR.GetElement("EventManagement", "PermissionsLink", "TVAdminPortalOR.xml")));
        }

        public IWebElement tabUpload()
        {
            iWait.Until<IWebElement>(ExpectedConditions.ElementIsVisible((OR.GetElement("EventManagement", "UploadTab", "TVAdminPortalOR.xml"))));
            return driver.FindElement((OR.GetElement("EventManagement", "UploadTab", "TVAdminPortalOR.xml"))).FindElement(By.TagName("a"));
        }

        public IWebElement tabvideoTab()
        {
            iWait.Until<IWebElement>(ExpectedConditions.ElementToBeClickable((OR.GetElement("EventManagement", "VideoTab", "TVAdminPortalOR.xml"))));
            return driver.FindElement((OR.GetElement("EventManagement", "VideoTab", "TVAdminPortalOR.xml"))).FindElements(By.TagName("li"))[1].FindElement(By.TagName("a"));
        }

        public IWebElement txtCityAddNewLocationPopup()
        {
            iWait.Until<IWebElement>(ExpectedConditions.ElementIsVisible((OR.GetElement("EventManagement", "CityTXT", "TVAdminPortalOR.xml"))));
            return driver.FindElement((OR.GetElement("EventManagement", "CityTXT", "TVAdminPortalOR.xml")));
        }

        public IWebElement txtDateAddVideoPopup()
        {
            iWait.Until<IWebElement>(ExpectedConditions.ElementIsVisible((OR.GetElement("EventManagement", "EventStartDT", "TVAdminPortalOR.xml"))));
            return driver.FindElement((OR.GetElement("EventManagement", "EventStartDT", "TVAdminPortalOR.xml")));
        }

        public IWebElement txtEmail()
        {
            iWait.Until<IWebElement>(ExpectedConditions.ElementIsVisible((OR.GetElement("EventManagement", "EventTXT", "TVAdminPortalOR.xml"))));
            return driver.FindElement((OR.GetElement("EventManagement", "EventTXT", "TVAdminPortalOR.xml")));
        }

        public IWebElement txtEmailNote()
        {
            iWait.Until<IWebElement>(ExpectedConditions.ElementIsVisible((OR.GetElement("EventManagement", "EmailNoteTXT", "TVAdminPortalOR.xml"))));
            return driver.FindElement((OR.GetElement("EventManagement", "EmailNoteTXT", "TVAdminPortalOR.xml")));
        }

        public IWebElement txtEventCode()
        {
            iWait.Until<IWebElement>(ExpectedConditions.ElementIsVisible((OR.GetElement("EventManagement", "EventCode", "TVAdminPortalOR.xml"))));
            return driver.FindElement((OR.GetElement("EventManagement", "EventCode", "TVAdminPortalOR.xml")));
        }

        public IWebElement txtEventDescription()
        {
            iWait.Until<IWebElement>(ExpectedConditions.ElementToBeClickable((OR.GetElement("SeriesManagement", "SearchTextBox", "TVAdminPortalOR.xml"))));
            return driver.FindElement((OR.GetElement("SeriesManagement", "SearchTextBox", "TVAdminPortalOR.xml")));
        }

        public IWebElement txtEventFromDate()
        {
            iWait.Until<IWebElement>(ExpectedConditions.ElementIsVisible((OR.GetElement("EventManagement", "EventFromDT", "TVAdminPortalOR.xml"))));
            return driver.FindElement((OR.GetElement("EventManagement", "EventFromDT", "TVAdminPortalOR.xml")));
        }

        public IWebElement txtEventTitle()
        {
            iWait.Until<IWebElement>(ExpectedConditions.ElementIsVisible((OR.GetElement("EventManagement", "EventTitleTXT", "TVAdminPortalOR.xml"))));
            return driver.FindElement((OR.GetElement("EventManagement", "EventTitleTXT", "TVAdminPortalOR.xml")));
        }

        public IWebElement txtEventToDate()
        {
            iWait.Until<IWebElement>(ExpectedConditions.ElementIsVisible((OR.GetElement("EventManagement", "EventToDT", "TVAdminPortalOR.xml"))));
            return driver.FindElement((OR.GetElement("EventManagement", "EventToDT", "TVAdminPortalOR.xml")));
        }

        public IWebElement txtEventURL()
        {
            iWait.Until<IWebElement>(ExpectedConditions.ElementIsVisible((OR.GetElement("EventManagement", "URLTxt", "TVAdminPortalOR.xml"))));
            return driver.FindElement((OR.GetElement("EventManagement", "URLTxt", "TVAdminPortalOR.xml")));
        }

        public IWebElement txtFromDateCD()
        {
            iWait.Until<IWebElement>(ExpectedConditions.ElementIsVisible((OR.GetElement("EventManagement", "FromDTCD", "TVAdminPortalOR.xml"))));
            return driver.FindElement((OR.GetElement("EventManagement", "FromDTCD", "TVAdminPortalOR.xml")));
        }

        public IWebElement txtFromTimeAddVideoPopup()
        {
            iWait.Until<IWebElement>(ExpectedConditions.ElementIsVisible((OR.GetElement("EventManagement", "EventFromTime", "TVAdminPortalOR.xml"))));
            return driver.FindElement((OR.GetElement("EventManagement", "EventFromTime", "TVAdminPortalOR.xml")));
        }

        public IWebElement txtFromTimeCD()
        {
            iWait.Until<IWebElement>(ExpectedConditions.ElementIsVisible((OR.GetElement("EventManagement", "FromTimeCD", "TVAdminPortalOR.xml"))));
            return driver.FindElement((OR.GetElement("EventManagement", "FromTimeCD", "TVAdminPortalOR.xml")));
        }

        public IWebElement txtLocationAddNewLocationPopup()
        {
            iWait.Until<IWebElement>(ExpectedConditions.ElementIsVisible((OR.GetElement("EventManagement", "AddLocationTXT", "TVAdminPortalOR.xml"))));
            return driver.FindElement((OR.GetElement("EventManagement", "AddLocationTXT", "TVAdminPortalOR.xml")));
        }

        public IWebElement txtPrice()
        {
            iWait.Until<IWebElement>(ExpectedConditions.ElementIsVisible((OR.GetElement("EventManagement", "PriceTXT", "TVAdminPortalOR.xml"))));
            return driver.FindElement((OR.GetElement("EventManagement", "PriceTXT", "TVAdminPortalOR.xml")));
        }

        public IWebElement txtRoomAddRoomPopup()
        {
            iWait.Until<IWebElement>(ExpectedConditions.ElementIsVisible(OR.GetElement("EventManagement", "AddRoomPopUp", "TVAdminPortalOR.xml")));
            return driver.FindElement(OR.GetElement("EventManagement", "AddRoomPopUp", "TVAdminPortalOR.xml"));
        }

        public IWebElement txtSearch()
        {
            iWait.Until<IWebElement>(ExpectedConditions.ElementToBeClickable((OR.GetElement("SeriesManagement", "SearchTextBox", "TVAdminPortalOR.xml"))));
            return driver.FindElement((OR.GetElement("SeriesManagement", "SearchTextBox", "TVAdminPortalOR.xml")));
        }

        public IWebElement txtSearchVideo()
        {
            iWait.Until<IWebElement>(ExpectedConditions.ElementIsVisible((OR.GetElement("SeriesManagement", "SearchTextBox", "TVAdminPortalOR.xml"))));
            return driver.FindElement((OR.GetElement("SeriesManagement", "SearchTextBox", "TVAdminPortalOR.xml")));
        }

        public IWebElement txtSessionID()
        {
            iWait.Until<IWebElement>(ExpectedConditions.ElementIsVisible((OR.GetElement("EventManagement", "SessionIDTXT", "TVAdminPortalOR.xml"))));
            return driver.FindElement((OR.GetElement("EventManagement", "SessionIDTXT", "TVAdminPortalOR.xml")));
        }

        public IWebElement txtSessionName()
        {
            iWait.Until<IWebElement>(ExpectedConditions.ElementIsVisible((OR.GetElement("EventManagement", "SessionNameTXT", "TVAdminPortalOR.xml"))));
            return driver.FindElement((OR.GetElement("EventManagement", "SessionNameTXT", "TVAdminPortalOR.xml")));
        }

        public IWebElement txtSponsorURL()
        {
            iWait.Until<IWebElement>(ExpectedConditions.ElementIsVisible((OR.GetElement("SeriesManagement", "SponsorTextField", "TVAdminPortalOR.xml"))));
            return driver.FindElement((OR.GetElement("SeriesManagement", "SponsorTextField", "TVAdminPortalOR.xml")));
        }

        public IWebElement txtToTimeAddVideoPopup()
        {
            iWait.Until<IWebElement>(ExpectedConditions.ElementIsVisible((OR.GetElement("EventManagement", "EventToTime", "TVAdminPortalOR.xml"))));
            return driver.FindElement((OR.GetElement("EventManagement", "EventToTime", "TVAdminPortalOR.xml")));
        }

        public IWebElement txtVenueAddVanuePopup()
        {
            iWait.Until<IWebElement>(ExpectedConditions.ElementIsVisible((OR.GetElement("EventManagement", "VenueTXT", "TVAdminPortalOR.xml"))));
            return driver.FindElement((OR.GetElement("EventManagement", "VenueTXT", "TVAdminPortalOR.xml")));
        }

        public IWebElement uplSponsorImage()
        {
            iWait.Until<IWebElement>(ExpectedConditions.ElementIsVisible((OR.GetElement("EventManagement", "UploadIMGBTN", "TVAdminPortalOR.xml"))));
            return driver.FindElement((OR.GetElement("EventManagement", "SponsorImgCSS", "TVAdminPortalOR.xml")));
        }

        public IWebElement accessCodeTab()
        {
            return driver.FindElement((OR.GetElement("VideoManagement", "AccessCodeTab", "TVAdminPortalOR.xml")));
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

        public IWebElement speakerAccessCode()
        {
            iWait.Until(ExpectedConditions.ElementExists((OR.GetElement("VideoManagement", "SpeakerAccessCode", "TVAdminPortalOR.xml"))));

            return driver.FindElement((OR.GetElement("VideoManagement", "SpeakerAccessCode", "TVAdminPortalOR.xml")));
        }

        #endregion

        #region Resusable Function

        public String addVideoDetails(Dictionary<string, string> locationDetails, DateTime videoDate)
        {
            Console.WriteLine("Inside addVideoDetails:::::");
            iWait.Until<IWebElement>(ExpectedConditions.ElementIsVisible((OR.GetElement("EventManagement", "ViewVideoModel", "TVAdminPortalOR.xml"))));
            new SelectElement(ddlLocationAddVideoPopup()).SelectByText(locationDetails["locationName"]);
            OverlayWait();
            //Thread.Sleep(0x3e8);
            Thread.Sleep(3000);
            new SelectElement(ddlVenueAddVideoPopup()).SelectByText(locationDetails["venueContent"]);
            OverlayWait();
            Thread.Sleep(0x3e8);
            new SelectElement(ddlRoomAddVideoPopup()).SelectByText(locationDetails["roomContent"]);
            OverlayWait();
            //Thread.Sleep(0x3e8);
            txtDateAddVideoPopup().Clear();
            //txtDateAddVideoPopup().SendKeys(String.Format("{0:dd/MM/yyyy}", videoDate));
            Thread.Sleep(3000);
            txtDateAddVideoPopup().SendKeys(videoDate.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture));

            Console.WriteLine("Add video date:  " + videoDate);
            DateTime time = DateTime.Now.AddMinutes(2.0);
            DateTime time2 = DateTime.Now.AddMinutes(30.0);
            Thread.Sleep(0x7d0);
            txtFromTimeAddVideoPopup().Clear();
            String day = String.Format("{0:dddd}", videoDate);
            Console.WriteLine("Ecpected Day of the video:::::::::: " + day);
            txtFromTimeAddVideoPopup().SendKeys(String.Format("{0:HHmm}", time));
            txtToTimeAddVideoPopup().Clear();
            txtToTimeAddVideoPopup().SendKeys(String.Format("{0:HHmm}", time2));
            txtSessionID().SendKeys("1");
            txtSessionName().SendKeys("autoTest");
            Thread.Sleep(1000);
            btnEventSave().Click();
            VerifySuccessBannerMessage("Record saved sucessfully.");
            return day;
        }

        public Dictionary<string, string> BasicInformation(string initEventName, DateTime eventStartDate, DateTime eventToDate)
        {
            Console.WriteLine("Inside BasicInformation:::::");

            //Dictionary<string, string> dictionary = new Dictionary<string, string> {
            //    { 
            //        "eventTitle",
            //        initEventName + " " + uf.getGuid()
            //    },
            //    { 
            //        "eventFromDate",
            //        String.Format("{0:dd/MM/yyyy}", eventStartDate)  
            //    },
            //    { 
            //        "eventToDate",
            //         String.Format("{0:dd/MM/yyyy}", eventToDate)
            //    }
            //};

            Dictionary<string, string> dictionary = new Dictionary<string, string> {
                { 
                    "eventTitle",
                    initEventName + " " + uf.getShortGuid()
                },
                { 
                    "eventFromDate", eventStartDate.ToString("dd/MM/yyyy",CultureInfo.InvariantCulture)  
                },
                { 
                    "eventToDate", eventToDate.ToString("dd/MM/yyyy",CultureInfo.InvariantCulture)
                }
            };





            txtEventTitle().Clear();
            txtEventTitle().SendKeys(dictionary["eventTitle"]);
            Console.WriteLine("Event Title : " + txtEventTitle().GetAttribute("value"));

            //changes
            event_Title = txtEventTitle().GetAttribute("value");

            cf.writingIntoXML("Admin", "Video Management", "eventTitle", event_Title, "TestCopy.xml");

            txtEventFromDate().Clear();
            txtEventFromDate().SendKeys(dictionary["eventFromDate"]);
            txtEventToDate().Clear();
            txtEventToDate().SendKeys(dictionary["eventToDate"]);
            Thread.Sleep(2000);
            iWait.Until<IWebElement>(ExpectedConditions.ElementExists((OR.GetElement("EventManagement", "FrameClass", "TVAdminPortalOR.xml"))));
            IWebElement frameElement = driver.FindElement((OR.GetElement("EventManagement", "FrameClass", "TVAdminPortalOR.xml")));
            driver.SwitchTo().Frame(frameElement);
            IWebElement element = driver.FindElement(By.TagName("body"));
            Actions actions = new Actions(driver);
            Thread.Sleep(3000);
            actions.SendKeys(element, eventDescriptionContent).Build().Perform();
            driver.SwitchTo().DefaultContent();
            txtEventURL().SendKeys(eventURLContent);

            return dictionary;
        }

        public String EventAccessCodes()
        {
            Console.WriteLine("Inside EventAccessCodes:::::");
            tabEventAccessCode().Click();
            string attribute = txtEventCode().GetAttribute("value");
            txtEmailNote().SendKeys("automation note field");
            txtEmail().SendKeys("babbanchandak26@gmail.com");
            return attribute;
        }

        public void EventPrice(string eventType)
        {
            Console.WriteLine("Inside EventPrice:::::");

            tabEventPrice().Click();
            if (eventType.Equals("free"))
            {
                rdbEventPriceFree().Click();
            }
            if (eventType.Equals("premium"))
            {
                rdbEventPricePremium().Click();
                txtPrice().Clear();
                txtPrice().SendKeys("50.00");
            }
        }

        public Dictionary<string, string> Location()
        {
            Console.WriteLine("Inside Location:::::");
            uf.scrollUp(driver);
            string text = "ven" + uf.getShortGuid();
            string str2 = "room" + uf.getShortGuid();

           // cf.writingIntoXML("Admin" ,"Event Management","EventRoom" ,str2 ,"TestCopy.xml");
            Thread.Sleep(2000);
            tabLocation().Click();
            
            new SelectElement(ddlLocationlist()).SelectByIndex(1);
            string str3 = new SelectElement(ddlLocationlist()).SelectedOption.Text;
            OverlayWait();
            Thread.Sleep(3000);
            btnAddVenue().Click();
            iWait.Until<IWebElement>(ExpectedConditions.ElementIsVisible((OR.GetElement("EventManagement", "AddVenue", "TVAdminPortalOR.xml"))));
            txtVenueAddVanuePopup().SendKeys(text);
            ContentPlaceHolder1_btnvenueSave().Click();
            VerifySuccessBannerMessage("Venue added successfully.");
            btnAddRoom().Click();

            OverlayWait();

            iWait.Until<IWebElement>(ExpectedConditions.ElementIsVisible((OR.GetElement("EventManagement", "AddRoom", "TVAdminPortalOR.xml"))));

           // string retrieveRoom = cf.readingXMLFile("Admin", "Event Management", "EventRoom", "TestCopy.xml");

            

            Thread.Sleep(5000);
            txtRoomAddRoomPopup().SendKeys(str2);
            //txtRoomAddRoomPopup().SendKeys(retrieveRoom);
            Thread.Sleep(3000);
            btnSaveAddRoomPopup().Click();
            VerifySuccessBannerMessage("Room added successfully.");
            btnAddGrid().Click();
            OverlayWait();
            //string retrieveRoom = cf.readingXMLFile("Admin", "Event Management", "EventRoom", "TestCopy.xml");
            return new Dictionary<string, string> { 
                { 
                    "locationName",
                    str3
                },
                { 
                    "venueContent",
                    text
                },
                { 
                    "roomContent",
                    //str2
                    str2
                    
                }
            };
        }

        public void OverlayWait()
        {
            iWait.Until(ExpectedConditions.InvisibilityOfElementLocated((OR.GetElement("SeriesManagement", "OverlayWait", "TVAdminPortalOR.xml"))));
        }

        public String PromoImage()
        {
            Console.WriteLine("Inside PromoImage:::::");

            Thread.Sleep(5000);
            rdbPromoImage().Click();
            IWebElement fileInput = driver.FindElement((OR.GetElement("EventManagement", "UploadPromImg", "TVAdminPortalOR.xml")));
            string str = cf.readingXMLFile("AdminPortal", "Channel_Management", "channelIcon", "Config.xml");
            string uploadPath = Environment.CurrentDirectory + @"\Upload\Images\" + str;
            uf.uploadfile(fileInput, uploadPath);
            return str;
        }

        public void PromoVideo(string promoVideoName)
        {
            Console.WriteLine("Inside PromoVideo:::::");
            rdbPromoVideo().Click();
            new SelectElement(ddlPromoVideo()).SelectByText(promoVideoName);
            promoVideoName = cf.readingXMLFile("AdminPortal", "VideoManagement", "PromoVideoName", "SysConfig.xml");
        }

        public void RedirectToEventManagement()
        {
            log.Info("inside RedirectToEventManagement  at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());
            ddlAdmin().Click();
            Thread.Sleep(0x7d0);
            lnkEventMangementInDropdown().Click();
        }

        public void SearchEvent(string eventName, string searchBy)
        {
            Console.WriteLine("Inside SearchEvent:::::");
            new SelectElement(ddlSearchBy()).SelectByText(searchBy);
            txtSearch().SendKeys(eventName);
            btnSearch().Click();
        }

        public void SearchVideo(string videoName)
        {
            Console.WriteLine("Inside SearchVideo:::::");
            new SelectElement(ddlSearchBy()).SelectByText("Title");
            txtSearch().SendKeys(videoName);
            btnSearch().Click();
            OverlayWait();
        }

        public bool SelectEventFromSearchResult(string eventName)
        {
            Console.WriteLine("Inside SelectEventFromSearchResult:::::");
            tableEventSearchResult();
            Elements elementsByTag = NSoupClient.Parse(driver.PageSource).GetElementById(OR.readingXMLFile("PollManagement", "VideoListSearch", "TVAdminPortalOR.xml")).GetElementsByTag("tr");
            int num = 0;
            bool flag = false;
            foreach (Element element in (IEnumerable<Element>)elementsByTag)
            {
                Attributes attributes = element.Attributes;
                if (attributes["class"].Equals("GridRowStyle") || attributes["class"].Equals("AltGridStyle"))
                {
                    log.Info("Row Counter :: " + num);


                    if (driver.FindElement(OR.GetElement("PollManagement", "TitleSearchLnk", "TVAdminPortalOR.xml", num)).Text.Trim().ToLower().Equals(eventName.ToLower()))
                    {
                        driver.FindElement(OR.GetElement("PollManagement", "TitleSearchLnk", "TVAdminPortalOR.xml", num)).Click();
                        uf.isJqueryActive(driver);
                        flag = true;
                    }
                    num++;
                }
            }
            return flag;
        }

        public void SelectVideoFromSearchResult(string videoName)
        {
            Console.WriteLine("Inside SelectEventFromSearchResult:::::");
            iWait.Until<IWebElement>(ExpectedConditions.ElementExists((OR.GetElement("PollManagement", "VideoListSearch", "TVAdminPortalOR.xml"))));
            IWebElement element = driver.FindElement((OR.GetElement("PollManagement", "VideoListSearch", "TVAdminPortalOR.xml")));
            iWait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy(By.TagName("tr")));
            IList<IWebElement> list = element.FindElements(By.TagName("tr"));
            bool flag = false;
            uf.isJqueryActive(driver);
            int num = 0;
            foreach (IWebElement element2 in list)
            {
                if (element2.GetAttribute("class").Equals("GridRowStyle") || element2.GetAttribute("class").Equals("AltGridStyle"))
                {
                    string str = element2.FindElements(By.TagName("td"))[1].FindElement(By.TagName("span")).Text.Trim();
                    Console.WriteLine("Video Title from Search page::" + str);
                    if (str.Equals(videoName))
                    {
                        flag = true;
                        Thread.Sleep(2000);
                        driver.FindElement(OR.GetElement("EventManagement", "SelectBTN", "TVAdminPortalOR.xml", num)).Click();
                        OverlayWait();
                    }
                    num++;
                }
            }
        }

        public void ShowCountdown()
        {
            Console.WriteLine("Inside ShowCountdown:::::");
            chkShowCountdown().Click();
            DateTime time = DateTime.Now.AddMinutes(1.0);
            iWait.Until<bool>(d => txtFromDateCD().Enabled);
            txtFromDateCD().SendKeys(time.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture));
            // txtFromDateCD().SendKeys(String.Format("{0:dd/MM/yyyy}", time));
            txtFromTimeCD().Clear();
            txtFromTimeCD().SendKeys(String.Format("{0:HH:mm}", time));

        }

        public Dictionary<string, string> SponsorDetails()
        {
            log.Info("inside SponsorDetails  at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());
            Dictionary<string, string> dictionary = new Dictionary<string, string> {
                { 
                    "url",eventURLContent
                    
                },
                { 
                    "imageName",
                    cf.readingXMLFile("AdminPortal", "Channel_Management", "channelIcon", "Config.xml")
                }
            };
            txtSponsorURL().SendKeys(dictionary["url"]);
            string uploadPath = Environment.CurrentDirectory + @"\Upload\Images\" + dictionary["imageName"];
            Console.WriteLine("uploadSponsorName: " + dictionary["imageName"]);
            Console.WriteLine("uploadSponsorPath: " + uploadPath);
            uf.uploadfile(uplSponsorImage(), uploadPath);
            btnSponsorUpload().Click();
            VerifySuccessBannerMessage("Record is added successfully.");
            return dictionary;
        }

        public String UploadEventLogo()
        {
            Console.WriteLine("Inside UploadEventLogo:::::");
            iWait.Until<IWebElement>(ExpectedConditions.ElementIsVisible((OR.GetElement("EventManagement", "UplEventLogo", "TVAdminPortalOR.xml"))));
            IWebElement fileInput = driver.FindElement((OR.GetElement("EventManagement", "UplEventLogo", "TVAdminPortalOR.xml")));
            string eventLogoName = cf.readingXMLFile("AdminPortal", "Channel_Management", "channelIcon", "Config.xml");
            string uploadPath = Environment.CurrentDirectory + @"\Upload\Images\" + eventLogoName;
            uf.uploadfile(fileInput, uploadPath);
            return eventLogoName;
        }

        public void VerifyDefaultSuccessBannerMessage(string message)
        {
            Console.WriteLine("Inside VerifyDefaultSuccessBannerMessage:::::");
            IWait<IWebDriver> wait = new WebDriverWait(driver, TimeSpan.FromSeconds(180.0));
            wait.Until<IWebElement>(ExpectedConditions.ElementIsVisible((OR.GetElement("VideoManagement", "SuccessBTN", "TVAdminPortalOR.xml"))));
            iWait.Until<bool>(d => d.FindElement((OR.GetElement("VideoManagement", "DefaultSuccessLang", "TVAdminPortalOR.xml"))).Text.Equals(message.Trim()));
            NUnit.Framework.Assert.AreEqual(message, driver.FindElement((OR.GetElement("VideoManagement", "DefaultSuccessLang", "TVAdminPortalOR.xml"))).Text.Trim());
            driver.FindElement((OR.GetElement("VideoManagement", "SuccessBTN", "TVAdminPortalOR.xml"))).Click();
            OverlayWait();
        }

        public void VerifySuccessBannerMessage(string message)
        {
            Console.WriteLine("Inside VerifySuccessBannerMessage:::::");
            IWait<IWebDriver> wait = new WebDriverWait(driver, TimeSpan.FromSeconds(180.0));
            wait.Until<IWebElement>(ExpectedConditions.ElementIsVisible((OR.GetElement("SeriesManagement", "SuccessOkButton", "TVAdminPortalOR.xml"))));
            iWait.Until<bool>(d => d.FindElement((OR.GetElement("SeriesManagement", "SucessMessage", "TVAdminPortalOR.xml"))).Text.Equals(message));
            NUnit.Framework.Assert.AreEqual(message, driver.FindElement((OR.GetElement("SeriesManagement", "SucessMessage", "TVAdminPortalOR.xml"))).Text.Trim());
            driver.FindElement((OR.GetElement("SeriesManagement", "SuccessOkButton", "TVAdminPortalOR.xml"))).Click();
            iWait.Until(ExpectedConditions.InvisibilityOfElementLocated((OR.GetElement("SeriesManagement", "SuccessOkButton", "TVAdminPortalOR.xml"))));
            OverlayWait();
        }

        public String CreateFreeLiveVideo(String currentDate)
        {
            Console.WriteLine("Inside CreateFreeLiveVideo:::::");

            objAdminVideoManagement.redirectToVideoManagement();

            String videoName = objAdminVideoManagement.basicInfoTab("Live Video ");

            objAdminVideoManagement.channelListTab();

            objAdminVideoManagement.pricingListTab("Free");

            objAdminVideoManagement.addcopyright();

            objAdminVideoManagement.publishTab().Click();

            objAdminVideoManagement.rdoLiveVideo().Click();

            //Getting sysytem current date and adding 2minutes in time as video upload time should be greater than system current time
            // String currentDate = DateTime.Now.AddMinutes(2).ToString("dd/MM/yyyy HHmm");

            String[] dateTime = currentDate.Split(' ');

            log.Info("Date to final publish video :: " + dateTime[0].Trim());

            log.Info("Time at video will final publish :: " + dateTime[1].Trim());

            //Selecting todays date from date picker
            objAdminVideoManagement.finalPublishFromDate().Clear();
            objAdminVideoManagement.finalPublishFromDate().SendKeys(dateTime[0].Trim());

            //enter the time in the Final Time field
            objAdminVideoManagement.finalPublishFromTime().Clear();
            objAdminVideoManagement.finalPublishFromTime().SendKeys(dateTime[1].Trim());


            //Selecting bitrates
            driver.FindElement((OR.GetElement("PollManagement", "Costombitrate1", "TVAdminPortalOR.xml"))).SendKeys("400");
            //new SelectElement(driver.FindElement(By.Id("combobox1"))).SelectByIndex(5);
            driver.FindElement((OR.GetElement("PollManagement", "Width1TXT", "TVAdminPortalOR.xml"))).SendKeys("360");
            driver.FindElement((OR.GetElement("PollManagement", "Height1TXT", "TVAdminPortalOR.xml"))).SendKeys("240");

            driver.FindElement((OR.GetElement("PollManagement", "SubmitLiveData", "TVAdminPortalOR.xml"))).Click();

            iWait.Until(ExpectedConditions.InvisibilityOfElementLocated((OR.GetElement("SeriesManagement", "OverlayWait", "TVAdminPortalOR.xml"))));

            // Getting sysytem current date and adding 2minutes in time as video upload time should be greater than system current time
            currentDate = DateTime.Now.AddMinutes(2).ToString("dd/MM/yyyy HHmm", CultureInfo.InvariantCulture);

            dateTime = currentDate.Split(' ');

            log.Info("Date to final publish video :: " + dateTime[0].Trim());

            log.Info("Time at video will final publish :: " + dateTime[1].Trim());

            //Selecting todays date from date picker
            objAdminVideoManagement.finalPublishFromDate().Clear();
            objAdminVideoManagement.finalPublishFromDate().SendKeys(dateTime[0].Trim());

            //enter the time in the Final Time field
            objAdminVideoManagement.finalPublishFromTime().Clear();
            objAdminVideoManagement.finalPublishFromTime().SendKeys(dateTime[1].Trim());

            objAdminVideoManagement.videoPublishButton().Click();

            //waiting for loader
            iWait.Until(ExpectedConditions.ElementExists((OR.GetElement("VideoManagement", "Loading", "TVAdminPortalOR.xml"))));
            log.Info("loading is over");


            //  iWait.Until(d => d.FindElement((OR.GetElement("VideoManagement", "DefaultSuccessLang", "TVAdminPortalOR.xml"))).Text.Equals("Final Video Published Successfully."));
            // SuccessBannerMessage = driver.FindElement((OR.GetElement("VideoManagement", "DefaultSuccessLang", "TVAdminPortalOR.xml")));

            iWait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy((OR.GetElement("VideoManagement", "CustomMsgCSS", "TVAdminPortalOR.xml"))));

            iWait.Until(ExpectedConditions.ElementIsVisible((OR.GetElement("VideoManagement", "SuccessBTN", "TVAdminPortalOR.xml"))));

            //Verifying Success banner message
            IWebElement SuccessBannerMessage = driver.FindElement((OR.GetElement("VideoManagement", "DefaultSuccessLang", "TVAdminPortalOR.xml")));
            String Publish_Successful_Message = SuccessBannerMessage.Text;

            Assert.AreEqual("Live Video Published Successfully.", Publish_Successful_Message);

            //Click on ok button of banner message
            IWebElement OkButton = driver.FindElement((OR.GetElement("VideoManagement", "SuccessBTN", "TVAdminPortalOR.xml")));
            executor.ExecuteScript("arguments[0].click();", OkButton);

            iWait.Until(ExpectedConditions.InvisibilityOfElementLocated((OR.GetElement("VideoManagement", "SuccessBTN", "TVAdminPortalOR.xml"))));

            return videoName;
        }

        public String CreateEventWithQA(String videoName)
        {
            string initEventName = "autoEvn";

            RedirectToEventManagement();

            btnAddNewEvent().Click();

            Dictionary<string, string> dictionary = BasicInformation(initEventName, DateTime.Now, DateTime.Now.AddDays(3.0));

            driver.FindElement((OR.GetElement("EventManagement", "Li2", "TVAdminPortalOR.xml"))).FindElement(By.TagName("a")).Click();

            iWait.Until(ExpectedConditions.ElementIsVisible((OR.GetElement("EventManagement", "DisplayQAChk", "TVAdminPortalOR.xml"))));

            driver.FindElement((OR.GetElement("EventManagement", "DisplayQAChk", "TVAdminPortalOR.xml"))).Click();

            Dictionary<string, string> locationDetails = Location();

            OverlayWait();

            btnSave().Click();

            VerifyDefaultSuccessBannerMessage("Event successfully added.");

            tabEventVideoAndTimetable().Click();

            tabvideoTab().Click();

            SearchVideo(videoName);

            SelectVideoFromSearchResult(videoName);

            addVideoDetails(locationDetails, DateTime.Now);

            OverlayWait();

            EventPrice("free");

            btnSave().Click();

            VerifySuccessBannerMessage("Event successfully saved.");

            btnPublish().Click();

            VerifyDefaultSuccessBannerMessage("Event successfully published.");

            return dictionary["eventTitle"];

        }

        public void SearchEvent(Dictionary<string, string> dictionary)
        {
            Console.WriteLine("Inside SearchEvent:::::");

            driver.FindElement((OR.GetElement("SeriesManagement", "SearchTextBox", "TVAdminPortalOR.xml"))).SendKeys(dictionary["eventTitle"]);

            driver.FindElement((OR.GetElement("SeriesManagement", "SearchButton", "TVAdminPortalOR.xml"))).Click();

            driver.FindElement((OR.GetElement("EventManagement", "ImageEdit", "TVAdminPortalOR.xml"))).Click();

            iWait.Until(ExpectedConditions.ElementIsVisible((OR.GetElement("EventManagement", "EventTitleTXT", "TVAdminPortalOR.xml"))));
        }

        //This function select the channel
        public String SelectChannel(String channelType)
        {
            String ChannelName = null;
            log.Info("inside SelectChannel " + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

            uf.scrollUp(driver);

            //Click on Channel tab
            driver.FindElement((OR.GetElement("EventManagement", "ChannellistTab", "TVAdminPortalOR.xml"))).FindElement(By.TagName("a")).Click();

            //Selecting channel from the default Channel dropdown
            iWait.Until(ExpectedConditions.ElementIsVisible((OR.GetElement("VideoManagement", "DefaultChannelDDL", "TVAdminPortalOR.xml"))));
            SelectElement DefaultChanneleSelector = new SelectElement(driver.FindElement((OR.GetElement("VideoManagement", "DefaultChannelDDL", "TVAdminPortalOR.xml"))));

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
            if (channelType.Equals("paid"))
            {
                // ChannelName = cf.readingXMLFile("AdminPortal", "Channel_Management", "PaidChannelName", "Config.xml");
                ChannelName = cf.readingXMLFile("Admin", "Channel Management", "channelname", "TestCopy.xml");
            }
            else if (channelType.Equals("member"))
            {
                ChannelName = cf.readingXMLFile("AdminPortal", "Channel_Management", "MemberChannelName", "Config.xml");

            }

            DefaultChanneleSelector.SelectByText(ChannelName);

            String adminChannelName = DefaultChanneleSelector.SelectedOption.Text;

            log.Info("Selected channel name :" + adminChannelName);

            return adminChannelName;

        }

        public void logOut()
        {
            Thread.Sleep(2000);

            iWait.Until(ExpectedConditions.ElementIsVisible((OR.GetElement("SeriesManagement", "MyIETDropDown", "TVAdminPortalOR.xml"))));

            IWebElement myAccountDropDown = driver.FindElement((OR.GetElement("SeriesManagement", "MyIETDropDown", "TVAdminPortalOR.xml")));

            executor.ExecuteScript("arguments[0].click()", myAccountDropDown);

            iWait.Until(ExpectedConditions.ElementIsVisible((OR.GetElement("SeriesManagement", "LogoutLink", "TVAdminPortalOR.xml"))));
            driver.FindElement((OR.GetElement("SeriesManagement", "LogoutLink", "TVAdminPortalOR.xml"))).FindElement(By.TagName("a")).Click();
        }

        #endregion

        /// <summary>
        /// Create free event with default image,countdown,promo image
        /// and verify the same on web portal
        /// </summary>
        [Test]
        public void TVAdmin_001_FreeEventFunctionality()
        {
            try
            {
                Console.WriteLine("TVAdmin_001_CreateFreeEvent test Started" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

                objAdminVideoManagement.redirectToVideoManagement();

                String initVideoName = "autoVid";
                String videoName = objAdminVideoManagement.basicInfoTab(initVideoName);

                SelectChannel("paid");

                objAdminVideoManagement.pricingListTab("free");

                objAdminVideoManagement.addcopyright();

                objAdminVideoManagement.uploadBrowseVideo();

                objAdminVideoManagement.finalPublishVideo("normal");

                string initEventName = "autoEvn";

                RedirectToEventManagement();

                btnAddNewEvent().Click();

                Dictionary<string, string> dictionary = BasicInformation(initEventName, DateTime.Now.AddDays(2.0), DateTime.Now.AddDays(3.0));

                Dictionary<string, string> locationDetails = Location();
                

                OverlayWait();

                Thread.Sleep(3000);

                btnSave().Click();

                VerifyDefaultSuccessBannerMessage("Event successfully added.");

                tabEventVideoAndTimetable().Click();

                tabvideoTab().Click();

                SearchVideo(videoName);
                //SearchVideo("autoVid913566");

                SelectVideoFromSearchResult(videoName);

                addVideoDetails(locationDetails, DateTime.Now.AddDays(2.0));

                OverlayWait();

                EventAccessCodes();

                tabUpload().Click();

                ShowCountdown();

                rdbPromoDefaultImage().Click();

                // bug-    Dictionary<string, string> dictionarySponsor = SponsorDetails();

                EventPrice("free");

                btnSave().Click();

                VerifySuccessBannerMessage("Event successfully saved.");

                btnPublish().Click();

                VerifyDefaultSuccessBannerMessage("Event successfully published.");

                Thread.Sleep(60000);

                uf.OpenNewTab(driver);

                log.Info("count ::: " + driver.WindowHandles.Count);

                string browserType = uf.getRunningBrowser(driver, driver.FindElement(By.TagName("html")));

                uf.SwitchToWebTab(driver, browserType);

                uf.NavigateWebPortal(cf, driver);

                uf.scrollUp(driver);

                Thread.Sleep(4000);

                handlePopup();

                Thread.Sleep(5000);

                objWebEventManagement.SearchEvent(dictionary["eventTitle"]);

                objWebEventManagement.ClickAndVerifyEventName(dictionary["eventTitle"]);

                objWebEventManagement.VerifyCountDownDetails();

                objWebEventManagement.VerifyDefaultImage();

                uf.SwitchToAdminTab(driver, browserType);

                SearchEvent(dictionary);

                tabUpload().Click();

                String promoImageName = PromoImage();

                btnSave().Click();

                VerifySuccessBannerMessage("Event successfully saved.");

                btnPublish().Click();

                VerifyDefaultSuccessBannerMessage("Event successfully published.");

                Thread.Sleep(60000);

                uf.SwitchToWebTab(driver, browserType);

                uf.NavigateWebPortal(cf, driver);

                objWebEventManagement.SearchEvent(dictionary["eventTitle"]);

                objWebEventManagement.ClickAndVerifyEventName(dictionary["eventTitle"]);

                objWebEventManagement.VerifyPromoImage(promoImageName);

                objWebEventManagement.VerifyFreeEvent();

                objWebEventManagement.VerifyEventURL(eventURLContent);

                objWebEventManagement.GetEventVideoStatus(videoName, "ON DEMAND");

            }
            catch (Exception e)
            {
                log.Error(e.Message + "\n" + e.StackTrace + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());
                Console.WriteLine(e.Message + "\n" + e.StackTrace + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());
                NUnit.Framework.Assert.AreEqual(true, false);
            }
        }

        /// <summary>
        /// Create premium event with default image,countdown,promo image,access code and logo image
        /// and verify the same on web portal
        /// </summary>
        [Test]
        public void TVAdmin_002_PremiumEventFunctionality()
        {
            try
            {
                Console.WriteLine("TVAdmin_002_PremiumEventFunctionality test Started" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

                #region create premium event
                //create premium event

                //only location tab n event price details n den save details
                string initEventName = "autoEvn";

                RedirectToEventManagement();

                Thread.Sleep(3000);

                btnAddNewEvent().Click();

                Dictionary<string, string> dictionary = BasicInformation(initEventName, DateTime.Now.AddDays(2.0), DateTime.Now.AddDays(3.0));

                Dictionary<string, string> locationDetails = Location();

                EventPrice("premium");

                OverlayWait();

                btnSave().Click();

                VerifyDefaultSuccessBannerMessage("Event successfully added.");

                //premium event
                #endregion

                objAdminVideoManagement.redirectToVideoManagement();

                String initVideoName = "autoVid";
                String videoName = objAdminVideoManagement.basicInfoTab(initVideoName);

                SelectChannel("paid");

                //changes 

                //pricing tab
                //generate video access code
                //create speaker - take the speaker access code generated
                //attach the event created


                //objAdminVideoManagement.pricingListTab("free");
                objAdminVideoManagement.pricingListTab("Event Priced");

                objAdminVideoManagement.videoAccessCodeTab();

                objAdminVideoManagement.addcopyright();

                Dictionary<String, String> adminSpeakerDetails = createSpeaker();

                verifySpeakerCreated();

                objAdminVideoManagement.eventTab();

                objAdminVideoManagement.uploadBrowseVideo();

                objAdminVideoManagement.finalPublishVideo("normal");

                //string initEventName = "autoEvn";

                //RedirectToEventManagement();

                //Thread.Sleep(3000);

                //btnAddNewEvent().Click();

                //Dictionary<string, string> dictionary = BasicInformation(initEventName, DateTime.Now.AddDays(2.0), DateTime.Now.AddDays(3.0));

                //Dictionary<string, string> locationDetails = Location();

                //OverlayWait();

                //btnSave().Click();

                //VerifyDefaultSuccessBannerMessage("Event successfully added.");

                tabEventVideoAndTimetable().Click();

                tabvideoTab().Click();

                SearchVideo(videoName);

                SelectVideoFromSearchResult(videoName);

                String videoEventday = addVideoDetails(locationDetails, DateTime.Now.AddDays(2.0));

                OverlayWait();

                EventAccessCodes();

                tabUpload().Click();

                ShowCountdown();

                rdbPromoDefaultImage().Click();

                EventPrice("premium");

                btnSave().Click();

                VerifySuccessBannerMessage("Event successfully saved.");

                btnPublish().Click();

                VerifyDefaultSuccessBannerMessage("Event successfully published.");

                //take the new event access code generated

                Thread.Sleep(60000);

                uf.OpenNewTab(driver);

                log.Info("count ::: " + driver.WindowHandles.Count);

                string browserType = uf.getRunningBrowser(driver, driver.FindElement(By.TagName("html")));

                uf.SwitchToWebTab(driver, browserType);

                uf.NavigateWebPortal(cf, driver);

                uf.scrollUp(driver);

                Thread.Sleep(4000);

                handlePopup();

                Thread.Sleep(5000);

                objWebEventManagement.SearchEvent(dictionary["eventTitle"]);

                objWebEventManagement.ClickAndVerifyEventName(dictionary["eventTitle"]);

                objWebEventManagement.VerifyCountDownDetails();

                objWebEventManagement.VerifyDefaultImage();

                uf.SwitchToAdminTab(driver, browserType);

                SearchEvent(dictionary);

                tabUpload().Click();

                String promoImageName = PromoImage();

                btnSave().Click();

                VerifySuccessBannerMessage("Event successfully saved.");

                btnPublish().Click();

                VerifyDefaultSuccessBannerMessage("Event successfully published.");

                Thread.Sleep(60000);

                uf.SwitchToWebTab(driver, browserType);

                uf.NavigateWebPortal(cf, driver);

                objWebEventManagement.SearchEvent(dictionary["eventTitle"]);

                objWebEventManagement.ClickAndVerifyEventName(dictionary["eventTitle"]);

                objWebEventManagement.VerifyPromoImage(promoImageName);

                objWebEventManagement.VerifyPremiumEvent();

                objWebEventManagement.VerifyEventURL(eventURLContent);

                uf.SwitchToAdminTab(driver, browserType);

                SearchEvent(dictionary);

                String eventAccessCodeContent = EventAccessCodes();

                btnSave().Click();

                VerifySuccessBannerMessage("Event successfully saved.");

                tabUpload().Click();
                String eventLogoName = UploadEventLogo();

                btnPublish().Click();

                VerifyDefaultSuccessBannerMessage("Event successfully published.");

                Thread.Sleep(6000);

                uf.SwitchToWebTab(driver, browserType);

                uf.NavigateWebPortal(cf, driver);

                handlePopup();

                objWebEventManagement.SearchEvent(dictionary["eventTitle"]);
                //objWebEventManagement.SearchEvent("autoEvn 0753d");

                objWebEventManagement.ClickAndVerifyEventName(dictionary["eventTitle"]);
                //objWebEventManagement.ClickAndVerifyEventName("autoEvn 0753d");

                objWebEventManagement.VerifyEventLogo(eventLogoName);

                objWebEventManagement.SearchAndClickVideoSchedule(videoEventday);
                //objWebEventManagement.SearchAndClickVideoSchedule("Wednesday");

                objWebEventManagement.SelectVideoFromVideoSheduleSection(videoName);
                //objWebEventManagement.SelectVideoFromVideoSheduleSection("autoVid29a5b6");

                objWebEventManagement.ClickAndVerifyEventAcessCode(eventAccessCodeContent);
                //objWebEventManagement.ClickAndVerifyEventAcessCode("EV9y8L9h5j8x54616");

                Thread.Sleep(3000);
                logOut();

                Console.WriteLine("TVAdmin_002_PremiumEventFunctionality test completed" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

            }
            catch (Exception e)
            {
                log.Error(e.Message + "\n" + e.StackTrace + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());
                Console.WriteLine(e.Message + "\n" + e.StackTrace + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());
                NUnit.Framework.Assert.AreEqual(true, false);
            }

        }

        //public void accessCodeTab()
        //{
        //    log.Info("inside accessCodeTab " + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());


        //}

        public void handlePopup()
        {
            Thread.Sleep(4000);

            objWebEventManagement.handlePromotionalPopup();

            objWebEventManagement.HandleEmergencyPopUp();
        }

        public void editCreatedEvent()
        {
            driver.FindElement((OR.GetElement("SeriesManagement", "SearchButton", "TVAdminPortalOR.xml"))).Click();

            driver.FindElement((OR.GetElement("EventManagement", "ImageEdit", "TVAdminPortalOR.xml"))).Click();

            iWait.Until(ExpectedConditions.ElementIsVisible((OR.GetElement("EventManagement", "EventTitleTXT", "TVAdminPortalOR.xml"))));

            //tabEventVideoAndTimetable().Click();

            //driver.FindElement((OR.GetElement("EventManagement", "EventEdit", "TVAdminPortalOR.xml"))).Click();
        }

        public string eventPublishVideoStatus()
        {
            tabEventVideoAndTimetable().Click();

            driver.FindElement((OR.GetElement("EventManagement", "EventEdit", "TVAdminPortalOR.xml"))).Click();

            Thread.Sleep(5000);
            txtDateAddVideoPopup().Clear();
            //txtDateAddVideoPopup().SendKeys(String.Format("{0:dd/MM/yyyy}", CultureInfo.InvariantCulture));
            txtDateAddVideoPopup().SendKeys(DateTime.Now.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture));

            String newVideoEventDay = String.Format("{0:dddd}", DateTime.Now);
            Console.WriteLine("day:::::::::: " + newVideoEventDay);

            txtFromTimeAddVideoPopup().Clear();
            Thread.Sleep(1000);
            txtFromTimeAddVideoPopup().SendKeys(String.Format("{0:HHmm}", DateTime.Now.AddMinutes(2)));

            txtToTimeAddVideoPopup().Clear();
            Thread.Sleep(1000);
            txtToTimeAddVideoPopup().SendKeys(String.Format("{0:HHmm}", DateTime.Now.AddMinutes(30)));

            Thread.Sleep(1000);
            executor.ExecuteScript("arguments[0].click()", driver.FindElement((OR.GetElement("EventManagement", "EventSaveBTN", "TVAdminPortalOR.xml"))));
            // btnEventSave().Click();
            // Thread.Sleep(2000);
            VerifySuccessBannerMessage("Record saved sucessfully.");

            btnPublish().Click();

            VerifyDefaultSuccessBannerMessage("Event successfully published.");

            Thread.Sleep(150000);

            return newVideoEventDay;
        }

        public Dictionary<String, String> createSpeaker()
        {
            Dictionary<String, String> speakerDetails = new Dictionary<String, String>();

            speakerDetails.Add("title", "Mr");

            speakerDetails.Add("firstName", "Automation" + " " + uf.GenerateName(5));

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

            string sn = speakerDetails["firstName"];

            cf.writingIntoXML("Admin", "Video Management", "SpeakerName", speakerDetails["firstName"], "TestCopy.xml");

            //last name field
            speakerLastName().SendKeys(speakerDetails["lastName"]);

            //Add button
            speakerAddButton().Click();

            VerifySuccessBannerMessage("Speaker added successfully.");

            return speakerDetails;

        }

        public void verifySpeakerCreated()
        {
            log.Info("inside verifyExternalAccountCreated" + "at line" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

            Thread.Sleep(2000);

            createdSpeakerName = cf.readingXMLFile("Admin", "Video Management", "SpeakerName", "TestCopy.xml");

            recentSpeakerCreated(createdSpeakerName);

            retrieveSpeakerAccessCode();
        }

        public void recentSpeakerCreated(String createdSpeakerName)
        {
            IList<IWebElement> speakerList;

            log.Info("inside recentVideoVerification " + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

            iWait.Until(ExpectedConditions.ElementExists(OR.GetElement("VideoManagement", "RecentSpeakerListing", "TVAdminPortalOR.xml")));

            try
            {
                IWebElement tblSpeakerListing = driver.FindElement(OR.GetElement("VideoManagement", "RecentSpeakerListing", "TVAdminPortalOR.xml"));

                iWait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy(By.TagName("tr")));

                speakerList = (IList<IWebElement>)tblSpeakerListing.FindElements(By.TagName("tr"));
            }
            catch (Exception e)
            {
                log.Info("Row detection failed at first instance for My video table");

                Thread.Sleep(2000);  // Row detection failed at first instance thus retrying after wait

                IWebElement tblVideoListing = driver.FindElement((OR.GetElement("VideoManagement", "RecentSpeakerListing", "TVAdminPortalOR.xml")));

                iWait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy(By.TagName("tr")));

                speakerList = (IList<IWebElement>)tblVideoListing.FindElements(By.TagName("tr"));
            }

            uf.isJqueryActive(driver);

            int i = 0;

            foreach (IWebElement currentRow in speakerList)
            {
                if (currentRow.GetAttribute("class").Equals("GridRowStyle") || currentRow.GetAttribute("class").Equals("AltGridStyle"))
                {
                    string columnName = currentRow.FindElements(By.TagName("td"))[3].Text.ToString();

                    string speakerCreated = cf.readingXMLFile("Admin", "Video Management", "SpeakerName", "TestCopy.xml");

                    if (columnName.Equals(speakerCreated))
                    {
                        IWebElement speakerEditBtn = driver.FindElement(OR.GetElement("VideoManagement", "SpeakerEditBtn", "TVAdminPortalOR.xml",i));

                        speakerEditBtn.Click();

                        break;
                    }
                    i++;
                }

            }


        }

        public void retrieveSpeakerAccessCode()
        {
            uf.scrollDown(driver);

            Thread.Sleep(3000);

            speaker_AccessCode = speakerAccessCode().GetAttribute("value");

            cf.writingIntoXML("Admin", "Video Management", "SpeakerAccessCode",speaker_AccessCode, "TestCopy.xml");
        }

        #region existing code

        ///// <summary>
        ///// Create free event with,Event URL, default image, countdown, sponsor URL(sponsor not working) and verify the same
        ///// </summary>
        //[Test]
        //public void TVAdmin_001_CreateFreeEventWithDefImgAndCntdown()
        //{
        //    try
        //    {
        //        Console.WriteLine("TVAdmin_001_CreateFreeEvent test Started" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

        //        objAdminVideoManagement.redirectToVideoManagement();

        //        String initVideoName = "autoVid";
        //        String videoName = objAdminVideoManagement.basicInfoTab(initVideoName);

        //        SelectChannel("paid");

        //        objAdminVideoManagement.pricingListTab("free");

        //        objAdminVideoManagement.addcopyright();

        //        objAdminVideoManagement.uploadBrowseVideo();

        //        objAdminVideoManagement.finalPublishVideo("normal");

        //        string initEventName = "autoEvn";

        //        RedirectToEventManagement();

        //        btnAddNewEvent().Click();

        //        Dictionary<string, string> dictionary = BasicInformation(initEventName, DateTime.Now.AddDays(2.0), DateTime.Now.AddDays(3.0));

        //        Dictionary<string, string> locationDetails = Location();

        //        OverlayWait();

        //        btnSave().Click();

        //        VerifyDefaultSuccessBannerMessage("Event successfully added.");

        //        tabEventVideoAndTimetable().Click();

        //        tabvideoTab().Click();

        //        SearchVideo(videoName);

        //        SelectVideoFromSearchResult(videoName);

        //        addVideoDetails(locationDetails, DateTime.Now.AddDays(2.0));

        //        OverlayWait();

        //        EventAccessCodes();

        //        tabUpload().Click();

        //        ShowCountdown();

        //        rdbPromoDefaultImage().Click();

        //        // bug-    Dictionary<string, string> dictionarySponsor = SponsorDetails();

        //        EventPrice("free");

        //        btnSave().Click();

        //        VerifySuccessBannerMessage("Event successfully saved.");

        //        btnPublish().Click();

        //        VerifyDefaultSuccessBannerMessage("Event successfully published.");

        //        Thread.Sleep(60000);

        //        uf.OpenNewTab(driver);

        //        log.Info("count ::: " + driver.WindowHandles.Count);

        //        string browserType = uf.getRunningBrowser(driver, driver.FindElement(By.TagName("html")));

        //        uf.SwitchToWebTab(driver, browserType);

        //        uf.NavigateWebPortal(cf, driver);

        //        uf.scrollUp(driver);

        //        Thread.Sleep(4000);

        //        objWebEventManagement.handlePromotionalPopup();

        //        objWebEventManagement.HandleEmergencyPopUp();

        //        Thread.Sleep(5000);

        //        //uf.scrollUp(driver);

        //        objWebEventManagement.SearchEvent(dictionary["eventTitle"]);

        //        OverlayWait();

        //        objWebEventManagement.ClickAndVerifyEventName(dictionary["eventTitle"]);

        //        objWebEventManagement.VerifyCountDownDetails();

        //        objWebEventManagement.VerifyDefaultImage();

        //        objWebEventManagement.VerifyFreeEvent();

        //        uf.scrollDown(driver);

        //        objWebEventManagement.VerifyEventURL(eventURLContent);

        //        //bug -     objWebEventManagement.VerifySponsorDetails(dictionarySponsor);

        //        Console.WriteLine("TVAdmin_001_CreateFreeEvent test completed" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

        //    }
        //    catch (Exception e)
        //    {
        //        log.Error(e.Message + "\n" + e.StackTrace + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());
        //        Console.WriteLine(e.Message + "\n" + e.StackTrace + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());
        //        NUnit.Framework.Assert.AreEqual(true, false);
        //    }
        //}

        ///// <summary>
        ///// Create premium event with,Event URL, default image, countdown, sponsor URL(sponsor not working) and verify the same
        ///// </summary>
        //[Test]
        //public void TVAdmin_002_CreatePremiumEventWithDefImgAndCntdown()
        //{
        //    try
        //    {
        //        Console.WriteLine("TVAdmin_002_CreatePremiumEventWithDefImgAndCntdown test Started" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

        //        objAdminVideoManagement.redirectToVideoManagement();

        //        String initVideoName = "autoVid";
        //        String videoName = objAdminVideoManagement.basicInfoTab(initVideoName);

        //        SelectChannel("paid");

        //        objAdminVideoManagement.pricingListTab("free");

        //        objAdminVideoManagement.addcopyright();

        //        objAdminVideoManagement.uploadBrowseVideo();

        //        objAdminVideoManagement.finalPublishVideo("normal");

        //        string initEventName = "autoEvn";

        //        RedirectToEventManagement();

        //        btnAddNewEvent().Click();

        //        Dictionary<string, string> dictionary = BasicInformation(initEventName, DateTime.Now.AddDays(2.0), DateTime.Now.AddDays(3.0));

        //        Dictionary<string, string> locationDetails = Location();

        //        OverlayWait();

        //        btnSave().Click();

        //        VerifyDefaultSuccessBannerMessage("Event successfully added.");

        //        tabEventVideoAndTimetable().Click();

        //        tabvideoTab().Click();

        //        SearchVideo(videoName);

        //        SelectVideoFromSearchResult(videoName);

        //        addVideoDetails(locationDetails, DateTime.Now.AddDays(2.0));

        //        OverlayWait();

        //        EventAccessCodes();

        //        tabUpload().Click();

        //        ShowCountdown();

        //        rdbPromoDefaultImage().Click();

        //        EventPrice("premium");

        //        btnSave().Click();

        //        VerifySuccessBannerMessage("Event successfully saved.");

        //        btnPublish().Click();

        //        VerifyDefaultSuccessBannerMessage("Event successfully published.");

        //        Thread.Sleep(60000);

        //        uf.OpenNewTab(driver);

        //        log.Info("count ::: " + driver.WindowHandles.Count);

        //        string browserType = uf.getRunningBrowser(driver, driver.FindElement(By.TagName("html")));

        //        uf.SwitchToWebTab(driver, browserType);

        //        uf.NavigateWebPortal(cf, driver);

        //        uf.scrollUp(driver);

        //        Thread.Sleep(4000);

        //        objWebEventManagement.handlePromotionalPopup();

        //        objWebEventManagement.HandleEmergencyPopUp();

        //        Thread.Sleep(5000);

        //        objWebEventManagement.SearchEvent(dictionary["eventTitle"]);

        //        objWebEventManagement.ClickAndVerifyEventName(dictionary["eventTitle"]);

        //        objWebEventManagement.VerifyCountDownDetails();

        //        objWebEventManagement.VerifyDefaultImage();

        //        objWebEventManagement.VerifyPremiumEvent();

        //        objWebEventManagement.VerifyEventURL(eventURLContent);

        //        Console.WriteLine("TVAdmin_002_CreatePremiumEventWithDefImgAndCntdown test completed" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

        //    }
        //    catch (Exception e)
        //    {
        //        log.Error(e.Message + "\n" + e.StackTrace + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());
        //        Console.WriteLine(e.Message + "\n" + e.StackTrace + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());
        //        NUnit.Framework.Assert.AreEqual(true, false);
        //    }
        //}

        ///// <summary>
        ///// Create free event with promo image
        ///// </summary>
        //[Test]
        //public void TVAdmin_003_CreateFreeEventWithPromoImage()
        //{
        //    try
        //    {
        //        Console.WriteLine("TVAdmin_003_CreateFreeEventWithPromoImage test Started" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

        //        objAdminVideoManagement.redirectToVideoManagement();

        //        String initVideoName = "autoVid";
        //        String videoName = objAdminVideoManagement.basicInfoTab(initVideoName);

        //        SelectChannel("paid");

        //        objAdminVideoManagement.pricingListTab("free");

        //        objAdminVideoManagement.addcopyright();

        //        objAdminVideoManagement.uploadBrowseVideo();

        //        objAdminVideoManagement.finalPublishVideo("normal");

        //        string initEventName = "autoEvn";
        //        RedirectToEventManagement();
        //        btnAddNewEvent().Click();
        //        Dictionary<string, string> dictionary = BasicInformation(initEventName, DateTime.Now.AddDays(2.0), DateTime.Now.AddDays(3.0));
        //        Dictionary<string, string> locationDetails = Location();
        //        OverlayWait();
        //        btnSave().Click();
        //        VerifyDefaultSuccessBannerMessage("Event successfully added.");
        //        tabEventVideoAndTimetable().Click();
        //        tabvideoTab().Click();
        //        SearchVideo(videoName);
        //        SelectVideoFromSearchResult(videoName);
        //        addVideoDetails(locationDetails, DateTime.Now.AddDays(2.0));
        //        EventAccessCodes();
        //        tabUpload().Click();
        //        String promoImageName = PromoImage();
        //        EventPrice("free");
        //        btnSave().Click();
        //        VerifySuccessBannerMessage("Event successfully saved.");
        //        btnPublish().Click();
        //        VerifyDefaultSuccessBannerMessage("Event successfully published.");
        //        uf.OpenNewTab(driver);

        //        log.Info("count ::: " + driver.WindowHandles.Count);

        //        string browserType = uf.getRunningBrowser(driver, driver.FindElement(By.TagName("html")));

        //        uf.SwitchToWebTab(driver, browserType);

        //        uf.NavigateWebPortal(cf, driver);

        //        uf.scrollUp(driver);

        //        Thread.Sleep(4000);

        //        objWebEventManagement.handlePromotionalPopup();

        //        objWebEventManagement.HandleEmergencyPopUp();

        //        objWebEventManagement.SearchEvent(dictionary["eventTitle"]);

        //        uf.scrollUp(driver);

        //        Thread.Sleep(4000);

        //        objWebEventManagement.ClickAndVerifyEventName(dictionary["eventTitle"]);

        //        objWebEventManagement.VerifyPromoImage(promoImageName);

        //        objWebEventManagement.VerifyFreeEvent();

        //        Console.WriteLine("TVAdmin_003_CreateFreeEventWithPromoImage test completed" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

        //    }
        //    catch (Exception e)
        //    {
        //        log.Error(e.Message + "\n" + e.StackTrace + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());
        //        Console.WriteLine(e.Message + "\n" + e.StackTrace + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());
        //        NUnit.Framework.Assert.AreEqual(true, false);
        //    }
        //}

        ///// <summary>
        ///// Create premium event with promo image
        ///// </summary>
        //[Test]
        //public void TVAdmin_004_CreatePremiumEventWithPromoImage()
        //{
        //    try
        //    {
        //        Console.WriteLine("TVAdmin_004_CreatePremiumEventWithPromoImage test Started" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

        //        objAdminVideoManagement.redirectToVideoManagement();

        //        String initVideoName = "autoVid";
        //        String videoName = objAdminVideoManagement.basicInfoTab(initVideoName);

        //        SelectChannel("paid");

        //        objAdminVideoManagement.pricingListTab("free");

        //        objAdminVideoManagement.addcopyright();

        //        objAdminVideoManagement.uploadBrowseVideo();

        //        objAdminVideoManagement.finalPublishVideo("normal");

        //        string initEventName = "autoEvn";
        //        RedirectToEventManagement();
        //        btnAddNewEvent().Click();
        //        Dictionary<string, string> dictionary = BasicInformation(initEventName, DateTime.Now.AddDays(2.0), DateTime.Now.AddDays(3.0));
        //        Dictionary<string, string> locationDetails = Location();
        //        OverlayWait();
        //        btnSave().Click();
        //        VerifyDefaultSuccessBannerMessage("Event successfully added.");
        //        tabEventVideoAndTimetable().Click();
        //        tabvideoTab().Click();
        //        SearchVideo(videoName);
        //        SelectVideoFromSearchResult(videoName);
        //        addVideoDetails(locationDetails, DateTime.Now.AddDays(2.0));
        //        EventAccessCodes();
        //        tabUpload().Click();
        //        String promoImageName = PromoImage();
        //        EventPrice("premium");
        //        btnSave().Click();
        //        VerifySuccessBannerMessage("Event successfully saved.");
        //        btnPublish().Click();
        //        VerifyDefaultSuccessBannerMessage("Event successfully published.");
        //        uf.OpenNewTab(driver);

        //        log.Info("count ::: " + driver.WindowHandles.Count);

        //        string browserType = uf.getRunningBrowser(driver, driver.FindElement(By.TagName("html")));

        //        uf.SwitchToWebTab(driver, browserType);

        //        uf.NavigateWebPortal(cf, driver);

        //        uf.scrollUp(driver);

        //        Thread.Sleep(4000);

        //        objWebEventManagement.handlePromotionalPopup();

        //        objWebEventManagement.HandleEmergencyPopUp();

        //        Thread.Sleep(5000);

        //        objWebEventManagement.SearchEvent(dictionary["eventTitle"]);

        //        objWebEventManagement.ClickAndVerifyEventName(dictionary["eventTitle"]);

        //        objWebEventManagement.VerifyPromoImage(promoImageName);

        //        objWebEventManagement.VerifyPremiumEvent();


        //        Console.WriteLine("TVAdmin_004_CreatePremiumEventWithPromoImage test completed" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

        //    }
        //    catch (Exception e)
        //    {
        //        log.Error(e.Message + "\n" + e.StackTrace + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());
        //        Console.WriteLine(e.Message + "\n" + e.StackTrace + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());
        //        NUnit.Framework.Assert.AreEqual(true, false);
        //    }
        //}

        ///// <summary>
        ///// Create Premium event with logo image,and verify access code
        ///// Access code not working
        ///// </summary>
        //[Test]
        //public void TVAdmin_005_CreatePremiumEventWithAccessCodeAndLogoImg()
        //{
        //    try
        //    {
        //        Console.WriteLine("TVAdmin_005_CreatePremiumEventWithAccessCodeAndLogoImg test Started" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

        //        objAdminVideoManagement.redirectToVideoManagement();

        //        String initVideoName = "autoVid";
        //        String videoName = objAdminVideoManagement.basicInfoTab(initVideoName);

        //        SelectChannel("paid");

        //        objAdminVideoManagement.pricingListTab("Subscription");

        //        objAdminVideoManagement.addcopyright();

        //        objAdminVideoManagement.uploadBrowseVideo();

        //        objAdminVideoManagement.finalPublishVideo("normal");

        //        string initEventName = "autoEvn";
        //        RedirectToEventManagement();
        //        btnAddNewEvent().Click();
        //        Dictionary<string, string> dictionary = BasicInformation(initEventName, DateTime.Now, DateTime.Now.AddDays(2.0));
        //        Dictionary<string, string> locationDetails = Location();
        //        OverlayWait();
        //        btnSave().Click();
        //        VerifyDefaultSuccessBannerMessage("Event successfully added.");
        //        tabEventVideoAndTimetable().Click();
        //        tabvideoTab().Click();
        //        SearchVideo(videoName);
        //        SelectVideoFromSearchResult(videoName);
        //        String videoEventday = addVideoDetails(locationDetails, DateTime.Now);
        //        String eventAccessCodeContent = EventAccessCodes();
        //        tabUpload().Click();
        //        String eventLogoName = UploadEventLogo();
        //        EventPrice("premium");
        //        btnSave().Click();
        //        VerifySuccessBannerMessage("Event successfully saved.");
        //        btnPublish().Click();
        //        VerifyDefaultSuccessBannerMessage("Event successfully published.");
        //        uf.OpenNewTab(driver);

        //        log.Info("count ::: " + driver.WindowHandles.Count);

        //        string browserType = uf.getRunningBrowser(driver, driver.FindElement(By.TagName("html")));

        //        uf.SwitchToWebTab(driver, browserType);

        //        uf.NavigateWebPortal(cf, driver);

        //        uf.scrollUp(driver);

        //        Thread.Sleep(4000);

        //        objWebEventManagement.handlePromotionalPopup();

        //        objWebEventManagement.HandleEmergencyPopUp();

        //        Thread.Sleep(5000);

        //        objWebEventManagement.SearchEvent(dictionary["eventTitle"]);

        //        objWebEventManagement.ClickAndVerifyEventName(dictionary["eventTitle"]);

        //        objWebEventManagement.VerifyEventLogo(eventLogoName);

        //        objWebEventManagement.VerifyPremiumEvent();

        //        objWebEventManagement.SearchAndClickVideoSchedule(videoEventday);

        //        objWebEventManagement.SelectVideoFromVideoSheduleSection(videoName);

        //        // bug - objWebEventManagement.ClickAndVerifyEventAcessCode(eventAccessCodeContent);

        //        Console.WriteLine("TVAdmin_005_CreatePremiumEventWithAccessCodeAndLogoImg test completed" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

        //    }
        //    catch (Exception e)
        //    {
        //        log.Error(e.Message + "\n" + e.StackTrace + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());
        //        Console.WriteLine(e.Message + "\n" + e.StackTrace + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());
        //        NUnit.Framework.Assert.AreEqual(true, false);
        //    }
        //}

        ///// <summary>
        ///// 1.Create new live video
        ///// 2.Create event and attach above video to it
        ///// 3.Verify status of video on web portal - Set as reminder , LIVE, Comingsoon by changing video date and time attached to the event
        ///// 4.Coming soon not working
        ///// </summary>
        //[Test]
        //public void TVAdmin_006_VerfiEventLiveVideoStatus()
        //{
        //    try
        //    {
        //        Console.WriteLine("TVAdmin_006_VerfiEventLiveVideoStatus test Started" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

        //        //String videoName = "vid5d10d045953a4c039bd77195bcc8a873";
        //        String videoName = CreateFreeLiveVideo(DateTime.Now.AddMinutes(2).ToString("dd/MM/yyyy HHmm", CultureInfo.InvariantCulture));
        //        string initEventName = "autoEvn";
        //        RedirectToEventManagement();
        //        btnAddNewEvent().Click();
        //        Dictionary<string, string> dictionary = BasicInformation(initEventName, DateTime.Now, DateTime.Now.AddDays(3.0));
        //        Dictionary<string, string> locationDetails = Location();
        //        OverlayWait();
        //        btnSave().Click();
        //        VerifyDefaultSuccessBannerMessage("Event successfully added.");
        //        tabEventVideoAndTimetable().Click();
        //        tabvideoTab().Click();
        //        SearchVideo(videoName);
        //        SelectVideoFromSearchResult(videoName);
        //        String videoEventday = addVideoDetails(locationDetails, DateTime.Now.AddDays(1.0));
        //        EventPrice("free");
        //        btnSave().Click();
        //        VerifySuccessBannerMessage("Event successfully saved.");
        //        btnPublish().Click();
        //        VerifyDefaultSuccessBannerMessage("Event successfully published.");
        //        uf.OpenNewTab(driver);

        //        log.Info("count ::: " + driver.WindowHandles.Count);

        //        string browserType = uf.getRunningBrowser(driver, driver.FindElement(By.TagName("html")));

        //        uf.SwitchToWebTab(driver, browserType);

        //        uf.NavigateWebPortal(cf, driver);

        //        objWebEventManagement.HandleEmergencyPopUp();

        //        objWebEventManagement.SearchEvent(dictionary["eventTitle"]);



        //        objWebEventManagement.ClickAndVerifyEventName(dictionary["eventTitle"]);

        //        objWebEventManagement.SearchAndClickVideoSchedule(videoEventday);

        //        objWebEventManagement.GetEventVideoStatus(videoName, "Set as Reminder");

        //        browserType = uf.getRunningBrowser(driver, driver.FindElement(By.TagName("html")));

        //        uf.SwitchToAdminTab(driver, browserType);

        //        driver.FindElement((OR.GetElement("SeriesManagement", "SearchTextBox", "TVAdminPortalOR.xml"))).SendKeys(dictionary["eventTitle"]);

        //        driver.FindElement((OR.GetElement("SeriesManagement", "SearchButton", "TVAdminPortalOR.xml"))).Click();

        //        driver.FindElement((OR.GetElement("EventManagement", "ImageEdit", "TVAdminPortalOR.xml"))).Click();

        //        iWait.Until(ExpectedConditions.ElementIsVisible((OR.GetElement("EventManagement", "EventTitleTXT", "TVAdminPortalOR.xml"))));

        //        tabEventVideoAndTimetable().Click();

        //        driver.FindElement((OR.GetElement("EventManagement", "EventEdit", "TVAdminPortalOR.xml"))).Click();

        //        txtDateAddVideoPopup().Clear();
        //        //String.Format("{0:d dd ddd dddd}", dt)
        //        txtDateAddVideoPopup().SendKeys(String.Format("{0:dd/MM/yyyy}", DateTime.Now));


        //        String newVideoEventDay = String.Format("{0:dddd}", DateTime.Now);
        //        Console.WriteLine("day:::::::::: " + newVideoEventDay);

        //        txtFromTimeAddVideoPopup().Clear();
        //        Thread.Sleep(1000);
        //        txtFromTimeAddVideoPopup().SendKeys(String.Format("{0:HHmm}", DateTime.Now.AddMinutes(2)));

        //        txtToTimeAddVideoPopup().Clear();
        //        Thread.Sleep(1000);
        //        txtToTimeAddVideoPopup().SendKeys(String.Format("{0:HHmm}", DateTime.Now.AddMinutes(30)));
        //        executor.ExecuteScript("arguments[0].click()", driver.FindElement((OR.GetElement("EventManagement", "EventSaveBTN", "TVAdminPortalOR.xml"))));

        //        // btnEventSave().Click();

        //        VerifySuccessBannerMessage("Record saved sucessfully.");

        //        btnPublish().Click();

        //        VerifyDefaultSuccessBannerMessage("Event successfully published.");

        //        Thread.Sleep(150000);
        //        browserType = uf.getRunningBrowser(driver, driver.FindElement(By.TagName("html")));

        //        uf.SwitchToWebTab(driver, browserType);

        //        driver.Navigate().Refresh();

        //        uf.isJqueryActive(driver);

        //        objWebEventManagement.SearchAndClickVideoSchedule(newVideoEventDay);

        //        objWebEventManagement.GetEventVideoStatus(videoName, "LIVE");

        //        //commented as no video name is dispplayed - bug
        //        /* Thread.Sleep(150000);

        //         driver.Navigate().Refresh();

        //         uf.isJqueryActive(driver);

        //         objWebEventManagement.SearchAndClickVideoSchedule(newVideoEventDay);

        //       //  objWebEventManagement.GetEventVideoStatus(videoName, "Coming Soon");
        //         */

        //        Console.WriteLine("TVAdmin_006_VerfiEventLiveVideoStatus test Completed" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

        //    }
        //    catch (Exception e)
        //    {
        //        Console.WriteLine(e.Message + "\n" + e.StackTrace + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());
        //        log.Error(e.Message + "\n" + e.StackTrace + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());
        //        Assert.AreEqual(true, false);
        //    }

        //}

        ///// <summary>
        ///// 1.Create new publish video
        ///// 2.Create event and attach above video to it
        ///// 3.Verify status of video on web portal - Set as reminder ,on Demand by changing video date and time attached to the event
        ///// </summary>
        //[Test]
        //public void TVAdmin_007_VerfiEventPublishVideoStatus()
        //{
        //    try
        //    {
        //        Console.WriteLine("TVAdmin_007_VerfiEventPublishVideoStatus test Started" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

        //        objAdminVideoManagement.redirectToVideoManagement();

        //        String initVideoName = "autoVid";
        //        String videoName = objAdminVideoManagement.basicInfoTab(initVideoName);

        //        objAdminVideoManagement.channelListTab();

        //        objAdminVideoManagement.pricingListTab("Free");

        //        objAdminVideoManagement.addcopyright();

        //        objAdminVideoManagement.uploadBrowseVideo();

        //        objAdminVideoManagement.finalPublishVideo("normal");

        //        string initEventName = "autoEvn";
        //        RedirectToEventManagement();
        //        btnAddNewEvent().Click();
        //        Dictionary<string, string> dictionary = BasicInformation(initEventName, DateTime.Now, DateTime.Now.AddDays(3.0));
        //        Dictionary<string, string> locationDetails = Location();
        //        OverlayWait();
        //        btnSave().Click();
        //        VerifyDefaultSuccessBannerMessage("Event successfully added.");
        //        tabEventVideoAndTimetable().Click();
        //        tabvideoTab().Click();
        //        SearchVideo(videoName);
        //        SelectVideoFromSearchResult(videoName);
        //        String videoEventday = addVideoDetails(locationDetails, DateTime.Now.AddDays(1));
        //        EventPrice("free");
        //        btnSave().Click();
        //        VerifySuccessBannerMessage("Event successfully saved.");
        //        btnPublish().Click();
        //        VerifyDefaultSuccessBannerMessage("Event successfully published.");

        //        uf.OpenNewTab(driver);

        //        log.Info("count ::: " + driver.WindowHandles.Count);

        //        string browserType = uf.getRunningBrowser(driver, driver.FindElement(By.TagName("html")));

        //        uf.SwitchToWebTab(driver, browserType);

        //        uf.NavigateWebPortal(cf, driver);

        //        uf.scrollUp(driver);

        //        Thread.Sleep(4000);

        //        objWebEventManagement.handlePromotionalPopup();

        //        objWebEventManagement.HandleEmergencyPopUp();

        //        uf.scrollUp(driver);

        //        Thread.Sleep(4000);

        //        objWebEventManagement.SearchEvent(dictionary["eventTitle"]);

        //        objWebEventManagement.ClickAndVerifyEventName(dictionary["eventTitle"]);

        //        objWebEventManagement.SearchAndClickVideoSchedule(videoEventday);

        //        objWebEventManagement.GetEventVideoStatus(videoName, "Set as Reminder");

        //        //Verify on demand status
        //        browserType = uf.getRunningBrowser(driver, driver.FindElement(By.TagName("html")));

        //        uf.SwitchToAdminTab(driver, browserType);

        //        SearchEvent(dictionary);

        //        tabEventVideoAndTimetable().Click();

        //        driver.FindElement((OR.GetElement("EventManagement", "EventEdit", "TVAdminPortalOR.xml"))).Click();

        //        Thread.Sleep(5000);
        //        txtDateAddVideoPopup().Clear();
        //        //txtDateAddVideoPopup().SendKeys(String.Format("{0:dd/MM/yyyy}", CultureInfo.InvariantCulture));
        //        txtDateAddVideoPopup().SendKeys(DateTime.Now.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture));

        //        String newVideoEventDay = String.Format("{0:dddd}", DateTime.Now);
        //        Console.WriteLine("day:::::::::: " + newVideoEventDay);

        //        txtFromTimeAddVideoPopup().Clear();
        //        Thread.Sleep(1000);
        //        txtFromTimeAddVideoPopup().SendKeys(String.Format("{0:HHmm}", DateTime.Now.AddMinutes(2)));

        //        txtToTimeAddVideoPopup().Clear();
        //        Thread.Sleep(1000);
        //        txtToTimeAddVideoPopup().SendKeys(String.Format("{0:HHmm}", DateTime.Now.AddMinutes(30)));

        //        Thread.Sleep(1000);
        //        executor.ExecuteScript("arguments[0].click()", driver.FindElement((OR.GetElement("EventManagement", "EventSaveBTN", "TVAdminPortalOR.xml"))));
        //        // btnEventSave().Click();
        //        // Thread.Sleep(2000);
        //        VerifySuccessBannerMessage("Record saved sucessfully.");

        //        btnPublish().Click();

        //        VerifyDefaultSuccessBannerMessage("Event successfully published.");

        //        Thread.Sleep(150000);
        //        browserType = uf.getRunningBrowser(driver, driver.FindElement(By.TagName("html")));

        //        uf.SwitchToWebTab(driver, browserType);

        //        driver.Navigate().Refresh();

        //        uf.isJqueryActive(driver);

        //        objWebEventManagement.SearchAndClickVideoSchedule(newVideoEventDay);

        //        objWebEventManagement.GetEventVideoStatus(videoName, "ON DEMAND");

        //        Console.WriteLine("TVAdmin_007_VerfiEventPublishVideoStatus test Completed" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());
        //    }

        //    catch (Exception e)
        //    {
        //        Console.WriteLine(e.Message + "\n" + e.StackTrace + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());
        //        log.Error(e.Message + "\n" + e.StackTrace + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());
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
