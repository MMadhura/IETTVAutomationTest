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
using System.Diagnostics;
using System.Drawing;
using IETTVWebportal.Chrome;
using Utilities.Object_Repository;




namespace IETTVAdminPortal.Chrome
{
    [TestFixture]
    public class Chrome_ContentManagementSystem
    {
        // This is to configure logger mechanism for Utilities.Config
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        #region Variable_Declration and Object Instantiation

        IWait<IWebDriver> iWait = null;

        Boolean flag = false;

        IJavaScriptExecutor executor;

        String imagePath = Environment.CurrentDirectory + "\\Upload\\Images\\CMS\\";

        public IWebDriver driver = null;

        string driverName = "", driverPath, appURL;

        Utility_Functions uf = new Utility_Functions();             // Instantiate object for Utility Functons

        Configuration cf = new Configuration();                     // Instantiate object for Configuration

        Chrome_AdminSetupTearDown st = new Chrome_AdminSetupTearDown();                         // Instantiate object for Chrome Setup Teardown

        Object_Repository_Class OR = new Object_Repository_Class();

        private Chrome_CMS_Verification objWebCMS;

        String content, descriptionContent, altTextForImage, imageURL;

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

                driverName = "webdriver.chrome.driver";                                    // Driver name for Chrome

                driverPath = baseDir + "/chromedriver.exe";                                // Path for ChromeDriver

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

            appURL = st.Chrome_Setup(driver, log, executor);                                // Calling Chrome Setup
            objWebCMS = new Chrome_CMS_Verification(driver, log, executor, iWait);

            content = cf.readingXMLFile("AdminPortal", "CMS", "titleContent", "Config.xml");
            descriptionContent = cf.readingXMLFile("AdminPortal", "CMS", "descriptionContent", "Config.xml");
            altTextForImage = cf.readingXMLFile("AdminPortal", "CMS", "altTextForImage", "Config.xml");
            imageURL = cf.readingXMLFile("AdminPortal", "CMS", "imageURL", "Config.xml");
        }


        #endregion

        #region  Common functions

        // This function puts the control on CMS page
        public void RedirectToCMS()
        {

            log.Info("Inside Redirecting to CMS Page");

            iWait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy((OR.GetElement("VideoRequestByUser", "AdminMenu", "TVAdminPortalOR.xml"))));
            driver.FindElement((OR.GetElement("VideoRequestByUser", "AdminMenu", "TVAdminPortalOR.xml"))).Click();
            Thread.Sleep(1000);

            iWait.Until(ExpectedConditions.ElementToBeClickable(OR.GetElement("CMS", "CMSLink", "TVAdminPortalOR.xml")));
            driver.FindElement(OR.GetElement("CMS", "CMSLink", "TVAdminPortalOR.xml")).Click();

