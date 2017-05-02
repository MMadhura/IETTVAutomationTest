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
using NSoup.Nodes;  // for nsoup documents
using NSoup.Select;  //for nsoup element
using log4net;
using log4net.Config;
using System.Reflection;
using System.IO;
using System.Diagnostics;
using Utilities;
using System.Data.SqlClient;
using System.Data;
using Sikuli4Net.sikuli_UTIL;
using Sikuli4Net.sikuli_REST;



namespace IETTVAdminPortal.Chrome
{

    [TestFixture]
    public class Chrome_SeriesManagement
    {

        #region variable declaration and object initialisation

        // This is to configure logger mechanism for Utilities.Config
        private static ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        string browserType = "";

        string driverName = "", driverPath, appURL;

        Boolean flag = false;

        IList<IWebElement> rowList = null;

        internal IWebDriver driver = null;


        string seriesName;

        private readonly Random _rng = new Random();

        private const string _chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

        //Instantiating Utilities function class

        Utility_Functions uf = new Utility_Functions();                                         // Instantiate object for Utility Functons

        Chrome_AdminSetupTearDown st = new Chrome_AdminSetupTearDown();

        Object_Repository_Class OR = new Object_Repository_Class();

        IJavaScriptExecutor executor;

        List<String> ChannelDataList = new List<String>();  //List to store current channel details.

        Random random = new Random();  // Obejct created to generate Random Number 

        Configuration cf = new Configuration();                                                  // Instantiate object for Configuration

        IWait<IWebDriver> iWait = null;

        Chrome_SeriesManagementVerification objSeriesMgtVeri = null;

        int videoEventSearchListCount = 0;

        Dictionary<String, String> adminSponsorDetails = null;

        string seriesLogoImage = null;

        APILauncher launcher = new APILauncher(true);

        #endregion

        #region Constructors

        public Chrome_SeriesManagement(IWebDriver driver, ILog log1, IJavaScriptExecutor Executor, IWait<IWebDriver> iWait)
        {
            this.driver = driver;

            log = log1;

            this.executor = Executor;

            this.iWait = iWait;

        }

