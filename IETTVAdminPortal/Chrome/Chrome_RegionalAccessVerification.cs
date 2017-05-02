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


namespace IETTVAdminPortal.Chrome
{
    [TestFixture]
    class Chrome_RegionalAccessVerification
    {

        // This is to configure logger mechanism for Utilities.Config
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        #region Variable_Declration

        int counter = 1;

        IWait<IWebDriver> iWait = null;

        Boolean flag = false;

        IList<IWebElement> rowList = null;

        Boolean isCategoryPage = false;

        SelectElement positionDropdown = null;

        IJavaScriptExecutor executor;

        String columData = null;

        internal IWebDriver driver = null;

        string driverName = "", driverPath, appURL;

        Utility_Functions uf = new Utility_Functions();                             // Instantiate object for Utility Functons

        Configuration cf = new Configuration();                                    // Instantiate object for Configuration

        Chrome_AdminSetupTearDown st = new Chrome_AdminSetupTearDown();           // Instantiate object for Chrome Setup Teardown
        

        #endregion


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


        #region Reusable Function for this module

        public void OverlayWait()
        {
            iWait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.ClassName("overlay")));
        }

        public void VerifyCreatedRegion(Dictionary<String, String> dataToVerify)
        {

            //Using Nsoup here to parse the html table
            Document doc = NSoup.NSoupClient.Parse(driver.PageSource);
            Elements rowListNsoup = doc.GetElementById("ContentPlaceHolder1_gvRegionAccessList").GetElementsByTag("tr");

            flag = false;


            #region Applying_Assert_On_Manage_Page

            log.Info("Applying_Assert_On_Manage_Page" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

            flag = false;

            int rowcounter = 0;
            foreach (Element currentRow in rowListNsoup)
            {
                Attributes attr = currentRow.Attributes;

                //Row that have class="GridRowStyle" or class="AltGridStyle"
                if (attr["class"].Equals("GridRowStyle") || attr["class"].Equals("AltGridStyle"))
                {
                    log.Info("Row Counter :: " + rowcounter);
                    columData = currentRow.GetElementsByTag("td")[1].GetElementById("ContentPlaceHolder1_gvRegionAccessList_lblCountry_" + rowcounter).OwnText().Trim();


                    if (columData.Equals(dataToVerify["CountryName"]))
                    {
                        flag = true;

                        // assert to check checkbox is displayed
                        Assert.AreEqual("checkbox", currentRow.GetElementsByTag("td")[0].GetElementById("ContentPlaceHolder1_gvRegionAccessList_chkSelect_" + rowcounter).Attributes["type"]);


                        //Country Name
                        Assert.AreEqual(dataToVerify["CountryName"], currentRow.GetElementsByTag("td")[1].GetElementById("ContentPlaceHolder1_gvRegionAccessList_lblCountry_" + rowcounter).OwnText().Trim());

                        // Access Type Assertion
                        Assert.AreEqual(dataToVerify["AccessType"], currentRow.GetElementsByTag("td")[2].GetElementById("ContentPlaceHolder1_gvRegionAccessList_lblRegionType_" + rowcounter).OwnText().Trim());

                        // assert to check  Redirection Text
                        Assert.AreEqual(dataToVerify["RedirectionText"], currentRow.GetElementsByTag("td")[3].GetElementById("ContentPlaceHolder1_gvRegionAccessList_lblRedirectionTxt_" + rowcounter).OwnText().Trim());

                        // Assert to verify Redirection URl
                        Assert.AreEqual(dataToVerify["RedirectionURL"], currentRow.GetElementsByTag("td")[4].GetElementById("ContentPlaceHolder1_gvRegionAccessList_lblRedirectionUrl_" + rowcounter).OwnText().Trim());

                        // Assert to verify Start Date
                        Assert.AreEqual(dataToVerify["StartDate"], currentRow.GetElementsByTag("td")[5].GetElementById("ContentPlaceHolder1_gvRegionAccessList_lblStartDate_" + rowcounter).OwnText().Trim());

                        // Assert to verify End Date
                        Assert.AreEqual(dataToVerify["EndDate"], currentRow.GetElementsByTag("td")[6].GetElementById("ContentPlaceHolder1_gvRegionAccessList_lblEndDate_" + rowcounter).OwnText().Trim());

                        // Assert to verify the Notes
                        Assert.AreEqual(dataToVerify["Notes"], currentRow.GetElementsByTag("td")[7].GetElementById("ContentPlaceHolder1_gvRegionAccessList_lblNotes_" + rowcounter).OwnText().Trim());


                        // Assert to verify status
                        Assert.AreEqual(dataToVerify["Status"], currentRow.GetElementsByTag("td")[8].GetElementById("ContentPlaceHolder1_gvRegionAccessList_lblStatus_" + rowcounter).OwnText().Trim());


                        //write assert to check presence of edit button
                        Assert.AreEqual(true, currentRow.GetElementsByTag("td")[9].GetElementById("ContentPlaceHolder1_gvRegionAccessList_imgEdit_" + rowcounter).GetElementsByTag("img")[0].Attributes["src"].Contains("Edit.png"));

                        //write assert to check Channel name is present of not
                        log.Info("Region is present on Manage" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());
                        break;
                    }
                    rowcounter++;
                }
            }

            #endregion

            Assert.AreEqual(true, flag);   //If this fails signifies that created Channel is not displayed on MANAGE page 


        }

        public Dictionary<String, String> CreateRegion(Boolean isRestricted)
        {

            String redirectionText = "This is redirection field";
            String redirectionURL = "http://www.redirectionURL.com";
            Dictionary<String, String> dataToVerify = new Dictionary<String, String>();

            String noteFieldContent = "Entering text into note field";

            RedirectingToRegionAccessManagement();

            List<String> manageCountryList = GetManageCountryList();

            ClickOnCreateTab();

            if (manageCountryList.Count == GetCountryDropdown().Options.Count)
            {
                DialogResult result2 = MessageBox.Show("No Country is remaining to create a region.", "ERROR",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
                if (result2 == DialogResult.OK)
                {
                    System.Windows.Forms.Application.Exit();
                }
            }

            // Selecting Country from country drop down
            String selectedCountry = SelectCountry(manageCountryList);


            #region Decide whether to create Redirected or Restricted

            if (isRestricted)
            {
                dataToVerify.Add("AccessType", "Restricted");
                dataToVerify.Add("RedirectionText", String.Empty);
                dataToVerify.Add("RedirectionURL", String.Empty);
            }
            else if (!isRestricted)
            {
                RedirectedRadioButton().Click();
                uf.isJqueryActive(driver);
                RedirectionTextField().SendKeys(redirectionText);
                RedirectionURlField().SendKeys(redirectionURL);
                dataToVerify.Add("AccessType", "Redirect");
                dataToVerify.Add("RedirectionText", redirectionText);
                dataToVerify.Add("RedirectionURL", redirectionURL);
            }
            #endregion


            // Clicking on Start Date Picker field.
            GetStartDatePicker().Click();

            waitingForDatePicker();

            #region Getting Current Date from Date Picker and Verifying same with system date

            // Store currently selected date from date picker
            Dictionary<String, String> datePickerDate = getCurrentDateFromDatePicker();

            log.Info("\n Current Date Information" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());
            log.Info("Date ::" + datePickerDate["CurrentDate"]);
            log.Info("Month ::" + datePickerDate["CurrentMonth"]);
            log.Info("Year ::" + datePickerDate["CurrentYear"]);

            String sysDate = DateTime.Now.ToString("f");
            log.Info("System current Date :: " + sysDate);

            Assert.AreEqual(datePickerDate["CurrentDate"], Convert.ToInt32(sysDate.Split(' ')[0]).ToString());
            Assert.AreEqual(datePickerDate["CurrentMonth"], sysDate.Split(' ')[1]);
            Assert.AreEqual(datePickerDate["CurrentYear"], sysDate.Split(' ')[2]);

            #endregion

            //Select start date
            GetStartDate();

            //Select end date
            GetEndDate();

            // Entering text into Notes field.
            GetNoteField().SendKeys(noteFieldContent);

            // Storing Details that need to verify
            dataToVerify.Add("CountryName", selectedCountry);
            dataToVerify.Add("StartDate", GetStartDatePicker().GetAttribute("value"));
            dataToVerify.Add("EndDate", GetEndDatePicker().GetAttribute("value"));
            dataToVerify.Add("Notes", GetNoteField().GetAttribute("value"));
            dataToVerify.Add("Status", "Active");

            ClickSaveButton();

            #region Verifying success banner message

            iWait.Until(ExpectedConditions.ElementIsVisible(By.Id("btnOk")));
            Assert.AreEqual(" Region Access added successfully".Trim(), driver.FindElement(By.Id("Sucess_Message")).Text.Trim());
            driver.FindElement(By.Id("btnOk")).Click();
            iWait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.Id("btnOk")));

            #endregion

            return dataToVerify;

        }


        //This function will move the control to Regional Access Management Page
        public void RedirectingToRegionAccessManagement()
        {
            iWait.Until(ExpectedConditions.ElementIsVisible(By.Id("rptMenu_aMenuItem_0")));
            
            //clicking on Admin dropdown
            driver.FindElement(By.Id("rptMenu_aMenuItem_0")).Click();

            uf.isJqueryActive(driver);

            iWait.Until(ExpectedConditions.ElementToBeClickable(By.LinkText("Regional Access")));

            //Clicking on Category Management
            driver.FindElement(By.LinkText("Regional Access")).Click();

            iWait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("div#viewContent > div > div > div.titleBox > h2")));
            isCategoryPage = driver.FindElement(By.CssSelector("div#viewContent > div > div > div.titleBox > h2")).Displayed;

            //Checking whether the user is on Category page
            Assert.AreEqual(true, isCategoryPage);

            //verifying the default active tab of Category management page
            Assert.AreEqual(String.Empty, driver.FindElement(By.Id("createRegion")).GetAttribute("class"));
            Assert.AreEqual("active", driver.FindElement(By.Id("RegionAccessList")).GetAttribute("class"));

        }

        // This function will return status of element checked or not
        public Boolean IsElementChecked(String checkboxID)
        {

            Boolean checkboxStatus = (Boolean)executor.ExecuteScript(" return document.getElementById('" + checkboxID + "').checked");

            log.Info("Status of element Checked :: " + checkboxStatus + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

            return checkboxStatus;

        }

        //This function will return status of element enabled or not
        public Boolean IsElementdisabled(String eleID)
        {
            Boolean enableStatus = (Boolean)executor.ExecuteScript("return document.getElementById('" + eleID + "').disabled");

            return enableStatus;
        }

        // This function Stores the country name that is already created and return list to calling function.
        public List<String> GetManageCountryList()
        {

            log.Info("\n Getting already created Country Name " + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

            List<String> countryList = new List<String>();

            //Using Nsoup here to parse the html table
            Document doc = NSoup.NSoupClient.Parse(driver.PageSource);
            Elements rowListNsoup = doc.GetElementById("ContentPlaceHolder1_gvRegionAccessList").GetElementsByTag("tr");

            flag = false;

            int rowcounter = 0;
            foreach (Element currentRow in rowListNsoup)
            {
                Attributes attr = currentRow.Attributes;

                //Row that have class="GridRowStyle" or class="AltGridStyle"
                if (attr["class"].Equals("GridRowStyle") || attr["class"].Equals("AltGridStyle"))
                {
                    countryList.Add(currentRow.GetElementsByTag("td")[1].GetElementById("ContentPlaceHolder1_gvRegionAccessList_lblCountry_" + rowcounter).OwnText().Trim());
                    rowcounter++;
                }
            }


            foreach (String str in countryList)
                log.Info(str);

            return countryList;
        }

        //This function selects country which is not present on manage page.
        public String SelectCountry(List<String> manageCountryList)
        {

            SelectElement countryDropdown = new SelectElement(driver.FindElement(By.Id("ContentPlaceHolder1_ddlCountry")));

            int i = 1;
            Boolean flag = false;
            while (true)
            {
                flag = false;
                countryDropdown.SelectByIndex(i);
                String selectedCountry = countryDropdown.SelectedOption.Text.Trim();

                foreach (String country in manageCountryList)
                {
                    if (selectedCountry.Equals(country))
                    {
                        flag = true;
                        break;
                    }
                }

                if (flag == false)  // to break while loop
                    break;

                i++;
            }




            return countryDropdown.SelectedOption.Text.Trim();
        }


        #endregion

        #region Reusable function in Date picker Handling

        // This will return Dictionary contains value and key of current month
        public Dictionary<int, String> getRandomMonthName(int currentMonth)
        {
            log.Info("\n Getting Random Month Name and Position" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

            Dictionary<int, String> monthList = new Dictionary<int, String>();
            monthList.Add(1, "January");
            monthList.Add(2, "February");
            monthList.Add(3, "March");
            monthList.Add(4, "April");
            monthList.Add(5, "May");
            monthList.Add(6, "June");
            monthList.Add(7, "July");
            monthList.Add(8, "August");
            monthList.Add(9, "September");
            monthList.Add(10, "October");
            monthList.Add(11, "November");
            monthList.Add(12, "December");

            Random getRandomNumber = new Random();


            int monthPosition = 0;
            monthPosition = getRandomNumber.Next(1, 12);
            while (monthPosition < currentMonth) // 
            {
                monthPosition = getRandomNumber.Next(1, 12);
            }

            Dictionary<int, String> monthList1 = new Dictionary<int, String>();
            monthList1.Add(monthPosition, monthList[monthPosition]);

            log.Info("Random Selected Month :: " + monthList1.ElementAt(0).Value + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

            return monthList1;
        }

        public int getDate(int currentMonth, int currentDate)
        {

            Dictionary<int, int> dateList = new Dictionary<int, int>();
            dateList.Add(1, 1);
            dateList.Add(2, 2);
            dateList.Add(3, 3);
            dateList.Add(4, 4);
            dateList.Add(5, 5);
            dateList.Add(6, 6);
            dateList.Add(7, 7);
            dateList.Add(8, 8);
            dateList.Add(9, 9);
            dateList.Add(10, 10);
            dateList.Add(11, 11);
            dateList.Add(12, 12);
            dateList.Add(13, 13);
            dateList.Add(14, 14);
            dateList.Add(15, 15);
            dateList.Add(16, 16);
            dateList.Add(17, 17);
            dateList.Add(18, 18);
            dateList.Add(19, 19);
            dateList.Add(20, 20);
            dateList.Add(21, 21);
            dateList.Add(22, 22);
            dateList.Add(23, 23);
            dateList.Add(24, 24);
            dateList.Add(25, 25);
            dateList.Add(26, 26);
            dateList.Add(27, 27);
            dateList.Add(28, 28);
            dateList.Add(29, 29);
            dateList.Add(30, 30);
            dateList.Add(31, 32);

            Random getRandomNumber = new Random();

            int date = 0;

            if (currentMonth == 1 || currentMonth == 3 || currentMonth == 5 || currentMonth == 7 || currentMonth == 8 || currentMonth == 10 || currentMonth == 12)
            {
                date = getRandomNumber.Next(1, 31);
                while (date < currentDate) // 
                {
                    date = getRandomNumber.Next(1, 31);
                }
            }
            else if (currentMonth == 4 || currentMonth == 6 || currentMonth == 9 || currentMonth == 11)
            {
                date = getRandomNumber.Next(1, 30);
                while (date < currentDate) // 
                {
                    date = getRandomNumber.Next(1, 30);
                }
            }
            else if (currentMonth == 2)
            {
                date = getRandomNumber.Next(1, 28);
                while (date < currentDate) // 
                {
                    date = getRandomNumber.Next(1, 28);
                }
            }

            return date;
        }

        public void SelectYearInDatePicker(int requiredYear)
        {

            // Retreiving Year from Date picker
            int yearFromDatePicker = Convert.ToInt32(driver.FindElement(By.ClassName("ui-datepicker-year")).Text.Trim());

            // Iterating till Year of Date picker is equal our required year.
            while (yearFromDatePicker != requiredYear)
            {
                driver.FindElement(By.ClassName("ui-icon-circle-triangle-e")).Click();
                uf.isJqueryActive(driver);
                yearFromDatePicker = Convert.ToInt32(driver.FindElement(By.ClassName("ui-datepicker-year")).Text.Trim());
            }
        }

        public void SelectMonthInDatePicker(Dictionary<int, String> requiredMonth)
        {

            // Retreiving month from Date picker
            String monthFromDatePicker = driver.FindElement(By.ClassName("ui-datepicker-month")).Text.Trim();

            // Iterating till month of Date picker is equal our required month.
            while (!monthFromDatePicker.Equals(requiredMonth.ElementAt(0).Value))
            {
                log.Info("MOnth from Date Picker :: " + monthFromDatePicker + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());
                driver.FindElement(By.ClassName("ui-icon-circle-triangle-e")).Click();
                uf.isJqueryActive(driver);
                monthFromDatePicker = driver.FindElement(By.ClassName("ui-datepicker-month")).Text.Trim();
            }
        }

        public void SelectDateInDatePicker(int date)
        {

            Boolean flag = false;
            IList<IWebElement> datepickerTable = (IList<IWebElement>)driver.FindElements(By.CssSelector("table.ui-datepicker-calendar >tbody > tr"));
            log.Info("No of Row in Date Picker :: " + datepickerTable.Count);

            IList<IWebElement> columnOfDatePicker = null;

            foreach (IWebElement currentRow in datepickerTable)
            {

                columnOfDatePicker = currentRow.FindElements(By.TagName("td"));
                log.Info("No of Column :: " + columnOfDatePicker.Count);

                foreach (IWebElement currentColumn in columnOfDatePicker)
                {
                    int isDateBlank = currentColumn.FindElements(By.TagName("a")).Count;
                    if (isDateBlank != 0)
                    {
                        log.Info("Date Value :: " + currentColumn.FindElement(By.TagName("a")).Text.Trim());
                        if (date == Convert.ToInt32(currentColumn.FindElement(By.TagName("a")).Text.Trim()))
                        {
                            flag = true;
                            Thread.Sleep(1000);
                            currentColumn.Click();
                            break;
                        }
                    }
                } // inner foreach
                if (flag)
                    break;
            } // outer foreach


        }

        public void isPreviousArrowEnabled()
        {



        }

        //This function will return Dictionary that contains current Date, Month, year.
        public Dictionary<String, String> getCurrentDateFromDatePicker()
        {

            Dictionary<String, String> currentDate = new Dictionary<String, String>();


            Boolean flag = false;
            IList<IWebElement> datepickerTable = (IList<IWebElement>)driver.FindElements(By.CssSelector("table.ui-datepicker-calendar >tbody > tr"));
            log.Info("No of Row in Date Picker :: " + datepickerTable.Count);

            IList<IWebElement> columnOfDatePicker = null;

            foreach (IWebElement currentRow in datepickerTable)
            {

                columnOfDatePicker = currentRow.FindElements(By.TagName("td"));
                log.Info("No of Column :: " + columnOfDatePicker.Count);

                foreach (IWebElement currentColumn in columnOfDatePicker)
                {
                    String classValue = currentColumn.GetAttribute("class");
                    if (!currentColumn.GetAttribute("class").Contains("ui-state-disabled"))
                    {
                        currentDate.Add("CurrentDate", currentColumn.FindElement(By.ClassName("ui-state-default")).Text.Trim());
                        currentDate.Add("CurrentMonth", driver.FindElement(By.ClassName("ui-datepicker-month")).Text.Trim());
                        currentDate.Add("CurrentYear", driver.FindElement(By.ClassName("ui-datepicker-year")).Text.Trim());

                        flag = true;

                        break;
                    }


                } // inner foreach
                if (flag)
                    break;
            } // outer foreach

            return currentDate;

        }

        //This function will get the Start date
        public void GetStartDate()
        {
            log.Info("\n Selecting Start Date" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

            DateTime SystemDate = DateTime.Now;

            SelectYearInDatePicker(SystemDate.Year);

            Dictionary<int, String> requiredStartMonth = getRandomMonthName(SystemDate.Month);

            SelectMonthInDatePicker(requiredStartMonth);

            int date = getDate(SystemDate.Month, SystemDate.Day);
            log.Info("Required Date :: " + date + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

            SelectDateInDatePicker(date);

            log.Info("\n Selecting Start Date completed" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());
        }

        //This function will get the End date
        public void GetEndDate()
        {
            log.Info("\n Selecting End Date started" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

            Boolean checkboxStatus = IsElementChecked("ContentPlaceHolder1_chkEndDate");

            if (!checkboxStatus)
            {
                Assert.AreEqual(false, GetEndDatePicker().Enabled);
                GetEndDateCheckBox().Click();
            }

            Assert.AreEqual(true, GetEndDateCheckBox().Enabled);


            string strDate = GetStartDatePicker().GetAttribute("value").ToString();

            DateTime dt = DateTime.ParseExact(strDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);


            DateTime dte = dt.AddDays(2.0);
            string enddate = dte.ToString("dd/MM/yyyy");

            log.Info("End Date  " + dte.ToString("dd/MM/yyyy") + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

            GetEndDatePicker().SendKeys(enddate);

            log.Info("Selecting End Date Completed" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

        }

        public String GetSystemDate(Double days)
        {
            DateTime currentDate = DateTime.Now.AddDays(days);
            return currentDate.ToString("dd-MM-yyyy").Replace('-', '/');
        }

        #endregion

        #region Reusable elements

        // This function clicks on create tab and wait till Create tab active.
        public void ClickOnCreateTab()
        {
            //Clicking on Create Tab
            driver.FindElement(By.Id("CreateRegionAccessTab")).Click();

            iWait.Until(d => d.FindElement(By.Id("createRegion")).GetAttribute("class").Equals("active"));

            uf.isJqueryActive(driver);
        }

        public SelectElement GetCountryDropdown()
        {
            return new SelectElement(driver.FindElement(By.Id("ContentPlaceHolder1_ddlCountry")));
        }

        public String GetSelectedOptionFromCountryDropdown()
        {
            return GetCountryDropdown().SelectedOption.Text.Trim();
        }

        public IWebElement RestrictedRedioButton()
        {
            return driver.FindElement(By.Id("ContentPlaceHolder1_rdoRegionAccessType_0"));
        }

        public IWebElement RedirectedRadioButton()
        {
            return driver.FindElement(By.Id("ContentPlaceHolder1_rdoRegionAccessType_1"));
        }

        public IWebElement RedirectionTextField()
        {
            return driver.FindElement(By.Id("ContentPlaceHolder1_txtRedirect"));
        }

        public IWebElement RedirectionURlField()
        {
            return driver.FindElement(By.Id("ContentPlaceHolder1_txtRedirectURL"));
        }

        public IWebElement GetStartDatePicker()
        {
            return driver.FindElement(By.Id("ContentPlaceHolder1_txtstartDate"));
        }

        public IWebElement GetEndDateCheckBox()
        {
            return driver.FindElement(By.Id("ContentPlaceHolder1_chkEndDate"));
        }

        public IWebElement GetEndDatePicker()
        {
            return driver.FindElement(By.Id("ContentPlaceHolder1_txtEndDate"));
        }

        public IWebElement GetNoteField()
        {
            return driver.FindElement(By.Id("ContentPlaceHolder1_txtNotes"));
        }

        public IWebElement SaveButton()
        {
            return driver.FindElement(By.Id("ContentPlaceHolder1_btnSave"));
        }

        public IWebElement CancelButton()
        {
            return driver.FindElement(By.Id("ContentPlaceHolder1_btnCancel"));
        }

        public IWebElement GetActivateButton()
        {
            return driver.FindElement(By.Id("ContentPlaceHolder1_btnActive"));
        }

        public IWebElement GetInactivateButton()
        {
            return driver.FindElement(By.Id("ContentPlaceHolder1_btnDeactive"));
        }

        public void ClickManagePageCheckBox(int rowCounter)
        {

            driver.FindElement(By.Id("ContentPlaceHolder1_gvRegionAccessList")).
                FindElements(By.TagName("tr"))[rowCounter + 1].
                FindElements(By.TagName("td"))[0].
                FindElement(By.Id("ContentPlaceHolder1_gvRegionAccessList_chkSelect_" + rowCounter)).
                Click();
        }

        public void ClickActivateButton()
        {
            GetActivateButton().Click();
        }

        public void ClickInactivateButton()
        {
            GetInactivateButton().Click();
        }

        public void waitingForDatePicker()
        {
            iWait.Until(ExpectedConditions.ElementIsVisible(By.Id("ui-datepicker-div")));
        }

        public void ClickSaveButton()
        {
            IJavaScriptExecutor exSave = (IJavaScriptExecutor)driver;
            exSave.ExecuteScript("arguments[0].click()", SaveButton());
            // SaveButton().Click();
        }

        public void ClickEditIcon(int rowCounter)
        {
            driver.FindElement(By.Id("ContentPlaceHolder1_gvRegionAccessList")).
            FindElements(By.TagName("tr"))[rowCounter + 1].
            FindElements(By.TagName("td"))[9].
            FindElement(By.Id("ContentPlaceHolder1_gvRegionAccessList_imgEdit_" + rowCounter)).
            FindElement(By.TagName("img")).
            Click();
        }

        #endregion



        [Test]
        public void TVAdmin_001_UIVerification()
        {
            SelectElement countryDropdown = null;

            try
            {
                log.Info("TVAdmin_001_UIVerification started" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

                RedirectingToRegionAccessManagement();

                #region Manage page verification

                log.Info("manage page UIVerification started" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

                // Geting all the record of manage grid
                IList<IWebElement> tableHeader = (IList<IWebElement>)driver.FindElement(By.Id("ContentPlaceHolder1_gvRegionAccessList")).FindElement(By.ClassName("GridHeader")).FindElements(By.TagName("th"));

                //Verify the number of columns 
                Assert.AreEqual(10, tableHeader.Count);

                string[] arrTableHeader = { "Select", "Country Name", "Access Type", "Redirection Text", "Redirection URL", "Start Date", "End Date", "Notes", "Status", "Edit" };
                int i = 0;

                foreach (IWebElement header in tableHeader)
                {
                    Assert.AreEqual(arrTableHeader[i], header.Text.Trim());
                    i++;
                }

                log.Info("manage page UIVerification completed" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());
                #endregion

                ClickOnCreateTab();

                #region Create Page Verification

                log.Info("Create page UIVerification started" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

                #region Verify the default Status

                //Verify Create Tab is Active
                Assert.AreEqual(true, driver.FindElement(By.Id("createRegion")).GetAttribute("class").Contains("active"));

                //Verify Country name default selected option
                countryDropdown = new SelectElement(driver.FindElement(By.Id("ContentPlaceHolder1_ddlCountry")));
                Assert.AreEqual("Select", countryDropdown.SelectedOption.Text);

                //verify the status of Restricted button
                Boolean isRestictedSelected = IsElementChecked("ContentPlaceHolder1_rdoRegionAccessType_0");
                Assert.AreEqual(true, isRestictedSelected);

                //verify the status of Redirected button
                Boolean isRedirectedChecked = IsElementChecked("ContentPlaceHolder1_rdoRegionAccessType_1");
                Assert.AreEqual(false, isRedirectedChecked);


                //verify the status of RedirectionText
                Boolean isRedirectionTextDisabled = IsElementdisabled("ContentPlaceHolder1_txtRedirect");
                Assert.AreEqual(true, isRedirectionTextDisabled);

                //verify the status of Redirection Url
                Boolean isRedirectionUrlDisabled = IsElementdisabled("ContentPlaceHolder1_txtRedirectURL");
                Assert.AreEqual(true, isRedirectionUrlDisabled);

                //verify the status of Start Date field
                Boolean isStartDateDisabled = IsElementdisabled("ContentPlaceHolder1_txtstartDate");
                Assert.AreEqual(false, isStartDateDisabled);

                //verify the status of End Date check box
                Boolean isEndDateCheckboxChecked = IsElementChecked("ContentPlaceHolder1_chkEndDate");
                Assert.AreEqual(false, isEndDateCheckboxChecked);

                //verify the status of End Date field
                Boolean isEndDateDisabled = IsElementdisabled("ContentPlaceHolder1_txtEndDate");
                Assert.AreEqual(true, isEndDateDisabled);

                //verify the status of Notes field
                Boolean isNotesFieldDisabled = IsElementdisabled("ContentPlaceHolder1_txtNotes");
                Assert.AreEqual(false, isNotesFieldDisabled);

                //verify Save and Cancel button
                Assert.AreEqual(true, SaveButton().Enabled);
                Assert.AreEqual(true, CancelButton().Enabled);

                #endregion


                #region verify changes on clicking Redirected

                //Click on redirected element
                RedirectedRadioButton().Click();

                //Verify the status of Redirection Text and Redirection Url
                isRedirectionTextDisabled = IsElementdisabled("ContentPlaceHolder1_txtRedirect");
                isRedirectionUrlDisabled = IsElementdisabled("ContentPlaceHolder1_txtRedirectURL");

                Assert.AreEqual(false, isRedirectionTextDisabled);
                Assert.AreEqual(false, isRedirectionUrlDisabled);

                #endregion

                #region verify changes on clicking Restricted

                //Click on restricted element
                RestrictedRedioButton().Click();

                //Verify the status of Redirection Text and Redirection Url
                isRedirectionTextDisabled = IsElementdisabled("ContentPlaceHolder1_txtRedirect");
                isRedirectionUrlDisabled = IsElementdisabled("ContentPlaceHolder1_txtRedirectURL");

                //Verify the status of Redirection Text and Redirection Url
                Assert.AreEqual(true, isRedirectionTextDisabled);
                Assert.AreEqual(true, isRedirectionUrlDisabled);
                #endregion

                #region verify changes on clicking End date checkbox

                GetEndDateCheckBox().Click();

                isEndDateCheckboxChecked = IsElementChecked("ContentPlaceHolder1_chkEndDate");
                isEndDateDisabled = IsElementdisabled("ContentPlaceHolder1_txtEndDate");

                Assert.AreEqual(true, isEndDateCheckboxChecked);
                Assert.AreEqual(false, isEndDateDisabled);
                #endregion

                log.Info("Create page UIVerification completed" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

                #endregion

                log.Info("TVAdmin_001_UIVerification completed" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

            }
            catch (Exception e)
            {

                log.Error(e.Message + "\n" + e.StackTrace + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());
                
                Assert.AreEqual(true, false);
            }
        }

        [Test]
        public void TVAdmin_002_CreateRestrictedRegions()
        {
            try
            {
                log.Info("TVAdmin_002_CreateRegions started\n");

                Dictionary<String, String> dataToVerify = CreateRegion(true);

                VerifyCreatedRegion(dataToVerify);

                log.Info("\nTVAdmin_002_CreateRegions completed" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

            }
            catch (Exception e)
            {

                log.Error(e.Message + "\n" + e.StackTrace + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());
                
                Assert.AreEqual(true, false);
            }
        }
        
        [Test]
        public void TVAdmin_003_CreateRedirectedRegions()
        {
            try
            {
                log.Info("TVAdmin_003_CreateRedirectedRegions started\n" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

                Dictionary<String, String> dataToVerify = CreateRegion(false);

                VerifyCreatedRegion(dataToVerify);

                log.Info("TVAdmin_003_CreateRedirectedRegions completed" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

            }
           catch (Exception e)
            {

                log.Error(e.Message + "\n" + e.StackTrace + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());
                
                Assert.AreEqual(true, false);
            }
        }

        [Test]
        public void TVAdmin_004_CancelButtonFunc()
        {

            String noteFieldContent = "Here We are testing Cancel button functionality";

            try
            {
                log.Info("TVAdmin_004_CancelButtonFunc started\n" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

                RedirectingToRegionAccessManagement();

                List<String> manageCountryList = GetManageCountryList();

                ClickOnCreateTab();

                // Selecting Country from country drop down
                String selectedCountry = SelectCountry(manageCountryList);
                log.Info("Created Country Name :: " + selectedCountry + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

                // Clicking on Date Picker field.
                GetStartDatePicker().Click();

                waitingForDatePicker();

                #region Getting Current Date from Date Picker and Verifying same with system date

                // Store currently selected date from date picker
                Dictionary<String, String> datePickerDate = getCurrentDateFromDatePicker();

                log.Info("\n Current Date Information" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());
                log.Info("Date ::" + datePickerDate["CurrentDate"]);
                log.Info("Month ::" + datePickerDate["CurrentMonth"]);
                log.Info("Year ::" + datePickerDate["CurrentYear"]);

                String sysDate = DateTime.Now.ToString("f");
                log.Info("System current Date :: " + sysDate);


                Assert.AreEqual(datePickerDate["CurrentDate"], Convert.ToInt32(sysDate.Split(' ')[0]).ToString());
                Assert.AreEqual(datePickerDate["CurrentMonth"], sysDate.Split(' ')[1]);
                Assert.AreEqual(datePickerDate["CurrentYear"], sysDate.Split(' ')[2]);

                #endregion

                //get start date
                GetStartDate();

                //get end date
                GetEndDate();

                // Entering text into Notes field.
                GetNoteField().SendKeys(noteFieldContent);


                driver.FindElement(By.Id("ContentPlaceHolder1_btnCancel")).Click();

                #region Verifying on Manage page


                uf.isJqueryActive(driver);

                //Using Nsoup here to parse the html table
                Document doc = NSoup.NSoupClient.Parse(driver.PageSource);
                Elements rowListNsoup = doc.GetElementById("ContentPlaceHolder1_gvRegionAccessList").GetElementsByTag("tr");

                flag = false;


                #region Applying_Assert_On_Manage_Page

                flag = false;

                int rowcounter = 0;
                foreach (Element currentRow in rowListNsoup)
                {
                    Attributes attr = currentRow.Attributes;

                    //Row that have class="GridRowStyle" or class="AltGridStyle"
                    if (attr["class"].Equals("GridRowStyle") || attr["class"].Equals("AltGridStyle"))
                    {
                        log.Info("Row Counter :: " + rowcounter);
                        columData = currentRow.GetElementsByTag("td")[1].GetElementById("ContentPlaceHolder1_gvRegionAccessList_lblCountry_" + rowcounter).OwnText().Trim();


                        if (columData.Equals(selectedCountry))
                        {
                            flag = true;
                            log.Info("Region is not present on Manage" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());
                            break;
                        }
                        rowcounter++;
                    }
                }

                #endregion

                Assert.AreEqual(false, flag);   //IF THIS FAILS MEANS created Channel IS NOT displayed ON MANAGE page 

                #endregion

                log.Info("\nTVAdmin_004_CancelButtonFunc completed" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());


            }
            catch (Exception e)
            {

                log.Error(e.Message + "\n" + e.StackTrace + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());
                
                Assert.AreEqual(true, false);
            }
        }


        [Test]
        public void TVAdmin_005_AlreadyExistValidation()
        {

            Dictionary<String, String> dataToVerify = new Dictionary<String, String>();
            String countryName = null;
            String noteFieldContent = "Here we are testing already existing region";

            try
            {

                log.Info("\nTVAdmin_005_AlreadyExistValidation started" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());



                RedirectingToRegionAccessManagement();

                List<String> manageCountryList = GetManageCountryList();

                #region Creating already exist Region

                ClickOnCreateTab();

                // Selecting Country from country drop down
                if (manageCountryList.Count == 0)
                {
                    dataToVerify = CreateRegion(true);
                    countryName = dataToVerify["CountryName"];
                    GetCountryDropdown().SelectByText(countryName);
                }
                else
                {
                    GetCountryDropdown().SelectByText(manageCountryList[0]);
                }

                // Clicking on Date Picker field.
                GetStartDatePicker().Click();

                waitingForDatePicker();

                #region Getting Current Date from Date Picker and Verifying same with system date

                // Store currently selected date from date picker
                Dictionary<String, String> datePickerDate = getCurrentDateFromDatePicker();

                log.Info("\n Current Date Information");
                log.Info("Date ::" + datePickerDate["CurrentDate"]);
                log.Info("Month ::" + datePickerDate["CurrentMonth"]);
                log.Info("Year ::" + datePickerDate["CurrentYear"]);

                String sysDate = DateTime.Now.ToString("f");
                log.Info("System current Date :: " + sysDate);


                Assert.AreEqual(datePickerDate["CurrentDate"], Convert.ToInt32(sysDate.Split(' ')[0]).ToString());
                Assert.AreEqual(datePickerDate["CurrentMonth"], sysDate.Split(' ')[1]);
                Assert.AreEqual(datePickerDate["CurrentYear"], sysDate.Split(' ')[2]);

                #endregion

                //get start date
                GetStartDate();

                //get end date
                GetEndDate();

                // Entering text into Notes field.
                GetNoteField().SendKeys(noteFieldContent);

                ClickSaveButton();

                #region Verifying success banner message


                iWait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("div.infomsg > button")));
                Assert.AreEqual(" Country already exists.".Trim(), driver.FindElement(By.CssSelector("div.infomsg > span#Info")).Text.Trim());
                driver.FindElement(By.CssSelector("div.infomsg > button")).Click();
                iWait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector("div.infomsg > button")));

                #endregion

                #endregion

                log.Info("\nTVAdmin_005_AlreadyExistValidation completed" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());


            }
            catch (Exception e)
            {

                log.Error(e.Message + "\n" + e.StackTrace + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());
                
                Assert.AreEqual(true, false);
            }
        }


        [Test]
        public void TVAdmin_006_ActivateInactivateFunc()
        {

            String regionName = null;

            Dictionary<String, String> dataToVerify = new Dictionary<String, String>();
            Dictionary<String, String> detailBeforeInactivate = new Dictionary<String, String>();
            Dictionary<String, String> detailBeforeActivate = new Dictionary<String, String>();

            try
            {

                log.Info("\nTVAdmin_006_ActivateInactivateFunc started" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

                RedirectingToRegionAccessManagement();

                iWait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.ClassName("overlay")));

                List<String> manageCountryList = GetManageCountryList();

                // Selecting Country from country drop down
                if (manageCountryList.Count == 0)
                {
                    dataToVerify = CreateRegion(true);
                    regionName = dataToVerify["CountryName"];
                }
                else
                {
                    regionName = manageCountryList[0];
                }


                //Using Nsoup here to parse the html table
                Document doc = NSoup.NSoupClient.Parse(driver.PageSource);
                Elements rowListNsoup = doc.GetElementById("ContentPlaceHolder1_gvRegionAccessList").GetElementsByTag("tr");


                #region Inactivating the Region

                #region Storing details and Clicking on Inactivate button

                log.Info("\n\tStoring details and Clicking on Inactivate button" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

                flag = false;

                int rowcounter = 0;
                foreach (Element currentRow in rowListNsoup)
                {
                    Attributes attr = currentRow.Attributes;

                    //Row that have class="GridRowStyle" or class="AltGridStyle"
                    if (attr["class"].Equals("GridRowStyle") || attr["class"].Equals("AltGridStyle"))
                    {
                        columData = currentRow.GetElementsByTag("td")[1].GetElementById("ContentPlaceHolder1_gvRegionAccessList_lblCountry_" + rowcounter).OwnText().Trim();


                        if (columData.Equals(regionName))
                        {
                            flag = true;


                            detailBeforeInactivate.Add("IsCheckbox", currentRow.GetElementsByTag("td")[0].GetElementById("ContentPlaceHolder1_gvRegionAccessList_chkSelect_" + rowcounter).Attributes["type"]);
                            detailBeforeInactivate.Add("CountryName", currentRow.GetElementsByTag("td")[1].GetElementById("ContentPlaceHolder1_gvRegionAccessList_lblCountry_" + rowcounter).OwnText().Trim());
                            detailBeforeInactivate.Add("AccessType", currentRow.GetElementsByTag("td")[2].GetElementById("ContentPlaceHolder1_gvRegionAccessList_lblRegionType_" + rowcounter).OwnText().Trim());

                            detailBeforeInactivate.Add("RedirectionText", currentRow.GetElementsByTag("td")[3].GetElementById("ContentPlaceHolder1_gvRegionAccessList_lblRedirectionTxt_" + rowcounter).OwnText().Trim());

                            detailBeforeInactivate.Add("RedirectionURL", currentRow.GetElementsByTag("td")[4].GetElementById("ContentPlaceHolder1_gvRegionAccessList_lblRedirectionUrl_" + rowcounter).OwnText().Trim());
                            detailBeforeInactivate.Add("StartDate", currentRow.GetElementsByTag("td")[5].GetElementById("ContentPlaceHolder1_gvRegionAccessList_lblStartDate_" + rowcounter).OwnText().Trim());
                            detailBeforeInactivate.Add("EndDate", currentRow.GetElementsByTag("td")[6].GetElementById("ContentPlaceHolder1_gvRegionAccessList_lblEndDate_" + rowcounter).OwnText().Trim());
                            detailBeforeInactivate.Add("Notes", currentRow.GetElementsByTag("td")[7].GetElementById("ContentPlaceHolder1_gvRegionAccessList_lblNotes_" + rowcounter).OwnText().Trim());
                            detailBeforeInactivate.Add("Status", currentRow.GetElementsByTag("td")[8].GetElementById("ContentPlaceHolder1_gvRegionAccessList_lblStatus_" + rowcounter).OwnText().Trim());


                            ClickManagePageCheckBox(rowcounter);

                            ClickInactivateButton();

                            //write assert to check presence of edit and delete button
                            if (detailBeforeInactivate["Status"].Equals("Active"))
                            {
                                Assert.AreEqual(true, currentRow.GetElementsByTag("td")[9].GetElementById("ContentPlaceHolder1_gvRegionAccessList_imgEdit_" + rowcounter).GetElementsByTag("img")[0].Attributes["src"].Contains("Edit.png"));
                            }
                            else if (detailBeforeInactivate["Status"].Equals("Inactive"))
                            {
                                Assert.AreEqual(0, currentRow.GetElementsByTag("td")[9].GetElementsByTag("span").Count);
                            }

                            //write assert to check Channel name is present of not
                            log.Info("Region is present on Manage" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());
                            break;
                        }
                        rowcounter++;
                    }
                }

                Assert.AreEqual(true, flag);   //IF THIS FAILS MEANS created Channel IS NOT displayed ON MANAGE page 

                log.Info("\n\tRegion is Inactivated");

                #endregion

                uf.isJqueryActive(driver);

                #region Verifying success banner message

                log.Info("\n\tVerifying success banner message" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

                iWait.Until(ExpectedConditions.ElementIsVisible(By.Id("btnOk")));
                //iWait.Until(d => d.FindElement(By.Id("Sucess_Message")).Equals(" Region Access added successfully"));
                Assert.AreEqual("Record(s) Inactivated successfully.".Trim(), driver.FindElement(By.Id("Sucess_Message")).Text.Trim());
                driver.FindElement(By.Id("btnOk")).Click();
                iWait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.ClassName("overlay")));

                #endregion

                #region Verifying details after inactivating Region

                log.Info("\n\tVerifying details after inactivating Region" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

                doc = NSoup.NSoupClient.Parse(driver.PageSource);
                rowListNsoup = doc.GetElementById("ContentPlaceHolder1_gvRegionAccessList").GetElementsByTag("tr");


                flag = false;

                rowcounter = 0;
                foreach (Element currentRow in rowListNsoup)
                {
                    Attributes attr = currentRow.Attributes;

                    //Row that have class="GridRowStyle" or class="AltGridStyle"
                    if (attr["class"].Equals("GridRowStyle") || attr["class"].Equals("AltGridStyle"))
                    {
                        log.Info("Row Counter :: " + rowcounter);
                        columData = currentRow.GetElementsByTag("td")[1].GetElementById("ContentPlaceHolder1_gvRegionAccessList_lblCountry_" + rowcounter).OwnText().Trim();


                        if (columData.Equals(regionName))
                        {
                            flag = true;


                            Assert.AreEqual(detailBeforeInactivate["IsCheckbox"], currentRow.GetElementsByTag("td")[0].GetElementById("ContentPlaceHolder1_gvRegionAccessList_chkSelect_" + rowcounter).Attributes["type"]);
                            Assert.AreEqual(detailBeforeInactivate["CountryName"], currentRow.GetElementsByTag("td")[1].GetElementById("ContentPlaceHolder1_gvRegionAccessList_lblCountry_" + rowcounter).OwnText().Trim());
                            Assert.AreEqual(detailBeforeInactivate["AccessType"], currentRow.GetElementsByTag("td")[2].GetElementById("ContentPlaceHolder1_gvRegionAccessList_lblRegionType_" + rowcounter).OwnText().Trim());

                            Assert.AreEqual(detailBeforeInactivate["RedirectionText"], currentRow.GetElementsByTag("td")[3].GetElementById("ContentPlaceHolder1_gvRegionAccessList_lblRedirectionTxt_" + rowcounter).OwnText().Trim());

                            Assert.AreEqual(detailBeforeInactivate["RedirectionURL"], currentRow.GetElementsByTag("td")[4].GetElementById("ContentPlaceHolder1_gvRegionAccessList_lblRedirectionUrl_" + rowcounter).OwnText().Trim());
                            Assert.AreEqual(detailBeforeInactivate["StartDate"], currentRow.GetElementsByTag("td")[5].GetElementById("ContentPlaceHolder1_gvRegionAccessList_lblStartDate_" + rowcounter).OwnText().Trim());
                            Assert.AreEqual(detailBeforeInactivate["EndDate"], currentRow.GetElementsByTag("td")[6].GetElementById("ContentPlaceHolder1_gvRegionAccessList_lblEndDate_" + rowcounter).OwnText().Trim());
                            Assert.AreEqual(detailBeforeInactivate["Notes"], currentRow.GetElementsByTag("td")[7].GetElementById("ContentPlaceHolder1_gvRegionAccessList_lblNotes_" + rowcounter).OwnText().Trim());

                            //write assert to check presence of edit as well as Status column
                            if (detailBeforeInactivate["Status"].Equals("Active"))
                            {
                                Assert.AreEqual("Inactive", currentRow.GetElementsByTag("td")[8].GetElementById("ContentPlaceHolder1_gvRegionAccessList_lblStatus_" + rowcounter).OwnText().Trim());
                                Assert.AreEqual(0, currentRow.GetElementsByTag("td")[9].GetElementsByTag("span").Count);
                            }
                            else if (detailBeforeInactivate["Status"].Equals("Inactive"))
                            {
                                Assert.AreEqual("Inactive", currentRow.GetElementsByTag("td")[8].GetElementById("ContentPlaceHolder1_gvRegionAccessList_lblStatus_" + rowcounter).OwnText().Trim());
                                Assert.AreEqual(0, currentRow.GetElementsByTag("td")[9].GetElementsByTag("span").Count);
                            }

                            //write assert to check Channel name is present of not
                            log.Info("Region is present on Manage" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());
                            break;
                        }
                        rowcounter++;
                    }
                }

                Assert.AreEqual(true, flag);   //IF THIS FAILS MEANS created Channel IS NOT displayed ON MANAGE page 

                log.Info("\n\tRegion is Inactivated successfully" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

                #endregion

                #endregion

                #region Activating the Region

                #region Storing details and Clicking on Activate button

                log.Info("\n\tStoring details and Clicking on Activate button" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

                //Using Nsoup here to parse the html table
                doc = NSoup.NSoupClient.Parse(driver.PageSource);
                rowListNsoup = doc.GetElementById("ContentPlaceHolder1_gvRegionAccessList").GetElementsByTag("tr");


                flag = false;

                rowcounter = 0;

                foreach (Element currentRow in rowListNsoup)
                {
                    Attributes attr = currentRow.Attributes;

                    //Row that have class="GridRowStyle" or class="AltGridStyle"
                    if (attr["class"].Equals("GridRowStyle") || attr["class"].Equals("AltGridStyle"))
                    {
                        columData = currentRow.GetElementsByTag("td")[1].GetElementById("ContentPlaceHolder1_gvRegionAccessList_lblCountry_" + rowcounter).OwnText().Trim();


                        if (columData.Equals(regionName))
                        {
                            flag = true;

                            detailBeforeActivate.Add("IsCheckbox", currentRow.GetElementsByTag("td")[0].GetElementById("ContentPlaceHolder1_gvRegionAccessList_chkSelect_" + rowcounter).Attributes["type"]);
                            detailBeforeActivate.Add("CountryName", currentRow.GetElementsByTag("td")[1].GetElementById("ContentPlaceHolder1_gvRegionAccessList_lblCountry_" + rowcounter).OwnText().Trim());
                            detailBeforeActivate.Add("AccessType", currentRow.GetElementsByTag("td")[2].GetElementById("ContentPlaceHolder1_gvRegionAccessList_lblRegionType_" + rowcounter).OwnText().Trim());

                            detailBeforeActivate.Add("RedirectionText", currentRow.GetElementsByTag("td")[3].GetElementById("ContentPlaceHolder1_gvRegionAccessList_lblRedirectionTxt_" + rowcounter).OwnText().Trim());

                            detailBeforeActivate.Add("RedirectionURL", currentRow.GetElementsByTag("td")[4].GetElementById("ContentPlaceHolder1_gvRegionAccessList_lblRedirectionUrl_" + rowcounter).OwnText().Trim());
                            detailBeforeActivate.Add("StartDate", currentRow.GetElementsByTag("td")[5].GetElementById("ContentPlaceHolder1_gvRegionAccessList_lblStartDate_" + rowcounter).OwnText().Trim());
                            detailBeforeActivate.Add("EndDate", currentRow.GetElementsByTag("td")[6].GetElementById("ContentPlaceHolder1_gvRegionAccessList_lblEndDate_" + rowcounter).OwnText().Trim());
                            detailBeforeActivate.Add("Notes", currentRow.GetElementsByTag("td")[7].GetElementById("ContentPlaceHolder1_gvRegionAccessList_lblNotes_" + rowcounter).OwnText().Trim());
                            detailBeforeActivate.Add("Status", currentRow.GetElementsByTag("td")[8].GetElementById("ContentPlaceHolder1_gvRegionAccessList_lblStatus_" + rowcounter).OwnText().Trim());

                            //write assert to check presence of edit and delete button
                            if (detailBeforeActivate["Status"].Equals("Active"))
                            {
                                Assert.AreEqual(true, currentRow.GetElementsByTag("td")[9].GetElementById("ContentPlaceHolder1_gvRegionAccessList_imgEdit_" + rowcounter).GetElementsByTag("img")[0].Attributes["src"].Contains("Edit.png"));
                            }
                            else if (detailBeforeActivate["Status"].Equals("Inactive"))
                            {
                                Assert.AreEqual(0, currentRow.GetElementsByTag("td")[9].GetElementsByTag("span").Count);
                            }

                            ClickManagePageCheckBox(rowcounter);

                            ClickActivateButton();


                            //write assert to check Channel name is present of not
                            log.Info("Region is present on Manage" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());
                            break;
                        }
                        rowcounter++;
                    }
                }

                Assert.AreEqual(true, flag);   //IF THIS FAILS MEANS created Channel IS NOT displayed ON MANAGE page 

                log.Info("\n\tRegion is activated");

                #endregion

                uf.isJqueryActive(driver);

                #region Verifying success banner message

                log.Info("\n\tVerifying success banner message" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

                iWait.Until(ExpectedConditions.ElementIsVisible(By.Id("btnOk")));
                //iWait.Until(d => d.FindElement(By.Id("Sucess_Message")).Equals(" Region Access added successfully"));
                Assert.AreEqual("Record(s) activated successfully.".Trim(), driver.FindElement(By.Id("Sucess_Message")).Text.Trim());
                driver.FindElement(By.Id("btnOk")).Click();
                iWait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.ClassName("overlay")));

                #endregion

                #region Verifying details after Activating Region

                log.Info("\n\tVerifying details after Activating Region" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

                //Using Nsoup here to parse the html table
                doc = NSoup.NSoupClient.Parse(driver.PageSource);
                rowListNsoup = doc.GetElementById("ContentPlaceHolder1_gvRegionAccessList").GetElementsByTag("tr");

                flag = false;

                rowcounter = 0;
                foreach (Element currentRow in rowListNsoup)
                {
                    Attributes attr = currentRow.Attributes;

                    //Row that have class="GridRowStyle" or class="AltGridStyle"
                    if (attr["class"].Equals("GridRowStyle") || attr["class"].Equals("AltGridStyle"))
                    {
                        log.Info("Row Counter :: " + rowcounter);
                        columData = currentRow.GetElementsByTag("td")[1].GetElementById("ContentPlaceHolder1_gvRegionAccessList_lblCountry_" + rowcounter).OwnText().Trim();


                        if (columData.Equals(regionName))
                        {
                            flag = true;


                            Assert.AreEqual(detailBeforeActivate["IsCheckbox"], currentRow.GetElementsByTag("td")[0].GetElementById("ContentPlaceHolder1_gvRegionAccessList_chkSelect_" + rowcounter).Attributes["type"]);
                            Assert.AreEqual(detailBeforeActivate["CountryName"], currentRow.GetElementsByTag("td")[1].GetElementById("ContentPlaceHolder1_gvRegionAccessList_lblCountry_" + rowcounter).OwnText().Trim());
                            Assert.AreEqual(detailBeforeActivate["AccessType"], currentRow.GetElementsByTag("td")[2].GetElementById("ContentPlaceHolder1_gvRegionAccessList_lblRegionType_" + rowcounter).OwnText().Trim());

                            Assert.AreEqual(detailBeforeActivate["RedirectionText"], currentRow.GetElementsByTag("td")[3].GetElementById("ContentPlaceHolder1_gvRegionAccessList_lblRedirectionTxt_" + rowcounter).OwnText().Trim());

                            Assert.AreEqual(detailBeforeActivate["RedirectionURL"], currentRow.GetElementsByTag("td")[4].GetElementById("ContentPlaceHolder1_gvRegionAccessList_lblRedirectionUrl_" + rowcounter).OwnText().Trim());
                            Assert.AreEqual(detailBeforeActivate["StartDate"], currentRow.GetElementsByTag("td")[5].GetElementById("ContentPlaceHolder1_gvRegionAccessList_lblStartDate_" + rowcounter).OwnText().Trim());
                            Assert.AreEqual(detailBeforeActivate["EndDate"], currentRow.GetElementsByTag("td")[6].GetElementById("ContentPlaceHolder1_gvRegionAccessList_lblEndDate_" + rowcounter).OwnText().Trim());
                            Assert.AreEqual(detailBeforeActivate["Notes"], currentRow.GetElementsByTag("td")[7].GetElementById("ContentPlaceHolder1_gvRegionAccessList_lblNotes_" + rowcounter).OwnText().Trim());

                            //write assert to check presence of edit as well as Status column
                            if (detailBeforeActivate["Status"].Equals("Active"))
                            {
                                Assert.AreEqual("Active", currentRow.GetElementsByTag("td")[8].GetElementById("ContentPlaceHolder1_gvRegionAccessList_lblStatus_" + rowcounter).OwnText().Trim());
                                Assert.AreEqual(true, currentRow.GetElementsByTag("td")[9].GetElementById("ContentPlaceHolder1_gvRegionAccessList_imgEdit_" + rowcounter).GetElementsByTag("img")[0].Attributes["src"].Contains("Edit.png"));
                            }
                            else if (detailBeforeActivate["Status"].Equals("Inactive"))
                            {
                                Assert.AreEqual("Active", currentRow.GetElementsByTag("td")[8].GetElementById("ContentPlaceHolder1_gvRegionAccessList_lblStatus_" + rowcounter).OwnText().Trim());
                                Assert.AreEqual(true, currentRow.GetElementsByTag("td")[9].GetElementById("ContentPlaceHolder1_gvRegionAccessList_imgEdit_" + rowcounter).GetElementsByTag("img")[0].Attributes["src"].Contains("Edit.png"));

                            }
                            //write assert to check Channel name is present of not
                            log.Info("Region is present on Manage" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());
                            break;
                        }
                        rowcounter++;
                    }
                }

                Assert.AreEqual(true, flag);   //IF THIS FAILS MEANS created Channel IS NOT displayed ON MANAGE page 

                log.Info("\n\tRegion is Activated successfully" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

                #endregion

                #endregion

                log.Info("TVAdmin_006_ActivateInactivateFunc completed" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());
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


            string redirectionURLLink = "https://www.google.com";

            try
            {
                log.Info("\nTVAdmin_007_MandatoryFieldValidation started" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());


                RedirectingToRegionAccessManagement();

                ClickOnCreateTab();

                #region Click on save button and verify inline message

                ClickSaveButton();

                //Verify the Country name inline message
                iWait.Until(ExpectedConditions.ElementIsVisible(By.Id("ContentPlaceHolder1_compval1")));
                Assert.AreEqual("Please select Country.", driver.FindElement(By.Id("ContentPlaceHolder1_compval1")).Text);

                //Verify the start Date inline message
                iWait.Until(ExpectedConditions.ElementIsVisible(By.Id("ContentPlaceHolder1_rfvStartDate")));
                Assert.AreEqual("Please enter Start Date.", driver.FindElement(By.Id("ContentPlaceHolder1_rfvStartDate")).Text);

                #endregion

                #region click on redirected button then save and verify inline message

                RedirectedRadioButton().Click();

                //click on Save button
                ClickSaveButton();

                //Verify the Redirection text inline message
                iWait.Until(ExpectedConditions.ElementIsVisible(By.Id("ContentPlaceHolder1_rfvtxtredirect")));
                Assert.AreEqual("Please enter Redirection Text.", driver.FindElement(By.Id("ContentPlaceHolder1_rfvtxtredirect")).Text);

                //Verify the Redirection Url inline message
                iWait.Until(ExpectedConditions.ElementIsVisible(By.Id("ContentPlaceHolder1_rfvtxtRedirectURL")));
                Assert.AreEqual("Please enter Redirection URL.", driver.FindElement(By.Id("ContentPlaceHolder1_rfvtxtRedirectURL")).Text);


                #endregion

                #region Fill all the details and verify inline messages removed

                //1.Verify the inline message is removed for country
                iWait.Until(ExpectedConditions.ElementIsVisible(By.Id("ContentPlaceHolder1_ddlCountry")));
                GetCountryDropdown().SelectByIndex(1);

                iWait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.Id("ContentPlaceHolder1_compval1")));
                Assert.AreEqual(false, driver.FindElement(By.Id("ContentPlaceHolder1_compval1")).Displayed);

                //2.Verify the inline message is removed for Redirection text
                RedirectionTextField().SendKeys("Testing RedirectionText inline message");

                ClickSaveButton();

                iWait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.Id("ContentPlaceHolder1_rfvtxtredirect")));

                Assert.AreEqual(false, driver.FindElement(By.Id("ContentPlaceHolder1_rfvtxtredirect")).Displayed);

                //3.Verify the inline message is removed for Redirection Url

                RedirectionURlField().SendKeys(redirectionURLLink);

                ClickSaveButton();

                iWait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.Id("ContentPlaceHolder1_rfvtxtRedirectURL")));
                Assert.AreEqual(false, driver.FindElement(By.Id("ContentPlaceHolder1_rfvtxtRedirectURL")).Displayed);


                //4.Verify the  inline message is removed for start date

                //click on end date check box
                GetEndDateCheckBox().Click();

                GetStartDatePicker().SendKeys(GetSystemDate(1.0));
                ClickSaveButton();
                uf.isJqueryActive(driver);

                iWait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.Id("ContentPlaceHolder1_rfvStartDate")));
                Assert.AreEqual(false, driver.FindElement(By.Id("ContentPlaceHolder1_rfvStartDate")).Displayed);


                //verify the inline message for end date
                iWait.Until(ExpectedConditions.ElementIsVisible(By.Id("ContentPlaceHolder1_lbltextEndDate")));
                Assert.AreEqual("Please enter Date.", driver.FindElement(By.Id("ContentPlaceHolder1_lbltextEndDate")).Text);

                //enter data in end date
                GetEndDatePicker().SendKeys(GetSystemDate(2.0));

                GetEndDateCheckBox().Click();

                //verify inline message for end date is removed
                iWait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.Id("ContentPlaceHolder1_lbltextEndDate")));
                Assert.AreEqual(false, driver.FindElement(By.Id("ContentPlaceHolder1_lbltextEndDate")).Displayed);




                #endregion

                log.Info("\nTVAdmin_007_MandatoryFieldValidation completed" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());
            }
            catch (Exception e)
            {

                log.Error(e.Message + "\n" + e.StackTrace + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());
                
                Assert.AreEqual(true, false);
            }


        }


        [Test]
        public void TVAdmin_008_EditSave()
        {
            String regionName = null;

            Dictionary<String, String> createdRegionDetail = new Dictionary<String, String>();
            Dictionary<String, String> detailBeforeEditSave = new Dictionary<String, String>();
            Dictionary<String, String> editedDetails = new Dictionary<String, String>();

            try
            {
                log.Info("\nTVAdmin_008_EditSave started" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

                RedirectingToRegionAccessManagement();

                OverlayWait();

                #region Decciding whether to create new region or not.

                List<String> manageCountryList = GetManageCountryList();

                if (manageCountryList.Count == 0)
                {
                    createdRegionDetail = CreateRegion(true);
                    regionName = createdRegionDetail["CountryName"];
                }
                else
                {
                    regionName = manageCountryList[0];
                }

                #endregion

                #region Store details of region that user want to edit and click on Edit icon

                log.Info("\n\tStoring details and before edit" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

                //Using Nsoup here to parse the html table
                Document doc = NSoup.NSoupClient.Parse(driver.PageSource);
                Elements rowListNsoup = doc.GetElementById("ContentPlaceHolder1_gvRegionAccessList").GetElementsByTag("tr");

                flag = false;

                int rowcounter = 0;

                foreach (Element currentRow in rowListNsoup)
                {
                    Attributes attr = currentRow.Attributes;

                    //Row that have class="GridRowStyle" or class="AltGridStyle"
                    if (attr["class"].Equals("GridRowStyle") || attr["class"].Equals("AltGridStyle"))
                    {
                        columData = currentRow.GetElementsByTag("td")[1].GetElementById("ContentPlaceHolder1_gvRegionAccessList_lblCountry_" + rowcounter).OwnText().Trim();


                        if (columData.Equals(regionName))
                        {
                            flag = true;

                            detailBeforeEditSave.Add("CountryName", currentRow.GetElementsByTag("td")[1].GetElementById("ContentPlaceHolder1_gvRegionAccessList_lblCountry_" + rowcounter).OwnText().Trim());
                            detailBeforeEditSave.Add("AccessType", currentRow.GetElementsByTag("td")[2].GetElementById("ContentPlaceHolder1_gvRegionAccessList_lblRegionType_" + rowcounter).OwnText().Trim());
                            detailBeforeEditSave.Add("RedirectionText", currentRow.GetElementsByTag("td")[3].GetElementById("ContentPlaceHolder1_gvRegionAccessList_lblRedirectionTxt_" + rowcounter).OwnText().Trim());
                            detailBeforeEditSave.Add("RedirectionURL", currentRow.GetElementsByTag("td")[4].GetElementById("ContentPlaceHolder1_gvRegionAccessList_lblRedirectionUrl_" + rowcounter).OwnText().Trim());
                            detailBeforeEditSave.Add("StartDate", currentRow.GetElementsByTag("td")[5].GetElementById("ContentPlaceHolder1_gvRegionAccessList_lblStartDate_" + rowcounter).OwnText().Trim());
                            detailBeforeEditSave.Add("EndDate", currentRow.GetElementsByTag("td")[6].GetElementById("ContentPlaceHolder1_gvRegionAccessList_lblEndDate_" + rowcounter).OwnText().Trim());
                            detailBeforeEditSave.Add("Notes", currentRow.GetElementsByTag("td")[7].GetElementById("ContentPlaceHolder1_gvRegionAccessList_lblNotes_" + rowcounter).Attributes["title"]);
                            detailBeforeEditSave.Add("Status", currentRow.GetElementsByTag("td")[8].GetElementById("ContentPlaceHolder1_gvRegionAccessList_lblStatus_" + rowcounter).OwnText().Trim());

                            ClickEditIcon(rowcounter);

                            break;
                        }
                        rowcounter++;
                    }
                }

                Assert.AreEqual(true, flag);   //IF THIS FAILS MEANS created Region IS NOT displayed ON MANAGE page 

                log.Info("\n\t Edit button is clicked" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

                #endregion


                #region Verify the pre-populated date in edit mode

                log.Info("\n\tVerify the pre-populated date in edit mode" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

                // waiting till Edit Region active
                iWait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.ClassName("overlay")));
                iWait.Until(d => d.FindElement(By.Id("createRegion")).GetAttribute("class").Equals("active"));

                Assert.AreEqual("Edit Regions", driver.FindElement(By.Id("CreateRegionAccessTab")).Text.Trim());

                // Verify the status and content of Country dropdown

                Assert.AreEqual(false, driver.FindElement(By.Id("ContentPlaceHolder1_ddlCountry")).Enabled);
                Assert.AreEqual(detailBeforeEditSave["CountryName"], GetCountryDropdown().SelectedOption.Text.Trim());

                Boolean isRestricted = detailBeforeEditSave["AccessType"].Equals("Restricted");

                if (isRestricted)
                {
                    Assert.AreEqual(true, IsElementChecked("ContentPlaceHolder1_rdoRegionAccessType_0"));
                    Assert.AreEqual(false, RedirectionTextField().Enabled);
                    Assert.AreEqual(false, RedirectionURlField().Enabled);
                    Assert.AreEqual(String.Empty, RedirectionTextField().GetAttribute("value"));
                    Assert.AreEqual(String.Empty, RedirectionURlField().GetAttribute("value"));
                }
                else
                {
                    Assert.AreEqual(true, IsElementChecked("ContentPlaceHolder1_rdoRegionAccessType_1"));
                    Assert.AreEqual(true, RedirectionTextField().Enabled);
                    Assert.AreEqual(true, RedirectionURlField().Enabled);
                    Assert.AreEqual(detailBeforeEditSave["RedirectionText"], RedirectionTextField().GetAttribute("value"));
                    Assert.AreEqual(detailBeforeEditSave["RedirectionURL"], RedirectionURlField().GetAttribute("value"));
                }

                // Verifying the status and value of Start date field
                log.Info("Start Date in Edit mode :: " + driver.FindElement(By.Id("ContentPlaceHolder1_txtstartDate")).GetAttribute("value"));
                Assert.AreEqual(false, GetStartDatePicker().Enabled);
                Assert.AreEqual(detailBeforeEditSave["StartDate"], GetStartDatePicker().GetAttribute("value"));


                if (detailBeforeEditSave["EndDate"].Equals(String.Empty))
                {
                    Assert.AreEqual(false, IsElementChecked("ContentPlaceHolder1_chkEndDate"));
                    Assert.AreEqual(false, GetEndDatePicker().Enabled);
                }
                else
                {
                    Assert.AreEqual(true, IsElementChecked("ContentPlaceHolder1_chkEndDate"));
                    Assert.AreEqual(true, GetEndDatePicker().Enabled);
                    Assert.AreEqual(detailBeforeEditSave["EndDate"], GetEndDatePicker().GetAttribute("value"));
                }

                Assert.AreEqual(detailBeforeEditSave["Notes"], GetNoteField().GetAttribute("value"));

                log.Info("\n\tVerification in edit mode is completed" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

                #endregion


                #region Editing the details in edit mode

                log.Info("\n\tEditing is started");


                editedDetails.Add("CountryName", GetCountryDropdown().SelectedOption.Text.Trim());

                if (isRestricted)
                {

                    editedDetails.Add("AccessType", "Redirect");

                    RedirectedRadioButton().Click();

                    //  driver.FindElement(By.Id("ContentPlaceHolder1_rdoRegionAccessType_1")).Click();
                    String redirectionText = "Redirection Text";

                    driver.FindElement(By.Id("ContentPlaceHolder1_txtRedirect")).SendKeys(redirectionText);
                    editedDetails.Add("RedirectionText", redirectionText);

                    String redirectionURL = "http://www.redirectionURL.com";
                    RedirectionURlField().SendKeys(redirectionURL);
                    //  driver.FindElement(By.Id("ContentPlaceHolder1_txtRedirectURL")).SendKeys(redirectionURL);
                    editedDetails.Add("RedirectionURL", redirectionURL);

                }
                else
                {

                    editedDetails.Add("AccessType", driver.FindElement(By.Id("ContentPlaceHolder1_rdoRegionAccessType")).
                         FindElement(By.TagName("tr")).
                         FindElements(By.TagName("td"))[0].
                         FindElement(By.TagName("label")).
                         Text.Trim());

                    driver.FindElement(By.Id("ContentPlaceHolder1_rdoRegionAccessType_0")).Click();
                    editedDetails.Add("RedirectionText", String.Empty);
                    editedDetails.Add("RedirectionURL", String.Empty);
                }

                editedDetails.Add("StartDate", GetStartDatePicker().GetAttribute("value"));

                if (detailBeforeEditSave["EndDate"].Equals(String.Empty))
                {
                    GetEndDateCheckBox().Click();
                    // driver.FindElement(By.Id("ContentPlaceHolder1_chkEndDate")).Click();

                    string strDate = GetStartDatePicker().GetAttribute("value").ToString();

                    DateTime dt = DateTime.ParseExact(strDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);

                    DateTime dte = dt.AddDays(2.0);
                    string enddate = dte.ToString("dd/MM/yyyy");

                    log.Info("End Date  " + dte.ToString("dd/MM/yyyy"));

                    GetEndDatePicker().SendKeys(enddate);

                    editedDetails.Add("EndDate", GetEndDatePicker().GetAttribute("value"));
                }
                else
                {
                    GetEndDateCheckBox().Click();
                    // driver.FindElement(By.Id("ContentPlaceHolder1_chkEndDate")).Click();
                    editedDetails.Add("EndDate", String.Empty);
                }

                String noteContent = " Here we are editing the region details and entering some text in note field";
                GetNoteField().Clear();


                GetNoteField().SendKeys(noteContent);

                editedDetails.Add("Notes", noteContent);

                ClickSaveButton();

                uf.isJqueryActive(driver);

                log.Info("\tEditing is done" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

                #endregion

                #region Verifying success banner message

                iWait.Until(ExpectedConditions.ElementIsVisible(By.Id("btnOk")));
                Assert.AreEqual(" Record Updated Successfully".Trim(), driver.FindElement(By.Id("Sucess_Message")).Text.Trim());
                driver.FindElement(By.Id("btnOk")).Click();
                iWait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.Id("btnOk")));

                #endregion

                #region Verifying Edited region in Manage page

                //Using Nsoup here to parse the html table
                doc = NSoup.NSoupClient.Parse(driver.PageSource);
                rowListNsoup = doc.GetElementById("ContentPlaceHolder1_gvRegionAccessList").GetElementsByTag("tr");

                flag = false;


                #region Applying_Assert_On_Manage_Page

                flag = false;

                rowcounter = 0;
                foreach (Element currentRow in rowListNsoup)
                {
                    Attributes attr = currentRow.Attributes;

                    //Row that have class="GridRowStyle" or class="AltGridStyle"
                    if (attr["class"].Equals("GridRowStyle") || attr["class"].Equals("AltGridStyle"))
                    {
                        log.Info("Row Counter :: " + rowcounter);
                        columData = currentRow.GetElementsByTag("td")[1].GetElementById("ContentPlaceHolder1_gvRegionAccessList_lblCountry_" + rowcounter).OwnText().Trim();


                        if (columData.Equals(regionName))
                        {
                            flag = true;

                            // assert to check checkbox is displayed
                            Assert.AreEqual("checkbox", currentRow.GetElementsByTag("td")[0].GetElementById("ContentPlaceHolder1_gvRegionAccessList_chkSelect_" + rowcounter).Attributes["type"]);


                            //Country Name
                            Assert.AreEqual(editedDetails["CountryName"], currentRow.GetElementsByTag("td")[1].GetElementById("ContentPlaceHolder1_gvRegionAccessList_lblCountry_" + rowcounter).OwnText().Trim());


                            // Access Type Assertion
                            Assert.AreEqual(editedDetails["AccessType"], currentRow.GetElementsByTag("td")[2].GetElementById("ContentPlaceHolder1_gvRegionAccessList_lblRegionType_" + rowcounter).OwnText().Trim());

                            // assert to check  Redirection Text
                            Assert.AreEqual(editedDetails["RedirectionText"], currentRow.GetElementsByTag("td")[3].GetElementById("ContentPlaceHolder1_gvRegionAccessList_lblRedirectionTxt_" + rowcounter).OwnText().Trim());

                            // Assert to verify Redirection URl
                            Assert.AreEqual(editedDetails["RedirectionURL"], currentRow.GetElementsByTag("td")[4].GetElementById("ContentPlaceHolder1_gvRegionAccessList_lblRedirectionUrl_" + rowcounter).OwnText().Trim());

                            // Assert to verify Start Date
                            Assert.AreEqual(editedDetails["StartDate"], currentRow.GetElementsByTag("td")[5].GetElementById("ContentPlaceHolder1_gvRegionAccessList_lblStartDate_" + rowcounter).OwnText().Trim());

                            // Assert to verify End Date
                            Assert.AreEqual(editedDetails["EndDate"], currentRow.GetElementsByTag("td")[6].GetElementById("ContentPlaceHolder1_gvRegionAccessList_lblEndDate_" + rowcounter).OwnText().Trim());

                            // Assert to verify the Notes
                            Assert.AreEqual(editedDetails["Notes"], currentRow.GetElementsByTag("td")[7].GetElementById("ContentPlaceHolder1_gvRegionAccessList_lblNotes_" + rowcounter).Attributes["title"]);


                            // Assert to verify status
                            Assert.AreEqual(detailBeforeEditSave["Status"], currentRow.GetElementsByTag("td")[8].GetElementById("ContentPlaceHolder1_gvRegionAccessList_lblStatus_" + rowcounter).OwnText().Trim());

                            //write assert to check presence of edit and delete button
                            if (detailBeforeEditSave["Status"].Equals("Active"))
                            {
                                Assert.AreEqual(true, currentRow.GetElementsByTag("td")[9].GetElementById("ContentPlaceHolder1_gvRegionAccessList_imgEdit_" + rowcounter).GetElementsByTag("img")[0].Attributes["src"].Contains("Edit.png"));
                            }
                            else if (detailBeforeEditSave["Status"].Equals("Inactive"))
                            {
                                Assert.AreEqual(0, currentRow.GetElementsByTag("td")[9].GetElementsByTag("span").Count);
                            }


                            //write assert to check Channel name is present of not
                            log.Info("Region is present on Manage" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());
                            break;
                        }
                        rowcounter++;
                    }
                }

                #endregion

                Assert.AreEqual(true, flag);   //IF THIS FAILS MEANS created Region IS NOT displayed ON MANAGE page 



                #endregion

                log.Info("\nTVAdmin_008_EditSave started" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());
            }
            catch (Exception e)
            {

                log.Error(e.Message + "\n" + e.StackTrace + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());
                
                Assert.AreEqual(true, false);
            }


        }

        [Test]
        public void TVWeb_009_CountryList_Validation()
        {
            try
            {
                log.Info("TVWeb_009_CountryList_Validation started" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

                string countryListName = cf.readingXMLFile("AdminPortal", "Regional_Access", "countryName", "Config.xml");

                string[] countryArrayList = countryListName.Split('#');


                RedirectingToRegionAccessManagement();

                ClickOnCreateTab();

                SelectElement country = new SelectElement(driver.FindElement(By.Id("ContentPlaceHolder1_ddlCountry")));
                IList<IWebElement> countryList = country.Options;


                int j = 0;

                foreach (IWebElement temp in countryList)
                {
                    string countryname = temp.Text;
                    Assert.AreEqual(countryArrayList[j].Trim(), countryname);
                    j++;
                }

                log.Info("TVWeb_009_CountryList_Validation started" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());
            }

            catch (Exception e)
            {

                log.Error(e.Message + "\n" + e.StackTrace + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

                Assert.AreEqual(true, false);
            }
        }

        [Test]
        [TestCaseSource("UrlDataSource")]
        public void TVAdmin_010_UrlAddressValidation(DataRow testData)
        {
            try
            {

                log.Info("Tesing TVAdmin_010_UrlAddressValidation Url " + counter + " Started");
                IWebElement save = null;

                RedirectingToRegionAccessManagement();

                ClickOnCreateTab();

                //Click on  Access type-Redirect
                driver.FindElement(By.Id("ContentPlaceHolder1_rdoRegionAccessType_1")).Click();

                // Clear Data from Redirection URl
                driver.FindElement(By.Id("ContentPlaceHolder1_txtRedirectURL")).Clear();

                //Enter Invalid URL
                driver.FindElement(By.Id("ContentPlaceHolder1_txtRedirectURL")).SendKeys(testData["Column1"].ToString());

                //Click on Save Button
                save = driver.FindElement(By.Id("ContentPlaceHolder1_btnSave"));
                save.Click();

                //Verify the inline Error message
                iWait.Until(ExpectedConditions.ElementIsVisible(By.Id("ContentPlaceHolder1_RegularExpressionValidator1")));
                Assert.AreEqual("Please Enter Valid URL.", driver.FindElement(By.Id("ContentPlaceHolder1_RegularExpressionValidator1")).Text);


                log.Info("Test TVAdmin_010_UrlAddressValidation Url " + counter++ + " Completed");
            }


            catch (Exception e)
            {

                log.Error(e.Message + "\n" + e.StackTrace + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

                Assert.AreEqual(true, false);
            }

        }

        //This function read the data from Excel file for Email negative test
       // public static IEnumerable<object> UrlDataSource
       // {
        //    get
        //    {
            //    int start_cnt = 0;

            //    log.Info("inside EmailDataSsource");

            //    string ExcelDataPath = Environment.CurrentDirectory + "\\TestData_AdminPortal\\" + "NormalDataset_IET_Admin.xlsx";

            //    FileStream stream = File.Open(ExcelDataPath, FileMode.Open, FileAccess.Read);

            //    // 2. Reading from a OpenXml Excel file (2007 format; *.xlsx)
            //    Excel.IExcelDataReader excelReader = Excel.ExcelReaderFactory.CreateOpenXmlReader(stream);

            //    // 3. DataSet - The result of each spreadsheet will be created in the result.Tables

            //    DataSet result = excelReader.AsDataSet();

            //    // 4. DataSet - Create column names from first row
            //    excelReader.IsFirstRowAsColumnNames = true;

            //    System.Data.DataTable currentSheet = result.Tables[1];

            //    foreach (DataRow dr in currentSheet.Rows)
            //    {

            //        if (start_cnt > 0)
            //        {
            //            yield return dr;
            //        }
            //        start_cnt++;
            //    }

            //    excelReader.Close();
           // }
       // }

      
        [TestFixtureTearDown]
        public void TearDown()
        {
            st.Chrome_TearDown(driver, log);
        }
      
    }
}
