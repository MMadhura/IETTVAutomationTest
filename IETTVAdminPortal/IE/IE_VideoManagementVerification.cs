using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.IE;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Interactions.Internal;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Utility_Classes;
using Utilities.Config;
using System.Threading;
using System.Drawing;
using Utilities.Object_Repository;
using IETTVAdminportal.Reusable;
//using IETTVWebportal.IE;
using log4net;
using log4net.Config;
using System.Reflection;
using System.IO;
using System.Drawing;

namespace IETTVAdminPortal.IE
{
   
    [TestFixture]
    public class IE_VideoManagementVerification
    {
        //// This is to configure logger mechanism for Utilities.Config
        //private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        //#region variable declaration and object initialisation
      
        //IJavaScriptExecutor executor;
        //String videoTitleUsedInRecentVideoSection = "";
        //string driverName = "", driverPath, appURL;
        //internal IWebDriver driver = null;
        //IWait<IWebDriver> iWait = null;
        //String content = "A paragraph from the Ancient Greek paragraphos. To write beside written beside is a self-contained unit of a discourse in writing dealing with a particular point or idea. A paragraph consists of one or more sentences.";

        //String GUID_Admin, VideoID_admin;
        //string videoname;

        //IWebElement AddNewVideo, VideoTitle, ShortDescription, VideoType, DefaultChannel, ChannelTab, PricingTab,
        //            PricingtypeFree, CopyrightTab, CopyrightuploadButton, Notes, UploadButton, SuccessBannerMessage, OkButton, KeywordTab,
        //            AddTagButton, TagName, chooseFileButton, UplaodVideoTab, uploadvideobutton, PreviewButton, FinalFromDate, FinalFromTime,
        //            PublishButton, AttachmentsTab, uploadAttachmentButton, PublishTab;

        ////Instantiating Utilities function class
        //Utility_Functions uf = new Utility_Functions();                                         // Instantiate object for Utility Functons

        //Configuration cf = new Configuration();                                                 // Instantiate object for Configuration

        //Object_Repository_Class OR = new Object_Repository_Class();

        //AdminAuth au = new AdminAuth();

        //IE_VideoSearchResult videoResult = new IE_VideoSearchResult();                            //creating a object for calling IETTVWebPortal project

        //int screenHeight, screenWidth;

        // #endregion 

        //#region Setup

        //[TestFixtureSetUp]
        //public void FixtureSetUp()
        //{
        //    try
        //    {
        //        List<string> globList = cf.readSysConfigFile("Global", "Automation", "SysConfig.xml");

        //        log4net.Config.XmlConfigurator.Configure(new FileInfo(AppDomain.CurrentDomain.BaseDirectory + "AdminPortal.config"));   //to configure the Logger functionality

        //        string baseDir = AppDomain.CurrentDomain.BaseDirectory;                                 // Get path till Base Directory

        //        driverName = "webdriver.ie.driver";                                                     // Driver name for Chrome

        //        driverPath = baseDir + "/IEDriverServer.exe";                                           // path for IE Driver

        //        System.Environment.SetEnvironmentVariable(driverName, driverPath);

        //        InternetExplorerOptions opt = new InternetExplorerOptions();                       // Ensuring Clean IE session

        //        opt.EnsureCleanSession = true;

        //        driver = new InternetExplorerDriver(opt);                                                 // Initialize IE driver  
               

        //        if (uf.getRunningBrowser(driver, driver.FindElement(By.TagName("html"))).ToString().Equals("IE11"))
        //        {

        //            if (uf.checkIE11RegistryPresence().Equals("true"))                                   // Check if Registry Entry is present for IE 11 browser
        //            {
        //                log.Info("Registry Created successfully / Present for IE 11" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());
        //            }
        //            else
        //            {
        //                log.Info("Registry couldn't be created. Test may not run properly in IE 11. Please contact administrator" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());
        //            }

        //        }

               
        //        if (globList.ElementAt(0).ToString().ToLower().Equals("yes"))                           // This is to check if AutoIt setting is set to 'Yes'
        //        {
        //            Boolean statLogin = au.authLogin("IE");

        //            log.Info("Login Status:" + statLogin + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());
        //        }

        //        driver.Manage().Cookies.DeleteAllCookies();

        //        appURL = cf.readingXMLFile("AdminPortal", "Login", "startURL", "Config.xml");

