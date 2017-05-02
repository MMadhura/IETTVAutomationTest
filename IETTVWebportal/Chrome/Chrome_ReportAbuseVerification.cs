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
    public class Chrome_ReportAbuseVerification
    {
        // This is to configure logger mechanism for Utilities.Config
        private readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        #region Variable Decration and object initialistaion

        internal IWebDriver driver = null;

        IWait<IWebDriver> iWait = null;
       
        string driverName = "", driverPath, appURL;
              
        IJavaScriptExecutor executor = null;

        Utility_Functions uf = new Utility_Functions();                          // Instantiate object for Utility Functons

        Configuration cf = new Configuration();                                  // Instantiate object for Configuration                               

        Object_Repository_Class OR = new Object_Repository_Class();             // Instantiate object for object repository

        IETTVWebportal.Reusable_Functions.Chrome_WebSetupTearDown st = new IETTVWebportal.Reusable_Functions.Chrome_WebSetupTearDown();             // Instantiate object for Chrome Setup Teardown

        string browserType = null;

        Boolean isCommentPage = false;

        Boolean flag = false;

        String columData = null;

        #endregion

        #region Constructors

        public Chrome_ReportAbuseVerification()
        {

        }

        public Chrome_ReportAbuseVerification(IWebDriver driver, ILog log1, IJavaScriptExecutor Executor, IWait<IWebDriver> iWait)
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
            iWait.Until(d => d.FindElement((OR.GetElement("SeriesManagement", "OverlayWait", "TVWebPortalOR.xml"))).GetCssValue("display").Equals("none"));
        }

        public void RedirectToLogin()
        {
            uf.isJqueryActive(driver);

            iWait.Until(ExpectedConditions.ElementExists((OR.GetElement("NonIETMemberRegistration", "LoginLink", "TVWebPortalOR.xml"))));

            IWebElement loginLink = driver.FindElement((OR.GetElement("NonIETMemberRegistration", "LoginLink", "TVWebPortalOR.xml")));

            executor.ExecuteScript("arguments[0].click();", loginLink);

        //    List<String> credential = cf.readSysConfigFile("WebPortal", "IndividualUser", "SysConfig.xml");

         //   log.Info("username    " + credential.ElementAt(0).ToString());

          //  log.Info("Password    " + credential.ElementAt(1).ToString());

            log.Info("username    " + cf.readingXMLFile("WebPortal", "InstitutionUser", "instUserName", "Config.xml"));

            log.Info("Password    " + cf.readingXMLFile("WebPortal", "InstitutionUser", "instPassWord", "Config.xml"));

            iWait.Until(ExpectedConditions.ElementIsVisible((OR.GetElement("VideoRequestByUser", "UserNameTB", "TVWebPortalOR.xml"))));

            driver.FindElement((OR.GetElement("VideoRequestByUser", "UserName", "TVWebPortalOR.xml"))).SendKeys(cf.readingXMLFile("WebPortal", "InstitutionUser", "instUserName", "Config.xml"));
            Thread.Sleep(1000);

            driver.FindElement((OR.GetElement("VideoRequestByUser", "Password", "TVWebPortalOR.xml"))).SendKeys(cf.readingXMLFile("WebPortal", "InstitutionUser", "instPassWord", "Config.xml"));

            iWait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy((OR.GetElement("VideoRequestByUser", "Login", "TVWebPortalOR.xml"))));

            driver.FindElement((OR.GetElement("VideoRequestByUser", "Login", "TVWebPortalOR.xml"))).Click();
            
            uf.isJqueryActive(driver);

            //OverlayWait();
            Thread.Sleep(2000);

            log.Info("already logged in count  : " + driver.FindElements((OR.GetElement("VideoRequestByUser", "YesButton", "TVWebPortalOR.xml"))).Count);
            //if user is already logged in
            if (driver.FindElements((OR.GetElement("VideoRequestByUser", "YesButton", "TVWebPortalOR.xml"))).Count > 0)
            {
                driver.FindElement((OR.GetElement("VideoRequestByUser", "YesButton", "TVWebPortalOR.xml"))).Click();
            }

            iWait.Until(ExpectedConditions.ElementIsVisible((OR.GetElement("VideoRequestByUser", "LoginWelcomeMessage", "TVWebPortalOR.xml"))));
            
            //Handling pop up message
            iWait.Until(ExpectedConditions.ElementIsVisible((OR.GetElement("ReportAbuse", "PopupMessage", "TVWebPortalOR.xml"))));  // Waiting for Popup window to appear after clicking on accept button

            IList<IWebElement> btnOK = driver.FindElements((OR.GetElement("ReportAbuse", "PopupMessage", "TVWebPortalOR.xml")));

            IWebElement element = btnOK.ElementAt(0);

            executor.ExecuteScript("arguments[0].click();", element);
   

        }

        //This function will move the control to Category Management Page
        public void RedirectingToCommentsManagement()
        {

            iWait.Until(ExpectedConditions.ElementIsVisible((OR.GetElement("VideoRequestByUser", "AdminMenu", "TVAdminPortalOR.xml"))));

            //clicking on Admin dropdown
            driver.FindElement((OR.GetElement("VideoRequestByUser", "AdminMenu", "TVAdminPortalOR.xml"))).Click();

            Thread.Sleep(3000);

            iWait.Until(ExpectedConditions.ElementToBeClickable((OR.GetElement("ReportAbuse", "CommentsManagementLink", "TVWebPortalOR.xml"))));

            //Clicking on Comments Management
            driver.FindElement((OR.GetElement("ReportAbuse", "CommentsManagementLink", "TVWebPortalOR.xml"))).Click();

            iWait.Until(ExpectedConditions.ElementIsVisible((OR.GetElement("ReportAbuse", "DivH2", "TVWebPortalOR.xml"))));
            isCommentPage = driver.FindElement((OR.GetElement("ReportAbuse", "DivH2", "TVWebPortalOR.xml"))).Displayed;

            //Checking whether the user is on Category page
            Assert.AreEqual(true, isCommentPage);

            //verify the Two tabs appearing on the page
            Thread.Sleep(3000);
            Assert.AreEqual("active", driver.FindElement((OR.GetElement("ReportAbuse", "CreateCategory", "TVWebPortalOR.xml"))).GetAttribute("class"));
            Assert.AreEqual(String.Empty, driver.FindElement((OR.GetElement("ReportAbuse", "ServiceList", "TVWebPortalOR.xml"))).GetAttribute("class"));


        }


        public void VideoSearch(String searchType, String searchText, String videoName)
        {

            iWait.Until(ExpectedConditions.ElementExists((OR.GetElement("ReportAbuse", "SearchTypeDDL", "TVWebPortalOR.xml"))));

            SelectElement videoSearchType = new SelectElement(driver.FindElement((OR.GetElement("ReportAbuse", "SearchTypeDDL", "TVWebPortalOR.xml"))));
            videoSearchType.SelectByText(searchType);

            VideoSearchText().Clear();
            VideoSearchText().SendKeys(searchText);

            //Click on search button
            VideoSearchButton().Click();

            OverlayWait();

            //NSoup to parse the code of Page.
            Document doc = NSoup.NSoupClient.Parse(driver.PageSource);

            iWait.Until(ExpectedConditions.ElementIsVisible((OR.GetElement("ReportAbuse", "VideoListingSearch", "TVWebPortalOR.xml"))));

            Elements rowListNsoup = doc.GetElementById(OR.readingXMLFile("ReportAbuse", "VideoListingSearch", "TVWebPortalOR.xml")).GetElementsByTag("tr");

            // Retreving all the rows of Manage Table 
            IList<IWebElement> rowListSelenium = (IList<IWebElement>)driver.FindElement((OR.GetElement("ReportAbuse", "VideoListingSearch", "TVWebPortalOR.xml"))).FindElements(By.TagName("tr"));

            flag = false;

            int rowcounter = 0;
            foreach (Element currentRow in rowListNsoup)
            {
                Attributes attr = currentRow.Attributes;

                //Row that have class="GridRowStyle" or class="AltGridStyle"
                if (attr["class"].Equals("GridRowStyle") || attr["class"].Equals("AltGridStyle"))
                {
                    log.Info("Row Counter :: " + rowcounter + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

                    columData = currentRow.GetElementsByTag("td")[1].GetElementById("ContentPlaceHolder1_grdVideoListingSearch_lnkTitleSearch_" + rowcounter).OwnText().Trim();


                    if (columData.Equals(videoName))
                    {
                        flag = true;
                        iWait.Until(ExpectedConditions.ElementIsVisible(OR.GetElement("ReportAbuse", "VideoListingSearchSelect", "TVWebPortalOR.xml",rowcounter)));

                        //click on Select Button
                        rowListSelenium[rowcounter + 1].FindElements(By.TagName("td"))[0].FindElement(OR.GetElement("ReportAbuse", "VideoListingSearchSelect", "TVWebPortalOR.xml",rowcounter)).Click();

                        OverlayWait();

                        iWait.Until(ExpectedConditions.ElementIsVisible((OR.GetElement("ReportAbuse", "SearchBTN", "TVWebPortalOR.xml"))));
                        log.Info("Clicked on select Button: " + rowcounter);

                        break;
                    }
                    rowcounter++;
                }
            }


        }

        //This Function hide the comment and verify the same 
        public void HideTest(int expectedCommentCount, String expectedluser, String expectedcommentText, String expectedDate)
        {
            log.Info("HideTest  started" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

            //  Click on show reported comment checkbox
            iWait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy((OR.GetElement("ReportAbuse", "CheckBox", "TVWebPortalOR.xml"))));

            driver.FindElement((OR.GetElement("ReportAbuse", "CheckBox", "TVWebPortalOR.xml"))).Click();

            OverlayWait();

            //get comment details

            log.Info("Getting Comment Details:::::");

            IList<IWebElement> commentDetails = CommentCount();
            int actualCommentCount = commentDetails.Count;

            log.Info("Verify commnet details of web portal in admin portal");

            Boolean checkboxStatus = IsElementChecked("ContentPlaceHolder1_RepterDetails_checkststus_0");

            //verify hide button
            Assert.AreEqual("Hide", HideNShowButton().GetAttribute("value"));

            //click on hide button
            Thread.Sleep(2000);
            executor.ExecuteScript("window.scrollTo(0, document.body.scrollHeight)", "");
            executor.ExecuteScript("window.scrollTo(0, 400)", "");
            HideNShowButton().Click();

           // Thread.Sleep(2000);
            Assert.AreEqual("Show", HideNShowButton().GetAttribute("value"));

            //click on save button
            CommentSaveButton().Click();

            SuccessBannerMessage();

            log.Info("HideTest  completed" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());


        }

        //This function perform show comment operation on hide record
        public void ShowButtonTest()
        {
            HideNShowButton().Click();

            CommentSaveButton().Click();

            SuccessBannerMessage();
        }

        public IWebElement HideNShowButton()
        {
            iWait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy((OR.GetElement("ReportAbuse", "HideBTN", "TVWebPortalOR.xml"))));
            return driver.FindElement((OR.GetElement("ReportAbuse", "HideBTN", "TVWebPortalOR.xml")));
        }

        public IWebElement CommentSaveButton()
        {
            iWait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy((OR.GetElement("ReportAbuse", "SaveBTN", "TVWebPortalOR.xml"))));
            return driver.FindElement((OR.GetElement("ReportAbuse", "SaveBTN", "TVWebPortalOR.xml")));
        }

        public void SuccessBannerMessage()
        {

            iWait.Until(ExpectedConditions.ElementIsVisible((OR.GetElement("ReportAbuse", "OkBTN", "TVWebPortalOR.xml"))));

            Assert.AreEqual("Comments Updated successfully.".Trim(), driver.FindElement((OR.GetElement("ReportAbuse", "SuccessMsg", "TVWebPortalOR.xml"))).Text.Trim());

            driver.FindElement((OR.GetElement("ReportAbuse", "OkBTN", "TVWebPortalOR.xml"))).Click();

            OverlayWait();
        }

        public IWebElement VideoSearchText()
        {
            iWait.Until(ExpectedConditions.ElementExists((OR.GetElement("ReportAbuse", "CreateCategory", "TVWebPortalOR.xml"))));

            return driver.FindElement((OR.GetElement("ReportAbuse", "SearchTXT", "TVWebPortalOR.xml")));
        }

        public IWebElement VideoSearchButton()
        {
            iWait.Until(ExpectedConditions.ElementExists((OR.GetElement("ReportAbuse", "SearchBTN", "TVWebPortalOR.xml"))));

            return driver.FindElement((OR.GetElement("ReportAbuse", "SearchBTN", "TVWebPortalOR.xml")));
        }

        public IList<IWebElement> CommentCount()
        {
            iWait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy((OR.GetElement("ReportAbuse", "CommentDetails", "TVWebPortalOR.xml"))));

            IList<IWebElement> commentDetails = driver.FindElement((OR.GetElement("ReportAbuse", "CommentDetails", "TVWebPortalOR.xml"))).FindElements((OR.GetElement("ReportAbuse", "DivClearFix", "TVWebPortalOR.xml")));

            return commentDetails;
        }

        public Boolean IsElementChecked(String checkboxID)
        {

            Boolean checkboxStatus = (Boolean)executor.ExecuteScript(" return document.getElementById('" + checkboxID + "').checked");

            log.Info("Status of element Checked :: " + checkboxStatus + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

            return checkboxStatus;

        }

        #endregion

        #region Report Abuse Test
        public void ReportAbuseTest(string videoname, string guid_Admin)
        {
            try
            {
                String commentContent = "Report Abuse Test";

                log.Info("ReportAbuseTest::::");

                uf.isJqueryActive(driver);

                RedirectToLogin();

                log.Info("user successfully logged in");

                Boolean flag = false;

                //wait till jquery gets completed
                uf.isJqueryActive(driver);

                log.Info("searchVideo::::");

                iWait.Until(ExpectedConditions.ElementExists((OR.GetElement("VideoLandingPage", "SearchTB", "TVWebPortalOR.xml"))));

                //search the required video
                IWebElement SearchTextField = driver.FindElement((OR.GetElement("VideoLandingPage", "SearchTB", "TVWebPortalOR.xml")));
                SearchTextField.SendKeys(videoname);

                iWait.Until(ExpectedConditions.ElementToBeClickable((OR.GetElement("VideoLandingPage", "SearchIcon", "TVWebPortalOR.xml"))));

                //Click on searchIcon
                IWebElement SearchIcon = driver.FindElement((OR.GetElement("VideoLandingPage", "SearchIcon", "TVWebPortalOR.xml")));
                executor.ExecuteScript("arguments[0].click()", SearchIcon);
//                SearchIcon.Click();
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


                iWait.Until(ExpectedConditions.ElementExists((OR.GetElement("ReportAbuse", "Comment", "TVWebPortalOR.xml"))));

                var elem = driver.FindElement((OR.GetElement("ReportAbuse", "Comment", "TVWebPortalOR.xml")));
                ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(true);", elem);
                Thread.Sleep(500);


                #region Insert comment

                log.Info("inserting comment:::");

                driver.FindElement((OR.GetElement("ReportAbuse", "Comment", "TVWebPortalOR.xml"))).SendKeys(commentContent);

                //Click on submit buton
                driver.FindElement((OR.GetElement("ReportAbuse", "SubmitComment", "TVWebPortalOR.xml"))).Click();
                uf.isJqueryActive(driver);

                Assert.AreEqual(commentContent, driver.FindElement((OR.GetElement("ReportAbuse", "DivVidComments", "TVWebPortalOR.xml"))).Text.Trim());

                //Click on report abuse
                driver.FindElement((OR.GetElement("ReportAbuse", "SpanVidComments", "TVWebPortalOR.xml"))).Click();

                #endregion

                #region verify banner message
              
                log.Info("verify banner message:::");

                iWait.Until(ExpectedConditions.ElementIsVisible((OR.GetElement("VideoLandingPage", "InfoMessage", "TVWebPortalOR.xml"))));

                Assert.AreEqual(" You have successfully reported this comment for abuse.OK".Trim(), driver.FindElement((OR.GetElement("VideoLandingPage", "InfoMsg", "TVWebPortalOR.xml"))).Text.Trim());

                driver.FindElement((OR.GetElement("ReportAbuse", "InfoMessageButton", "TVWebPortalOR.xml"))).Click();

                #endregion

                #region Get all comments contents


                executor.ExecuteScript("window.scrollTo(0, document.body.scrollHeight)", "");
                executor.ExecuteScript("window.scrollTo(0, 400)", "");

                Thread.Sleep(3000);

                IList<IWebElement> allCommentContentRow = driver.FindElement((OR.GetElement("ReportAbuse", "VideoComments", "TVWebPortalOR.xml"))).FindElements((OR.GetElement("ReportAbuse", "CommentsHeading", "TVWebPortalOR.xml")));

                int expectedCommentCount = allCommentContentRow.Count;

                String expectedluser = allCommentContentRow[0].FindElement((OR.GetElement("ReportAbuse", "DivHeading", "TVWebPortalOR.xml"))).Text.Trim();

                String expectedcommentText = allCommentContentRow[0].FindElement((OR.GetElement("ReportAbuse", "DivParagraph", "TVWebPortalOR.xml"))).Text.Trim();

                String expectedDate = allCommentContentRow[0].FindElement((OR.GetElement("ReportAbuse", "DivWhitSpace", "TVWebPortalOR.xml"))).Text.Trim();


                #endregion

                #region Verify comment in admin portal

                log.Info("calling admin portal");

                browserType = uf.getRunningBrowser(driver, driver.FindElement(By.TagName("html")));

                uf.SwitchToAdminTab(driver, browserType);

                RedirectingToCommentsManagement();

                log.Info("videoTitle  :  " + videoname);

                VideoSearch("Title", videoname, videoname);

                HideTest(expectedCommentCount, expectedluser, expectedcommentText, expectedDate);
                #endregion

                #region Verify comment is Hiden or not in Web Portal
                //Verify comment is Hiden or not

                uf.SwitchToWebTab(driver, browserType);

                log.Info("Verify comment is Hiden or not");

                driver.Navigate().Refresh();

                uf.isJqueryActive(driver);


                executor.ExecuteScript("window.scrollTo(0, document.body.scrollHeight)", "");
                executor.ExecuteScript("window.scrollTo(0, 400)", "");

                Thread.Sleep(3000);

                Thread.Sleep(5000);
                allCommentContentRow = driver.FindElement((OR.GetElement("ReportAbuse", "VideoComments", "TVWebPortalOR.xml"))).FindElements((OR.GetElement("ReportAbuse", "CommentsHeading", "TVWebPortalOR.xml")));

                Assert.AreEqual(0, allCommentContentRow[0].FindElements((OR.GetElement("ReportAbuse", "DivParagraph", "TVWebPortalOR.xml"))).Count);
                #endregion

                #region verify comment is shown or not in Web Portal

                uf.SwitchToAdminTab(driver, browserType);

                //verify comment is shown or not
                log.Info("Verify comment is shown or not");

                VideoSearch("Title", videoname, videoname);

                ShowButtonTest();

                uf.isJqueryActive(driver);

                uf.SwitchToWebTab(driver, browserType);

                driver.Navigate().Refresh();

                uf.isJqueryActive(driver);

                allCommentContentRow = driver.FindElement((OR.GetElement("ReportAbuse", "VideoComments", "TVWebPortalOR.xml"))).FindElements((OR.GetElement("ReportAbuse", "CommentsHeading", "TVWebPortalOR.xml")));

                Assert.AreEqual(1, allCommentContentRow[0].FindElements((OR.GetElement("ReportAbuse", "DivParagraph", "TVWebPortalOR.xml"))).Count);
                #endregion

                
            }
            catch (Exception e)
            {
                log.Error(e.Message + "\n" + e.StackTrace + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

                Console.WriteLine(e.Message + "\n" + e.StackTrace + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

                Assert.AreEqual(true, false);
            }
        }
        #endregion

        [TestFixtureTearDown]
        public void TearDown()
        {
            st.Chrome_TearDown(driver, log);                        // Calling Chrome Teardown
        }

    }

}
