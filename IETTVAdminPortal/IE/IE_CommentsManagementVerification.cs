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
using Utility_Classes;
using Utilities.Config;
using System.Data;
using Microsoft.Office.Interop.Excel;
using System.IO;
using IETTVAdminportal.Reusable;
using NSoup.Nodes;  // for nsoup documents
using NSoup.Select;  //for nsoup element
using log4net;
using log4net.Config;
using System.Reflection;
using System.Drawing;
using System.Globalization;
using System.Diagnostics;
using System.Xml;


namespace IETTVAdminPortal.IE
{
   
    [TestFixture]
    public class IE_CommentsManagementVerification
    {

        // This is to configure logger mechanism for Utilities.Config
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        #region Variable_Declration

        IWait<IWebDriver> iWait = null;

        Boolean flag = false;

        IList<IWebElement> rowList = null;

        Boolean isCommentPage = false;

        SelectElement positionDropdown = null;

        String columData = null;

        string categoryNameWithGuid = "Cat";                            // This is the prefix for category

        String pageSourceCode = null;                                   // This is used for storing the page source code

        internal IWebDriver driver = null;

        IJavaScriptExecutor executor;

        string driverName = "", driverPath, appURL;

        Utility_Functions uf = new Utility_Functions();                  // Instantiate object for Utility Functons

        Configuration cf = new Configuration();                         // Instantiate object for Configuration
        
        GetMarkLogicData gm = new GetMarkLogicData();                 // Instantiate object for Marklogic

        IE_AdminSetupTearDown st = new IE_AdminSetupTearDown();       // Instantiate object for IE Setup Teardown

        #endregion

        #region Setup

        [TestFixtureSetUp]
        public void FixtureSetUp()
        {
            try
            {
                log4net.Config.XmlConfigurator.Configure(new FileInfo(AppDomain.CurrentDomain.BaseDirectory + "AdminPortal.config"));   //to configure the Logger functionality

                string baseDir = AppDomain.CurrentDomain.BaseDirectory;                                 // Get path till Base Directory

                driverName = "webdriver.ie.driver";                                                     // Driver name for Chrome

                driverPath = baseDir + "/IEDriverServer.exe";                                           // Path for IE Driver

                System.Environment.SetEnvironmentVariable(driverName, driverPath);

                InternetExplorerOptions opt = new InternetExplorerOptions();                            // Ensuring Clean IE session

                opt.EnsureCleanSession = true;

                driver = new InternetExplorerDriver(opt);                                               // Initialize IE driver  

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
            appURL = st.IE_Setup(driver, log, executor);                             // Calling IE Setup

        }

        #endregion

        #region Reusable elements


        #region Abuse Filtration Tab

        public IWebElement AbuseFiltrationTab()
        {
            iWait.Until(ExpectedConditions.ElementToBeClickable(By.Id("ServiceList")));

            return driver.FindElement(By.Id("ServiceList")).FindElement(By.TagName("a"));
        }

        public IWebElement KeywordsTextField()
        {
            iWait.Until(ExpectedConditions.ElementIsVisible(By.ClassName("tagBox-input")));

            // return driver.FindElement(By.CssSelector())
            return driver.FindElement(By.ClassName("tagBox-input"));
        }

        public IWebElement AddtagButton()
        {
            iWait.Until(ExpectedConditions.ElementToBeClickable(By.ClassName("tagBox-add-tag")));

            return driver.FindElement(By.ClassName("tagBox-add-tag"));
        }

        public IWebElement SaveButton()
        {
            iWait.Until(ExpectedConditions.ElementToBeClickable(By.Id("ContentPlaceHolder1_btnsave")));

            return driver.FindElement(By.Id("ContentPlaceHolder1_btnsave"));
        }

        public IWebElement ResetButton()
        {
            iWait.Until(ExpectedConditions.ElementToBeClickable(By.Id("ContentPlaceHolder1_btncancel")));

            return driver.FindElement(By.Id("ContentPlaceHolder1_btncancel"));
        }

        #endregion

        #region Comment Management Tab

        public IWebElement CommentsManagementTab()
        {
            iWait.Until(ExpectedConditions.ElementToBeClickable(By.Id("createCategory")));

            return driver.FindElement(By.Id("createCategory")).FindElement(By.TagName("a"));
        }

        public IWebElement VideoSearchText()
        {
            iWait.Until(ExpectedConditions.ElementExists(By.Id("createCategory")));

            return driver.FindElement(By.Id("ContentPlaceHolder1_txtSearch"));
        }

        public IWebElement VideoSearchButton()
        {
            iWait.Until(ExpectedConditions.ElementExists(By.Id("ContentPlaceHolder1_btnSearch")));

            return driver.FindElement(By.Id("ContentPlaceHolder1_btnSearch"));
        }

        public IWebElement HideNShowButton()
        {
            iWait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy(By.Id("ContentPlaceHolder1_RepterDetails_BtnHide_0")));
            return driver.FindElement(By.Id("ContentPlaceHolder1_RepterDetails_BtnHide_0"));
        }

