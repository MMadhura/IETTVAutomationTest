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
using NSoup.Nodes;
using NSoup.Select;
using Sikuli4Net.sikuli_UTIL;
using Sikuli4Net.sikuli_REST;
using System.Globalization;


namespace IETTVAdminPortal.Chrome
{
    [TestFixture]
    public class Chrome_PollManagement
    {

        // This is to configure logger mechanism for Utilities.Config
        public static ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        
        #region variable declaration and object initialisation

        IJavaScriptExecutor executor;

        string driverName = "", driverPath, appURL;

        internal IWebDriver driver = null;

        IWait<IWebDriver> iWait = null;

        List<string> globList;

        //Instantiating Utilities function class

        Utility_Functions uf = new Utility_Functions();                                         // Instantiate object for Utility Functons

        Configuration cf = new Configuration();                                                 // Instantiate object for Configuration

        Object_Repository_Class OR = new Object_Repository_Class();

        Chrome_AdminSetupTearDown st = new Chrome_AdminSetupTearDown();

        Chrome_PollManagementVerification objWebPollManagement = null;

        Chrome_VideoManagement objAdminVideoManagement = null;

        APILauncher launcher;

        #endregion

        #region Constructors

         public Chrome_PollManagement(IWebDriver driver, ILog log1, IJavaScriptExecutor Executor, IWait<IWebDriver> iWait)
        {
            this.driver = driver;
            log = log1;
            this.executor = Executor;
            this.iWait = iWait;

            objAdminVideoManagement = new Chrome_VideoManagement(driver, log, executor, iWait);


        }

         public Chrome_PollManagement() { }

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
            objWebPollManagement = new Chrome_PollManagementVerification(driver, log, executor, iWait);

            appURL = st.Chrome_Setup(driver, log, executor);                                               // Calling Chrome Setup  
        }

        #endregion

        #region Reusable Elements

        public IWebElement chkActive()
        {
            return driver.FindElement((OR.GetElement("PollManagement", "ChkActive", "TVAdminPortalOR.xml")));
        }

        public IWebElement chkDisplayPolling()
        {
            iWait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy((OR.GetElement("PollManagement", "ChkDisplayPolling", "TVAdminPortalOR.xml"))));

