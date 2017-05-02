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
//using IETTVAdminPortal.Chrome;
using Microsoft.Expression.Encoder.ScreenCapture;
using IETTVWebportal.Reusable_Functions;


namespace IETTVWebportal.Chrome
{
    [TestFixture]
    public class Chrome_SeriesManagementVerification
    {
        // This is to configure logger mechanism for Utilities.Config
        private readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        #region Variable Decration and object initialistaion

        internal IWebDriver driver = null;

        IWait<IWebDriver> iWait = null;

        string videoName;

        string driverName = "", driverPath, appURL;

        IJavaScriptExecutor executor = null;

        Utility_Functions uf = new Utility_Functions();                          // Instantiate object for Utility Functons

        Configuration cf = new Configuration();                                  // Instantiate object for Configuration                               

        Object_Repository_Class OR = new Object_Repository_Class();             // Instantiate object for object repository

        IETTVWebportal.Reusable_Functions.Chrome_WebSetupTearDown st = new IETTVWebportal.Reusable_Functions.Chrome_WebSetupTearDown();             // Instantiate object for Chrome Setup Teardown


        int videoEventSearchListCount = 0;
        #endregion

        #region constructor
        public Chrome_SeriesManagementVerification()
        {

        }

        public Chrome_SeriesManagementVerification(IWebDriver driver, ILog log1, IJavaScriptExecutor Executor, IWait<IWebDriver> iWait)
        {
            this.driver = driver;

            log = log1;

            this.executor = Executor;

            this.iWait = iWait;

        }
        #endregion

        #region SetUp