        public Chrome_SeriesManagement()
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
                log.Error(e.Message + "\n" + e.StackTrace + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());
                Assert.AreEqual(true, false);
            }

        }


        [SetUp]
        public void SetUp()
        {
            appURL = st.Chrome_Setup(driver, log, executor);                                               // Calling Chrome Setup  
        }
        

        #endregion

        #region Functions
        //This function wait invisibility of overlay class
        public void OverlayWait()
        {
            iWait.Until(ExpectedConditions.InvisibilityOfElementLocated((OR.GetElement("SeriesManagement", "OverlayWait", "TVAdminPortalOR.xml"))));
        }

        public void OverlayWaitDisplayBlock(string message)
        {
            
            iWait.Until(ExpectedConditions.ElementIsVisible((OR.GetElement("CategoryManagement", "WarningYesButton", "TVAdminPortalOR.xml"))));

            iWait.Until(d => d.FindElement(OR.GetElement("CategoryManagement", "WarningMessage", "TVAdminPortalOR.xml"))).Text.Equals(message);

            Assert.AreEqual(message, driver.FindElement(OR.GetElement("CategoryManagement", "WarningMessage", "TVAdminPortalOR.xml")).Text.Trim());

            //Click on ok button banner message

            executor.ExecuteScript("arguments[0].click()", driver.FindElement((OR.GetElement("CategoryManagement", "WarningYesButton", "TVAdminPortalOR.xml"))));
          
        }

        public IWebElement adminDropdown()
        {
            iWait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy(OR.GetElement("CategoryManagement", "AdminMenu", "TVAdminPortalOR.xml")));

            return driver.FindElement(OR.GetElement("CategoryManagement", "AdminMenu", "TVAdminPortalOR.xml"));
        }

        //selecting Series Management link from Admin Dropdown
        public void redirectToSeriesManagement()
        {
            log.Info("inside redirectToSeriesManagement " + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

            //clicking on Admin dropdown   
            adminDropdown().Click();

            //Clicking on series Management Link
            seriesManagementLink().Click();

            iWait.Until(ExpectedConditions.ElementIsVisible(OR.GetElement("SeriesManagement", "SeriesManagementMenu", "TVAdminPortalOR.xml")));

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

        public IWebElement seriesManagementLink()
        {
            iWait.Until(ExpectedConditions.ElementToBeClickable((OR.GetElement("SeriesManagement", "SeriesManagementLink", "TVAdminPortalOR.xml"))));

            return driver.FindElement((OR.GetElement("SeriesManagement", "SeriesManagementLink", "TVAdminPortalOR.xml")));
        }

        public void CreateNewSeriesTab()
        {

            log.Info("inside CreateNewSeriesTab " + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

            //getting the uique name for the series
            seriesName = "ser" + uf.getShortGuid();

            log.Info("Series name  : " + seriesName);

            driver.FindElement((OR.GetElement("SeriesManagement", "SeriesName", "TVAdminPortalOR.xml"))).SendKeys(seriesName);

            //Enter data in Description field
            driver.FindElement((OR.GetElement("SeriesManagement", "Description", "TVAdminPortalOR.xml"))).SendKeys("Test Description");

            adminSponsorDetails = EditSponsor();

            #region Add Video
            SelectElement searchByDropDown = new SelectElement(driver.FindElement((OR.GetElement("SeriesManagement", "SearchByDDL", "TVAdminPortalOR.xml"))));
            searchByDropDown.SelectByText("Title");

            //Search for the video
            driver.FindElement((OR.GetElement("SeriesManagement", "SearchTextBox", "TVAdminPortalOR.xml"))).SendKeys(cf.readingXMLFile("AdminPortal", "Series_Management", "videoName", "Config.xml"));

            //Click on Search Video/Event
            driver.FindElement((OR.GetElement("SeriesManagement", "SearchButton", "TVAdminPortalOR.xml"))).Click();

            //Wait till the result table
            iWait.Until(ExpectedConditions.ElementIsVisible((OR.GetElement("SeriesManagement", "CheckBox", "TVAdminPortalOR.xml"))));

            //Select the searched video check box
            driver.FindElement((OR.GetElement("SeriesManagement", "CheckBox", "TVAdminPortalOR.xml"))).Click();

            //Click on Add Button
            driver.FindElement((OR.GetElement("SeriesManagement", "AddButton", "TVAdminPortalOR.xml"))).Click();

            OverlayWait();
            #endregion

            #region Add Event

            //Wait till event radio button is clickable
            iWait.Until(ExpectedConditions.ElementToBeClickable((OR.GetElement("SeriesManagement", "EventRadioButton", "TVAdminPortalOR.xml"))));

            //click on event radion button
            driver.FindElement((OR.GetElement("SeriesManagement", "EventRadioButton", "TVAdminPortalOR.xml"))).Click();

            OverlayWait();
            Thread.Sleep(1000);

            //wait till Search by drop down is clickable
            iWait.Until(ExpectedConditions.ElementIsVisible((OR.GetElement("SeriesManagement", "SearchByDDL", "TVAdminPortalOR.xml"))));

            SelectElement searchEevntByDropDown = new SelectElement(driver.FindElement((OR.GetElement("SeriesManagement", "SearchByDDL", "TVAdminPortalOR.xml"))));
            searchEevntByDropDown.SelectByText("Title");

            //Search for the event
            driver.FindElement((OR.GetElement("SeriesManagement", "SearchTextBox", "TVAdminPortalOR.xml"))).SendKeys(cf.readingXMLFile("AdminPortal", "Series_Management", "eventName", "Config.xml"));

            //Click on Search Video/Event
            driver.FindElement((OR.GetElement("SeriesManagement", "SearchButton", "TVAdminPortalOR.xml"))).Click();

            OverlayWait();
            Thread.Sleep(1000);

            //Select the searched video check box
            executor.ExecuteScript("arguments[0].click()", driver.FindElement((OR.GetElement("SeriesManagement", "CheckBox", "TVAdminPortalOR.xml"))));

            //Click on Add Button
            driver.FindElement((OR.GetElement("SeriesManagement", "AddButton", "TVAdminPortalOR.xml"))).Click();

            #endregion

            OverlayWait();

            seriesLogoImage = UploadSeriesImage();

            //Click on Save Button
            driver.FindElement((OR.GetElement("SeriesManagement", "SaveButton", "TVAdminPortalOR.xml"))).Click();

            //Wait till confirmation message
            iWait.Until(ExpectedConditions.ElementIsVisible((OR.GetElement("SeriesManagement", "SuccessOkButton", "TVAdminPortalOR.xml"))));

            //Click on Ok Button
            driver.FindElement((OR.GetElement("SeriesManagement", "SuccessOkButton", "TVAdminPortalOR.xml"))).Click();
        }

        public Dictionary<String, String> EditSponsor()
        {
            log.Info("inside editSponsor " + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

            Dictionary<String, String> sponsorDetails = new Dictionary<String, String>();

            sponsorDetails.Add("url", "http://www.google.com");

            sponsorDetails.Add("imageName", cf.readingXMLFile("AdminPortal", "Channel_Management", "channelIcon", "Config.xml"));

            SponsorTextField().SendKeys(sponsorDetails["url"]);

            //string sponsorImagePath = Environment.CurrentDirectory + "\\Upload\\Images\\" + sponsorDetails["imageName"];
            string sponsorImagePath = AppDomain.CurrentDomain.BaseDirectory + "\\Upload\\Images\\" + sponsorDetails["imageName"];

            log.Info("uploadSponsorName: " + sponsorDetails["imageName"]);

            log.Info("uploadSponsorPath: " + sponsorImagePath);

            IWebElement sponsorChooseButton = driver.FindElement((OR.GetElement("SeriesManagement", "SponsorChooseButton", "TVAdminPortalOR.xml")));

            uf.uploadfile(sponsorChooseButton, sponsorImagePath);

            //click on sponsor upload button
            SponsorUploadButton().Click();

            verifySuccessBannerMessage("Sponser details added successfully.");

            return sponsorDetails;
        }

        public string UploadSeriesImage()
        {
            log.Info("inside UploadSeriesImage " + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

            string imageName = cf.readingXMLFile("AdminPortal", "Channel_Management", "backgroundImage", "Config.xml");

            //string imageUploadPath = Environment.CurrentDirectory + "\\Upload\\Images\\" + imageName;

            string imageUploadPath = AppDomain.CurrentDomain.BaseDirectory + "\\Upload\\Images\\" + imageName;

            IWebElement chooseImageButton = driver.FindElement((OR.GetElement("SeriesManagement", "SeriesIcon", "TVAdminPortalOR.xml")));

            uf.uploadfile(chooseImageButton, imageUploadPath);

            return imageName;
        }

        public IWebElement SponsorUploadButton()
        {
            iWait.Until(ExpectedConditions.ElementIsVisible((OR.GetElement("SeriesManagement", "SponsorUploadButton", "TVAdminPortalOR.xml"))));
            return driver.FindElement((OR.GetElement("SeriesManagement", "SponsorUploadButton", "TVAdminPortalOR.xml")));
        }

        public IWebElement SponsorTextField()
        {
            return driver.FindElement((OR.GetElement("SeriesManagement", "SponsorTextField", "TVAdminPortalOR.xml")));
        }

        public void verifySuccessBannerMessage(String message)
        {
           iWait.Until(ExpectedConditions.ElementIsVisible((OR.GetElement("SeriesManagement", "SuccessOkButton", "TVAdminPortalOR.xml"))));

            iWait.Until(d => d.FindElement((OR.GetElement("SeriesManagement", "SucessMessage", "TVAdminPortalOR.xml"))).Text.Equals(message));

            Assert.AreEqual(message, driver.FindElement((OR.GetElement("SeriesManagement", "SucessMessage", "TVAdminPortalOR.xml"))).Text.Trim());

            //Click on ok button banner message

            driver.FindElement((OR.GetElement("SeriesManagement", "SuccessOkButton", "TVAdminPortalOR.xml"))).Click();

            
            iWait.Until(ExpectedConditions.InvisibilityOfElementLocated((OR.GetElement("SeriesManagement", "SuccessOkButton", "TVAdminPortalOR.xml"))));

        }

        #endregion

        #region Series Management Tests

        /// <summary>
        /// This test verifies Buy Video Functionality        
        /// </summary>
        [Test]
        public void TVAdmin_001_CreateSeriesTest()
        {
            try
            {
                log.Info("TVAdmin_001_CreateSeriesTest test Started" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

                objSeriesMgtVeri = new Chrome_SeriesManagementVerification(driver, log, executor, iWait);

                String browsertype = uf.getRunningBrowser(driver, driver.FindElement(By.TagName("html")));
               
                redirectToSeriesManagement();

                //Click on Create Series
                driver.FindElement((OR.GetElement("SeriesManagement", "Series_Dtls", "TVAdminPortalOR.xml"))).Click();

                CreateNewSeriesTab();

                Thread.Sleep(3000);

                uf.OpenNewTab(driver);

                log.Info("count ::: " + driver.WindowHandles.Count);

                uf.SwitchToWebTab(driver, browsertype);

                uf.NavigateWebPortal(cf, driver);

                //calling from IETTVWebportal project to search created series on web portal and verifying the same
                videoEventSearchListCount = objSeriesMgtVeri.TestSeriesManagementVerification(seriesName, cf.readingXMLFile("AdminPortal", "Series_Management", "videoName", "Config.xml"), cf.readingXMLFile("AdminPortal", "Series_Management", "eventName", "Config.xml"));

                //Verifying the sponsered details at web end
                objSeriesMgtVeri.VerifySponsorDetails(adminSponsorDetails);

                //verifying the uploaded Series Logo at web end.
                objSeriesMgtVeri.VerifySeriesLogo(seriesLogoImage);

                //Retreiving the browsertype again as it get lost during the above steps to switch to admin tab again.
                browserType = uf.getRunningBrowser(driver, driver.FindElement(By.TagName("html")));

                uf.SwitchToAdminTab(driver, browserType);

                iWait.Until(ExpectedConditions.ElementIsVisible((OR.GetElement("SeriesManagement", "GridDetails", "TVAdminPortalOR.xml"))));

                IList<IWebElement> videoRowList = (IList<IWebElement>)driver.FindElement((OR.GetElement("SeriesManagement", "GridDetails", "TVAdminPortalOR.xml"))).FindElements(By.TagName("tr"));
                int i = 0;
                foreach (IWebElement currentRow in videoRowList)
                {
                    //Check Row that have class="GridRowStyle" or class="AltGridStyle"

                    if (currentRow.GetAttribute("class").Equals("GridRowStyle") || currentRow.GetAttribute("class").Equals("AltGridStyle"))
                    {
                        String columData = currentRow.FindElements(By.TagName("td"))[2].FindElement(By.TagName("span")).Text.Trim();

                        log.Info("Series Title from manage series::" + columData);

                        if (columData.Equals(seriesName))
                        {
                            //Click on Edit button
                            IWebElement editButton = driver.FindElement((OR.GetElement("SeriesManagement", "GridImage", "TVAdminPortalOR.xml")));

                            editButton.Click();

                            iWait.Until(ExpectedConditions.ElementIsVisible((OR.GetElement("SeriesManagement", "CreateSeriesTab", "TVAdminPortalOR.xml"))));

                            uf.scrollDown(driver);

                            //Click on Delete Event Button
                            driver.FindElement((OR.GetElement("SeriesManagement", "DeleteFirstRowButton", "TVAdminPortalOR.xml"))).Click();

                            OverlayWaitDisplayBlock("Are you sure, you wish to delete?");

                            iWait.Until(ExpectedConditions.ElementIsVisible((OR.GetElement("SeriesManagement", "SuccessOkButton", "TVAdminPortalOR.xml"))));

                            iWait.Until(d => d.FindElement((OR.GetElement("SeriesManagement", "SucessMessage", "TVAdminPortalOR.xml"))).Text.Equals("Record deleted sucessfully."));

                            Assert.AreEqual("Record deleted sucessfully.", driver.FindElement((OR.GetElement("SeriesManagement", "SucessMessage", "TVAdminPortalOR.xml"))).Text.Trim());
                            
                            //Click on ok button banner message
                            executor.ExecuteScript("arguments[0].click()", driver.FindElement((OR.GetElement("SeriesManagement", "SuccessOkButton", "TVAdminPortalOR.xml"))));

                            uf.scrollUp(driver);
                            uf.scrollDown(driver);

                            //Click on Save Button
                            executor.ExecuteScript("arguments[0].scrollIntoView(true);", driver.FindElement((OR.GetElement("SeriesManagement", "SaveButton", "TVAdminPortalOR.xml"))));
                            Thread.Sleep(500); 

                            executor.ExecuteScript("arguments[0].click()",driver.FindElement((OR.GetElement("SeriesManagement", "SaveButton", "TVAdminPortalOR.xml"))));

                            //Wait till confirmation message
                            iWait.Until(ExpectedConditions.ElementIsVisible((OR.GetElement("SeriesManagement", "SuccessOkButton", "TVAdminPortalOR.xml"))));

                            //Click on Ok Button
                            driver.FindElement((OR.GetElement("SeriesManagement", "SuccessOkButton", "TVAdminPortalOR.xml"))).Click();
                            i++;
                            break;
                        }
                        break;
                    }
                    //Checking for 1st Record Only
                    if (i == 1)
                        break;
                }

                browserType = uf.getRunningBrowser(driver, driver.FindElement(By.TagName("html")));

                uf.SwitchToWebTab(driver, browsertype);

                //Refresh after the record delete at admin to check the count (Should be 1 less than the previous no. of vidoes / events.)
                driver.Navigate().Refresh();

                uf.isJqueryActive(driver);

                //verifying the search result
                IList<IWebElement> videoEventSearchList = (IList<IWebElement>)driver.FindElement((OR.GetElement("SeriesManagement", "Search_Result", "TVAdminPortalOR.xml"))).FindElements((OR.GetElement("SeriesManagement", "SearchResultRecord", "TVAdminPortalOR.xml")));

                //Checking for the no. of counts it should be less the previous one.
                Assert.AreEqual(videoEventSearchList.Count, videoEventSearchListCount - 1);

                log.Info("TVAdmin_001_CreateSeriesTest test completed" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());
            }

            catch (Exception e)
            {
                log.Error(e.Message + "\n" + e.StackTrace + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());
                Console.WriteLine(e.Message + "\n" + e.StackTrace + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());
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