            return driver.FindElement((OR.GetElement("PollManagement", "ChkDisplayPolling", "TVAdminPortalOR.xml")));
        }

        public IWebElement chkDisplayQA()
        {
            iWait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy((OR.GetElement("PollManagement", "ChkDisplayQA", "TVAdminPortalOR.xml"))));

            return driver.FindElement((OR.GetElement("PollManagement", "ChkDisplayQA", "TVAdminPortalOR.xml")));
        }

        public IWebElement optPollManagement()
        {
            iWait.Until(ExpectedConditions.ElementToBeClickable((OR.GetElement("PollManagement", "PollManagementLink", "TVAdminPortalOR.xml"))));

            return driver.FindElement((OR.GetElement("PollManagement", "PollManagementLink", "TVAdminPortalOR.xml")));
        }

        public IWebElement rdoLiveVideo()
        {
            iWait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy((OR.GetElement("VideoManagement", "RadioLiveVid", "TVAdminPortalOR.xml"))));

            return driver.FindElement((OR.GetElement("VideoManagement", "RadioLiveVid", "TVAdminPortalOR.xml")));
        }

        public SelectElement ddlSearchBy()
        {
            iWait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy((OR.GetElement("SeriesManagement", "SearchByDDL", "TVAdminPortalOR.xml"))));

            return new SelectElement(driver.FindElement((OR.GetElement("SeriesManagement", "SearchByDDL", "TVAdminPortalOR.xml"))));
        }

        public IWebElement txtSearchPollManagement()
        {
            return driver.FindElement((OR.GetElement("SeriesManagement", "SearchTextBox", "TVAdminPortalOR.xml")));
        }

        public IWebElement btnSearchPollManagement()
        {
            return driver.FindElement((OR.GetElement("SeriesManagement", "SearchButton", "TVAdminPortalOR.xml")));
        }

        public IWebElement txtQuestion()
        {
            return driver.FindElement((OR.GetElement("PollManagement", "QuestionTXT", "TVAdminPortalOR.xml")));
        }

        public IWebElement txtAnswer1()
        {
            return driver.FindElement((OR.GetElement("PollManagement", "Answer1TXT", "TVAdminPortalOR.xml")));
        }

        public IWebElement txtAnswer2()
        {
            return driver.FindElement((OR.GetElement("PollManagement", "Answer2TXT", "TVAdminPortalOR.xml")));
        }


        public IWebElement btnSave()
        {
            iWait.Until(ExpectedConditions.ElementExists((OR.GetElement("PollManagement", "Savebutton", "TVAdminPortalOR.xml"))));

            return driver.FindElement((OR.GetElement("PollManagement", "Savebutton", "TVAdminPortalOR.xml")));
        }

        public IWebElement txtSearchVideoManagement()
        {
            iWait.Until(ExpectedConditions.ElementIsVisible((OR.GetElement("SeriesManagement", "SearchTextBox", "TVAdminPortalOR.xml"))));
            return driver.FindElement((OR.GetElement("SeriesManagement", "SearchTextBox", "TVAdminPortalOR.xml")));
        }


        public IWebElement btnSearchVideoManagement()
        {
            return driver.FindElement((OR.GetElement("SeriesManagement", "SearchButton", "TVAdminPortalOR.xml")));
        }

        

        #endregion

        #region  Reusable Methods

        public void RedirectPollManagement()
        {
            
            log.Info("inside redirectToPollManagement " + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

            //clicking on Admin dropdown   
            objAdminVideoManagement.adminDropdown().Click();

            Thread.Sleep(1000);

            //Clicking on video Management Link

            optPollManagement().Click();

        }

        public String CreateFreeLiveVideoWithPolling()
        {
           return  CreateFreeLiveVideo(false);
        }

        public String CreateFreeLiveVideo(Boolean isQA)
        {
            objAdminVideoManagement.redirectToVideoManagement();

            String videoName = objAdminVideoManagement.basicInfoTab("Live Video ");

            //objAdminVideoManagement.channelTab().Click();

            objAdminVideoManagement.channelListTab();

            objAdminVideoManagement.pricingListTab("Free");

            objAdminVideoManagement.addcopyright();

            objAdminVideoManagement.advanceTab().Click();

            //Permission tab

            iWait.Until(ExpectedConditions.ElementExists((OR.GetElement("VideoManagement", "PermissionTab", "TVAdminPortalOR.xml"))));

            driver.FindElement((OR.GetElement("VideoManagement", "PermissionTab", "TVAdminPortalOR.xml"))).Click();

            log.Info("inside permission tab");

            if (isQA)
                chkDisplayQA().Click();
            else if (!isQA)
            chkDisplayPolling().Click();

            objAdminVideoManagement.publishTab().Click();

            rdoLiveVideo().Click();

            //Getting sysytem current date and adding 2minutes in time as video upload time should be greater than system current time
            String currentDate = DateTime.Now.AddMinutes(2).ToString("dd/MM/yyyy HHmm", CultureInfo.InvariantCulture);

            String[] dateTime = currentDate.Split(' ');

            log.Info("Date to final publish video :: " + dateTime[0].Trim());

            log.Info("Time at video will final publish :: " + dateTime[1].Trim());

            //Selecting todays date from date picker
            objAdminVideoManagement.finalPublishFromDate().Clear();
            objAdminVideoManagement.finalPublishFromDate().SendKeys(dateTime[0].Trim());

            //enter the time in the Final Time field
            objAdminVideoManagement.finalPublishFromTime().Clear();
            objAdminVideoManagement.finalPublishFromTime().SendKeys(dateTime[1].Trim());


            //Selecting bitrates
            driver.FindElement((OR.GetElement("PollManagement", "Costombitrate1", "TVAdminPortalOR.xml"))).SendKeys("400");
            //new SelectElement(driver.FindElement(By.Id("combobox1"))).SelectByIndex(5);
            driver.FindElement((OR.GetElement("PollManagement", "Width1TXT", "TVAdminPortalOR.xml"))).SendKeys("360");
            driver.FindElement((OR.GetElement("PollManagement", "Height1TXT", "TVAdminPortalOR.xml"))).SendKeys("240");

            driver.FindElement((OR.GetElement("PollManagement", "SubmitLiveData", "TVAdminPortalOR.xml"))).Click();

            iWait.Until(ExpectedConditions.InvisibilityOfElementLocated((OR.GetElement("SeriesManagement", "OverlayWait", "TVAdminPortalOR.xml"))));

            //Getting sysytem current date and adding 2minutes in time as video upload time should be greater than system current time
            currentDate = DateTime.Now.AddMinutes(2).ToString("dd/MM/yyyy HHmm",CultureInfo.InvariantCulture);

            dateTime = currentDate.Split(' ');

            log.Info("Date to final publish video :: " + dateTime[0].Trim());

            log.Info("Time at video will final publish :: " + dateTime[1].Trim());

            //Selecting todays date from date picker
            objAdminVideoManagement.finalPublishFromDate().Clear();
            objAdminVideoManagement.finalPublishFromDate().SendKeys(dateTime[0].Trim());

            //enter the time in the Final Time field
            objAdminVideoManagement.finalPublishFromTime().Clear();
            objAdminVideoManagement.finalPublishFromTime().SendKeys(dateTime[1].Trim());

            objAdminVideoManagement.videoPublishButton().Click();

            //waiting for loader
            iWait.Until(ExpectedConditions.ElementExists((OR.GetElement("VideoManagement", "Loading", "TVAdminPortalOR.xml"))));
            log.Info("loading is over");


            //  iWait.Until(d => d.FindElement(By.Id("Default_Success_Language")).Text.Equals("Final Video Published Successfully."));
            // SuccessBannerMessage = driver.FindElement(By.Id("Default_Success_Language"));

            iWait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy((OR.GetElement("VideoManagement", "CustomMsgCSS", "TVAdminPortalOR.xml"))));

            iWait.Until(ExpectedConditions.ElementIsVisible((OR.GetElement("VideoManagement", "SuccessBTN", "TVAdminPortalOR.xml"))));

            //Verifying Success banner message
            IWebElement SuccessBannerMessage = driver.FindElement((OR.GetElement("VideoManagement", "DefaultSuccessLang", "TVAdminPortalOR.xml")));
            String Publish_Successful_Message = SuccessBannerMessage.Text;

            Assert.AreEqual("Live Video Published Successfully.", Publish_Successful_Message);

            //Click on ok button of banner message
            IWebElement OkButton = driver.FindElement((OR.GetElement("VideoManagement", "SuccessBTN", "TVAdminPortalOR.xml")));
            executor.ExecuteScript("arguments[0].click();", OkButton);

            iWait.Until(ExpectedConditions.InvisibilityOfElementLocated((OR.GetElement("VideoManagement", "SuccessBTN", "TVAdminPortalOR.xml"))));

            //waiting for 2.5 minutes as video will be publish after 2minutes from current system time
            Thread.Sleep(150000);

            return videoName;
        }

        public void SearchVideoUnderPoll(String videoName, Boolean isVideo)
        {


            iWait.Until(ExpectedConditions.ElementIsVisible((OR.GetElement("SeriesManagement", "EventRadioButton", "TVAdminPortalOR.xml"))));

            if (!isVideo)
                driver.FindElement((OR.GetElement("SeriesManagement", "EventRadioButton", "TVAdminPortalOR.xml"))).Click();
            else if(isVideo)
                driver.FindElement((OR.GetElement("PollManagement", "Radio0", "TVAdminPortalOR.xml"))).Click();

            uf.isJqueryActive(driver);

            ddlSearchBy().SelectByText("Title");

            txtSearchPollManagement().SendKeys(videoName);

            btnSearchPollManagement().Click();
        }

        public Dictionary<String, String> CreatePoll(String videoName)
        {

            Dictionary<String, String> pollDetails = new Dictionary<string, string>();

            pollDetails.Add("question", "Who is your favourite player?");
            pollDetails.Add("option1", "Anderson");
            pollDetails.Add("option2", "James");

            SearchVideoUnderPoll(videoName,true);

            iWait.Until(ExpectedConditions.ElementIsVisible((OR.GetElement("PollManagement", "DivSearch", "TVAdminPortalOR.xml"))));

            driver.FindElement((OR.GetElement("PollManagement", "AddVidEventLink", "TVAdminPortalOR.xml"))).Click();

            iWait.Until(ExpectedConditions.InvisibilityOfElementLocated((OR.GetElement("SeriesManagement", "OverlayWait", "TVAdminPortalOR.xml"))));

            iWait.Until(d => d.FindElement((OR.GetElement("PollManagement", "PollTypeTXT", "TVAdminPortalOR.xml"))).Text.Length != 0);

         
            txtQuestion().SendKeys(pollDetails["question"]);

            txtAnswer1().SendKeys(pollDetails["option1"]);

            txtAnswer2().SendKeys(pollDetails["option2"]);

            btnSave().Click();

            iWait.Until(ExpectedConditions.ElementIsVisible((OR.GetElement("SeriesManagement", "SuccessOkButton", "TVAdminPortalOR.xml"))));

            Assert.AreEqual("Record saved sucessfully.", driver.FindElement((OR.GetElement("SeriesManagement", "SucessMessage", "TVAdminPortalOR.xml"))).Text.Trim());

            driver.FindElement((OR.GetElement("SeriesManagement", "SuccessOkButton", "TVAdminPortalOR.xml"))).Click();

            iWait.Until(ExpectedConditions.InvisibilityOfElementLocated((OR.GetElement("SeriesManagement", "SuccessOkButton", "TVAdminPortalOR.xml"))));

            return pollDetails;
        }

        public Dictionary<String, String> CreatePollForExistingVideo(String videoName)
        {

            Dictionary<String, String> pollDetails = new Dictionary<string, string>();

            pollDetails.Add("question", "Who is your favourite player?");
            pollDetails.Add("option1", "Anderson");
            pollDetails.Add("option2", "James");

            SearchVideoUnderPoll(videoName, true);

            iWait.Until(ExpectedConditions.ElementIsVisible((OR.GetElement("PollManagement", "DivSearch", "TVAdminPortalOR.xml"))));

            driver.FindElement((OR.GetElement("PollManagement", "AddVidEventLink", "TVAdminPortalOR.xml"))).Click();

            iWait.Until(ExpectedConditions.InvisibilityOfElementLocated((OR.GetElement("SeriesManagement", "OverlayWait", "TVAdminPortalOR.xml"))));

            iWait.Until(d => d.FindElement((OR.GetElement("PollManagement", "PollTypeTXT", "TVAdminPortalOR.xml"))).Text.Length != 0);


            txtQuestion().SendKeys(pollDetails["question"]);

            txtAnswer1().SendKeys(pollDetails["option1"]);

            txtAnswer2().SendKeys(pollDetails["option2"]);

            btnSave().Click();

            iWait.Until(ExpectedConditions.ElementIsVisible((OR.GetElement("SeriesManagement", "SuccessOkButton", "TVAdminPortalOR.xml"))));

            Assert.AreEqual("Record saved sucessfully.", driver.FindElement((OR.GetElement("SeriesManagement", "SucessMessage", "TVAdminPortalOR.xml"))).Text.Trim());

            driver.FindElement((OR.GetElement("SeriesManagement", "SuccessOkButton", "TVAdminPortalOR.xml"))).Click();

            iWait.Until(ExpectedConditions.InvisibilityOfElementLocated((OR.GetElement("SeriesManagement", "SuccessOkButton", "TVAdminPortalOR.xml"))));

            return pollDetails;
        }

        public Dictionary<String, String> CreatePollforEvent(String videoName)
        {

            Dictionary<String, String> pollDetails = new Dictionary<string, string>();

            pollDetails.Add("question", "Who is your favourite player? "+uf.getGuid());
            pollDetails.Add("option1", "Anderson");
            pollDetails.Add("option2", "James");

            SearchVideoUnderPoll(videoName, false);

            iWait.Until(ExpectedConditions.ElementIsVisible((OR.GetElement("PollManagement", "DivSearch", "TVAdminPortalOR.xml"))));

            driver.FindElement((OR.GetElement("PollManagement", "AddVidEventLink", "TVAdminPortalOR.xml"))).Click();

            iWait.Until(ExpectedConditions.InvisibilityOfElementLocated((OR.GetElement("SeriesManagement", "OverlayWait", "TVAdminPortalOR.xml"))));

            iWait.Until(d => d.FindElement((OR.GetElement("PollManagement", "PollTypeTXT", "TVAdminPortalOR.xml"))).Text.Length != 0);


            txtQuestion().SendKeys(pollDetails["question"]);

            txtAnswer1().SendKeys(pollDetails["option1"]);

            txtAnswer2().SendKeys(pollDetails["option2"]);

            btnSave().Click();

            iWait.Until(ExpectedConditions.ElementIsVisible((OR.GetElement("SeriesManagement", "SuccessOkButton", "TVAdminPortalOR.xml"))));

            Assert.AreEqual("Record saved sucessfully.", driver.FindElement((OR.GetElement("SeriesManagement", "SucessMessage", "TVAdminPortalOR.xml"))).Text.Trim());

            driver.FindElement((OR.GetElement("SeriesManagement", "SuccessOkButton", "TVAdminPortalOR.xml"))).Click();

            iWait.Until(ExpectedConditions.InvisibilityOfElementLocated((OR.GetElement("SeriesManagement", "SuccessOkButton", "TVAdminPortalOR.xml"))));

            return pollDetails;
        }

        //selecting video management link from Admin Dropdown
        public void RedirectToVideoManagement()
        {

            log.Info("inside redirectToVideoManagement " + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

            //clicking on Admin dropdown   
            objAdminVideoManagement.adminDropdown().Click();

            Thread.Sleep(1000);

            //Clicking on video Management Link

            objAdminVideoManagement.videoManagementLink().Click();

           
        }

        public Dictionary<String, String> CreateInactivatePoll(String videoName)
        {

            Dictionary<String, String> pollDetails = new Dictionary<string, string>();

            pollDetails.Add("question", "Who is your favourite player? " + uf.getGuid());
            pollDetails.Add("option1", "Anderson");
            pollDetails.Add("option2", "James");


            SearchVideoUnderPoll(videoName, false);

            iWait.Until(ExpectedConditions.ElementIsVisible((OR.GetElement("PollManagement", "DivSearch", "TVAdminPortalOR.xml"))));

            driver.FindElement((OR.GetElement("PollManagement", "AddVidEventLink", "TVAdminPortalOR.xml"))).Click();

            iWait.Until(ExpectedConditions.InvisibilityOfElementLocated((OR.GetElement("SeriesManagement", "OverlayWait", "TVAdminPortalOR.xml"))));

            iWait.Until(d => d.FindElement((OR.GetElement("PollManagement", "PollTypeTXT", "TVAdminPortalOR.xml"))).Text.Length != 0);

            chkActive().Click();

            txtQuestion().SendKeys(pollDetails["question"]);

            txtAnswer1().SendKeys(pollDetails["option1"]);

            txtAnswer2().SendKeys(pollDetails["option2"]);

            btnSave().Click();

            iWait.Until(ExpectedConditions.ElementIsVisible((OR.GetElement("SeriesManagement", "SuccessOkButton", "TVAdminPortalOR.xml"))));

            Assert.AreEqual("Record saved sucessfully.", driver.FindElement((OR.GetElement("SeriesManagement", "SucessMessage", "TVAdminPortalOR.xml"))).Text.Trim());

            driver.FindElement((OR.GetElement("SeriesManagement", "SuccessOkButton", "TVAdminPortalOR.xml"))).Click();

            iWait.Until(ExpectedConditions.InvisibilityOfElementLocated((OR.GetElement("SeriesManagement", "SuccessOkButton", "TVAdminPortalOR.xml"))));

            return pollDetails;
        }


        public void StopAllPoll()
        {
  
            //Using Nsoup here to parse the html table
            Document doc = NSoup.NSoupClient.Parse(driver.PageSource);
            Elements rowListNsoup = doc.GetElementById("ContentPlaceHolder1_grdEventQuestions").GetElementsByTag("tr");

            int rowCounter = 0;
            Boolean flag = false;
            String labelOfPollActionButton = null;

            foreach (Element currentRow in rowListNsoup)
            {
                Attributes attr = currentRow.Attributes;

                //Row that have class="GridRowStyle" or class="AltGridStyle"
                if (attr["class"].Equals("GridRowStyle") || attr["class"].Equals("AltGridStyle"))
                {
                    log.Info("Row Counter :: " + rowCounter);
                    labelOfPollActionButton = driver.FindElement((OR.GetElement("PollManagement", "StopSearchBTN", "TVAdminPortalOR.xml",rowCounter))).GetAttribute("value").Trim();


                    if (labelOfPollActionButton.ToLower().Equals("Stop".ToLower()))
                    {
                        Thread.Sleep(2000);

                        driver.FindElement(OR.GetElement("PollManagement", "StopSearchBTN", "TVAdminPortalOR.xml",rowCounter)).Click();

                        iWait.Until(d => d.FindElement((OR.GetElement("PollManagement", "OkButtonId", "TVAdminPortalOR.xml"))));

                        executor.ExecuteScript("arguments[0].click()", driver.FindElement((OR.GetElement("PollManagement", "OkButtonId", "TVAdminPortalOR.xml"))));

                        iWait.Until(d => d.FindElement((OR.GetElement("SeriesManagement", "OverlayWait", "TVAdminPortalOR.xml"))).GetAttribute("class").Equals(OR.readingXMLFile("SeriesManagement", "OverlayWait", "TVAdminPortalOR.xml"))); 

                        //iWait.Until(ExpectedConditions.InvisibilityOfElementLocated((OR.GetElement("PollManagement", "OkButtonId", "TVAdminPortalOR.xml"))));

                    }
                    rowCounter++;
                }
            }
        }

        public void StartPoll(String question)
        {
              //Using Nsoup here to parse the html table
                Document doc = NSoup.NSoupClient.Parse(driver.PageSource);
                Elements rowListNsoup = doc.GetElementById("ContentPlaceHolder1_grdEventQuestions").GetElementsByTag("tr");

                int rowCounter = 0;
                Boolean flag = false;
                String currentQuestion = null;

                foreach (Element currentRow in rowListNsoup)
                {
                    Attributes attr = currentRow.Attributes;

                    //Row that have class="GridRowStyle" or class="AltGridStyle"
                    if (attr["class"].Equals("GridRowStyle") || attr["class"].Equals("AltGridStyle"))
                    {
                        log.Info("Row Counter :: " + rowCounter);
                        currentQuestion = currentRow.GetElementsByTag("td")[0].GetElementById("ContentPlaceHolder1_grdEventQuestions_lblQuestionText_" + rowCounter).OwnText().Trim();


                        if (currentQuestion.Equals(question))
                        {
                            flag = true;


                           // iWait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy((OR.GetElement("PollManagement", "StopSearchBTN", "TVAdminPortalOR.xml") + rowCounter)));
                            Thread.Sleep(2000);

                            driver.FindElement(OR.GetElement("PollManagement", "StopSearchBTN", "TVAdminPortalOR.xml",rowCounter)).Click();

                            iWait.Until(d => d.FindElement((OR.GetElement("PollManagement", "OkButtonId", "TVAdminPortalOR.xml"))));

                            executor.ExecuteScript("arguments[0].click()", driver.FindElement((OR.GetElement("PollManagement", "OkButtonId", "TVAdminPortalOR.xml"))));

                            iWait.Until(d=> d.FindElement((OR.GetElement("SeriesManagement", "OverlayWait", "TVAdminPortalOR.xml"))).GetAttribute("class").Equals(OR.readingXMLFile("SeriesManagement", "OverlayWait", "TVAdminPortalOR.xml"))); 

                            //write assert to check Channel name is present of not
                            log.Info("Question is created" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());
                            break;
                        }
                        rowCounter++;
                    }
                }

              
                Assert.AreEqual(true, flag);   //If this fails signifies that created Channel is not displayed on MANAGE page 


        }


        public void PollActivation(String question)
        {
            //Using Nsoup here to parse the html table
            Document doc = NSoup.NSoupClient.Parse(driver.PageSource);
            Elements rowListNsoup = doc.GetElementById("ContentPlaceHolder1_grdEventQuestions").GetElementsByTag("tr");

            int rowCounter = 0;
            Boolean flag = false;
            String currentQuestion = null;

            foreach (Element currentRow in rowListNsoup)
            {
                Attributes attr = currentRow.Attributes;

                //Row that have class="GridRowStyle" or class="AltGridStyle"
                if (attr["class"].Equals("GridRowStyle") || attr["class"].Equals("AltGridStyle"))
                {
                    log.Info("Row Counter :: " + rowCounter);
                    currentQuestion = currentRow.GetElementsByTag("td")[0].GetElementById("ContentPlaceHolder1_grdEventQuestions_lblQuestionText_" + rowCounter).OwnText().Trim();


                    if (currentQuestion.Equals(question))
                    {
                        flag = true;

                        driver.FindElement(OR.GetElement("PollManagement", "EventQsEdit", "TVAdminPortalOR.xml",rowCounter)).Click();

                        uf.isJqueryActive(driver);
                        
                        chkActive().Click();

                        btnSave().Click();
                        
                        log.Info("Poll is activated" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

                        break;
                    }
                    rowCounter++;
                }
            }


            Assert.AreEqual(true, flag);   //If this fails signifies that created Channel is not displayed on MANAGE page 

        }

        public void HandleAdminSuccessMessage(String message)
        {
            iWait.Until(ExpectedConditions.ElementIsVisible((OR.GetElement("SeriesManagement", "SuccessOkButton", "TVAdminPortalOR.xml"))));

            Assert.AreEqual(message, driver.FindElement((OR.GetElement("SeriesManagement", "SucessMessage", "TVAdminPortalOR.xml"))).Text.Trim());

            driver.FindElement((OR.GetElement("SeriesManagement", "SuccessOkButton", "TVAdminPortalOR.xml"))).Click();

            iWait.Until(ExpectedConditions.InvisibilityOfElementLocated((OR.GetElement("SeriesManagement", "SuccessOkButton", "TVAdminPortalOR.xml"))));
        }

        public int GenerateRandomOption(int totalOption)
        {
            return new Random().Next(1,totalOption);
        }

        public int[] UpdatingOptionResult(int selectedOption, int[] expectedOptionResponse)
        {

           
            switch (selectedOption)
            {
                case 1:
                    expectedOptionResponse[0] = expectedOptionResponse[0] + 1;
                    break;
                case 2:
                    expectedOptionResponse[1] = expectedOptionResponse[1] + 1;
                    break;
                case 3:
                    expectedOptionResponse[2] = expectedOptionResponse[2] + 1;
                    break;
                case 4:
                    expectedOptionResponse[3] = expectedOptionResponse[3] + 1;
                    break;
                case 5:
                    expectedOptionResponse[4] = expectedOptionResponse[4] + 1;
                    break;
                case 6:
                    expectedOptionResponse[5] = expectedOptionResponse[5] + 1;
                    break;
                case 7:
                    expectedOptionResponse[6] = expectedOptionResponse[6] + 1;
                    break;
            }

            return expectedOptionResponse;
        }

        public Dictionary<String, String> CustomCreatePoll(String videoName, int totalOption, String[] options)
        {

            Dictionary<String, String> pollDetails = new Dictionary<string, string>();

            String GUID = uf.getGuid();

            int subconuter = GUID.Length / 2;

            pollDetails.Add("question", "Who is your favourite player? "+ GUID.Substring(0, subconuter - 1)+" "+GUID.Substring(subconuter, subconuter));

            for (int i = 0; i < totalOption; i++)
            {
                pollDetails.Add("option" + (i + 1), options[i]);
            }

            SearchVideoUnderPoll(videoName, true);

            iWait.Until(ExpectedConditions.ElementIsVisible((OR.GetElement("PollManagement", "DivSearch", "TVAdminPortalOR.xml"))));

            driver.FindElement((OR.GetElement("PollManagement", "AddVidEventLink", "TVAdminPortalOR.xml"))).Click();

            iWait.Until(ExpectedConditions.InvisibilityOfElementLocated((OR.GetElement("SeriesManagement", "OverlayWait", "TVAdminPortalOR.xml"))));

            iWait.Until(d => d.FindElement((OR.GetElement("PollManagement", "PollTypeTXT", "TVAdminPortalOR.xml"))).Text.Length != 0);

            txtQuestion().SendKeys(pollDetails["question"]);

            txtAnswer1().SendKeys(pollDetails["option1"]);

            txtAnswer2().SendKeys(pollDetails["option2"]);


            int j = 0;
            for (int i = 3; i <= totalOption; i++)
            {
                driver.FindElement(OR.GetElement("PollManagement", "AddFieldButton", "TVAdminPortalOR.xml")).Click();

                Thread.Sleep(1000);

                driver.FindElement((OR.GetElement("PollManagement", "DynamicAnsDiv", "TVAdminPortalOR.xml")))
                    .FindElements(OR.GetElement("PollManagement", "TotalOptionRow", "TVAdminPortalOR.xml"))[j].
                    FindElement(OR.GetElement("PollManagement", "TotalOptionTextbox", "TVAdminPortalOR.xml")).SendKeys(pollDetails["option" + i]);

                j++;

            }

            executor.ExecuteScript("arguments[0].click()", btnSave());
            //btnSave().Click();

            iWait.Until(ExpectedConditions.ElementIsVisible((OR.GetElement("SeriesManagement", "OverlayWait", "TVAdminPortalOR.xml"))));

            iWait.Until(ExpectedConditions.TextToBePresentInElement(driver.FindElement((OR.GetElement("SeriesManagement", "SucessMessage", "TVAdminPortalOR.xml"))), "Record saved sucessfully."));
            Assert.AreEqual("Record saved sucessfully.", driver.FindElement((OR.GetElement("SeriesManagement", "SucessMessage", "TVAdminPortalOR.xml"))).Text.Trim());


            executor.ExecuteScript("arguments[0].click()", driver.FindElement((OR.GetElement("SeriesManagement", "SuccessOkButton", "TVAdminPortalOR.xml"))));
            iWait.Until(ExpectedConditions.InvisibilityOfElementLocated((OR.GetElement("SeriesManagement", "OverlayWait", "TVAdminPortalOR.xml"))));

            return pollDetails;
        }

        #endregion


        [Test]
        public void TVAdmin_001_PollManagementFuncForVideo()
        {

            #region Variable Declaration

            int totalOption = 2;

            String[] options = { "Zaheer", "Anderson", "Watson", "AB", "Hamla", "Dravid", "Morkel" };

            int expectedTotalResponse = 0;

            int[] expectedOptionResponse = {0,0,0,0,0,0,0};
            String[] expectedOptionPercentResult = {null,null,null,null,null,null,null};
    
            #endregion 

            try
            {

                log.Info("TVAdmin_001_PollManagementFuncForVideo Test Started" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

                objAdminVideoManagement = new Chrome_VideoManagement(driver,  log,  executor, iWait);

                int maxOption = Convert.ToInt32(cf.readingXMLFile("AdminPortal", "Poll_Management", "maximumOption", "Config.xml"));
              
                totalOption = new Random().Next(2, maxOption);
              
                #region Create Live Free Video

                String videoName = CreateFreeLiveVideoWithPolling();

                #endregion

                RedirectPollManagement();

                #region Create and Start the Poll

                log.Info("Create and Start the Poll Started" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

                Dictionary<String, String> pollDetails = CustomCreatePoll(videoName, totalOption,options);

                Thread.Sleep(1000);

                executor.ExecuteScript("window.scrollTo(0, document.body.scrollHeight)", "");
                executor.ExecuteScript("window.scrollTo(0, 0)", "");
              

                SearchVideoUnderPoll(videoName, true);

                iWait.Until(ExpectedConditions.ElementIsVisible((OR.GetElement("PollManagement", "DivSearch", "TVAdminPortalOR.xml"))));

                driver.FindElement((OR.GetElement("PollManagement", "AddVidEventLink", "TVAdminPortalOR.xml"))).Click();

                //iWait.Until(ExpectedConditions.InvisibilityOfElementLocated((OR.GetElement("SeriesManagement", "OverlayWait", "TVAdminPortalOR.xml"))));

                iWait.Until(d => d.FindElement((OR.GetElement("SeriesManagement", "OverlayWait", "TVAdminPortalOR.xml"))).GetAttribute("class").Equals(OR.readingXMLFile("SeriesManagement", "OverlayWait", "TVAdminPortalOR.xml"))); 

                StopAllPoll();

                StartPoll(pollDetails["question"]); // question

                #endregion

                #region Open new tab and navigate to web portal 

                log.Info("navigate to web portal Started" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

                uf.OpenNewTab(driver);

                log.Info("count ::: " + driver.WindowHandles.Count);

                String browsertype = uf.getRunningBrowser(driver, driver.FindElement(By.TagName("html")));

                uf.SwitchToWebTab(driver, browsertype);

                uf.NavigateWebPortal(cf, driver);

                uf.isJqueryActive(driver);

                iWait.Until(ExpectedConditions.InvisibilityOfElementLocated((OR.GetElement("SeriesManagement", "OverlayWait", "TVAdminPortalOR.xml"))));

                #endregion

                #region Search video and Verify the presence of Poll Panel

                log.Info("Verify the presence of Poll Panel Started" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

                // Search the video on Web Portal
                objWebPollManagement.Search(videoName, null, true);
                uf.isJqueryActive(driver);
                objWebPollManagement.HandlingEmergencyMessage();
                objWebPollManagement.ClickOnVideo(videoName);
                uf.isJqueryActive(driver);
               
                // Verify that poll panel is displayed
                Assert.AreEqual(true, objWebPollManagement.IsPollPanelDisplayed());

                #endregion

                #region Switch to Admin and Stop Poll and Verify on Web Portal

                log.Info("Switch to Admin and Stop Poll and Verify on Web Portal Started" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

                uf.SwitchToAdminTab(driver, browsertype);

                StopAllPoll();

                uf.SwitchToWebTab(driver, browsertype);

               // driver.Navigate().Refresh();
                uf.isJqueryActive(driver);

                // Verify that poll panel is not displayed
                Boolean b = objWebPollManagement.IsPollPanelDisplayed();
                Assert.AreEqual(false, objWebPollManagement.IsPollPanelDisplayed());

                #endregion

                #region Start the Poll

                log.Info("Start the Poll" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

                uf.SwitchToAdminTab(driver, browsertype);
                uf.isJqueryActive(driver);

                StartPoll(pollDetails["question"]);

                Thread.Sleep(2000);

                #endregion

                #region Perform Poll for User 1

                log.Info("Perform Poll for User 1" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

                uf.SwitchToWebTab(driver, browsertype);

                objWebPollManagement.VerifyingPollDetails(pollDetails);

                objWebPollManagement.ClickOnLoginLink();

                objWebPollManagement.Login(cf.readingXMLFile("AdminPortal", "Poll_Management", "userName1", "Config.xml"), cf.readingXMLFile("AdminPortal", "Poll_Management", "Password1", "Config.xml"));

                uf.isJqueryActive(driver);

                objWebPollManagement.HandlingWelcomePopup();

                int selectedOption = GenerateRandomOption(totalOption);

                objWebPollManagement.SelectOption(selectedOption);

                objWebPollManagement.HandlingSuccessMessage("Your poll response has been posted successfully.");

                Dictionary<String, String> result = objWebPollManagement.StoreCurrentResult(totalOption);

                expectedTotalResponse++;

                expectedOptionResponse = UpdatingOptionResult(selectedOption, expectedOptionResponse);

                int j=0;
                for (int i = 0; i < maxOption; i++)
                {
                    Decimal d = Decimal.Divide(expectedOptionResponse[i],expectedTotalResponse);
                    expectedOptionPercentResult[i] = ((double)d * 100).ToString("F");
                }


                Assert.AreEqual(expectedTotalResponse, Convert.ToInt32(result["totalResponse"]));

                for (int i = 0; i < totalOption; i++)
                {
                    Assert.AreEqual(expectedOptionResponse[i], Convert.ToInt32(result["option" + (i + 1) + "Response"]));

                    Assert.AreEqual(expectedOptionPercentResult[i] + "%", result["option" + (i + 1) + "PercentResponse"]);
                }

                 objWebPollManagement.Logout();

                #endregion


                #region Perform Poll for User 2

                log.Info("Perform Poll for User 1" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

                objWebPollManagement.Search(videoName, null, true);

                    objWebPollManagement.HandlingEmergencyMessage();

                objWebPollManagement.ClickOnVideo(videoName);

                uf.isJqueryActive(driver);

                objWebPollManagement.ClickOnLoginLink();

                objWebPollManagement.Login(cf.readingXMLFile("AdminPortal", "Poll_Management", "userName2", "Config.xml"), cf.readingXMLFile("AdminPortal", "Poll_Management", "Password2", "Config.xml"));

                objWebPollManagement.HandlingWelcomePopup();

                selectedOption = GenerateRandomOption(totalOption);

                objWebPollManagement.SelectOption(selectedOption);

                objWebPollManagement.HandlingSuccessMessage("Your poll response has been posted successfully.");

                result = objWebPollManagement.StoreCurrentResult(totalOption);

                expectedTotalResponse++;

                expectedOptionResponse = UpdatingOptionResult(selectedOption, expectedOptionResponse);

               
                for (int i = 0; i < maxOption; i++)
                {
                    Decimal d = Decimal.Divide(expectedOptionResponse[i], expectedTotalResponse);
                    expectedOptionPercentResult[i] = ((double)d * 100).ToString("F");
                }


                Assert.AreEqual(expectedTotalResponse, Convert.ToInt32(result["totalResponse"]));

                for (int i = 0; i < totalOption; i++)
                {
                    Assert.AreEqual(expectedOptionResponse[i], Convert.ToInt32(result["option" + (i + 1) + "Response"]));

                    Assert.AreEqual(expectedOptionPercentResult[i] + "%", result["option" + (i + 1) + "PercentResponse"]);
                }

                objWebPollManagement.Logout();

                log.Info("TVAdmin_001_PollManagementFuncForVideo Test Completed" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());


                #endregion

            }
            catch(Exception e)
            {
                log.Error(e.Message + "\n" + e.StackTrace + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

                Assert.AreEqual(true, false);

            }

        }

        [Test]
        public void TVAdmin_002_PollingDisplayFuncForVideo()
        {
            #region Variable Declaration

            int totalOption = 2;

            String[] options = { "Zaheer", "Anderson", "Watson", "AB", "Hamla", "Dravid", "Morkel" };

            #endregion

            try
            {

                log.Info("TVAdmin_002_PollingDisplayFuncForVideo Test Started" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

                objAdminVideoManagement = new Chrome_VideoManagement(driver, log, executor, iWait);

                totalOption = new Random().Next(2, Convert.ToInt32(cf.readingXMLFile("AdminPortal", "Poll_Management", "maximumOption", "Config.xml")));

                String videoName = CreateFreeLiveVideoWithPolling();

                RedirectPollManagement();
    
                #region  Creating Poll

                log.Info("Creating Poll Started" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

                Dictionary<String, String> pollDetails = CustomCreatePoll(videoName,totalOption,options);

                uf.scrollDown(driver);
                uf.scrollUp(driver);

                SearchVideoUnderPoll(videoName, true);

                iWait.Until(ExpectedConditions.ElementIsVisible((OR.GetElement("PollManagement", "DivSearch", "TVAdminPortalOR.xml"))));

                driver.FindElement((OR.GetElement("PollManagement", "AddVidEventLink", "TVAdminPortalOR.xml"))).Click();

                iWait.Until(ExpectedConditions.InvisibilityOfElementLocated((OR.GetElement("SeriesManagement", "OverlayWait", "TVAdminPortalOR.xml"))));

                StopAllPoll();

                StartPoll(pollDetails["question"]);

                #endregion

                #region Uncheck the Display polling Checkbox under Video Management

                log.Info("Uncheck the Display polling Checkbox under Video Management" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

                //Redirect to Video Management
                RedirectToVideoManagement();

                // Search the Video
                txtSearchVideoManagement().SendKeys(videoName);

                btnSearchVideoManagement().Click();

                // waiting till search operation completed
              //  uf.isJqueryActive(driver);

                iWait.Until(ExpectedConditions.InvisibilityOfElementLocated((OR.GetElement("SeriesManagement", "OverlayWait", "TVAdminPortalOR.xml"))));

                iWait.Until(ExpectedConditions.ElementExists((OR.GetElement("PollManagement", "VideoListSearch", "TVAdminPortalOR.xml"))));

                #region Select Required Video from search result

                //Using Nsoup here to parse the html table
                Document doc = NSoup.NSoupClient.Parse(driver.PageSource);
                Elements rowListNsoup = doc.GetElementById(OR.readingXMLFile("PollManagement", "VideoListSearch", "TVAdminPortalOR.xml")).GetElementsByTag("tr");

                int rowCounter = 0;
                Boolean flag = false;
                String videoTitle = null;

                foreach (Element currentRow in rowListNsoup)
                {
                    Attributes attr = currentRow.Attributes;

                    //Row that have class="GridRowStyle" or class="AltGridStyle"
                    if (attr["class"].Equals("GridRowStyle") || attr["class"].Equals("AltGridStyle"))
                    {
                        log.Info("Row Counter :: " + rowCounter);
                        videoTitle = driver.FindElement(OR.GetElement("PollManagement", "TitleSearchLnk", "TVAdminPortalOR.xml", rowCounter)).GetAttribute("title").Trim();


                        if (videoTitle.ToLower().Equals(videoName.ToLower()))
                        {
                            //Clicknig on Edit button
                            driver.FindElement(OR.GetElement("PollManagement", "EditImgBTN", "TVAdminPortalOR.xml", rowCounter)).Click();

                            uf.isJqueryActive(driver);

                        }
                        rowCounter++;
                    }
                }

                #endregion

                // Uncheck the Display Polling under Advance
                objAdminVideoManagement.advanceTab().Click();

                objAdminVideoManagement.tabPermission().Click();

                objAdminVideoManagement.chkDisplayPolling().Click();

                objAdminVideoManagement.publishTab().Click();

                objAdminVideoManagement.videoPublishButton().Click();

                iWait.Until(ExpectedConditions.ElementIsVisible((OR.GetElement("VideoManagement", "SuccessBTN", "TVAdminPortalOR.xml"))));

                driver.FindElement((OR.GetElement("VideoManagement", "SuccessBTN", "TVAdminPortalOR.xml"))).Click();

                iWait.Until(ExpectedConditions.InvisibilityOfElementLocated((OR.GetElement("VideoManagement", "SuccessBTN", "TVAdminPortalOR.xml"))));

                #endregion

                #region Verfiy on web portal

                log.Info("Verfiy on web portal Started" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

                uf.OpenNewTab(driver);
                
                log.Info("count ::: " + driver.WindowHandles.Count);

                String browsertype = uf.getRunningBrowser(driver, driver.FindElement(By.TagName("html")));

                uf.SwitchToWebTab(driver, browsertype);

                uf.NavigateWebPortal(cf, driver);

                objWebPollManagement.Search(videoName, null, true);
                objWebPollManagement.HandlingEmergencyMessage();
                objWebPollManagement.ClickOnVideo(videoName);
                uf.isJqueryActive(driver);

                Thread.Sleep(2000);

                Assert.AreEqual(false, objWebPollManagement.IsPollPanelDisplayed());

                #endregion

                #region Check the Display polling Checkbox under Video Management

                log.Info("Check the Display polling Checkbox under Video Management Started" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

                uf.SwitchToAdminTab(driver, browsertype);

                // Search the Video
                txtSearchVideoManagement().SendKeys(videoName);

                btnSearchVideoManagement().Click();

                // waiting till search operation completed
                uf.isJqueryActive(driver);
                iWait.Until(ExpectedConditions.InvisibilityOfElementLocated((OR.GetElement("SeriesManagement", "OverlayWait", "TVAdminPortalOR.xml"))));

              

                #region Searching Required Video from search result

                //Using Nsoup here to parse the html table
                doc = NSoup.NSoupClient.Parse(driver.PageSource);
                rowListNsoup = doc.GetElementById(OR.readingXMLFile("PollManagement", "VideoListSearch", "TVAdminPortalOR.xml")).GetElementsByTag("tr");

                rowCounter = 0;
                flag = false;
                videoTitle = null;

                foreach (Element currentRow in rowListNsoup)
                {
                    Attributes attr = currentRow.Attributes;

                    //Row that have class="GridRowStyle" or class="AltGridStyle"
                    if (attr["class"].Equals("GridRowStyle") || attr["class"].Equals("AltGridStyle"))
                    {
                        log.Info("Row Counter :: " + rowCounter);
                        videoTitle = driver.FindElement(OR.GetElement("PollManagement", "TitleSearchLnk", "TVAdminPortalOR.xml",rowCounter)).GetAttribute("title").Trim();


                        if (videoTitle.ToLower().Equals(videoName.ToLower()))
                        {
                            //Clicknig on Edit button
                            driver.FindElement(OR.GetElement("PollManagement", "EditImgBTN", "TVAdminPortalOR.xml",rowCounter)).Click();

                            uf.isJqueryActive(driver);

                        }
                        rowCounter++;
                    }
                }

                #endregion

                // Uncheck the Display Polling under Advance
                objAdminVideoManagement.advanceTab().Click();

                objAdminVideoManagement.tabPermission().Click();

                objAdminVideoManagement.chkDisplayPolling().Click();

                objAdminVideoManagement.publishTab().Click();

                objAdminVideoManagement.videoPublishButton().Click();

                iWait.Until(ExpectedConditions.ElementIsVisible((OR.GetElement("VideoManagement", "SuccessBTN", "TVAdminPortalOR.xml"))));

                driver.FindElement((OR.GetElement("VideoManagement", "SuccessBTN", "TVAdminPortalOR.xml"))).Click();

                iWait.Until(ExpectedConditions.InvisibilityOfElementLocated((OR.GetElement("VideoManagement", "SuccessBTN", "TVAdminPortalOR.xml"))));


                #endregion

                #region Verifying Active poll on Web Portal

                log.Info("Verifying Active poll on Web Portal Started" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

                uf.SwitchToWebTab(driver, browsertype);

                Thread.Sleep(1000);

                objWebPollManagement.Search(videoName, null, true);
                 objWebPollManagement.ClickOnVideo(videoName);
                 uf.isJqueryActive(driver);

                 Thread.Sleep(2000);

                Assert.AreEqual(true, objWebPollManagement.IsPollPanelDisplayed());

                #endregion

                log.Info("TVAdmin_002_PollingDisplayFuncForVideo Test Completed" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());


            }

            catch (Exception e)
            {
                log.Error(e.Message + "\n" + e.StackTrace + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());
                Assert.AreEqual(true, false);
            }

        }


        #region Not Tested Due to bug

        [Test]
        public void TVAdmin_003_PollManagementFuncForEvent()
        {

            #region Variable Declaration

            int expectedTotalResponse = 0;
            int expectedOption1Response = 0;
            int expectedOption2Response = 0;
            double expectedOption1PercentResult = 0.00;
            double expectedOption2PercentResult = 0.00;

            #endregion

            try
            {

                log.Info("Poll Management for event Test Started" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

                #region Create Event

                String eventName = cf.readingXMLFile("AdminPortal", "Event_Management", "eventName", "Config.xml");

                #endregion

                RedirectPollManagement();

                #region Create and Start the Poll

                Dictionary<String, String> pollDetails = CreatePollforEvent(eventName);

                uf.scrollUp(driver);

                SearchVideoUnderPoll(eventName,false);

                iWait.Until(ExpectedConditions.ElementIsVisible((OR.GetElement("PollManagement", "DivSearch", "TVAdminPortalOR.xml"))));

                driver.FindElement((OR.GetElement("PollManagement", "AddVidEventLink", "TVAdminPortalOR.xml"))).Click();

                iWait.Until(ExpectedConditions.InvisibilityOfElementLocated((OR.GetElement("SeriesManagement", "OverlayWait", "TVAdminPortalOR.xml"))));

                StopAllPoll();

                StartPoll(pollDetails["question"]); // question

                #endregion

                #region Open new tab and navigate to web portal
                
                uf.OpenNewTab(driver);

                log.Info("count ::: " + driver.WindowHandles.Count);

                String browsertype = uf.getRunningBrowser(driver, driver.FindElement(By.TagName("html")));

                uf.SwitchToWebTab(driver, browsertype);

                uf.NavigateWebPortal(cf, driver);

                uf.isJqueryActive(driver);

                iWait.Until(ExpectedConditions.InvisibilityOfElementLocated((OR.GetElement("SeriesManagement", "OverlayWait", "TVAdminPortalOR.xml"))));

                #endregion

                #region Search video and Verify the presence of Poll Panel

                objWebPollManagement.Search(eventName, null, false);

                uf.isJqueryActive(driver);

                // Verify that poll panel is displayed
                Assert.AreEqual(true, objWebPollManagement.IsPollPanelDisplayed());

                #endregion

                #region Switch to Admin and Stop Poll and Verify on Web Portal

                uf.SwitchToAdminTab(driver, browsertype);

                StopAllPoll();

                uf.SwitchToWebTab(driver, browsertype);
                // driver.Navigate().Refresh();
                uf.isJqueryActive(driver);

                // Verify that poll panel is not displayed
                Boolean b = objWebPollManagement.IsPollPanelDisplayed();
                Assert.AreEqual(false, objWebPollManagement.IsPollPanelDisplayed());

                #endregion

                #region Start the Poll

                uf.SwitchToAdminTab(driver, browsertype);
                uf.isJqueryActive(driver);

                StartPoll(pollDetails["question"]);

                Thread.Sleep(2000);

                #endregion

                #region Perform Poll for User 1

                uf.SwitchToWebTab(driver, browsertype);

                objWebPollManagement.VerifyingPollDetails(pollDetails);

                //objWebPollManagement.SelectOption(1);

                //objWebPollManagement.VerifyBannerMessage("Please login to continue Polling.");

                objWebPollManagement.ClickOnLoginLink();

                objWebPollManagement.Login(cf.readingXMLFile("AdminPortal", "Poll_Management", "userName1", "Config.xml"), cf.readingXMLFile("AdminPortal", "Poll_Management", "Password1", "Config.xml"));

                objWebPollManagement.HandlingWelcomePopup();

                objWebPollManagement.SelectOption(1);

                objWebPollManagement.HandlingSuccessMessage("Your poll response has been posted successfully.");

                Dictionary<String, String> result = objWebPollManagement.StoreCurrentResult();

                Assert.AreEqual(expectedTotalResponse + 1, Convert.ToInt32(result["totalResponse"]));
                Assert.AreEqual(expectedOption1Response + 1, Convert.ToInt32(result["option1Response"]));
                Assert.AreEqual(expectedOption2Response, Convert.ToInt32(result["option2Response"]));
                Assert.AreEqual((expectedOption1PercentResult + 100.00).ToString("F") + "%", result["option1PercentResult"]);
                Assert.AreEqual(expectedOption1PercentResult.ToString("F") + "%", result["option2PercentResult"]);

                objWebPollManagement.Logout();

                #endregion

                #region Perform Poll for User 2
          
                objWebPollManagement.Search(eventName, null, false);

                uf.isJqueryActive(driver);

                objWebPollManagement.ClickOnLoginLink();

                objWebPollManagement.Login(cf.readingXMLFile("AdminPortal", "Poll_Management", "userName2", "Config.xml"), cf.readingXMLFile("AdminPortal", "Poll_Management", "Password2", "Config.xml"));

                objWebPollManagement.HandlingWelcomePopup();

                objWebPollManagement.SelectOption(2);

                objWebPollManagement.HandlingSuccessMessage("Your poll response has been posted successfully.");

                result = objWebPollManagement.StoreCurrentResult();

                Assert.AreEqual(expectedTotalResponse + 2, Convert.ToInt32(result["totalResponse"]));
                Assert.AreEqual(expectedOption1Response + 1, Convert.ToInt32(result["option1Response"]));
                Assert.AreEqual(expectedOption2Response + 1, Convert.ToInt32(result["option2Response"]));
                Assert.AreEqual((expectedOption1PercentResult + 50.00).ToString("F") + "%", result["option1PercentResult"]);
                Assert.AreEqual((expectedOption1PercentResult + 50.00).ToString("F") + "%", result["option2PercentResult"]);

                objWebPollManagement.Logout();

                #endregion

            }

            catch (Exception e)
            {
                log.Error(e.Message + "\n" + e.StackTrace + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());
                Assert.AreEqual(true, false);
            }

        }

        [Test]
        public void TVAdmin_004_PollActivateInactivateForVideo()
        {
            try
            {

                log.Info("activateInactivateTest  Started" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

                objAdminVideoManagement = new Chrome_VideoManagement(driver, log, executor, iWait);

                String videoName = CreateFreeLiveVideoWithPolling();

                RedirectPollManagement();

                #region  Creating Inactive Poll

                Dictionary<String, String> pollDetails = CreateInactivatePoll(videoName);

                uf.scrollUp(driver);

                SearchVideoUnderPoll(videoName, true);

                iWait.Until(ExpectedConditions.ElementIsVisible((OR.GetElement("PollManagement", "DivSearch", "TVAdminPortalOR.xml"))));

                driver.FindElement((OR.GetElement("PollManagement", "AddVidEventLink", "TVAdminPortalOR.xml"))).Click();

                iWait.Until(ExpectedConditions.InvisibilityOfElementLocated((OR.GetElement("SeriesManagement", "OverlayWait", "TVAdminPortalOR.xml"))));

                StopAllPoll();

                StartPoll("question");

                #endregion

                #region Verfiying Inactive poll on web portal

                uf.OpenNewTab(driver);

                log.Info("count ::: " + driver.WindowHandles.Count);

                String browsertype = uf.getRunningBrowser(driver, driver.FindElement(By.TagName("html")));

                uf.SwitchToWebTab(driver, browsertype);

                uf.NavigateWebPortal(cf, driver);

                objWebPollManagement.Search(videoName, null, true);

                Assert.AreEqual(false, objWebPollManagement.IsPollPanelDisplayed());

                #endregion

                #region Activating the poll

                uf.SwitchToAdminTab(driver, browsertype);

                PollActivation(pollDetails["question"]);

                HandleAdminSuccessMessage("Record saved sucessfully.");

                #endregion 

                #region Verifying Active poll on Web Portal

                uf.SwitchToWebTab(driver, browsertype);

                Thread.Sleep(2000);

                Assert.AreEqual(true, objWebPollManagement.IsPollPanelDisplayed());

                #endregion

                #region Inactivating Poll in Edit mode 

                uf.SwitchToAdminTab(driver, browsertype);

                SearchVideoUnderPoll(videoName, true);

                iWait.Until(ExpectedConditions.ElementIsVisible((OR.GetElement("PollManagement", "DivSearch", "TVAdminPortalOR.xml"))));

                driver.FindElement((OR.GetElement("PollManagement", "AddVidEventLink", "TVAdminPortalOR.xml"))).Click();

                iWait.Until(ExpectedConditions.InvisibilityOfElementLocated((OR.GetElement("SeriesManagement", "OverlayWait", "TVAdminPortalOR.xml"))));

                PollActivation(pollDetails["question"]);

                HandleAdminSuccessMessage("Record saved sucessfully.");



                #endregion


                #region Verfiying Inactive poll on web portal

                uf.SwitchToWebTab(driver, browsertype);

                Assert.AreEqual(false, objWebPollManagement.IsPollPanelDisplayed());

                #endregion

               
            }

            catch (Exception e)
            {
                log.Error(e.Message + "\n" + e.StackTrace + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());
                Assert.AreEqual(true, false);
                
            }


        }

        [Test]
        public void TVAdmin_005_PollActivateInactivateForEvent()
        {
            try
            {

                log.Info("Event activate Inactivate Test  Started" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

                objAdminVideoManagement = new Chrome_VideoManagement(driver, log, executor, iWait);

                String eventName = cf.readingXMLFile("AdminPortal", "Event_Management", "eventName", "Config.xml");

                RedirectPollManagement();

                #region  Creating Inactive Poll

                Dictionary<String, String> pollDetails = CreateInactivatePoll(eventName);

                uf.scrollUp(driver);

                SearchVideoUnderPoll(eventName, true);

                iWait.Until(ExpectedConditions.ElementIsVisible((OR.GetElement("PollManagement", "DivSearch", "TVAdminPortalOR.xml"))));

                driver.FindElement((OR.GetElement("PollManagement", "AddVidEventLink", "TVAdminPortalOR.xml"))).Click();

                iWait.Until(ExpectedConditions.InvisibilityOfElementLocated((OR.GetElement("SeriesManagement", "OverlayWait", "TVAdminPortalOR.xml"))));

                

                StopAllPoll();

                StartPoll("question");

                #endregion

                #region Verfiying Inactive poll on web portal

                uf.OpenNewTab(driver);

                log.Info("count ::: " + driver.WindowHandles.Count);

                String browsertype = uf.getRunningBrowser(driver, driver.FindElement(By.TagName("html")));

                uf.SwitchToWebTab(driver, browsertype);

                uf.NavigateWebPortal(cf, driver);

                objWebPollManagement.Search(eventName, null, false);

                Assert.AreEqual(false, objWebPollManagement.IsPollPanelDisplayed());

                #endregion

                #region Activating the poll

                uf.SwitchToAdminTab(driver, browsertype);

                PollActivation(pollDetails["question"]);

                HandleAdminSuccessMessage("Record saved sucessfully.");

                #endregion

                #region Verifying Active poll on Web Portal

                uf.SwitchToWebTab(driver, browsertype);

                Thread.Sleep(2000);

                Assert.AreEqual(true, objWebPollManagement.IsPollPanelDisplayed());

                #endregion

                #region Inactivating Poll in Edit mode

                uf.SwitchToAdminTab(driver, browsertype);

                SearchVideoUnderPoll(eventName, true);

                iWait.Until(ExpectedConditions.ElementIsVisible((OR.GetElement("PollManagement", "DivSearch", "TVAdminPortalOR.xml"))));

                driver.FindElement((OR.GetElement("PollManagement", "AddVidEventLink", "TVAdminPortalOR.xml"))).Click();

                iWait.Until(ExpectedConditions.InvisibilityOfElementLocated((OR.GetElement("SeriesManagement", "OverlayWait", "TVAdminPortalOR.xml"))));

                PollActivation(pollDetails["question"]);

                HandleAdminSuccessMessage("Record saved sucessfully.");


                #endregion

                #region Verfiying Inactive poll on web portal

                uf.SwitchToWebTab(driver, browsertype);

                Assert.AreEqual(false, objWebPollManagement.IsPollPanelDisplayed());

                #endregion

            }

            catch (Exception e)
            {
                log.Error(e.Message + "\n" + e.StackTrace + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());
                Assert.AreEqual(true, false);
            }


        }

        [Test]
        public void TVAdmin_006_PollingDisplayFuncForEvent()
        {
            try
            {

                log.Info("Display Polling Func Started" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

                objAdminVideoManagement = new Chrome_VideoManagement(driver, log, executor, iWait);

               // String videoName = "Live Video ea0dd883b88942aeadfa24858216f0a3";

                String videoName = CreateFreeLiveVideoWithPolling();

                RedirectPollManagement();

                #region  Creating Poll

                Dictionary<String, String> pollDetails = CreatePoll(videoName);

                uf.scrollUp(driver);

                SearchVideoUnderPoll(videoName, true);

                iWait.Until(ExpectedConditions.ElementIsVisible((OR.GetElement("PollManagement", "DivSearch", "TVAdminPortalOR.xml"))));

                driver.FindElement((OR.GetElement("PollManagement", "AddVidEventLink", "TVAdminPortalOR.xml"))).Click();

                iWait.Until(ExpectedConditions.InvisibilityOfElementLocated((OR.GetElement("SeriesManagement", "OverlayWait", "TVAdminPortalOR.xml"))));

                StopAllPoll();

                StartPoll(pollDetails["question"]);

                #endregion

                #region Uncheck the Display polling Checkbox under Video Mgmt

                //Redirect to Video Management
                RedirectToVideoManagement();

                // Search the Video
                txtSearchVideoManagement().SendKeys(videoName);

                btnSearchVideoManagement().Click();

                // waiting till search operation completed
                uf.isJqueryActive(driver);

                #region Select Required Video from search result

                //Using Nsoup here to parse the html table
                Document doc = NSoup.NSoupClient.Parse(driver.PageSource);
                Elements rowListNsoup = doc.GetElementById(OR.readingXMLFile("PollManagement", "VideoListSearch", "TVAdminPortalOR.xml")).GetElementsByTag("tr");

                int rowCounter = 0;
                Boolean flag = false;
                String videoTitle = null;

                foreach (Element currentRow in rowListNsoup)
                {
                    Attributes attr = currentRow.Attributes;

                    //Row that have class="GridRowStyle" or class="AltGridStyle"
                    if (attr["class"].Equals("GridRowStyle") || attr["class"].Equals("AltGridStyle"))
                    {
                        log.Info("Row Counter :: " + rowCounter);
                        videoTitle = driver.FindElement(OR.GetElement("PollManagement", "TitleSearchLnk", "TVAdminPortalOR.xml",rowCounter)).GetAttribute("title").Trim();


                        if (videoTitle.ToLower().Equals(videoName.ToLower()))
                        {
                            //Clicknig on Edit button
                            driver.FindElement(OR.GetElement("PollManagement", "EditImgBTN", "TVAdminPortalOR.xml",rowCounter)).Click();

                            uf.isJqueryActive(driver);

                        }
                        rowCounter++;
                    }
                }

                #endregion

                // Uncheck the Display Polling under Advance
                objAdminVideoManagement.advanceTab().Click();

                objAdminVideoManagement.tabPermission().Click();

                objAdminVideoManagement.chkDisplayPolling().Click();

                objAdminVideoManagement.publishTab().Click();

                objAdminVideoManagement.videoPublishButton().Click();

                iWait.Until(ExpectedConditions.ElementIsVisible((OR.GetElement("VideoManagement", "SuccessBTN", "TVAdminPortalOR.xml"))));

                driver.FindElement((OR.GetElement("VideoManagement", "SuccessBTN", "TVAdminPortalOR.xml"))).Click();

                iWait.Until(ExpectedConditions.InvisibilityOfElementLocated((OR.GetElement("VideoManagement", "SuccessBTN", "TVAdminPortalOR.xml"))));

                #endregion

                #region Verfiy on web portal

                uf.OpenNewTab(driver);

                log.Info("count ::: " + driver.WindowHandles.Count);

                String browsertype = uf.getRunningBrowser(driver, driver.FindElement(By.TagName("html")));

                uf.SwitchToWebTab(driver, browsertype);

                uf.NavigateWebPortal(cf, driver);

                objWebPollManagement.Search(videoName, null, true);

                Assert.AreEqual(false, objWebPollManagement.IsPollPanelDisplayed());

                #endregion

                #region Check the Display polling Checkbox under Video Mgmt

                uf.SwitchToAdminTab(driver, browsertype);

                // Search the Video
                txtSearchVideoManagement().SendKeys(videoName);

                btnSearchVideoManagement().Click();

                // waiting till search operation completed
                uf.isJqueryActive(driver);

                #region Searching Required Video from search result

                //Using Nsoup here to parse the html table
                doc = NSoup.NSoupClient.Parse(driver.PageSource);
                rowListNsoup = doc.GetElementById(OR.readingXMLFile("PollManagement", "VideoListSearch", "TVAdminPortalOR.xml")).GetElementsByTag("tr");

                rowCounter = 0;
                flag = false;
                videoTitle = null;

                foreach (Element currentRow in rowListNsoup)
                {
                    Attributes attr = currentRow.Attributes;

                    //Row that have class="GridRowStyle" or class="AltGridStyle"
                    if (attr["class"].Equals("GridRowStyle") || attr["class"].Equals("AltGridStyle"))
                    {
                        log.Info("Row Counter :: " + rowCounter);
                        videoTitle = driver.FindElement(OR.GetElement("PollManagement", "TitleSearchLnk", "TVAdminPortalOR.xml",rowCounter)).GetAttribute("title").Trim();


                        if (videoTitle.ToLower().Equals(videoName.ToLower()))
                        {
                            //Clicknig on Edit button
                            driver.FindElement(OR.GetElement("PollManagement", "EditImgBTN", "TVAdminPortalOR.xml",rowCounter)).Click();

                            uf.isJqueryActive(driver);

                        }
                        rowCounter++;
                    }
                }

                #endregion

                // Uncheck the Display Polling under Advance
                objAdminVideoManagement.advanceTab().Click();

                objAdminVideoManagement.tabPermission().Click();

                objAdminVideoManagement.chkDisplayPolling().Click();

                objAdminVideoManagement.publishTab().Click();

                objAdminVideoManagement.videoPublishButton().Click();

                iWait.Until(ExpectedConditions.ElementIsVisible((OR.GetElement("VideoManagement", "SuccessBTN", "TVAdminPortalOR.xml"))));

                driver.FindElement((OR.GetElement("VideoManagement", "SuccessBTN", "TVAdminPortalOR.xml"))).Click();

                iWait.Until(ExpectedConditions.InvisibilityOfElementLocated((OR.GetElement("VideoManagement", "SuccessBTN", "TVAdminPortalOR.xml"))));


                #endregion

                #region Verifying Active poll on Web Portal

                uf.SwitchToWebTab(driver, browsertype);

                Thread.Sleep(2000);

                objWebPollManagement.Search(videoName, null, true);

                Assert.AreEqual(true, objWebPollManagement.IsPollPanelDisplayed());

                #endregion


            }

            catch (Exception e)
            {
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
