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
using Sikuli4Net.sikuli_UTIL;
using Sikuli4Net.sikuli_REST;


namespace IETTVAdminPortal.Chrome
{

    [TestFixture]
    public class Chrome_UserGeneratedContent
    {

        // This is to configure logger mechanism for Utilities.Config
        private static ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        #region variable declaration and object initialisation

        IJavaScriptExecutor executor;

        string driverName = "", driverPath, appURL;

        internal IWebDriver driver = null;

        IWait<IWebDriver> iWait = null;

        //Instantiating Utilities function class

        Utility_Functions uf = new Utility_Functions();                                         // Instantiate object for Utility Functons

        Configuration cf = new Configuration();                                                 // Instantiate object for Configuration

        Object_Repository_Class OR = new Object_Repository_Class();

        Chrome_AdminSetupTearDown st = new Chrome_AdminSetupTearDown();

        Chrome_UserGeneratedContentVerification objWebUserGenContent;

        Chrome_VideoManagement objAdminVideoMngmnt;

        Chrome_VideoManagementVerification objWebVideoMngmnt;

        Chrome_PollManagementVerification objWebPollManagement = null;

        String guid_Admin;

        #endregion

        public Chrome_UserGeneratedContent(IWebDriver driver, ILog log1, IJavaScriptExecutor Executor, IWait<IWebDriver> iWait)
        {
            this.driver = driver;

            log = log1;

            this.executor = Executor;

            this.iWait = iWait;

        }


        public Chrome_UserGeneratedContent()
        {

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
            objAdminVideoMngmnt = new Chrome_VideoManagement(driver, log, executor, iWait);

            objWebVideoMngmnt = new Chrome_VideoManagementVerification(driver, log, executor, iWait);

            objWebUserGenContent = new Chrome_UserGeneratedContentVerification(driver, log, executor, iWait);            // Creating a object for calling IETTVWebPortal project

            appURL = st.Chrome_Setup(driver, log, executor);                                               // Calling Chrome Setup  
        }


        #endregion

        #region Resuable Elements

        public IWebElement adminDropdown()
        {
            iWait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy((OR.GetElement("VideoRequestByUser", "AdminMenu", "TVAdminPortalOR.xml"))));

            return driver.FindElement((OR.GetElement("VideoRequestByUser", "AdminMenu", "TVAdminPortalOR.xml")));
        }

        public IWebElement userGeneratedContentLink()
        {
            iWait.Until(ExpectedConditions.ElementToBeClickable(OR.GetElement("UserGeneratedContent", "UserContentLink", "TVAdminPortalOR.xml")));

            return driver.FindElement((OR.GetElement("UserGeneratedContent", "UserContentLink", "TVAdminPortalOR.xml")));
        }
        #endregion

        #region Reusable Function

        public void OverlayWait()
        {
            iWait.Until(ExpectedConditions.InvisibilityOfElementLocated((OR.GetElement("SeriesManagement", "OverlayWait", "TVAdminPortalOR.xml"))));
        }

        public void VerifySuccessBannerMessage(String message)
        {
            iWait.Until(ExpectedConditions.ElementIsVisible((OR.GetElement("SeriesManagement", "SuccessOkButton", "TVAdminPortalOR.xml"))));

            iWait.Until(d => d.FindElement((OR.GetElement("SeriesManagement", "SucessMessage", "TVAdminPortalOR.xml"))).Text.Equals(message));

            Assert.AreEqual(message, driver.FindElement((OR.GetElement("SeriesManagement", "SucessMessage", "TVAdminPortalOR.xml"))).Text.Trim());

            //Click on ok button banner message

            driver.FindElement((OR.GetElement("SeriesManagement", "SuccessOkButton", "TVAdminPortalOR.xml"))).Click();

            OverlayWait();

        }

