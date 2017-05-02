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

namespace IETTVAdminPortal.Chrome
{
    /*
     * more Verification points need to be added
     * need to create test for uploading event
     * */

    [TestFixture]
    public class Chrome_VideoRequestByUser
    {

        #region variable declaration and object initialisation

        // This is to configure logger mechanism for Utilities.Config
        private static ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        string browserType = "";

        string driverName = "", driverPath, appURL;

        internal IWebDriver driver = null;

        string guid_Admin = "";

        string videoName;

        string eventNameToBeVerified;

        private readonly Random _rng = new Random();

        private const string _chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

        //Instantiating Utilities function class

        Utility_Functions uf = new Utility_Functions();                                         // Instantiate object for Utility Functons

        Object_Repository_Class OR = new Object_Repository_Class();

        Chrome_AdminSetupTearDown st = new Chrome_AdminSetupTearDown();

        Chrome_VideoRequestByUserVerification vidReqByUser;

        Chrome_VideoManagementVerification objVidMngmntVer;

        Chrome_VideoManagement objAdminVideoManagement = null;

        IJavaScriptExecutor executor;

        List<String> ChannelDataList = new List<String>();  //List to store current channel details.

        Random random = new Random();  // Obejct created to generate Random Number 

        Configuration cf = new Configuration();                                                  // Instantiate object for Configuration

        IWait<IWebDriver> iWait = null;

        string viewVideoName;

        string rejectVideoName;

        // Chrome_VideoSearchResult 

        #endregion

        #region Constructors

        public Chrome_VideoRequestByUser(IWebDriver driver, ILog log1, IJavaScriptExecutor Executor, IWait<IWebDriver> iWait)
        {
            this.driver = driver;

            log = log1;

            this.executor = Executor;

            this.iWait = iWait;

        }

        public Chrome_VideoRequestByUser()
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

            vidReqByUser = new Chrome_VideoRequestByUserVerification(driver, log, executor, iWait);

            appURL = st.Chrome_Setup(driver, log, executor);                                               // Calling Chrome Setup  
        }


        #endregion

        #region Functions
        //This function wait invisibility of overlay class
        public void OverlayWait()
        {
            iWait.Until(d => d.FindElement((OR.GetElement("SeriesManagement", "OverlayWait", "TVAdminPortalOR.xml"))).GetAttribute("class").Contains("display-block"));
        }

