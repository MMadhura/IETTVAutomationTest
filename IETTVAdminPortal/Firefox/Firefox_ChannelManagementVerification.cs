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
using NSoup.Nodes;
using NSoup.Select;
using log4net;
using log4net.Config;
using System.Reflection;
using System.IO;
using System.Diagnostics;

namespace IETTVAdminPortal.Firefox
{
   
    [TestFixture]
    public class Firefox_ChannelManagementVerification
    {
        // This is to configure logger mechanism for Utilities.Config
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType); 

        #region Variable Declaration and Object Instantiation

        String backgroundHexColor, buttonHexColor, successMessage;

        int numberOfCategory, randomSelector;

        IWebElement ChannelCategory, ChannelType, ChannelPosition, PricingType, BackgroundColourPicker, ButtonColourPicker, Player, watermarkRequired,
                   ChannelIcon, ShowChannelIcon, BackgroundImage, Notes, Savebutton, okButton;

        IJavaScriptExecutor executor;

        List<String> ChannelDataList = new List<String>();  //List to store current channel details.

        internal IWebDriver driver = null;

        string appURL;

        Boolean flag = false;

        String columData = null;  // Used to store Channel Name

        Random random = new Random();  // Obejct created to generate Random Number 

        IWait<IWebDriver> iWait = null;
        
        //Instantiating Utilities function class
        Utility_Functions uf = new Utility_Functions();

        Configuration cf = new Configuration();                                                 // Instantiate object for Configuration

        Object_Repository_Class OR = new Object_Repository_Class();

        AdminAuth au = new AdminAuth();                                                         // Instantiate object for Authentication      
       
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

        #region Reusable Function


        // This function will redirect the control to Channel Management Page
        public void RedirectToChannelManagement()
        {
            log.Info("Inside RedirectToChannelManagement Function" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

            iWait.Until(ExpectedConditions.ElementIsVisible(By.Id(OR.readingXMLFile("ChannelManagement", "AdminMenu", "TVAdminPortalOR.xml"))));

            // Clicking on Admin dropdown
            driver.FindElement(By.Id(OR.readingXMLFile("ChannelManagement", "AdminMenu", "TVAdminPortalOR.xml"))).Click();

            Thread.Sleep(3000);

            iWait.Until(ExpectedConditions.ElementToBeClickable(By.LinkText(OR.readingXMLFile("ChannelManagement", "ChannelManagement", "TVAdminPortalOR.xml"))));

            // Clicking on Channel Management
            driver.FindElement(By.LinkText(OR.readingXMLFile("ChannelManagement", "ChannelManagement", "TVAdminPortalOR.xml"))).Click();

            iWait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector(OR.readingXMLFile("ChannelManagement", "ChannelManagementLabel", "TVAdminPortalOR.xml"))));
            Boolean isChannelPage = driver.FindElement(By.CssSelector(OR.readingXMLFile("ChannelManagement", "ChannelManagementLabel", "TVAdminPortalOR.xml"))).Displayed;
            Assert.AreEqual(true, isChannelPage);      //Checking whether the user is on Channel page

