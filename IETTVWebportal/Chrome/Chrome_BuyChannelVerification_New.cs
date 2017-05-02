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
    public class Chrome_BuyChannelVerification_New
    {
        #region Variable Decration and object initialistaion

        // This is to configure logger mechanism for Utilities.Config
        private readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        internal IWebDriver driver = null;

        IWait<IWebDriver> iWait = null;

        string driverName = "", driverPath, appURL;

        IJavaScriptExecutor executor = null;

        Utility_Functions uf = new Utility_Functions();                          // Instantiate object for Utility Functons

        Configuration cf = new Configuration();                                  // Instantiate object for Configuration                               

        Object_Repository_Class OR = new Object_Repository_Class();             // Instantiate object for object repository

        IETTVWebportal.Reusable_Functions.Chrome_WebSetupTearDown st = new IETTVWebportal.Reusable_Functions.Chrome_WebSetupTearDown();             // Instantiate object for Chrome Setup Teardown

        #endregion

        #region Constructors
        public Chrome_BuyChannelVerification_New()
        {

        }

        public Chrome_BuyChannelVerification_New(IWebDriver driver, ILog log1, IJavaScriptExecutor Executor, IWait<IWebDriver> iWait)
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
            iWait.Until(d => d.FindElement((OR.GetElement("SeriesManagement", "OverlayWait", "TVWebPortalOR.xml"))).GetAttribute("class").Contains("display-block"));
        }

        public void RedirectToVideoLandingPage()
        {
            uf.isJqueryActive(driver);

            //iWait.Until(ExpectedConditions.ElementExists(By.Id("searchtextbox")));
            iWait.Until(ExpectedConditions.ElementExists((OR.GetElement("VideoLandingPage", "SearchTB", "TVWebPortalOR.xml"))));

            //search the required video

            //IWebElement SearchTextField = driver.FindElement(By.Id("searchtextbox"));
            IWebElement SearchTextField = driver.FindElement((OR.GetElement("VideoLandingPage", "SearchTB", "TVWebPortalOR.xml")));

            // Get the Video management node list   

            List<String> videoname = cf.readSysConfigFile("AdminPortal", "VideoManagement", "SysConfig.xml");

            log.Info("videoname    " + videoname.ElementAt(0).ToString());

            SearchTextField.SendKeys(videoname.ElementAt(0).ToString());

            iWait.Until(ExpectedConditions.ElementToBeClickable((OR.GetElement("VideoLandingPage", "SearchIcon", "TVWebPortalOR.xml"))));

            //Click on searchIcon

            //IWebElement SearchIcon = driver.FindElement(By.Id("searchicon"));
            IWebElement SearchIcon = driver.FindElement((OR.GetElement("VideoLandingPage", "SearchIcon", "TVWebPortalOR.xml")));

            SearchIcon.Click();

            uf.isJqueryActive(driver);

            IWebElement searchresultDetails = driver.FindElement(OR.GetElement("VideoLandingPage", "SearchResultDetails", "TVWebPortalOR.xml"));
            //IWebElement searchresultDetails = driver.FindElement((OR.GetElement("VideoLandingPage", "SearchResultDetails", "TVWebPortalOR.xml")));

            //IWebElement serachResultLink = driver.FindElement(By.CssSelector("div.margin-left-148.margin-right.video-description-clear-both > span > a"));
            IWebElement serachResultLink = driver.FindElement((OR.GetElement("VideoLandingPage", "SearchResultLink", "TVWebPortalOR.xml")));

            String webvideoTitle = searchresultDetails.Text.Trim();

            log.Info("Web Video Title:" + webvideoTitle + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

            serachResultLink.Click();

            uf.isJqueryActive(driver);
        }

        public void RedirectToLogin()
        {
            uf.isJqueryActive(driver);

            iWait.Until(ExpectedConditions.ElementExists((OR.GetElement("NonIETMemberRegistration", "LoginLink", "TVWebPortalOR.xml"))));

            IWebElement loginLink = driver.FindElement((OR.GetElement("NonIETMemberRegistration", "LoginLink", "TVWebPortalOR.xml")));

            executor.ExecuteScript("arguments[0].click();", loginLink);

            string userName = cf.readingXMLFile("WebPortal", "IndividualUser", "IndividualUserName", "Config.xml");
            string userPassword = cf.readingXMLFile("WebPortal", "IndividualUser", "IndividualPassword", "Config.xml");

            log.Info("username    " + userName);

            log.Info("Password    " + userPassword);

            iWait.Until(ExpectedConditions.ElementIsVisible(OR.GetElement("VideoRequestByUser", "UserNameTB", "TVWebPortalOR.xml")));

            driver.FindElement(OR.GetElement("VideoRequestByUser", "UserName", "TVWebPortalOR.xml")).SendKeys(userName);
            Thread.Sleep(1000);

            driver.FindElement(OR.GetElement("VideoRequestByUser", "Password", "TVWebPortalOR.xml")).SendKeys(userPassword);

            iWait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy(OR.GetElement("VideoRequestByUser", "Login", "TVWebPortalOR.xml")));

            driver.FindElement(OR.GetElement("VideoRequestByUser", "Login", "TVWebPortalOR.xml")).Click();

            //need to comment below region to run at UAT
            #region relogin process
            Thread.Sleep(10000);

            iWait.Until(ExpectedConditions.ElementIsVisible(OR.GetElement("VideoRequestByUser", "UserNameTB", "TVWebPortalOR.xml")));

            driver.FindElement(OR.GetElement("VideoRequestByUser", "UserName", "TVWebPortalOR.xml")).SendKeys(userName);
            Thread.Sleep(1000);

            driver.FindElement(OR.GetElement("VideoRequestByUser", "Password", "TVWebPortalOR.xml")).SendKeys(userPassword);


            iWait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy(OR.GetElement("VideoRequestByUser", "Login", "TVWebPortalOR.xml")));

            driver.FindElement(OR.GetElement("VideoRequestByUser", "Login", "TVWebPortalOR.xml")).Click();
            #endregion

            Thread.Sleep(10000);

            log.Info("already logged in count  : " + driver.FindElements(OR.GetElement("VideoRequestByUser", "YesButton", "TVWebPortalOR.xml")).Count);
            //if user is already logged in
            if (driver.FindElements(OR.GetElement("VideoRequestByUser", "YesButton", "TVWebPortalOR.xml")).Count > 0)
            {
                driver.FindElement(OR.GetElement("VideoRequestByUser", "YesButton", "TVWebPortalOR.xml")).Click();
            }


            //Handling pop up message
            iWait.Until(ExpectedConditions.ElementIsVisible(OR.GetElement("ReportAbuse", "PopupMessage", "TVWebPortalOR.xml")));  // Waiting for Popup window to appear after clicking on accept button

            IList<IWebElement> btnOK = driver.FindElements(OR.GetElement("ReportAbuse", "PopupMessage", "TVWebPortalOR.xml"));

            IWebElement element = btnOK.ElementAt(0);

            executor.ExecuteScript("arguments[0].click();", element);


        }

        #endregion

        #region Buy Channel Testing
        public void VerifyBuyChannel(string videoname)
        {
            try
            {
                log.Info("VerifyBuyVideo::::");

                uf.isJqueryActive(driver);

                RedirectToLogin();

                Console.Write("user successfully logged in");

                log.Info("searchPremiumVideo::::");

                Boolean flag = false;

                //wait till jquery gets completed
                uf.isJqueryActive(driver);

                appURL = cf.readingXMLFile("WebPortal", "BuyVidedoLogin", "startURLForBuyVideoAndChannel", "Config.xml");

                driver.Navigate().GoToUrl(appURL);

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

                //verifying the search result                
                IList<IWebElement> videoSearchList = (IList<IWebElement>)driver.FindElement((OR.GetElement("BuyVideoVerification", "SearchResult", "TVWebPortalOR.xml"))).FindElements((OR.GetElement("VideoLandingPage", "SearchResultRecord", "TVWebPortalOR.xml")));


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

                        IWebElement serachResultLink = driver.FindElement((OR.GetElement("VideoLandingPage", "SearchResultLink", "TVWebPortalOR.xml")));

                        log.Info("Web Video Title:" + webvideoTitle + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

                        serachResultLink.Click();

                        uf.isJqueryActive(driver);

                        break;
                    }
                }

                log.Info("Checking wheather buy video button is enabled.");
                Assert.AreEqual(true, driver.FindElement((OR.GetElement("BuyVideoVerification", "PremiumVideoBTN", "TVWebPortalOR.xml"))).Enabled);

                driver.FindElement((OR.GetElement("BuyVideoVerification", "PremiumVideoBTN", "TVWebPortalOR.xml"))).Click();

                Thread.Sleep(10000);

                //Handling pop up message
                iWait.Until(ExpectedConditions.ElementIsVisible((OR.GetElement("BuyChannelVerification", "ChannelBTN", "TVWebPortalOR.xml"))));  // Waiting for Popup window to appear after clicking on buy video

                driver.FindElement((OR.GetElement("BuyChannelVerification", "ChannelBTN", "TVWebPortalOR.xml"))).Click();

                uf.scrollDown(driver);

                driver.FindElement((OR.GetElement("BuyChannelVerification", "CheckoutBTN", "TVWebPortalOR.xml"))).Click();

                uf.isJqueryActive(driver);

                iWait.Until(ExpectedConditions.InvisibilityOfElementLocated((OR.GetElement("SeriesManagement", "OverlayWait", "TVWebPortalOR.xml"))));

                iWait.Until(ExpectedConditions.ElementIsVisible((OR.GetElement("BuyVideoVerification", "AddNewBTN", "TVWebPortalOR.xml"))));

                driver.FindElement((OR.GetElement("BuyVideoVerification", "AddNewBTN", "TVWebPortalOR.xml"))).Click();

                driver.FindElement((OR.GetElement("BuyVideoVerification", "PostalCode", "TVWebPortalOR.xml"))).SendKeys(cf.readingXMLFile("WebPortal", "BuyChannel", "PostalCode", "Config.xml"));

                IWebElement element = driver.FindElement((OR.GetElement("NonIETMemberRegistration", "Country", "TVWebPortalOR.xml")));
                SelectElement countrySelector = new SelectElement(element);
                countrySelector.SelectByText(cf.readingXMLFile("WebPortal", "BuyChannel", "Country", "Config.xml"));

                IWebElement element1 = driver.FindElement((OR.GetElement("BuyVideoVerification", "AddressType", "TVWebPortalOR.xml")));
                SelectElement addTypeSelector = new SelectElement(element1);
                addTypeSelector.SelectByText(cf.readingXMLFile("WebPortal", "BuyChannel", "BillingAddressType", "Config.xml"));

                driver.FindElement((OR.GetElement("BuyVideoVerification", "AddressTB", "TVWebPortalOR.xml"))).SendKeys(cf.readingXMLFile("WebPortal", "BuyChannel", "Address", "Config.xml"));
                driver.FindElement((OR.GetElement("BuyVideoVerification", "City", "TVWebPortalOR.xml"))).SendKeys(cf.readingXMLFile("WebPortal", "BuyChannel", "City", "Config.xml"));

                iWait.Until(ExpectedConditions.ElementIsVisible((OR.GetElement("BuyVideoVerification", "AddBTN", "TVWebPortalOR.xml"))));

                IWebElement saveButton = driver.FindElement((OR.GetElement("BuyVideoVerification", "AddBTN", "TVWebPortalOR.xml")));
                executor.ExecuteScript("arguments[0].click()", saveButton);

                Thread.Sleep(3000);

                if (driver.FindElement((OR.GetElement("VideoLandingPage", "InfoMessage", "TVWebPortalOR.xml"))).Displayed)
                    driver.FindElement((OR.GetElement("VideoLandingPage", "InfoMessage", "TVWebPortalOR.xml"))).Click();
                else
                    driver.FindElement((OR.GetElement("VideoRequestByUser", "BannerMessageOkButton", "TVWebPortalOR.xml"))).Click();

                uf.isJqueryActive(driver);

                iWait.Until(ExpectedConditions.InvisibilityOfElementLocated((OR.GetElement("SeriesManagement", "OverlayWait", "TVWebPortalOR.xml"))));

                driver.FindElement((OR.GetElement("BuyVideoVerification", "ContinueBTN", "TVWebPortalOR.xml"))).Click();

                iWait.Until(ExpectedConditions.ElementIsVisible((OR.GetElement("BuyVideoVerification", "DivDeclaration", "TVWebPortalOR.xml"))));

                IWebElement element2 = driver.FindElement((OR.GetElement("BuyVideoVerification", "CountryDeclaration", "TVWebPortalOR.xml")));
                SelectElement countrySelector1 = new SelectElement(element2);
                countrySelector1.SelectByText(cf.readingXMLFile("WebPortal", "BuyChannel", "CountryForVATCal", "Config.xml"));


                driver.FindElement((OR.GetElement("BuyVideoVerification", "ConfirmDeclarationBTN", "TVWebPortalOR.xml"))).Click();

                uf.isJqueryActive(driver);
                iWait.Until(ExpectedConditions.ElementIsVisible((OR.GetElement("BuyVideoVerification", "ProceedBTN", "TVWebPortalOR.xml"))));

                //Proceed Button Click
                driver.FindElement((OR.GetElement("BuyVideoVerification", "ProceedBTN", "TVWebPortalOR.xml"))).Click();

                //Credit Card Details

                iWait.Until(ExpectedConditions.ElementIsVisible(OR.GetElement("BuyChannelVerification", "CardNumber", "TVWebPortalOR.xml")));
                driver.FindElement(OR.GetElement("BuyChannelVerification", "CardNumber", "TVWebPortalOR.xml")).SendKeys(cf.readingXMLFile("WebPortal", "BuyChannel", "CardNumber", "Config.xml"));

                driver.FindElement(OR.GetElement("BuyChannelVerification", "NbxCV2", "TVWebPortalOR.xml")).SendKeys(cf.readingXMLFile("WebPortal", "BuyChannel", "CVNumber", "Config.xml"));
                driver.FindElement(OR.GetElement("BuyChannelVerification", "NbxIssueNumber", "TVWebPortalOR.xml")).SendKeys(cf.readingXMLFile("WebPortal", "BuyChannel", "IssueNumber", "Config.xml"));

                IWebElement element3 = driver.FindElement(OR.GetElement("BuyChannelVerification", "NbxMonthStart", "TVWebPortalOR.xml"));
                SelectElement monthStart = new SelectElement(element3);
                monthStart.SelectByText(cf.readingXMLFile("WebPortal", "BuyChannel", "MonthStart", "Config.xml"));

                IWebElement element4 = driver.FindElement(OR.GetElement("BuyChannelVerification", "NbxYearStart", "TVWebPortalOR.xml"));
                SelectElement yrStar = new SelectElement(element4);
                yrStar.SelectByText(cf.readingXMLFile("WebPortal", "BuyChannel", "YearStart", "Config.xml"));


                IWebElement element5 = driver.FindElement(OR.GetElement("BuyChannelVerification", "Expiry1", "TVWebPortalOR.xml"));
                SelectElement expiryMonth = new SelectElement(element5);
                expiryMonth.SelectByText(cf.readingXMLFile("WebPortal", "BuyChannel", "MonthExpire", "Config.xml"));

                IWebElement element6 = driver.FindElement(OR.GetElement("BuyChannelVerification", "Expiry2", "TVWebPortalOR.xml"));
                SelectElement expiryYear = new SelectElement(element6);
                expiryYear.SelectByText(cf.readingXMLFile("WebPortal", "BuyChannel", "YearExpire", "Config.xml"));

                driver.FindElement(OR.GetElement("BuyChannelVerification", "Email", "TVWebPortalOR.xml")).SendKeys(cf.readingXMLFile("WebPortal", "BuyChannel", "Email", "Config.xml"));
                driver.FindElement(OR.GetElement("BuyChannelVerification", "Houseno", "TVWebPortalOR.xml")).SendKeys(cf.readingXMLFile("WebPortal", "BuyChannel", "HouseNumber", "Config.xml"));

                driver.FindElement((OR.GetElement("BuyVideoVerification", "CreditCardSubmitBTN", "TVWebPortalOR.xml"))).Click();

                uf.isJqueryActive(driver);

                iWait.Until(ExpectedConditions.InvisibilityOfElementLocated((OR.GetElement("SeriesManagement", "OverlayWait", "TVWebPortalOR.xml"))));

                driver.FindElement((OR.GetElement("BuyVideoVerification", "GoToHomePageBTN", "TVWebPortalOR.xml"))).Click();

                uf.isJqueryActive(driver);

                //Again Searching for created channel and checking wheather play Button is visible OR not for that video

                iWait.Until(ExpectedConditions.ElementExists((OR.GetElement("VideoLandingPage", "SearchTB", "TVWebPortalOR.xml"))));

                //search the required video
                SearchTextField = driver.FindElement((OR.GetElement("VideoLandingPage", "SearchTB", "TVWebPortalOR.xml")));
                SearchTextField.SendKeys(videoname);

                iWait.Until(ExpectedConditions.ElementToBeClickable((OR.GetElement("VideoLandingPage", "SearchIcon", "TVWebPortalOR.xml"))));

                //Click on searchIcon
                SearchIcon = driver.FindElement((OR.GetElement("VideoLandingPage", "SearchIcon", "TVWebPortalOR.xml")));
                SearchIcon.Click();
                Thread.Sleep(2000);

                uf.isJqueryActive(driver);

                videoSearchList = (IList<IWebElement>)driver.FindElement((OR.GetElement("BuyVideoVerification", "SearchResult", "TVWebPortalOR.xml"))).FindElements((OR.GetElement("VideoLandingPage", "SearchResultRecord", "TVWebPortalOR.xml")));


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

                        IWebElement serachResultLink = driver.FindElement((OR.GetElement("VideoLandingPage", "SearchResultLink", "TVWebPortalOR.xml")));

                        log.Info("Web Video Title:" + webvideoTitle + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

                        serachResultLink.Click();

                        uf.isJqueryActive(driver);

                        break;
                    }
                }

                iWait.Until(ExpectedConditions.ElementIsVisible((OR.GetElement("BuyVideoVerification", "PlayBTN", "TVWebPortalOR.xml"))));

                Assert.AreEqual(true, driver.FindElement((OR.GetElement("BuyVideoVerification", "PlayBTN", "TVWebPortalOR.xml"))).Enabled);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message + "\n" + e.StackTrace + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

                Assert.AreEqual(true, false);
            }
        }
        #endregion Buy Channel Testing

        [TestFixtureTearDown]
        public void TearDown()
        {

            try
            {
                st.Chrome_TearDown(driver, log);             // Calling Chrome Teardown
            }
            catch (Exception e)
            {
                log.Info(e.Message + "\n" + e.StackTrace);
                Assert.AreEqual(true, false);
            }
        }
    }
}

           
       

  

