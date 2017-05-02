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
using NSoup.Select;
using System.Data.SqlClient;
using System.Data;
using Sikuli4Net.sikuli_UTIL;
using Sikuli4Net.sikuli_REST;




namespace IETTVAdminPortal.Chrome
{

    [TestFixture]
    public class Chrome_BuyChannel
    {
        #region variable declaration and object initialisation

        private readonly Random _rng = new Random();

        private const string _chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

        // This is to configure logger mechanism for Utilities.Config
        private static ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        string connetionString = null;

        string driverName = "", driverPath, appURL;

        internal IWebDriver driver = null;
        
        int rowcounter;

        //Instantiating Utilities function class

        Utility_Functions uf = new Utility_Functions();                                         // Instantiate object for Utility Functons

        Object_Repository_Class OR = new Object_Repository_Class();

        Chrome_AdminSetupTearDown st = new Chrome_AdminSetupTearDown();

        Chrome_BuyChannelVerification_New videoResult;

        Chrome_BuyChannelVerification_New buyVidVerification;

        Chrome_VideoManagement objAdminVideoManagement = null;

        // Instantiate object of admin portal
        IETTVAdminPortal.Chrome.Chrome_IndividualChannelPricing icpAdmin = null;

        //variables used while creating channel
        String backgroundHexColor, buttonHexColor, successMessage;

        int numberOfCategory, randomSelector;

        IWebElement ChannelCategory, ChannelType, ChannelPosition, PricingType, BackgroundColourPicker, ButtonColourPicker, Player, watermarkRequired,
                   ChannelIcon, ShowChannelIcon, BackgroundImage, Notes, Savebutton, okButton;

        IJavaScriptExecutor executor;

        IWebElement checkBox;

        List<String> ChannelDataList = new List<String>();  //List to store current channel details.

        List<string> globList;

        Random random = new Random();  // Obejct created to generate Random Number 

        Configuration cf = new Configuration();                                                  // Instantiate object for Configuration

        IWait<IWebDriver> iWait = null;

        APILauncher launcher;
        

        #endregion

        #region Constructors

        public Chrome_BuyChannel()
        {

        }

        public Chrome_BuyChannel(IWebDriver driver, ILog log1, IJavaScriptExecutor Executor, IWait<IWebDriver> iWait)
        {
            this.driver = driver;

            log = log1;

            this.executor = Executor;

            this.iWait = iWait;

        }


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

            videoResult = new Chrome_BuyChannelVerification_New(driver, log, executor, iWait);            // Creating a object for calling IETTVWebPortal project

            buyVidVerification = new Chrome_BuyChannelVerification_New(driver, log, executor, iWait);

            objAdminVideoManagement = new IETTVAdminPortal.Chrome.Chrome_VideoManagement(driver, log, executor, iWait);

            appURL = st.Chrome_Setup(driver, log, executor);                                               // Calling Chrome Setup  

            IETTVAdminPortal.Chrome.Chrome_IndividualChannelPricing icpAdmin = new IETTVAdminPortal.Chrome.Chrome_IndividualChannelPricing(driver, log, executor, iWait); 
        }

        #endregion

