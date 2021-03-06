﻿using NUnit.Framework;
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
using System.Diagnostics;
using Utilities.Object_Repository;

namespace IETTVAdminPortal.Chrome
{
    [TestFixture]
   public class Chrome_IndividualChannelPricing
    {
        // This is to configure logger mechanism for Utilities.Config
        private ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
 
        #region Variable_Declration

        IWait<IWebDriver> iWait = null;

        public IWebDriver driver = null;

        string driverName = "", driverPath, appURL;
     
        IJavaScriptExecutor executor;

        Boolean flag =false;

        String columData = null;

        Object_Repository_Class OR = new Object_Repository_Class();

        Utility_Functions uf = new Utility_Functions();                                           // Instantiate object for Utility Functons

        Configuration cf = new Configuration();                                                  // Instantiate object for Configuration

        //AdminAuth au = new AdminAuth();                                                         // Instantiate object for Authentication

        int rowcounter;

        Elements rowListNsoup;

        String ChannelName = null;

        Boolean checkBoxStatus;

        IWebElement checkBox;

        // Instantiate object for creating channel
        Chrome_ChannelManagementVerification channelAdmin = new Chrome_ChannelManagementVerification();

        Chrome_AdminSetupTearDown st = new Chrome_AdminSetupTearDown();                         // Instantiate object for Chrome Setup Teardown

        #endregion

        public Chrome_IndividualChannelPricing()
        {

        }

        public Chrome_IndividualChannelPricing(IWebDriver driver, ILog log1, IJavaScriptExecutor Executor, IWait<IWebDriver> iWait)
        {
            this.driver = driver;

            log = log1;

            this.executor = Executor;

            this.iWait = iWait;

        }

        #region Setup

        [TestFixtureSetUp]
        public void FixtureSetUp()
        {
            try
            {
                log4net.Config.XmlConfigurator.Configure(new FileInfo(AppDomain.CurrentDomain.BaseDirectory + "AdminPortal.config"));     //To configure logger funtionality

                log.Info("Base Directory Admin :: " + AppDomain.CurrentDomain.BaseDirectory);

                string baseDir = AppDomain.CurrentDomain.BaseDirectory;

                driverName = "webdriver.chrome.driver";                                    // Driver name for Chrome

                driverPath = baseDir + "/chromedriver.exe";                                // Path for ChromeDriver

                System.Environment.SetEnvironmentVariable(driverName, driverPath);

                driver = new ChromeDriver();                                               // Initialize chrome driver           

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
            appURL = st.Chrome_Setup(driver, log, executor);                                // Calling Chrome Setup
        }

        #endregion

        #region Reusable Elements

        public IWebElement SaveButton()
        {
            return driver.FindElement(OR.GetElement("BuyChannel", "BtnSaveChanges", "TVAdminPortalOR.xml"));

        }

        public IWebElement ArchiveTextField(int rowcounter)
        {
            return driver.FindElement(OR.GetElement("BuyChannel", "TxtArchivePrice", "TVAdminPortalOR.xml",rowcounter));

        }

        public IWebElement FrontTextField(int rowcounter)
        {
            return driver.FindElement(OR.GetElement("BuyChannel", "TxtFrontPrice", "TVAdminPortalOR.xml",rowcounter));

        }

        public IWebElement AllTextField(int rowcounter)
        {
            return driver.FindElement(OR.GetElement("BuyChannel", "TxtCombinePrice", "TVAdminPortalOR.xml",rowcounter));

        }

        public IWebElement CheckBox(int rowcounter)
        {
            return driver.FindElement(OR.GetElement("BuyChannel", "GridViewCheckRow", "TVAdminPortalOR.xml",rowcounter));

        }

        public IWebElement TotalBuyMoreText()
        {
            return driver.FindElement(By.Id("ContentPlaceHolder1_txtChannelOffer"));
        }

        public IWebElement MessageBuyMoreText()
        {
            return driver.FindElement(By.Id("ContentPlaceHolder1_txtareaChannelOfferDesc"));
        }

        public IWebElement MembershipUrlText()
        {
            return driver.FindElement(By.Id("ContentPlaceHolder1_txtMemberOffer"));
        }

        #endregion


        #region Reusable Functions

        public void WaitOverlay()
        {
            iWait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.Id("overlay")));
        }

        public void RedirectToIndividualChannelPricing()
        {
            iWait.Until(ExpectedConditions.ElementIsVisible(OR.GetElement("BuyChannel", "IndividualPricingLink", "TVAdminPortalOR.xml")));

            // Clicking on IndividualChannelPricing tab
            driver.FindElement(OR.GetElement("BuyChannel", "IndividualPricingLink", "TVAdminPortalOR.xml")).FindElement(By.TagName("a")).Click();

            Thread.Sleep(3000);
        }