        public void RedirectToUserGeneratedContent()
        {
            log.Info("inside redirectToUserGeneratedContent " + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

            //clicking on Admin dropdown   
            adminDropdown().Click();

            Thread.Sleep(3000);

            //Clicking on User Generated Content Link

            userGeneratedContentLink().Click();
        }

        public String SearchRequiredVideoFromTable(String videoName, String action)
        {
            log.Info("Search for the required video");

            String abstractContent = "Abstract field content";

            uf.isJqueryActive(driver);

            iWait.Until(ExpectedConditions.ElementIsVisible((OR.GetElement("VideoRequestByUser", "SearchVideoTB", "TVAdminPortalOR.xml"))));

            driver.FindElement((OR.GetElement("VideoRequestByUser", "SearchVideoTB", "TVAdminPortalOR.xml"))).SendKeys(videoName);

            //search the required video
            driver.FindElement((OR.GetElement("VideoRequestByUser", "SearchButton", "TVAdminPortalOR.xml"))).Click();

            OverlayWait();

            Thread.Sleep(3000);

            iWait.Until(ExpectedConditions.ElementExists((OR.GetElement("UserGeneratedContent", "VideoStatus", "TVAdminPortalOR.xml"))));

            IWebElement tblVideoListing = driver.FindElement((OR.GetElement("UserGeneratedContent", "VideoStatus", "TVAdminPortalOR.xml")));

            iWait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy(By.TagName("tr")));

            IList<IWebElement> userContentRowList = (IList<IWebElement>)tblVideoListing.FindElements(By.TagName("tr"));

            Boolean flag = false;

            int i = 0;

            foreach (IWebElement currentRow in userContentRowList)
            {

                //Check Row that have class="GridRowStyle" or class="AltGridStyle"
                if (currentRow.GetAttribute("class").Equals("GridRowStyle") || currentRow.GetAttribute("class").Equals("AltGridStyle"))
                {

                    String columData = currentRow.FindElements(By.TagName("td"))[3].FindElement(By.TagName("span")).Text.Trim();

                    log.Info("Video Title from manage page::" + columData);

                    if (columData.Equals(videoName))
                    {
                        flag = true;

                        if (action.Equals("reject"))
                        {
                            log.Info("Performing reject operation");

                            //click on Reject
                            driver.FindElement(OR.GetElement("UserGeneratedContent", "RejectButton", "TVAdminPortalOR.xml", i)).Click();

                            //driver.SwitchTo().Alert();
                            //VerifySuccessBannerMessage("Record deleted successfully");

                            //////////// new added /////////////
                            iWait.Until(ExpectedConditions.ElementIsVisible((OR.GetElement("VideoRequestByUser", "WarningYesButton", "TVAdminPortalOR.xml"))));

                            driver.FindElement((OR.GetElement("VideoRequestByUser", "WarningYesButton", "TVAdminPortalOR.xml"))).Click();

                            iWait.Until(ExpectedConditions.ElementIsVisible((OR.GetElement("SeriesManagement", "SuccessOkButton", "TVAdminPortalOR.xml"))));

                            driver.FindElement((OR.GetElement("SeriesManagement", "SuccessOkButton", "TVAdminPortalOR.xml"))).Click();
                            //////////// new added /////////////

                            //VerifySuccessBannerMessage("Record deleted successfully");

                            iWait.Until(ExpectedConditions.ElementIsVisible((OR.GetElement("VideoRequestByUser", "SearchVideoTB", "TVAdminPortalOR.xml"))));

                            driver.FindElement((OR.GetElement("VideoRequestByUser", "SearchVideoTB", "TVAdminPortalOR.xml"))).Clear();
                            driver.FindElement((OR.GetElement("VideoRequestByUser", "SearchVideoTB", "TVAdminPortalOR.xml"))).SendKeys(videoName);
                            Thread.Sleep(2000);
                            uf.scrollUp(driver);
                            //search the required video
                            driver.FindElement((OR.GetElement("VideoRequestByUser", "SearchButton", "TVAdminPortalOR.xml"))).Click();

                            OverlayWait();

                            #region verify status at admin table

                            log.Info("verify status at admin table");

                            iWait.Until(ExpectedConditions.ElementIsVisible((OR.GetElement("UserGeneratedContent", "VideoStatus", "TVAdminPortalOR.xml"))));

                            tblVideoListing = driver.FindElement((OR.GetElement("UserGeneratedContent", "VideoStatus", "TVAdminPortalOR.xml")));

                            iWait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy(By.TagName("tr")));

                            userContentRowList = (IList<IWebElement>)tblVideoListing.FindElements(By.TagName("tr"));

                            flag = false;

                            i = 0;

                            foreach (IWebElement currentRow1 in userContentRowList)
                            {

                                //Check Row that have class="GridRowStyle" or class="AltGridStyle"
                                if (currentRow1.GetAttribute("class").Equals("GridRowStyle") || currentRow1.GetAttribute("class").Equals("AltGridStyle"))
                                {

                                    columData = currentRow1.FindElements(By.TagName("td"))[3].FindElement(By.TagName("span")).Text.Trim();

                                    log.Info("Video Title from manage page::" + columData);

                                    if (columData.Equals(videoName))
                                    {
                                        flag = true;

                                        Assert.AreEqual("Rejected", currentRow1.FindElements(By.TagName("td"))[7].FindElement(By.TagName("span")).Text.Trim());
                                    }

                                    break;
                                }

                                i++;
                            }
                            #endregion


                        }
                        else
                        {
                            //click on Create
                            driver.FindElement(OR.GetElement("UserGeneratedContent", "CreateButton", "TVAdminPortalOR.xml", i)).Click();

                            iWait.Until(ExpectedConditions.ElementIsVisible((OR.GetElement("VideoManagement", "VidNumberTXT", "TVAdminPortalOR.xml"))));

                            //getting GUID of the current video
                            guid_Admin = driver.FindElement((OR.GetElement("VideoRequestByUser", "GuAdminId", "TVAdminPortalOR.xml"))).GetAttribute("value");

                            log.Info("Guid_Admin:: " + guid_Admin);

                            #region Enter abstract data
                            //Enter data into abstract field
                            iWait.Until(ExpectedConditions.ElementExists((OR.GetElement("VideoManagement", "Abstract", "TVAdminPortalOR.xml"))));

                            IWebElement abstract_frame = driver.FindElement((OR.GetElement("VideoManagement", "Abstract", "TVAdminPortalOR.xml")));

                            driver.SwitchTo().Frame(abstract_frame);

                            IWebElement editor_body = driver.FindElement(By.TagName("body"));

                            Thread.Sleep(3000);

                            OpenQA.Selenium.Interactions.Actions act = new OpenQA.Selenium.Interactions.Actions(driver);
                            act.SendKeys(editor_body, abstractContent).Build().Perform();
                            driver.SwitchTo().DefaultContent();

                            #endregion

                            objAdminVideoMngmnt.channelListTab();

                            objAdminVideoMngmnt.addcopyright();

                            log.Info("inside uploadBrowseVideo " + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

                            iWait.Until(ExpectedConditions.ElementIsVisible((OR.GetElement("VideoManagement", "UploadVidTab", "TVAdminPortalOR.xml"))));
                            //Click on uplaod video tab
                            driver.FindElement((OR.GetElement("VideoManagement", "UploadVidTab", "TVAdminPortalOR.xml"))).FindElement(By.TagName("a")).Click();

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

                                IWebElement videoPreviewButton = driver.FindElement((OR.GetElement("VideoManagement", "VidPreviewBTN", "TVAdminPortalOR.xml")));

                                //Click on the preview button
                                executor.ExecuteScript("arguments[0].click();", videoPreviewButton);
                            }

                            Assert.AreEqual("Status: READY", status);

                            Thread.Sleep(2000);

                            objAdminVideoMngmnt.finalPublishVideo("normal");

                            OverlayWait();

                            iWait.Until(ExpectedConditions.ElementIsVisible((OR.GetElement("VideoRequestByUser", "SearchVideoTB", "TVAdminPortalOR.xml"))));

                            driver.FindElement((OR.GetElement("VideoRequestByUser", "SearchVideoTB", "TVAdminPortalOR.xml"))).Clear();
                            driver.FindElement((OR.GetElement("VideoRequestByUser", "SearchVideoTB", "TVAdminPortalOR.xml"))).SendKeys(videoName);

                            //search the required video
                            driver.FindElement((OR.GetElement("VideoRequestByUser", "SearchButton", "TVAdminPortalOR.xml"))).Click();

                            OverlayWait();

                            #region verify status at admin table

                            log.Info("verify status at admin table");

                            iWait.Until(ExpectedConditions.ElementIsVisible((OR.GetElement("UserGeneratedContent", "VideoStatus", "TVAdminPortalOR.xml"))));

                            tblVideoListing = driver.FindElement((OR.GetElement("UserGeneratedContent", "VideoStatus", "TVAdminPortalOR.xml")));

                            iWait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy(By.TagName("tr")));

                            userContentRowList = (IList<IWebElement>)tblVideoListing.FindElements(By.TagName("tr"));

                            flag = false;

                            i = 0;

                            foreach (IWebElement currentRow1 in userContentRowList)
                            {

                                //Check Row that have class="GridRowStyle" or class="AltGridStyle"
                                if (currentRow1.GetAttribute("class").Equals("GridRowStyle") || currentRow1.GetAttribute("class").Equals("AltGridStyle"))
                                {

                                    columData = currentRow1.FindElements(By.TagName("td"))[3].FindElement(By.TagName("a")).Text.Trim();

                                    log.Info("Video Title from manage page::" + columData);

                                    if (columData.Equals(videoName))
                                    {
                                        flag = true;

                                        Assert.AreEqual("Published", currentRow1.FindElements(By.TagName("td"))[7].FindElement(By.TagName("span")).Text.Trim());
                                    }

                                    break;
                                }

                                i++;
                            }
                            #endregion

                            return guid_Admin;
                        }
                    }
                }
            }
            return null;
        }

        #endregion

        [Test]
        public void TVAdmin_001_UserGeneratedContentFunctionality()
        {
            try
            {
                log.Info("TVAdmin_001_UserGeneratedContentFunctionality test Started" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

                RedirectToUserGeneratedContent();

                uf.OpenNewTab(driver);

                log.Info("count ::: " + driver.WindowHandles.Count);

                String browsertype = uf.getRunningBrowser(driver, driver.FindElement(By.TagName("html")));

                uf.SwitchToWebTab(driver, browsertype);

                uf.NavigateWebPortal(cf, driver);

                objWebUserGenContent.RedirectToLogin();

                ClickYesButton();

                objWebUserGenContent.handlePromotionalPopup();

                objWebUserGenContent.HandleEmergencyPopUp();

                Thread.Sleep(2000);

                String videoRejectName = objWebUserGenContent.GenerateContent();
                Thread.Sleep(2000);

                String videoAcceptName = objWebUserGenContent.GenerateContent();
                Thread.Sleep(2000);

                uf.SwitchToAdminTab(driver, browsertype);

                driver.Navigate().Refresh();

                SearchRequiredVideoFromTable(videoRejectName, "reject");

                uf.SwitchToWebTab(driver, browsertype);

                driver.Navigate().Refresh();

                objWebUserGenContent.VerifyStatus(videoRejectName, "reject");

                objWebVideoMngmnt.searchVideo(videoRejectName);

                log.Info("Search the rejected video::");

                //verifying the video with no result found
                String NoResultFound = driver.FindElement((OR.GetElement("VideoLandingPage", "SpanResult", "TVWebPortalOR.xml"))).Text;

                Assert.AreEqual("No result found", NoResultFound);

                uf.SwitchToAdminTab(driver, browsertype);

                driver.Navigate().Refresh();

                guid_Admin = SearchRequiredVideoFromTable(videoAcceptName, "accept");

                uf.SwitchToWebTab(driver, browsertype);

                driver.Navigate().Refresh();


                log.Info("Inside redirectToUserGeneratedContent");

                Thread.Sleep(2000);

                iWait.Until(ExpectedConditions.ElementIsVisible((OR.GetElement("VideoRequestByUser", "MyIETDropDown", "TVWebPortalOR.xml"))));

                IWebElement myAccountDropDown = driver.FindElement((OR.GetElement("VideoRequestByUser", "MyIETDropDown", "TVWebPortalOR.xml")));

                executor.ExecuteScript("arguments[0].click()", myAccountDropDown);

                iWait.Until(ExpectedConditions.ElementIsVisible((OR.GetElement("UserGeneratedContent", "VideoContent", "TVWebPortalOR.xml"))));
                driver.FindElement((OR.GetElement("UserGeneratedContent", "VideoContent", "TVWebPortalOR.xml"))).FindElement(By.TagName("a")).Click();


                Thread.Sleep(2000);

                iWait.Until(ExpectedConditions.ElementIsVisible(By.Id("btnStatus")));

                driver.FindElement(By.Id("btnStatus")).Click();

                objWebUserGenContent.VerifyStatus(videoAcceptName, "accept");

                Thread.Sleep(150000);

                log.Info("Search the video::");

                // Search the video on Web Portal
                objWebVideoMngmnt.searchVideo(videoAcceptName);

                uf.isJqueryActive(driver);

                objWebVideoMngmnt.verifySearchedVideo(videoAcceptName, guid_Admin);

                log.Info("User is getting log out:::");

                objWebUserGenContent.LogOut();

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

        //[Test]
        //public void TVAdmin_001_UserGenContentRejectFunc()
        //{
        //    try
        //    {
        //        log.Info("TVAdmin_001_UserGenContentRejectFunc test Started" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

        //        RedirectToUserGeneratedContent();

        //        uf.OpenNewTab(driver);

        //        log.Info("count ::: " + driver.WindowHandles.Count);

        //        String browsertype = uf.getRunningBrowser(driver, driver.FindElement(By.TagName("html")));

        //        uf.SwitchToWebTab(driver, browsertype);

        //        uf.NavigateWebPortal(cf, driver);

        //        String videoName = objWebUserGenContent.GenerateContent();
        //        Thread.Sleep(2000);

        //        uf.SwitchToAdminTab(driver, browsertype);

        //        driver.Navigate().Refresh();

        //        SearchRequiredVideoFromTable(videoName, "reject");

        //        uf.SwitchToWebTab(driver, browsertype);

        //        driver.Navigate().Refresh();

        //        objWebUserGenContent.VerifyStatus(videoName, "reject");

        //        objWebVideoMngmnt.searchVideo(videoName);

        //        log.Info("Search the rejected video::");

        //        //verifying the video with no result found
        //        String NoResultFound = driver.FindElement((OR.GetElement("VideoLandingPage", "SpanResult", "TVWebPortalOR.xml"))).Text;

        //        Assert.AreEqual("No result found", NoResultFound);

        //        log.Info("User is getting log out:::");

        //        objWebUserGenContent.LogOut();
        //    }

        //    catch (Exception e)
        //    {
        //        Console.WriteLine(e.Message + "\n" + e.StackTrace + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());
        //        log.Error(e.Message + "\n" + e.StackTrace + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

        //        Assert.AreEqual(true, false);

        //    }
        //}

        //[Test]
        //public void TVAdmin_002_UserGenContentAcceptFunc()
        //{
        //    try
        //    {

        //        log.Info("TVAdmin_002_UserGenContentAcceptFunc test Started" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

        //        RedirectToUserGeneratedContent();

        //        uf.OpenNewTab(driver);

        //        log.Info("count ::: " + driver.WindowHandles.Count);

        //        String browsertype = uf.getRunningBrowser(driver, driver.FindElement(By.TagName("html")));

        //        uf.SwitchToWebTab(driver, browsertype);

        //        uf.NavigateWebPortal(cf, driver);

        //        String videoName = objWebUserGenContent.GenerateContent();
        //        Thread.Sleep(2000);

        //        uf.SwitchToAdminTab(driver, browsertype);

        //        driver.Navigate().Refresh();

        //        guid_Admin = SearchRequiredVideoFromTable(videoName, "accept");

        //        uf.SwitchToWebTab(driver, browsertype);

        //        driver.Navigate().Refresh();

        //        objWebUserGenContent.VerifyStatus(videoName, "accept");

        //        Thread.Sleep(150000);

        //        log.Info("Search the video::");

        //        // Search the video on Web Portal
        //        objWebVideoMngmnt.searchVideo(videoName);

        //        uf.isJqueryActive(driver);

        //        objWebVideoMngmnt.verifySearchedVideo(videoName, guid_Admin);

        //        log.Info("User is getting log out:::");

        //        objWebUserGenContent.LogOut();
        //    }



        //    catch (Exception e)
        //    {
        //        log.Error(e.Message + "\n" + e.StackTrace + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

        //        Assert.AreEqual(true, false);
        //    }
        //}

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