        public IWebElement CommentSaveButton()
        {
            iWait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy(By.Id("ContentPlaceHolder1_btnsave1")));
            return driver.FindElement(By.Id("ContentPlaceHolder1_btnsave1"));
        }






        #endregion

        #endregion


        #region Reusable Function for this module

        //This function will move the control to Category Management Page
        public void RedirectingToCommentsManagement()
        {

            iWait.Until(ExpectedConditions.ElementIsVisible(By.Id("rptMenu_aMenuItem_0")));

            //clicking on Admin dropdown
            driver.FindElement(By.Id("rptMenu_aMenuItem_0")).Click();

            Thread.Sleep(3000);

            iWait.Until(ExpectedConditions.ElementToBeClickable(By.LinkText("Comments Management")));

            //Clicking on Category Management
            driver.FindElement(By.LinkText("Comments Management")).Click();

            iWait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("div.content_New > div  > div.row > h2")));
            isCommentPage = driver.FindElement(By.CssSelector("div.content_New > div  > div.row > h2")).Displayed;

            //Checking whether the user is on Category page
            Assert.AreEqual(true, isCommentPage);

            //verify the Two tabs appearing on the page
            Thread.Sleep(3000);
            Assert.AreEqual("active", driver.FindElement(By.Id("createCategory")).GetAttribute("class"));
            Assert.AreEqual(String.Empty, driver.FindElement(By.Id("ServiceList")).GetAttribute("class"));


        }

        //This function wait invisibility of overlay class
        public void OverlayWait()
        {
            iWait.Until(d => d.FindElement(By.ClassName("overlay")).GetCssValue("display").Equals("none"));
        }

        #region Abuse Filtration

        //This Function Add Keyword and verify the same
        public String AddNewKeyword()
        {
            log.Info("Inside AddNewKeyword function");

            RedirectingToCommentsManagement();

            AbuseFiltrationTab().Click();

            iWait.Until(ExpectedConditions.ElementToBeClickable(By.ClassName("tagBox-add-tag")));

            String generateTagName = "atag" + uf.getGuid();
            log.Info("TagNameToBe Added  :" + generateTagName);

            AddKeywordsText(generateTagName);

            AddtagButton().Click();

            SaveButton().Click();

            //Verify the banner message
            VerifyBannerMessage();

            log.Info("New Keyword is being added");

            return generateTagName;


        }

        //This Function write keyword inside keyword text field
        public void AddKeywordsText(String tagname)
        {
            KeywordsTextField().SendKeys(tagname);
        }

        public void VerifyBannerMessage()
        {
            iWait.Until(ExpectedConditions.ElementIsVisible(By.Id("btnOk")));

            Assert.AreEqual("User settings have been saved successfully".Trim(), driver.FindElement(By.Id("Sucess_Message")).Text.Trim());

            driver.FindElement(By.Id("btnOk")).Click();

            OverlayWait();

        }


        //This Function remove Tag and verify the same
        public void RemoveTag(String generateTagName)
        {
            log.Info("Inside Remove tag");

            //NSoup to parse the code of Page.
            Document doc = NSoup.NSoupClient.Parse(driver.PageSource);
            Elements rowListNsoup = doc.GetElementsByClass("tagBox-list")[0].GetElementsByClass("tagBox-item");

            // Retreving all the rows of Manage Table 
            iWait.Until(ExpectedConditions.ElementExists((By.ClassName("tagBox-list"))));
            IList<IWebElement> seleniumTaglist = driver.FindElement(By.ClassName("tagBox-list")).FindElements(By.ClassName("tagBox-item"));

            flag = false;

            int rowcounter = 0;

            foreach (Element temp in rowListNsoup)
            {
                var firstText = temp.GetElementsByClass("tagBox-item-content")[0].OwnText();
                var firstChildText = temp.GetElementsByClass("tagBox-item-content")[0].GetElementsByTag("a")[0].OwnText();

                firstText = firstText.Replace(firstChildText, string.Empty);

                if (firstText.Trim().Equals(generateTagName.Trim()))
                {
                    flag = true;
                    // seleniumTaglist[rowcounter].FindElement(By.CssSelector("span.tagBox-item-content > a.tagBox-remove")).Click();

                    IWebElement removetag = seleniumTaglist[rowcounter].FindElement(By.ClassName("tagBox-item-content")).FindElement(By.TagName("a"));

                    executor.ExecuteScript("arguments[0].scrollIntoView(true);", removetag);
                    executor.ExecuteScript("arguments[0].click()", removetag);

                    SaveButton().Click();

                    VerifyBannerMessage();

                    break;
                }
                rowcounter++;

            }
            Assert.AreEqual(true, flag);

            Assert.AreEqual(false, driver.PageSource.Contains(generateTagName.Trim()));
            log.Info(" Remove tag process completed");
        }

        #endregion

        #region CommentTab

        public Boolean IsElementChecked(String checkboxID)
        {

            Boolean checkboxStatus = (Boolean)executor.ExecuteScript(" return document.getElementById('" + checkboxID + "').checked");

            log.Info("Status of element Checked :: " + checkboxStatus + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

            return checkboxStatus;

        }

        public void VideoSearch(String searchType, String searchText, String videoName)
        {

            iWait.Until(ExpectedConditions.ElementExists(By.Id("ContentPlaceHolder1_ddlSelectTypeSearch")));

            SelectElement videoSearchType = new SelectElement(driver.FindElement(By.Id("ContentPlaceHolder1_ddlSelectTypeSearch")));
            videoSearchType.SelectByText(searchType);

            VideoSearchText().Clear();
            VideoSearchText().SendKeys(searchText);

            //Click on search button
            VideoSearchButton().Click();

            OverlayWait();

            //NSoup to parse the code of Page.
            Document doc = NSoup.NSoupClient.Parse(driver.PageSource);

            iWait.Until(ExpectedConditions.ElementIsVisible(By.Id("ContentPlaceHolder1_grdVideoListingSearch")));

            Elements rowListNsoup = doc.GetElementById("ContentPlaceHolder1_grdVideoListingSearch").GetElementsByTag("tr");

            // Retreving all the rows of Manage Table 
            IList<IWebElement> rowListSelenium = (IList<IWebElement>)driver.FindElement(By.Id("ContentPlaceHolder1_grdVideoListingSearch")).FindElements(By.TagName("tr"));

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
                        iWait.Until(ExpectedConditions.ElementIsVisible(By.Id("ContentPlaceHolder1_grdVideoListingSearch_btnSelect_" + rowcounter)));

                        //click on Select Button
                        rowListSelenium[rowcounter + 1].FindElements(By.TagName("td"))[0].FindElement(By.Id("ContentPlaceHolder1_grdVideoListingSearch_btnSelect_" + rowcounter)).Click();

                        OverlayWait();

                        iWait.Until(ExpectedConditions.ElementIsVisible(By.Id("ContentPlaceHolder1_btnSearch")));
                        log.Info("Clicked on select Button: " + rowcounter);

                        break;
                    }
                    rowcounter++;
                }
            }


        }

        public IList<IWebElement> CommentCount()
        {
            iWait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy(By.Id("ContentPlaceHolder1_CommentDetails")));

            IList<IWebElement> commentDetails = driver.FindElement(By.Id("ContentPlaceHolder1_CommentDetails")).FindElements(By.CssSelector("div.clearfix"));

            return commentDetails;
        }

        public void SuccessBannerMessage()
        {

            iWait.Until(ExpectedConditions.ElementIsVisible(By.Id("btnOk")));

            Assert.AreEqual("Comments Updated successfully.".Trim(), driver.FindElement(By.Id("Sucess_Message")).Text.Trim());

            driver.FindElement(By.Id("btnOk")).Click();

            OverlayWait();
        }

        //This Function hide the comment and verify the same 
        public void HideTest(int expectedCommentCount, String expectedluser, String expectedcommentText, String expectedDate)
        {
            log.Info("HideTest  started" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

            //  Click on show reported comment checkbox
            iWait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy(By.Id("ContentPlaceHolder1_CheckBox3")));

            driver.FindElement(By.Id("ContentPlaceHolder1_CheckBox3")).Click();

            OverlayWait();

            //get comment details

            log.Info("Getting Comment Details:::::");

            IList<IWebElement> commentDetails = CommentCount();
            int actualCommentCount = commentDetails.Count;

            String userFirstName = commentDetails[1].FindElement(By.Id("ContentPlaceHolder1_RepterDetails_lblFirstName_0")).Text.Trim();

            String userLastName = commentDetails[1].FindElement(By.CssSelector("div > div.col-md-9 > span#ContentPlaceHolder1_RepterDetails_lblLastName_0")).Text.Trim();

            String userAccountName = commentDetails[1].FindElement(By.CssSelector("div > div.col-md-9 > span#ContentPlaceHolder1_RepterDetails_lblAccountName_0")).Text.Trim();

            String actualUser = userFirstName + " " + userLastName + " - " + userAccountName;

            String actualCommentDate = commentDetails[1].FindElement(By.CssSelector("div > div.col-md-9 > span#ContentPlaceHolder1_RepterDetails_lblcommentdate_0")).Text.Trim();

            String actualCommentText = commentDetails[1].FindElement(By.CssSelector("div > div.col-md-9 > span#ContentPlaceHolder1_RepterDetails_lblCommentText_0")).Text.Trim();

            log.Info("Verify commnet details of web portal in admin portal");

            Boolean checkboxStatus = IsElementChecked("ContentPlaceHolder1_RepterDetails_checkststus_0");

            Assert.AreEqual(true, checkboxStatus);
            Assert.AreEqual(expectedluser, actualUser);
            Assert.AreEqual(expectedDate, actualCommentDate);
            Assert.AreEqual(expectedcommentText, actualCommentText);

            //verify hide button
            Assert.AreEqual("Hide", HideNShowButton().GetAttribute("value"));

            //click on hide button
            HideNShowButton().Click();

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

        #endregion

        #endregion


        [Test]
        public void TVAdmin_001_UI_Verification()
        {
            try
            {
                log.Info("TVAdmin_001_UI_Verification test started" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

                RedirectingToCommentsManagement();

                //Click on Abuse Filtration tab
                AbuseFiltrationTab().Click();

                #region Region to Verfiy the Default Value or status of Each Element

                //Waiting for Keywords textfield
                iWait.Until(ExpectedConditions.ElementExists(By.ClassName("tagBox-container")));

                //Keywords textbox is enabled.
                Assert.AreEqual(true, KeywordsTextField().Enabled);

                //AddTag button is enabled
                Assert.AreEqual(true, AddtagButton().Enabled);
                Thread.Sleep(500);

                Assert.AreEqual(true, SaveButton().Enabled);
                Thread.Sleep(500);

                Assert.AreEqual(true, ResetButton().Enabled);

                #endregion

                log.Info("TVAdmin_001_UI_Verification test completed" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

            }

            catch (Exception e)
            {
                log.Error(e.Message + "\n" + e.StackTrace + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

                Assert.AreEqual(true, false);
            }

        }


        [Test]
        public void TVAdmin_002_AddKeywordsTest()
        {
            try
            {
                log.Info("TVAdmin_002_AbuseFiltration_KeywordsTest  started" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

                String generateTagName = AddNewKeyword();

                //NSoup to parse the code of Page.
                Document doc = NSoup.NSoupClient.Parse(driver.PageSource);
                Elements rowListNsoup = doc.GetElementsByClass("tagBox-list")[0].GetElementsByClass("tagBox-item");


                #region Applying_Assert

                flag = false;

                foreach (Element temp in rowListNsoup)
                {
                    var firstText = temp.GetElementsByClass("tagBox-item-content")[0].OwnText();
                    var firstChildText = temp.GetElementsByClass("tagBox-item-content")[0].GetElementsByTag("a")[0].OwnText();

                    firstText = firstText.Replace(firstChildText, string.Empty);

                    if (firstText.Trim().Equals(generateTagName.Trim()))
                    {
                        flag = true;
                        log.Info("success");

                        break;
                    }

                }
                Assert.AreEqual(true, flag);
                #endregion

                #region  Remove Added Tag

                RemoveTag(generateTagName);

                #endregion


                log.Info("TVAdmin_002_AbuseFiltration_KeywordsTest completed" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());
            }

            catch (Exception e)
            {
                log.Error(e.Message + "\n" + e.StackTrace + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

                Assert.AreEqual(true, false);
            }
        }


        [Test]
        public void TVAdmin_003_MandatoryAbuseTest()
        {
            try
            {
                log.Info("TVAdmin_003_MandatoryTest  started" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

                RedirectingToCommentsManagement();

                AbuseFiltrationTab().Click();

                AddtagButton().Click();

                iWait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("div.infomsg > button.btn-sm")));

                Assert.AreEqual("Please enter Keyword", driver.FindElement(By.Id("Info")).Text.Trim());

                driver.FindElement(By.CssSelector("div.infomsg > button.btn-sm")).Click();

                OverlayWait();

                String generateTagName = AddNewKeyword();

                AddKeywordsText(generateTagName);

                AddtagButton().Click();

                iWait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("div.infomsg > button.btn-sm")));

                Assert.AreEqual("Keyword already present", driver.FindElement(By.Id("Info")).Text.Trim());

                driver.FindElement(By.CssSelector("div.infomsg > button.btn-sm")).Click();

                OverlayWait();

                RemoveTag(generateTagName);

                log.Info("TVAdmin_003_MandatoryTest completed" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());
            }


            catch (Exception e)
            {
                log.Error(e.Message + "\n" + e.StackTrace + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

                Assert.AreEqual(true, false);
            }
        }

        [Test]
        public void TVAdmin_004_ResetButtnTest()
        {
            try
            {
                log.Info("TVAdmin_004_ResetButtnTest  started" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());


                RedirectingToCommentsManagement();

                AbuseFiltrationTab().Click();

                iWait.Until(ExpectedConditions.ElementToBeClickable(By.ClassName("tagBox-add-tag")));

                String generateTagName = "atag" + uf.getGuid();
                log.Info("TagNameToBe Added  :" + generateTagName);

                AddKeywordsText(generateTagName);

                AddtagButton().Click();

                uf.scrollDown(driver);

                iWait.Until(ExpectedConditions.ElementToBeClickable(By.Id("ContentPlaceHolder1_btncancel")));

                driver.FindElement(By.Id("ContentPlaceHolder1_btncancel")).Click();

                iWait.Until(ExpectedConditions.ElementIsVisible(By.Id("ContentPlaceHolder1_btncancel")));

                Assert.AreEqual(false, driver.PageSource.Contains(generateTagName.Trim()));

                log.Info("TVAdmin_004_ResetButtnTest completed" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());
            }



            catch (Exception e)
            {
                log.Error(e.Message + "\n" + e.StackTrace + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

                Assert.AreEqual(true, false);
            }
        }

        [Test]
        public void TVAdmin_005_SearchParameterTest()
        {
            try
            {
                log.Info("TVAdmin_005_SearchParameterTest  started" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

                RedirectingToCommentsManagement();

                List<String> videoname = cf.readSysConfigFile("AdminPortal", "VideoManagement", "SysConfig.xml");

                String videoTitle = videoname.ElementAt(0).ToString();
                String videoID = videoname.ElementAt(1).ToString();
                String guid = videoname.ElementAt(2).ToString();
                String streamID = videoname.ElementAt(3).ToString();
                String speakerName = videoname.ElementAt(4).ToString();
                String locationName = videoname.ElementAt(5).ToString();

                log.Info("videoTitle  :  " + videoTitle);
                log.Info("videoID   : " + videoID);
                log.Info("guid   : " + guid);
                log.Info("streamID   : " + streamID);
                log.Info("speakerName  :  " + speakerName);
                log.Info("locationName  :  " + locationName);

                String[] vidSearchArr = { "Title", "Video ID", "GUID", "Stream ID", "Speaker", "Location" };

                String[] vidSearchTextArr = { videoTitle, videoID, guid, streamID, speakerName, locationName };

                for (int i = 0; i < vidSearchArr.Length; i++)
                {

                    VideoSearch(vidSearchArr[i], vidSearchTextArr[i], videoTitle);

                    Assert.AreEqual(videoTitle, driver.FindElement(By.Id("ContentPlaceHolder1_lblVideoDetails")).Text.Trim());

                    Assert.AreEqual(guid, driver.FindElement(By.Id("ContentPlaceHolder1_lblVideoIdDetails")).Text.Trim());


                }

                log.Info("TVAdmin_005_SearchParameterTest  Completed" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

            }


            catch (Exception e)
            {
                log.Error(e.Message + "\n" + e.StackTrace + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

                Assert.AreEqual(true, false);
            }
        }

        [Test]
        public void TVAdmin_006_BlankRecordTest()
        {
            try
            {
                log.Info("TVAdmin_006_BlankRecordTest  started" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());


                String searchText = "NoResultb6343f2";
                RedirectingToCommentsManagement();

                VideoSearchText().Clear();
                VideoSearchText().SendKeys(searchText);

                //Click on search button
                VideoSearchButton().Click();

                OverlayWait();

                Assert.AreEqual("No records to display.", driver.FindElement(By.ClassName("norecords_display")).Text.Trim());
                log.Info("TVAdmin_006_BlankRecordTest  Completed" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());
            }


            catch (Exception e)
            {
                log.Error(e.Message + "\n" + e.StackTrace + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

                Assert.AreEqual(true, false);
            }
        }

        [Test]
        public void TVAdmin_007_VerifyMyVideos()
        {
            iWait.Until(ExpectedConditions.ElementIsVisible(By.Id("rptMenu_aMenuItem_0")));

            RedirectingToCommentsManagement();

            // Clicking on My videos checkbox

            driver.FindElement(By.Id("ContentPlaceHolder1_chkMyVideosSearch")).Click();

            string[] welcomeText = driver.FindElement(By.Id("lblUserName")).Text.Split(':');

            Console.WriteLine("Username:" + welcomeText[1]);

            if (welcomeText[1].Length > 0)
            {
                List<String> videoTitles = new List<String>();

                string userID = gm.connecDBGetUserID(welcomeText[1].Trim());

                if (userID != null)
                {
                    string xmlVideofile = gm.getMyVideos(userID);

                    Console.WriteLine("Response:" + xmlVideofile);

                    if (xmlVideofile.Equals("<Videos>NONE</Videos>"))
                    {
                        Console.WriteLine("No videos present in My Videos for User");

                        iWait.Until(ExpectedConditions.ElementIsVisible(By.ClassName("norecords_display")));

                        Assert.AreEqual("No records to display.", driver.FindElement(By.ClassName("norecords_display")).Text.ToString());
                    }
                    else
                    {
                        XmlDocument xdoc = new XmlDocument();

                        xdoc.LoadXml(xmlVideofile);

                        XmlNode videoDetailsgNode = xdoc.SelectSingleNode("Videos");

                        XmlNodeList videoNodeList = videoDetailsgNode.SelectNodes("Video");

                        foreach (XmlNode node in videoNodeList)
                        {
                            videoTitles.Add(node.SelectSingleNode("Title") != null ? node.SelectSingleNode("Title").InnerText : string.Empty);

                        }

                        iWait.Until(ExpectedConditions.ElementIsVisible(By.Id("ContentPlaceHolder1_grdVideoListingSearch")));

                        List<String> videoTitleFromUI = new List<String>();

                        IList<IWebElement> tableRows = (IList<IWebElement>)driver.FindElements(By.CssSelector("table#ContentPlaceHolder1_grdVideoListingSearch > tbody > tr"));

                        // Verifying total videos match on UI

                        Assert.AreEqual(videoTitles.Count, tableRows.Count - 2);

                        int counter = 0;

                        foreach (IWebElement row in tableRows)
                        {
                            String classAttributeOfRow = row.GetAttribute("class");
                            if (classAttributeOfRow.Equals("GridRowStyle") || classAttributeOfRow.Equals("AltGridStyle"))
                            {
                                // Add video title from UI in video list
                                videoTitleFromUI.Add(row.FindElement(By.CssSelector("td > span#ContentPlaceHolder1_grdVideoListingSearch_lnkTitleSearch_" + counter)).Text);
                                counter++;
                            }
                        }

                        //Comparing values of both the list.
                        counter = 0;
                        foreach (String str in videoTitles)
                            Assert.AreEqual(str, videoTitleFromUI[counter++]);

                    }
                }
                else
                {
                    log.Info("User ID returned null");
                    Assert.AreEqual(true, false);
                }

            }
            else
            {
                log.Info("User name returned blank");
                Assert.AreEqual(true, false);
            }


        }

        [TestFixtureTearDown]
        public void TearDown()
        {
            st.IE_TearDown(driver, log);
        }

    }
}