        //        screenHeight = uf.getScreenHeight(driver);

        //        screenWidth = uf.getScreenWidth(driver);

        //        log.Info("Screen Height:" + screenHeight + "Screen Width:" + screenWidth + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

        //        driver.Manage().Window.Position = new System.Drawing.Point(0, 0);

        //        driver.Manage().Window.Size = new Size(screenWidth, screenHeight);

              
            
        //    }
        //    catch (Exception e)
        //    {
        //        log.Error(e.Message + "\n" + e.StackTrace + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

        //        Assert.AreEqual(true, false);
        //    }
        //}

        //[SetUp]
        //public void SetUp()
        //{
        //    try
        //    {
        //        driver.Navigate().GoToUrl(appURL);

        //        Thread.Sleep(9000);

        //        iWait = new OpenQA.Selenium.Support.UI.WebDriverWait(driver, TimeSpan.FromSeconds(60.00));

        //        executor = (IJavaScriptExecutor)driver;
        //    }
        //    catch (Exception e)
        //    {
        //        log.Error(e.Message + "\n" + e.StackTrace + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

        //        Assert.AreEqual(true, false);
        //    }

        //}
      
        //#endregion

        //#region Reusable Function

        ////selecting video management link from Admin Dropdown
        //public void redirectToVideoManagement()
        //{
        //    iWait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy(By.Id("rptMenu_aMenuItem_0")));
            
        //    //clicking on Admin dropdown
        //    driver.FindElement(By.Id("rptMenu_aMenuItem_0")).Click();                                     

        //    Thread.Sleep(3000);

        //    //Clicking on video Management Link
        //    iWait.Until(ExpectedConditions.ElementToBeClickable(By.LinkText("Video Management")));        
        //    driver.FindElement(By.LinkText("Video Management")).Click();                                    

        //    //Click on Add New Button
        //    iWait.Until(ExpectedConditions.ElementToBeClickable(By.Id("ContentPlaceHolder1_btnAddNewRecord")));     //Admin dropdown-Video Management

        //    AddNewVideo = driver.FindElement(By.Id("ContentPlaceHolder1_btnAddNewRecord"));
        //    AddNewVideo.Click();
        //}

        ////This function fill the details on the basic Info tab
        //public void BasicInfoTab()
        //{

        //    iWait.Until(ExpectedConditions.ElementExists(By.Id("ContentPlaceHolder1_ucVideosBasicInformationID_txtBasicInfoTitle")));

        //    //getting GUID of the current video
        //    GUID_Admin = driver.FindElement(By.Id("ContentPlaceHolder1_ucVideosBasicInformationID_txtID")).GetAttribute("value");
        //    log.Info("Guid_Admin:: " + GUID_Admin);

        //    //getting the uique name for the video title
        //    videoname = "vid" + uf.getGuid();

        //    //Enter data in Title field
        //    VideoTitle = driver.FindElement(By.Id("ContentPlaceHolder1_ucVideosBasicInformationID_txtBasicInfoTitle"));
        //    VideoTitle.SendKeys(videoname);

        //    //Storing the videoname in xml
        //    cf.writingIntoXML("AdminPortal", "VideoManagement", "VideoName", videoname , "SysConfig.xml");

        //    //Enter data in ShortDescription field
        //    ShortDescription = driver.FindElement(By.Id("ContentPlaceHolder1_ucVideosBasicInformationID_txtShortDescription"));
        //    ShortDescription.SendKeys("This field is for writing Description");

        //    //Enter data into abstract field
        //    IWebElement abstract_frame = driver.FindElement(By.ClassName("cke_wysiwyg_frame"));
        //    driver.SwitchTo().Frame(abstract_frame);
        //    IWebElement editor_body = driver.FindElement(By.TagName("body"));


        //    OpenQA.Selenium.Interactions.Actions action = new OpenQA.Selenium.Interactions.Actions(driver);
        //    action.SendKeys(editor_body, content).Build().Perform();
        //    driver.SwitchTo().DefaultContent();

        //    iWait.Until(ExpectedConditions.ElementExists(By.Id("ContentPlaceHolder1_ucVideosBasicInformationID_ddlVideoType")));