            iWait.Until(ExpectedConditions.ElementIsVisible(OR.GetElement("CMS", "BtnSaveChanges", "TVAdminPortalOR.xml")));
        }

        public void MoveToServices_Filming()
        {
            // Clicking on Service Menu to open the Service menu
            driver.FindElement(OR.GetElement("CMS", "ServicesDropdown", "TVAdminPortalOR.xml")).FindElement(By.TagName("a")).Click();

            Thread.Sleep(1500);

            iWait.Until(ExpectedConditions.ElementIsVisible(OR.GetElement("CMS", "ServicesUl", "TVAdminPortalOR.xml")));
            string expectedCaptionOfPageTitle = driver.FindElement(OR.GetElement("CMS", "ServicesUl", "TVAdminPortalOR.xml")).Text.Trim();

            // Clicking on Filming option under help menu
            driver.FindElement(OR.GetElement("CMS", "ServicesUl", "TVAdminPortalOR.xml")).Click();

            uf.isJqueryActive(driver);

            iWait.Until(ExpectedConditions.ElementIsVisible(OR.GetElement("CMS", "PageName", "TVAdminPortalOR.xml")));
            Assert.AreEqual(expectedCaptionOfPageTitle, driver.FindElement(OR.GetElement("CMS", "PageName", "TVAdminPortalOR.xml")).Text.Trim());
        }

        public void ModifyCMSHelpTitle()
        {

            iWait.Until(ExpectedConditions.ElementIsVisible(OR.GetElement("CMS", "BtnSaveChanges", "TVAdminPortalOR.xml")));
            String currentTitle = driver.FindElement(OR.GetElement("CMS", "TitleDiv", "TVAdminPortalOR.xml")).Text;
            log.Info("Current Title::" + currentTitle);

            driver.FindElement(OR.GetElement("CMS", "BtnPreview", "TVAdminPortalOR.xml")).Click();  //click on Edit button
            iWait.Until(ExpectedConditions.ElementToBeClickable(OR.GetElement("CMS", "BtnEditForTitle", "TVAdminPortalOR.xml")));
            executor.ExecuteScript("arguments[0].click()", driver.FindElement(OR.GetElement("CMS", "BtnEditForTitle", "TVAdminPortalOR.xml")));

            iWait.Until(ExpectedConditions.ElementIsVisible(OR.GetElement("CMS", "PopUpModal1", "TVAdminPortalOR.xml")));

            IWebElement title_frame = driver.FindElement(OR.GetElement("VideoManagement", "Abstract", "TVAdminPortalOR.xml"));     //Enter data into Title field
            driver.SwitchTo().Frame(title_frame);
            IWebElement editor_body = driver.FindElement(By.TagName("body"));
            editor_body.Clear();
            editor_body.SendKeys(content);
            driver.SwitchTo().DefaultContent();

            Thread.Sleep(2000);
            driver.FindElement(OR.GetElement("CMS", "BtnSaveHeading", "TVAdminPortalOR.xml")).Click();                      //click on Apply change button
            iWait.Until(ExpectedConditions.ElementExists(OR.GetElement("CMS", "TitleDiv", "TVAdminPortalOR.xml")));

            log.Info("\n modified Title:: " + driver.FindElement(OR.GetElement("CMS", "TitleDiv", "TVAdminPortalOR.xml")).Text);

        }

        public void ModifyCMSHelpDescription()
        {
            Thread.Sleep(2000);

            // Clicking on Edit icon of Content
            driver.FindElement(OR.GetElement("CMS", "BtnEditForContent", "TVAdminPortalOR.xml")).Click();

            // Waiting till dialog box displayed.
            iWait.Until(ExpectedConditions.ElementIsVisible(OR.GetElement("CMS", "PopUpModal2", "TVAdminPortalOR.xml")));

            // Getting control of Iframe
            IWebElement frame = driver.FindElement(OR.GetElement("CMS", "ContentsFrame", "TVAdminPortalOR.xml")).FindElement(OR.GetElement("VideoManagement", "Abstract", "TVAdminPortalOR.xml"));
            driver.SwitchTo().Frame(frame);

            Thread.Sleep(1000);

            //  Getting Control of Body
            IWebElement body = driver.FindElement(By.TagName("body"));
            body.Clear();

            Thread.Sleep(1000);

            // Writing content in Text Area
            body.SendKeys(descriptionContent);
            driver.SwitchTo().DefaultContent();

            Thread.Sleep(2000);

            // Clicking on Apply Change button
            driver.FindElement(OR.GetElement("CMS", "BtnSaveEditor", "TVAdminPortalOR.xml")).Click();

            // Waiting till Pop up gets closed.
            iWait.Until(ExpectedConditions.InvisibilityOfElementLocated(OR.GetElement("CMS", "PopUpModal2", "TVAdminPortalOR.xml")));

        }

        public void ModifyHelpFirstImage(String imageName1, String imageURL, String altTextForImage)
        {

            // Clicking on Edit icon of Media 1
            driver.FindElement(OR.GetElement("CMS", "BtnEditForImage1", "TVAdminPortalOR.xml")).Click();

            // Verfying that Select Media pop up is displayed or not
            iWait.Until(ExpectedConditions.ElementIsVisible(OR.GetElement("CMS", "PopUpModal3", "TVAdminPortalOR.xml")));

            IWebElement selectFile = driver.FindElement(OR.GetElement("CMS", "SelectMedia1", "TVAdminPortalOR.xml"));
            uf.uploadfile(selectFile, imagePath + imageName1);

            // Clicking on Upload Button
            driver.FindElement(OR.GetElement("CMS", "BtnMediaUpload1", "TVAdminPortalOR.xml")).Click();

            // Entering text into ALT field
            driver.FindElement(OR.GetElement("CMS", "TxtAltTextImage1", "TVAdminPortalOR.xml")).Clear();
            driver.FindElement(OR.GetElement("CMS", "TxtAltTextImage1", "TVAdminPortalOR.xml")).SendKeys(altTextForImage);

            // Entering URL for the images
            Thread.Sleep(8000);
            driver.FindElement(OR.GetElement("CMS", "TxtLinkImage1", "TVAdminPortalOR.xml")).Clear();
            driver.FindElement(OR.GetElement("CMS", "TxtLinkImage1", "TVAdminPortalOR.xml")).SendKeys(imageURL);

            // Clicking on Apply Change button
            driver.FindElement(OR.GetElement("CMS", "BtnSaveImage1", "TVAdminPortalOR.xml")).Click();

            // waiting till pop up gets closed
            iWait.Until(ExpectedConditions.InvisibilityOfElementLocated(OR.GetElement("CMS", "PopUpModal3", "TVAdminPortalOR.xml")));
            iWait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.ClassName("in")));


        }

        public void ModifyHelpSecondImage(String imageName2, String imageURL, String altTextForImage)
        {
            iWait.Until(ExpectedConditions.ElementIsVisible(OR.GetElement("CMS", "BtnEditForImage2", "TVAdminPortalOR.xml")));

            // Clicking on Edit icon of Media 2
            driver.FindElement(OR.GetElement("CMS", "BtnEditForImage2", "TVAdminPortalOR.xml")).Click();

            // Verfying that Select Media pop up is displayed or not
            iWait.Until(ExpectedConditions.ElementIsVisible(OR.GetElement("CMS", "PopUpModal4", "TVAdminPortalOR.xml")));


            IWebElement selectFile = driver.FindElement(OR.GetElement("CMS", "SelectMedia2", "TVAdminPortalOR.xml"));
            uf.uploadfile(selectFile, imagePath + imageName2);

            // Clicking on Upload Button
            driver.FindElement(OR.GetElement("CMS", "BtnMediaUpload2", "TVAdminPortalOR.xml")).Click();

            // Enetring text into ALT field
            driver.FindElement(OR.GetElement("CMS", "TxtAltTextImage2", "TVAdminPortalOR.xml")).Clear();
            driver.FindElement(OR.GetElement("CMS", "TxtAltTextImage2", "TVAdminPortalOR.xml")).SendKeys(altTextForImage);

            // ENtering URL for the images
            driver.FindElement(OR.GetElement("CMS", "TxtLinkImage2", "TVAdminPortalOR.xml")).Clear();
            driver.FindElement(OR.GetElement("CMS", "TxtLinkImage2", "TVAdminPortalOR.xml")).SendKeys(imageURL);

            // Clicking on Apply Change button
            driver.FindElement(OR.GetElement("CMS", "BtnSaveImage2", "TVAdminPortalOR.xml")).Click();

            // waiting till pop up gets closed
            iWait.Until(ExpectedConditions.InvisibilityOfElementLocated(OR.GetElement("CMS", "PopUpModal4", "TVAdminPortalOR.xml")));
            iWait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.ClassName("in")));

        }

        public void VerifySuccessBannerMsg(String message)
        {
            //Verify the banner Message
            iWait.Until(ExpectedConditions.ElementIsVisible(OR.GetElement("PollManagement", "OkButtonId", "TVAdminPortalOR.xml")));
            string successMessage = driver.FindElements(OR.GetElement("CMS", "Successmsg", "TVAdminPortalOR.xml"))[1].FindElement(OR.GetElement("CMS", "Sucess_Message", "TVAdminPortalOR.xml")).Text;
            //  string successMessage = driver.FindElement(OR.GetElement("CMS", "Sucess_Sucess_Message", "TVAdminPortalOR.xml")).Text;
            Assert.AreEqual(message, successMessage);


            //click on ok button banner message
            driver.FindElement(OR.GetElement("PollManagement", "OkButtonId", "TVAdminPortalOR.xml")).Click();
            iWait.Until(ExpectedConditions.InvisibilityOfElementLocated(OR.GetElement("PollManagement", "OkButtonId", "TVAdminPortalOR.xml")));

        }

        public void VerifyContentAfterPreviewAndSave()
        {
            //click on preview button
            iWait.Until(ExpectedConditions.ElementToBeClickable(OR.GetElement("CMS", "BtnPreview", "TVAdminPortalOR.xml")));
            executor.ExecuteScript("arguments[0].click()", driver.FindElement(OR.GetElement("CMS", "BtnPreview", "TVAdminPortalOR.xml")));
            //driver.FindElement(OR.GetElement("CMS", "BtnPreview", "TVAdminPortalOR.xml")).Click();
            Assert.AreEqual(content, driver.FindElement(OR.GetElement("CMS", "TitleDiv", "TVAdminPortalOR.xml")).Text);       //verify title content
            Assert.AreEqual(descriptionContent, driver.FindElement(OR.GetElement("CMS", "ContentDiv", "TVAdminPortalOR.xml")).Text);     //verify Description content


            //click on Save Changes button
            iWait.Until(ExpectedConditions.ElementToBeClickable(OR.GetElement("CMS", "BtnSaveChanges", "TVAdminPortalOR.xml")));
            driver.FindElement(OR.GetElement("CMS", "BtnSaveChanges", "TVAdminPortalOR.xml")).Click();

            VerifySuccessBannerMsg("Content Edited Successfully.".Trim());

            Assert.AreEqual(content, driver.FindElement(OR.GetElement("CMS", "TitleDiv", "TVAdminPortalOR.xml")).Text);               //verify title content
            Assert.AreEqual(descriptionContent, driver.FindElement(OR.GetElement("CMS", "ContentDiv", "TVAdminPortalOR.xml")).Text);                 //verify Description content
        }

        // This function renames the file with Unique GUID
        public Dictionary<String, String> renameFile()
        {

            Dictionary<String, String> images = new Dictionary<String, String>();

            String path = Environment.CurrentDirectory + "\\Upload\\Images\\CMS";
            String[] imageFiles = Directory.GetFiles(path);

            int i = 1;
            foreach (String currentFile in imageFiles)
            {
                log.Info("Current File Name :: " + currentFile);
                FileInfo fileInfo = new FileInfo(currentFile);

                fileInfo.MoveTo(path + "\\" + uf.getGuid() + ".png");

                Thread.Sleep(2000);

                images.Add("imageName" + i, fileInfo.Name);

                log.Info("File Name :: " + fileInfo.Name);
                i++;
            }

            return images;
        }

        #region Function used to verify Media 1 with video

        public void EnterEmbedCodeforVideo1(String videoEmbedCode)
        {
            // Clicking on Video Tab
            driver.FindElement(OR.GetElement("CMS", "Video", "TVAdminPortalOR.xml")).Click();

            // Waiting untill Video Tab becomes active
            iWait.Until(d => d.FindElement(OR.GetElement("CMS", "VideoTab3", "TVAdminPortalOR.xml")).FindElements(By.TagName("li"))[1].GetAttribute("class").Equals("active"));

            // Entering Embed code into field
            driver.FindElement(OR.GetElement("CMS", "EmbedCode1", "TVAdminPortalOR.xml")).Clear();
            driver.FindElement(OR.GetElement("CMS", "EmbedCode1", "TVAdminPortalOR.xml")).SendKeys(videoEmbedCode);

            // Clicking on Apply changes button button
            driver.FindElement(OR.GetElement("CMS", "BtnSaveImage1", "TVAdminPortalOR.xml")).Click();

            iWait.Until(ExpectedConditions.InvisibilityOfElementLocated(OR.GetElement("CMS", "PopUpModal3", "TVAdminPortalOR.xml")));


        }

        public void SaveAndVerifyVideo1()
        {

            // Clickcing on Save button           
            driver.FindElement(OR.GetElement("CMS", "BtnSaveChanges", "TVAdminPortalOR.xml")).Click();

            VerifySuccessBannerMsg("Content Edited Successfully.".Trim());

            // Verfying the Banner message
            //iWait.Until(ExpectedConditions.ElementIsVisible(OR.GetElement("PollManagement", "OkButtonId", "TVAdminPortalOR.xml")));

            //Assert.AreEqual("Content Edited Successfully.", driver.FindElements(OR.GetElement("CMS", "Sucess_Sucess_Message", "TVAdminPortalOR.xml"))[1].Text.Trim());
            //driver.FindElement(OR.GetElement("PollManagement", "OkButtonId", "TVAdminPortalOR.xml")).Click();

            //// waiting till Banner message gets invisible
            //iWait.Until(ExpectedConditions.InvisibilityOfElementLocated(OR.GetElement("PollManagement", "OkButtonId", "TVAdminPortalOR.xml")));

            int isVideoAdded = driver.FindElement(OR.GetElement("CMS", "VideoPlayer1", "TVAdminPortalOR.xml")).FindElements(By.TagName("iframe")).Count;
            Assert.AreEqual(1, isVideoAdded);



        }



        #endregion

        #region Function used to verify Media 2 with video

        public void EnterEmbedCodeforVideo2(String videoEmbedCode)
        {
            // Clicking on Video Tab
            driver.FindElement(OR.GetElement("CMS", "Video", "TVAdminPortalOR.xml")).Click();

            // Waiting untill Video Tab becomes active
            iWait.Until(d => d.FindElement(OR.GetElement("CMS", "VideoTab4", "TVAdminPortalOR.xml")).FindElements(By.TagName("li"))[1].GetAttribute("class").Equals("active"));

            // Entering Embed code into field
            driver.FindElement(OR.GetElement("CMS", "EmbedCode2", "TVAdminPortalOR.xml")).Clear();
            driver.FindElement(OR.GetElement("CMS", "EmbedCode2", "TVAdminPortalOR.xml")).SendKeys(videoEmbedCode);

            // Clicking on Apply changes button button
            driver.FindElement(OR.GetElement("CMS", "BtnSaveImage2", "TVAdminPortalOR.xml")).Click();

            iWait.Until(ExpectedConditions.InvisibilityOfElementLocated(OR.GetElement("CMS", "PopUpModal4", "TVAdminPortalOR.xml")));
            //iWait.Until(ExpectedConditions.InvisibilityOfElementLocated(OR.GetElement("CMS", "PopUpModal4", "TVAdminPortalOR.xml")));
        }

        public void SaveAndVerifyVideo2()
        {

            // Clicking on Save button
            driver.FindElement(OR.GetElement("CMS", "BtnSaveChanges", "TVAdminPortalOR.xml")).Click();

            VerifySuccessBannerMsg("Content Edited Successfully.".Trim());

            // Verfying the Banner message
            //iWait.Until(ExpectedConditions.ElementIsVisible(OR.GetElement("PollManagement", "OkButtonId", "TVAdminPortalOR.xml")));
            //driver.FindElement(OR.GetElement("PollManagement", "OkButtonId", "TVAdminPortalOR.xml")).Click();

            int isVideoAdded = driver.FindElement(OR.GetElement("CMS", "VideoPlayer2", "TVAdminPortalOR.xml")).FindElements(By.TagName("iframe")).Count;
            Assert.AreEqual(1, isVideoAdded);
        }

        #endregion

        public void OpenCKEditor()
        {

            //click on edit button
            driver.FindElement(OR.GetElement("CMS", "BtnPreview", "TVAdminPortalOR.xml")).Click();

            //click on Title edit button
            iWait.Until(ExpectedConditions.ElementIsVisible(OR.GetElement("CMS", "BtnEditForTitle", "TVAdminPortalOR.xml")));
            driver.FindElement(OR.GetElement("CMS", "BtnEditForTitle", "TVAdminPortalOR.xml")).Click();

            iWait.Until(ExpectedConditions.ElementIsVisible(OR.GetElement("CMS", "PopUpModal1", "TVAdminPortalOR.xml")));

        }

        public void WriteIntoCKEditor()
        {
            //writing into ck editor Title field
            IWebElement title_frame = driver.FindElement(OR.GetElement("VideoManagement", "Abstract", "TVAdminPortalOR.xml"));
            driver.SwitchTo().Frame(title_frame);
            IWebElement editor_body = driver.FindElement(By.TagName("body"));

            //clear the data and write new data
            Thread.Sleep(1000);
            editor_body.Clear();

            Thread.Sleep(1000);
            editor_body.SendKeys(content);
            Thread.Sleep(1000);

            driver.SwitchTo().DefaultContent();

            driver.FindElement(OR.GetElement("CMS", "BtnSaveHeading", "TVAdminPortalOR.xml")).Click();
            Thread.Sleep(1000);

            iWait.Until(ExpectedConditions.InvisibilityOfElementLocated(OR.GetElement("CMS", "PopUpModal1", "TVAdminPortalOR.xml")));

        }

        public void WritingDescriptionEditor()
        {
            Thread.Sleep(2000);

            // Clicking on Edit button
            //  driver.FindElement(OR.GetElement("CMS", "BtnPreview", "TVAdminPortalOR.xml")).Click();

            // waiting till Edit icon appears
            iWait.Until(ExpectedConditions.ElementIsVisible(OR.GetElement("CMS", "BtnEditForContent", "TVAdminPortalOR.xml")));

            // Clicking on Edit icon of Content
            driver.FindElement(OR.GetElement("CMS", "BtnEditForContent", "TVAdminPortalOR.xml")).Click();

            // Waiting till dialog box displayed.
            iWait.Until(ExpectedConditions.ElementIsVisible(OR.GetElement("CMS", "PopUpModal2", "TVAdminPortalOR.xml")));

            // Getting control of Iframe
            IWebElement frame = driver.FindElement(OR.GetElement("CMS", "ContentsFrame", "TVAdminPortalOR.xml")).FindElement(OR.GetElement("VideoManagement", "Abstract", "TVAdminPortalOR.xml"));
            driver.SwitchTo().Frame(frame);

            Thread.Sleep(1000);

            //  Getting Control of Body
            IWebElement body = driver.FindElement(By.TagName("body"));
            body.Clear();

            Thread.Sleep(1000);

            // Writing content in Text Area
            body.SendKeys(descriptionContent);
            driver.SwitchTo().DefaultContent();

            Thread.Sleep(2000);

            // Clicking on Apply Change button
            driver.FindElement(OR.GetElement("CMS", "BtnSaveEditor", "TVAdminPortalOR.xml")).Click();

            // Waiting till Pop up gets closed.
            iWait.Until(ExpectedConditions.InvisibilityOfElementLocated(OR.GetElement("CMS", "PopUpModal2", "TVAdminPortalOR.xml")));

        }



        #endregion


        [Test]
        public void TVAdmin_001_VerifyHelpInstnWithImages()
        {
            try
            {
                log.Info("TVAdmin_001_VerifyHelpInstnSectionWithImages Started" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

                RedirectToCMS();

                ModifyCMSHelpTitle();

                ModifyCMSHelpDescription();

                Dictionary<String, String> imageNames = renameFile();

                Thread.Sleep(8000);
                ModifyHelpFirstImage(imageNames["imageName1"], imageURL, altTextForImage);

                Thread.Sleep(8000);

                ModifyHelpSecondImage(imageNames["imageName2"], imageURL, altTextForImage);

                VerifyContentAfterPreviewAndSave();

                uf.OpenNewTab(driver);

                log.Info("count ::: " + driver.WindowHandles.Count);

                string browserType = uf.getRunningBrowser(driver, driver.FindElement(By.TagName("html")));

                uf.SwitchToWebTab(driver, browserType);

                uf.NavigateWebPortal(cf, driver);

                uf.scrollUp(driver);

                Thread.Sleep(4000);

                objWebCMS.handlePromotionalPopup();

                objWebCMS.HandleEmergencyPopUp();

                uf.scrollUp(driver);
                Thread.Sleep(5000);

                objWebCMS.RedirectToHelpInstitution();

                objWebCMS.VerifyTitle(content);

                objWebCMS.VerifyDescription(descriptionContent);

                objWebCMS.VerifyFirstImage(imageNames["imageName1"],imageURL, altTextForImage);

                objWebCMS.VerifySecondImage(imageNames["imageName2"],imageURL, altTextForImage);

                log.Info("TVAdmin_001_VerifyHelpInstnSectionWithImages completed" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

            }
            catch (Exception e)
            {

                Console.WriteLine(e.Message + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());
                log.Error(e.Message + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());
                Assert.Fail();
            }

        }

        [Test]
        public void TVAdmin_002_VerifyServicesFilmingWithImages()
        {
            try
            {

                Dictionary<String, String> imageNames = renameFile();

                log.Info("TVAdmin_002_VerifyServicesFilmingWithImages Started" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

                RedirectToCMS();

                MoveToServices_Filming();

                OpenCKEditor();

                WriteIntoCKEditor();

                WritingDescriptionEditor();

                ModifyHelpFirstImage(imageNames["imageName1"], imageURL, altTextForImage);

                ModifyHelpSecondImage(imageNames["imageName2"], imageURL, altTextForImage);

                VerifyContentAfterPreviewAndSave();

                uf.OpenNewTab(driver);

                log.Info("count ::: " + driver.WindowHandles.Count);

                string browserType = uf.getRunningBrowser(driver, driver.FindElement(By.TagName("html")));

                uf.SwitchToWebTab(driver, browserType);

                uf.NavigateWebPortal(cf, driver);

                uf.scrollUp(driver);

                Thread.Sleep(4000);

                objWebCMS.handlePromotionalPopup();

                objWebCMS.HandleEmergencyPopUp();

                objWebCMS.RedirectToServices_Filming();

                objWebCMS.VerifyTitle(content);

                objWebCMS.VerifyDescription(descriptionContent);

                objWebCMS.VerifyFirstImage(imageNames["imageName1"], imageURL, altTextForImage);

                objWebCMS.VerifySecondImage(imageNames["imageName2"], imageURL, altTextForImage);

                log.Info("TVAdmin_002_VerifyServicesFilmingWithImages completed" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());


            }
            catch (Exception e)
            {

                Console.WriteLine(e.Message + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());
                log.Error(e.Message + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());
                Assert.Fail();
            }


        }

        [Test]
        public void TVAdmin_003_VerifyHelpInstWithVideos()
        {
            try
            {

                log.Info("TVAdmin_001_VerifyHelpInstnSectionWithImages Started" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

                RedirectToCMS();

                ModifyCMSHelpTitle();

                ModifyCMSHelpDescription();

                // Clicking on Edit icon of Media 1
                iWait.Until(ExpectedConditions.ElementIsVisible(OR.GetElement("CMS", "BtnEditForImage1", "TVAdminPortalOR.xml")));
                driver.FindElement(OR.GetElement("CMS", "BtnEditForImage1", "TVAdminPortalOR.xml")).Click();

                iWait.Until(ExpectedConditions.ElementIsVisible(OR.GetElement("CMS", "PopUpModal3", "TVAdminPortalOR.xml")));

                #region Working with Media 1 with Video
                Console.WriteLine("video code  : " + cf.readingXMLFile("AdminPortal", "CMS", "videoCode", "Config.xml"));
                EnterEmbedCodeforVideo1("<iframe width=550 height=310 src="+cf.readingXMLFile("AdminPortal", "CMS", "videoCode", "Config.xml") +"</iframe>");

                //#endregion

                #region Verifying the details of video in Preview Mode

                //Clicing on Preview button
                driver.FindElement(OR.GetElement("CMS", "BtnPreview", "TVAdminPortalOR.xml")).Click();

                int isVideoAdded = driver.FindElement(OR.GetElement("CMS", "VideoPlayer1", "TVAdminPortalOR.xml")).FindElements(By.TagName("iframe")).Count;
                Assert.AreEqual(1, isVideoAdded);

                #endregion

                #region Verifying the details of video after Save

                SaveAndVerifyVideo1();

                #endregion

                #endregion

                #region Working with Media 2 with Video

                // Clicking on Edit button
                iWait.Until(ExpectedConditions.ElementIsVisible(OR.GetElement("CMS", "BtnPreview", "TVAdminPortalOR.xml")));
                driver.FindElement(OR.GetElement("CMS", "BtnPreview", "TVAdminPortalOR.xml")).Click();
                Thread.Sleep(2000);

                // Clicking on Edit icon of Media 2
                iWait.Until(ExpectedConditions.ElementIsVisible(OR.GetElement("CMS", "BtnEditForImage2", "TVAdminPortalOR.xml")));
                driver.FindElement(OR.GetElement("CMS", "BtnEditForImage2", "TVAdminPortalOR.xml")).Click();

                iWait.Until(ExpectedConditions.ElementIsVisible(OR.GetElement("CMS", "PopUpModal4", "TVAdminPortalOR.xml")));

                EnterEmbedCodeforVideo2("<iframe width=550 height=310 src=" + cf.readingXMLFile("AdminPortal", "CMS", "videoCode", "Config.xml") + "</iframe>");

                #region Verifying the details of video in Preview Mode

                //Clicking on Preview button
                driver.FindElement(OR.GetElement("CMS", "BtnPreview", "TVAdminPortalOR.xml")).Click();

                isVideoAdded = driver.FindElement(OR.GetElement("CMS", "VideoPlayer2", "TVAdminPortalOR.xml")).FindElements(By.TagName("iframe")).Count;
                Assert.AreEqual(1, isVideoAdded);

                #endregion

                #region Verifying the details of video after Save

                SaveAndVerifyVideo2();

                #endregion


                #endregion

                uf.OpenNewTab(driver);

                log.Info("count ::: " + driver.WindowHandles.Count);

                string browserType = uf.getRunningBrowser(driver, driver.FindElement(By.TagName("html")));

                uf.SwitchToWebTab(driver, browserType);

                uf.NavigateWebPortal(cf, driver);

                uf.scrollUp(driver);

                Thread.Sleep(4000);

                objWebCMS.handlePromotionalPopup();

                objWebCMS.HandleEmergencyPopUp();

                uf.scrollUp(driver);

                Thread.Sleep(2000);

                objWebCMS.RedirectToHelpInstitution();

                objWebCMS.VerifyTitle(content);

                objWebCMS.VerifyDescription(descriptionContent);

                objWebCMS.VerifyVideos();

            }
            catch (Exception e)
            {

                Console.WriteLine(e.Message + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());
                log.Error(e.Message + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());
                Assert.Fail();
            }

        }

        [Test]
        public void TVAdmin_004_VerifyServicesFilmingWithVideos()
        {
            try
            {

                log.Info("TVAdmin_004_VerifyServicesFilmingWithVideos Started" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

                RedirectToCMS();

                MoveToServices_Filming();

                OpenCKEditor();

                WriteIntoCKEditor();

                WritingDescriptionEditor();

                // Clicking on Edit icon of Media 1
                iWait.Until(ExpectedConditions.ElementIsVisible(OR.GetElement("CMS", "BtnEditForImage1", "TVAdminPortalOR.xml")));
                driver.FindElement(OR.GetElement("CMS", "BtnEditForImage1", "TVAdminPortalOR.xml")).Click();

                iWait.Until(ExpectedConditions.ElementIsVisible(OR.GetElement("CMS", "PopUpModal3", "TVAdminPortalOR.xml")));

                #region Working with Media 1 with Video

                EnterEmbedCodeforVideo1("<iframe width=550 height=310 src=" + cf.readingXMLFile("AdminPortal", "CMS", "videoCode", "Config.xml") + "</iframe>");

                #region Verifying the details of video in Preview Mode

                //Clicing on Preview button

                iWait.Until(ExpectedConditions.ElementIsVisible(OR.GetElement("CMS", "BtnPreview", "TVAdminPortalOR.xml")));
                driver.FindElement(OR.GetElement("CMS", "BtnPreview", "TVAdminPortalOR.xml")).Click();

                int isVideoAdded = driver.FindElement(OR.GetElement("CMS", "VideoPlayer1", "TVAdminPortalOR.xml")).FindElements(By.TagName("iframe")).Count;
                Assert.AreEqual(1, isVideoAdded);

                #endregion

                #region Verifying the details of video after Save

                SaveAndVerifyVideo1();

                #endregion

                #endregion

                #region Working with Media 2 with Video

                // Clicking on Edit button
                iWait.Until(ExpectedConditions.ElementIsVisible(OR.GetElement("CMS", "BtnPreview", "TVAdminPortalOR.xml")));
                driver.FindElement(OR.GetElement("CMS", "BtnPreview", "TVAdminPortalOR.xml")).Click();
                Thread.Sleep(2000);

                // Clicking on Edit icon of Media 2
                iWait.Until(ExpectedConditions.ElementIsVisible(OR.GetElement("CMS", "BtnEditForImage2", "TVAdminPortalOR.xml")));
                driver.FindElement(OR.GetElement("CMS", "BtnEditForImage2", "TVAdminPortalOR.xml")).Click();

                iWait.Until(ExpectedConditions.ElementIsVisible(OR.GetElement("CMS", "PopUpModal4", "TVAdminPortalOR.xml")));

                //  OpenPopUp("btnEditForImage2", "modal4", expectedCaptionOfPreviewButton, captionOfHelpMenu, captionOfServiceMenu); // opening media 1 pop up window

                EnterEmbedCodeforVideo2("<iframe width=550 height=310 src=" + cf.readingXMLFile("AdminPortal", "CMS", "videoCode", "Config.xml") + "</iframe>");


                // Clicking on Save button
                // driver.FindElement(OR.GetElement("CMS", "BtnSaveImage2", "TVAdminPortalOR.xml")).Click();

                //  iWait.Until(ExpectedConditions.InvisibilityOfElementLocated(OR.GetElement("CMS", "PopUpModal4", "TVAdminPortalOR.xml")));


                #region Verifying the details of video in Preview Mode

                //Clicking on Preview button
                iWait.Until(ExpectedConditions.ElementIsVisible(OR.GetElement("CMS", "BtnPreview", "TVAdminPortalOR.xml")));
                driver.FindElement(OR.GetElement("CMS", "BtnPreview", "TVAdminPortalOR.xml")).Click();

                isVideoAdded = driver.FindElement(OR.GetElement("CMS", "VideoPlayer2", "TVAdminPortalOR.xml")).FindElements(By.TagName("iframe")).Count;
                Assert.AreEqual(1, isVideoAdded);

                #endregion

                #region Verifying the details of video after Save

                SaveAndVerifyVideo2();

                #endregion


                #endregion

                Thread.Sleep(2000);
                uf.OpenNewTab(driver);

                log.Info("count ::: " + driver.WindowHandles.Count);

                string browserType = uf.getRunningBrowser(driver, driver.FindElement(By.TagName("html")));

                uf.SwitchToWebTab(driver, browserType);

                uf.NavigateWebPortal(cf, driver);

                uf.scrollUp(driver);

                Thread.Sleep(4000);

                objWebCMS.handlePromotionalPopup();

                objWebCMS.HandleEmergencyPopUp();

                uf.scrollUp(driver);

                Thread.Sleep(4000);

                objWebCMS.RedirectToServices_Filming();

                objWebCMS.VerifyTitle(content);

                objWebCMS.VerifyDescription(descriptionContent);

                objWebCMS.VerifyVideos();

                log.Info("TVAdmin_004_VerifyServicesFilmingWithVideos Completed" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

            }
            catch (Exception e)
            {

                Console.WriteLine(e.Message + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());
                log.Error(e.Message + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());
                Assert.Fail();
            }
        }


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



