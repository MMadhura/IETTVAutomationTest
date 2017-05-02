//using NUnit.Framework;
//using OpenQA.Selenium;
//using OpenQA.Selenium.Chrome;
//using OpenQA.Selenium.Firefox;
//using OpenQA.Selenium.IE;
//using OpenQA.Selenium.Interactions;
//using OpenQA.Selenium.Support.UI;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading;
//using System.Threading.Tasks;
//using System.Windows.Forms;
//using System.Drawing;
//using Utility_Classes;
//using Utilities.Config;
//using Utilities.Object_Repository;
//using log4net;
//using log4net.Config;
//using System.Reflection;
//using System.IO;
//using System.Diagnostics;
//using NSoup.Nodes;  // for nsoup documents
//using NSoup.Select;  //for nsoup element
//using IETTVAdminPortal.IE;
//using IETTVWebportal.Reusable_Functions;


//namespace IETTVWebportal.IE
//{
//    [TestFixture]
//    class IE_VideoLandingPage
//    {
//        // This is to configure logger mechanism for Utilities.Config
//        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

//        #region Variable Decration and object initialistaion

//        internal IWebDriver driver = null;

//        IWait<IWebDriver> iWait = null;

//        string driverName = "", driverPath, appURL;

//        String columData = null;

//        Boolean flag;

//        IJavaScriptExecutor executor = null;

//        Utility_Functions uf = new Utility_Functions();                                         // Instantiate object for Utility Functons

//        Configuration cf = new Configuration();                                                 // Instantiate object for Configuration

//        Object_Repository_Class or = new Object_Repository_Class();                             // Instantiate object for object repository

//        // Instantiate object of admin portal
//        IE_CommentsManagementVerification commentAdmin = new IE_CommentsManagementVerification();

//        IE_WebSetupTearDown st = new IE_WebSetupTearDown();                                     // Instantiate object for IE Setup Teardown

//        #endregion


//        #region SetUp

//        [TestFixtureSetUp]
//        public void FixtureSetUp()
//        {
//            log4net.Config.XmlConfigurator.Configure(new FileInfo(AppDomain.CurrentDomain.BaseDirectory + "WebPortal.config"));

//            log.Info("Inside Fixture Setup of IE - BottomBar Verification Test" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

//            uf.CreateOrReplaceVideoFolder();

//            string baseDir = AppDomain.CurrentDomain.BaseDirectory;

//            driverName = "webdriver.ie.driver";                                                // Driver name for IE

//            driverPath = baseDir + "/IEDriverServer.exe";                                      // path for IE Driver

//            System.Environment.SetEnvironmentVariable(driverName, driverPath);

//            InternetExplorerOptions opt = new InternetExplorerOptions();                       // Ensuring Clean IE session

//            opt.EnsureCleanSession = true;

//            driver = new InternetExplorerDriver(opt);                                          // Initialize IE driver              

//            if (uf.getRunningBrowser(driver, driver.FindElement(By.TagName("html"))).ToString().Equals("IE11"))
//            {

//                log.Info("IE11 detected in setup" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

//                // Check if Registry Entry is present for IE 11 browser

//                if (uf.checkIE11RegistryPresence().Equals("true"))
//                {
//                    log.Info("Registry Created successfully / Present for IE 11" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());
//                }
//                else
//                {
//                    log.Info("Registry couldn't be created. Test may not run properly in IE 11. Please contact administrator" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());
//                }

//            }

//            iWait = new WebDriverWait(driver, TimeSpan.FromSeconds(60));

//            executor = (IJavaScriptExecutor)driver;
//        }


//        [SetUp]
//        public void SetUp()
//        {
//            appURL = st.IE_Setup(driver, log, executor);
//        }

//        #endregion


//        #region Reusable Function

//        //This function wait invisibility of overlay class
//        public void OverlayWait()
//        {
//            iWait.Until(d => d.FindElement(By.ClassName("overlay")).GetAttribute("class").Contains("display-block"));
//        }


//        public void RedirectToVideoLandingPage()
//        {
//            uf.isJqueryActive(driver);

//            iWait.Until(ExpectedConditions.ElementExists(By.Id("searchtextbox")));

//            //search the required video

//            IWebElement SearchTextField = driver.FindElement(By.Id("searchtextbox"));

//            // Get the Video management node list   

