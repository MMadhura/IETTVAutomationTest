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
using Utilities.Object_Repository;
using System.Drawing;
using System.Diagnostics;

namespace IETTVAdminPortal.Firefox
{
    [TestFixture]
    class Firefox_ConfigurationManagementVerification
    {
        // This is to configure logger mechanism for Utilities.Config
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);


        #region Variable Declaration and Object Instantiation

        String[] columnName = {  "channelOffer", "memberShipOffer", "userSessionTimeoutMinutes", "fromEmailId" ,
                                  "mailRegards","adminCopyright","defaultPlayerID","autoDefaultImage","autoDefaultchannelImage",
                                  "autoSpeakerIconDefaultImage","autoServiceIconDefaultImage","adminNotificationValueInDays",
                                  "accountRenewalNotificationFrequency","notificationValueInDays","notificationFrequencyInDays",
                                  "welcomeMessage","toEmailid","notificationDetailsPath","replacementTextForHiddenComment","buyChannelsText",
                                  "autoSeriesIconDefaultImage","adminToEmailId" 
                                  };


        internal IWebDriver driver = null;
       
        string appURL;

        IJavaScriptExecutor executor;

        Boolean flag = false;

        //Instantiating Utilities function class

        Utility_Functions uf = new Utility_Functions();

        Configuration cf = new Configuration();                                                 // Instantiate object for Configuration

        Object_Repository_Class OR = new Object_Repository_Class();

        AdminAuth au = new AdminAuth();                                                     // Instantiate object for Authentication

        IWait<IWebDriver> iWait = null;

        Firefox_AdminSetupTearDown st = new Firefox_AdminSetupTearDown();                       // Instantiate object for Firefox Setup Teardown

        #endregion


        #region Setup

        [TestFixtureSetUp]
        public void FixtureSetUp()
        {
            log4net.Config.XmlConfigurator.Configure(new FileInfo(AppDomain.CurrentDomain.BaseDirectory + "AdminPortal.config"));   //to configure the Logger functionality

            log.Info("Base Directory Admin :: " + AppDomain.CurrentDomain.BaseDirectory);

            List<string> globList = cf.readSysConfigFile("Global", "Automation", "SysConfig.xml");

            if (globList.ElementAt(0).ToString().ToLower().Equals("yes"))    // This is to check if AutoIt setting is set to 'Yes'                      
            {
                driver = new FirefoxDriver();
            }
            else
            {
                FirefoxProfile profile = new FirefoxProfile();
                profile.SetPreference("network.automatic-ntlm-auth.trusted-uris", "http://192.168.2.74");
                driver = new FirefoxDriver(profile);
            }

            // This is to check if AutoIt setting is set to 'Yes'
            if (globList.ElementAt(0).ToString().ToLower().Equals("yes"))
            {
                Boolean statLogin = au.authLogin("Firefox");

                log.Info("Login Status:" + statLogin + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());
            }

            executor = (IJavaScriptExecutor)driver;

            iWait = new WebDriverWait(driver, TimeSpan.FromSeconds(60)); 
        }


        [SetUp]
        public void SetUp()
        {
            appURL = st.Firefox_Setup(driver, log, executor);             // Calling Firefox Setup
        }


        #endregion


        #region  Reusable functions