        //    //selecting the VideoType from the dropdown
        //    VideoType = driver.FindElement(By.Id("ContentPlaceHolder1_ucVideosBasicInformationID_ddlVideoType"));
        //    SelectElement VideoTypeSelector = new SelectElement(VideoType);
        //    VideoTypeSelector.SelectByIndex(3);
        //    String selectedVideoType = VideoTypeSelector.SelectedOption.Text.Trim();
        //}   

        ////This function select the channel
        //public void ChannelListTab()
        //{
        //    //Click on Channel tab
        //    ChannelTab = driver.FindElement(By.Id("channellisttab")).FindElement(By.TagName("a"));   
        //    ChannelTab.Click();

        //    iWait.Until(ExpectedConditions.ElementIsVisible(By.Id("ContentPlaceHolder1_ucVideosBasicInformationID_ddlDefaultChannel")));

        //    //Selecting channel from the default Channel dropdown
        //    DefaultChannel = driver.FindElement(By.Id("ContentPlaceHolder1_ucVideosBasicInformationID_ddlDefaultChannel"));                   
        //    SelectElement DefaultChanneleSelector = new SelectElement(DefaultChannel);

        //    //getting number of channels from default channel dropdown
        //    int NumberofChannels = DefaultChanneleSelector.Options.Count;
           

        //    //check number of channel in the dropdown if it is zero the first create channel
        //    if (NumberofChannels == 0)
        //    {
        //        //Call Create Channel
        //    }
       
        //    DefaultChanneleSelector.SelectByIndex(5);
        //    String selectedDefaultChannel = DefaultChanneleSelector.SelectedOption.Text.Trim();
        //}

        ////This function select the pricing type for the video
        //public void PricingListTab()
        //{
        //    //Click on Pricing tab
        //    PricingTab = driver.FindElement(By.Id("pricingtab")).FindElement(By.TagName("a"));   
        //    PricingTab.Click();

        //    iWait.Until(ExpectedConditions.ElementToBeClickable(By.Id("ContentPlaceHolder1_ucVideosBasicInformationID_rdblPricingType_1")));     

        //    //Cliking on Pricing - 'free' radio button
        //    PricingtypeFree = driver.FindElement(By.Id("ContentPlaceHolder1_ucVideosBasicInformationID_rdblPricingType_1"));                    
        //    PricingtypeFree.Click();
        //}

        ////This function fill the detials required in the Copyright Tab
        //public void CopyrightListTab()
        //{
        //    iWait.Until(ExpectedConditions.ElementToBeClickable(By.Id("copyrighttab")));     

        //    //Click on the CopyrightTab
        //    CopyrightTab = driver.FindElement(By.Id("copyrighttab")).FindElement(By.TagName("a"));
        //    CopyrightTab.Click();

        //    //reading copyright.txt file from xml
        //    String uploadCopyright = cf.readingXMLFile("AdminPortal", "Video_Management", "CopyrightUpload", "Config.xml");  
        //    string uploadCopyrightPath = Environment.CurrentDirectory + "\\Upload\\Documents\\" + uploadCopyright;

        //    //choosefile button
        //    iWait.Until(ExpectedConditions.ElementIsVisible(By.Id("copyrightAttachmentUploadButton")));    

        //    CopyrightuploadButton = driver.FindElement(By.CssSelector("div#divPlCopyrightAttachmentUploadContainer > div > input"));

        //    //calling upload funtionalty from the Utility class to upload the required file
        //    uf.uploadfile(CopyrightuploadButton, uploadCopyrightPath);        


        //    //Write the data in the Notes field
        //    Notes = driver.FindElement(By.Id("ContentPlaceHolder1_ucVideosBasicInformationID_txtCopyrightNotes"));
        //    Notes.SendKeys("This is notes field Automation_test");

        //    //Click on the upload button
        //    UploadButton = driver.FindElement(By.Id("ContentPlaceHolder1_ucVideosBasicInformationID_btnAddCopyright"));
        //    UploadButton.Click();                                           

        //    //  Success banner message
        //    iWait.Until(ExpectedConditions.ElementIsVisible(By.Id("btnOk")));
        //    SuccessBannerMessage = driver.FindElement(By.Id("Sucess_Message"));
          
        //    String copyright_Successful_Message = SuccessBannerMessage.Text;
        //    Assert.AreEqual("Record is Added Successfully.", copyright_Successful_Message);

        //    //Click on ok button banner message
        //    driver.FindElement(By.Id("btnOk")).Click();
           
        //}

