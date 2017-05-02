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
using System.Diagnostics;
using Utilities.Object_Repository;


namespace IETTVAdminPortal.IE
{
    [TestFixture]
    class IE_CategoryManagementVerification
    {

        // This is to configure logger mechanism for Utilities.Config
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
 
        #region Variable Declaration and Object Instantiation

        IWait<IWebDriver> iWait = null;
       
        Boolean flag = false;

        IJavaScriptExecutor executor;
      
        IList<IWebElement> rowList = null;
       
        String position = "1";
       
        Boolean isCategoryPage = false;
       
        SelectElement positionDropdown = null;
       
        String columData = null;
       
        string categoryNameWithGuid = "Cat";
       
        String pageSourceCode = null;
 
        //Instantiating Utilities function class
        Utility_Functions utilityFunction = new Utility_Functions();

        internal IWebDriver driver = null;

        string driverName = "", driverPath, appURL;

        Utility_Functions uf = new Utility_Functions();                                    // Instantiate object for Utility Functons

        Configuration cf = new Configuration();                                           // Instantiate object for Configuration
                
        Object_Repository_Class or = new Object_Repository_Class();

        IE_AdminSetupTearDown st = new IE_AdminSetupTearDown();                         // Instantiate object for IE Setup Teardown

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


        #region Reusable Function for this module

        //This function will move the control to Category Management Page
        public void RedirectingToCategoryManagement()
        {
            log.Info("Inside RedirectingToCategoryManagement " + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());


            iWait.Until(ExpectedConditions.ElementIsVisible(By.Id("rptMenu_aMenuItem_0")));

            //clicking on Admin dropdown
            driver.FindElement(By.Id("rptMenu_aMenuItem_0")).Click();

            Thread.Sleep(3000);

            iWait.Until(ExpectedConditions.ElementToBeClickable(By.LinkText("Category Management")));

            //Clicking on Category Management
            driver.FindElement(By.LinkText("Category Management")).Click();

            iWait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("div#viewContent > div > div > div.titleBox > h2")));
            isCategoryPage = driver.FindElement(By.CssSelector("div#viewContent > div > div > div.titleBox > h2")).Displayed;

