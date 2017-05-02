using IETTVAdminportal.Reusable;
using IETTVWebportal.Chrome;
using log4net;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Utilities.Config;
using Utilities.Object_Repository;
using Utility_Classes;

namespace IETTVAdminPortal.Chrome
{
    [TestFixture]
    public class Chrome_ExternalAccessManagement
    {
        private static ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        #region variable declaration and object initialisation

        internal IWebDriver driver = null;

        IJavaScriptExecutor executor;

        IWait<IWebDriver> iWait = null;

        Utility_Functions uf = new Utility_Functions();

        string driverName = "", driverPath, appURL;

        Chrome_AdminSetupTearDown st = new Chrome_AdminSetupTearDown();

        Object_Repository_Class OR = new Object_Repository_Class();

        Configuration cf = new Configuration();

        Chrome_ExternalAccessVerification externalAccessResult;

        String guid_Admin, videoID_Admin;

        private Chrome_VideoManagement objAdminVideoManagement = null;

        public string videoName;

        String videoTitleUsedInRecentVideoSection = "";

        String abstractContent = "A paragraph from the Ancient Greek paragraphos.";

        Chrome_VideoManagementVerification videoResult;

      

        #endregion

        public Chrome_ExternalAccessManagement()
        {

        }

        public Chrome_ExternalAccessManagement(IWebDriver driver, ILog log1, IJavaScriptExecutor Executor, IWait<IWebDriver> iWait)
        {
            this.driver = driver;

            log = log1;

            this.executor = Executor;

            this.iWait = iWait;

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
            externalAccessResult = new Chrome_ExternalAccessVerification(driver, log, executor, iWait);            // Creating a object for calling IETTVWebPortal project

            objAdminVideoManagement = new Chrome_VideoManagement(driver, log, executor, iWait);

            videoResult = new Chrome_VideoManagementVerification(driver, log, executor, iWait);

            appURL = st.Chrome_Setup(driver, log, executor);                                               // Calling Chrome Setup  
        }

        #endregion

        #region Resuable Elements

        public IWebElement videoType()
        {
            iWait.Until(ExpectedConditions.ElementExists((OR.GetElement("VideoRequestByUser", "VideoTypeDDL", "TVAdminPortalOR.xml"))));

            return driver.FindElement((OR.GetElement("VideoRequestByUser", "VideoTypeDDL", "TVAdminPortalOR.xml")));
        }

        public IWebElement externalAccessTab()
        {
            iWait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy((OR.GetElement("ExternalAccessManagement", "ExternalAccessMenu", "TVAdminPortalOR.xml"))));

            return driver.FindElement((OR.GetElement("ExternalAccessManagement", "ExternalAccessMenu", "TVAdminPortalOR.xml")));
        }

        public IWebElement createAccessTab()
        {
            iWait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy((OR.GetElement("ExternalAccessManagement", "CreateAccessTab", "TVAdminPortalOR.xml"))));