        // This function will return status of element checked or not
        public Boolean IsElementChecked(String checkboxID)
        {

            Boolean checkboxStatus = (Boolean)executor.ExecuteScript(" return document.getElementById('" + checkboxID + "').checked");

            log.Info("Status of element Checked :: " + checkboxStatus + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

            return checkboxStatus;

        }

        //This function generates random number for 'Total' 
        public string GetTotalRandomPrice()
        {
            Random r = new Random();

            int r1 = r.Next(150, 250);

            Decimal d = Convert.ToDecimal(r.NextDouble());

            log.Info("Random Value ::: " + (r1 + Decimal.Round(d, 2)).ToString());
            return (r1 + Decimal.Round(d, 2)).ToString();

        }
 
        //This function generates random number
        public string GetRandomPrice()
        {
            Random r = new Random();

            int r1 = r.Next(1, 999);

            Decimal d = Convert.ToDecimal(r.NextDouble());

            log.Info("Random Value ::: " + (r1 + Decimal.Round(d, 2)).ToString());
            return (r1 + Decimal.Round(d, 2)).ToString();
            
        }

        //This function store all the columns of the Individual Pricing table
        public Elements GetRowListNsoup()
        {
            //Using Nsoup here to parse the html table
            Document doc = NSoup.NSoupClient.Parse(driver.PageSource);
           
            Elements rowListNsoup = doc.GetElementById("ContentPlaceHolder1_GridView1").GetElementsByTag("tr");
            
            return rowListNsoup;
        }

        //This function search for the required Channel
        public int FindRequiredChannel(Elements rowListNsoup, String channelName)
        {

            flag = false;

            int rowcounter = 0;
            foreach (Element currentRow in rowListNsoup)
            {
                Attributes attr = currentRow.Attributes;

                //Row that have class="GridRowStyle" or class="AltGridStyle"
                if (attr["class"].Equals("GridRowStyle") || attr["class"].Equals("AltGridStyle"))
                {
                    log.Info("Row Counter :: " + rowcounter);
                    columData = currentRow.GetElementsByTag("td")[1].GetElementById("ContentPlaceHolder1_GridView1_lblName_" + rowcounter).OwnText().Trim();


                    if (columData.Equals(channelName))
                    {
                        flag = true;
                        break;
                    }
                    rowcounter++;
                }
            }
            return rowcounter;
        }

        public String GetChannelName()
        {
            String ChannelName = null;

            // need to add condition to call create channel if no channel present
           
            //Retreving all the rows of  Table 
            int rowCount = driver.FindElement(By.Id("ContentPlaceHolder1_GridView1")).FindElements(By.TagName("tr")).Count;

            //Verify if no record present then need to create 'Paid channel'
            if (rowCount == 1)
            {
                log.Info("No Record Present , need to create channel" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

                ChannelName =  channelAdmin.CreatePaidChannel("13 April");
            }

            else
            {
                IWebElement Firstrecord = driver.FindElement(By.Id("ContentPlaceHolder1_GridView1_lblName_0"));
                ChannelName = Firstrecord.Text;
                log.Info("first record data name " + ChannelName);
            }

            return ChannelName;

        }

        //This function verify the status of all elements as per checkbox status
        public void VerifyStatusOfElements()
        {
            Boolean checkBoxStatus = IsElementChecked("ContentPlaceHolder1_GridView1_chkRow_" + rowcounter);


            // Uncheck the CheckBox and Verify the status of all columns

            //Verify the status of checkbox
            if (checkBoxStatus == true)
            {
                //uncheck the checkBox
                checkBox = driver.FindElement(OR.GetElement("BuyChannel", "GridViewCheckRow", "TVAdminPortalOR.xml",rowcounter));
                checkBox.Click();
            }
            
            //check status of all columns

            //1.checkBox
            Assert.AreEqual(false, IsElementChecked("ContentPlaceHolder1_GridView1_chkRow_" + rowcounter));

            //2.Archive
            Assert.AreEqual(false, ArchiveTextField(rowcounter).Enabled);

            //3.Front
            Assert.AreEqual(false, FrontTextField(rowcounter).Enabled);

            //4.All
            Assert.AreEqual(false, AllTextField(rowcounter).Enabled);

            //perform check operation on checkbox and verify
            checkBox = driver.FindElement(OR.GetElement("BuyChannel", "GridViewCheckRow", "TVAdminPortalOR.xml",rowcounter));
            checkBox.Click();

            //Archive
            Assert.AreEqual(true, ArchiveTextField(rowcounter).Enabled);

            //Front
            Assert.AreEqual(true, FrontTextField(rowcounter).Enabled);

            //All
            Assert.AreEqual(true, AllTextField(rowcounter).Enabled);
        }

        //This Function  enter data in 'Archive','Front', 'All' fields
        public void UpdatingPrice(String expectedArchive, String expectedFront, String expectedAll)
        {
            //Enter Archive front price value
            ArchiveTextField(rowcounter).Clear();
            ArchiveTextField(rowcounter).SendKeys(expectedArchive);

            //Enter random front price value
            FrontTextField(rowcounter).Clear();
            FrontTextField(rowcounter).SendKeys(expectedFront);

            //Enter random All price value
            AllTextField(rowcounter).Clear();
            AllTextField(rowcounter).SendKeys(expectedAll);

            SaveButton().Click();
        }

        public void VerifyBannerMessage()
        {
            iWait.Until(ExpectedConditions.ElementIsVisible(OR.GetElement("SeriesManagement", "SuccessOkButton", "TVAdminPortalOR.xml")));

            Assert.AreEqual(" Individual Channel Pricing Sucessfully Updated".Trim(), driver.FindElement(OR.GetElement("CMS", "Sucess_Message", "TVAdminPortalOR.xml")).Text.Trim());

            driver.FindElement(OR.GetElement("SeriesManagement", "SuccessOkButton", "TVAdminPortalOR.xml")).Click();

        }

        public void OverlayWait()
        {
            iWait.Until(ExpectedConditions.InvisibilityOfElementLocated(OR.GetElement("SeriesManagement", "OverlayWait", "TVAdminPortalOR.xml")));
        }

        //This function  enter content into the BuyMore section
        public void EnterBuyMoreData(String TotalPriceContent, String MessageContent, String MembershipUrlContent)
        {

            TotalBuyMoreText().Clear();
            TotalBuyMoreText().SendKeys(TotalPriceContent);
            log.Info("value inside Total  " + TotalPriceContent);

            MessageBuyMoreText().Clear();
            MessageBuyMoreText().SendKeys(MessageContent);
            log.Info("value inside Message  " + MessageContent);

            MembershipUrlText().Clear();
            MembershipUrlText().SendKeys(MembershipUrlContent);
            log.Info("value inside MembershipUrl  " + MembershipUrlContent);

            SaveButton().Click();
        }
        
        #endregion


        [Test]
        public void TVAdmin_001_VerifyAndUpdateChannelPrice()
        {
            try
            {
                log.Info("TVAdmin_001_VerifyAndUpdateChannelPrice started" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

                RedirectToIndividualChannelPricing();

                Elements rowListNsoup = GetRowListNsoup();

                log.Info("Getting Channel Name" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

                String channelName = GetChannelName();

                int rowcounter = FindRequiredChannel(rowListNsoup, channelName);

                VerifyStatusOfElements();

                //Enter random Archive price value 
                string expectedArchive = GetRandomPrice();
                string expectedFront = GetRandomPrice();
                string expectedAll = GetRandomPrice();

                log.Info("Updating channel price" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

                UpdatingPrice(expectedArchive, expectedFront, expectedAll);

                VerifyBannerMessage();

                OverlayWait();

                log.Info("verifying updated channel price" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

                #region verify the changes

                Assert.AreEqual(expectedArchive, ArchiveTextField(rowcounter).GetAttribute("value"));

                Assert.AreEqual(expectedFront, FrontTextField(rowcounter).GetAttribute("value"));

                Assert.AreEqual(expectedAll, AllTextField(rowcounter).GetAttribute("value"));

                #endregion

                log.Info("TVAdmin_001_VerifyAndUpdateChannelPrice completed" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());
            }

            catch (Exception e)
            {
                log.Error(e.Message + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());
                Assert.AreEqual(true, false);
            }
        }

        [Test]
        public void TVAdmin_002_VerifyValidation()
        {
            try
            {
                log.Info("TVAdmin_002_VerifyValidation started" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

                RedirectToIndividualChannelPricing();

                Elements rowListNsoup = GetRowListNsoup();

                log.Info("Getting Channel Name" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

                String channelName = GetChannelName();

                int rowcounter = FindRequiredChannel(rowListNsoup, channelName);

                #region Verify the blank data validation
               
                //Enter price 
                string expectedArchive = "";
                string expectedFront = GetRandomPrice();
                string expectedAll = GetRandomPrice();

                log.Info("Updating Archive price" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

                uf.scrollUp(driver);

                UpdatingPrice(expectedArchive, expectedFront, expectedAll);

                iWait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("div.infomsg > button.btn-sm")));

                Assert.AreEqual("Please enter valid Archive price".Trim(), driver.FindElement(By.Id("Info")).Text.Trim());
               
                driver.FindElement(By.CssSelector("div.infomsg > button.btn-sm")).Click();

                OverlayWait();

                //Enter price 
                expectedArchive = GetRandomPrice();
                expectedFront = "";
                expectedAll = GetRandomPrice();

                log.Info("Updating Front price" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

                uf.scrollUp(driver);

                UpdatingPrice(expectedArchive, expectedFront, expectedAll);

                iWait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("div.infomsg > button.btn-sm")));

                Assert.AreEqual("Please enter valid Front price".Trim(), driver.FindElement(By.Id("Info")).Text.Trim());

                driver.FindElement(By.CssSelector("div.infomsg > button.btn-sm")).Click();


                OverlayWait();
               
                //Enter price value 
                expectedArchive = GetRandomPrice();
                expectedFront = GetRandomPrice();
                expectedAll = "";

                log.Info("Updating All price" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

                uf.scrollUp(driver);

                UpdatingPrice(expectedArchive, expectedFront, expectedAll);

                iWait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("div.infomsg > button.btn-sm")));

                Assert.AreEqual("Please enter valid All price".Trim(), driver.FindElement(By.Id("Info")).Text.Trim());

                driver.FindElement(By.CssSelector("div.infomsg > button.btn-sm")).Click();


                OverlayWait();
               
                #endregion

                #region Verify the Invalid data validation

                //Enter price 
                expectedArchive = "0.00";
                expectedFront = GetRandomPrice();
                expectedAll = GetRandomPrice();

                log.Info("Updating Archive price" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

                uf.scrollUp(driver);

                UpdatingPrice(expectedArchive, expectedFront, expectedAll);

                iWait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("div.infomsg > button.btn-sm")));

                Assert.AreEqual("Please enter valid Archive price".Trim(), driver.FindElement(By.Id("Info")).Text.Trim());

                driver.FindElement(By.CssSelector("div.infomsg > button.btn-sm")).Click();


                OverlayWait();

                //Enter price 
                expectedArchive = GetRandomPrice();
                expectedFront = "0.00";
                expectedAll = GetRandomPrice();

                log.Info("Updating Front price" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

                uf.scrollUp(driver);

                UpdatingPrice(expectedArchive, expectedFront, expectedAll);

                iWait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("div.infomsg > button.btn-sm")));

                Assert.AreEqual("Please enter valid Front price".Trim(), driver.FindElement(By.Id("Info")).Text.Trim());

                driver.FindElement(By.CssSelector("div.infomsg > button.btn-sm")).Click();

                OverlayWait();

                //Enter price value 
                expectedArchive = GetRandomPrice();
                expectedFront = GetRandomPrice();
                expectedAll = "0.00";

                log.Info("Updating All price" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

                uf.scrollUp(driver);

                UpdatingPrice(expectedArchive, expectedFront, expectedAll);

                iWait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("div.infomsg > button.btn-sm")));

                Assert.AreEqual("Please enter valid All price".Trim(), driver.FindElement(By.Id("Info")).Text.Trim());

                driver.FindElement(By.CssSelector("div.infomsg > button.btn-sm")).Click();

                OverlayWait();

                #endregion


                log.Info("TVAdmin_002_VerifyValidation completed" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());


            }

            catch (Exception e)
            {
                log.Error(e.Message + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());
                Assert.AreEqual(true, false);
            }
        }

        [Test]
        public void TVAdmin_003_VerifyBuyMore()
        {
            try
            {
                log.Info("TVAdmin_003_VerifyBuyMore started" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());


                String TotalPriceContent = GetRandomPrice();

                String MessageContent = "To gain FREE access to all IET.tv Technology Channels, why not become a member? IET Members receive free access as part of their membership package. Join the IET here or continue to Checkout.";

                String MembershipUrlContent = "http://192.168.2.74/IETTVPortal/view/LoginNew.html";

                RedirectToIndividualChannelPricing();

                //Enter content 
                EnterBuyMoreData(TotalPriceContent, MessageContent, MembershipUrlContent);

                VerifyBannerMessage();

                //Verify the changes
                Assert.AreEqual(TotalPriceContent, TotalBuyMoreText().GetAttribute("value"));

                Assert.AreEqual(MessageContent, MessageBuyMoreText().GetAttribute("value"));

                Assert.AreEqual(MembershipUrlContent, MembershipUrlText().GetAttribute("value"));

                log.Info("TVAdmin_003_VerifyBuyMore completed" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

            }

            catch (Exception e)
            {
                log.Error(e.Message + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());
                Assert.AreEqual(true, false);
            }
        }

        [TestFixtureTearDown]
        public void TearDown()
        {
            st.Chrome_TearDown(driver, log);
        }
    }

}