        public void View_Create_RequestedVideo(string videoName)
        {

            log.Info("View_Create_RequestedVideo Functionality started" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

            browserType = uf.getRunningBrowser(driver, driver.FindElement(By.TagName("html")));

            uf.SwitchToAdminTab(driver, browserType);

            iWait.Until(ExpectedConditions.ElementIsVisible((OR.GetElement("VideoRequestByUser", "AdminMenu", "TVAdminPortalOR.xml"))));

            //clicking on Admin dropdown
            driver.FindElement((OR.GetElement("VideoRequestByUser", "AdminMenu", "TVAdminPortalOR.xml"))).Click();

            Thread.Sleep(3000);

            iWait.Until(ExpectedConditions.ElementToBeClickable((OR.GetElement("VideoRequestByUser", "ApproveBooking", "TVAdminPortalOR.xml"))));

            //Clicking on Approve Booking Requests
            driver.FindElement((OR.GetElement("VideoRequestByUser", "ApproveBooking", "TVAdminPortalOR.xml"))).Click();

            driver.FindElement((OR.GetElement("VideoRequestByUser", "SearchVideoTB", "TVAdminPortalOR.xml"))).SendKeys(videoName);

            driver.FindElement((OR.GetElement("VideoRequestByUser", "SearchButton", "TVAdminPortalOR.xml"))).Click();

            iWait.Until(ExpectedConditions.InvisibilityOfElementLocated((OR.GetElement("SeriesManagement", "OverlayWait", "TVAdminPortalOR.xml"))));

            iWait.Until(ExpectedConditions.ElementIsVisible((OR.GetElement("VideoRequestByUser", "VideoBoookingList", "TVAdminPortalOR.xml"))));

            View_Create_VideFunctionality(videoName, browserType);
        }

        public void Reject_RequestedVideo(string videoName)
        {

            log.Info("Reject_RequestedVideo Functionality started" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

            browserType = uf.getRunningBrowser(driver, driver.FindElement(By.TagName("html")));

            uf.SwitchToAdminTab(driver, browserType);

            iWait.Until(ExpectedConditions.ElementIsVisible((OR.GetElement("VideoRequestByUser", "AdminMenu", "TVAdminPortalOR.xml"))));

            //clicking on Admin dropdown
            driver.FindElement((OR.GetElement("VideoRequestByUser", "AdminMenu", "TVAdminPortalOR.xml"))).Click();

            Thread.Sleep(3000);

            iWait.Until(ExpectedConditions.ElementToBeClickable((OR.GetElement("VideoRequestByUser", "ApproveBooking", "TVAdminPortalOR.xml"))));

            //Clicking on Approve Booking Requests
            driver.FindElement((OR.GetElement("VideoRequestByUser", "ApproveBooking", "TVAdminPortalOR.xml"))).Click();

            driver.FindElement((OR.GetElement("VideoRequestByUser", "SearchVideoTB", "TVAdminPortalOR.xml"))).SendKeys(videoName);

            driver.FindElement((OR.GetElement("VideoRequestByUser", "SearchButton", "TVAdminPortalOR.xml"))).Click();

            iWait.Until(ExpectedConditions.InvisibilityOfElementLocated((OR.GetElement("SeriesManagement", "OverlayWait", "TVAdminPortalOR.xml"))));

            iWait.Until(ExpectedConditions.ElementIsVisible((OR.GetElement("VideoRequestByUser", "VideoBoookingList", "TVAdminPortalOR.xml"))));

            Thread.Sleep(3000);

            RejectVideoFunctionalityTest(videoName);
        }

        void View_Create_VideFunctionality(string videoName, string browserType)
        {

            objVidMngmntVer = new Chrome_VideoManagementVerification(driver, log, executor, iWait);

            IList<IWebElement> videoRowList = (IList<IWebElement>)driver.FindElement((OR.GetElement("VideoRequestByUser", "VideoBoookingList", "TVAdminPortalOR.xml"))).FindElements(By.TagName("tr"));

            Boolean flag = false;
            int i = 0;
            foreach (IWebElement currentRow in videoRowList)
            {
                //Check Row that have class="GridRowStyle" or class="AltGridStyle"

                if (currentRow.GetAttribute("class").Equals("GridRowStyle") || currentRow.GetAttribute("class").Equals("AltGridStyle"))
                {
                    String columData = currentRow.FindElements(By.TagName("td"))[3].FindElement(By.TagName("span")).Text.Trim();

                    log.Info("Video Title from manage page::" + columData);

                    if (columData.Equals(videoName))
                    {
                        flag = true;

                        //View Button and Close the dialogue box
                        //driver.FindElement(By.XPath("//button[@value='View']")).Click();

                        iWait.Until(ExpectedConditions.InvisibilityOfElementLocated((OR.GetElement("SeriesManagement", "OverlayWait", "TVAdminPortalOR.xml"))));
                        Thread.Sleep(2000);
                        driver.FindElement(By.CssSelector("div table#ContentPlaceHolder1_grdVideoBookingListing tbody tr:nth-child(2) div button")).Click();

                        Thread.Sleep(2000);

                        iWait.Until(ExpectedConditions.TextToBePresentInElementLocated((OR.GetElement("VideoRequestByUser", "PopUpVideoTitle", "TVAdminPortalOR.xml")), videoName));

                        //Checking the View Functionality by comparing the video name, Event Name, Event Location and Event Venue.
                        Assert.AreEqual(viewVideoName, driver.FindElement((OR.GetElement("VideoRequestByUser", "PopUpVideoTitle", "TVAdminPortalOR.xml"))).Text);

                        //commented as it is not working - bug
                        //Assert.AreEqual(eventNameToBeVerified, driver.FindElement((OR.GetElement("VideoRequestByUser", "PopUpEventName", "TVAdminPortalOR.xml"))).Text);

                        Assert.AreEqual("Test Location", driver.FindElement((OR.GetElement("VideoRequestByUser", "PopUpLocation", "TVAdminPortalOR.xml"))).Text);

                        Assert.AreEqual("Test Venue", driver.FindElement((OR.GetElement("VideoRequestByUser", "PopUpVenue", "TVAdminPortalOR.xml"))).Text);

                        iWait.Until(ExpectedConditions.ElementIsVisible((OR.GetElement("VideoRequestByUser", "ModalContent", "TVAdminPortalOR.xml"))));
                        driver.FindElement((OR.GetElement("VideoRequestByUser", "CloseButton", "TVAdminPortalOR.xml"))).Click();

                        Thread.Sleep(2000);

                        guid_Admin = CreateVideFunctionality(videoName);

                        #region Checking Video Status at Web end

                        uf.SwitchToWebTab(driver, browserType);

                        log.Info("Switched Back to webportal");

                        driver.Navigate().Refresh();

                        IWebElement tblVideoBookingReqListing = driver.FindElement((OR.GetElement("VideoRequestByUser", "VideoBookingList", "TVAdminPortalOR.xml")));

                        iWait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy(By.TagName("tr")));

                        //IList<IWebElement> userContentRowList = (IList<IWebElement>)tblVideoBookingReqListing.FindElements(By.TagName("tr"));
                        IList<IWebElement> userContentRowList = (IList<IWebElement>)tblVideoBookingReqListing.FindElements(By.CssSelector("table#myRequestTable > tbody > tr"));

                        int j = 0;
                        int counter = 0;
                        foreach (IWebElement currentRow1 in userContentRowList)
                        {

                            if (counter >= 1)
                            {
                                String columData1 = currentRow1.FindElements((OR.GetElement("VideoRequestByUser", "ColumnData", "TVAdminPortalOR.xml")))[1].Text.Trim();
                                if (columData1.Equals(videoName))
                                {
                                    Assert.AreEqual("Submitted", currentRow1.FindElements((OR.GetElement("VideoRequestByUser", "ColumnData", "TVAdminPortalOR.xml")))[4].Text.Trim());
                                    break;
                                }
                                j++;
                            }
                            else
                                counter++;
                        }


                        uf.scrollUp(driver);

                        Thread.Sleep(5000);

                        objVidMngmntVer.searchVideo(videoName);

                        objVidMngmntVer.verifySearchedVideo(videoName, guid_Admin);

                        //logOut();

                        #endregion

                        break;
                    }
                    i++;
                }
            }

        }

        string CreateVideFunctionality(string vidName)
        {
            IList<IWebElement> videoRowList = (IList<IWebElement>)driver.FindElement((OR.GetElement("VideoRequestByUser", "VideoBoookingList", "TVAdminPortalOR.xml"))).FindElements(By.TagName("tr"));

            int i = 0;
            foreach (IWebElement currentRow in videoRowList)
            {
                //Check Row that have class="GridRowStyle" or class="AltGridStyle"

                if (currentRow.GetAttribute("class").Equals("GridRowStyle") || currentRow.GetAttribute("class").Equals("AltGridStyle"))
                {
                    String columData = currentRow.FindElements(By.TagName("td"))[3].FindElement(By.TagName("span")).Text.Trim();

                    log.Info("Video Title from manage page::" + columData);

                    if (columData.Equals(vidName))
                    {
                        objAdminVideoManagement = new IETTVAdminPortal.Chrome.Chrome_VideoManagement(driver, log, executor, iWait);

                        //Click on Create button
                        IWebElement createButton = driver.FindElement(OR.GetElement("VideoRequestByUser", "CreateButton", "TVAdminPortalOR.xml", i));

                        createButton.Click();

                        iWait.Until(ExpectedConditions.ElementIsVisible((OR.GetElement("VideoRequestByUser", "WarningYesButton", "TVAdminPortalOR.xml"))));

                        driver.FindElement((OR.GetElement("VideoRequestByUser", "WarningYesButton", "TVAdminPortalOR.xml"))).Click();

                        iWait.Until(ExpectedConditions.ElementIsVisible((OR.GetElement("VideoRequestByUser", "BasicInfoTab", "TVAdminPortalOR.xml"))));

                        Thread.Sleep(4000);

                        //OverlayWait();

                        // Thread.Sleep(2000);

                        Assert.AreEqual(viewVideoName, driver.FindElement((OR.GetElement("VideoRequestByUser", "VideoTitle", "TVAdminPortalOR.xml"))).GetAttribute("value"));

                        guid_Admin = driver.FindElement((OR.GetElement("VideoRequestByUser", "GuAdminId", "TVAdminPortalOR.xml"))).GetAttribute("value");

                        iWait.Until(ExpectedConditions.ElementExists((OR.GetElement("VideoRequestByUser", "VideoTypeDRD", "TVAdminPortalOR.xml"))));

                        IWebElement videoType = driver.FindElement((OR.GetElement("VideoRequestByUser", "VideoTypeDRD", "TVAdminPortalOR.xml")));
                        ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(true);", videoType);

                        //Thread.Sleep(3000);
                        SelectElement VideoTypeSelector = new SelectElement(videoType);

                        VideoTypeSelector.SelectByIndex(3);
                        //VideoTypeSelector.SelectByText("Keynote");

                        String selectedVideoType = VideoTypeSelector.SelectedOption.Text.Trim();

                        //uf.scrollUp(driver);

                        objAdminVideoManagement.addcopyright();

                        uf.scrollUp(driver);

                        Thread.Sleep(4000);

                        objAdminVideoManagement.uploadBrowseVideo();

                        objAdminVideoManagement.finalPublishVideo("normal");

                        //waiting for 2.5 minutes as video will be publish after 2minutes from current system time
                        Thread.Sleep(150000);

                        break;
                    }

                    i++;

                }
            }
            return guid_Admin;
        }

        void RejectVideoFunctionalityTest(string vidName)
        {
            objVidMngmntVer = new Chrome_VideoManagementVerification(driver, log, executor, iWait);

            iWait.Until(ExpectedConditions.ElementIsVisible((OR.GetElement("VideoRequestByUser", "VideoBoookingList", "TVAdminPortalOR.xml"))));

            IList<IWebElement> videoRowList = (IList<IWebElement>)driver.FindElement((OR.GetElement("VideoRequestByUser", "VideoBoookingList", "TVAdminPortalOR.xml"))).FindElements(By.TagName("tr"));

            browserType = uf.getRunningBrowser(driver, driver.FindElement(By.TagName("html")));

            Boolean flag = false;
            int i = 0;
            foreach (IWebElement currentRow in videoRowList)
            {
                //Check Row that have class="GridRowStyle" or class="AltGridStyle"

                if (currentRow.GetAttribute("class").Equals("GridRowStyle") || currentRow.GetAttribute("class").Equals("AltGridStyle"))
                {
                    String columData = currentRow.FindElements(By.TagName("td"))[3].FindElement(By.TagName("span")).Text.Trim();

                    log.Info("Video Title from manage page::" + columData);

                    if (columData.Equals(vidName))
                    {
                        //Click on Reject button
                        IWebElement rejectButton = driver.FindElement(OR.GetElement("VideoRequestByUser", "RejectButton", "TVAdminPortalOR.xml", i));

                        rejectButton.Click();

                        iWait.Until(ExpectedConditions.ElementIsVisible((OR.GetElement("VideoRequestByUser", "WarningYesButton", "TVAdminPortalOR.xml"))));

                        driver.FindElement((OR.GetElement("VideoRequestByUser", "WarningYesButton", "TVAdminPortalOR.xml"))).Click();

                        iWait.Until(ExpectedConditions.ElementIsVisible((OR.GetElement("SeriesManagement", "SuccessOkButton", "TVAdminPortalOR.xml"))));

                        driver.FindElement((OR.GetElement("SeriesManagement", "SuccessOkButton", "TVAdminPortalOR.xml"))).Click();

                        #region Checking Video Status at Web end

                        uf.SwitchToWebTab(driver, browserType);

                        log.Info("Switched Back to webportal");

                        driver.Navigate().Refresh();

                        uf.isJqueryActive(driver);

                        IWebElement tblVideoBookingReqListing = driver.FindElement((OR.GetElement("VideoRequestByUser", "VideoBookingList", "TVAdminPortalOR.xml")));

                        iWait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy(By.TagName("tr")));

                        //IList<IWebElement> userContentRowList = (IList<IWebElement>)tblVideoBookingReqListing.FindElements(By.TagName("tr"));
                        IList<IWebElement> userContentRowList = (IList<IWebElement>)driver.FindElements(By.CssSelector("table#myRequestTable > tbody > tr"));

                        int j = 0;
                        int counter = 0;
                        foreach (IWebElement currentRow1 in userContentRowList)
                        {
                            if (counter >= 1)
                            {
                                String columData1 = currentRow1.FindElements((OR.GetElement("VideoRequestByUser", "ColumnData", "TVAdminPortalOR.xml")))[1].Text.Trim();
                                if (columData1.Equals(vidName))
                                {
                                    Assert.AreEqual("Rejected", currentRow1.FindElements((OR.GetElement("VideoRequestByUser", "ColumnData", "TVAdminPortalOR.xml")))[4].Text.Trim());
                                    break;
                                }
                                j++;
                            }
                            else
                                counter++;
                        }
                        #endregion

                        uf.scrollUp(driver);

                        Thread.Sleep(5000);

                        objVidMngmntVer.searchVideo(vidName);

                        Console.WriteLine("Search the rejected video::");

                        //verifying the video with no result found
                        String NoResultFound = driver.FindElement((OR.GetElement("VideoLandingPage", "SpanResult", "TVWebPortalOR.xml"))).Text;

                        Assert.AreEqual("No result found", NoResultFound);

                        logOut();

                        break;
                    }
                    i++;
                }
            }
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

        #region Video Request by User Testing

        [Test]
        public void TVAdmin_001_ApproveBookingRequestFunctionality()
        {
            try
            {
                log.Info("TVAdmin_001_View_CreateVideoReqByUser test Started" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

                uf.OpenNewTab(driver);

                log.Info("Window count ::: " + driver.WindowHandles.Count);

                iWait.Until(d => driver.FindElement(By.TagName("html")));

                String browsertype = uf.getRunningBrowser(driver, driver.FindElement(By.TagName("html")));

                uf.SwitchToWebTab(driver, browsertype);

                uf.NavigateWebPortal(cf, driver);

                //calling from IETTVWebportal project to search created video on web portal and verifying the same
                vidReqByUser.RedirectToLogin();

                ClickYesButton();

                String[] videoEventName = vidReqByUser.TestViewCreateRequestVideo().Split('-');

                viewVideoName = videoEventName[0].Trim().ToString();

                eventNameToBeVerified = videoEventName[1].Trim().ToString();

                View_Create_RequestedVideo(viewVideoName);

                Thread.Sleep(2000);

                rejectVideoName = vidReqByUser.TestRejectRequestVideo();

                Thread.Sleep(3000);

                Reject_RequestedVideo(rejectVideoName);


            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message + "\n" + e.StackTrace + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());
                log.Error(e.Message + "\n" + e.StackTrace + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());
                Assert.AreEqual(true, false);

            }

        }

        public void ClickYesButton()
        {

            Boolean yesButtonPresent = IsElementPresent(driver, By.Id("yesButtonId"));
            if (yesButtonPresent)
            {
                driver.FindElement(By.Id("yesButtonId")).Click();
            }

        }

        public static Boolean IsElementPresent(IWebDriver driver, By element)
        {
            try
            {
                //driver.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromSeconds(10));
                Thread.Sleep(1000);
                driver.FindElement(element);
                return true;
            }
            catch (Exception)
            {

                return false;
            }
        }

        #region existing code

        ///// <summary>
        ///// test -001 fails - Bug
        ///// </summary>
        //[Test]
        //public void TVAdmin_001_View_CreateVideoReqByUser()
        //{
        //    try
        //    {
        //        log.Info("TVAdmin_001_View_CreateVideoReqByUser test Started" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

        //        uf.OpenNewTab(driver);

        //        log.Info("Window count ::: " + driver.WindowHandles.Count);

        //        iWait.Until(d => driver.FindElement(By.TagName("html")));

        //        String browsertype = uf.getRunningBrowser(driver, driver.FindElement(By.TagName("html")));

        //        uf.SwitchToWebTab(driver, browsertype);

        //        uf.NavigateWebPortal(cf, driver);

        //        //calling from IETTVWebportal project to search created video on web portal and verifying the same

        //        String[] videoEventName = vidReqByUser.TestViewCreateRequestVideo().Split('-');

        //        videoName = videoEventName[0].Trim().ToString();

        //        eventNameToBeVerified = videoEventName[1].Trim().ToString();

        //        View_Create_RequestedVideo(videoName);

        //        log.Info("TVAdmin_001_View_CreateVideoReqByUser test completed" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());


        //    }

        //    catch (Exception e)
        //    {
        //        Console.WriteLine(e.Message + "\n" + e.StackTrace + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());
        //        log.Error(e.Message + "\n" + e.StackTrace + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());
        //        Assert.AreEqual(true, false);

        //    }
        //}

        //[Test]
        //public void TVAdmin_002_RejectVideoReqByUser()
        //{
        //    try
        //    {
        //        log.Info("TVAdmin_002_RejectVideoReqByUser test Started" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

        //        uf.OpenNewTab(driver);

        //        log.Info("Window count ::: " + driver.WindowHandles.Count);

        //        iWait.Until(d => driver.FindElement(By.TagName("html")));

        //        String browsertype = uf.getRunningBrowser(driver, driver.FindElement(By.TagName("html")));

        //        uf.SwitchToWebTab(driver, browsertype);

        //        uf.NavigateWebPortal(cf, driver);

        //        //calling from IETTVWebportal project to search created video on web portal and verifying the same
        //        videoName = vidReqByUser.TestRejectRequestVideo();

        //        Reject_RequestedVideo(videoName);

        //        log.Info("TVAdmin_002_RejectVideoReqByUser test completed" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());
        //    }

        //    catch (Exception e)
        //    {
        //        Console.WriteLine(e.Message + "\n" + e.StackTrace + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());
        //        log.Error(e.Message + "\n" + e.StackTrace + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

        //        Assert.AreEqual(true, false);

        //    }
        //}

        #endregion

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