//            List<String> videoname = cf.readSysConfigFile("AdminPortal", "VideoManagement", "SysConfig.xml");

//            log.Info("videoname    " + videoname.ElementAt(0).ToString());

//            SearchTextField.SendKeys(videoname.ElementAt(0).ToString());

//            iWait.Until(ExpectedConditions.ElementToBeClickable(By.Id("searchicon")));

//            //Click on searchIcon

//            IWebElement SearchIcon = driver.FindElement(By.Id("searchicon"));

//            SearchIcon.Click();

//            uf.isJqueryActive(driver);

//            IWebElement searchresultDetails = driver.FindElement(By.CssSelector("div.video-description-clear-both > span > a"));

//            IWebElement serachResultLink = driver.FindElement(By.CssSelector("div.margin-left-148.margin-right.video-description-clear-both > span > a"));

//            String webvideoTitle = searchresultDetails.Text.Trim();

//            log.Info("Web Video Title:" + webvideoTitle + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

//            // Assert.AreEqual(webvideoTitle, videoname.ElementAt(0).ToString());

//            serachResultLink.Click();

//            uf.isJqueryActive(driver);
//        }

//        public void RedirectToLogin()
//        {
//            uf.isJqueryActive(driver);

//            iWait.Until(ExpectedConditions.ElementExists(By.LinkText("Please Log in")));

//            IWebElement loginLink = driver.FindElement(By.LinkText("Please Log in"));

//            executor.ExecuteScript("arguments[0].click();", loginLink);

//            List<String> credential = cf.readSysConfigFile("WebPortal", "NonIETRegistration", "SysConfig.xml");

//            log.Info("username    " + credential.ElementAt(0).ToString());

//            log.Info("Password    " + credential.ElementAt(1).ToString());

//            iWait.Until(ExpectedConditions.ElementIsVisible(By.Id("usernameText")));

//            driver.FindElement(By.Id("UserName")).SendKeys(credential.ElementAt(0).ToString());
//            Thread.Sleep(1000);

//            driver.FindElement(By.Id("Password")).SendKeys(credential.ElementAt(1).ToString());

//            iWait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy(By.Id("login")));

//            driver.FindElement(By.Id("login")).Click();

//            OverlayWait();

//            // iWait.Until(ExpectedConditions.ElementIsVisible(By.Id("yesButtonId")));

//            log.Info("already logged in count  : " + driver.FindElements(By.Id("yesButtonId")).Count);
//            //if user is already logged in
//            if (driver.FindElements(By.Id("yesButtonId")).Count > 0)
//            {
//                driver.FindElement(By.Id("yesButtonId")).Click();
//            }


//            //Handling pop up message
//            iWait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("div.modal-content > div.modal-footer > div > button.btn.btn1.btn-success.ok_btn_size")));  // Waiting for Popup window to appear after clicking on Login button

//            IList<IWebElement> btnOK = driver.FindElements(By.CssSelector("div.modal-content > div.modal-footer > div > button.btn.btn1.btn-success.ok_btn_size"));

//            IWebElement element = btnOK.ElementAt(0);

//            executor.ExecuteScript("arguments[0].click();", element);


//        }

//        #endregion

//        [Test]
//        public void TVWeb_001_VerifyCommentReport()
//        {
//            try
//            {
//                log.Info("TVWeb_001_VerifyCommentReport  started" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

//                commentAdmin.FixtureSetUp();
//                commentAdmin.SetUp();
//                String generateTagName = commentAdmin.AddNewKeyword();

//                RedirectToLogin();

//                log.Info("user successfully logged in");

//                RedirectToVideoLandingPage();

//                iWait.Until(ExpectedConditions.ElementExists(By.Id("comment")));

//                var elem = driver.FindElement(By.Id("comment"));
//                ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(true);", elem);
//                Thread.Sleep(500);

//                driver.FindElement(By.Id("comment")).SendKeys(generateTagName);

//                //click on submit buton

//                driver.FindElement(By.Id("commentsubmit")).Click();

//                iWait.Until(ExpectedConditions.ElementIsVisible(By.Id("InfoMessageId")));

//                Assert.AreEqual(" Your comments consist of restricted words and hence cannot be posted.OK".Trim(), driver.FindElement(By.ClassName("infomsg")).Text.Trim());

