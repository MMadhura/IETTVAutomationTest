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
using Microsoft.Expression.Encoder.ScreenCapture;
using System.Diagnostics;
using IETTVWebportal.Reusable_Functions;

namespace IETTVWebportal.IE
{
   
    [TestFixture]
    public class IE_VideoSearchResult
    {
        // This is to configure logger mechanism for Utilities.Config
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        internal IWebDriver driver = null;
        
        string driverName = "", driverPath, appURL;
        
        IWait<IWebDriver> iWait = null;

        IJavaScriptExecutor executor;

        Utility_Functions uf = new Utility_Functions();                       // Instantiate object for Utility Functons

        Configuration cf = new Configuration();                               // Instantiate object for Configuration

        Object_Repository_Class or = new Object_Repository_Class();           // Instantiate object for object repository

        IE_WebSetupTearDown st = new IE_WebSetupTearDown();                   // Instantiate object for IE Setup Teardown

        [TestFixtureSetUp]
        public void FixtureSetUp()
        {
            log4net.Config.XmlConfigurator.Configure(new FileInfo(AppDomain.CurrentDomain.BaseDirectory + "WebPortal.config"));

            log.Info("Inside Fixture Setup of IE - BottomBar Verification Test" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

            uf.CreateOrReplaceVideoFolder();

            string baseDir = AppDomain.CurrentDomain.BaseDirectory;

            driverName = "webdriver.ie.driver";                                                // Driver name for IE

            driverPath = baseDir + "/IEDriverServer.exe";                                      // path for IE Driver

            System.Environment.SetEnvironmentVariable(driverName, driverPath);

            InternetExplorerOptions opt = new InternetExplorerOptions();                       // Ensuring Clean IE session

            opt.EnsureCleanSession = true;

            driver = new InternetExplorerDriver(opt);                                          // Initialize IE driver              

            if (uf.getRunningBrowser(driver, driver.FindElement(By.TagName("html"))).ToString().Equals("IE11"))
            {

                log.Info("IE11 detected in setup" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

                // Check if Registry Entry is present for IE 11 browser

                if (uf.checkIE11RegistryPresence().Equals("true"))
                {
                    log.Info("Registry Created successfully / Present for IE 11" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());
                }
                else
                {
                    log.Info("Registry couldn't be created. Test may not run properly in IE 11. Please contact administrator" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());
                }

            }

            iWait = new WebDriverWait(driver, TimeSpan.FromSeconds(60));

            executor = (IJavaScriptExecutor)driver;
        }


        [SetUp]
        public void SetUp()
        {
            appURL = st.IE_Setup(driver, log, executor);
        }

        //This function search the required video and verify the same
        public void TVWeb_001_SearchVideofunctionality(IWebDriver driver, IWait<IWebDriver> iWait, string videoname, string GUID_Admin)
        {
          
            appURL = cf.readingXMLFile("WebPortal", "Login", "startURL", "Config.xml");

            driver.Navigate().GoToUrl(appURL); 
            driver.Manage().Window.Maximize();

            //wait till jquery gets completed
            uf.isJqueryActive(driver);
            iWait.Until(ExpectedConditions.ElementExists(By.Id("searchtextbox")));

            //search the required video
            IWebElement SearchTextField = driver.FindElement(By.Id("searchtextbox"));
            SearchTextField.SendKeys(videoname);

            iWait.Until(ExpectedConditions.ElementToBeClickable(By.Id("searchicon")));    

            //Click on searchIcon
            IWebElement SearchIcon = driver.FindElement(By.Id("searchicon"));   
            SearchIcon.Click();
            uf.isJqueryActive(driver);
            //verifying the search result

            IList<IWebElement> videoSearchList = (IList<IWebElement>)driver.FindElement(By.Id("SearchResult")).FindElements(By.ClassName("searchresultrecord"));
            //gettting the search result details
          
            foreach (IWebElement currentSearchrecord in videoSearchList)
            {
                IWebElement searchresultDetails = driver.FindElement(By.CssSelector("div.video-description-clear-both > span > a"));

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

            driver.Navigate().GoToUrl(appURL);   //need to remove hardcoded URL
            driver.Manage().Window.Maximize();

            //wait till jquery gets completed
            uf.isJqueryActive(driver);
            iWait.Until(ExpectedConditions.ElementExists(By.Id("searchtextbox")));

            //search the required video
            IWebElement SearchTextField = driver.FindElement(By.Id("searchtextbox"));
            SearchTextField.SendKeys(videoname);

            iWait.Until(ExpectedConditions.ElementToBeClickable(By.Id("searchicon")));

            //Click on searchIcon
            IWebElement SearchIcon = driver.FindElement(By.Id("searchicon"));   
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
            st.IE_TearDown(driver, log);
        }

    }
}
