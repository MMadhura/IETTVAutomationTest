using IETTVWebportal.Reusable_Functions;
using log4net;
using Microsoft.Expression.Encoder.ScreenCapture;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Utilities.Config;
using Utilities.Object_Repository;
using Utility_Classes;

namespace IETTVWebportal.Chrome
{
    public class Chrome_ExternalAccessVerification
    {
        private static ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        internal IWebDriver driver = null;

        IJavaScriptExecutor executor;

        IWait<IWebDriver> iWait;

        string driverName = "", driverPath, appURL;

        Utility_Functions uf = new Utility_Functions();                      // Instantiate object for Utility Functons

        Configuration cf = new Configuration();                             // Instantiate object for Configuration

        Object_Repository_Class OR = new Object_Repository_Class();        // Instantiate object for object repository

        ScreenCaptureJob job = null;

        Chrome_WebSetupTearDown st = new Chrome_WebSetupTearDown();

        IWebElement chooseFileButton, SuccessBannerMessage, OkButton;

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

        public Chrome_ExternalAccessVerification(IWebDriver driver, log4net.ILog log1, IJavaScriptExecutor Executor, IWait<IWebDriver> iWait)
        {
            this.driver = driver;

            log = log1;

            this.executor = Executor;

            this.iWait = iWait;

        }

        #region Reusable Elements

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

        public IWebElement externalPublishTab()
        {
            return driver.FindElement((OR.GetElement("ExternalAccessManagement", "ExternalPublishTab", "TVAdminPortalOR.xml")));
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

        #endregion

        public void ExternalAccountLogin()
        { 
        
        }

        public void switchToExternalSite()
        {
            log.Info("inside switchToExternalSite" +"at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

            uf.OpenNewTab(driver);

            String browsertype = uf.getRunningBrowser(driver, driver.FindElement(By.TagName("html")));

            uf.SwitchToWebTab(driver, browsertype);

            uf.NavigateExternalPortal(cf, driver);
        }

        public void enterExternalSiteCredentials()
        {
            log.Info("inside enterExternalSiteCredentials::");

            iWait.Until(ExpectedConditions.ElementExists(OR.GetElement("ExternalManagement","ExternalUsername","TVWebPortalOR.xml")));

            string username = cf.readingXMLFile("Admin", "External Management", "UserEmailAddress", "TestCopy.xml");

            driver.FindElement(OR.GetElement("ExternalManagement", "ExternalUsername", "TVWebPortalOR.xml")).SendKeys(username);

            //driver.FindElement(OR.GetElement("ExternalManagement", "ExternalUsername", "TVWebPortalOR.xml")).SendKeys("Madhura.Mungekar@northgateps.com");

            Thread.Sleep(2000);

            iWait.Until(ExpectedConditions.ElementExists(OR.GetElement("ExternalManagement", "ExternalPassword", "TVWebPortalOR.xml")));

            string password = cf.readingXMLFile("Admin", "External Management", "Password", "TestCopy.xml");

            driver.FindElement(OR.GetElement("ExternalManagement", "ExternalPassword", "TVWebPortalOR.xml")).SendKeys(password);

            //driver.FindElement(OR.GetElement("ExternalManagement", "ExternalPassword", "TVWebPortalOR.xml")).SendKeys("h6n5RDmk");

            iWait.Until(ExpectedConditions.ElementExists(OR.GetElement("ExternalManagement", "ExternalLognBtn", "TVWebPortalOR.xml")));

            driver.FindElement(OR.GetElement("ExternalManagement", "ExternalLognBtn", "TVWebPortalOR.xml")).Click();

        }

        public void verifySuccessBannerMessage(String message)
        {
            log.Info("inside verifySuccessBannerMessage" + "at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

            iWait.Until(ExpectedConditions.ElementIsVisible((OR.GetElement("SeriesManagement", "SuccessOkButton", "TVAdminPortalOR.xml"))));

            iWait.Until(d => d.FindElement((OR.GetElement("SeriesManagement", "SucessMessage", "TVAdminPortalOR.xml"))).Text.Equals(message));

            Assert.AreEqual(message, driver.FindElement((OR.GetElement("SeriesManagement", "SucessMessage", "TVAdminPortalOR.xml"))).Text.Trim());

            //Click on ok button banner message

            Thread.Sleep(2000);

            driver.FindElement((OR.GetElement("SeriesManagement", "SuccessOkButton", "TVAdminPortalOR.xml"))).Click();

            OverlayWait();

        }

        public void OverlayWait()
        {
            iWait.Until(ExpectedConditions.InvisibilityOfElementLocated((OR.GetElement("SeriesManagement", "OverlayWait", "TVAdminPortalOR.xml"))));
        }

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

                if (value.Equals("Upload"))
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

        public void publishExternalVideo(String videoType)
        {
            Thread.Sleep(2000);

            iWait.Until(ExpectedConditions.ElementToBeClickable(OR.GetElement("ExternalAccessManagement", "ExternalPublishTab", "TVAdminPortalOR.xml")));

            externalPublishTab().Click();

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

            //Verifying Success banner messages
            SuccessBannerMessage = driver.FindElement((OR.GetElement("VideoManagement", "DefaultSuccessLang", "TVAdminPortalOR.xml")));
            String Publish_Successful_Message = SuccessBannerMessage.Text;

            Assert.AreEqual("Final Video Published Successfully.", Publish_Successful_Message);

            //Click on ok button of banner message
            OkButton = driver.FindElement((OR.GetElement("VideoManagement", "SuccessBTN", "TVAdminPortalOR.xml")));
            executor.ExecuteScript("arguments[0].click();", OkButton);

            iWait.Until(ExpectedConditions.InvisibilityOfElementLocated((OR.GetElement("VideoManagement", "SuccessBTN", "TVAdminPortalOR.xml"))));
        }

        [TestFixtureTearDown]
        public void TearDown()
        {
            st.Chrome_TearDown(driver, log);                        // Calling Chrome Teardown
        }
       
    }
}