//                driver.FindElement(By.Id("InfoMessageId")).Click();

//                Thread.Sleep(1000);

//                commentAdmin.RemoveTag(generateTagName);

//                log.Info("TVWeb_001_VerifyCommentReport completed" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());
//            }



//            catch (Exception e)
//            {
//                log.Error(e.Message + "\n" + e.StackTrace + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

//                Assert.AreEqual(true, false);
//            }
//        }

//        [Test]
//        public void TVWeb_002_InvalidTest()
//        {
//            try
//            {
//                log.Info("TVWeb_002_InvalidTest  started" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

//                RedirectToLogin();

//                log.Info("user successfully logged in");

//                RedirectToVideoLandingPage();

//                iWait.Until(ExpectedConditions.ElementExists(By.Id("comment")));

//                var elem = driver.FindElement(By.Id("comment"));
//                ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(true);", elem);
//                Thread.Sleep(500);

//                //click on submit buton

//                driver.FindElement(By.Id("commentsubmit")).Click();

//                iWait.Until(ExpectedConditions.ElementIsVisible(By.Id("InfoMessageId")));

//                Assert.AreEqual(" Please define the comment.OK".Trim(), driver.FindElement(By.ClassName("infomsg")).Text.Trim());

//                log.Info("TVWeb_002_InvalidTest  completed" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

//                commentAdmin.TearDown();
            
//            }

//            catch (Exception e)
//            {
//                log.Error(e.Message + "\n" + e.StackTrace + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

//                Assert.AreEqual(true, false);
//            }

//        }

//        [Test]
//        public void TVWeb_003_ReportCommentTest()
//        {
//            try
//            {

//                String commentContent = "commnetContentTest";

//                log.Info("TVWeb_003_ReportCommentTest  started" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

//                RedirectToLogin();

//                log.Info("user successfully logged in");

//                RedirectToVideoLandingPage();

//                iWait.Until(ExpectedConditions.ElementExists(By.Id("comment")));

//                var elem = driver.FindElement(By.Id("comment"));
//                ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(true);", elem);
//                Thread.Sleep(500);


//                #region Insert comment

//                driver.FindElement(By.Id("comment")).SendKeys(commentContent);

//                //Click on submit buton
//                driver.FindElement(By.Id("commentsubmit")).Click();
//                uf.isJqueryActive(driver);

//                //Getting comment time
//                String commentTime = driver.FindElement(By.CssSelector("div.video-comments > div > div > span.white-space")).Text.Trim();

//                Assert.AreEqual(commentContent, driver.FindElement(By.CssSelector("div.video-comments > div > div > p")).Text.Trim());

//                //Click on report abuse
//                driver.FindElement(By.CssSelector("div.video-comments > div > div > a > span")).Click();

//                #endregion


//                #region verify banner message
//                iWait.Until(ExpectedConditions.ElementIsVisible(By.Id("InfoMessageId")));

//                Assert.AreEqual(" You have successfully reported this comment for abuse.OK".Trim(), driver.FindElement(By.ClassName("infomsg")).Text.Trim());

//                driver.FindElement(By.CssSelector("div.infomsg  > button#InfoMessageId")).Click();

//                #endregion

//                #region Get all comments contents

//                IList<IWebElement> allCommentContentRow = driver.FindElement(By.ClassName("video-comments")).FindElements(By.ClassName("comment-heading"));

//                int expectedCommentCount = allCommentContentRow.Count;

//                String expectedluser = allCommentContentRow[0].FindElement(By.CssSelector("div > span.content-heading")).Text.Trim();

//                String expectedcommentText = allCommentContentRow[0].FindElement(By.CssSelector("div > p")).Text.Trim();

//                String expectedDate = allCommentContentRow[0].FindElement(By.CssSelector("div > span.white-space")).Text.Trim();


//                #endregion

//                #region Verify comment in admin portal

//                log.Info("calling admin portal");

//                commentAdmin.FixtureSetUp();

//                commentAdmin.SetUp();

//                commentAdmin.RedirectingToCommentsManagement();

//                List<String> videoname = cf.readSysConfigFile("AdminPortal", "VideoManagement", "SysConfig.xml");

//                String videoTitle = videoname.ElementAt(0).ToString();

//                log.Info("videoTitle  :  " + videoTitle);

//                commentAdmin.VideoSearch("Title", videoTitle, videoTitle);