        ////This function Add the keywords to the video
        //public void KeywordsTab()
        //{
        //    //wait till banner message 'ok' button from copyright to get invisible
        //    iWait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.Id("btnOk")));       

        //    //click on Keyword Tab
        //    KeywordTab = driver.FindElement(By.Id("keywordsLitab")).FindElement(By.TagName("a"));   
        //    KeywordTab.Click();

        //    //waiting for loader
        //    iWait = new WebDriverWait(driver, TimeSpan.FromSeconds(60));
        //    iWait.Until(ExpectedConditions.ElementExists(By.Id("loading")));

        //    iWait.Until(ExpectedConditions.ElementExists(By.CssSelector("div > span.tagBox-container > a.tagBox-add-tag")));   

        //    //Tag name to be enter
        //    TagName = driver.FindElement(By.CssSelector("div > span.tagBox-container > input.tagBox-input"));    
        //    TagName.SendKeys("AutomationRave");

        //    //click on add tag button
        //    AddTagButton = driver.FindElement(By.CssSelector("div > span.tagBox-container > a.tagBox-add-tag"));
        //    AddTagButton.Click();

        //}

        ////This funtion will attach a documents to the video
        //public void UploadAttachmentTab()
        //{
        //    iWait.Until(ExpectedConditions.ElementToBeClickable(By.Id("attachmenttab")));

        //    //Click on the Attachment Tab
        //    AttachmentsTab = driver.FindElement(By.Id("attachmenttab")).FindElement(By.TagName("a"));       
        //    AttachmentsTab.Click();

        //    iWait.Until(ExpectedConditions.ElementToBeClickable(By.Id("ContentPlaceHolder1_ucVideoAttachment_btnUpload")));  

        //    chooseFileButton = driver.FindElement(By.CssSelector("div#divPlUploadContainer > div > input"));


        //    //reading xml file to upload Attachment
        //    String uploadAttachment = cf.readingXMLFile("AdminPortal", "Video_Management", "AttachmentUpload", "Config.xml");
        //    string uploadAttachmenttPath = Environment.CurrentDirectory + "\\Upload\\Documents\\" + uploadAttachment;
   
        //    uf.uploadfile(chooseFileButton, uploadAttachmenttPath);

        //    //click on upload button
        //    uploadAttachmentButton = driver.FindElement(By.Id("ContentPlaceHolder1_ucVideoAttachment_btnUpload"));  
        //    uploadAttachmentButton.Click();
          
        //    iWait.Until(ExpectedConditions.ElementIsVisible(By.Id("btnOk")));

        //    //Success banner message verification
        //    SuccessBannerMessage = driver.FindElement(By.Id("Sucess_Message"));
           
        //    String Attachment_Successful_Message = SuccessBannerMessage.Text;
 
        //    Assert.AreEqual("Record is added successfully.", Attachment_Successful_Message);

        //    //click on ok button of banner message
        //    OkButton = driver.FindElement(By.Id("btnOk"));  
        //    Actions btnClick = new Actions(driver);
        //    btnClick.MoveToElement(OkButton).Click().Build().Perform();
        //    iWait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.Id("btnOk")));

        //}

        ////This function will upload the required video
        //public void UploadVideoTab()
        //{

        //    iWait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.Id("btnOk")));    
        //    iWait.Until(ExpectedConditions.ElementToBeClickable(By.Id("uploadVideoTab")));

        //    //Click on uplaod video tab
        //    UplaodVideoTab = driver.FindElement(By.Id("uploadVideoTab")).FindElement(By.TagName("a"));
        //    UplaodVideoTab.Click();

        //    //find the div sibling in order to get upload button
        //    IList<IWebElement> siblings = (IList<IWebElement>)driver.FindElements(By.XPath("//input[@id='UploadBrowseButton']/following-sibling::div"));
   

        //    chooseFileButton = siblings[0].FindElement(By.TagName("input"));

          
        //    //reading xml file to upload video
        //    String uploadvideo = cf.readingXMLFile("AdminPortal", "Video_Management", "VideoUpload", "Config.xml");
        //    string uploadvideoPath = Environment.CurrentDirectory + "\\Upload\\Videos\\" + uploadvideo;
 
        //    uf.uploadfile(chooseFileButton, uploadvideoPath);