        [TestFixtureSetUp]
        public void FixtureSetUp()
        {
            log4net.Config.XmlConfigurator.Configure(new FileInfo(AppDomain.CurrentDomain.BaseDirectory + "WebPortal.config"));

            log.Info("Inside Fixture Setup of chrome - NonIETmemberRegistration Test" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

            //uf.CreateOrReplaceVideoFolder();

            string baseDir = AppDomain.CurrentDomain.BaseDirectory;                        // Get path till Base Directory

            driverName = "webdriver.chrome.driver";                                        // Driver name for Chrome

            driverPath = baseDir + "/chromedriver.exe";                                    // Path for ChromeDriver

            System.Environment.SetEnvironmentVariable(driverName, driverPath);

            driver = new ChromeDriver();                                                   // Initialize chrome driver   

            executor = (IJavaScriptExecutor)driver;

            iWait = new WebDriverWait(driver, TimeSpan.FromSeconds(60));

        }

        [SetUp]
        public void SetUp()
        {
            appURL = st.Chrome_Setup(driver, log, executor);                                // Calling Chrome Setup
        }

        #endregion

        #region Reusable Function

        //This function wait invisibility of overlay class
        public void OverlayWait()
        {
            iWait.Until(d => d.FindElement((OR.GetElement("SeriesManagement", "OverlayWait", "TVWebPortalOR.xml"))).GetAttribute("class").Contains("none"));
        }

        public void handlePromotionalPopup()
        {
            IWebElement promotionalPopup = driver.FindElement((OR.GetElement("SeriesManagement", "Promotionaldiv", "TVWebPortalOR.xml")));

            // iWait.Until(d => d.FindElement((OR.GetElement("SeriesManagement", "Promotionaldiv", "TVWebPortalOR.xml"))).GetCssValue("display").Equals("block"));

            String PromoPopup = promotionalPopup.GetCssValue("display").ToString();

             if (PromoPopup.Equals("block"))
            {
                driver.FindElement((OR.GetElement("SeriesManagement", "PromotionalPopup", "TVWebPortalOR.xml"))).Click();

                iWait.Until(ExpectedConditions.InvisibilityOfElementLocated(OR.GetElement("SeriesManagement", "Promotionaldiv", "TVWebPortalOR.xml")));
            }
        }

        public void handleEmergencyPopUp()
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

        // This function verify Sponsor image
        public void VerifySponsorDetails(Dictionary<String, String> adminSponsorDetails)
        {
            log.Info("verifySponsorDetails::::");

             iWait.Until(ExpectedConditions.ElementExists((OR.GetElement("SeriesManagement", "Sponserdiv", "TVWebPortalOR.xml"))));

            var elem = driver.FindElement((OR.GetElement("SeriesManagement", "Sponserdiv", "TVWebPortalOR.xml")));
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
            Console.WriteLine("urlaftersplit :: " + sponsorRedirectUrl.Split(Convert.ToChar(str))[1]);

            log.Info("urlaftersplit :: " + sponsorRedirectUrl.Split(Convert.ToChar(str))[1]);

            Assert.AreEqual("OK", uf.getStatusCode(new Uri(url)));

        }

        public void VerifySeriesLogo(string seriesLogoDetails)
        {
            //verify FreeSeriesLogo image
            log.Info("Inside VerifySeriesLogo :::::");

            String seriesLogoDownloadImageName = "actualSeriesImage.jpg";

            String imageLinkUrl = driver.FindElement((OR.GetElement("SeriesManagement", "SeriesLogo", "TVWebPortalOR.xml"))).GetAttribute("src");

            string downloadPath = Environment.CurrentDirectory + "\\Upload\\DownloadedImages\\";

            uf.download(imageLinkUrl, downloadPath, seriesLogoDownloadImageName);

            string seriesLogoImagePath = Environment.CurrentDirectory + "\\Upload\\Images\\" + seriesLogoDetails;

            Assert.AreEqual(true, uf.imageComparision(new Bitmap(downloadPath + seriesLogoDownloadImageName), new Bitmap(seriesLogoImagePath)));
        }

        #endregion

        public int TestSeriesManagementVerification(string seriesName,string vidName, string eventName)
        {
            try
            {
                log.Info("TestSeriesManagementVerification  started" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

                uf.isJqueryActive(driver);

                handlePromotionalPopup();

                handleEmergencyPopUp();

                #region Verify Series / Video

                iWait.Until(ExpectedConditions.ElementIsVisible((OR.GetElement("SeriesManagement", "SearchTB", "TVWebPortalOR.xml"))));

                iWait.Until(ExpectedConditions.ElementExists((OR.GetElement("SeriesManagement", "SearchTB", "TVWebPortalOR.xml"))));

                //search the required video
                IWebElement SearchTextField = driver.FindElement((OR.GetElement("SeriesManagement", "SearchTB", "TVWebPortalOR.xml")));
                SearchTextField.SendKeys(seriesName);

                iWait.Until(ExpectedConditions.ElementToBeClickable((OR.GetElement("VideoLandingPage", "SearchIcon", "TVWebPortalOR.xml"))));

                //Click on searchIcon
                IWebElement SearchIcon = driver.FindElement((OR.GetElement("VideoLandingPage", "SearchIcon", "TVWebPortalOR.xml")));

                SearchIcon.Click();

                uf.isJqueryActive(driver);

                Thread.Sleep(5000);
                //Waiting for Searched Series to be clickable.
                //iWait.Until(ExpectedConditions.ElementIsVisible((OR.GetElement("SeriesManagement", "SearchResult", "TVWebPortalOR.xml"))));

                //new added
                var elem = driver.FindElement((OR.GetElement("SeriesManagement", "SearchResultLink", "TVWebPortalOR.xml")));
                ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(true);", elem);

                IWebElement searchResultLink = driver.FindElement((OR.GetElement("SeriesManagement", "SearchResultLink", "TVWebPortalOR.xml")));

                searchResultLink.Click();

                //new added

                //Clicking on Searched Series name
                //driver.FindElement((OR.GetElement("SeriesManagement", "SearchedWord", "TVWebPortalOR.xml"))).Click();

                uf.isJqueryActive(driver);

                Thread.Sleep(5000);

                iWait.Until(ExpectedConditions.ElementIsVisible((OR.GetElement("SeriesManagement", "Video", "TVWebPortalOR.xml"))));

                //Verifying Series Name
                Assert.AreEqual(seriesName, driver.FindElement((OR.GetElement("SeriesManagement", "SeriesName", "TVWebPortalOR.xml"))).Text.Trim());

                #endregion

                //verifying the search result
                IList<IWebElement> videoEventSearchList = (IList<IWebElement>)driver.FindElement((OR.GetElement("VideoLandingPage", "SearchResult", "TVWebPortalOR.xml"))).FindElements(OR.GetElement("VideoLandingPage", "SearchResultRecord", "TVWebPortalOR.xml"));

                videoEventSearchListCount = videoEventSearchList.Count;

                //gettting the search result details
                foreach (IWebElement currentSearchrecord in videoEventSearchList)
                {
                    String videoEventTitle = currentSearchrecord.Text.Trim();

                    //getting video Title from search result
                    if (videoEventTitle.Equals(vidName))
                    {
                        Assert.AreEqual(vidName, videoEventTitle);
                    }

                    //Test for Event Name
                    if (videoEventTitle.Equals(eventName))
                    {
                        Assert.AreEqual(eventName, videoEventTitle);
                    }

                }

                Thread.Sleep(2000);
                return videoEventSearchListCount;

            }

            catch (Exception e)
            {
                log.Error(e.Message + "\n" + e.StackTrace + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());
                Console.WriteLine(e.Message + "\n" + e.StackTrace + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());
                Assert.AreEqual(true, false);
                return videoEventSearchListCount;
            }
        }

        [TestFixtureTearDown]
        public void TearDown()
        {
            //st.Chrome_TearDown(driver, log);                        // Calling Chrome Teardown
        }

    }

}