//                commentAdmin.HideTest(expectedCommentCount, expectedluser, expectedcommentText, expectedDate);


//                #endregion


//                //Verify comment is Hiden or not
//                log.Info("Verify comment is Hiden or not");

//                driver.Navigate().Refresh();

//                uf.isJqueryActive(driver);

//                allCommentContentRow = driver.FindElement(By.ClassName("video-comments")).FindElements(By.ClassName("comment-heading"));

//                Assert.AreEqual(0, allCommentContentRow[0].FindElements(By.CssSelector("div > p")).Count);


//                //verify comment is shown or not
//                log.Info("Verify comment is shown or not");

//                commentAdmin.VideoSearch("Title", videoTitle, videoTitle);

//                commentAdmin.ShowButtonTest();

//                driver.Navigate().Refresh();

//                uf.isJqueryActive(driver);

//                allCommentContentRow = driver.FindElement(By.ClassName("video-comments")).FindElements(By.ClassName("comment-heading"));

//                Assert.AreEqual(1, allCommentContentRow[0].FindElements(By.CssSelector("div > p")).Count);

//                log.Info("TVWeb_003_ReportCommentTest  completed" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

//                commentAdmin.TearDown();
            
//            }


//            catch (Exception e)
//            {
//                log.Error(e.Message + "\n" + e.StackTrace + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

//                Assert.AreEqual(true, false);
//            }

//        }

//        [Test]
//        public void TVWeb_004_CommentWithoutloggedIn()
//        {
//            try
//            {
//                log.Info("TVWeb_004_CommentWithoutloggedIn  started" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

//                uf.isJqueryActive(driver);

//                iWait.Until(ExpectedConditions.ElementExists(By.Id("searchtextbox")));

//                //search the required video

//                IWebElement SearchTextField = driver.FindElement(By.Id("searchtextbox"));

//                // Get the Video management node list   

//                List<String> videoname = cf.readSysConfigFile("AdminPortal", "VideoManagement", "SysConfig.xml");

//                log.Info("videoname    " + videoname.ElementAt(0).ToString());

//                SearchTextField.SendKeys(videoname.ElementAt(0).ToString());

//                iWait.Until(ExpectedConditions.ElementToBeClickable(By.Id("searchicon")));

//                //Click on searchIcon

//                IWebElement SearchIcon = driver.FindElement(By.Id("searchicon"));

//                SearchIcon.Click();

//                //Handling pop up message
//                iWait.Until(ExpectedConditions.ElementIsVisible(By.Id("div_emergency")));  // Waiting for Popup window to appear after clicking on accept button

//                IList<IWebElement> btnOK = driver.FindElements(By.CssSelector("div.modal-content > div.modal-footer > div.text-center > button.btn.btn1.btn-success.ok_btn_size "));

//                IWebElement element = btnOK.ElementAt(0);

//                executor.ExecuteScript("arguments[0].click();", element);

//                IWebElement searchresultDetails = driver.FindElement(By.CssSelector("div.video-description-clear-both > span > a"));

//                String webvideoTitle = searchresultDetails.Text.Trim();

//                log.Info("Web Video Title:" + webvideoTitle + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

//                iWait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.Id("div_emergency")));  // Waiting for Popup window to appear 

//                IWebElement serachResultLink = driver.FindElement(By.CssSelector("div.margin-left-148.margin-right.video-description-clear-both > span > a"));

//                serachResultLink.Click();

//                uf.isJqueryActive(driver);

//                iWait.Until(ExpectedConditions.ElementExists(By.Id("comment")));

//                var elem = driver.FindElement(By.Id("comment"));
//                ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(true);", elem);
//                Thread.Sleep(500);

//                Assert.AreEqual(false, driver.FindElement(By.Id("comment")).Enabled);

//                Assert.AreEqual(false, driver.FindElement(By.Id("commentsubmit")).Enabled);

//                log.Info("TVWeb_004_CommentWithoutloggedIn  started" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

//            }


//            catch (Exception e)
//            {
//                log.Error(e.Message + "\n" + e.StackTrace + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

//                Assert.AreEqual(true, false);
//            }

//        }


//        [TestFixtureTearDown]
//        public void TearDown()
//        {
//            st.IE_TearDown(driver, log);
//        }

//    }

//}