        //    //   Click on video upload button
        //    uploadvideobutton = driver.FindElement(By.Id("ContentPlaceHolder1_vuVideoUpload_btnUploadVideo"));
        //    uploadvideobutton.Click();

        //    //wait till uploading of the video gets completed
        //    while (true)
        //    {
        //        String value = driver.FindElement(By.Id("ContentPlaceHolder1_vuVideoUpload_btnUploadVideo")).GetAttribute("value").Trim();
        //        if (value.Equals("Uploaded"))            
        //            break;

        //        Thread.Sleep(3000);
        //    }

        //    Thread.Sleep(3000);
           
        //    iWait.Until(ExpectedConditions.ElementIsVisible(By.Id("btnOk")));

        //    //Verify the success banner message
        //    SuccessBannerMessage = driver.FindElement(By.Id("Sucess_Message"));
        //    String VideoUploaded_Successful_Message = SuccessBannerMessage.Text;
     
        //    Assert.AreEqual("Video uploaded successfully.", VideoUploaded_Successful_Message);

        //    //Click on Ok button of banner message
        //    OkButton = driver.FindElement(By.Id("btnOk"));

        //    Actions btnClick = new Actions(driver);

        //    btnClick.MoveToElement(OkButton).Click().Build().Perform();

        //    iWait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.Id("btnOk")));

        //    uf.scrollUp(driver);    //preview button at times not visible so need to scroll
        //    uf.scrollDown(driver);

        //    int count = 0;
        //    String status = null; ;

        //    //checking the status of video for 5mins untill it gets Ready
        //    while (count < 60)  
        //    {
                  
        //        //check status of video
        //        status = driver.FindElement(By.Id("ContentPlaceHolder1_vuVideoUpload_lblStatus")).Text.ToString();

        //        if (status.Equals("Status: READY"))
        //            break;
        //        Thread.Sleep(5000);

        //        count = count + 1;

        //        iWait.Until(ExpectedConditions.ElementToBeClickable(By.Id("ContentPlaceHolder1_vuVideoUpload_lnkPreviewVideo")));

        //        //Click on the preview button
        //        PreviewButton = driver.FindElement(By.Id("ContentPlaceHolder1_vuVideoUpload_lnkPreviewVideo"));         
        //        executor.ExecuteScript("arguments[0].click();", PreviewButton);

        //    }

        //    Assert.AreEqual("Status: READY", status);
        //    Thread.Sleep(2000);
        //}

        ////This funtion will select the Date and Time and click on publish button
        //public void PublishVideoTab()
        //{
        //    //Getting sysytem current date and adding 2minutes in time as video upload time should be greater than system current time
           
        //    String currentDate = DateTime.Now.AddMinutes(2).ToString("dd/MM/yyyy HHmm");
           
        //    String[] dateTime = currentDate.Split(' ');

        //    log.Info("Date :: " + dateTime[0].Trim());
        //    log.Info("Time :: " + dateTime[1].Trim());

        //    iWait.Until(ExpectedConditions.ElementToBeClickable(By.Id("publishtab"))).FindElement(By.TagName("a"));

        //    //Click on Publish tab
        //    PublishTab = driver.FindElement(By.Id("publishtab")).FindElement(By.TagName("a"));
        //    executor.ExecuteScript("arguments[0].click();", PublishTab);            

        //    iWait.Until(ExpectedConditions.ElementToBeClickable(By.Id("ContentPlaceHolder1_ucVideoPublish_txtFinalPublishFromDate")));

        //    //Selecting todays date from date picker
        //    FinalFromDate = driver.FindElement(By.Id("ContentPlaceHolder1_ucVideoPublish_txtFinalPublishFromDate"));
        //    FinalFromDate.SendKeys(dateTime[0].Trim());                                     

        //    iWait.Until(ExpectedConditions.ElementToBeClickable(By.Id("ContentPlaceHolder1_ucVideoPublish_txtFinalPublishFromTime")));

        //    //enter the time in the Final Time field
        //    FinalFromTime = driver.FindElement(By.Id("ContentPlaceHolder1_ucVideoPublish_txtFinalPublishFromTime"));
        //    FinalFromTime.Clear();
        //    FinalFromTime.SendKeys(dateTime[1].Trim());                                                                    

        //    //Clicking on video publish radio button to close date picker
        //    driver.FindElement(By.Id("ContentPlaceHolder1_ucVideoPublish_rdbVideoPublish")).Click();  
        //    Thread.Sleep(2000);