            return driver.FindElement((OR.GetElement("ExternalAccessManagement", "CreateAccessTab", "TVAdminPortalOR.xml")));
        }

        public IWebElement accountFirstNameField()
        {
            iWait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy((OR.GetElement("ExternalAccessManagement", "FirstNameField", "TVAdminPortalOR.xml"))));

            return driver.FindElement((OR.GetElement("ExternalAccessManagement", "FirstNameField", "TVAdminPortalOR.xml")));
        }

        public IWebElement accountLastNameField()
        {
            iWait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy((OR.GetElement("ExternalAccessManagement", "LastNameField", "TVAdminPortalOR.xml"))));

            return driver.FindElement((OR.GetElement("ExternalAccessManagement", "LastNameField", "TVAdminPortalOR.xml")));
        }

        public IWebElement accountEmailAddressField()
        {
            iWait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy((OR.GetElement("ExternalAccessManagement", "EmailAddressField", "TVAdminPortalOR.xml"))));

            return driver.FindElement((OR.GetElement("ExternalAccessManagement", "EmailAddressField", "TVAdminPortalOR.xml")));
        }

        public IWebElement accountPhoneNumberField()
        {
            iWait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy((OR.GetElement("ExternalAccessManagement", "PhoneNumberField", "TVAdminPortalOR.xml"))));

            return driver.FindElement((OR.GetElement("ExternalAccessManagement", "PhoneNumberField", "TVAdminPortalOR.xml")));
        }

        public IWebElement accountSaveBtn()
        {
            iWait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy((OR.GetElement("ExternalAccessManagement", "ExternalSaveBtn", "TVAdminPortalOR.xml"))));

            return driver.FindElement((OR.GetElement("ExternalAccessManagement", "ExternalSaveBtn", "TVAdminPortalOR.xml")));
        }

        public IWebElement accountSearch()
        {
            iWait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy((OR.GetElement("ExternalAccessManagement", "ExternalAccountSearch", "TVAdminPortalOR.xml"))));

            return driver.FindElement((OR.GetElement("ExternalAccessManagement", "ExternalAccountSearch", "TVAdminPortalOR.xml")));
        }

        public IWebElement accountSearchBtn()
        {
            iWait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy((OR.GetElement("ExternalAccessManagement", "ExternalAccountSearchBtn", "TVAdminPortalOR.xml"))));

            return driver.FindElement((OR.GetElement("ExternalAccessManagement", "ExternalAccountSearchBtn", "TVAdminPortalOR.xml")));
        }

        public IWebElement externalAccountGrid()
        {
            iWait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy((OR.GetElement("ExternalAccessManagement", "RecentExternalAccessListing", "TVAdminPortalOR.xml"))));

            return driver.FindElement((OR.GetElement("ExternalAccessManagement", "RecentExternalAccessListing", "TVAdminPortalOR.xml")));
        }

        public IWebElement btnVideoSave()
        {
            iWait.Until(ExpectedConditions.ElementToBeClickable((OR.GetElement("EventManagement", "SaveBTN", "TVAdminPortalOR.xml"))));
            return driver.FindElement((OR.GetElement("EventManagement", "SaveBTN", "TVAdminPortalOR.xml")));
        }

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

        public IWebElement publishTab()
        {
            return driver.FindElement((OR.GetElement("VideoManagement", "PublishTab", "TVAdminPortalOR.xml")));
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

        public IWebElement videoTitleField()
        {
            iWait.Until(ExpectedConditions.ElementExists((OR.GetElement("VideoRequestByUser", "VideoTitle", "TVAdminPortalOR.xml"))));

            return driver.FindElement((OR.GetElement("VideoRequestByUser", "VideoTitle", "TVAdminPortalOR.xml")));
        }

        public IWebElement videoDescription()
        {
            iWait.Until(ExpectedConditions.ElementExists((OR.GetElement("VideoManagement", "ShortDesc", "TVAdminPortalOR.xml"))));

            return driver.FindElement((OR.GetElement("VideoManagement", "ShortDesc", "TVAdminPortalOR.xml")));
        }

        public IWebElement pricingTab()
        {
            return driver.FindElement((OR.GetElement("VideoManagement", "PricingTab", "TVAdminPortalOR.xml")));
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

        public IWebElement videoPrice()
        {
            iWait.Until(ExpectedConditions.ElementIsVisible((OR.GetElement("VideoManagement", "VideoPrice", "TVAdminPortalOR.xml"))));

            return driver.FindElement((OR.GetElement("VideoManagement", "VideoPrice", "TVAdminPortalOR.xml")));
        }

        #endregion

        #region Reusable function

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

        #endregion

        [Test]
        public void TVAdmin_001_ExternalAccessfunctionality()
        {
            log.Info("TVAdmin_001_ExternalAccessfunctionality test started" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

            externalAccessTab().Click();

            createAccessTab().Click();

            createExternalAccount();

            Thread.Sleep(3000);

            accountSaveBtn().Click();

            //click manage tab
            // driver.FindElement(By.Id("viewaccessDtls")).Click();

            externalAccessResult.verifySuccessBannerMessage("User created sucessfully.");

            verifyExternalAccountCreated();

            uf.OpenNewTab(driver);

            String browsertype = uf.getRunningBrowser(driver, driver.FindElement(By.TagName("html")));

            uf.SwitchToWebTab(driver, browsertype);

            //yopmail
            uf.NavigateYopMail(cf, driver);

            getPasswordFromMail();

            externalAccessResult.switchToExternalSite();

            externalAccessResult.enterExternalSiteCredentials();

            addExternalVideo();

            uf.SwitchToAdminTab(driver, browsertype);

            redirectToVideoManagement();

            videoName = cf.readingXMLFile("Admin", "External Management", "VideoName", "TestCopy.xml");
            recentExternalVideo(videoName);

            saveExternalVideo();

            uf.SwitchToExternalPortal(driver, browsertype);

            ////driver.FindElement(OR.GetElement("ExternalAccessManagement", "VideoSearch", "TVAdminPortalOR.xml")).SendKeys(videoName);

            ////driver.FindElement(OR.GetElement("ExternalAccessManagement", "VideoSearchbtn", "TVAdminPortalOR.xml")).Click();

            Thread.Sleep(4000);

            redirectToVideoManagement();

            recentExternalVideo(videoName);

            externalAccessResult.publishExternalVideo("normal");

            Thread.Sleep(150000);

            uf.OpenNewTab(driver);

            uf.SwitchToWebTab(driver, browsertype);

            uf.NavigateWebPortal(cf, driver);

            videoResult.searchVideoVerification(videoName, guid_Admin);

            videoResult.verifyVideoName(videoName);

            videoResult.verifyFreeVideo();

            log.Info("TVAdmin_001_ExternalAccessfunctionality test completed" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());
        }

        public void createExternalAccount()
        {
            log.Info("inside createExternalAccount" + "at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

            //enter value in first name field
            string accountName = "AutoFName" + uf.getShortGuid();
            accountFirstNameField().SendKeys(accountName);

            cf.writingIntoXML("Admin", "External Management", "accountname", accountName, "TestCopy.xml");

            //enter value in last name field
            string accountLastName = "Auto LName" + uf.getShortGuid();
            accountLastNameField().SendKeys(accountLastName);

            //enter value in Email address field
            string accountEmail = cf.readingXMLFile("Admin", "External Management", "accountname", "TestCopy.xml");

            string UserEmailAddress = accountEmail + "@yopmail.com";
            accountEmailAddressField().SendKeys(UserEmailAddress);

            cf.writingIntoXML("Admin", "External Management", "UserEmailAddress", UserEmailAddress, "TestCopy.xml");

            accountPhoneNumberField().SendKeys("1234567890");

        }

        public void verifyExternalAccountCreated()
        {
            log.Info("inside verifyExternalAccountCreated" + "at line" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

            Thread.Sleep(2000);

            accountSearch().SendKeys(cf.readingXMLFile("Admin", "External Management", "accountname", "TestCopy.xml"));
            //accountSearch().SendKeys("Madhura");

            accountSearchBtn().Click();

            recentExternalAccount();

        }

        public void recentExternalAccount()
        {
            log.Info("inside recentExternalAccount" + "at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

            IList<IWebElement> externalAccountList;

            iWait.Until(ExpectedConditions.ElementExists(OR.GetElement("ExternalAccessManagement", "RecentExternalAccessListing", "TVAdminPortalOR.xml")));

            try
            {
                Thread.Sleep(3000);
                IWebElement tblAccountListing = driver.FindElement((OR.GetElement("ExternalAccessManagement", "RecentExternalAccessListing", "TVAdminPortalOR.xml")));

                iWait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy(By.TagName("tr")));

                externalAccountList = (IList<IWebElement>)tblAccountListing.FindElements(By.TagName("tr"));
            }
            catch (Exception ex)
            {
                log.Info("Row detection failed at first instance for My video table");

                Thread.Sleep(2000);  // Row detection failed at first instance thus retrying after wait

                IWebElement tblAccountListing = driver.FindElement((OR.GetElement("ExternalAccessManagement", "RecentExternalAccessListing", "TVAdminPortalOR.xml")));

                iWait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy(By.TagName("tr")));

                externalAccountList = (IList<IWebElement>)tblAccountListing.FindElements(By.TagName("tr"));
            }

            int i = 0;

            foreach (IWebElement currentrow in externalAccountList)
            {
                if (currentrow.GetAttribute("class").Equals("GridRowStyle") || currentrow.GetAttribute("class").Equals("AltGridStyle"))
                {
                    string columnName = currentrow.FindElements(By.TagName("td"))[1].Text.ToString();

                    string firstNamecreated = cf.readingXMLFile("Admin", "External Management", "accountname", "TestCopy.xml");

                    //string firstNamecreated = "Madhura";

                    Assert.AreEqual(columnName, firstNamecreated);

                    break;
                }
            }
        }

        public void getPasswordFromMail()
        {
            log.Info("inside getPasswordFromMail " + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());
            #region getting password from mail

            iWait.Until(ExpectedConditions.ElementIsVisible(By.Id("login")));

            string accountEmailAddress = cf.readingXMLFile("Admin", "External Management", "UserEmailAddress", "TestCopy.xml");

            IWebElement YopUsername = driver.FindElement(By.Id("login"));
            YopUsername.SendKeys(accountEmailAddress);

            //YopUsername.SendKeys("AutoFName130595@yopmail.com");

            IWebElement submitBtn = driver.FindElement(By.CssSelector("input.sbut"));
            submitBtn.Click();

            //Thread.Sleep(150000);

            // IWait iWait = new IWait<IWebDriver>();

            WebDriverWait wait = new WebDriverWait(driver, new TimeSpan(0, 2, 30));
            wait.Until(ExpectedConditions.ElementIsVisible(By.Id("ifmail")));

            IWebElement mailFrame = driver.FindElement(By.Id("ifmail"));
            driver.SwitchTo().Frame(mailFrame);

            wait.Until(ExpectedConditions.ElementIsVisible(By.Id("mailmillieu")));
            IWebElement mailText = driver.FindElement(By.Id("mailmillieu"));

            string[] passwordText = mailText.Text.Split(':');

            string finalPassword = passwordText[3].Substring(1, 8);

            cf.writingIntoXML("Admin", "External Management", "Password", finalPassword, "TestCopy.xml");

            #endregion
        }

        public void addExternalVideo()
        {
            log.Info("inside addExternalVideo" + "at line :" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

            iWait.Until(ExpectedConditions.ElementToBeClickable((OR.GetElement("VideoManagement", "AddNewBTN", "TVAdminPortalOR.xml"))));

            driver.FindElement(OR.GetElement("VideoManagement", "AddNewBTN", "TVAdminPortalOR.xml")).Click();

            String initVidName = "AutoExternal";

            //String VideoName = objAdminVideoManagement.basicInfoTab(initVidName);

            //objAdminVideoManagement.basicInfoTab();

            basicInfoTab(initVidName);

            String adminSelectedChannel = objAdminVideoManagement.channelListTab();

            externalAccessResult.uploadBrowseVideo();

            btnVideoSave().Click();

            Thread.Sleep(2000);
            //iWait.Until(ExpectedConditions.ElementIsVisible((OR.GetElement("VideoRequestByUser", "Overlay", "TVWebPortalOR.xml"))));

            driver.FindElement((OR.GetElement("ChannelManagement", "OkButton", "TVAdminPortalOR.xml"))).Click();

        }

        public void basicInfoTab(String initVidName)
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
            videoName = initVidName + uf.getShortGuid();
            log.Info("Video name  : " + videoName);

            //Store the video details in sysconfig.xml file
            //cf.writingIntoXML("AdminPortal", "VideoManagement", "VideoName", videoName, "SysConfig.xml");
            cf.writingIntoXML("Admin", "External Management", "VideoName", videoName, "TestCopy.xml");

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

        public void redirectToVideoManagement()
        {

            log.Info("inside redirectToVideoManagement " + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

            //clicking on Admin dropdown   
            adminDropdown().Click();

            Thread.Sleep(3000);

            //Clicking on video Management Link

            videoManagementLink().Click();
        }

        public void recentExternalVideo(String videoName)
        {
            log.Info("inside recentExternalVideo" + "at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

            IList<IWebElement> videoRowList;

            log.Info("inside recentVideoVerification " + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

            //Wait for Add New Button
            iWait.Until(ExpectedConditions.ElementToBeClickable((OR.GetElement("VideoManagement", "AddNewBTN", "TVAdminPortalOR.xml"))));

            //click on my video check box
            //driver.FindElement((OR.GetElement("VideoManagement", "ChkMyVid", "TVAdminPortalOR.xml"))).Click();

            //seacrh recent created video
            driver.FindElement(OR.GetElement("ExternalAccessManagement", "VideoSearch", "TVAdminPortalOR.xml")).SendKeys(videoName);

            //clcik search button
            driver.FindElement(OR.GetElement("ExternalAccessManagement", "VideoSearchbtn", "TVAdminPortalOR.xml")).Click();

            //OverlayWait();
            Thread.Sleep(3000);
            iWait.Until(ExpectedConditions.ElementExists((OR.GetElement("ExternalAccessManagement", "RecentVideoSearch", "TVAdminPortalOR.xml"))));

            try
            {
                Thread.Sleep(3000);
                IWebElement tblVideoListing = driver.FindElement((OR.GetElement("ExternalAccessManagement", "RecentVideoSearch", "TVAdminPortalOR.xml")));

                iWait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy(By.TagName("tr")));

                videoRowList = (IList<IWebElement>)tblVideoListing.FindElements(By.TagName("tr"));
            }
            catch (Exception e)
            {
                log.Info("Row detection failed at first instance for My video table");

                Thread.Sleep(2000);  // Row detection failed at first instance thus retrying after wait

                IWebElement tblVideoListing = driver.FindElement((OR.GetElement("ExternalAccessManagement", "RecentVideoSearch", "TVAdminPortalOR.xml")));

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

                        IWebElement VideoEditButton = driver.FindElement(OR.GetElement("ExternalAccessManagement", "RecentVideoEdit", "TVAdminPortalOR.xml", i));
                        VideoEditButton.Click();
                        break;
                    }
                    i++;
                }
            }


        }

        public void saveExternalVideo()
        {
            log.Info("inside saveExternalVideo" + "at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

            iWait.Until(ExpectedConditions.ElementExists((OR.GetElement("VideoManagement", "VidNumberTXT", "TVAdminPortalOR.xml"))));

            pricingListTab("Free");

            objAdminVideoManagement.addcopyright();

            Thread.Sleep(3000);

            //click save button
            driver.FindElement(OR.GetElement("ExternalAccessManagement", "SaveVideobtn", "TVAdminPortalOR.xml")).Click();

            objAdminVideoManagement.verifySuccessBannerMessage("Record Saved Successfully.");
        }

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
        }

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