        #region Functions
        //This function select the channel
        public String channelListTab(string channelName)
        {
            log.Info("inside channelListTab " + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

            uf.scrollUp(driver);

            //Click on Channel tab
            objAdminVideoManagement.channelTab().FindElement(By.TagName("a")).Click();

            //Selecting channel from the default Channel dropdown
            SelectElement DefaultChanneleSelector = new SelectElement(objAdminVideoManagement.channelDefaultDropDown());

            //getting number of channels from default channel dropdown
            int NumberofChannels = DefaultChanneleSelector.Options.Count;
            log.Info("number of channels in default channel dropdown:: " + NumberofChannels);

            //check number of channel in the dropdown if it is zero then first create channel
            if (NumberofChannels == 0)
            {
                log.Info("No default channel Present:: ");
                System.Windows.Forms.Application.Exit();
                //Call Create Channel                
            }

            //String memberChannelName = cf.readingXMLFile("AdminPortal", "ChannelManagement", "MemberChannelName", "SysConfig.xml");

            DefaultChanneleSelector.SelectByText(channelName);

            String adminChannelName = DefaultChanneleSelector.SelectedOption.Text;

            log.Info("Selected channel name :" + adminChannelName);

            return adminChannelName;

        }

        //Create paid channel
        public string CreatePaidChannel(String CategoryName)
        {
            log.Info("Inside CreatePaidChannel Function" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

            //Waiting and clicking on Create Tab.
            iWait.Until(ExpectedConditions.ElementIsVisible((OR.GetElement("ChannelManagement", "ChannelCreateTab", "TVAdminPortalOR.xml"))));
            iWait.Until(ExpectedConditions.ElementToBeClickable((OR.GetElement("ChannelManagement", "ChannelCreateTab", "TVAdminPortalOR.xml"))));
            driver.FindElement((OR.GetElement("ChannelManagement", "ChannelCreateTab", "TVAdminPortalOR.xml"))).Click();

            #region filling_Details

            string channelNameWithGuid = "Chan " + uf.getGuid();  // Concatenating the GUID 

            //Waiting and Entering Channel Name
            iWait.Until(ExpectedConditions.ElementExists(OR.GetElement("ChannelManagement", "ChannelName", "TVAdminPortalOR.xml")));
            driver.FindElement((OR.GetElement("ChannelManagement", "ChannelName", "TVAdminPortalOR.xml"))).SendKeys(channelNameWithGuid);

            #region Pending
            //refer cancelbuttonfunctionality
            #endregion

            //Getting control of Channel Category dropdown. 
            ChannelCategory = driver.FindElement((OR.GetElement("ChannelManagement", "ChannelCategory", "TVAdminPortalOR.xml")));
            SelectElement ChannelCategorySelector = new SelectElement(ChannelCategory);

            // getting number of categories in the channel category field dropdown
            numberOfCategory = ChannelCategorySelector.Options.Count;

            log.Info("Number of categories : " + numberOfCategory + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

            randomSelector = random.Next(1, numberOfCategory);  //Generating random number to select category from dropdown

            ChannelCategorySelector.SelectByText(CategoryName);

            Thread.Sleep(5000);    //need to see to replace Sleep()

            //Getting control of Channel Type dropdown.
            ChannelType = driver.FindElement((OR.GetElement("ChannelManagement", "ChannelType", "TVAdminPortalOR.xml")));
            SelectElement ChannelTypeSelector = new SelectElement(ChannelType);
            ChannelTypeSelector.SelectByText("Standard");


            //Getting Control of Position dropdown
            iWait.Until(d => new SelectElement(d.FindElement((OR.GetElement("ChannelManagement", "ChannelPosition", "TVAdminPortalOR.xml")))).Options.Count > 0);
            ChannelPosition = driver.FindElement((OR.GetElement("ChannelManagement", "ChannelPosition", "TVAdminPortalOR.xml")));
            SelectElement ChannelPositionSelector = new SelectElement(ChannelPosition);
            ChannelPositionSelector.SelectByText("1");

            //Getting control and selecting Pricing Type
            PricingType = driver.FindElement((OR.GetElement("ChannelManagement", "PricingType", "TVAdminPortalOR.xml")));
            SelectElement PricingTypeSelector = new SelectElement(PricingType);
            PricingTypeSelector.SelectByText("Paid");

            //#region Handling Background Color Picker

            //// Clicking on Color Picker
            //BackgroundColourPicker = driver.FindElement((OR.GetElement("ChannelManagement", "BackgroundColour", "TVAdminPortalOR.xml")));                        //BackgroundColour
            //BackgroundColourPicker.Click();

            //randomSelector = random.Next(1, 10);    // Generating Random Number to select a color from color picker.

            //IList<IWebElement> backgroundColourCollection = (IList<IWebElement>)driver.FindElement((OR.GetElement("ChannelManagement", "ColourCollection", "TVAdminPortalOR.xml"))).FindElements(By.TagName("tr"))[1].FindElements(By.TagName("td"));
            //String[] bgColour = backgroundColourCollection[Convert.ToInt32(randomSelector)].GetAttribute("style").Split(':')[1].Split('(')[1].Split(')')[0].Split(',');

            //// converting RGB() to hexadecimal for background colour
            //int red = Convert.ToInt32(bgColour[0]);
            //int green = Convert.ToInt32(bgColour[1]);
            //int blue = Convert.ToInt32(bgColour[2]);
            //backgroundHexColor = "#" + red.ToString("X") + green.ToString("X") + blue.ToString("X");

            //// Clearing and Entering Selected color into color text field.
            //BackgroundColourPicker.Clear();
            //BackgroundColourPicker.SendKeys(backgroundHexColor);

            ////Clicking on Body to close color picker.
            //driver.FindElement(By.Id("ContentPlaceHolder1_rdoChannelAccessType")).FindElements(By.TagName("tr"))[0].FindElement(By.TagName("td")).FindElement(By.TagName("label")).Click();

            //#endregion

            //#region Handling Button Color Picker

            //// Waiting to close the Background Color picker.
            //iWait.Until(ExpectedConditions.InvisibilityOfElementLocated((OR.GetElement("ChannelManagement", "ColourCollection", "TVAdminPortalOR.xml"))));

            //// Clicking on Color Picker
            //ButtonColourPicker = driver.FindElement((OR.GetElement("BuyChannel", "CP3", "TVAdminPortalOR.xml")));
            //ButtonColourPicker.Click();

            //randomSelector = random.Next(1, 10);     // Generating Random Number to select a color from color picker.

            //iWait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("table.evo-palette")));

            //IList<IWebElement> ButtonColourCollection = (IList<IWebElement>)driver.FindElement((OR.GetElement("ChannelManagement", "ColourCollection", "TVAdminPortalOR.xml"))).FindElements(By.TagName("tr"))[3].FindElements(By.TagName("td"));
            //String[] buttonColour = ButtonColourCollection[Convert.ToInt32(randomSelector)].GetAttribute("style").Split(':')[1].Split('(')[1].Split(')')[0].Split(',');

            ////converting RGB() to hexadecimal for button colour
            //red = Convert.ToInt32(buttonColour[0]);
            //green = Convert.ToInt32(buttonColour[1]);
            //blue = Convert.ToInt32(buttonColour[2]);
            //buttonHexColor = "#" + red.ToString("X") + green.ToString("X") + blue.ToString("X");

            //// Clearing and Entering Selected color into color text field.
            //ButtonColourPicker.Clear();
            //ButtonColourPicker.SendKeys(buttonHexColor);

            ////Clicking on Body to close color picker.
            //driver.FindElement(By.Id("ContentPlaceHolder1_rdoChannelAccessType")).FindElements(By.TagName("tr"))[0].FindElement(By.TagName("td")).FindElement(By.TagName("label")).Click();

            //#endregion

            ////Getting control of Player dropdown
            //Player = driver.FindElement((OR.GetElement("ChannelManagement", "Player", "TVAdminPortalOR.xml")));                                     //Player
            //SelectElement PlayerSelector = new SelectElement(Player);

            ////Selecting Player from Player List.
            //PlayerSelector.SelectByText("Player1");

            //#region Handling WaterMark

            //// Selecitng Yes or No option
            //randomSelector = random.Next(0, 1);
            //watermarkRequired = driver.FindElement((OR.GetElement("ChannelManagement", "WatermarkRequired", "TVAdminPortalOR.xml")));             //watermark field
            //SelectElement watermarkRequiredSelector = new SelectElement(watermarkRequired);
            //watermarkRequiredSelector.SelectByIndex(randomSelector);

            //#endregion

            //// Getting control to Channel Upload Element
            //iWait.Until(ExpectedConditions.ElementExists((OR.GetElement("ChannelManagement", "ChannelIcon", "TVAdminPortalOR.xml"))));
            //ChannelIcon = driver.FindElement((OR.GetElement("ChannelManagement", "ChannelIcon", "TVAdminPortalOR.xml")));                          //Channel Icon upload button

            ////reading xml file to upload image
            //String uploadChannel = cf.readingXMLFile("AdminPortal", "Channel_Management", "channelIcon", "Config.xml");
            //string ChannelIconPath = Environment.CurrentDirectory + "\\Upload\\Images\\" + uploadChannel;
            //uf.uploadfile(ChannelIcon, ChannelIconPath);


            //iWait.Until(ExpectedConditions.ElementExists((OR.GetElement("ChannelManagement", "ShowChannelIcon", "TVAdminPortalOR.xml"))));
            //ShowChannelIcon = driver.FindElement((OR.GetElement("BuyChannel", "ChkShowOnPortal", "TVAdminPortalOR.xml")));                     //show in channel icon checkbox
            //ShowChannelIcon.Click();

            //iWait.Until(ExpectedConditions.ElementExists((OR.GetElement("ChannelManagement", "BackgroundImage", "TVAdminPortalOR.xml"))));         //BackgroundImage upload button
            //BackgroundImage = driver.FindElement((OR.GetElement("BuyChannel", "BackgroundImage", "TVAdminPortalOR.xml")));

            ////reading xml file to upload backgroung image
            //String uploadBgImage = cf.readingXMLFile("AdminPortal", "Channel_Management", "backgroundImage", "Config.xml");
            //string bgImagePath = Environment.CurrentDirectory + "\\Upload\\Images\\" + uploadBgImage;
            //uf.uploadfile(BackgroundImage, bgImagePath);

            //Notes = driver.FindElement((OR.GetElement("ChannelManagement", "Notes", "TVAdminPortalOR.xml")));                   //Note field
            //Notes.SendKeys("Hello this is automation test");

            //ChannelDataList.Add("1");                                             // (0) Adding Selected Position into ChannelDataList.  
            //ChannelDataList.Add(CategoryName);                                     // (1) Adding Selected Category into List.
            //ChannelDataList.Add(ChannelTypeSelector.SelectedOption.Text.Trim());  //(2) ChannelType
            //ChannelDataList.Add("Free");                                         // (3) pricing type
            //ChannelDataList.Add(driver.FindElement((OR.GetElement("BuyChannel", "ChannelAccessTypeRDO", "TVAdminPortalOR.xml"))).FindElements(By.TagName("tr"))[0].FindElement(By.TagName("td")).FindElement(By.TagName("label")).Text.Trim());  //accesstype
            //ChannelDataList.Add(driver.FindElements((OR.GetElement("BuyChannel", "ColorId", "TVAdminPortalOR.xml")))[0].GetCssValue("background-color"));      // (5) bgcolour
            //ChannelDataList.Add(PlayerSelector.SelectedOption.Text.Trim());                                                // (6) player


            //// Storing the Current selected Watermark option from dropdown
            //if (watermarkRequiredSelector.SelectedOption.Text.Trim().Equals("No"))
            //{
            //    ChannelDataList.Add("None");                                    // (7) waterMark
            //}
            //else if (watermarkRequiredSelector.SelectedOption.Text.Trim().Equals("Yes"))
            //{
            //    SelectElement watermarkDropdown = new SelectElement(driver.FindElement((OR.GetElement("BuyChannel", "WaterMarkDDL", "TVAdminPortalOR.xml"))));
            //    ChannelDataList.Add(watermarkDropdown.SelectedOption.Text.Trim());
            //}

            #endregion

            Thread.Sleep(5000);
            uf.scrollDown(driver);
            //Clicking on Save button
            Savebutton = driver.FindElement((OR.GetElement("ChannelManagement", "Savebutton", "TVAdminPortalOR.xml")));
            executor.ExecuteScript("arguments[0].click();", Savebutton);


            // Wait for banner message to appear

            iWait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy((OR.GetElement("BuyChannel", "CustomMessage", "TVAdminPortalOR.xml"))));

            // Success banner message

            iWait.Until(ExpectedConditions.ElementExists((OR.GetElement("ChannelManagement", "OkButton", "TVAdminPortalOR.xml"))));

            iWait.Until(ExpectedConditions.TextToBePresentInElementLocated((OR.GetElement("BuyChannel", "SpanCustomMessage", "TVAdminPortalOR.xml")), "Channel Added Successfully."));

            successMessage = driver.FindElement((OR.GetElement("ChannelManagement", "SuccessMessage", "TVAdminPortalOR.xml"))).Text;

            uf.isJavaScriptActive(driver);

            Assert.AreEqual("Channel Added Successfully.", successMessage);

            //Click on ok button of banner message
            okButton = driver.FindElement((OR.GetElement("ChannelManagement", "OkButton", "TVAdminPortalOR.xml")));               //Save button
            executor.ExecuteScript("arguments[0].click();", okButton);

            return channelNameWithGuid;
        }

        //Used for updating the random string for Product Code in Channel Pricing
        private string RandomString(int size)
        {
            char[] buffer = new char[size];

            for (int i = 0; i < size; i++)
            {
                buffer[i] = _chars[_rng.Next(_chars.Length)];
            }
            return new string(buffer);
        }

        // This function will redirect the control to Channel Management Page
        public void RedirectToChannelManagement()
        {
            log.Info("Inside RedirectToChannelManagement Function" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

            iWait.Until(ExpectedConditions.ElementIsVisible((OR.GetElement("ChannelManagement", "AdminMenu", "TVAdminPortalOR.xml"))));

            // Clicking on Admin dropdown
            driver.FindElement((OR.GetElement("ChannelManagement", "AdminMenu", "TVAdminPortalOR.xml"))).Click();

            Thread.Sleep(3000);

            iWait.Until(ExpectedConditions.ElementToBeClickable((OR.GetElement("ChannelManagement", "ChannelManagement", "TVAdminPortalOR.xml"))));

            // Clicking on Channel Management
            driver.FindElement((OR.GetElement("ChannelManagement", "ChannelManagement", "TVAdminPortalOR.xml"))).Click();

            iWait.Until(ExpectedConditions.ElementIsVisible((OR.GetElement("ChannelManagement", "ChannelManagementLabel", "TVAdminPortalOR.xml"))));
            Boolean isChannelPage = driver.FindElement((OR.GetElement("ChannelManagement", "ChannelManagementLabel", "TVAdminPortalOR.xml"))).Displayed;
            Assert.AreEqual(true, isChannelPage);      //Checking whether the user is on Channel page

            //verifying the default active tab of Channel management page
            Assert.AreEqual(String.Empty, driver.FindElement((OR.GetElement("ChannelManagement", "ChannelCreateTab", "TVAdminPortalOR.xml"))).GetAttribute("class"));
            Assert.AreEqual("active", driver.FindElement(OR.GetElement("ChannelManagement", "TabChannel", "TVAdminPortalOR.xml")).GetAttribute("class"));
        }

        public void RedirectToProductCode()
        {
            log.Info("Inside RedirectToProductCode Function" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

            iWait.Until(ExpectedConditions.ElementIsVisible((OR.GetElement("ChannelManagement", "AdminMenu", "TVAdminPortalOR.xml"))));

            // Clicking on Admin dropdown
            driver.FindElement((OR.GetElement("ChannelManagement", "AdminMenu", "TVAdminPortalOR.xml"))).Click();

            Thread.Sleep(3000);

            iWait.Until(ExpectedConditions.ElementToBeClickable((OR.GetElement("ProductCode", "ProductCode", "TVAdminPortalOR.xml"))));

            // Clicking on Product Code
            driver.FindElement((OR.GetElement("ProductCode", "ProductCode", "TVAdminPortalOR.xml"))).Click();

            iWait.Until(ExpectedConditions.ElementIsVisible((OR.GetElement("ProductCode", "ProductCodeLabel", "TVAdminPortalOR.xml"))));
            Boolean isProductCodePage = driver.FindElement((OR.GetElement("ProductCode", "ProductCodeLabel", "TVAdminPortalOR.xml"))).Displayed;
            Assert.AreEqual(true, isProductCodePage);      //Checking whether the user is on ProductCode Page

            //verifying the default active tab of ProductCode page
            Assert.AreEqual(String.Empty, driver.FindElement((OR.GetElement("ProductCode", "ProductCodeCreateTab", "TVAdminPortalOR.xml"))).GetAttribute("class"));
            Assert.AreEqual("active", driver.FindElement((OR.GetElement("ProductCode", "ProductCodeListTab", "TVAdminPortalOR.xml"))).GetAttribute("class"));
        }

        public void UpdatingPrice(String expectedArchive, String expectedFront, String expectedAll, IETTVAdminPortal.Chrome.Chrome_IndividualChannelPricing icpAdmin)
        {
            //Enter Archive front price value
            icpAdmin.ArchiveTextField(rowcounter).Clear();
            icpAdmin.ArchiveTextField(rowcounter).SendKeys(expectedArchive);

            //Enter random front price value
            icpAdmin.FrontTextField(rowcounter).Clear();
            icpAdmin.FrontTextField(rowcounter).SendKeys(expectedFront);

            //Enter random All price value
            icpAdmin.AllTextField(rowcounter).Clear();
            icpAdmin.AllTextField(rowcounter).SendKeys(expectedAll);

            icpAdmin.SaveButton().Click();
        }

        //This function verify the status of all elements as per checkbox status
        public void VerifyStatusOfElements(IETTVAdminPortal.Chrome.Chrome_IndividualChannelPricing icpAdmin)
        {
            Boolean checkBoxStatus = icpAdmin.IsElementChecked("ContentPlaceHolder1_GridView1_chkRow_" + rowcounter);


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
            Assert.AreEqual(false, icpAdmin.IsElementChecked("ContentPlaceHolder1_GridView1_chkRow_" + rowcounter));

            //2.Archive
            Assert.AreEqual(false, icpAdmin.ArchiveTextField(rowcounter).Enabled);

            //3.Front
            Assert.AreEqual(false, icpAdmin.FrontTextField(rowcounter).Enabled);

            //4.All
            Assert.AreEqual(false, icpAdmin.AllTextField(rowcounter).Enabled);

            //perform check operation on checkbox and verify
            checkBox = driver.FindElement(OR.GetElement("BuyChannel", "GridViewCheckRow", "TVAdminPortalOR.xml",rowcounter));
            checkBox.Click();

            //Archive
            Assert.AreEqual(true, icpAdmin.ArchiveTextField(rowcounter).Enabled);

            //Front
            Assert.AreEqual(true, icpAdmin.FrontTextField(rowcounter).Enabled);

            //All
            Assert.AreEqual(true, icpAdmin.AllTextField(rowcounter).Enabled);
        }

        public void RedirectToPricing(string channelName, IETTVAdminPortal.Chrome.Chrome_IndividualChannelPricing icpAdmin)
        {
            log.Info("RedirectToChannelManagement1 started" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

            icpAdmin.RedirectToIndividualChannelPricing();

            Elements rowListNsoup = icpAdmin.GetRowListNsoup();

            log.Info("Getting Channel Name" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

            //String channelName = GetChannelName();

            rowcounter = icpAdmin.FindRequiredChannel(rowListNsoup, channelName);

            VerifyStatusOfElements(icpAdmin);

            //Enter random Archive price value 
            string expectedArchive = icpAdmin.GetRandomPrice();
            string expectedFront = icpAdmin.GetRandomPrice();
            string expectedAll = icpAdmin.GetRandomPrice();

            log.Info("Updating channel price" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

            UpdatingPrice(expectedArchive, expectedFront, expectedAll,icpAdmin);

            icpAdmin.VerifyBannerMessage();

            icpAdmin.OverlayWait();

            log.Info("verifying updated channel price" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());
        }

        public void UpdateProductCode(string channelName)
        {
           
            connetionString = cf.readingXMLFile("AdminPortal", "DBConnection", "ConnectionString", "Config.xml");

            Console.WriteLine("Procedure : updateProductCode");

            using (SqlConnection con = new SqlConnection(connetionString))
            {
                using (SqlCommand cmd = new SqlCommand("InsertProductCodeForChannel", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@ChannelName", channelName);
                    cmd.Parameters.AddWithValue("@ContentCategory", 1);
                    cmd.Parameters.AddWithValue("@ProductCode", RandomString(6));
                    con.Open();
                    cmd.ExecuteNonQuery();
                }
            }


            using (SqlConnection con = new SqlConnection(connetionString))
            {
                using (SqlCommand cmd = new SqlCommand("InsertProductCodeForChannel", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@ChannelName", channelName);
                    cmd.Parameters.AddWithValue("@ContentCategory", 2);
                    cmd.Parameters.AddWithValue("@ProductCode", RandomString(6));
                    con.Open();
                    cmd.ExecuteNonQuery();
                }
            }

            using (SqlConnection con = new SqlConnection(connetionString))
            {
                using (SqlCommand cmd = new SqlCommand("InsertProductCodeForChannel", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@ChannelName", channelName);
                    cmd.Parameters.AddWithValue("@ContentCategory", 3);
                    cmd.Parameters.AddWithValue("@ProductCode", cf.readingXMLFile("WebPortal", "BuyChannel", "ProductCodeForAll_BuyChannel", "Config.xml"));                    
                    con.Open();
                    cmd.ExecuteNonQuery();
                }
            }
            

             //con.Close();

           // cnn.Dispose();
        }

        #endregion

        #region Buy Channel Testing

        /// <summary>        
        /// This test verifies Buy Channel Functionality        
        /// First it will create the subscription video in Admin portal
        /// Then it will check the buy channel functionality on web portal
        /// </summary>
        [Test]
        public void TVAdmin_001_BuyChannelFunctionality()
        {
            try
           { 
                IETTVAdminPortal.Chrome.Chrome_IndividualChannelPricing icpAdmin = new IETTVAdminPortal.Chrome.Chrome_IndividualChannelPricing(driver, log, executor, iWait);

                log.Info("TVAdmin_001_BuyChannelFunctionality test Started" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

                string categoryName = cf.readingXMLFile("WebPortal", "BuyChannel", "CategoryName", "Config.xml");

                // moving to Channel Management Page
                RedirectToChannelManagement();

                string channelName = CreatePaidChannel(categoryName);

                UpdateProductCode(channelName);

                RedirectToPricing(channelName, icpAdmin);

                objAdminVideoManagement.redirectToVideoManagement();

                objAdminVideoManagement.basicInfoTab();

                String adminSelectedChannel = channelListTab(channelName);

                objAdminVideoManagement.pricingListTab("Subscription");

                objAdminVideoManagement.addcopyright();

                objAdminVideoManagement.uploadBrowseVideo();

                objAdminVideoManagement.finalPublishVideo("normal");

                //waiting for 2.5 minutes as video will be publish after 2minutes from current system time
                Thread.Sleep(150000);

                uf.OpenNewTab(driver);

                log.Info("count ::: " + driver.WindowHandles.Count);

                String browsertype = uf.getRunningBrowser(driver, driver.FindElement(By.TagName("html")));

                uf.SwitchToWebTab(driver, browsertype);

                uf.NavigateWebPortal(cf, driver);

                buyVidVerification.VerifyBuyChannel(objAdminVideoManagement.videoName);

                log.Info("TVAdmin_001_BuyChannelFunctionality test completed" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());
            }

            catch (Exception e)
            {
                log.Error(e.Message + "\n" + e.StackTrace + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());
                Console.WriteLine(e.Message + "\n" + e.StackTrace + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

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