        //    PublishButton = driver.FindElement(By.Id("ContentPlaceHolder1_btPublish"));  
        //    executor.ExecuteScript("arguments[0].click();", PublishButton);

        //    iWait = new WebDriverWait(driver, TimeSpan.FromSeconds(60));
        //    iWait.Until(ExpectedConditions.ElementExists(By.Id("loading")));             //waiting for loader
        //    log.Info("publish loading is over");

        //    Thread.Sleep(3000);



        //   // WebDriverWait waitStyle = new WebDriverWait(driver, TimeSpan.FromSeconds(60));
        //   // waitStyle.Until(d => d.FindElement(By.Id("Default_Success_Language")).Text.Equals("Final Video Published Successfully."));

        //    iWait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy(By.CssSelector("div.successcustommsg.custom_messages")));

        //    iWait.Until(ExpectedConditions.ElementIsVisible(By.Id("btnSuccessOK")));

        //    //Verifying Success banner message
        //    SuccessBannerMessage = driver.FindElement(By.Id("Default_Success_Language"));
        //    String Publish_Successful_Message = SuccessBannerMessage.Text;

        //    Assert.AreEqual("Final Video Published Successfully.", Publish_Successful_Message);

        //    //Click on ok button of banner message
        //    IWebElement OkButtonTwo = driver.FindElement(By.Id("btnSuccessOK"));
        //    Actions btnClick = new Actions(driver);
        //    btnClick.MoveToElement(OkButtonTwo).Click().Build().Perform();
        //    iWait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.Id("btnSuccessOK")));


        //}

        ////This funtion verify the recent video created and hide the record for the same
        //public void RecentVideoVerification()
        //{
        //    //Click on Add New Button

        //    iWait.Until(ExpectedConditions.ElementToBeClickable(By.Id("ContentPlaceHolder1_btnAddNewRecord")));     //waiting for add new button

        //    IList<IWebElement> videoRowList = (IList<IWebElement>)driver.FindElement(By.Id("ContentPlaceHolder1_grdVideoListingRecent")).FindElements(By.TagName("tr"));
          

        //    Boolean flag = false;
        //    int i = 0;
        //    foreach (IWebElement currentRow in videoRowList)
        //    {

        //        //Check Row that have class="GridRowStyle" or class="AltGridStyle"
               
        //        if (currentRow.GetAttribute("class").Equals("GridRowStyle") || currentRow.GetAttribute("class").Equals("AltGridStyle"))
        //        {
                    
        //            String columData = currentRow.FindElements(By.TagName("td"))[0].FindElement(By.TagName("a")).GetAttribute("title").Trim();
        //            log.Info("Video Title ::" + columData);


        //            if (columData.Equals(videoname))
        //            {
        //                flag = true;

        //                //Write assert to check edit button
        //                 Assert.AreEqual(true, currentRow.FindElements(By.TagName("td"))[7].FindElement(By.Id("ContentPlaceHolder1_grdVideoListingRecent_imgEditRecent_" + i)).GetAttribute("src").Contains("Edit.png"));

        //                //click on edit button
        //                IWebElement VideoEditButton = driver.FindElement(By.Id("ContentPlaceHolder1_grdVideoListingRecent_imgEditRecent_" + i));
        //                VideoEditButton.Click();
        //                break;
        //            }
        //            i++;
        //        }
        //    }


        //    //Get videoID
        //    iWait.Until(ExpectedConditions.ElementExists(By.Id("ContentPlaceHolder1_ucVideosBasicInformationID_txtVideoNumber")));

        //    VideoID_admin = driver.FindElement(By.Id("ContentPlaceHolder1_ucVideosBasicInformationID_txtVideoNumber")).GetAttribute("value");

        //    //Get GUID
        //    GUID_Admin = driver.FindElement(By.Id("ContentPlaceHolder1_ucVideosBasicInformationID_txtID")).GetAttribute("value");
        //    log.Info("Guid_Admin:: " + GUID_Admin);

        //    iWait.Until(ExpectedConditions.ElementIsVisible(By.Id("advancedinfotab")));
        //    IWebElement AdvanceTab = driver.FindElement(By.Id("advancedinfotab"));
        //    AdvanceTab.Click();