            //Checking whether the user is on Category page
            Assert.AreEqual(true, isCategoryPage);
        }


        //This function will return the position where category is going to create.
        public int CalculatingPositionForCategory(String categoryPosition, SelectElement positionDropdown)
        {
            log.Info("Inside CalculatingPositionForCategory " + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());


            if (categoryPosition.Equals("first"))
                return 0;
            else if (categoryPosition.Equals("last"))
                return (positionDropdown.Options.Count) - 1;
            else if (categoryPosition.Equals("middle"))
            {
                Random random = new Random();
                return random.Next(1, positionDropdown.Options.Count - 2);
            }
            else
                return -1;
        }

        //This function creates a Category by Calculating position
        public void CreatingCategory(String categoryPosition)
        {
            log.Info("Inside CreatingCategory Function " + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

            //verifying the default active tab of Category management page

            Assert.AreEqual(String.Empty, driver.FindElement(By.Id("createCategory")).GetAttribute("class"));
            Assert.AreEqual("active", driver.FindElement(By.Id("CategoryList")).GetAttribute("class"));

            iWait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.Id("btnOk")));

            //Clicking on Create Tab
            driver.FindElement(By.Id("CreateCategoryTab")).Click();

            #region Creating Category

            // Creating unique category GUID
            categoryNameWithGuid = categoryNameWithGuid + uf.getGuid();

            iWait.Until(ExpectedConditions.ElementIsVisible(By.Id("ContentPlaceHolder1_txtCategoryName")));
            driver.FindElement(By.Id("ContentPlaceHolder1_txtCategoryName")).Clear();

            driver.FindElement(By.Id("ContentPlaceHolder1_txtCategoryName")).SendKeys(categoryNameWithGuid);

            positionDropdown = new SelectElement(driver.FindElement(By.Id("ContentPlaceHolder1_ddlPosition1")));

            int position = CalculatingPositionForCategory(categoryPosition, positionDropdown);

            positionDropdown = new SelectElement(driver.FindElement(By.Id("ContentPlaceHolder1_ddlPosition1")));

            if (!categoryPosition.Equals("last"))
                positionDropdown.SelectByIndex(position);
            else if (categoryPosition.Equals("first") || categoryPosition.Equals("middle"))
                positionDropdown.SelectByIndex(position);

            driver.FindElement(By.Id("ContentPlaceHolder1_btnSaveChanges")).Click();

            #endregion

            //Verfiying User friendly message
            iWait.Until(ExpectedConditions.ElementIsVisible(By.Id("btnOk")));
            Assert.AreEqual("Category added successfully.", driver.FindElement(By.Id("Sucess_Message")).Text.Trim());  //Verfying that correct banner message is displayed.
            driver.FindElement(By.Id("btnOk")).Click();

            iWait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.Id("btnOk")));


        }

        // This function apply Assert on created Category
        public void AssertAfterCreatingCategory()
        {
            log.Info("Inside AssertAfterCreatingCategory Function " + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

            //After Creating Category verifying the active tab i.e Manage or Create
            Assert.AreEqual("active", driver.FindElement(By.Id("CategoryList")).GetAttribute("class"));

            String categoryName = categoryNameWithGuid;


            //Using Nsoup here to parse the html table
            Document doc = NSoup.NSoupClient.Parse(driver.PageSource);
            Elements rowListNsoup = doc.GetElementById("ContentPlaceHolder1_gvCategoryList").GetElementsByTag("tr");

            // Retreving all the rows of Manage Table 
            IList<IWebElement> rowListSelenium = (IList<IWebElement>)driver.FindElement(By.Id("ContentPlaceHolder1_gvCategoryList")).FindElements(By.TagName("tr"));

            #region Applying_Assert_On_Manage_Page

            flag = false;

            int rowcounter = 0;
            foreach (Element currentRow in rowListNsoup)
            {
                Attributes attr = currentRow.Attributes;

                //Row that have class="GridRowStyle" or class="AltGridStyle"

                if (attr["class"].Equals("GridRowStyle") || attr["class"].Equals("AltGridStyle"))
                {
                    log.Info("Value of Row count " + rowcounter + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

                    columData = currentRow.GetElementsByTag("td")[2].GetElementById("ContentPlaceHolder1_gvCategoryList_lblName_" + rowcounter).OwnText().Trim();


                    if (columData.Equals(categoryName))
                    {
                        flag = true;

                        //write assert to check checkbox is displayed
                        Assert.AreEqual("checkbox", currentRow.GetElementsByTag("td")[0].GetElementById("ContentPlaceHolder1_gvCategoryList_chkSelect_" + rowcounter).Attributes["type"]);

                        // Write assert to check position of newly created category

                        SelectElement positionDropsownOnManage = new SelectElement(rowListSelenium[rowcounter + 1].FindElements(By.TagName("td"))[1].FindElement(By.Id("ContentPlaceHolder1_gvCategoryList_ddlPosition_" + rowcounter)));

                        Assert.AreEqual(1, Convert.ToInt32(positionDropsownOnManage.SelectedOption.Text.Trim()));

                        //Category Type Assertion

                        Assert.AreEqual("Channel", currentRow.GetElementsByTag("td")[3].GetElementById("ContentPlaceHolder1_gvCategoryList_lblType_" + rowcounter).OwnText().Trim());

                        //Write assert to check  default status and should be active

                        Assert.AreEqual("Active", currentRow.GetElementsByTag("td")[4].GetElementById("ContentPlaceHolder1_gvCategoryList_lblStatus_" + rowcounter).OwnText().Trim());

                        //Write assert to check edit and delete button

                        Assert.AreEqual(true, currentRow.GetElementsByTag("td")[5].GetElementById("ContentPlaceHolder1_gvCategoryList_imgEdit_" + rowcounter).GetElementsByTag("img")[0].Attributes["src"].Contains("Edit.png"));

                        Assert.AreEqual(true, currentRow.GetElementsByTag("td")[5].GetElementById("ContentPlaceHolder1_gvCategoryList_ImgDelete_" + rowcounter).Attributes["src"].Contains("Delete.png"));

                        //Write assert to check Category name is present of not
                        log.Info("Yes Category is present on Manage" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

                        break;
                    }
                    rowcounter++;
                }
            }

            #endregion

            Assert.AreEqual(true, flag);   //IF THIS FAILS MEANS cATEGORY IS NOT PRESENT ON mANAGE paGE 



        }

        #endregion


        [Test]
        public void TVAdmin_001_createCategory()
        {
            try
            {
                log.Info("TVAdmin_001_createCategory test started" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

                //Moving to Category Management Page
                RedirectingToCategoryManagement();

                //Getting the no of Category already present on Manage Screen
                int rowCount = (driver.FindElement(By.Id(or.readingXMLFile("CategoryManagement", "CategoryListTable", "TVAdminPortalOR.xml"))).FindElements(By.TagName("tr"))).Count;

                //Creating Category based on Categories already present on Manage Page.
                if (rowCount == 0)
                {
                    CreatingCategory("first");
                    AssertAfterCreatingCategory();
                }
                else if (rowCount <= 2)
                {
                    // This will create category at first position
                    CreatingCategory("first");
                    AssertAfterCreatingCategory();
                    categoryNameWithGuid = "Cat";
                    // This will create category at last position
                    CreatingCategory("last");
                }
                else if (rowCount > 2)
                {
                    CreatingCategory("first");
                    AssertAfterCreatingCategory();

                    categoryNameWithGuid = "Cat";
                    CreatingCategory("last");

                    categoryNameWithGuid = "Cat";
                    // This will create category at middle position
                    CreatingCategory("middle");

                }
                log.Info("TVAdmin_001_createCategory test has completed" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());
            }
            catch (Exception e)
            {
                log.Error(e.Message + "\n" + e.StackTrace + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());
                Assert.AreEqual(true, false);
            }

        }

        [Test]
        public void TVAdmin_002_dupCategoryCreation()
        {

            try
            {
                log.Info("TVAdmin_002_duplicateCategoryCreation test started" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

                RedirectingToCategoryManagement();

                // Creating Catagory on first position
                CreatingCategory("first");

                iWait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.Id(or.readingXMLFile("CategoryManagement", "SuccessOkButton", "TVAdminPortalOR.xml"))));

                //Clicking on Create Tab
                driver.FindElement(By.Id(or.readingXMLFile("CategoryManagement", "CreateCategoryTab", "TVAdminPortalOR.xml"))).Click();

                //getting last created Category Name
                String categoryName = categoryNameWithGuid;

                #region Creating Category

                iWait.Until(ExpectedConditions.ElementIsVisible(By.Id(or.readingXMLFile("CategoryManagement", "CategoryName", "TVAdminPortalOR.xml"))));

                driver.FindElement(By.Id(or.readingXMLFile("CategoryManagement", "CategoryName", "TVAdminPortalOR.xml"))).SendKeys(categoryName);

                positionDropdown = new SelectElement(driver.FindElement(By.Id(or.readingXMLFile("CategoryManagement", "CategoryPosition", "TVAdminPortalOR.xml"))));

                positionDropdown.SelectByText("1");

                driver.FindElement(By.Id(or.readingXMLFile("CategoryManagement", "SaveButton", "TVAdminPortalOR.xml"))).Click();

                #endregion

                //Verfiying User friendly message
                log.Info("Verfiying User friendly message" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

                iWait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector(or.readingXMLFile("CategoryManagement", "InfoOkButton", "TVAdminPortalOR.xml"))));
                Assert.AreEqual("Category Already Exists", driver.FindElement(By.Id(or.readingXMLFile("CategoryManagement", "InfoMessage", "TVAdminPortalOR.xml"))).Text.Trim());    //Verifying infomration message for Duplicate Category.
                driver.FindElement(By.CssSelector(or.readingXMLFile("CategoryManagement", "InfoOkButton", "TVAdminPortalOR.xml"))).Click();

                iWait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector(or.readingXMLFile("CategoryManagement", "InfoOkButton", "TVAdminPortalOR.xml"))));

                log.Info("TVAdmin_002_duplicateCategoryCreation test has completed" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());
            }
            catch (Exception e)
            {
                log.Error(e.Message + "\n" + e.StackTrace + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());
                Assert.AreEqual(true, false);
            }

        }

        [Test]
        public void TVAdmin_003_mandatoryFieldValidation()
        {
            try
            {
                log.Info("TVAdmin_003_mandatoryFieldValidation test started" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

                iWait.Until(ExpectedConditions.ElementIsVisible(By.Id(or.readingXMLFile("CategoryManagement", "AdminMenu", "TVAdminPortalOR.xml"))));

                //clicking on Admin dropdown
                driver.FindElement(By.Id(or.readingXMLFile("CategoryManagement", "AdminMenu", "TVAdminPortalOR.xml"))).Click();

                iWait.Until(ExpectedConditions.ElementToBeClickable(By.LinkText(or.readingXMLFile("CategoryManagement", "CategoryManagementlink", "TVAdminPortalOR.xml"))));
                //Clicking on Category Management
                driver.FindElement(By.LinkText(or.readingXMLFile("CategoryManagement", "CategoryManagementlink", "TVAdminPortalOR.xml"))).Click();

                iWait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector(or.readingXMLFile("CategoryManagement", "CategoryPage", "TVAdminPortalOR.xml"))));
                isCategoryPage = driver.FindElement(By.CssSelector(or.readingXMLFile("CategoryManagement", "CategoryPage", "TVAdminPortalOR.xml"))).Displayed;
                Assert.AreEqual(true, isCategoryPage);    //Checking whether the user is on Category page

                //verifying the default active tab of Category management page
                Assert.AreEqual(String.Empty, driver.FindElement(By.Id(or.readingXMLFile("CategoryManagement", "CreateCategory", "TVAdminPortalOR.xml"))).GetAttribute("class"));
                Assert.AreEqual("active", driver.FindElement(By.Id(or.readingXMLFile("CategoryManagement", "CategoryListTab", "TVAdminPortalOR.xml"))).GetAttribute("class"));

                iWait.Until(ExpectedConditions.ElementIsVisible(By.Id(or.readingXMLFile("CategoryManagement", "CreateCategoryTab", "TVAdminPortalOR.xml"))));
                driver.FindElement(By.Id(or.readingXMLFile("CategoryManagement", "CreateCategoryTab", "TVAdminPortalOR.xml"))).Click();    //Clicking on Create Tab

                driver.FindElement(By.Id(or.readingXMLFile("CategoryManagement", "SaveButton", "TVAdminPortalOR.xml"))).Click(); //Clicking on Save button

                iWait.Until(ExpectedConditions.ElementIsVisible(By.Id(or.readingXMLFile("CategoryManagement", "CategoryInline", "TVAdminPortalOR.xml"))));
                Assert.AreEqual("Please Enter the Category Name", driver.FindElement(By.Id(or.readingXMLFile("CategoryManagement", "CategoryInline", "TVAdminPortalOR.xml"))).Text);   //Verifying that Mendatroy message is dispayed correclty

                log.Info("TVAdmin_003_mandatoryFieldValidation test has completed" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

            }
            catch (Exception e)
            {
                log.Error(e.Message + "\n" + e.StackTrace + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

                Assert.AreEqual(true, false);
            }
        }

        [Test]
        public void TVAdmin_004_inactivateActivateFunc()
        {

            try
            {
                log.Info("TVAdmin_004_inactivateActivateFunc test started" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

                //Go to Category Management Page
                RedirectingToCategoryManagement();

                // Creating Catagory at first position
                CreatingCategory("first");

                //getting last created Category Name
                String categoryName = categoryNameWithGuid;

                log.Info("Verify inactivate function started" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

                #region Inactivating Category

                //Nsoup is used here to parse the HTML table
                Document doc = NSoup.NSoupClient.Parse(driver.PageSource);

                Elements rowListNsoup = doc.GetElementById(or.readingXMLFile("CategoryManagement", "CategoryListTable", "TVAdminPortalOR.xml")).GetElementsByTag("tr");

                // Retreving all the rows of Manage Table 
                IList<IWebElement> rowListSelenium = (IList<IWebElement>)driver.FindElement(By.Id(or.readingXMLFile("CategoryManagement", "CategoryListTable", "TVAdminPortalOR.xml"))).FindElements(By.TagName("tr"));

                #region To Select Created Category and clicking on Checkbox

                int rowCounter = 0;

                foreach (Element tempCurrentRow in rowListNsoup)
                {
                    Attributes attr = tempCurrentRow.Attributes;

                    //Row that have class="GridRowStyle" or class="AltGridStyle"
                    if (attr["class"].Equals("GridRowStyle") || attr["class"].Equals("AltGridStyle"))
                    {

                        log.Info("Value of row count " + rowCounter + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

                        columData = tempCurrentRow.GetElementsByTag("td")[2].GetElementById(or.readingXMLFile("CategoryManagement", "CategoryListName", "TVAdminPortalOR.xml") + rowCounter).OwnText().Trim();

                        if (columData.Equals(categoryName))
                        {
                            rowListSelenium[rowCounter + 1].FindElements(By.TagName("td"))[0].FindElement(By.TagName("input")).Click();
                            break;
                        }
                        rowCounter++;
                    }

                }
                #endregion

                //Clicking on InActivate button
                driver.FindElement(By.Id(or.readingXMLFile("CategoryManagement", "InactiveButton", "TVAdminPortalOR.xml"))).Click();

                //Clicking on Ok button of Success Message Message
                iWait.Until(ExpectedConditions.ElementIsVisible(By.Id(or.readingXMLFile("CategoryManagement", "SucessMessage", "TVAdminPortalOR.xml"))));

                Thread.Sleep(3000);

                Assert.AreEqual("Record(s) Inactivated successfully.", driver.FindElement(By.Id(or.readingXMLFile("CategoryManagement", "SucessMessage", "TVAdminPortalOR.xml"))).Text.Trim());

                driver.FindElement(By.Id(or.readingXMLFile("CategoryManagement", "SuccessOkButton", "TVAdminPortalOR.xml"))).Click();

                Thread.Sleep(5000);


                #region To Track Created Category and Aplying Assert to check Whether Inactivated or not


                doc = NSoup.NSoupClient.Parse(driver.PageSource);

                rowListNsoup = doc.GetElementById(or.readingXMLFile("CategoryManagement", "CategoryListTable", "TVAdminPortalOR.xml")).GetElementsByTag("tr");

                // Retreving all the rows of Manage Table 
                rowListSelenium = (IList<IWebElement>)driver.FindElement(By.Id(or.readingXMLFile("CategoryManagement", "CategoryListTable", "TVAdminPortalOR.xml"))).FindElements(By.TagName("tr"));

                rowCounter = 0;
                foreach (Element tempCurrentRow in rowListNsoup)
                {
                    Attributes attr = tempCurrentRow.Attributes;

                    //Row that have class="GridRowStyle" or class="AltGridStyle"
                    if (attr["class"].Equals("GridRowStyle") || attr["class"].Equals("AltGridStyle"))
                    {

                        log.Info("Value of row count " + rowCounter + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

                        columData = tempCurrentRow.GetElementsByTag("td")[2].GetElementById(or.readingXMLFile("CategoryManagement", "CategoryListName", "TVAdminPortalOR.xml") + rowCounter).OwnText().Trim();

                        if (columData.Equals(categoryName))
                        {

                            //Assert to check status = 'Activate / Inactivate' of Invactivated Category and this should be InActive
                            Assert.AreEqual("Inactive", tempCurrentRow.GetElementsByTag("td")[4].GetElementById(or.readingXMLFile("CategoryManagement", "Status", "TVAdminPortalOR.xml") + rowCounter).OwnText().Trim());

                            //Verfying that Edit button is not present for Inactivated Category
                            Assert.AreEqual(0, rowListSelenium[rowCounter + 1].FindElements(By.TagName("td"))[5].FindElements(By.TagName("span")).Count);
                            break;
                        }
                        rowCounter++;
                    }
                }
                #endregion

                #endregion

                log.Info("Verify Activate function started" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

                #region Activating Category

                log.Info("\nActivating Category" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

                log.Info("Category Name :: " + categoryName + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

                doc = NSoup.NSoupClient.Parse(driver.PageSource);
                rowListNsoup = doc.GetElementById(or.readingXMLFile("CategoryManagement", "CategoryListTable", "TVAdminPortalOR.xml")).GetElementsByTag("tr");

                //// Retreving all the rows of Manage Table 
                rowListSelenium = (IList<IWebElement>)driver.FindElement(By.Id(or.readingXMLFile("CategoryManagement", "CategoryListTable", "TVAdminPortalOR.xml"))).FindElements(By.TagName("tr"));

                #region

                rowCounter = 0;
                foreach (Element tempCurrentRow in rowListNsoup)
                {
                    Attributes attr = tempCurrentRow.Attributes;

                    //Row that have class="GridRowStyle" or class="AltGridStyle"
                    if (attr["class"].Equals("GridRowStyle") || attr["class"].Equals("AltGridStyle"))
                    {

                        log.Info("Row Count  :: " + rowCounter + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

                        columData = tempCurrentRow.GetElementsByTag("td")[2].GetElementById(or.readingXMLFile("CategoryManagement", "CategoryListName", "TVAdminPortalOR.xml") + rowCounter).OwnText().Trim();

                        if (columData.Equals(categoryName))
                        {

                            rowListSelenium[rowCounter + 1].FindElements(By.TagName("td"))[0].FindElement(By.TagName("input")).Click();

                            driver.FindElement(By.Id(or.readingXMLFile("CategoryManagement", "ActiveButton", "TVAdminPortalOR.xml"))).Click();   //Clicking on Activate Button
                            break;
                        }
                        rowCounter++;
                    }
                }
                #endregion

                //Clicking on Ok button of Success Message Message
                iWait.Until(ExpectedConditions.ElementIsVisible(By.Id(or.readingXMLFile("CategoryManagement", "SuccessOkButton", "TVAdminPortalOR.xml"))));
                Assert.AreEqual("Record(s) activated successfully.", driver.FindElement(By.Id(or.readingXMLFile("CategoryManagement", "SucessMessage", "TVAdminPortalOR.xml"))).Text.Trim());  //Verifying that correct Success message is displayed or not
                driver.FindElement(By.Id(or.readingXMLFile("CategoryManagement", "SuccessOkButton", "TVAdminPortalOR.xml"))).Click();


                #region To Track Created Category and Aplying Assert to check Whether Activated or not

                doc = NSoup.NSoupClient.Parse(driver.PageSource);
                rowListNsoup = doc.GetElementById("ContentPlaceHolder1_gvCategoryList").GetElementsByTag("tr");

                //// Retreving all the rows of Manage Table 
                rowListSelenium = (IList<IWebElement>)driver.FindElement(By.Id(or.readingXMLFile("CategoryManagement", "CategoryListTable", "TVAdminPortalOR.xml"))).FindElements(By.TagName("tr"));

                rowCounter = 0;
                foreach (Element tempCurrentRow in rowListNsoup)
                {
                    Attributes attr = tempCurrentRow.Attributes;

                    //Row that have class="GridRowStyle" or class="AltGridStyle"
                    if (attr["class"].Equals("GridRowStyle") || attr["class"].Equals("AltGridStyle"))
                    {

                        log.Info("Row Count :: " + rowCounter + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

                        columData = tempCurrentRow.GetElementsByTag("td")[2].GetElementById(or.readingXMLFile("CategoryManagement", "CategoryListName", "TVAdminPortalOR.xml") + rowCounter).OwnText().Trim();

                        if (columData.Equals(categoryName))
                        {

                            //Assert to check status = 'Activate / Inactivate' of Invactivated Category and this should be Active
                            Assert.AreEqual("Active", tempCurrentRow.GetElementsByTag("td")[4].GetElementById(or.readingXMLFile("CategoryManagement", "Status", "TVAdminPortalOR.xml") + rowCounter).OwnText().Trim());

                            //Verfying that Edit button is not present for Inactivated Category
                            Assert.AreNotEqual(0, rowListSelenium[rowCounter + 1].FindElements(By.TagName("td"))[5].FindElements(By.TagName("span")).Count);

                            break;
                        }
                    }
                }
                #endregion

                #endregion

                iWait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector(or.readingXMLFile("CategoryManagement", "InfoOkButton", "TVAdminPortalOR.xml"))));

                log.Info("TVAdmin_004_inactivateActivateFunc test has completed" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

            }
            catch (Exception e)
            {
                log.Error(e.Message + "\n" + e.StackTrace + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

                Assert.AreEqual(true, false);
            }

        }


        [Test]
        public void TVAdmin_005_deletingCategory()
        {
            try
            {
                log.Info("TVAdmin_005_deletingCategory test started" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

                //Go to Category Management Page
                RedirectingToCategoryManagement();

                CreatingCategory("first");

                //getting last created Category Name
                String categoryName = categoryNameWithGuid;

                #region Deleting_Category

                //Retreving all the rows of Manage Table 
                rowList = (IList<IWebElement>)driver.FindElement(By.Id(or.readingXMLFile("CategoryManagement", "CategoryListTable", "TVAdminPortalOR.xml"))).FindElements(By.TagName("tr"));

                #region To Select Created Category

                int i = 0;
                foreach (IWebElement tempCurrentRow in rowList)
                {
                    //Row that have class="GridRowStyle" or class="AltGridStyle"
                    if (tempCurrentRow.GetAttribute("class").Equals("GridRowStyle") || tempCurrentRow.GetAttribute("class").Equals("AltGridStyle"))
                    {
                        columData = tempCurrentRow.FindElements(By.TagName("td"))[2].FindElement(By.TagName("span")).Text.Trim();

                        if (columData.Equals(categoryName))
                        {
                            Thread.Sleep(1000);
                            tempCurrentRow.FindElements(By.TagName("td"))[5].FindElement(By.Id(or.readingXMLFile("CategoryManagement", "DeleteButton", "TVAdminPortalOR.xml") + i)).Click();   // clicking on Delete Icon

                            iWait.Until(ExpectedConditions.ElementIsVisible(By.Id(or.readingXMLFile("CategoryManagement", "WarningNoButton", "TVAdminPortalOR.xml"))));
                            Assert.AreEqual("Are you sure, you wish to delete?", driver.FindElement(By.Id(or.readingXMLFile("CategoryManagement", "WarningMessage", "TVAdminPortalOR.xml"))).Text.Trim());  //Verfying that correct Warning MEssage is displayed
                            driver.FindElement(By.Id(or.readingXMLFile("CategoryManagement", "WarningNoButton", "TVAdminPortalOR.xml"))).Click();   //Clicking on No button of Warning Message

                            //Verifying that categary present or not 
                            pageSourceCode = driver.PageSource;

                            //if this fails then category is not present and NO button functionality is not proper.
                            Assert.AreEqual(true, pageSourceCode.Contains(categoryName));

                            Thread.Sleep(1000);
                            tempCurrentRow.FindElements(By.TagName("td"))[5].FindElement(By.Id(or.readingXMLFile("CategoryManagement", "DeleteButton", "TVAdminPortalOR.xml") + i)).Click();   // clicking on Delete Icon

                            iWait.Until(ExpectedConditions.ElementIsVisible(By.Id(or.readingXMLFile("CategoryManagement", "WarningYesButton", "TVAdminPortalOR.xml"))));
                            Assert.AreEqual("Are you sure, you wish to delete?", driver.FindElement(By.Id(or.readingXMLFile("CategoryManagement", "WarningMessage", "TVAdminPortalOR.xml"))).Text.Trim());  //Verfying that correct Warning MEssage is displayed
                            driver.FindElement(By.Id(or.readingXMLFile("CategoryManagement", "WarningYesButton", "TVAdminPortalOR.xml"))).Click();   //Clicking on Yes button of Warning Message

                            iWait.Until(ExpectedConditions.ElementIsVisible(By.Id(or.readingXMLFile("CategoryManagement", "SuccessOkButton", "TVAdminPortalOR.xml"))));
                            Assert.AreEqual("Record Deleted successfully", driver.FindElement(By.Id(or.readingXMLFile("CategoryManagement", "SucessMessage", "TVAdminPortalOR.xml"))).Text.Trim());   //Verifying that Successcull message is displayed after delete
                            driver.FindElement(By.Id(or.readingXMLFile("CategoryManagement", "SuccessOkButton", "TVAdminPortalOR.xml"))).Click();     //Clicking on Ok button of Success Message

                            uf.isJavaScriptActive(driver);

                            uf.isJqueryActive(driver);

                            Assert.AreEqual(false, driver.PageSource.Contains(categoryName));   //if this fails then Delete functinality is not working.
                            break;
                        }
                    }
                }

                #endregion

                #endregion

                log.Info("TVAdmin_005_deletingCategory test has completed" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

            }
            catch (Exception e)
            {
                log.Error(e.Message + "\n" + e.StackTrace + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());
                Assert.AreEqual(true, false);
            }


        }


        [Test]
        public void TVAdmin_006_cancelButtonFunc()
        {
            try
            {
                log.Info("TVAdmin_006_cancelButtonFunc test started" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

                String categoryName = "Automation Unique Category";


                RedirectingToCategoryManagement();

                iWait.Until(ExpectedConditions.ElementIsVisible(By.Id(or.readingXMLFile("CategoryManagement", "CreateCategoryTab", "TVAdminPortalOR.xml"))));

                //Clicking on Create Tab
                driver.FindElement(By.Id(or.readingXMLFile("CategoryManagement", "CreateCategoryTab", "TVAdminPortalOR.xml"))).Click();

                #region Filling_Details

                iWait.Until(ExpectedConditions.ElementIsVisible(By.Id(or.readingXMLFile("CategoryManagement", "CategoryName", "TVAdminPortalOR.xml"))));
                driver.FindElement(By.Id(or.readingXMLFile("CategoryManagement", "CategoryName", "TVAdminPortalOR.xml"))).SendKeys(categoryName);

                positionDropdown = new SelectElement(driver.FindElement(By.Id(or.readingXMLFile("CategoryManagement", "CategoryPosition", "TVAdminPortalOR.xml"))));
                positionDropdown.SelectByText("1");

                driver.FindElement(By.Id(or.readingXMLFile("CategoryManagement", "CancelButton", "TVAdminPortalOR.xml"))).Click();

                #endregion

                #region Verifying_the_Active_Tab

                //verifying the default active tab of Category management page
                iWait.Until(ExpectedConditions.ElementIsVisible(By.Id(or.readingXMLFile("CategoryManagement", "CreateCategory", "TVAdminPortalOR.xml"))));
                Assert.AreEqual(String.Empty, driver.FindElement(By.Id(or.readingXMLFile("CategoryManagement", "CreateCategory", "TVAdminPortalOR.xml"))).GetAttribute("class"));
                Assert.AreEqual("active", driver.FindElement(By.Id(or.readingXMLFile("CategoryManagement", "CategoryListTab", "TVAdminPortalOR.xml"))).GetAttribute("class"));

                #endregion

                log.Info("Verifying_Record_is_Created_or_not" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

                #region Verifying_Record_is_Created_or_not

                //Retreving all the rows of Manage Table 
                rowList = (IList<IWebElement>)driver.FindElement(By.Id(or.readingXMLFile("CategoryManagement", "CategoryListTable", "TVAdminPortalOR.xml"))).FindElements(By.TagName("tr"));

                String pageSource = driver.PageSource.ToString();

                //If it fails that means Application is creating Category name by clicking on Cancel button
                Assert.AreEqual(false, pageSource.Contains(categoryName));

                #endregion

                log.Info("TVAdmin_006_cancelButtonFunc test has completed" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());
            }
            catch (Exception e)
            {
                log.Error(e.Message + "\n" + e.StackTrace + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

                Assert.AreEqual(true, false);
            }
        }


        [Test]
        public void TVAdmin_007_editButtonFunc()
        {
            try
            {
                log.Info("TVAdmin_007_editButtonFunc test started" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

                //Current Information Before Edit
                Boolean isCheckBox = true;
                String currentCategoryName = null;
                String currentStatus = null;
                String currentPosition = null;
                String currentCategory = null;

                // Set the variable for edit button presence 1 = Present / Active

                int isEditbutton = 1;

                Boolean isDeleteButton = true;

                String cancelbuttonfunctionality = "Checking Cancel button in Edit Mode";

                String editedPosition = "2";

                RedirectingToCategoryManagement();

                //            /* 
                //             * Here We are considering that there is at least one record present on Manage Page.
                //             * So we will edit always first record.
                //             */

                log.Info("Scenario 1 : Changing Category Name and Position and clicking on Save button started" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

                #region Scenario 1 : Changing Category Name and Position and clicking on Save button

                log.Info("Inside Scenario 1" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());


                #region This will store the present details of first category that is to be edited

                rowList = (IList<IWebElement>)driver.FindElement(By.Id(or.readingXMLFile("CategoryManagement", "CategoryListTable", "TVAdminPortalOR.xml"))).FindElements(By.TagName("tr"));

                isCheckBox = rowList[1].FindElements(By.TagName("td"))[0].FindElement(By.TagName("input")).Displayed;

                SelectElement positionDropsown = new SelectElement(rowList[1].FindElements(By.TagName("td"))[1].FindElement(By.Name("ctl00$ContentPlaceHolder1$gvCategoryList$ctl02$ddlPosition")));
                currentPosition = positionDropsown.SelectedOption.Text.Trim();

                currentCategoryName = rowList[1].FindElements(By.TagName("td"))[2].FindElement(By.TagName("span")).Text.Trim();

                currentCategory = rowList[1].FindElements(By.TagName("td"))[3].FindElement(By.TagName("span")).Text;

                currentStatus = rowList[1].FindElements(By.TagName("td"))[4].FindElement(By.TagName("span")).Text;

                isEditbutton = rowList[1].FindElements(By.TagName("td"))[5].FindElements(By.CssSelector("span > img")).Count;

                isDeleteButton = rowList[1].FindElements(By.TagName("td"))[5].FindElement(By.Id(or.readingXMLFile("CategoryManagement", "DeleteButton", "TVAdminPortalOR.xml") + 0)).GetAttribute("src").Contains("Delete.png");

                rowList[1].FindElements(By.TagName("td"))[5].FindElement(By.CssSelector("span > img")).Click();  //Clicking on Edit Button

                #endregion


                #region Verifying prepopulated details on edit Page

                iWait.Until(ExpectedConditions.ElementIsVisible(By.Id(or.readingXMLFile("CategoryManagement", "CreateCategoryTab", "TVAdminPortalOR.xml"))));

                Assert.AreEqual("Edit Category", driver.FindElement(By.Id(or.readingXMLFile("CategoryManagement", "CreateCategoryTab", "TVAdminPortalOR.xml"))).Text);  //Checking whether the text of Create is changed to Edit Category

                Assert.AreEqual(true, driver.FindElement(By.Id(or.readingXMLFile("CategoryManagement", "CategoryName", "TVAdminPortalOR.xml"))).Enabled);  //Verifying that Category Textfield is enable.

                Assert.AreNotEqual(String.Empty, driver.FindElement(By.Id(or.readingXMLFile("CategoryManagement", "CategoryName", "TVAdminPortalOR.xml"))).GetAttribute("value")); // verifying that Category name has some pre populated data

                Assert.AreEqual(currentCategoryName, driver.FindElement(By.Id(or.readingXMLFile("CategoryManagement", "CategoryName", "TVAdminPortalOR.xml"))).GetAttribute("value")); //verfiying that populated Categry was old one 

                Assert.AreEqual(currentPosition, new SelectElement(driver.FindElement(By.Id(or.readingXMLFile("CategoryManagement", "CategoryPosition", "TVAdminPortalOR.xml")))).SelectedOption.Text.Trim()); //verifying the prepopulated position is old one

                #endregion


                #region Editting the Category and Verfiying the Banner Message

                categoryNameWithGuid = categoryNameWithGuid + uf.getGuid();

                driver.FindElement(By.Id(or.readingXMLFile("CategoryManagement", "CategoryName", "TVAdminPortalOR.xml"))).Clear();
                driver.FindElement(By.Id(or.readingXMLFile("CategoryManagement", "CategoryName", "TVAdminPortalOR.xml"))).SendKeys(categoryNameWithGuid);
                positionDropdown = new SelectElement(driver.FindElement(By.Id(or.readingXMLFile("CategoryManagement", "CategoryPosition", "TVAdminPortalOR.xml"))));
                positionDropdown.SelectByText(editedPosition);
                driver.FindElement(By.Id(or.readingXMLFile("CategoryManagement", "SaveButton", "TVAdminPortalOR.xml"))).Click();


                //Verfiying User friendly message
                iWait.Until(ExpectedConditions.ElementIsVisible(By.Id(or.readingXMLFile("CategoryManagement", "SuccessOkButton", "TVAdminPortalOR.xml"))));
                Assert.AreEqual("Record Updated Successfully", driver.FindElement(By.Id(or.readingXMLFile("CategoryManagement", "SucessMessage", "TVAdminPortalOR.xml"))).Text.Trim());
                driver.FindElement(By.Id(or.readingXMLFile("CategoryManagement", "SuccessOkButton", "TVAdminPortalOR.xml"))).Click();



                #endregion


                #region Searching the above category that is edited on Manage Page and verifying the update is applied or not

                //Retreving all the rows of Manage Table 
                rowList = (IList<IWebElement>)driver.FindElement(By.Id(or.readingXMLFile("CategoryManagement", "CategoryListTable", "TVAdminPortalOR.xml"))).FindElements(By.TagName("tr"));

                int i = 0;
                flag = false;

                foreach (IWebElement currentRow in rowList)
                {
                    // Row that have class="GridRowStyle" or class="AltGridStyle"
                    if (currentRow.GetAttribute("class").Equals("GridRowStyle") || currentRow.GetAttribute("class").Equals("AltGridStyle"))
                    {
                        log.Info("Row Count :: " + i + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

                        columData = currentRow.FindElements(By.TagName("td"))[2].FindElement(By.TagName("span")).Text.Trim();

                        if (columData.Equals(categoryNameWithGuid))
                        {
                            flag = true;

                            //write assert to check checkbox is displayed
                            Assert.AreEqual(isCheckBox, currentRow.FindElements(By.TagName("td"))[0].FindElement(By.TagName("input")).Displayed);
                            Assert.AreEqual("checkbox", currentRow.FindElements(By.TagName("td"))[0].FindElement(By.TagName("input")).GetAttribute("type"));

                            //Write assert to check position
                            positionDropsown = new SelectElement(currentRow.FindElements(By.TagName("td"))[1].FindElement(By.Id(or.readingXMLFile("CategoryManagement", "CategoryListPosition", "TVAdminPortalOR.xml") + i)));
                            Assert.AreEqual(editedPosition, positionDropsown.SelectedOption.Text.Trim());

                            //write assert to check  default status and should be active
                            Assert.AreEqual(currentStatus, currentRow.FindElements(By.TagName("td"))[4].FindElement(By.TagName("span")).Text);

                            //write assert to check edit and delete button
                            Assert.AreEqual(isEditbutton, currentRow.FindElements(By.TagName("td"))[5].FindElements(By.CssSelector("span > img")).Count);
                            Assert.AreEqual(isDeleteButton, currentRow.FindElements(By.TagName("td"))[5].FindElement(By.Id(or.readingXMLFile("CategoryManagement", "DeleteButton", "TVAdminPortalOR.xml") + i)).GetAttribute("src").Contains("Delete.png"));


                            //Category Type Assertion
                            Assert.AreEqual(currentCategory, currentRow.FindElements(By.TagName("td"))[3].FindElement(By.TagName("span")).Text.Trim());

                            //write assert to check Category name is present of not
                            log.Info("Yes Category is present on Manage" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

                            break;
                        }
                        i++;
                    }
                }
                Assert.AreEqual(true, flag);   //If It fails then Edited Category name is not displyed on Manage Page

                #endregion



                #endregion

                log.Info("Scenario 2 : Changing Category Name and Position and clicking on Cancel button started" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

                #region Scenario 2 : Changing Category Name and Position and clicking on Cancel button

                log.Info("Inside Scenarion 2" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

                #region Region to Store the Current Information of Category that is to be edited

                rowList = (IList<IWebElement>)driver.FindElement(By.Id(or.readingXMLFile("CategoryManagement", "CategoryListTable", "TVAdminPortalOR.xml"))).FindElements(By.TagName("tr"));

                isCheckBox = rowList[1].FindElements(By.TagName("td"))[0].FindElement(By.TagName("input")).Displayed;

                positionDropsown = new SelectElement(rowList[1].FindElements(By.TagName("td"))[1].FindElement(By.Name("ctl00$ContentPlaceHolder1$gvCategoryList$ctl02$ddlPosition")));

                currentPosition = positionDropsown.SelectedOption.Text.Trim();

                currentCategoryName = rowList[1].FindElements(By.TagName("td"))[2].FindElement(By.TagName("span")).Text.Trim();

                currentCategory = rowList[1].FindElements(By.TagName("td"))[3].FindElement(By.TagName("span")).Text;

                currentStatus = rowList[1].FindElements(By.TagName("td"))[4].FindElement(By.TagName("span")).Text;

                isEditbutton = rowList[1].FindElements(By.TagName("td"))[5].FindElements(By.CssSelector("span > img")).Count;

                isDeleteButton = rowList[1].FindElements(By.TagName("td"))[5].FindElement(By.Id(or.readingXMLFile("CategoryManagement", "DeleteButton", "TVAdminPortalOR.xml") + 0)).GetAttribute("src").Contains("Delete.png");

                rowList[1].FindElements(By.TagName("td"))[5].FindElement(By.CssSelector("span > img")).Click();  //Clicking on Edit Button

                #endregion

                #region Region to Apply Assert On edit Page

                iWait.Until(ExpectedConditions.ElementIsVisible(By.Id("CreateCategoryTab")));

                Assert.AreEqual("Edit Category", driver.FindElement(By.Id("CreateCategoryTab")).Text);  //Checking whether the text of Create is changed to Edit Category

                Assert.AreEqual(true, driver.FindElement(By.Id("ContentPlaceHolder1_txtCategoryName")).Enabled);  //Verifying that Category Textfield is enable.

                Assert.AreNotEqual(String.Empty, driver.FindElement(By.Id("ContentPlaceHolder1_txtCategoryName")).GetAttribute("value")); // verifying that Category name has some pre populated data

                Assert.AreEqual(currentCategoryName, driver.FindElement(By.Id("ContentPlaceHolder1_txtCategoryName")).GetAttribute("value")); //verfiying that populated Categry was old one 

                Assert.AreEqual(currentPosition, new SelectElement(driver.FindElement(By.Id("ContentPlaceHolder1_ddlPosition1"))).SelectedOption.Text.Trim()); //verifying the prepopulated position is old one

                #endregion


                #region Region to Edit the Category and Verfiying Banner Message

                driver.FindElement(By.Id("ContentPlaceHolder1_txtCategoryName")).Clear();
                driver.FindElement(By.Id("ContentPlaceHolder1_txtCategoryName")).SendKeys(cancelbuttonfunctionality);
                positionDropdown = new SelectElement(driver.FindElement(By.Id("ContentPlaceHolder1_ddlPosition1")));
                positionDropdown.SelectByText("1");
                driver.FindElement(By.Id("ContentPlaceHolder1_btnCancel")).Click();

                #endregion

                Assert.AreEqual(false, driver.PageSource.Contains(cancelbuttonfunctionality));  //Checking that Category is not Edited.
                Assert.AreEqual(true, driver.PageSource.Contains(currentCategoryName));   //checking that if user clicks on Cancel button then system does not Edit and preserve the old name.


                #endregion

                log.Info("Scenario 3 : Changing Existing Category Name and Position and clicking on Save button, to check duplication in Edit mode" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

                #region Scenario 3 : Changing Existing Category Name and Position and clicking on Save button, to check duplication in Edit mode

                log.Info("Inside Scenarion 3" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

                CreatingCategory("first");

                String CategoryName = categoryNameWithGuid;  //getting last created Category name

                CreatingCategory("first");

                #region Region to Click on Edit button

                rowList = (IList<IWebElement>)driver.FindElement(By.Id(or.readingXMLFile("CategoryManagement", "CategoryListTable", "TVAdminPortalOR.xml"))).FindElements(By.TagName("tr"));
                rowList[1].FindElements(By.TagName("td"))[5].FindElement(By.CssSelector("span > img")).Click();  //Clicking on Edit Button

                #endregion

                #region Region to Edit the Category and Verfiying Banner Message

                iWait.Until(ExpectedConditions.ElementIsVisible(By.Id(or.readingXMLFile("CategoryManagement", "CategoryName", "TVAdminPortalOR.xml"))));

                driver.FindElement(By.Id(or.readingXMLFile("CategoryManagement", "CategoryName", "TVAdminPortalOR.xml"))).Clear();
                driver.FindElement(By.Id(or.readingXMLFile("CategoryManagement", "CategoryName", "TVAdminPortalOR.xml"))).SendKeys(CategoryName);

                positionDropdown = new SelectElement(driver.FindElement(By.Id(or.readingXMLFile("CategoryManagement", "CategoryPosition", "TVAdminPortalOR.xml"))));
                positionDropdown.SelectByText("1");

                driver.FindElement(By.Id(or.readingXMLFile("CategoryManagement", "SaveButton", "TVAdminPortalOR.xml"))).Click();

                #endregion

                // Verfiying User friendly message
                iWait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector(or.readingXMLFile("CategoryManagement", "InfoOkButton", "TVAdminPortalOR.xml"))));

                //Verifying infomration message for Duplicate Category.
                Assert.AreEqual("Category Already Exists", driver.FindElement(By.Id(or.readingXMLFile("CategoryManagement", "InfoMessage", "TVAdminPortalOR.xml"))).Text.Trim());

                driver.FindElement(By.CssSelector(or.readingXMLFile("CategoryManagement", "InfoOkButton", "TVAdminPortalOR.xml"))).Click();

                iWait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector(or.readingXMLFile("CategoryManagement", "InfoOkButton", "TVAdminPortalOR.xml"))));

                #endregion

                log.Info("TVAdmin_007_editButtonFunc test has completed" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

            }
            catch (Exception e)
            {
                log.Error(e.Message + "\n" + e.StackTrace + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

                Assert.AreEqual(true, false);
            }
        }


        [TestCaseSource("CategorynameDataSource")]
        [Test]
        public void TVAdmin_008_categoryNameValidation(DataRow testData)
        {
            try
            {
                log.Info("TVAdmin_008_categoryNameValidation test started" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

                RedirectingToCategoryManagement();      //moving to Category Management page 

                iWait.Until(ExpectedConditions.ElementIsVisible(By.Id(or.readingXMLFile("CategoryManagement", "CreateCategoryTab", "TVAdminPortalOR.xml"))));

                driver.FindElement(By.Id(or.readingXMLFile("CategoryManagement", "CreateCategoryTab", "TVAdminPortalOR.xml"))).Click();    //Clicking on Create Tab

                #region Filling_Details

                iWait.Until(ExpectedConditions.ElementIsVisible(By.Id(or.readingXMLFile("CategoryManagement", "CategoryName", "TVAdminPortalOR.xml"))));
                driver.FindElement(By.Id(or.readingXMLFile("CategoryManagement", "CategoryName", "TVAdminPortalOR.xml"))).SendKeys(testData["Column1"].ToString());

                driver.FindElement(By.Id(or.readingXMLFile("CategoryManagement", "SaveButton", "TVAdminPortalOR.xml"))).Click();


                #endregion

                uf.isJqueryActive(driver);

                iWait.Until(ExpectedConditions.ElementIsVisible(By.Id(or.readingXMLFile("CategoryManagement", "RegularExpression", "TVAdminPortalOR.xml"))));
                Assert.AreEqual("Please Enter Valid Category Name", driver.FindElement(By.Id(or.readingXMLFile("CategoryManagement", "RegularExpression", "TVAdminPortalOR.xml"))).Text);   //Verifying that Mendatroy message is dispayed correclty

                log.Info("TVAdmin_008_categoryNameValidation test has completed" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());
            }
            catch (Exception e)
            {
                log.Error(e.Message + "\n" + e.StackTrace + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

                Assert.AreEqual(true, false);
            }
        }

        public static IEnumerable<object> CategorynameDataSource
        {

            get
            {
                int start_cnt = 0;

                string ExcelDataPath = Environment.CurrentDirectory + "\\TestData_AdminPortal\\" + "NormalDataset_IET_Admin.xlsx";

                FileStream stream = File.Open(ExcelDataPath, FileMode.Open, FileAccess.Read);



                // 2. Reading from a OpenXml Excel file (2007 format; *.xlsx)
                Excel.IExcelDataReader excelReader = Excel.ExcelReaderFactory.CreateOpenXmlReader(stream);

                // 3. DataSet - The result of each spreadsheet will be created in the result.Tables

                DataSet result = excelReader.AsDataSet();

                // 4. DataSet - Create column names from first row
                excelReader.IsFirstRowAsColumnNames = true;

                System.Data.DataTable currentSheet = result.Tables[0];  //Accesing sheet 1 

                foreach (DataRow dr in currentSheet.Rows)
                {

                    if (start_cnt > 0)
                    {
                        yield return dr;
                    }
                    start_cnt++;
                }

                excelReader.Close();

            }
        }


        [Test]
        public void TVAdmin_009_verifySelectRecordMsg()
        {

            try
            {
                log.Info("TVAdmin_009_verifySelectRecordMessage test started" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

                RedirectingToCategoryManagement();

                driver.FindElement(By.Id(or.readingXMLFile("CategoryManagement", "ActiveButton", "TVAdminPortalOR.xml"))).Click();  //Clicking on Activate Button

                //Verfiying User friendly message
                iWait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector(or.readingXMLFile("CategoryManagement", "InfoOkButton", "TVAdminPortalOR.xml"))));

                Thread.Sleep(3000);

                Assert.AreEqual("Please select at least one record.", driver.FindElement(By.Id(or.readingXMLFile("CategoryManagement", "InfoMessage", "TVAdminPortalOR.xml"))).Text.Trim());    //Verifying infomration message for Duplicate Category.
                driver.FindElement(By.CssSelector(or.readingXMLFile("CategoryManagement", "InfoOkButton", "TVAdminPortalOR.xml"))).Click();

                Thread.Sleep(3000);
                iWait.Until(ExpectedConditions.ElementToBeClickable(By.Id(or.readingXMLFile("CategoryManagement", "InactiveButton", "TVAdminPortalOR.xml"))));
                driver.FindElement(By.Id(or.readingXMLFile("CategoryManagement", "InactiveButton", "TVAdminPortalOR.xml"))).Click();  //Clicking on Inctivate Button

                //Verfiying User friendly message
                iWait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector(or.readingXMLFile("CategoryManagement", "InfoOkButton", "TVAdminPortalOR.xml"))));

                Assert.AreEqual("Please select at least one record.", driver.FindElement(By.Id(or.readingXMLFile("CategoryManagement", "InfoMessage", "TVAdminPortalOR.xml"))).Text.Trim());    //Verifying infomration message for Duplicate Category.
                driver.FindElement(By.CssSelector(or.readingXMLFile("CategoryManagement", "InfoOkButton", "TVAdminPortalOR.xml"))).Click();

                log.Info("TVAdmin_009_verifySelectRecordMessage test has completed" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

            }
            catch (Exception e)
            {
                log.Error(e.Message + "\n" + e.StackTrace + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

                Assert.AreEqual(true, false);
            }
        }


        [Test]
        public void TVAdmin_010_positionChangingFunc()
        {
            try
            {

                log.Info("TVAdmin_010_positionChangingFunctionality test started" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

                RedirectingToCategoryManagement();

                log.Info("Inside Position Change" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

                //Retreving all the rows of Manage Table 
                int rowCount = driver.FindElement(By.Id(or.readingXMLFile("CategoryManagement", "CategoryListTable", "TVAdminPortalOR.xml"))).FindElements(By.TagName("tr")).Count;

                if (rowCount == 1)
                {
                    log.Info("No Record Present to do Position Change" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

                    Assert.AreEqual(true, false);
                }
                else
                {
                    rowList = (IList<IWebElement>)driver.FindElement(By.Id(or.readingXMLFile("CategoryManagement", "CategoryListTable", "TVAdminPortalOR.xml"))).FindElements(By.TagName("tr"));

                    String CategoryNameWhosePositionIsGoingToChanged = rowList[1].FindElements(By.TagName("td"))[2].FindElement(By.TagName("span")).Text.Trim();

                    SelectElement positionDropdown = new SelectElement(rowList[1].FindElements(By.TagName("td"))[1].FindElement(By.Id(or.readingXMLFile("CategoryManagement", "CategoryListPosition", "TVAdminPortalOR.xml") + 0)));

                    String PostitionBeforeChange = positionDropdown.SelectedOption.Text.Trim();

                    Random generator = new Random();
                    int changedPosition = generator.Next(1, 10);
                    while (changedPosition == Convert.ToInt32(PostitionBeforeChange))
                    {
                        changedPosition = generator.Next(1, positionDropdown.AllSelectedOptions.Count);
                    }

                    positionDropdown.SelectByText(changedPosition.ToString());

                    iWait.Until(ExpectedConditions.ElementIsVisible(By.Id(or.readingXMLFile("CategoryManagement", "WarningNoButton", "TVAdminPortalOR.xml"))));
                    Assert.AreEqual("Are you sure, you wish to change?", driver.FindElement(By.Id(or.readingXMLFile("CategoryManagement", "WarningMessage", "TVAdminPortalOR.xml"))).Text.Trim());  //Verfying that correct Warning MEssage is displayed
                    driver.FindElement(By.Id(or.readingXMLFile("CategoryManagement", "WarningNoButton", "TVAdminPortalOR.xml"))).Click();   //Clicking on No button of Warning Message

                    log.Info("Verfiying Position is not Changed" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

                    //Verfiy that Position is not Changed
                    //Retreving all the rows of Manage Table 
                    int i = 0;
                    rowList = (IList<IWebElement>)driver.FindElement(By.Id(or.readingXMLFile("CategoryManagement", "CategoryListTable", "TVAdminPortalOR.xml"))).FindElements(By.TagName("tr"));
                    foreach (IWebElement currentRow in rowList)
                    {
                        if (currentRow.GetAttribute("class").Equals("GridRowStyle") || currentRow.GetAttribute("class").Equals("AltGridStyle"))
                        {
                            columData = currentRow.FindElements(By.TagName("td"))[2].FindElement(By.TagName("span")).Text.Trim();
                            if (columData.Equals(CategoryNameWhosePositionIsGoingToChanged))
                            {
                                SelectElement positionDropdown1 = new SelectElement(driver.FindElement(By.Id(or.readingXMLFile("CategoryManagement", "CategoryListPosition", "TVAdminPortalOR.xml") + i)));
                                Assert.AreEqual(PostitionBeforeChange, positionDropdown1.SelectedOption.Text.Trim());
                                break;
                            }
                            i++;
                        }
                    }

                    positionDropdown.SelectByText(changedPosition.ToString());

                    iWait.Until(ExpectedConditions.ElementIsVisible(By.Id(or.readingXMLFile("CategoryManagement", "WarningYesButton", "TVAdminPortalOR.xml"))));
                    Assert.AreEqual("Are you sure, you wish to change?", driver.FindElement(By.Id(or.readingXMLFile("CategoryManagement", "WarningMessage", "TVAdminPortalOR.xml"))).Text.Trim());  //Verfying that correct Warning MEssage is displayed
                    driver.FindElement(By.Id(or.readingXMLFile("CategoryManagement", "WarningYesButton", "TVAdminPortalOR.xml"))).Click();   //Clicking on Yes button of Warning Message

                    iWait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.Id(or.readingXMLFile("CategoryManagement", "WarningYesButton", "TVAdminPortalOR.xml"))));

                    log.Info("Verfiying Position is  Changed" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

                    //Verfiy that position is changed
                    //Retreving all the rows of Manage Table 
                    i = 0;
                    rowList = (IList<IWebElement>)driver.FindElement(By.Id(or.readingXMLFile("CategoryManagement", "CategoryListTable", "TVAdminPortalOR.xml"))).FindElements(By.TagName("tr"));
                    foreach (IWebElement currentRow in rowList)
                    {
                        if (currentRow.GetAttribute("class").Equals("GridRowStyle") || currentRow.GetAttribute("class").Equals("AltGridStyle"))
                        {
                            columData = currentRow.FindElements(By.TagName("td"))[2].FindElement(By.TagName("span")).Text.Trim();
                            if (columData.Equals(CategoryNameWhosePositionIsGoingToChanged))
                            {
                                SelectElement positionDropdown1 = new SelectElement(driver.FindElement(By.Id(or.readingXMLFile("CategoryManagement", "CategoryListPosition", "TVAdminPortalOR.xml") + i)));
                                Assert.AreEqual(changedPosition.ToString(), positionDropdown1.SelectedOption.Text.Trim());

                                break;
                            }
                            i++;
                        }
                    }

                    iWait.Until(ExpectedConditions.ElementIsVisible(By.Id(or.readingXMLFile("CategoryManagement", "SuccessOkButton", "TVAdminPortalOR.xml"))));
                    Assert.AreEqual("Position is Updated Successfully.", driver.FindElement(By.Id(or.readingXMLFile("CategoryManagement", "SucessMessage", "TVAdminPortalOR.xml"))).Text.Trim());   //Verifying that Successcull message is displayed after delete
                    driver.FindElement(By.Id(or.readingXMLFile("CategoryManagement", "SuccessOkButton", "TVAdminPortalOR.xml"))).Click();     //Clicking on Ok button of Success Message

                }

                log.Info("TVAdmin_010_positionChangingFunctionality test has completed" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

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
            st.IE_TearDown(driver, log);
        }
    }
}
