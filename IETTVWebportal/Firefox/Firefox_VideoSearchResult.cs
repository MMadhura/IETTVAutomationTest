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

namespace IETTVWebportal.Firefox
{
    
    [TestFixture]
    public class Firefox_VideoSearchResult
    {
        // This is to configure logger mechanism for Utilities.Config
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        internal IWebDriver driver = null;
               
        string appURL;

        int screenHeight, screenWidth;

        IJavaScriptExecutor executor;

        IWait<IWebDriver> iWait;

        Utility_Functions uf = new Utility_Functions();                          // Instantiate object for Utility Functons

        Configuration cf = new Configuration();                                  // Instantiate object for Configuration

        Object_Repository_Class or = new Object_Repository_Class();             // Instantiate object for object repository

        Firefox_WebSetupTearDown st = new Firefox_WebSetupTearDown();           // Instantiate object for Firefox Setup Teardown

        [TestFixtureSetUp]
        public void FixtureSetUp()
        {
            log4net.Config.XmlConfigurator.Configure(new FileInfo(AppDomain.CurrentDomain.BaseDirectory + "WebPortal.config"));

            log.Info("Inside Fixture Setup of Firefox - BottomBar Verification Test" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

            driver = new FirefoxDriver();                                                       // Initialize Firefox driver  
            
            uf.CreateOrReplaceVideoFolder();

            iWait = new WebDriverWait(driver, TimeSpan.FromSeconds(60));

            executor = (IJavaScriptExecutor)driver;
        }

        [SetUp]
        public void SetUp()
        {
            appURL = st.Firefox_Setup(driver, log, executor);                                   // Calling Firefox Setup
        }

        //This function search the required video and verify the same
        public void TVWeb_001_SearchVideofunctionality(IWebDriver driver, IWait<IWebDriver> iWait, string videoname, string GUID_Admin)
        {
           
            appURL = cf.readingXMLFile("WebPortal", "Login", "startURL", "Config.xml");

            driver.Navigate().GoToUrl(appURL);  

            driver.Manage().Window.Maximize();

            //wait till jquery gets completed
            uf.isJqueryActive(driver);
            iWait.Until(ExpectedConditions.ElementExists(By.Id(or.readingXMLFile("VideoLandingPage", "SearchTB", "TVWebPortalOR.xml"))));

            //search the required video
            IWebElement SearchTextField = driver.FindElement(By.Id(or.readingXMLFile("VideoLandingPage", "SearchTB", "TVWebPortalOR.xml")));
            SearchTextField.SendKeys(videoname);

            iWait.Until(ExpectedConditions.ElementToBeClickable(By.Id(or.readingXMLFile("VideoLandingPage", "SearchIcon", "TVWebPortalOR.xml"))));    

            //Click on searchIcon
            IWebElement SearchIcon = driver.FindElement(By.Id(or.readingXMLFile("VideoLandingPage", "SearchIcon", "TVWebPortalOR.xml")));  
            SearchIcon.Click();
            uf.isJqueryActive(driver);

            //verifying the search result
            IList<IWebElement> videoSearchList = (IList<IWebElement>)driver.FindElement(By.Id(or.readingXMLFile("VideoLandingPage", "SearchResult", "TVWebPortalOR.xml"))).FindElements(By.ClassName(or.readingXMLFile("VideoLandingPage", "SearchResultRecord", "TVWebPortalOR.xml")));
           
            //gettting the search result details
            foreach (IWebElement currentSearchrecord in videoSearchList)
            {
                IWebElement searchresultDetails = driver.FindElement(By.CssSelector(or.readingXMLFile("VideoLandingPage", "SearchResultDetails", "TVWebPortalOR.xml")));  //

                //getting video Title frm search result
                String webvideoTitle = searchresultDetails.Text.Trim();  
                if (webvideoTitle.Equals(videoname))
                {
                    String videoID_Web = searchresultDetails.GetAttribute("data-videono");
                    String GUID_Web = searchresultDetails.GetAttribute("data-videoid");

                    //verifying the Video Guid match on webportal with admin portal 
                    Assert.AreEqual(GUID_Admin, GUID_Web);

                    break;
                }
            }
        }

        //This function search the hide record video and verify the same
        public void verifyingNoresultFound(IWebDriver driver, IWait<IWebDriver> iWait, string videoname)
        {
            appURL = cf.readingXMLFile("WebPortal", "Login", "startURL", "Config.xml");

            driver.Navigate().GoToUrl(appURL);  
            driver.Manage().Window.Maximize();

            //wait till jquery gets completed
            uf.isJqueryActive(driver);
            iWait.Until(ExpectedConditions.ElementExists(By.Id(or.readingXMLFile("VideoLandingPage", "SearchTB", "TVWebPortalOR.xml"))));

            //search the required video
            IWebElement SearchTextField = driver.FindElement(By.Id(or.readingXMLFile("VideoLandingPage", "SearchTB", "TVWebPortalOR.xml")));
            SearchTextField.SendKeys(videoname);

            iWait.Until(ExpectedConditions.ElementToBeClickable(By.Id(or.readingXMLFile("VideoLandingPage", "SearchIcon", "TVWebPortalOR.xml"))));

            //Click on searchIcon
            IWebElement SearchIcon = driver.FindElement(By.Id(or.readingXMLFile("VideoLandingPage", "SearchIcon", "TVWebPortalOR.xml")));   
            SearchIcon.Click();
            uf.isJqueryActive(driver);

            iWait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy(By.CssSelector("div > span#result > p")));

            //verifying the video with no result found
            String NoResultFound = driver.FindElement(By.CssSelector("div > span#result > p")).Text;
          
            Thread.Sleep(1000);
            Assert.AreEqual("No result found", NoResultFound);
        }

        [TestFixtureTearDown]
        public void TearDown()
        {
            st.Firefox_TearDown(driver, log);                                                   // Calling Firefox Teardown
        }

    }
}