            //verifying the default active tab of Channel management page
            Assert.AreEqual(String.Empty, driver.FindElement(By.Id(OR.readingXMLFile("ChannelManagement", "ChannelCreateTab", "TVAdminPortalOR.xml"))).GetAttribute("class"));
            Assert.AreEqual("active", driver.FindElement(By.Id("tabChannel")).GetAttribute("class"));
        }

        // This function Apply Assert on created Channel
        public void AssertAfterCreatingChannel(String channelName)
        {
            log.Info("Inside AssertAfterCreatingChannel Function" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

            //After Creating Categary verifying te active tab 
            Assert.AreEqual("active", driver.FindElement(By.Id("tabChannel")).GetAttribute("class"));

            //NSoup to parse the code of Page.
            Document doc = NSoup.NSoupClient.Parse(driver.PageSource);
            Elements rowListNsoup = doc.GetElementById("ContentPlaceHolder1_gvChannelCreation").GetElementsByTag("tr");

            // Retreving all the rows of Manage Table 
            IList<IWebElement> rowListSelenium = (IList<IWebElement>)driver.FindElement(By.Id("ContentPlaceHolder1_gvChannelCreation")).FindElements(By.TagName("tr"));

            #region Applying_Assert_On_Manage_Page

            flag = false;

            int rowcounter = 0;
            foreach (Element currentRow in rowListNsoup)
            {
                Attributes attr = currentRow.Attributes;

                //Row that have class="GridRowStyle" or class="AltGridStyle"
                if (attr["class"].Equals("GridRowStyle") || attr["class"].Equals("AltGridStyle"))
                {
                    log.Info("Row Counter :: " + rowcounter + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

                    columData = currentRow.GetElementsByTag("td")[3].GetElementById("ContentPlaceHolder1_gvChannelCreation_lblName_" + rowcounter).OwnText().Trim();


                    if (columData.Equals(channelName))
                    {
                        flag = true;

                        // assert to check checkbox is displayed
                        Assert.AreEqual("checkbox", currentRow.GetElementsByTag("td")[0].GetElementById("ContentPlaceHolder1_gvChannelCreation_chkSelect_" + rowcounter).Attributes["type"]);

                        // Write assert to check position
                        SelectElement positionDropsownOnManage = new SelectElement(rowListSelenium[rowcounter + 1].FindElements(By.TagName("td"))[1].FindElement(By.Id("ContentPlaceHolder1_gvChannelCreation_ddlPosition_" + rowcounter)));
                        Assert.AreEqual(1, Convert.ToInt32(positionDropsownOnManage.SelectedOption.Text.Trim()));

                        //category name

                        //Channel Type Assertion
                        Assert.AreEqual(ChannelDataList[2], currentRow.GetElementsByTag("td")[4].GetElementById("ContentPlaceHolder1_gvChannelCreation_lblChannelCategoryType_" + rowcounter).OwnText().Trim());

                        // assert to check  default status and default status should be active
                        Assert.AreEqual("Active", currentRow.GetElementsByTag("td")[5].GetElementById("ContentPlaceHolder1_gvChannelCreation_lblStatus_" + rowcounter).OwnText().Trim());

                        // Assert to verify the Pricing Type vlaue
                        Assert.AreEqual(ChannelDataList[3], currentRow.GetElementsByTag("td")[6].GetElementById("ContentPlaceHolder1_gvChannelCreation_lblType_" + rowcounter).OwnText().Trim());

                        // Assert to verify the Access Type vlaue
                        Assert.AreEqual(ChannelDataList[4], currentRow.GetElementsByTag("td")[7].GetElementById("ContentPlaceHolder1_gvChannelCreation_lblAccessType_" + rowcounter).OwnText().Trim());

                        //pending   //Bgcolour

                        log.Info("BG_Colour:: " + rowListSelenium[rowcounter + 1].FindElements(By.TagName("td"))[8].FindElement(By.Id("ContentPlaceHolder1_gvChannelCreation_colorDiv_" + rowcounter)).GetCssValue("background-color") + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

                        Assert.AreEqual(ChannelDataList[5], rowListSelenium[rowcounter + 1].FindElements(By.TagName("td"))[8].FindElement(By.Id("ContentPlaceHolder1_gvChannelCreation_colorDiv_" + rowcounter)).GetCssValue("background-color"));

                        // Assert to verify the Player value
                        Assert.AreEqual(ChannelDataList[6], currentRow.GetElementsByTag("td")[9].GetElementById("ContentPlaceHolder1_gvChannelCreation_lblPlayer_" + rowcounter).OwnText().Trim());


                        //watermark
                        Assert.AreEqual(ChannelDataList[7], currentRow.GetElementsByTag("td")[10].GetElementById("ContentPlaceHolder1_gvChannelCreation_lblWatermark_" + rowcounter).OwnText().Trim());


                        //write assert to check presence of edit and delete button
                        Assert.AreEqual(true, currentRow.GetElementsByTag("td")[11].GetElementById("ContentPlaceHolder1_gvChannelCreation_imgEdit_" + rowcounter).GetElementsByTag("img")[0].Attributes["src"].Contains("Edit.png"));
                        Assert.AreEqual(true, currentRow.GetElementsByTag("td")[11].GetElementById("ContentPlaceHolder1_gvChannelCreation_ImgDelete_" + rowcounter).GetElementsByTag("img")[0].Attributes["src"].Contains("Delete.png"));

                        //write assert to check Channel name is present of not

                        log.Info("Yes Channel is present on Manage" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

                        break;
                    }
                    rowcounter++;
                }
            }

            #endregion

            Assert.AreEqual(true, flag);   //IF THIS FAILS MEANS created Channel IS NOT displayed ON MANAGE page 

        }

        //If Category is not present then Create Category code is remaining. Should be added after clicking on Create Tab.
        public string CreateChannel(String CategoryName)
        {
            log.Info("Inside CreateChannel Function" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());


            //Waiting and clicking on Create Tab.
            iWait.Until(ExpectedConditions.ElementToBeClickable(By.Id(OR.readingXMLFile("ChannelManagement", "ChannelCreateTab", "TVAdminPortalOR.xml"))));

            driver.FindElement(By.Id(OR.readingXMLFile("ChannelManagement", "ChannelCreateTab", "TVAdminPortalOR.xml"))).Click();

            #region filling_Details

            string channelNameWithGuid = "Chan " + uf.getGuid();  // Concatenating the GUID 

            //Waiting and Entering Channel Name
            iWait.Until(ExpectedConditions.ElementExists(By.Id("ContentPlaceHolder1_txtChannelName")));
            driver.FindElement(By.Id(OR.readingXMLFile("ChannelManagement", "ChannelName", "TVAdminPortalOR.xml"))).SendKeys(channelNameWithGuid);

            #region Pending
            //refer cancelbuttonfunctionality
            #endregion

            //Getting control of Channel Category dropdown. 
            ChannelCategory = driver.FindElement(By.Id(OR.readingXMLFile("ChannelManagement", "ChannelCategory", "TVAdminPortalOR.xml")));
            SelectElement ChannelCategorySelector = new SelectElement(ChannelCategory);

            // getting number of categories in the channel category field dropdown
            numberOfCategory = ChannelCategorySelector.Options.Count;

            log.Info("Number of categories : " + numberOfCategory + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

            randomSelector = random.Next(1, numberOfCategory);  //Generating random number to select category from dropdown

            ChannelCategorySelector.SelectByText("13 April");

            Thread.Sleep(5000);    //need to see to replace Sleep()

            //Getting control of Channel Type dropdown.
            ChannelType = driver.FindElement(By.Id(OR.readingXMLFile("ChannelManagement", "ChannelType", "TVAdminPortalOR.xml")));
            SelectElement ChannelTypeSelector = new SelectElement(ChannelType);
            ChannelTypeSelector.SelectByText("Standard");


            //Getting Control of Position dropdown
            iWait.Until(d => new SelectElement(d.FindElement(By.Id(OR.readingXMLFile("ChannelManagement", "ChannelPosition", "TVAdminPortalOR.xml")))).Options.Count > 0);
            ChannelPosition = driver.FindElement(By.Id(OR.readingXMLFile("ChannelManagement", "ChannelPosition", "TVAdminPortalOR.xml")));
            SelectElement ChannelPositionSelector = new SelectElement(ChannelPosition);
            ChannelPositionSelector.SelectByText("1");

            //Getting control and selecting Pricing Type
            PricingType = driver.FindElement(By.Id(OR.readingXMLFile("ChannelManagement", "PricingType", "TVAdminPortalOR.xml")));
            SelectElement PricingTypeSelector = new SelectElement(PricingType);
            PricingTypeSelector.SelectByText("Free");

            #region Handling Background Color Picker

            // Clicking on Color Picker
            BackgroundColourPicker = driver.FindElement(By.Id(OR.readingXMLFile("ChannelManagement", "BackgroundColour", "TVAdminPortalOR.xml")));                        //BackgroundColour
            BackgroundColourPicker.Click();

            randomSelector = random.Next(1, 10);    // Generating Random Number to select a color from color picker.

            IList<IWebElement> backgroundColourCollection = (IList<IWebElement>)driver.FindElement(By.CssSelector(OR.readingXMLFile("ChannelManagement", "ColourCollection", "TVAdminPortalOR.xml"))).FindElements(By.TagName("tr"))[1].FindElements(By.TagName("td"));
            String[] bgColour = backgroundColourCollection[Convert.ToInt32(randomSelector)].GetAttribute("style").Split(':')[1].Split('(')[1].Split(')')[0].Split(',');

            // converting RGB() to hexadecimal for background colour
            int red = Convert.ToInt32(bgColour[0]);
            int green = Convert.ToInt32(bgColour[1]);
            int blue = Convert.ToInt32(bgColour[2]);
            backgroundHexColor = "#" + red.ToString("X") + green.ToString("X") + blue.ToString("X");

            // Clearing and Entering Selected color into color text field.
            BackgroundColourPicker.Clear();
            BackgroundColourPicker.SendKeys(backgroundHexColor);

            //Clicking on Body to close color picker.
            driver.FindElement(By.Id("ContentPlaceHolder1_rdoChannelAccessType")).FindElements(By.TagName("tr"))[0].FindElement(By.TagName("td")).FindElement(By.TagName("label")).Click();

            #endregion

            #region Handling Button Color Picker

            // Waiting to close the Background Color picker.
            iWait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector(OR.readingXMLFile("ChannelManagement", "ColourCollection", "TVAdminPortalOR.xml"))));

            // Clicking on Color Picker
            ButtonColourPicker = driver.FindElement(By.Id("cp3"));
            ButtonColourPicker.Click();

            randomSelector = random.Next(1, 10);     // Generating Random Number to select a color from color picker.

            iWait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("table.evo-palette")));

            IList<IWebElement> ButtonColourCollection = (IList<IWebElement>)driver.FindElement(By.CssSelector(OR.readingXMLFile("ChannelManagement", "ColourCollection", "TVAdminPortalOR.xml"))).FindElements(By.TagName("tr"))[3].FindElements(By.TagName("td"));
            String[] buttonColour = ButtonColourCollection[Convert.ToInt32(randomSelector)].GetAttribute("style").Split(':')[1].Split('(')[1].Split(')')[0].Split(',');

            //converting RGB() to hexadecimal for button colour
            red = Convert.ToInt32(buttonColour[0]);
            green = Convert.ToInt32(buttonColour[1]);
            blue = Convert.ToInt32(buttonColour[2]);
            buttonHexColor = "#" + red.ToString("X") + green.ToString("X") + blue.ToString("X");

            // Clearing and Entering Selected color into color text field.
            ButtonColourPicker.Clear();
            ButtonColourPicker.SendKeys(buttonHexColor);

            //Clicking on Body to close color picker.
            driver.FindElement(By.Id("ContentPlaceHolder1_rdoChannelAccessType")).FindElements(By.TagName("tr"))[0].FindElement(By.TagName("td")).FindElement(By.TagName("label")).Click();

            #endregion

            //Getting control of Player dropdown
            Player = driver.FindElement(By.Id(OR.readingXMLFile("ChannelManagement", "Player", "TVAdminPortalOR.xml")));                                     //Player
            SelectElement PlayerSelector = new SelectElement(Player);

            //Selecting Player from Player List.
            PlayerSelector.SelectByText("Player1");

            #region Handling WaterMark

            // Selecitng Yes or No option
            randomSelector = random.Next(0, 1);
            watermarkRequired = driver.FindElement(By.Id(OR.readingXMLFile("ChannelManagement", "WatermarkRequired", "TVAdminPortalOR.xml")));             //watermark field
            SelectElement watermarkRequiredSelector = new SelectElement(watermarkRequired);
            watermarkRequiredSelector.SelectByIndex(randomSelector);

            #endregion

            // Getting control to Channel Upload Element
            iWait.Until(ExpectedConditions.ElementExists(By.Id(OR.readingXMLFile("ChannelManagement", "ChannelIcon", "TVAdminPortalOR.xml"))));
            ChannelIcon = driver.FindElement(By.Id(OR.readingXMLFile("ChannelManagement", "ChannelIcon", "TVAdminPortalOR.xml")));                          //Channel Icon upload button

            //reading xml file to upload image
            String uploadChannel = cf.readingXMLFile("AdminPortal", "Channel_Management", "channelIcon", "Config.xml");
            string ChannelIconPath = Environment.CurrentDirectory + "\\Upload\\Images\\" + uploadChannel;
            uf.uploadfile(ChannelIcon, ChannelIconPath);


            iWait.Until(ExpectedConditions.ElementExists(By.Id(OR.readingXMLFile("ChannelManagement", "ShowChannelIcon", "TVAdminPortalOR.xml"))));
            ShowChannelIcon = driver.FindElement(By.Id("ContentPlaceHolder1_chkShowonportal"));                     //show in channel icon checkbox
            ShowChannelIcon.Click();

            iWait.Until(ExpectedConditions.ElementExists(By.Id(OR.readingXMLFile("ChannelManagement", "BackgroundImage", "TVAdminPortalOR.xml"))));         //BackgroundImage upload button
            BackgroundImage = driver.FindElement(By.Id("ContentPlaceHolder1_fileBackgroundImage"));

            //reading xml file to upload backgroung image
            String uploadBgImage = cf.readingXMLFile("AdminPortal", "Channel_Management", "backgroundImage", "Config.xml");
            string bgImagePath = Environment.CurrentDirectory + "\\Upload\\Images\\" + uploadBgImage;
            uf.uploadfile(BackgroundImage, bgImagePath);

            Notes = driver.FindElement(By.Id(OR.readingXMLFile("ChannelManagement", "Notes", "TVAdminPortalOR.xml")));                   //Note field
            Notes.SendKeys("Hello this is automation test");

            ChannelDataList.Add("1");                                             // (0) Adding Selected Position into ChannelDataList.  
            ChannelDataList.Add("13 April");                                     // (1) Adding Selected Category into List.
            ChannelDataList.Add(ChannelTypeSelector.SelectedOption.Text.Trim());  //(2) ChannelType
            ChannelDataList.Add("Free");                                         // (3) pricing type
            ChannelDataList.Add(driver.FindElement(By.Id("ContentPlaceHolder1_rdoChannelAccessType")).FindElements(By.TagName("tr"))[0].FindElement(By.TagName("td")).FindElement(By.TagName("label")).Text.Trim());  //accesstype
            ChannelDataList.Add(driver.FindElements(By.ClassName("evo-colorind"))[0].GetCssValue("background-color"));      // (5) bgcolour
            ChannelDataList.Add(PlayerSelector.SelectedOption.Text.Trim());                                                // (6) player


            // Storing the Current selected Watermark option from dropdown
            if (watermarkRequiredSelector.SelectedOption.Text.Trim().Equals("No"))
            {
                ChannelDataList.Add("None");                                    // (7) waterMark
            }
            else if (watermarkRequiredSelector.SelectedOption.Text.Trim().Equals("Yes"))
            {
                SelectElement watermarkDropdown = new SelectElement(driver.FindElement(By.Id("ContentPlaceHolder1_ddlWatermark")));
                ChannelDataList.Add(watermarkDropdown.SelectedOption.Text.Trim());
            }

            #endregion

            Thread.Sleep(5000);

            //Clicking on Save button
            Savebutton = driver.FindElement(By.Id(OR.readingXMLFile("ChannelManagement", "Savebutton", "TVAdminPortalOR.xml")));
            executor.ExecuteScript("arguments[0].click();", Savebutton);

            // Wait for banner message to appear

            iWait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy(By.CssSelector("div.successmsg.custom_messages")));

            // Success banner message

            iWait.Until(ExpectedConditions.ElementExists(By.Id(OR.readingXMLFile("ChannelManagement", "OkButton", "TVAdminPortalOR.xml"))));

            iWait.Until(ExpectedConditions.TextToBePresentInElementLocated(By.CssSelector("div.successmsg.custom_messages > span"), "Channel Added Successfully."));

            successMessage = driver.FindElement(By.Id(OR.readingXMLFile("ChannelManagement", "SuccessMessage", "TVAdminPortalOR.xml"))).Text;

            uf.isJavaScriptActive(driver);

            Assert.AreEqual("Channel Added Successfully.", successMessage);

            //Click on ok button of banner message
            okButton = driver.FindElement(By.Id(OR.readingXMLFile("ChannelManagement", "OkButton", "TVAdminPortalOR.xml")));               //Save button
            executor.ExecuteScript("arguments[0].click();", okButton);

            return channelNameWithGuid;
        }

        //Create paid channel
        public string CreatePaidChannel(String CategoryName)
        {
            log.Info("Inside CreatePaidChannel Function" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

            //Waiting and clicking on Create Tab.
            iWait.Until(ExpectedConditions.ElementIsVisible(By.Id(OR.readingXMLFile("ChannelManagement", "ChannelCreateTab", "TVAdminPortalOR.xml"))));
            iWait.Until(ExpectedConditions.ElementToBeClickable(By.Id(OR.readingXMLFile("ChannelManagement", "ChannelCreateTab", "TVAdminPortalOR.xml"))));
            driver.FindElement(By.Id(OR.readingXMLFile("ChannelManagement", "ChannelCreateTab", "TVAdminPortalOR.xml"))).Click();

            #region filling_Details

            string channelNameWithGuid = "Chan " + uf.getGuid();  // Concatenating the GUID 

            //Waiting and Entering Channel Name
            iWait.Until(ExpectedConditions.ElementExists(By.Id("ContentPlaceHolder1_txtChannelName")));
            driver.FindElement(By.Id(OR.readingXMLFile("ChannelManagement", "ChannelName", "TVAdminPortalOR.xml"))).SendKeys(channelNameWithGuid);

            #region Pending
            //refer cancelbuttonfunctionality
            #endregion

            //Getting control of Channel Category dropdown. 
            ChannelCategory = driver.FindElement(By.Id(OR.readingXMLFile("ChannelManagement", "ChannelCategory", "TVAdminPortalOR.xml")));
            SelectElement ChannelCategorySelector = new SelectElement(ChannelCategory);

            // getting number of categories in the channel category field dropdown
            numberOfCategory = ChannelCategorySelector.Options.Count;

            log.Info("Number of categories : " + numberOfCategory + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

            randomSelector = random.Next(1, numberOfCategory);  //Generating random number to select category from dropdown

            ChannelCategorySelector.SelectByText("13 April");

            Thread.Sleep(5000);    //need to see to replace Sleep()

            //Getting control of Channel Type dropdown.
            ChannelType = driver.FindElement(By.Id(OR.readingXMLFile("ChannelManagement", "ChannelType", "TVAdminPortalOR.xml")));
            SelectElement ChannelTypeSelector = new SelectElement(ChannelType);
            ChannelTypeSelector.SelectByText("Standard");


            //Getting Control of Position dropdown
            iWait.Until(d => new SelectElement(d.FindElement(By.Id(OR.readingXMLFile("ChannelManagement", "ChannelPosition", "TVAdminPortalOR.xml")))).Options.Count > 0);
            ChannelPosition = driver.FindElement(By.Id(OR.readingXMLFile("ChannelManagement", "ChannelPosition", "TVAdminPortalOR.xml")));
            SelectElement ChannelPositionSelector = new SelectElement(ChannelPosition);
            ChannelPositionSelector.SelectByText("1");

            //Getting control and selecting Pricing Type
            PricingType = driver.FindElement(By.Id(OR.readingXMLFile("ChannelManagement", "PricingType", "TVAdminPortalOR.xml")));
            SelectElement PricingTypeSelector = new SelectElement(PricingType);
            PricingTypeSelector.SelectByText("Paid");

            #region Handling Background Color Picker

            // Clicking on Color Picker
            BackgroundColourPicker = driver.FindElement(By.Id(OR.readingXMLFile("ChannelManagement", "BackgroundColour", "TVAdminPortalOR.xml")));                        //BackgroundColour
            BackgroundColourPicker.Click();

            randomSelector = random.Next(1, 10);    // Generating Random Number to select a color from color picker.

            IList<IWebElement> backgroundColourCollection = (IList<IWebElement>)driver.FindElement(By.CssSelector(OR.readingXMLFile("ChannelManagement", "ColourCollection", "TVAdminPortalOR.xml"))).FindElements(By.TagName("tr"))[1].FindElements(By.TagName("td"));
            String[] bgColour = backgroundColourCollection[Convert.ToInt32(randomSelector)].GetAttribute("style").Split(':')[1].Split('(')[1].Split(')')[0].Split(',');

            // converting RGB() to hexadecimal for background colour
            int red = Convert.ToInt32(bgColour[0]);
            int green = Convert.ToInt32(bgColour[1]);
            int blue = Convert.ToInt32(bgColour[2]);
            backgroundHexColor = "#" + red.ToString("X") + green.ToString("X") + blue.ToString("X");

            // Clearing and Entering Selected color into color text field.
            BackgroundColourPicker.Clear();
            BackgroundColourPicker.SendKeys(backgroundHexColor);

            //Clicking on Body to close color picker.
            driver.FindElement(By.Id("ContentPlaceHolder1_rdoChannelAccessType")).FindElements(By.TagName("tr"))[0].FindElement(By.TagName("td")).FindElement(By.TagName("label")).Click();

            #endregion

            #region Handling Button Color Picker

            // Waiting to close the Background Color picker.
            iWait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector(OR.readingXMLFile("ChannelManagement", "ColourCollection", "TVAdminPortalOR.xml"))));

            // Clicking on Color Picker
            ButtonColourPicker = driver.FindElement(By.Id("cp3"));
            ButtonColourPicker.Click();

            randomSelector = random.Next(1, 10);     // Generating Random Number to select a color from color picker.

            iWait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("table.evo-palette")));

            IList<IWebElement> ButtonColourCollection = (IList<IWebElement>)driver.FindElement(By.CssSelector(OR.readingXMLFile("ChannelManagement", "ColourCollection", "TVAdminPortalOR.xml"))).FindElements(By.TagName("tr"))[3].FindElements(By.TagName("td"));
            String[] buttonColour = ButtonColourCollection[Convert.ToInt32(randomSelector)].GetAttribute("style").Split(':')[1].Split('(')[1].Split(')')[0].Split(',');

            //converting RGB() to hexadecimal for button colour
            red = Convert.ToInt32(buttonColour[0]);
            green = Convert.ToInt32(buttonColour[1]);
            blue = Convert.ToInt32(buttonColour[2]);
            buttonHexColor = "#" + red.ToString("X") + green.ToString("X") + blue.ToString("X");

            // Clearing and Entering Selected color into color text field.
            ButtonColourPicker.Clear();
            ButtonColourPicker.SendKeys(buttonHexColor);

            //Clicking on Body to close color picker.
            driver.FindElement(By.Id("ContentPlaceHolder1_rdoChannelAccessType")).FindElements(By.TagName("tr"))[0].FindElement(By.TagName("td")).FindElement(By.TagName("label")).Click();

            #endregion

            //Getting control of Player dropdown
            Player = driver.FindElement(By.Id(OR.readingXMLFile("ChannelManagement", "Player", "TVAdminPortalOR.xml")));                                     //Player
            SelectElement PlayerSelector = new SelectElement(Player);

            //Selecting Player from Player List.
            PlayerSelector.SelectByText("Player1");

            #region Handling WaterMark

            // Selecitng Yes or No option
            randomSelector = random.Next(0, 1);
            watermarkRequired = driver.FindElement(By.Id(OR.readingXMLFile("ChannelManagement", "WatermarkRequired", "TVAdminPortalOR.xml")));             //watermark field
            SelectElement watermarkRequiredSelector = new SelectElement(watermarkRequired);
            watermarkRequiredSelector.SelectByIndex(randomSelector);

            #endregion

            // Getting control to Channel Upload Element
            iWait.Until(ExpectedConditions.ElementExists(By.Id(OR.readingXMLFile("ChannelManagement", "ChannelIcon", "TVAdminPortalOR.xml"))));
            ChannelIcon = driver.FindElement(By.Id(OR.readingXMLFile("ChannelManagement", "ChannelIcon", "TVAdminPortalOR.xml")));                          //Channel Icon upload button

            //reading xml file to upload image
            String uploadChannel = cf.readingXMLFile("AdminPortal", "Channel_Management", "channelIcon", "Config.xml");
            string ChannelIconPath = Environment.CurrentDirectory + "\\Upload\\Images\\" + uploadChannel;
            uf.uploadfile(ChannelIcon, ChannelIconPath);


            iWait.Until(ExpectedConditions.ElementExists(By.Id(OR.readingXMLFile("ChannelManagement", "ShowChannelIcon", "TVAdminPortalOR.xml"))));
            ShowChannelIcon = driver.FindElement(By.Id("ContentPlaceHolder1_chkShowonportal"));                     //show in channel icon checkbox
            ShowChannelIcon.Click();

            iWait.Until(ExpectedConditions.ElementExists(By.Id(OR.readingXMLFile("ChannelManagement", "BackgroundImage", "TVAdminPortalOR.xml"))));         //BackgroundImage upload button
            BackgroundImage = driver.FindElement(By.Id("ContentPlaceHolder1_fileBackgroundImage"));

            //reading xml file to upload backgroung image
            String uploadBgImage = cf.readingXMLFile("AdminPortal", "Channel_Management", "backgroundImage", "Config.xml");
            string bgImagePath = Environment.CurrentDirectory + "\\Upload\\Images\\" + uploadBgImage;
            uf.uploadfile(BackgroundImage, bgImagePath);

            Notes = driver.FindElement(By.Id(OR.readingXMLFile("ChannelManagement", "Notes", "TVAdminPortalOR.xml")));                   //Note field
            Notes.SendKeys("Hello this is automation test");

            ChannelDataList.Add("1");                                             // (0) Adding Selected Position into ChannelDataList.  
            ChannelDataList.Add("13 April");                                     // (1) Adding Selected Category into List.
            ChannelDataList.Add(ChannelTypeSelector.SelectedOption.Text.Trim());  //(2) ChannelType
            ChannelDataList.Add("Free");                                         // (3) pricing type
            ChannelDataList.Add(driver.FindElement(By.Id("ContentPlaceHolder1_rdoChannelAccessType")).FindElements(By.TagName("tr"))[0].FindElement(By.TagName("td")).FindElement(By.TagName("label")).Text.Trim());  //accesstype
            ChannelDataList.Add(driver.FindElements(By.ClassName("evo-colorind"))[0].GetCssValue("background-color"));      // (5) bgcolour
            ChannelDataList.Add(PlayerSelector.SelectedOption.Text.Trim());                                                // (6) player


            // Storing the Current selected Watermark option from dropdown
            if (watermarkRequiredSelector.SelectedOption.Text.Trim().Equals("No"))
            {
                ChannelDataList.Add("None");                                    // (7) waterMark
            }
            else if (watermarkRequiredSelector.SelectedOption.Text.Trim().Equals("Yes"))
            {
                SelectElement watermarkDropdown = new SelectElement(driver.FindElement(By.Id("ContentPlaceHolder1_ddlWatermark")));
                ChannelDataList.Add(watermarkDropdown.SelectedOption.Text.Trim());
            }

            #endregion

            Thread.Sleep(5000);

            //Clicking on Save button
            Savebutton = driver.FindElement(By.Id(OR.readingXMLFile("ChannelManagement", "Savebutton", "TVAdminPortalOR.xml")));
            executor.ExecuteScript("arguments[0].click();", Savebutton);

            // Wait for banner message to appear

            iWait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy(By.CssSelector("div.successmsg.custom_messages")));

            // Success banner message

            iWait.Until(ExpectedConditions.ElementExists(By.Id(OR.readingXMLFile("ChannelManagement", "OkButton", "TVAdminPortalOR.xml"))));

            iWait.Until(ExpectedConditions.TextToBePresentInElementLocated(By.CssSelector("div.successmsg.custom_messages > span"), "Channel Added Successfully."));

            successMessage = driver.FindElement(By.Id(OR.readingXMLFile("ChannelManagement", "SuccessMessage", "TVAdminPortalOR.xml"))).Text;

            uf.isJavaScriptActive(driver);

            Assert.AreEqual("Channel Added Successfully.", successMessage);

            //Click on ok button of banner message
            okButton = driver.FindElement(By.Id(OR.readingXMLFile("ChannelManagement", "OkButton", "TVAdminPortalOR.xml")));               //Save button
            executor.ExecuteScript("arguments[0].click();", okButton);

            return channelNameWithGuid;
        }


        #endregion

        [Test]
        public void TVAdmin_001_UIVerifyOfCreateChannel()
        {

            try
            {
                log.Info("TVAdmin_001_UIVerificationOfCreateChannel test started" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

                RedirectToChannelManagement();   //Going to Channel Management Page

                //Waiting and clicking on Create Tab.
                iWait.Until(ExpectedConditions.ElementToBeClickable(By.Id(OR.readingXMLFile("ChannelManagement", "ChannelCreateTab", "TVAdminPortalOR.xml"))));
                driver.FindElement(By.Id(OR.readingXMLFile("ChannelManagement", "ChannelCreateTab", "TVAdminPortalOR.xml"))).FindElement(By.TagName("a")).Click();

                #region Region to Verfiy the Default Value or status of Each Element

                //Waiting and Entering Channel Name
                iWait.Until(ExpectedConditions.ElementExists(By.Id("ContentPlaceHolder1_txtChannelName")));

                //Channel name textbox is enabled.
                Assert.AreEqual(true, driver.FindElement(By.Id("ContentPlaceHolder1_txtChannelName")).Enabled);

                //Channel name textfield is blank
                Assert.AreEqual(String.Empty, driver.FindElement(By.Id("ContentPlaceHolder1_txtChannelName")).Text);

                //Verfifying the Default option of Category Dropdown
                SelectElement CategoryDropdown = new SelectElement(driver.FindElement(By.Id("ContentPlaceHolder1_ddlChannelCategory")));
                Assert.AreEqual("Select", CategoryDropdown.SelectedOption.Text.Trim());

                //Verifying the Default option of Channel Type dropdown
                SelectElement ChannelType = new SelectElement(driver.FindElement(By.Id("ContentPlaceHolder1_ddlCategoryType")));
                Assert.AreEqual("Standard", ChannelType.SelectedOption.Text.Trim());

                //Verifying the content of Channel Type dropdown
                IList<IWebElement> contentOfChannelTypeDropdown = ChannelType.Options;

                Assert.AreEqual("Student Only", contentOfChannelTypeDropdown[1].Text.Trim());
                Assert.AreEqual("Member Only", contentOfChannelTypeDropdown[2].Text.Trim());
                Assert.AreEqual("Staff", contentOfChannelTypeDropdown[3].Text.Trim());

                //Verifying the Default option of Position dropdown
                SelectElement positionDropdown = new SelectElement(driver.FindElement(By.Id("ContentPlaceHolder1_ddlChannelEditPosition")));
                Assert.AreEqual(0, positionDropdown.Options.Count);

                //Verifying the Default option of Pricing Type dropdown
                SelectElement pricingType = new SelectElement(driver.FindElement(By.Id("ContentPlaceHolder1_ddlChannelType")));
                Assert.AreEqual("Select", pricingType.SelectedOption.Text.Trim());

                //Verifying the content of Pricing Type dropdown
                IList<IWebElement> contentOfPricingTypeDropdown = pricingType.Options;

                Assert.AreEqual("Free", contentOfPricingTypeDropdown[1].Text.Trim());
                Assert.AreEqual("Paid", contentOfPricingTypeDropdown[2].Text.Trim());

                //Verifying the Default status of Access Type radio buttons and should be disable
                IList<IWebElement> accessTypeRow = (IList<IWebElement>)driver.FindElement(By.Id("ContentPlaceHolder1_rdoChannelAccessType")).FindElements(By.TagName("tr"));

                Assert.AreEqual(false, accessTypeRow[0].FindElements(By.TagName("td"))[0].FindElement(By.Id("ContentPlaceHolder1_rdoChannelAccessType_0")).Enabled);
                Assert.AreEqual(false, accessTypeRow[0].FindElements(By.TagName("td"))[1].FindElement(By.Id("ContentPlaceHolder1_rdoChannelAccessType_1")).Enabled);


                //Verifying the Default option of Player dropdown
                SelectElement playerDropdown = new SelectElement(driver.FindElement(By.Id("ContentPlaceHolder1_ddlPlayer")));
                Assert.AreEqual("Select", playerDropdown.SelectedOption.Text.Trim());


                //Verifying the Default option of Watermark dropdown
                SelectElement watermark = new SelectElement(driver.FindElement(By.Id("ContentPlaceHolder1_ddlIsWaterMarkRequired")));
                Assert.AreEqual("No", watermark.SelectedOption.Text.Trim());

                //Verifying the Defauklt status  of Channel Icon placeholder
                Assert.AreEqual(null, driver.FindElement(By.Id("ContentPlaceHolder1_imgChannelIcon")).GetAttribute("src"));

                //Verifying the Defauklt status  of Background Image placeholder
                Assert.AreEqual(null, driver.FindElement(By.Id("ContentPlaceHolder1_imgBackground")).GetAttribute("src"));

                //Verifying the status of 'Show in Channel Menu' drop down and should be disable
                Assert.AreEqual(false, driver.FindElement(By.Id("ContentPlaceHolder1_chkShowonportal")).Enabled);

                //Verfiying the ciontent of Note field.
                Assert.AreEqual(String.Empty, driver.FindElement(By.Id("ContentPlaceHolder1_txtNotes")).Text.Trim());

                //Verifying the status of Save button
                Assert.AreEqual(true, driver.FindElement(By.Id("ContentPlaceHolder1_btnSave")).Enabled);

                //Verifying the status of Cancel button
                Assert.AreEqual(true, driver.FindElement(By.Id("ContentPlaceHolder1_btnCancel")).Enabled);

                #endregion

                log.Info("Region to check status of Pricing started" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());


                #region Region to Check status of Pricing Dropdown based on Channel Type dorpdown

                // Selecting 'Standard' option from 'Category Type' and verifying the status of 'Price Type' dropdown.
                ChannelType = new SelectElement(driver.FindElement(By.Id("ContentPlaceHolder1_ddlCategoryType")));
                ChannelType.SelectByIndex(0);
                Assert.AreEqual(true, driver.FindElement(By.Id("ContentPlaceHolder1_ddlChannelType")).Enabled);

                // Selecting 'Member Only' option from 'Category Type' and verifying the status of 'Price Type' dropdown.
                ChannelType = new SelectElement(driver.FindElement(By.Id("ContentPlaceHolder1_ddlCategoryType")));
                ChannelType.SelectByIndex(1);
                Assert.AreEqual(false, driver.FindElement(By.Id("ContentPlaceHolder1_ddlChannelType")).Enabled);

                // Selecting 'Student' option from 'Category Type' and verifying the status of 'Price Type' dropdown.
                ChannelType = new SelectElement(driver.FindElement(By.Id("ContentPlaceHolder1_ddlCategoryType")));
                ChannelType.SelectByIndex(2);
                Assert.AreEqual(false, driver.FindElement(By.Id("ContentPlaceHolder1_ddlChannelType")).Enabled);

                // Selecting 'Staff' option from 'Category Type' and verifying the status of 'Price Type' dropdown.
                ChannelType = new SelectElement(driver.FindElement(By.Id("ContentPlaceHolder1_ddlCategoryType")));
                ChannelType.SelectByIndex(3);
                Assert.AreEqual(false, driver.FindElement(By.Id("ContentPlaceHolder1_ddlChannelType")).Enabled);


                #endregion

                #region Pending Region To check Status of Access Type Radion Button

                // Not able to check because Functionality is not developed appropriately.

                #endregion

                log.Info("Region to check Watermark started" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());


                #region Region to check Watermark

                SelectElement waterMark = new SelectElement(driver.FindElement(By.Id("ContentPlaceHolder1_ddlIsWaterMarkRequired")));

                //Verfying that Select watermark dropdown is displayed or not
                Assert.AreEqual(false, driver.FindElement(By.Id("ContentPlaceHolder1_lblWatermarkText")).Displayed);

                //Verfying that Select watermark dropdown is displayed or not
                Assert.AreEqual(false, driver.FindElement(By.Id("ContentPlaceHolder1_lblWatermarkText")).Displayed);


                //Selecting Yes option from WaterMark dropdown
                waterMark.SelectByText("Yes");

                //Verifying that Select watermark dropdown is displayed or not
                Assert.AreEqual(true, driver.FindElement(By.Id("ContentPlaceHolder1_lblWatermarkText")).Displayed);

                //Verfying that Select watermark dropdown is displayed or not
                Assert.AreEqual(true, driver.FindElement(By.Id("ContentPlaceHolder1_lblWatermarkText")).Displayed);


                //Verifying the Default selected  option of Selected Watermark drowpdown
                SelectElement selectWaterMark = new SelectElement(driver.FindElement(By.Id("ContentPlaceHolder1_ddlWatermark")));
                Assert.AreEqual("Select", selectWaterMark.SelectedOption.Text.Trim());

                Assert.AreNotEqual(0, selectWaterMark.Options.Count);


                //Selecting No option from WaterMark dropdown
                waterMark.SelectByText("No");

                //Verifying that Select watermark dropdown is displayed or not
                Assert.AreEqual(false, driver.FindElement(By.Id("ContentPlaceHolder1_lblWatermarkText")).Displayed);

                //Verfying that Select watermark dropdown is displayed or not
                Assert.AreEqual(false, driver.FindElement(By.Id("ContentPlaceHolder1_lblWatermarkText")).Displayed);

                #endregion

                log.Info("TVAdmin_001_UIVerificationOfCreateChannel test completed" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());


            }
            catch (Exception e)
            {
                log.Error(e.Message + "\n" + e.StackTrace + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

                Assert.AreEqual(true, false);
            }

        }

        [Test]
        public void TVAdmin_002_CreateChannel()
        {
            try
            {
                log.Info("TVAdmin_002_CreateChannel test started" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

                // moving to Channel Management Page
                RedirectToChannelManagement();

                // Creating new Channel and Storing the Channel Name into 'ChannelName' variable
                string channelName = CreateChannel("Category Name" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

                // Verifying the details of created channel
                AssertAfterCreatingChannel(channelName);

                log.Info("TVAdmin_002_CreateChannel test completed" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

            }
            catch (Exception e)
            {
                log.Error(e.Message + "\n" + e.StackTrace + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

                Assert.AreEqual(true, false);
            }

        }

        [Test]
        public void TVAdmin_003_CancelButtonFunc()
        {
            try
            {
                log.Info("TVAdmin_003_CancelButtonFunc test started" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

                String ChannelName = "Automation_Cancel_Button_Functionality";

                RedirectToChannelManagement();   // moving to Channel Management Page

                // Waiting and clicking on Create Tab.
                iWait.Until(ExpectedConditions.ElementToBeClickable(By.Id(OR.readingXMLFile("ChannelManagement", "ChannelCreateTab", "TVAdminPortalOR.xml"))));
                driver.FindElement(By.Id(OR.readingXMLFile("ChannelManagement", "ChannelCreateTab", "TVAdminPortalOR.xml"))).FindElement(By.TagName("a")).Click();

                #region filling_Details

                //Waiting and Entering Channel Name
                iWait.Until(ExpectedConditions.ElementExists(By.Id("ContentPlaceHolder1_txtChannelName")));
                driver.FindElement(By.Id(OR.readingXMLFile("ChannelManagement", "ChannelName", "TVAdminPortalOR.xml"))).SendKeys(ChannelName);

                #region Regio to select Category

                //Getting control of Channel Category dropdown. 
                ChannelCategory = driver.FindElement(By.Id(OR.readingXMLFile("ChannelManagement", "ChannelCategory", "TVAdminPortalOR.xml")));
                SelectElement ChannelCategorySelector = new SelectElement(ChannelCategory);

                // getting number of categories in the channel category field dropdown
                numberOfCategory = ChannelCategorySelector.Options.Count;

                if (numberOfCategory == 0)
                {
                    // 1.Call method to create a category
                    // 2. Call function to redirect to Channel Management.
                    // 3. Click on Create Tab
                    // 4. Enter Channel Name
                    // 5. Select Created Category.
                }
                else
                {
                    randomSelector = random.Next(1, numberOfCategory);  //Generating random number to select category from dropdown

                    //ChannelCategorySelector.SelectByIndex(Convert.ToInt32(randomSelector));  //Seelcting Category from Channel Category Dropdown.
                    ChannelCategorySelector.SelectByIndex(1);
                }


                #endregion


                //Getting control of Channel Type dropdown.
                ChannelType = driver.FindElement(By.Id(OR.readingXMLFile("ChannelManagement", "ChannelType", "TVAdminPortalOR.xml")));
                SelectElement ChannelTypeSelector = new SelectElement(ChannelType);
                ChannelTypeSelector.SelectByText("Standard");


                //Getting Control of Position dropdown

                iWait.Until(d => new SelectElement(d.FindElement(By.Id(OR.readingXMLFile("ChannelManagement", "ChannelPosition", "TVAdminPortalOR.xml")))).Options.Count > 0);

                iWait.Until(ExpectedConditions.ElementExists(By.Id(OR.readingXMLFile("ChannelManagement", "ChannelPosition", "TVAdminPortalOR.xml"))));
                ChannelPosition = driver.FindElement(By.Id(OR.readingXMLFile("ChannelManagement", "ChannelPosition", "TVAdminPortalOR.xml")));
                SelectElement ChannelPositionSelector = new SelectElement(ChannelPosition);

                ChannelPositionSelector.SelectByIndex(0);

                //Getting control and selecting Pricing Type
                PricingType = driver.FindElement(By.Id(OR.readingXMLFile("ChannelManagement", "PricingType", "TVAdminPortalOR.xml")));
                SelectElement PricingTypeSelector = new SelectElement(PricingType);
                PricingTypeSelector.SelectByText("Free");

                log.Info("Handling Background Color Picker started" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

                #region Handling Background Color Picker

                BackgroundColourPicker = driver.FindElement(By.Id(OR.readingXMLFile("ChannelManagement", "BackgroundColour", "TVAdminPortalOR.xml")));                        //BackgroundColour
                BackgroundColourPicker.Click();

                randomSelector = random.Next(1, 10);

                IList<IWebElement> backgroundColourCollection = (IList<IWebElement>)driver.FindElement(By.CssSelector(OR.readingXMLFile("ChannelManagement", "ColourCollection", "TVAdminPortalOR.xml"))).FindElements(By.TagName("tr"))[1].FindElements(By.TagName("td"));
                String[] bgColour = backgroundColourCollection[Convert.ToInt32(randomSelector)].GetAttribute("style").Split(':')[1].Split('(')[1].Split(')')[0].Split(',');

                // converting RGB() to hexadecimal for background colour
                int red = Convert.ToInt32(bgColour[0]);
                int green = Convert.ToInt32(bgColour[1]);
                int blue = Convert.ToInt32(bgColour[2]);
                backgroundHexColor = "#" + red.ToString("X") + green.ToString("X") + blue.ToString("X");

                BackgroundColourPicker.Clear();
                BackgroundColourPicker.SendKeys(backgroundHexColor);

                //Clicking on Body to close color picker.
                driver.FindElement(By.Id("ContentPlaceHolder1_rdoChannelAccessType")).FindElements(By.TagName("tr"))[0].FindElement(By.TagName("td")).FindElement(By.TagName("label")).Click();

                #endregion

                #region Handling Button Color Picker

                iWait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector(OR.readingXMLFile("ChannelManagement", "ColourCollection", "TVAdminPortalOR.xml"))));
                ButtonColourPicker = driver.FindElement(By.Id("cp3"));
                ButtonColourPicker.Click();

                randomSelector = random.Next(1, 10);
              
                iWait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("table.evo-palette")));

                IList<IWebElement> ButtonColourCollection = (IList<IWebElement>)driver.FindElement(By.CssSelector(OR.readingXMLFile("ChannelManagement", "ColourCollection", "TVAdminPortalOR.xml"))).FindElements(By.TagName("tr"))[3].FindElements(By.TagName("td"));
                String[] buttonColour = ButtonColourCollection[Convert.ToInt32(randomSelector)].GetAttribute("style").Split(':')[1].Split('(')[1].Split(')')[0].Split(',');

                //converting RGB() to hexadecimal for button colour
                red = Convert.ToInt32(buttonColour[0]);
                green = Convert.ToInt32(buttonColour[1]);
                blue = Convert.ToInt32(buttonColour[2]);
                buttonHexColor = "#" + red.ToString("X") + green.ToString("X") + blue.ToString("X");

                ButtonColourPicker.Clear();
                ButtonColourPicker.SendKeys(buttonHexColor);

                //Clicking on Body to close color picker.
                driver.FindElement(By.Id("ContentPlaceHolder1_rdoChannelAccessType")).FindElements(By.TagName("tr"))[0].FindElement(By.TagName("td")).FindElement(By.TagName("label")).Click();

                #endregion

                //Getting control of Player dropdown
                Player = driver.FindElement(By.Id(OR.readingXMLFile("ChannelManagement", "Player", "TVAdminPortalOR.xml")));                                     //Player
                SelectElement PlayerSelector = new SelectElement(Player);

                //Selecting Player from Player List.
                PlayerSelector.SelectByText("Player1");
                //   extraDataList.Add(PlayerSelector.SelectedOption.Text.Trim());

                log.Info("Handling WaterMark started" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());


                #region Handling WaterMark

                randomSelector = random.Next(0, 1);
                watermarkRequired = driver.FindElement(By.Id(OR.readingXMLFile("ChannelManagement", "WatermarkRequired", "TVAdminPortalOR.xml")));             //watermark field
                SelectElement watermarkRequiredSelector = new SelectElement(watermarkRequired);

                watermarkRequiredSelector.SelectByIndex(randomSelector);
                //if (randomSelector == 0)
                //    extraDataList.Add("");
                //else if (randomSelector == 1)
                //    extraDataList.Add(watermarkRequiredSelector.SelectedOption.Text.Trim());


                #endregion

                iWait.Until(ExpectedConditions.ElementExists(By.Id(OR.readingXMLFile("ChannelManagement", "ChannelIcon", "TVAdminPortalOR.xml"))));
                ChannelIcon = driver.FindElement(By.Id(OR.readingXMLFile("ChannelManagement", "ChannelIcon", "TVAdminPortalOR.xml")));                          //Channel Icon upload button

                //reading xml file to upload image
                String uploadChannel = cf.readingXMLFile("AdminPortal", "Channel_Management", "channelIcon", "Config.xml");
                string ChannelIconPath = Environment.CurrentDirectory + "\\Upload\\" + uploadChannel;
                uf.uploadfile(ChannelIcon, ChannelIconPath);

                iWait.Until(ExpectedConditions.ElementExists(By.Id(OR.readingXMLFile("ChannelManagement", "ShowChannelIcon", "TVAdminPortalOR.xml"))));
                ShowChannelIcon = driver.FindElement(By.Id("ContentPlaceHolder1_chkShowonportal"));                     //show in channel icon checkbox
                ShowChannelIcon.Click();

                iWait.Until(ExpectedConditions.ElementExists(By.Id(OR.readingXMLFile("ChannelManagement", "BackgroundImage", "TVAdminPortalOR.xml"))));
                BackgroundImage = driver.FindElement(By.Id("ContentPlaceHolder1_fileBackgroundImage"));

                //reading xml file to upload backgroung image
                String uploadBgImage = cf.readingXMLFile("AdminPortal", "Channel_Management", "backgroundImage", "Config.xml");
                string bgImagePath = Environment.CurrentDirectory + "\\Upload\\" + uploadBgImage;
                uf.uploadfile(BackgroundImage, bgImagePath);

                Notes = driver.FindElement(By.Id(OR.readingXMLFile("ChannelManagement", "Notes", "TVAdminPortalOR.xml")));
                Notes.SendKeys("Hello this is automation test");


                #endregion


                // Clicking on Cancel button
                driver.FindElement(By.Id("ContentPlaceHolder1_btnCancel")).Click();

                //Wait untill Manage tab Enabled.
                iWait.Until(d => d.FindElement(By.Id("tabChannel")).GetAttribute("class").Equals("active"));

                uf.isJqueryActive(driver);

                Assert.AreEqual(false, driver.PageSource.Contains(ChannelName));

                log.Info("TVAdmin_003_CancelButtonFunc test completed" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

            }
            catch (Exception e)
            {
                log.Error(e.Message + "\n" + e.StackTrace + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());
                Assert.AreEqual(true, false);
            }

        }

        [Test]
        public void TVAdmin_004_DeleteFunc()
        {
            try
            {
                log.Info("TVAdmin_004_DeleteFunc test started" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

                RedirectToChannelManagement();   // moving to Channel Management Page

                // Creating new Channel and Storing the Channel Name into 'ChannelName' variable
                string channelName = CreateChannel("Categry name");

                #region Deleting_Channel

                //Wait untill Manage Tab becomes active
                iWait.Until(d => d.FindElement(By.Id("tabChannel")).GetAttribute("class").Equals("active"));

                //Retreving all the rows of Manage Table 
                IList<IWebElement> rowList = (IList<IWebElement>)driver.FindElement(By.Id("ContentPlaceHolder1_gvChannelCreation")).FindElements(By.TagName("tr"));

                #region To Select Created Category

                int i = 0;
                foreach (IWebElement currentRow in rowList)
                {
                    //Row that have class="GridRowStyle" or class="AltGridStyle"
                    if (currentRow.GetAttribute("class").Equals("GridRowStyle") || currentRow.GetAttribute("class").Equals("AltGridStyle"))
                    {
                        log.Info("Row Counter :: " + i + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

                        String columData = currentRow.FindElements(By.TagName("td"))[3].FindElement(By.TagName("span")).Text.Trim();

                        if (columData.Equals(channelName))
                        {

                            Thread.Sleep(1000);
                            currentRow.FindElements(By.TagName("td"))[11].FindElement(By.Id("ContentPlaceHolder1_gvChannelCreation_ImgDelete_" + i)).Click();   // clicking on Delete Icon

                            iWait.Until(ExpectedConditions.ElementIsVisible(By.Id("btnNo")));
                            Assert.AreEqual("Are you sure, you wish to delete?", driver.FindElement(By.Id("Default_Language")).Text.Trim());  //Verfying that correct Warning MEssage is displayed
                            driver.FindElement(By.Id("btnNo")).Click();   // Clicking on Yes button of Warning Message

                            Assert.AreEqual(true, driver.PageSource.Contains(channelName)); //if this fails means NO button functionlity is not wroking properly

                            Thread.Sleep(1000);
                            currentRow.FindElements(By.TagName("td"))[11].FindElement(By.Id("ContentPlaceHolder1_gvChannelCreation_ImgDelete_" + i)).Click();   // clicking on Delete Icon

                            iWait.Until(ExpectedConditions.ElementIsVisible(By.Id("btnYes")));
                            Assert.AreEqual("Are you sure, you wish to delete?", driver.FindElement(By.Id("Default_Language")).Text.Trim());  //Verfying that correct Warning MEssage is displayed
                            driver.FindElement(By.Id("btnYes")).Click();   //Clicking on Yes button of Warning Message

                            iWait.Until(ExpectedConditions.ElementIsVisible(By.Id("btnOk")));
                            Assert.AreEqual("Channel Deleted Successfully.", driver.FindElement(By.Id("Sucess_Message")).Text.Trim());   //Verifying that Successcull message is displayed after delete
                            driver.FindElement(By.Id("btnOk")).Click();     //Clicking on Ok button of Success Message

                            iWait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.Id("btnOk")));

                            uf.isJqueryActive(driver);

                            Assert.AreEqual(false, driver.PageSource.Contains(channelName));   //if this fails then Delete functinality is not working.
                            break;
                        }
                    }
                }

                #endregion

                #endregion

                log.Info("TVAdmin_004_DeleteFunc test completed" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

            }
            catch (Exception e)
            {
                log.Error(e.Message + "\n" + e.StackTrace + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

                Assert.AreEqual(true, false);
            }
        }
        
        [Test]
        public void TVAdmin_005_ActivateInvactivateFunc()
        {
            try
            {
                log.Info("TVAdmin_005_ActivateInvactivateFunc test started" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

                RedirectToChannelManagement();   // moving to Channel Management Page

                // Creating new Channel and Storing the Channel Name into 'ChannelName' variable
                string channelName = CreateChannel("Categry name");

                //Wait untill Manage Tab becomes active
                iWait.Until(d => d.FindElement(By.Id("tabChannel")).GetAttribute("class").Equals("active"));


                #region Inactivating Category

                log.Info("Inactivating Category" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

                //Using Nsoup here to parse the html table
                Document doc = NSoup.NSoupClient.Parse(driver.PageSource);
                Elements rowListNsoup = doc.GetElementById("ContentPlaceHolder1_gvChannelCreation").GetElementsByTag("tr");


                // Retreving all the rows of Manage Table 
                IList<IWebElement> rowList = (IList<IWebElement>)driver.FindElement(By.Id("ContentPlaceHolder1_gvChannelCreation")).FindElements(By.TagName("tr"));

                #region To Select Created Category and clicking on Checkbox

                int i = 0;

                foreach (Element currentRow in rowListNsoup)
                {
                    Attributes attr = currentRow.Attributes;

                    //Row that have class="GridRowStyle" or class="AltGridStyle"
                    if (attr["class"].Equals("GridRowStyle") || attr["class"].Equals("AltGridStyle"))
                    {
                        log.Info("Row Count  :: " + i + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

                        String columData = currentRow.GetElementsByTag("td")[3].GetElementsByTag("span")[0].OwnText().Trim();


                        if (columData.Equals(channelName))
                        {
                            // Clicking on Check box
                            rowList[i + 1].FindElements(By.TagName("td"))[0].FindElement(By.TagName("input")).Click();

                            // currentRow.FindElements(By.TagName("td"))[0].FindElement(By.TagName("input")).Click();   // Clicking on Check box
                            //currentRow1 = currentRow;
                            break;
                        }
                        i++;
                    }
                }
                #endregion



                //Clicking on InActivate button
                driver.FindElement(By.Id("ContentPlaceHolder1_btnInactivate")).Click();

                //Clicking on Ok button of Success Message Message
                iWait.Until(ExpectedConditions.ElementIsVisible(By.Id("btnOk")));

                Assert.AreEqual("Record Inactivated Successfully.", driver.FindElement(By.Id("Sucess_Message")).Text.Trim());

                driver.FindElement(By.Id("btnOk")).Click();

                iWait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.Id("btnOk")));

                //Wait untill Manage Tab becomes active
                iWait.Until(d => d.FindElement(By.Id("tabChannel")).GetAttribute("class").Equals("active"));

                uf.isJqueryActive(driver);

                #region To Track Created Channel and Aplying Assert to check Whether Inactivated or not

                //Using Nsoup here to parse the html table
                doc = NSoup.NSoupClient.Parse(driver.PageSource);
                rowListNsoup = doc.GetElementById("ContentPlaceHolder1_gvChannelCreation").GetElementsByTag("tr");


                // Retreving all the rows of Manage Table 
                rowList = (IList<IWebElement>)driver.FindElement(By.Id("ContentPlaceHolder1_gvChannelCreation")).FindElements(By.TagName("tr"));

                i = 0;  // to track record number
                foreach (Element tempCurrentRow in rowListNsoup)
                {
                    Attributes attr = tempCurrentRow.Attributes;

                    //Row that have class="GridRowStyle" or class="AltGridStyle"
                    if (attr["class"].Equals("GridRowStyle") || attr["class"].Equals("AltGridStyle"))
                    {
                        log.Info("Row Count  :: " + i + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

                        String columData = tempCurrentRow.GetElementsByTag("td")[3].GetElementsByTag("span")[0].OwnText().Trim();

                        if (columData.Equals(channelName))
                        {

                            //Assert to check status = 'Activate / Inactivate' of Invactivated Channel and this should be InActive
                            Assert.AreEqual("Inactive", tempCurrentRow.GetElementsByTag("td")[5].GetElementsByTag("span")[0].OwnText());

                            //Verfying that Edit button is not present for Inactivated Channel
                            Assert.AreEqual(0, rowList[i + 1].FindElements(By.TagName("td"))[11].FindElements(By.Id("ContentPlaceHolder1_gvChannelCreation_imgEdit_" + i)).Count);
                            break;
                        }
                        i++;
                    }

                }
                #endregion



                #endregion

                log.Info("Activating Category started" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());
                
                #region Activating Category

                log.Info("\nActivating Channel" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

                //Using Nsoup here to parse the html table
                doc = NSoup.NSoupClient.Parse(driver.PageSource);
                rowListNsoup = doc.GetElementById("ContentPlaceHolder1_gvChannelCreation").GetElementsByTag("tr");

                // Retreving all the rows of Manage Table 
                rowList = (IList<IWebElement>)driver.FindElement(By.Id("ContentPlaceHolder1_gvChannelCreation")).FindElements(By.TagName("tr"));

                #region To Select Created Category

                i = 0;

                foreach (Element currentRow in rowListNsoup)
                {
                    Attributes attr = currentRow.Attributes;

                    //Row that have class="GridRowStyle" or class="AltGridStyle"
                    if (attr["class"].Equals("GridRowStyle") || attr["class"].Equals("AltGridStyle"))
                    {
                        log.Info("Row Count  :: " + i + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

                        String columData = currentRow.GetElementsByTag("td")[3].GetElementsByTag("span")[0].OwnText().Trim();


                        if (columData.Equals(channelName))
                        {

                            log.Info("Is checkBox :: " + currentRow.GetElementsByTag("td")[0].GetElementsByTag("input")[0].Attributes["type"] + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

                            // Clicking on Check box
                            rowList[i + 1].FindElements(By.TagName("td"))[0].FindElement(By.TagName("input")).Click();

                            driver.FindElement(By.Id("ContentPlaceHolder1_btnActive")).Click();   //Clicking on Activate Button

                            //  currentRow1 = currentRow;

                            break;
                        }
                    }
                }
                #endregion

                //Clicking on Ok button of Success Message Message
                iWait.Until(ExpectedConditions.ElementIsVisible(By.Id("btnOk")));

                Assert.AreEqual("Record is Activated Successfully.", driver.FindElement(By.Id("Sucess_Message")).Text.Trim());  //Verifying that correct Success message is displayed or not

                driver.FindElement(By.Id("btnOk")).Click();

                iWait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.Id("btnOk")));

                #region To Track Created Channel and Aplying Assert to check Whether Activated or not

                //Using Nsoup here to parse the html table
                doc = NSoup.NSoupClient.Parse(driver.PageSource);
                rowListNsoup = doc.GetElementById("ContentPlaceHolder1_gvChannelCreation").GetElementsByTag("tr");


                // Retreving all the rows of Manage Table
                rowList = (IList<IWebElement>)driver.FindElement(By.Id("ContentPlaceHolder1_gvChannelCreation")).FindElements(By.TagName("tr"));
                i = 0;  // to track record number
                foreach (Element tempCurrentRow in rowListNsoup)
                {
                    Attributes attr = tempCurrentRow.Attributes;

                    //Row that have class="GridRowStyle" or class="AltGridStyle"
                    if (attr["class"].Equals("GridRowStyle") || attr["class"].Equals("AltGridStyle"))
                    {
                        log.Info("Row Count  :: " + i + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

                        String columData = tempCurrentRow.GetElementsByTag("td")[3].GetElementsByTag("span")[0].OwnText().Trim();

                        if (columData.Equals(channelName))
                        {

                            //Assert to check status = 'Activate / Inactivate' of Invactivated Channel and this should be InActive
                            Assert.AreEqual("Active", tempCurrentRow.GetElementsByTag("td")[5].GetElementsByTag("span")[0].OwnText());


                            //Verfying that Edit button is not present for Inactivated Channel
                            Assert.AreNotEqual(0, rowList[i + 1].FindElements(By.TagName("td"))[11].FindElements(By.Id("ContentPlaceHolder1_gvChannelCreation_imgEdit_" + i)).Count);
                            break;

                        }
                    } i++;
                }
                #endregion

                #endregion



                iWait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector("div.infomsg > button.btn-sm")));

                log.Info("TVAdmin_005_ActivateInvactivateFunc test completed" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

            }
            catch (Exception e)
            {
                log.Error(e.Message + "\n" + e.StackTrace + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

                Assert.AreEqual(true, false);
            }
        }

        [Test]
        public void TVAdmin_006_actinactWithoutSelectRec()
        {
            try
            {
                log.Info("TVAdmin_006_ActivateInactivateWithoutSelectingRecord test started" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

                RedirectToChannelManagement();  // moving to Channel Management Page

                driver.FindElement(By.Id("ContentPlaceHolder1_btnActive")).Click();  //Clicking on Activate Button

                //Verfiying User friendly message
                iWait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("div.infomsg > button.btn-sm")));
                Assert.AreEqual("Please select at least one record.", driver.FindElement(By.Id("Info")).Text.Trim());    //Verifying infomration message for Duplicate Channel.
                driver.FindElement(By.CssSelector("div.infomsg > button.btn-sm")).Click();

                iWait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector("div.infomsg > button.btn-sm")));

                iWait.Until(ExpectedConditions.ElementToBeClickable(By.Id("ContentPlaceHolder1_btnInactivate")));
                driver.FindElement(By.Id("ContentPlaceHolder1_btnInactivate")).Click();  //Clicking on Inctivate Button

                //Verfiying User friendly message
                iWait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("div.infomsg > button.btn-sm")));
                Assert.AreEqual("Please select at least one record.", driver.FindElement(By.Id("Info")).Text.Trim());    //Verifying infomration message for Duplicate Channel.
                driver.FindElement(By.CssSelector("div.infomsg > button.btn-sm")).Click();

                log.Info("TVAdmin_006_ActivateInactivateWithoutSelectingRecord test completed" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

            }
            catch (Exception e)
            {
                log.Error(e.Message + "\n" + e.StackTrace + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

                Assert.AreEqual(true, false);
            }

        }

        [Test]
        public void TVAdmin_007_MandatoryFieldValidation()
        {
            try
            {
                log.Info("TVAdmin_007_MandatoryFieldValidation test started" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());
     
                RedirectToChannelManagement(); //moving to Channel Management Page.

                //Waiting and clicking on Create Tab.
                iWait.Until(ExpectedConditions.ElementToBeClickable(By.Id(OR.readingXMLFile("ChannelManagement", "ChannelCreateTab", "TVAdminPortalOR.xml"))));
                driver.FindElement(By.Id(OR.readingXMLFile("ChannelManagement", "ChannelCreateTab", "TVAdminPortalOR.xml"))).FindElement(By.TagName("a")).Click();

                #region  Clicking on Save button and Verfiy that Inline Error message is displayed or not

                driver.FindElement(By.Id("ContentPlaceHolder1_btnSave")).Click(); //Clicking on Save button

                //Verifying that Mendatroy message is dispayed correclty for Channel Field ?
                iWait.Until(ExpectedConditions.ElementIsVisible(By.Id("ContentPlaceHolder1_rfvtxtChannelName")));
                Assert.AreEqual("Please enter Channel name.", driver.FindElement(By.Id("ContentPlaceHolder1_rfvtxtChannelName")).Text);

                //Verifying that Mendatroy message is dispayed correclty for Category dropdown ?
                iWait.Until(ExpectedConditions.ElementIsVisible(By.Id("ContentPlaceHolder1_compval1")));
                Assert.AreEqual("Please choose Channel Category.", driver.FindElement(By.Id("ContentPlaceHolder1_compval1")).Text);

                //Verifying that Mendatroy message is dispayed correclty for Pricing Type dropdown ?
                iWait.Until(ExpectedConditions.ElementIsVisible(By.Id("ContentPlaceHolder1_cvPricingType")));
                Assert.AreEqual("Please choose Pricing Type.", driver.FindElement(By.Id("ContentPlaceHolder1_cvPricingType")).Text);

                #endregion

                log.Info("Entering Valid Details in Mandatory Fields" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());
               
                #region  Entering Valid Details in Mandatory Fields and Verify that Inline error message is removed or not.

                //Entering Channel Name and verfiy that Mandoatry Inline Error message is disaplayed ?
                driver.FindElement(By.Id("ContentPlaceHolder1_txtChannelName")).SendKeys("Channel Name");
                driver.FindElement(By.Id("ContentPlaceHolder1_btnSave")).Click(); //Clicking on Save button

                //Verifying that Mendatroy message is dispayed correclty for Channel Field ?
                iWait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.Id("ContentPlaceHolder1_rfvtxtChannelName")));
                Assert.AreEqual("hidden", driver.FindElement(By.Id("ContentPlaceHolder1_rfvtxtChannelName")).GetCssValue("visibility"));

                //Selecting Catagory Name and verfiy that Mandoatry Inline Error message is disaplayed ?

                ChannelCategory = driver.FindElement(By.Id(OR.readingXMLFile("ChannelManagement", "ChannelCategory", "TVAdminPortalOR.xml")));
                SelectElement ChannelCategorySelector = new SelectElement(ChannelCategory);

                if (ChannelCategorySelector.Options.Count > 0)
                {
                    ChannelCategorySelector.SelectByIndex(1);

                    //Verifying that Mendatroy message is dispayed for Category Field ?
                    iWait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.Id("ContentPlaceHolder1_compval1")));
                    Assert.AreEqual("none", driver.FindElement(By.Id("ContentPlaceHolder1_compval1")).GetCssValue("display"));
                }


                //Getting control and selecting Pricing Type
                PricingType = driver.FindElement(By.Id(OR.readingXMLFile("ChannelManagement", "PricingType", "TVAdminPortalOR.xml")));
                SelectElement PricingTypeSelector = new SelectElement(PricingType);
                PricingTypeSelector.SelectByIndex(1);

                //Verifying that Mendatroy message is dispayed for Category Field ?
                iWait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.Id("ContentPlaceHolder1_cvPricingType")));
                Assert.AreEqual("none", driver.FindElement(By.Id("ContentPlaceHolder1_cvPricingType")).GetCssValue("display"));

                #endregion

                log.Info("TVAdmin_007_MandatoryFieldValidation test completed" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

            }
            catch (Exception e)
            {
                log.Error(e.Message + "\n" + e.StackTrace + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());
                Assert.AreEqual(true, false);
            }

        }

        [Test]
        public void TVAdmin_008_EditingChannel()
        {
            try
            {
                log.Info("TVAdmin_008_EditingChannel test started" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

                String cancelbuttonfunctionality = "Checking Cancel button in Edit Mode";

                RedirectToChannelManagement();

                #region Scenario 1 : Changing Category Name and Position and clicking on Save button

                log.Info("Inside Scenario 1" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

                #region Storing the Current Information of Channel that is to be Edited


                //NSoup to parse the source code of current page
                Document doc = NSoup.NSoupClient.Parse(driver.PageSource);
                Elements rowListNsoup = doc.GetElementById("ContentPlaceHolder1_gvChannelCreation").GetElementsByTag("tr");

                IList<IWebElement> rowListSelenium = (IList<IWebElement>)driver.FindElement(By.Id("ContentPlaceHolder1_gvChannelCreation")).FindElements(By.TagName("tr"));

                Boolean isCheckBox = true;
                isCheckBox = rowListSelenium[1].FindElements(By.TagName("td"))[0].FindElement(By.TagName("input")).Displayed;


                String currentPosition = null;
                SelectElement positionDropsown = new SelectElement(rowListSelenium[1].FindElements(By.TagName("td"))[1].FindElement(By.Name("ctl00$ContentPlaceHolder1$gvChannelCreation$ctl02$ddlPosition")));
                currentPosition = positionDropsown.SelectedOption.Text.Trim();

                String currentCategoryName = null;
                currentCategoryName = rowListNsoup[1].GetElementsByTag("td")[2].GetElementsByTag("span")[0].OwnText().Trim();

                String currentChannelName = null;
                currentChannelName = rowListNsoup[1].GetElementsByTag("td")[3].GetElementsByTag("span")[0].OwnText().Trim();

                String currentChannelType = null;
                currentChannelType = rowListNsoup[1].GetElementsByTag("td")[4].GetElementsByTag("span")[0].OwnText().Trim();

                String currentStatus = null;
                currentStatus = rowListNsoup[1].GetElementsByTag("td")[5].GetElementsByTag("span")[0].OwnText().Trim();

                String currentPricingType = null;
                currentPricingType = rowListNsoup[1].GetElementsByTag("td")[6].GetElementsByTag("span")[0].OwnText().Trim();

                String currentAccessType = null;
                currentAccessType = rowListNsoup[1].GetElementsByTag("td")[7].GetElementsByTag("span")[0].OwnText().Trim();

                String currentColour = null;
                currentColour = rowListSelenium[1].FindElements(By.TagName("td"))[8].FindElement(By.ClassName("color_div_border")).GetAttribute("style");

                String currentPlayer = null;
                currentPlayer = rowListNsoup[1].GetElementsByTag("td")[9].GetElementsByTag("span")[0].OwnText().Trim();

                String currentWaterMark = null;
                currentWaterMark = rowListNsoup[1].GetElementsByTag("td")[10].GetElementsByTag("span")[0].OwnText().Trim();

                int isEditbutton = 1;               //Here We are using int type because if Selenium does not find Element so gives Exception. So we are applying assert by using count. 
                isEditbutton = rowListSelenium[1].FindElements(By.TagName("td"))[11].FindElements(By.CssSelector("span > img")).Count;

                Boolean isDeleteButton = true;
                isDeleteButton = rowListSelenium[1].FindElements(By.TagName("td"))[11].FindElement(By.Id("ContentPlaceHolder1_gvChannelCreation_ImgDelete_0")).FindElement(By.TagName("img")).GetAttribute("src").Contains("Delete.png");

                rowListSelenium[1].FindElements(By.TagName("td"))[11].FindElement(By.CssSelector("span > img")).Click();  //Clicking on Edit Button
                #endregion

                #region Region to Apply Assert On Edit Channel Page


                uf.isJqueryActive(driver);

                iWait.Until(ExpectedConditions.TextToBePresentInElementLocated(By.Id("tabCreate"), "Edit Channel"));

                iWait.Until(d => d.FindElement(By.Id("ContentPlaceHolder1_txtChannelName")).GetAttribute("value").Length > 0);

                Assert.AreEqual("Edit Channel", driver.FindElement(By.Id("tabCreate")).FindElement(By.TagName("a")).Text);  //Checking whether the text of Create is changed to Edit Category

                Assert.AreEqual(true, driver.FindElement(By.Id("ContentPlaceHolder1_txtChannelName")).Enabled);  //Verifying that Channel Textfield is enable.

                Assert.AreNotEqual(String.Empty, driver.FindElement(By.Id("ContentPlaceHolder1_txtChannelName")).GetAttribute("Value")); // verifying that Channel name has some pre populated data

                Assert.AreEqual(currentChannelName, driver.FindElement(By.Id("ContentPlaceHolder1_txtChannelName")).GetAttribute("value")); //verfiying that populated Channel name was old one 

                Assert.AreEqual(currentCategoryName, new SelectElement(driver.FindElement(By.Id("ContentPlaceHolder1_ddlChannelCategory"))).SelectedOption.Text.Trim()); //verifying the prepopulated category Name is old one

                Assert.AreEqual(currentChannelType, new SelectElement(driver.FindElement(By.Id("ContentPlaceHolder1_ddlCategoryType"))).SelectedOption.Text.Trim()); //verifying the prepopulated channeltype is old one

                Assert.AreEqual(currentPosition, new SelectElement(driver.FindElement(By.Id("ContentPlaceHolder1_ddlChannelEditPosition"))).SelectedOption.Text.Trim()); //verifying the prepopulated position is old one

                Assert.AreEqual(currentPricingType, new SelectElement(driver.FindElement(By.Id("ContentPlaceHolder1_ddlChannelType"))).SelectedOption.Text.Trim()); //verifying the prepopulated pricing type is old one

                Assert.AreEqual(currentColour, driver.FindElements(By.ClassName("evo-colorind-ff"))[0].GetAttribute("style")); //verifying the prepopulated background is old one

                Assert.AreEqual(currentPlayer, new SelectElement(driver.FindElement(By.Id("ContentPlaceHolder1_ddlPlayer"))).SelectedOption.Text.Trim()); //verifying the prepopulated palyer is old one

                //Assert.AreEqual(currentWaterMark, new SelectElement(driver.FindElement(By.Id("ContentPlaceHolder1_ddlIsWaterMarkRequired"))).SelectedOption.Text.Trim()); //verifying the prepopulated wtaermark is old one


                #endregion

                #region Region to Edit the details of channel on Edot page and Verfiying Banner Message

                string channelName = "Chan" + uf.getGuid();

                log.Info("Clearing pre populated channel name and Entering new channel name" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

                driver.FindElement(By.Id("ContentPlaceHolder1_txtChannelName")).Clear();  //Clearing pre populated channel name
                driver.FindElement(By.Id("ContentPlaceHolder1_txtChannelName")).SendKeys(channelName);

                log.Info("Editing channel category information" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

                //Editing Channel Category information
                SelectElement channelcategory = null;

                int channelcategoryIndex = 2;

                channelcategory = new SelectElement(driver.FindElement(By.Id("ContentPlaceHolder1_ddlChannelCategory")));

                channelcategory.SelectByIndex(channelcategoryIndex);

                iWait.Until(ExpectedConditions.StalenessOf(driver.FindElement(By.Id("ContentPlaceHolder1_ddlChannelCategory"))));

                String channelcategorytext = new SelectElement(driver.FindElement(By.Id("ContentPlaceHolder1_ddlChannelCategory"))).SelectedOption.Text;

                Thread.Sleep(5000);

                log.Info("Editing channel type information" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

                //Editing Channnel Type information
                SelectElement channeltype = null;

                int channeltypeIndex = 2;

                channeltype = new SelectElement(driver.FindElement(By.Id("ContentPlaceHolder1_ddlCategoryType")));

                channeltype.SelectByIndex(channeltypeIndex);

                String channeltypetext = new SelectElement(driver.FindElement(By.Id("ContentPlaceHolder1_ddlCategoryType"))).SelectedOption.Text;


                log.Info("Editing pricing type information" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());
                //Editing PricingType information
                SelectElement pricingType = null;

                int pricingTypeIndex = 2;

                pricingType = new SelectElement(driver.FindElement(By.Id("ContentPlaceHolder1_ddlChannelType")));

                pricingType.SelectByIndex(pricingTypeIndex);

                String pricingTypetext = new SelectElement(driver.FindElement(By.Id("ContentPlaceHolder1_ddlChannelType"))).SelectedOption.Text;

                // Editing Background Color Picker
                BackgroundColourPicker = driver.FindElement(By.Id(OR.readingXMLFile("ChannelManagement", "BackgroundColour", "TVAdminPortalOR.xml")));                        //BackgroundColour
                BackgroundColourPicker.Click();

                randomSelector = random.Next(1, 10);

                IList<IWebElement> backgroundColourCollection = (IList<IWebElement>)driver.FindElement(By.CssSelector(OR.readingXMLFile("ChannelManagement", "ColourCollection", "TVAdminPortalOR.xml"))).FindElements(By.TagName("tr"))[1].FindElements(By.TagName("td"));
                String[] bgColour = backgroundColourCollection[Convert.ToInt32(randomSelector)].GetAttribute("style").Split(':')[1].Split('(')[1].Split(')')[0].Split(',');

                // converting RGB() to hexadecimal for background colour
                int red = Convert.ToInt32(bgColour[0]);
                int green = Convert.ToInt32(bgColour[1]);
                int blue = Convert.ToInt32(bgColour[2]);
                backgroundHexColor = "#" + red.ToString("X") + green.ToString("X") + blue.ToString("X");

                BackgroundColourPicker.Clear();
                BackgroundColourPicker.SendKeys(backgroundHexColor);

                //Clicking on Body to close color picker.
                driver.FindElement(By.Id("ContentPlaceHolder1_rdoChannelAccessType")).FindElements(By.TagName("tr"))[0].FindElement(By.TagName("td")).FindElement(By.TagName("label")).Click();

                //Editing Player information
                SelectElement player = null;
                int playerIndex = 2;
                player = new SelectElement(driver.FindElement(By.Id("ContentPlaceHolder1_ddlPlayer")));
                player.SelectByIndex(playerIndex);
                String playerText = player.SelectedOption.Text;

                //clicking on save button
                driver.FindElement(By.Id("ContentPlaceHolder1_btnSave")).Click();

                //Verfiying User friendly message
                iWait.Until(ExpectedConditions.ElementIsVisible(By.Id("btnOk")));
                Assert.AreEqual("Channel Edited Successfully.", driver.FindElement(By.Id("Sucess_Message")).Text.Trim());
                driver.FindElement(By.Id("btnOk")).Click();

                iWait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.Id("btnOk")));

                #endregion

                #region searching the Channel that is edited on Manage Page and Checking Information


                //NSoup to parse the source code of current page
                doc = NSoup.NSoupClient.Parse(driver.PageSource);
                rowListNsoup = doc.GetElementById("ContentPlaceHolder1_gvChannelCreation").GetElementsByTag("tr");

                //Retreving all the rows of Manage Table
                rowListSelenium = (IList<IWebElement>)driver.FindElement(By.Id("ContentPlaceHolder1_gvChannelCreation")).FindElements(By.TagName("tr"));

                int rowCounter = 0;  //to track the record no
                flag = false;
                foreach (Element currentRow in rowListNsoup)
                {
                    Attributes attr = currentRow.Attributes;

                    // Row that have class="GridRowStyle" or class="AltGridStyle"
                    if (attr["class"].Equals("GridRowStyle") || attr["class"].Equals("AltGridStyle"))
                    {
                        log.Info("Row Count  :: " + rowCounter + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

                        columData = currentRow.GetElementsByTag("td")[3].GetElementById("ContentPlaceHolder1_gvChannelCreation_lblName_" + rowCounter).OwnText().Trim();

                        if (columData.Equals(channelName))
                        {
                            flag = true;

                            //write assert to check checkbox is displayed
                            Assert.AreEqual(isCheckBox, rowListSelenium[rowCounter + 1].FindElements(By.TagName("td"))[0].FindElement(By.TagName("input")).Displayed);
                            Assert.AreEqual("checkbox", currentRow.GetElementsByTag("td")[0].GetElementById("ContentPlaceHolder1_gvChannelCreation_chkSelect_" + rowCounter).Attributes["type"]);

                            ////Write assert to check position
                            //positionDropsown = new SelectElement(currentRow.FindElements(By.TagName("td"))[1].FindElement(By.Id("ContentPlaceHolder1_gvChannelCreation_ddlPosition_" + i)));

                            //Assert.AreEqual("1", positionDropsown.SelectedOption.Text.Trim());

                            //write assert to check Category
                            Assert.AreEqual(channelcategorytext, currentRow.GetElementsByTag("td")[2].GetElementById("ContentPlaceHolder1_gvChannelCreation_lblCategory_" + rowCounter).OwnText().Trim());

                            //write assert to check Channel name
                            Assert.AreEqual(channelName, currentRow.GetElementsByTag("td")[3].GetElementById("ContentPlaceHolder1_gvChannelCreation_lblName_" + rowCounter).OwnText().Trim());

                            //write assert to check Channel Type
                            Assert.AreEqual(channeltypetext, currentRow.GetElementsByTag("td")[4].GetElementById("ContentPlaceHolder1_gvChannelCreation_lblChannelCategoryType_" + rowCounter).OwnText().Trim());

                            //write assert to check  default status and should be active
                            Assert.AreEqual(currentStatus, currentRow.GetElementsByTag("td")[5].GetElementById("ContentPlaceHolder1_gvChannelCreation_lblStatus_" + rowCounter).OwnText().Trim());

                            //write assert to check  Pricing type
                            Assert.AreEqual(String.Empty, currentRow.GetElementsByTag("td")[6].GetElementById("ContentPlaceHolder1_gvChannelCreation_lblType_" + rowCounter).OwnText().Trim());

                            //write assert to check to check colour
                            log.Info("Background COloe :: " + rowListSelenium[rowCounter + 1].FindElements(By.TagName("td"))[8].FindElement(By.Id("ContentPlaceHolder1_gvChannelCreation_colorDiv_" + rowCounter)).GetCssValue("background-color") + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());
                            // Assert.AreEqual(backgroundHexColor, rowListSelenium[rowCounter + 1].FindElements(By.TagName("td"))[8].FindElement(By.Id("ContentPlaceHolder1_gvChannelCreation_colorDiv_"+rowCounter)).GetCssValue("background-color"));

                            //write assert to check  Player
                            Assert.AreEqual(playerText, currentRow.GetElementsByTag("td")[9].GetElementById("ContentPlaceHolder1_gvChannelCreation_lblPlayer_" + rowCounter).OwnText().Trim());

                            //write assert to check edit and delete button
                            Assert.AreEqual(isEditbutton, rowListSelenium[rowCounter + 1].FindElements(By.TagName("td"))[11].FindElements(By.CssSelector("span > img")).Count);
                            Assert.AreEqual(isDeleteButton, rowListSelenium[rowCounter + 1].FindElements(By.TagName("td"))[11].FindElement(By.Id("ContentPlaceHolder1_gvChannelCreation_ImgDelete_" + rowCounter)).FindElement(By.TagName("img")).GetAttribute("src").Contains("Delete.png"));

                            //write assert to check Channel name is present of not
                            log.Info("Yes Channel is present on Manage" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());
                            break;
                        }
                        rowCounter++;
                    }
                }
                Assert.AreEqual(true, flag);   //If It fails then Edited Channel name is not displyed on Manage Page

                #endregion

                #endregion

                log.Info("Scenario 2 : Changing data and clicking on Cancel button started" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());
               
                #region Scenario 2 : Changing data and clicking on Cancel button

                //  log.Info("Inside Scenarion 2");


                //   channelName = createChannel("Category Name");

                //  #region Region to Click on Edit button

                //  //nsoup code to get result from table faster
                //   doc = NSoup.NSoupClient.Parse(driver.PageSource);
                //  rowListNsoup = doc.GetElementById("ContentPlaceHolder1_gvChannelCreation").GetElementsByTag("tr");

                //rowListSelenium = (IList<IWebElement>)driver.FindElement(By.Id("ContentPlaceHolder1_gvChannelCreation")).FindElements(By.TagName("tr"));

                //   rowCounter = 0;
                //  flag = false;
                //  //IWebElement tempCurrentRow = null;
                //  foreach (Element currentRow in rowListNsoup)
                //  {
                //      Attributes attr = currentRow.Attributes;

                //      // Row that have class="GridRowStyle" or class="AltGridStyle"
                //      if (attr["class"].Equals("GridRowStyle") || attr["class"].Equals("AltGridStyle"))
                //      {
                //          log.Info("Row Count :: " + rowCounter);
                //          columData = currentRow.GetElementsByTag("td")[3].GetElementById("ContentPlaceHolder1_gvChannelCreation_lblName_" + rowCounter).OwnText().Trim();
                //         
                //          if (columData.Equals(channelName))
                //          {
                //              flag = true;

                //              //Clicking on Edit button
                //              rowListSelenium[rowCounter + 1].FindElements(By.TagName("td"))[11].FindElement(By.Id("ContentPlaceHolder1_gvChannelCreation_imgEdit_" + rowCounter)).FindElement(By.TagName("img")).Click();

                //              //write assert to check Channel name is present of not
                //              log.Info("Yes Channel is present on Manage");
                //              break;
                //          }
                //          rowCounter++;
                //      }
                //  }
                //  Assert.AreEqual(true, flag);   //If It fails then Edited Channel name is not displyed on Manage Page

                //  #endregion


                //  #region filling_Details

                //  //Waiting and Entering Channel Name
                //  iWait.Until(ExpectedConditions.ElementIsVisible(By.Id("ContentPlaceHolder1_txtChannelName")));
                //  driver.FindElement(By.Id(OR.readingXMLFile("ChannelManagement", "ChannelName", "TVAdminPortalOR.xml"))).Clear();
                //  driver.FindElement(By.Id(OR.readingXMLFile("ChannelManagement", "ChannelName", "TVAdminPortalOR.xml"))).SendKeys(cancelbuttonfunctionality);

                //  //Getting control of Channel Category dropdown. 
                //  ChannelCategory = driver.FindElement(By.Id(OR.readingXMLFile("ChannelManagement", "ChannelCategory", "TVAdminPortalOR.xml")));
                //  SelectElement ChannelCategorySelector = new SelectElement(ChannelCategory);

                //  // getting number of categories in the channel category field dropdown
                //  numberOfCategory = ChannelCategorySelector.Options.Count;

                //  randomSelector = random.Next(1, numberOfCategory);  //Generating random number to select category from dropdown

                //  //ChannelCategorySelector.SelectByIndex(Convert.ToInt32(randomSelector));  //Seelcting Category from Channel Category Dropdown.
                //  ChannelCategorySelector.SelectByIndex(1);

                //  Thread.Sleep(5000);    //need to see to replace Sleep()

                //  //Getting control of Channel Type dropdown.
                //  ChannelType = driver.FindElement(By.Id(OR.readingXMLFile("ChannelManagement", "ChannelType", "TVAdminPortalOR.xml")));
                //  SelectElement ChannelTypeSelector = new SelectElement(ChannelType);
                //  ChannelTypeSelector.SelectByText("Standard");

                //  //Getting Control of Position dropdown
                //  iWait.Until(ExpectedConditions.ElementExists(By.Id(OR.readingXMLFile("ChannelManagement", "ChannelPosition", "TVAdminPortalOR.xml"))));
                //  ChannelPosition = driver.FindElement(By.Id(OR.readingXMLFile("ChannelManagement", "ChannelPosition", "TVAdminPortalOR.xml")));
                //  SelectElement ChannelPositionSelector = new SelectElement(ChannelPosition);
                //  ChannelPositionSelector.SelectByText("1");

                //  //Getting control and selecting Pricing Type
                //  PricingType = driver.FindElement(By.Id(OR.readingXMLFile("ChannelManagement", "PricingType", "TVAdminPortalOR.xml")));
                //  SelectElement PricingTypeSelector = new SelectElement(PricingType);
                //  PricingTypeSelector.SelectByText("Free");


                //  #region Handling Background Color Picker

                //  BackgroundColourPicker = driver.FindElement(By.Id(OR.readingXMLFile("ChannelManagement", "BackgroundColour", "TVAdminPortalOR.xml")));                        //BackgroundColour
                //  BackgroundColourPicker.Click();

                //  randomSelector = random.Next(1, 10);

                //  backgroundColourCollection = (IList<IWebElement>)driver.FindElement(By.CssSelector(OR.readingXMLFile("ChannelManagement", "ColourCollection", "TVAdminPortalOR.xml"))).FindElements(By.TagName("tr"))[1].FindElements(By.TagName("td"));
                //   bgColour = backgroundColourCollection[Convert.ToInt32(randomSelector)].GetAttribute("style").Split(':')[1].Split('(')[1].Split(')')[0].Split(',');

                //  // converting RGB() to hexadecimal for background colour
                //    red = Convert.ToInt32(bgColour[0]);
                //   green = Convert.ToInt32(bgColour[1]);
                //    blue = Convert.ToInt32(bgColour[2]);
                //  backgroundHexColor = "#" + red.ToString("X") + green.ToString("X") + blue.ToString("X");

                //  BackgroundColourPicker.Clear();
                //  BackgroundColourPicker.SendKeys(backgroundHexColor);

                //  //Clicking on Body to close color picker.
                //  driver.FindElement(By.Id("ContentPlaceHolder1_rdoChannelAccessType")).FindElements(By.TagName("tr"))[0].FindElement(By.TagName("td")).FindElement(By.TagName("label")).Click();

                //  #endregion


                //  //Getting control of Player dropdown
                //  Player = driver.FindElement(By.Id(OR.readingXMLFile("ChannelManagement", "Player", "TVAdminPortalOR.xml")));                                     //Player
                //  SelectElement PlayerSelector = new SelectElement(Player);

                //  //Selecting Player from Player List.
                //  PlayerSelector.SelectByText("Player1");

                //  #region Handling WaterMark

                //  randomSelector = random.Next(0, 1);
                //  watermarkRequired = driver.FindElement(By.Id(OR.readingXMLFile("ChannelManagement", "WatermarkRequired", "TVAdminPortalOR.xml")));             //watermark field
                //  SelectElement watermarkRequiredSelector = new SelectElement(watermarkRequired);

                //  watermarkRequiredSelector.SelectByIndex(randomSelector);

                //  driver.FindElement(By.Id("ContentPlaceHolder1_btnCancel")).Click();

                //  #endregion


                //  #endregion

                //  iWait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.Id("ContentPlaceHolder1_txtChannelName")));

                //  uf.isJqueryActive(driver);

                //  Assert.AreEqual(false, driver.PageSource.Contains(cancelbuttonfunctionality));  //Checking that Channel is not Edited.
                //  Assert.AreEqual(true, driver.PageSource.Contains(channelName));  //Checking that Channel is not Edited.


                #endregion

                // //need to add existing channel name-duplication

                log.Info("Scenario 3 : started" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());
               
                #region Scenario 3 : Chekcing Duplication Message at the time of Edit

                //String duplicateChannelName = createChannel("Category Name");

                ////String recentlyCreatedChannelName = channelName;

                //////randomDataList.Clear();
                //////completeDataList.Clear();
                //////extraDataList.Clear();

                //String channelNeedtoEdit = createChannel("Category Name");


                #region Region to Click on Edit button

                ////nsoup code to get result from table faster
                //doc = NSoup.NSoupClient.Parse(driver.PageSource);
                //rowListNsoup = doc.GetElementById("ContentPlaceHolder1_gvChannelCreation").GetElementsByTag("tr");

                //rowListSelenium = (IList<IWebElement>)driver.FindElement(By.Id("ContentPlaceHolder1_gvChannelCreation")).FindElements(By.TagName("tr"));

                //rowCounter = 0;
                //flag = false;
                ////IWebElement tempCurrentRow = null;
                //foreach (Element currentRow in rowListNsoup)
                //{
                //    Attributes attr = currentRow.Attributes;

                //    // Row that have class="GridRowStyle" or class="AltGridStyle"
                //    if (attr["class"].Equals("GridRowStyle") || attr["class"].Equals("AltGridStyle"))
                //    {
                //       
                //        columData = currentRow.GetElementsByTag("td")[3].GetElementById("ContentPlaceHolder1_gvChannelCreation_lblName_" + rowCounter).OwnText().Trim();
                //       
                //        if (columData.Equals(channelNeedtoEdit))
                //        {
                //            flag = true;

                //            //Clicking on Edit button
                //            rowListSelenium[rowCounter + 1].FindElements(By.TagName("td"))[11].FindElement(By.Id("ContentPlaceHolder1_gvChannelCreation_imgEdit_" + rowCounter)).FindElement(By.TagName("img")).Click();

                //            //write assert to check Channel name is present of not
                //           log.Info("Yes Channel is present on Manage");
                //            break;
                //        }
                //        rowCounter++;
                //    }
                //}
                //Assert.AreEqual(true, flag);   //If It fails then Edited Channel name is not displyed on Manage Page

                #endregion


                #region Region to edit the Category

                //iWait.Until(ExpectedConditions.ElementIsVisible(By.Id("ContentPlaceHolder1_txtChannelName")));

                //driver.FindElement(By.Id("ContentPlaceHolder1_txtChannelName")).Clear();  //Clearing pre populated channel name
                //driver.FindElement(By.Id("ContentPlaceHolder1_txtChannelName")).SendKeys(duplicateChannelName);

                ////clicking on save button
                //driver.FindElement(By.Id("ContentPlaceHolder1_btnSave")).Click();


                ////Verfiying User friendly message
                //iWait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("div.infomsg > button.btn-sm")));
                //Assert.AreEqual("Duplicate Record Found", driver.FindElement(By.Id("Info")).Text.Trim());    //Verifying infomration message for Duplicate Category.
                //driver.FindElement(By.CssSelector("div.infomsg > button.btn-sm")).Click();

                ////iWait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector("div.infomsg > button.btn-sm")));


                #endregion


                #endregion

                log.Info("TVAdmin_008_EditingChannel test completed" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

            }
            catch (Exception e)
            {
                log.Error(e.Message + "\n" + e.StackTrace + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());
                Assert.AreEqual(true, false);
            }

        }

        [Test]
        public void TVAdmin_009_PositionChangeFunc()
        {

            try
            {
                log.Info("TVAdmin_009_PositionChangeFunc test started" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

                RedirectToChannelManagement();

                String PostitionBeforeChange = "";

                int changedPosition = 0;                

                String ChannelNameWhosePositionIsGoingToChanged = CreateChannel("Category Name");

                CreateChannel("Category Name");


                #region Region to Check NO button functionality of Warning Message

                // Geting all the record of manage grid
                IList<IWebElement> rowList = (IList<IWebElement>)driver.FindElement(By.Id("ContentPlaceHolder1_gvChannelCreation")).FindElements(By.TagName("tr"));

                int i = 0;
                flag = false;
                foreach (IWebElement currentRow in rowList)
                {
                    // Row that have class="GridRowStyle" or class="AltGridStyle"
                    if (currentRow.GetAttribute("class").Equals("GridRowStyle") || currentRow.GetAttribute("class").Equals("AltGridStyle"))
                    {
                        log.Info("Row Count :: " + i + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

                        columData = currentRow.FindElements(By.TagName("td"))[3].FindElement(By.TagName("span")).Text.Trim();

                        if (columData.Equals(ChannelNameWhosePositionIsGoingToChanged))
                        {
                            flag = true;

                            SelectElement positionDropdown = new SelectElement(currentRow.FindElements(By.TagName("td"))[1].FindElement(By.Id("ContentPlaceHolder1_gvChannelCreation_ddlPosition_" + i)));
                            PostitionBeforeChange = positionDropdown.SelectedOption.Text.Trim();

                            Random generator = new Random();
                            changedPosition = generator.Next(1, positionDropdown.Options.Count);
                            while (changedPosition == Convert.ToInt32(PostitionBeforeChange))
                            {
                                changedPosition = generator.Next(1, positionDropdown.Options.Count);
                            }

                            positionDropdown.SelectByText(changedPosition.ToString());

                            iWait.Until(ExpectedConditions.ElementIsVisible(By.Id("btnNo")));
                            Assert.AreEqual("Are you sure, you wish to change?", driver.FindElement(By.Id("Default_Language")).Text.Trim());  //Verfying that correct Warning MEssage is displayed
                            driver.FindElement(By.Id("btnNo")).Click();   //Clicking on No button of Warning Message
                            break;
                        }
                        i++;
                    }
                }

                #endregion

                log.Info("Region to check Whether position change started" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());
               
                #region Region to check Whether position change

                i = 0;
                rowList = (IList<IWebElement>)driver.FindElement(By.Id("ContentPlaceHolder1_gvChannelCreation")).FindElements(By.TagName("tr"));
                foreach (IWebElement currentRow in rowList)
                {
                    if (currentRow.GetAttribute("class").Equals("GridRowStyle") || currentRow.GetAttribute("class").Equals("AltGridStyle"))
                    {
                        columData = currentRow.FindElements(By.TagName("td"))[3].FindElement(By.TagName("span")).Text.Trim();
                        if (columData.Equals(ChannelNameWhosePositionIsGoingToChanged))
                        {
                            Assert.AreEqual(PostitionBeforeChange, currentRow.FindElements(By.TagName("td"))[1].FindElement(By.Id("ContentPlaceHolder1_gvChannelCreation_hdnPosition_" + i)).GetAttribute("value").Trim());
                            break;
                        }
                        i++;
                    }
                }

                #endregion

                #region Region to Check YES button functionality of Warning Message

                rowList = (IList<IWebElement>)driver.FindElement(By.Id("ContentPlaceHolder1_gvChannelCreation")).FindElements(By.TagName("tr"));


                i = 0;
                flag = false;
                IWebElement tempCurrentRow = null;
                foreach (IWebElement currentRow in rowList)
                {
                    // Row that have class="GridRowStyle" or class="AltGridStyle"
                    if (currentRow.GetAttribute("class").Equals("GridRowStyle") || currentRow.GetAttribute("class").Equals("AltGridStyle"))
                    {
                        log.Info("Row Count :: " + i + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

                        columData = currentRow.FindElements(By.TagName("td"))[3].FindElement(By.TagName("span")).Text.Trim();

                        if (columData.Equals(ChannelNameWhosePositionIsGoingToChanged))
                        {
                            flag = true;

                            SelectElement positionDropdown = new SelectElement(currentRow.FindElements(By.TagName("td"))[1].FindElement(By.Id("ContentPlaceHolder1_gvChannelCreation_ddlPosition_" + i)));
                            PostitionBeforeChange = positionDropdown.SelectedOption.Text.Trim();

                            Random generator = new Random();
                            changedPosition = generator.Next(1, positionDropdown.Options.Count);
                            while (changedPosition == Convert.ToInt32(PostitionBeforeChange))
                            {
                                changedPosition = generator.Next(1, positionDropdown.Options.Count);
                            }

                            positionDropdown.SelectByText(changedPosition.ToString());

                            iWait.Until(ExpectedConditions.ElementIsVisible(By.Id("btnYes")));
                            Assert.AreEqual("Are you sure, you wish to change?", driver.FindElement(By.Id("Default_Language")).Text.Trim());  //Verfying that correct Warning MEssage is displayed
                            driver.FindElement(By.Id("btnYes")).Click();   //Clicking on Yes button of Warning Message

                            break;
                        }
                        i++;
                    }
                }

                #endregion


                iWait.Until(ExpectedConditions.ElementIsVisible(By.Id("btnOk")));
                Assert.AreEqual("Position is Updated Successfully.", driver.FindElement(By.Id("Sucess_Message")).Text.Trim());   //Verifying that Successcull message is displayed after delete
                driver.FindElement(By.Id("btnOk")).Click();     //Clicking on Ok button of Success Message

                iWait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.Id("btnOk")));

                #region Region to check Whether position changed ?

                i = 0;
                rowList = (IList<IWebElement>)driver.FindElement(By.Id("ContentPlaceHolder1_gvChannelCreation")).FindElements(By.TagName("tr"));
                foreach (IWebElement currentRow in rowList)
                {
                    if (currentRow.GetAttribute("class").Equals("GridRowStyle") || currentRow.GetAttribute("class").Equals("AltGridStyle"))
                    {
                        columData = currentRow.FindElements(By.TagName("td"))[3].FindElement(By.TagName("span")).Text.Trim();
                        if (columData.Equals(ChannelNameWhosePositionIsGoingToChanged))
                        {
                            Assert.AreEqual(changedPosition.ToString(), currentRow.FindElements(By.TagName("td"))[1].FindElement(By.Id("ContentPlaceHolder1_gvChannelCreation_hdnPosition_" + i)).GetAttribute("value").Trim());
                            break;
                        }
                        i++;
                    }
                }

                #endregion

                log.Info("TVAdmin_009_PositionChangeFunc test completed" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

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