        public void WaitOverlay()
        {
            log.Info("Inside WaitOverlay Function " + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());
           
            iWait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.Id("overlay")));
        }

        public int SearchText(string Text)
        {
            log.Info("Inside SearchText Function " + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());
           
            WaitOverlay();

            //NSoup to parse the code of Page.
            Document doc = NSoup.NSoupClient.Parse(driver.PageSource);
            Elements rowListNsoup = doc.GetElementById("config_parameter").GetElementsByTag("tr");

            log.Info("row: " + rowListNsoup.Count);

            // Retreving all the rows of Manage Table 
            IList<IWebElement> rowList = (IList<IWebElement>)driver.FindElement(By.Id("config_parameter")).FindElements(By.TagName("tr"));
            //Elements rowListNsoup = doc.GetElementById("config_parameter").GetElementsByTag("tbody")[0].GetElementsByTag("tr");

            Boolean flag = false;

            int rowcounter = 0;
            foreach (Element currentRow in rowListNsoup)
            {
                Attributes attr = currentRow.Attributes;

                //Row that have class="GridRowStyle" or class="AltGridStyle
                if (attr["class"].Equals("config_table"))
                {
                    String columData = currentRow.GetElementsByTag("td")[0].GetElementsByTag("b")[0].OwnText().Trim();

                    if (columData.Equals(Text))
                    {
                        log.Info("column :" + columData);
                        flag = true;
                        break;
                    }
                                           
                    rowcounter++;
                }
            }

            return rowcounter;
        }

        public void PopoverFunc(int rowCounter)
        {
            log.Info("Inside PopoverFunc Function " + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());
            
            // Retreving all the rows of Manage Table 
            IWebElement row = driver.FindElement(By.Id("config_parameter")).FindElements(By.ClassName("config_table"))[rowCounter];

            row.FindElements(By.TagName("td"))[0].FindElement(By.TagName("span")).Click();

            iWait.Until(d => d.FindElement(By.ClassName("popover")).Displayed);

            uf.isJqueryActive(driver);

            Assert.AreEqual(true, row.FindElements(By.TagName("td"))[0].FindElement(By.ClassName("popover")).Displayed);

            row.FindElements(By.TagName("td"))[0].FindElement(By.TagName("span")).Click();

            iWait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.ClassName("popover")));

            // iWait.Until(d => !d.FindElement(By.ClassName("popover")).Displayed);

            uf.isJqueryActive(driver);

        }

        public void RedirectToConfigurationManagement()
        {
            log.Info("Inside RedirectToConfigurationManagement Function " + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());
           
            iWait.Until(ExpectedConditions.ElementIsVisible(By.Id("rptMenu_aMenuItem_0")));

            // Clicking on Admin dropdown
            driver.FindElement(By.Id("rptMenu_aMenuItem_0")).Click();

            Thread.Sleep(3000);

            iWait.Until(ExpectedConditions.ElementToBeClickable(By.LinkText("Configuration Management")));

            // Clicking on Configuration Management
            driver.FindElement(By.LinkText("Configuration Management")).Click();

        }

        public void VerifyBannerMessage()
        {
            log.Info("Inside VerifyBannerMessage Function " + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());
            
            iWait.Until(ExpectedConditions.ElementIsVisible(By.Id("btnOk")));

            Assert.AreEqual(" Record(s) updated successfully.".Trim(), driver.FindElement(By.Id("Sucess_Message")).Text.Trim());

            driver.FindElement(By.Id("btnOk")).Click();

            WaitOverlay();


        }

        //This Function will replace old image name with new image      
        public string ImageName(String oldImagePath, String newImageName, String defaultImageName)
        {
            log.Info("Inside ImageName Function " + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());


            int lastIndex = oldImagePath.LastIndexOf("/");

            //substring
            if (oldImagePath.Substring(lastIndex + 1).Equals(defaultImageName))
                return oldImagePath.Replace(oldImagePath.Substring(lastIndex + 1), newImageName);
            else
                return oldImagePath.Replace(oldImagePath.Substring(lastIndex + 1), defaultImageName);

        }



        #endregion

        #region  Reusable Elements

        public IWebElement GetValueField(int value)
        {
            return driver.FindElement(By.Id("ContentPlaceHolder1_repConfiguration_txtConfigValue_" + value));
        }

        public IWebElement EditButton(int value)
        {
            return driver.FindElement(By.Id("ContentPlaceHolder1_repConfiguration_lnkEdit_" + value));
        }

        public IWebElement CancelButton(int value)
        {
            return driver.FindElement(By.Id("ContentPlaceHolder1_repConfiguration_lnkCancel_" + value));
        }

        public IWebElement SaveButton(int value)
        {
            return driver.FindElement(By.Id("ContentPlaceHolder1_repConfiguration_lnkEdit_" + value));
        }

        #endregion


        [Test]
        public void TVAdmin_001_VerMandatoryField()
        {
            try
            {
                log.Info("TVAdmin_001_VerifyMandatoryField started" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

                //redirect to Configuration Management
                RedirectToConfigurationManagement();

                //Search for the required data
                log.Info("searching  : ChannelOffer");

                int position = SearchText("ChannelOffer");

                //get the 'value' field of the required data
                IWebElement valueField = GetValueField(position);

                //verify the status of 'value filed'
                Assert.AreEqual(false, valueField.Enabled);

                //Verify the text of Edit button
                Assert.AreEqual("Edit", EditButton(position).GetAttribute("value"));

                //Click on Edit button
                EditButton(position).Click();

                Assert.AreEqual(true, GetValueField(position).Enabled);

                //1.verify the save button text
                Assert.AreEqual("Save", SaveButton(position).GetAttribute("value"));

                //clear the data inside 'value' field
                GetValueField(position).Clear();

                SaveButton(position).Click();

                Thread.Sleep(3000);

                
                //verify the mandatory field for blank data
                log.Info("verifying the mandatory field for blank data" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

                String borderColor =  GetValueField(position).GetCssValue("border-left-color").ToString();
                log.Info("border color  :" + borderColor);

                iWait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector(".textbox.textareaAutoHeight.form-control.input-height.textbox_border")));

                //IList<IWebElement> ele = driver.FindElements(By.CssSelector(".textbox.textareaAutoHeight.form-control.input-height.textbox_border"));
          
                Assert.AreEqual("rgba(255, 0, 0, 1)", borderColor);

                String borderText = driver.FindElement(By.ClassName("validate_value")).Text;
                Assert.AreEqual("Please enter some value", borderText);

                //2.Verify the mandatory field in edit mode
                log.Info("Verifying the mandatory field in edit mode" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

                CancelButton(position).Click();

                uf.isJqueryActive(driver);

                EditButton(position).Click();

                //edit data of any other field
                position = SearchText("MemberShipOffer");

                //get the 'value' field of the required data
                valueField = GetValueField(position);

                //Click on Edit button
                EditButton(position).Click();
                iWait.Until(ExpectedConditions.ElementIsVisible(By.ClassName("textbox_border")));

                Thread.Sleep(3000);
                borderColor = driver.FindElement(By.Id("ContentPlaceHolder1_repConfiguration_txtConfigValue_0")).GetCssValue("border-left-color").ToString();
                Assert.AreEqual("rgba(255, 0, 0, 1)", borderColor);

                Assert.AreEqual(false, GetValueField(position).Enabled);
               
                log.Info("TVAdmin_001_VerifyMandatoryField completed" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

            }

            catch (Exception e)
            {
                log.Error(e.Message + "\n" + e.StackTrace + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

                Assert.AreEqual(true, false);
            }
        }

        [Test]
        public void TVAdmin_002_VerAllFieldsPopOver()
        {
            try
            {
                log.Info("TVAdmin_002_VerifyAllFieldsPopOver Started" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

                //redirect to Configuration Management
                RedirectToConfigurationManagement();

                iWait.Until(ExpectedConditions.ElementIsVisible(By.Id("ContentPlaceHolder1_repConfiguration_lnkEdit_0")));

                for (int i = 0; i < columnName.Length; i++)
                {
                    PopoverFunc(i);

                }

                log.Info("TVAdmin_002_VerifyAllFieldsPopOver completed" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());
            }



            catch (Exception e)
            {
                log.Error(e.Message + "\n" + e.StackTrace + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

                Assert.AreEqual(true, false);
            }
        }
        
        [Test]
        public void TVAdmin_003_VerCancelButtonFunc()
        {
            try
            {
                log.Info("TVAdmin_003_VerifyCancelButtonFunc Started" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

                //redirect to Configuration Management
                RedirectToConfigurationManagement();

                iWait.Until(ExpectedConditions.ElementIsVisible(By.Id("ContentPlaceHolder1_repConfiguration_lnkEdit_0")));

                for (int i = 0; i < columnName.Length - 1; i++)
                {
                    string newContent = cf.readingXMLFile("AdminPortal", "Configuration_Management", columnName[i], "Config.xml");

                    //get the 'value' field of the required data
                    IWebElement valueField = GetValueField(i);

                    //Verify the status of 'Value' field
                    Assert.AreEqual(false, valueField.Enabled);

                    //Verify the text of Action Column

                    log.Info("Verifying Action Column status" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

                    //1.Edit button
                    Assert.AreEqual("Edit", EditButton(i).GetAttribute("value"));

                    //2.Verify the status of cancel button
                    Assert.AreEqual(false, CancelButton(i).Displayed);

                    EditButton(i).Click();

                    //verify the save button text
                    Assert.AreEqual("Save", SaveButton(i).GetAttribute("value"));

                    // Verfiy that Cancel button displayed
                    Assert.AreEqual(true, CancelButton(i).Displayed);

                    //Verify the status of 'Value' field
                    log.Info("Verifying 'Value' field status" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

                    Assert.AreEqual(true, valueField.Enabled);

                    //Verify the Data inside 'Value' field
                    valueField = GetValueField(i);
                    String valueFieldContent = valueField.Text.Trim();

                    // Changing the content of value field and clicking on Cancel button
                    valueField.Clear();
                    valueField.SendKeys(newContent);
                    CancelButton(i).Click();

                    uf.isJqueryActive(driver);
                    Assert.AreEqual(valueFieldContent, GetValueField(i).Text.Trim());

                    log.Info("TVAdmin_003_VerifyCancelButtonFunc completed" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());


                }
            }

            catch (Exception e)
            {
                log.Error(e.Message + "\n" + e.StackTrace + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

                Assert.AreEqual(true, false);
            }
            
        }

        [Test]
        public void TVAdmin_004_VerSaveButtonFunc()
        {
            try
            {

                log.Info("TVAdmin_004_VerifySaveButtonFunc started" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

                //redirect to Configuration Management
                RedirectToConfigurationManagement();

                iWait.Until(ExpectedConditions.ElementIsVisible(By.Id("ContentPlaceHolder1_repConfiguration_lnkEdit_0")));

                for (int i = 0; i < columnName.Length; i++)
                {
                    log.Info("Column Name  :" + columnName[i]);

                    string newContent = cf.readingXMLFile("AdminPortal", "Configuration_Management", columnName[i], "Config.xml");

                    //get the 'value' field of the required data
                    IWebElement valueField = GetValueField(i);

                    //Verify the status of 'Value' field
                    Assert.AreEqual(false, valueField.Enabled);

                    //Verify the text of Action Column

                    log.Info("Verifying Action Column status" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

                    //1.Edit button
                    Assert.AreEqual("Edit", EditButton(i).GetAttribute("value"));

                    //2.Verify the status of cancel button
                    Assert.AreEqual(false, CancelButton(i).Displayed);
                    Thread.Sleep(2000);

                    EditButton(i).Click();

                    //verify the save button text
                    Assert.AreEqual("Save", SaveButton(i).GetAttribute("value"));

                    // Verfiy that Cancel button displayed
                    Assert.AreEqual(true, CancelButton(i).Displayed);

                    //Verify the status of 'Value' field
                    log.Info("Verifying 'Value' field status" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

                    Assert.AreEqual(true, valueField.Enabled);

                    //Verify the Data inside 'Value' field
                    valueField = GetValueField(i);
                    String valueFieldContent = valueField.Text.Trim();

                    string imagesfolderPath = cf.readingXMLFile("AdminPortal", "Configuration_Management", "imagesPath", "Config.xml");

                    #region Default image
                    if (columnName[i].Equals("autoDefaultImage"))
                    {
                        string newDefaultImageContent = cf.readingXMLFile("AdminPortal", "Configuration_Management", "autoDefaultImage", "Config.xml");
                        string oldDefaultImageContent = cf.readingXMLFile("AdminPortal", "Configuration_Management", "defaultImage", "Config.xml");


                        //verifying image presence in the required folder path
                        DirectoryInfo di = new DirectoryInfo(imagesfolderPath);
                        FileInfo[] files = di.GetFiles(newDefaultImageContent);

                        if (!files[0].Name.Equals("Autologo.png"))
                            throw new Exception();

                        newContent = ImageName(GetValueField(i).Text.ToString(), newDefaultImageContent, oldDefaultImageContent);
                        log.Info("newpath  " + newContent);
                    }
                    #endregion

                    #region Default Channel Image
                    else if (columnName[i].Equals("autoDefaultchannelImage"))
                    {
                        string newDefaultchannelImageContent = cf.readingXMLFile("AdminPortal", "Configuration_Management", "autoDefaultchannelImage", "Config.xml");
                        string oldDefaultChannelImageContent = cf.readingXMLFile("AdminPortal", "Configuration_Management", "defaultChannelImage", "Config.xml");

                        //verifying image presence in the required folder path
                        DirectoryInfo di = new DirectoryInfo(imagesfolderPath);
                        FileInfo[] files = di.GetFiles(newDefaultchannelImageContent);

                        if (!files[0].Name.Equals("AutoDefaultChannel.png"))
                            throw new Exception();

                        newContent = ImageName(GetValueField(i).Text.ToString(), newDefaultchannelImageContent, oldDefaultChannelImageContent);
                        log.Info("newpath  " + newContent);
                    }
                    #endregion

                    #region Default Speaker
                    else if (columnName[i].Equals("autoSpeakerIconDefaultImage"))
                    {
                        string newSpeakerIconDefaultImageContent = cf.readingXMLFile("AdminPortal", "Configuration_Management", "autoSpeakerIconDefaultImage", "Config.xml");
                        string oldDefaultSpeakerImageContent = cf.readingXMLFile("AdminPortal", "Configuration_Management", "defaultSpeakerIconImage", "Config.xml");

                        //verifying image presence in the required folder path
                        DirectoryInfo di = new DirectoryInfo(imagesfolderPath);
                        FileInfo[] files = di.GetFiles(newSpeakerIconDefaultImageContent);

                        if (!files[0].Name.Equals("AutoDefaultSpeaker.png"))
                            throw new Exception();

                        newContent = ImageName(GetValueField(i).Text.ToString(), newSpeakerIconDefaultImageContent, oldDefaultSpeakerImageContent);
                        log.Info("newpath  " + newContent);
                    }
                    #endregion

                    #region Default Service
                    else if (columnName[i].Equals("autoServiceIconDefaultImage"))
                    {
                        string newServiceIconDefaultImageContent = cf.readingXMLFile("AdminPortal", "Configuration_Management", "autoServiceIconDefaultImage", "Config.xml");
                        string oldDefaultServiceImageContent = cf.readingXMLFile("AdminPortal", "Configuration_Management", "defaultServiceIconImage", "Config.xml");

                        //verifying image presence in the required folder path
                        DirectoryInfo di = new DirectoryInfo(imagesfolderPath);
                        FileInfo[] files = di.GetFiles(newServiceIconDefaultImageContent);

                        if (!files[0].Name.Equals("AutoDefaultService.png"))
                            throw new Exception();

                        newContent = ImageName(GetValueField(i).Text.ToString(), newServiceIconDefaultImageContent, oldDefaultServiceImageContent);
                        log.Info("newpath  " + newContent);
                    }

                    #endregion

                    #region Default Series
                    else if (columnName[i].Equals("autoSeriesIconDefaultImage"))
                    {
                        string newSeriesIconDefaultImageContent = cf.readingXMLFile("AdminPortal", "Configuration_Management", "autoSeriesIconDefaultImage", "Config.xml");
                        string oldDefaultSeriesImageContent = cf.readingXMLFile("AdminPortal", "Configuration_Management", "defaultSeriesIconDefaultImage", "Config.xml");

                        //verifying image presence in the required folder path
                        DirectoryInfo di = new DirectoryInfo(imagesfolderPath);
                        FileInfo[] files = di.GetFiles(oldDefaultSeriesImageContent);

                        if (!files[0].Name.Equals("AutoDefaultSeries.png"))
                            throw new Exception();

                        newContent = ImageName(GetValueField(i).Text.ToString(), newSeriesIconDefaultImageContent, oldDefaultSeriesImageContent);
                        log.Info("newpath  " + newContent);
                    }

                    #endregion

                    // Changing the content of value field and clicking on Save button

                    GetValueField(i).Clear();

                    GetValueField(i).SendKeys(newContent);

                    SaveButton(i).Click();

                    uf.isJqueryActive(driver);

                    #region Verifying success banner message

                    VerifyBannerMessage();

                    #endregion


                    // iWait.Until(ExpectedConditions.ElementIsVisible(By.Id("ContentPlaceHolder1_repConfiguration_lnkEdit_0")));

                    //Verify the new content
                    Assert.AreEqual(newContent, GetValueField(i).Text.Trim());
                    log.Info("TVAdmin_004_VerifySaveButtonFunc completed" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

                }
            }



            catch (Exception e)
            {
                log.Error(e.Message + "\n" + e.StackTrace + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

                Assert.AreEqual(true, false);
            }
        }

        [TestFixtureTearDown]
        public void TearDown()
        {
            st.Firefox_TearDown(driver, log);
        }
    }
}
