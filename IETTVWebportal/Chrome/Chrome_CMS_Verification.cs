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
    public class Chrome_CMS_Verification
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

        public Chrome_CMS_Verification() { }

        public Chrome_CMS_Verification(IWebDriver driver, ILog log1, IJavaScriptExecutor Executor, IWait<IWebDriver> iWait)
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

        public void RedirectToHelpInstitution()
        {
            uf.isJqueryActive(driver);
            iWait.Until(ExpectedConditions.ElementIsVisible((OR.GetElement("Chrome_CMS", "HelpDestopButton", "TVWebPortalOR.xml"))));

            //Click on Help dropdown
            driver.FindElement((OR.GetElement("Chrome_CMS", "HelpDestopButton", "TVWebPortalOR.xml"))).Click();

            //  Thread.Sleep(2000);
            //click on Institution under IETTv help
            iWait.Until(ExpectedConditions.ElementIsVisible((OR.GetElement("Chrome_CMS", "ItemContainer", "TVWebPortalOR.xml"))));
            IList<IWebElement> help = driver.FindElement((OR.GetElement("Chrome_CMS", "ItemContainer", "TVWebPortalOR.xml"))).FindElements(OR.GetElement("Chrome_CMS", "InstitutionHelp", "TVWebPortalOR.xml"));
            executor.ExecuteScript("arguments[0].click()", help[0].FindElement(By.TagName("a")));
            Console.WriteLine("Clicked on Institution");
        }

        public void RedirectToServices_Filming()
        {
            uf.isJqueryActive(driver);

            iWait.Until(ExpectedConditions.ElementIsVisible((OR.GetElement("Chrome_CMS", "ServiceDD", "TVWebPortalOR.xml"))));

            //Click on SERVICES link
            driver.FindElement((OR.GetElement("Chrome_CMS", "ServiceDD", "TVWebPortalOR.xml"))).Click();

            // click on 'Filming' section under Services
            iWait.Until(ExpectedConditions.ElementIsVisible((OR.GetElement("Chrome_CMS", "ExpectedNoOfLinks", "TVWebPortalOR.xml"))));
            IList<IWebElement> servicesContainer = driver.FindElements((OR.GetElement("Chrome_CMS", "ExpectedNoOfLinks", "TVWebPortalOR.xml")));

            servicesContainer[0].FindElement(By.TagName("a")).Click();
            log.Info("Clicked on Filming");
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

        public void VerifyTitle(String expectedTitle)
        {
            //get the Title of the Institution
            iWait.Until(ExpectedConditions.ElementIsVisible(OR.GetElement("Chrome_CMS", "DivTitle", "TVWebPortalOR.xml")));
            String institustionTitleContent = driver.FindElement(OR.GetElement("Chrome_CMS", "InstitutionTitle2", "TVWebPortalOR.xml")).Text;
            Console.WriteLine("ActualTitleContent::: " + institustionTitleContent);
            Assert.AreEqual(expectedTitle, institustionTitleContent);
        }

        public void VerifyDescription(String expectedDescription)
        {
            log.Info("verified the HELP arrow and its presence and clicked on it");

            //get the Description of the Institution

            String institustionTitleContent = driver.FindElement(OR.GetElement("Chrome_CMS", "InstitutionDesc", "TVWebPortalOR.xml")).Text;
            Console.WriteLine("ActualDescriptionContent::: " + institustionTitleContent);
            Assert.AreEqual(expectedDescription, institustionTitleContent);
        }

        public void VerifyFirstImage(String expectedImageName , String expectedImageURL,String expectedAltTextForImage)
        {
            Console.WriteLine("expectedImageName : " + expectedImageName);

            //get the Image of the Institution
            uf.isJqueryActive(driver);
            String institutionFirstImage = driver.FindElement((OR.GetElement("Chrome_CMS", "ImageIcon", "TVWebPortalOR.xml"))).GetAttribute("src");

            Console.WriteLine("Inside VerifyHelpFirstImage :::::" + institutionFirstImage);

            string downloadPath = Environment.CurrentDirectory + "\\Upload\\DownloadedImages\\";

            uf.download(institutionFirstImage, downloadPath,"cmsActualImage.jpg");

            string cmsImagePath = Environment.CurrentDirectory + "\\Upload\\Images\\CMS\\" + expectedImageName;

            Assert.AreEqual(true, uf.imageComparision(new Bitmap(downloadPath + "cmsActualImage.jpg"), new Bitmap(cmsImagePath)));

            // Checking image URL
            Console.WriteLine("Actual Image URl  :" + driver.FindElement((OR.GetElement("Chrome_CMS", "ImageUrl", "TVWebPortalOR.xml"))).GetAttribute("href"));

            Assert.AreEqual(expectedImageURL, driver.FindElement((OR.GetElement("Chrome_CMS", "ImageUrl", "TVWebPortalOR.xml"))).GetAttribute("href"));

            //checking image url status
            Console.WriteLine("verifying  image url status:::::");

            string imageRedirectUrl = driver.FindElement((OR.GetElement("Chrome_CMS", "ImageUrl", "TVWebPortalOR.xml"))).GetAttribute("href");

            Assert.AreEqual("OK", uf.getStatusCode(new Uri(imageRedirectUrl)));

            // Checking Alt Text 
            Assert.AreEqual(expectedAltTextForImage, driver.FindElement((OR.GetElement("Chrome_CMS", "ImageIcon", "TVWebPortalOR.xml"))).GetAttribute("alt"));


            Console.WriteLine("image url status completed");

        }

        public void VerifySecondImage(String expectedImageName, String expectedImageURL, String expectedAltTextForImage)
        {
            String institutionSecondImage = driver.FindElement((OR.GetElement("Chrome_CMS", "ImageIcon2", "TVWebPortalOR.xml"))).GetAttribute("src");

            Console.WriteLine("Inside VerifyHelpFirstImage :::::");
            string downloadPath = Environment.CurrentDirectory + "\\Upload\\DownloadedImages\\";

            uf.download(institutionSecondImage, downloadPath, "cmsActualImage2.jpg");

            string cmsImagePath = Environment.CurrentDirectory + "\\Upload\\Images\\CMS\\" + expectedImageName;

            Assert.AreEqual(true, uf.imageComparision(new Bitmap(downloadPath + "cmsActualImage2.jpg"), new Bitmap(cmsImagePath)));

            // Checking URL
            Assert.AreEqual(expectedImageURL, driver.FindElement((OR.GetElement("Chrome_CMS", "ImageUrl2", "TVWebPortalOR.xml"))).GetAttribute("href"));


            //checking image url status
            Console.WriteLine("verifying  image url status:::::");

            string imageRedirectUrl =driver.FindElement((OR.GetElement("Chrome_CMS", "ImageUrl2", "TVWebPortalOR.xml"))).GetAttribute("href");

            Assert.AreEqual("OK", uf.getStatusCode(new Uri(imageRedirectUrl)));

            // Checking Alt text
            Assert.AreEqual(expectedAltTextForImage, driver.FindElement((OR.GetElement("Chrome_CMS", "ImageIcon2", "TVWebPortalOR.xml"))).GetAttribute("alt"));

            log.Info("imageTest  media 2 completed");
        }

        public void VerifyVideos()
        {
            //Media 1 verification
            int isVideoAdded = driver.FindElement((OR.GetElement("Chrome_CMS", "VideoPlayer1", "TVWebPortalOR.xml"))).FindElements(By.TagName("iframe")).Count;
            Assert.AreEqual(1, isVideoAdded);
            log.Info("Media 1 verification DONE");

            //Media 2 verification
            isVideoAdded = driver.FindElement((OR.GetElement("Chrome_CMS", "VideoPlayer2", "TVWebPortalOR.xml"))).FindElements(By.TagName("iframe")).Count;
            Assert.AreEqual(1, isVideoAdded);
            log.Info("Media 2 verification DONE");
        }



        [TestFixtureTearDown]
        public void TearDown()
        {
            st.Chrome_TearDown(driver, log);                        // Calling Chrome Teardown
        }


    }
}




