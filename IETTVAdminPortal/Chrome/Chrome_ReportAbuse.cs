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
using NSoup.Select;
using System.Data.SqlClient;
using System.Data;
using Sikuli4Net.sikuli_UTIL;
using Sikuli4Net.sikuli_REST;

namespace IETTVAdminPortal.Chrome
{

    [TestFixture]
    public class Chrome_ReportAbuse
    {
        #region variable declaration and object initialisation
        
        // This is to configure logger mechanism for Utilities.Config
        private static ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        string driverName = "", driverPath, appURL;

        internal IWebDriver driver = null;
        
        String guid_Admin, videoID_Admin;

        int rowcounter;

        IWebElement CopyrightuploadButton, SuccessBannerMessage, OkButton, chooseFileButton;

        //Instantiating Utilities function class

        Utility_Functions uf = new Utility_Functions();                                         // Instantiate object for Utility Functons

        Object_Repository_Class OR = new Object_Repository_Class();

        Chrome_AdminSetupTearDown st = new Chrome_AdminSetupTearDown();

        Chrome_ReportAbuseVerification reportAbuseVerification;

        Chrome_VideoManagement objAdminVideoManagement = null;

        //variables used while creating channel
        String backgroundHexColor, buttonHexColor, successMessage;

        int numberOfCategory, randomSelector;

        IWebElement ChannelCategory, ChannelType, ChannelPosition, PricingType, BackgroundColourPicker, ButtonColourPicker, Player, watermarkRequired,
                   ChannelIcon, ShowChannelIcon, BackgroundImage, Notes, Savebutton, okButton;

        IJavaScriptExecutor executor;

        IWebElement checkBox;

        List<String> ChannelDataList = new List<String>();  //List to store current channel details.

        Boolean flag = false;

        String columData = null;  // Used to store Channel Name

        Random random = new Random();  // Obejct created to generate Random Number 

        Configuration cf = new Configuration();                                                  // Instantiate object for Configuration

        IWait<IWebDriver> iWait = null;

        #endregion

        #region Constructors
            public Chrome_ReportAbuse(IWebDriver driver, ILog log1, IJavaScriptExecutor Executor, IWait<IWebDriver> iWait)
        {
            this.driver = driver;

            log = log1;

            this.executor = Executor;

            this.iWait = iWait;

        }

            public Chrome_ReportAbuse()
        {

        }
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
                driver = new ChromeDriver(baseDir + "/DLLs", new ChromeOptions(), TimeSpan.FromSeconds(120)); // Initialize chrome driver 
                EventFire ef = new EventFire(driver);
                driver = ef; 

                executor = (IJavaScriptExecutor)driver;

                iWait = new WebDriverWait(driver, TimeSpan.FromSeconds(60));
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message + "\n" + e.StackTrace + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());
                log.Error(e.Message + "\n" + e.StackTrace + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());
                Assert.AreEqual(true, false);
            }
        }

        [SetUp]
        public void SetUp()
        {
  
            reportAbuseVerification = new Chrome_ReportAbuseVerification(driver, log, executor, iWait);

            objAdminVideoManagement = new IETTVAdminPortal.Chrome.Chrome_VideoManagement(driver, log, executor, iWait);

            appURL = st.Chrome_Setup(driver, log, executor);                                               // Calling Chrome Setup  
            
        }

        #endregion

        #region Report Abuse Testing

        /// <summary>
        /// This test verifies Report Abuse Comment for the video
        /// bug- getting fail as admin portal is abuse checkbox is not working
        /// </summary>
        [Test]
        public void TVAdmin_001_ReportAbuseTest()
        {
            try
            {

                log.Info("TVAdmin_001_ReportAbuseTest Started" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

                objAdminVideoManagement.redirectToVideoManagement();

                objAdminVideoManagement.basicInfoTab();

                String adminSelectedChannel = objAdminVideoManagement.channelListTab();

                objAdminVideoManagement.pricingListTab("Free");

                objAdminVideoManagement.addcopyright();

                objAdminVideoManagement.uploadBrowseVideo();

                objAdminVideoManagement.finalPublishVideo("normal");

                //waiting for 2.5 minutes as video will be publish after 2minutes from current system time
                Thread.Sleep(150000);
               
                uf.OpenNewTab(driver);

                log.Info("Window count ::: " + driver.WindowHandles.Count);

                String browsertype = uf.getRunningBrowser(driver, driver.FindElement(By.TagName("html")));

                uf.SwitchToWebTab(driver, browsertype);

                uf.NavigateWebPortal(cf, driver);

                reportAbuseVerification.ReportAbuseTest(objAdminVideoManagement.videoName, guid_Admin);

                log.Info("TVAdmin_001_ReportAbuseTest completed" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

            }

            catch (Exception e)
            {
                Console.WriteLine(e.Message + "\n" + e.StackTrace + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

                log.Error(e.Message + "\n" + e.StackTrace + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

                Assert.AreEqual(true, false);

            }
        }

        #endregion

        [TearDown]
        public void TearDown()
        {
            try
            {
                if (TestContext.CurrentContext.Result.Status.ToString().Equals("Failed"))
                {
                    st.Chrome_SetUpTearDowm(driver, log,true);
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