        //    //Click on Permisison Tab
        //    iWait.Until(ExpectedConditions.ElementExists(By.Id("permissiontabLi")));  
        //    IWebElement PermisionTab = driver.FindElement(By.Id("permissiontabLi"));
        //    PermisionTab.Click();

        //    //Click Hide record radio button
        //    iWait.Until(ExpectedConditions.ElementExists(By.Id("ContentPlaceHolder1_ucAdvancedInformation_chkHidRecord")));   
        //    IWebElement HideRecord = driver.FindElement(By.Id("ContentPlaceHolder1_ucAdvancedInformation_chkHidRecord"));
        //    HideRecord.Click();

        //    iWait.Until(ExpectedConditions.ElementExists(By.Id("publishtab")));  //click on publish tab
        //    PublishTab = driver.FindElement(By.Id("publishtab"));
        //    PublishTab.Click();

        //    PublishButton = driver.FindElement(By.Id("ContentPlaceHolder1_btPublish"));
        //    PublishButton.Click();

        //    iWait = new WebDriverWait(driver, TimeSpan.FromSeconds(60));
        //    iWait.Until(ExpectedConditions.ElementExists(By.Id("loading")));
        //    log.Info("loading is over");

        //    iWait.Until(ExpectedConditions.ElementExists(By.Id("Default_Success_Language")));

        //    //Success banner message
        //    iWait.Until(d => d.FindElement(By.Id("Default_Success_Language")).Text.Equals("Final Video Published Successfully."));
        //    SuccessBannerMessage = driver.FindElement(By.Id("Default_Success_Language"));
        //    String Publish_Successful_Message = SuccessBannerMessage.Text;
           
        //    Assert.AreEqual("Final Video Published Successfully.", Publish_Successful_Message);

        //    //click on ok button of banner  message
        //    OkButton = driver.FindElement(By.Id("btnSuccessOK"));
        //    executor.ExecuteScript("arguments[0].click();", OkButton);

        //}

        //#endregion


        //[Test]
        //public void TVAdmin_001_CreateVideo()
        //{
        //    try
        //    {
        //        log.Info("Createvideo test Started");

        //        redirectToVideoManagement();
        //        BasicInfoTab();
        //        ChannelListTab();
        //        PricingListTab();
        //        CopyrightListTab();
        //        KeywordsTab();
        //        UploadAttachmentTab();
        //        UploadVideoTab();
        //        PublishVideoTab();


        //        //waiting for 3 minutes as video will be publish after 2minutes from current system time
        //        Thread.Sleep(180000);

        //        //calling from IETTVWebportal project to search created video on web portal and verifying the same
        //        videoResult.TVWeb_001_SearchVideofunctionality(driver, iWait, videoname, GUID_Admin);

        //        log.Info("Createvideo test Completed");
        //    }
        //    catch (Exception e)
        //    {
        //        log.Error(e.Message + "\n" + e.StackTrace);
        //        Assert.AreEqual(true, false);

        //    }
        //}


        //[Test]
        //public void TVAdmin_002_HideVideo()
        //{
        //    try
        //    {
        //        log.Info("Hidevideo test started");

        //        #region Creating Video

        //        redirectToVideoManagement();
        //        BasicInfoTab();
        //        ChannelListTab();
        //        PricingListTab();
        //        CopyrightListTab();
        //        KeywordsTab();
        //        UploadAttachmentTab();
        //        UploadVideoTab();
        //        PublishVideoTab();

        //        #endregion

        //        //verify recent video created and edit the same by Selecting hide record button
        //        RecentVideoVerification();

        //        //waiting for 3 minutes as video will be publish after 2minutes from current system time
        //        Thread.Sleep(180000);

        //        //Verify the video on web portal
        //        videoResult.verifyingNoresultFound(driver, iWait, videoname);

        //        log.Info("Hidevideo test Completed");
        //    }
        //    catch (Exception e)
        //    {
        //        Console.WriteLine(e.Message + "\n" + e.StackTrace);
        //        log.Error(e.Message + "\n" + e.StackTrace);
        //        Assert.AreEqual(true, false);

        //    }
        //}


        //[TestFixtureTearDown]
        //public void TearDown()
        //{
        //    try
        //    {
        //        driver.Quit();
        //    }
        //    catch (Exception e)
        //    {
                
        //        log.Error(e.Message + "\n" + e.StackTrace);
        //        Assert.AreEqual(true, false);

        //    }
        //}

       
    }
}
