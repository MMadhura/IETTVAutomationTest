using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.IE;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Utility_Classes;
using Utilities.Config;
using System.Data;
using Microsoft.Office.Interop.Excel;
using System.Drawing;
using System.IO;
using Utilities.Object_Repository;
using log4net;
using log4net.Config;
using System.Reflection;
using System.Diagnostics;
using Microsoft.Expression.Encoder.ScreenCapture;
using IETTVWebportal.Reusable_Functions;

namespace IETTVWebportal.Chrome
{
    
    [TestFixture]
    public class Chrome_NonIETMemberRegistration
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        //List to store data that need to put in excel file
        List<String> excelDataList = new List<String>();                                 

        String selectedTitle = "", selectedCountry = "";

        internal IWebDriver driver = null;

        IWebElement element = null;
        
        int currentRecord = 2;

        IJavaScriptExecutor executor;

        IWait<IWebDriver> iWait;

        string driverName = "", driverPath, appURL;

        Utility_Functions uf = new Utility_Functions();                    // Instantiate object for Utility Functons

        Configuration cf = new Configuration();                            // Instantiate object for Configuration

        Object_Repository_Class OR = new Object_Repository_Class();

        Chrome_WebSetupTearDown st = new Chrome_WebSetupTearDown();        // Instantiate object for Chrome Setup Teardown

       
        [TestFixtureSetUp]
        public void FixtureSetUp()
        {
            log4net.Config.XmlConfigurator.Configure(new FileInfo(AppDomain.CurrentDomain.BaseDirectory + "WebPortal.config"));

            log.Info("Inside Fixture Setup of chrome - NonIETmemberRegistration Test" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

            uf.CreateOrReplaceVideoFolder();

            string baseDir = AppDomain.CurrentDomain.BaseDirectory;                         // Get path till Base Directory

            driverName = "webdriver.chrome.driver";                                        // Driver name for Chrome

            driverPath = baseDir + "/chromedriver.exe";                                    // Path for ChromeDriver

            System.Environment.SetEnvironmentVariable(driverName, driverPath);

            driver = new ChromeDriver();                                                  // Initialize chrome driver   

            executor = (IJavaScriptExecutor)driver;

            iWait = new WebDriverWait(driver, TimeSpan.FromSeconds(60));

        }

        [SetUp]
        public void SetUp()
        {
            appURL = st.Chrome_Setup(driver, log, executor);                                // Calling Chrome Setup
        }

         //This function click on login link and click on registration link
        public void RegistrationPage()
        {
            //clicking on Login button
            iWait = new OpenQA.Selenium.Support.UI.WebDriverWait(driver, TimeSpan.FromSeconds(30.00));
            iWait.Until(ExpectedConditions.ElementToBeClickable(By.LinkText(OR.readingXMLFile("NonIETMemberRegistration", "LoginLink", "TVWebPortalOR.xml"))));
           
            element = driver.FindElement(By.LinkText(OR.readingXMLFile("NonIETMemberRegistration", "LoginLink", "TVWebPortalOR.xml")));                            
            executor.ExecuteScript("arguments[0].click();", element);

            //clicking on Registration link
            iWait = new OpenQA.Selenium.Support.UI.WebDriverWait(driver, TimeSpan.FromSeconds(30.00));
            iWait.Until(ExpectedConditions.ElementToBeClickable(By.LinkText(OR.readingXMLFile("IETMemberRegistration", "NotRegistered", "TVWebPortalOR.xml"))));
            
            element = driver.FindElement(By.LinkText(OR.readingXMLFile("NonIETMemberRegistration", "RegistrationLink", "TVWebPortalOR.xml")));        
            executor.ExecuteScript("arguments[0].click();", element);
        }


        [Test]
        public void TVWeb_001_Registration()
        {
            try
            {
                log.Info("Registration Test started" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

                prepareData();


                //Function to go to registration page 
                RegistrationPage();

                WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(60));

                wait.Until(ExpectedConditions.ElementExists(By.Id(OR.readingXMLFile("NonIETMemberRegistration", "NonMemberButton", "TVWebPortalOR.xml"))));
                log.Info("wait is over" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

                //click on No button
                element = driver.FindElement(By.Id(OR.readingXMLFile("NonIETMemberRegistration", "NonMemberButton", "TVWebPortalOR.xml")));                                 //Clicking on No button for registration of Not IET member
                executor.ExecuteScript("arguments[0].click();", element);


                #region  filling all the fields at registraion page


                element = wait.Until<IWebElement>((d) =>
                {
                    return driver.FindElement(By.Id(OR.readingXMLFile("NonIETMemberRegistration", "Title", "TVWebPortalOR.xml")));
                });

                Thread.Sleep(3000);

                SelectElement titleSelector = new SelectElement(element);
                titleSelector.SelectByIndex(Convert.ToInt32(excelDataList[0]));
                selectedTitle = titleSelector.SelectedOption.Text;

                driver.FindElement(By.Id(OR.readingXMLFile("NonIETMemberRegistration", "Forenames", "TVWebPortalOR.xml"))).SendKeys(excelDataList[1]);
                Thread.Sleep(1000);

                driver.FindElement(By.Id(OR.readingXMLFile("NonIETMemberRegistration", "Surname", "TVWebPortalOR.xml"))).SendKeys(excelDataList[2]);
                Thread.Sleep(1000);

                //storing forename and surname
                String username_surname = driver.FindElement(By.Id(OR.readingXMLFile("NonIETMemberRegistration", "Forenames", "TVWebPortalOR.xml"))).GetAttribute("value") + " " + driver.FindElement(By.Id(OR.readingXMLFile("NonIETMemberRegistration", "Surname", "TVWebPortalOR.xml"))).GetAttribute("value"); 

                driver.FindElement(By.Id(OR.readingXMLFile("NonIETMemberRegistration", "EmailAddress", "TVWebPortalOR.xml"))).SendKeys(excelDataList[3]);
                Thread.Sleep(1000);

                driver.FindElement(By.Id(OR.readingXMLFile("NonIETMemberRegistration", "ConfirmEmailAddress", "TVWebPortalOR.xml"))).SendKeys(excelDataList[3]);
                Thread.Sleep(1000);

                driver.FindElement(By.Id(OR.readingXMLFile("NonIETMemberRegistration", "Username", "TVWebPortalOR.xml"))).SendKeys(excelDataList[4]);
                Thread.Sleep(1000);

                driver.FindElement(By.Id(OR.readingXMLFile("NonIETMemberRegistration", "Password", "TVWebPortalOR.xml"))).SendKeys(excelDataList[5]);
                Thread.Sleep(1000);

                driver.FindElement(By.Id(OR.readingXMLFile("NonIETMemberRegistration", "ConfirmPassword", "TVWebPortalOR.xml"))).SendKeys(excelDataList[6]);
                Thread.Sleep(1000);

                element = driver.FindElement(By.Id(OR.readingXMLFile("NonIETMemberRegistration", "Country", "TVWebPortalOR.xml")));
                SelectElement countrySelector = new SelectElement(element);
                countrySelector.SelectByIndex(Convert.ToInt32(excelDataList[7]));

                selectedCountry = countrySelector.SelectedOption.Text;
                Thread.Sleep(1000);


                #endregion

                //Click on continue button
                element = driver.FindElement(By.Id(OR.readingXMLFile("NonIETMemberRegistration", "ContinueButton", "TVWebPortalOR.xml")));                                 //Clicking on Continue button for registration of Not IET member
                executor.ExecuteScript("arguments[0].click();", element);

                WebDriverWait waitStyle = new WebDriverWait(driver, TimeSpan.FromSeconds(20));
                waitStyle.Until(d => d.FindElement(By.Id(OR.readingXMLFile("NonIETMemberRegistration", "BannerMessage", "TVWebPortalOR.xml"))).Text.Equals("You have registered successfully.OK"));

                element = driver.FindElement(By.Id(OR.readingXMLFile("NonIETMemberRegistration", "BannerMessage", "TVWebPortalOR.xml")));

                String register_Successful_Message = element.Text;


                //Register successful message is verified                             
                Assert.AreEqual("You have registered successfully.OK", register_Successful_Message);

                element = driver.FindElement(By.Id(OR.readingXMLFile("NonIETMemberRegistration", "BannerMessageOkButton", "TVWebPortalOR.xml")));
                executor.ExecuteScript("arguments[0].click();", element);

                //write/Store into xml username and password
                cf.writingIntoXML("AdminPortal", "NonIETRegistration", "UserName", excelDataList[4], "SysConfig.xml");
                cf.writingIntoXML("AdminPortal", "NonIETRegistration", "Password", excelDataList[5], "SysConfig.xml");


                //inorder to test this we need 395 server access

                Thread.Sleep(5000);

                //Re-login with the user which is registered and getting data from the excel file lastrecord

                iWait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector(OR.readingXMLFile("NonIETMemberRegistration", "PleaseWaitLoader", "TVWebPortalOR.xml"))));                                 //Waiting for please wait option

                log.Info("Please Wait loader is over" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

                wait.Until(ExpectedConditions.ElementExists(By.Id(OR.readingXMLFile("NonIETMemberRegistration", "UserName", "TVWebPortalOR.xml"))));

                driver.FindElement(By.Id(OR.readingXMLFile("NonIETMemberRegistration", "UserName", "TVWebPortalOR.xml"))).SendKeys(excelDataList[4]);
                Thread.Sleep(1000);

                driver.FindElement(By.Id(OR.readingXMLFile("NonIETMemberRegistration", "Password", "TVWebPortalOR.xml"))).SendKeys(excelDataList[5]);
                Thread.Sleep(1000);

                element = wait.Until<IWebElement>((d) =>
            {
                return driver.FindElement(By.ClassName(OR.readingXMLFile("NonIETMemberRegistration", "LoginButton", "TVWebPortalOR.xml")));
            });

                element.Click();


                //Terms and condition page-Positive test
                element = wait.Until<IWebElement>((d) =>
                {
                    return driver.FindElement(By.ClassName(OR.readingXMLFile("NonIETMemberRegistration", "AcceptButton", "TVWebPortalOR.xml")));                                //accept button of T&C
                });
                executor = (IJavaScriptExecutor)driver;
                executor.ExecuteScript("arguments[0].click();", element);

                iWait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector(OR.readingXMLFile("NonIETMemberRegistration", "button.ok_btn_size", "TVWebPortalOR.xml"))));  // Waiting for Popup window to appear after clicking on accept button

                IList<IWebElement> btnOK = driver.FindElements(By.CssSelector(OR.readingXMLFile("NonIETMemberRegistration", "button.ok_btn_size", "TVWebPortalOR.xml")));

                element = btnOK.ElementAt(0);

                executor.ExecuteScript("arguments[0].click();", element);

                element = wait.Until<IWebElement>((d) =>
                {
                    return driver.FindElement(By.Id(OR.readingXMLFile("NonIETMemberRegistration", "Message", "TVWebPortalOR.xml")));                                         //getting username who logged-in 
                });

                String welcome_user_name = element.Text;

                //validating username and surname who logged-in
                Assert.AreEqual(welcome_user_name, "Welcome: " + username_surname);

                //click on my IET dropdown
                element = driver.FindElement(By.Id(OR.readingXMLFile("NonIETMemberRegistration", "MyIETDropDown", "TVWebPortalOR.xml")));                                       //Clicking myIET dropdown 
                executor.ExecuteScript("arguments[0].click();", element);

                //Click on log out link
                element = driver.FindElement(By.Id(OR.readingXMLFile("NonIETMemberRegistration", "LogoutLink", "TVWebPortalOR.xml")));                                     //clicking on log out link
                executor.ExecuteScript("arguments[0].click();", element);

                log.Info("Registration Test Completed" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());
            }
            catch (Exception e)
            {
                log.Error(e.Message + "\n" + e.StackTrace + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

                Assert.AreEqual(true, false);

            }
        }


        [Test]
        public void TVWeb_002_Existing_Email_Address_Validation()
        {
            try
            {
                log.Info("Existing_Email_Address_Validation Test started" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

                TVWeb_001_Registration();    //registration process

                RegistrationPage();      //funtion to redirect to registration page

                WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(60));
                wait.Until(ExpectedConditions.ElementExists(By.Id(OR.readingXMLFile("NonIETMemberRegistration", "NonMemberButton", "TVWebPortalOR.xml"))));
                log.Info("wait is over after No button" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());


                //click on NO button
                element = driver.FindElement(By.Id(OR.readingXMLFile("NonIETMemberRegistration", "NonMemberButton", "TVWebPortalOR.xml")));                                //Clicking on NO button
                executor.ExecuteScript("arguments[0].click();", element);


                //Read last record data and put into UI-Registration Page
                element = wait.Until<IWebElement>((d) =>
                {
                    return driver.FindElement(By.Id(OR.readingXMLFile("NonIETMemberRegistration", "Title", "TVWebPortalOR.xml")));
                });
                Thread.Sleep(3000);
                SelectElement titleSelector = new SelectElement(element);
                titleSelector.SelectByIndex(Convert.ToInt32(excelDataList[0]));
                selectedTitle = titleSelector.SelectedOption.Text;

                driver.FindElement(By.Id(OR.readingXMLFile("NonIETMemberRegistration", "Forenames", "TVWebPortalOR.xml"))).SendKeys(excelDataList[1]);
                Thread.Sleep(1000);

                driver.FindElement(By.Id(OR.readingXMLFile("NonIETMemberRegistration", "Surname", "TVWebPortalOR.xml"))).SendKeys(excelDataList[2]);
                Thread.Sleep(1000);

                String username_surname = driver.FindElement(By.Id(OR.readingXMLFile("NonIETMemberRegistration", "Forenames", "TVWebPortalOR.xml"))).GetAttribute("value") + " " + driver.FindElement(By.Id(OR.readingXMLFile("NonIETMemberRegistration", "Surname", "TVWebPortalOR.xml"))).GetAttribute("value");  //storing forename and surname

                driver.FindElement(By.Id(OR.readingXMLFile("NonIETMemberRegistration", "EmailAddress", "TVWebPortalOR.xml"))).SendKeys(excelDataList[3]);
                Thread.Sleep(1000);

                driver.FindElement(By.Id(OR.readingXMLFile("NonIETMemberRegistration", "ConfirmEmailAddress", "TVWebPortalOR.xml"))).SendKeys(excelDataList[3]);
                Thread.Sleep(1000);

                driver.FindElement(By.Id(OR.readingXMLFile("NonIETMemberRegistration", "Username", "TVWebPortalOR.xml"))).SendKeys("emailAddressValidation");
                Thread.Sleep(1000);

                driver.FindElement(By.Id(OR.readingXMLFile("NonIETMemberRegistration", "Password", "TVWebPortalOR.xml"))).SendKeys(excelDataList[5]);
                Thread.Sleep(1000);

                driver.FindElement(By.Id(OR.readingXMLFile("NonIETMemberRegistration", "ConfirmPassword", "TVWebPortalOR.xml"))).SendKeys(excelDataList[6]);
                Thread.Sleep(1000);

                element = driver.FindElement(By.Id(OR.readingXMLFile("NonIETMemberRegistration", "Country", "TVWebPortalOR.xml")));
                SelectElement countrySelector = new SelectElement(element);
                countrySelector.SelectByIndex(Convert.ToInt32(excelDataList[7]));
                selectedCountry = countrySelector.SelectedOption.Text;
                Thread.Sleep(1000);

                //Click on Continue button
                element = driver.FindElement(By.Id(OR.readingXMLFile("NonIETMemberRegistration", "ContinueButton", "TVWebPortalOR.xml")));                                 //Clicking on Continue button for registration of Not IET member
                executor.ExecuteScript("arguments[0].click();", element);

                //Register successful banner message
                WebDriverWait waitStyle = new WebDriverWait(driver, TimeSpan.FromSeconds(20));
                waitStyle.Until(d => d.FindElement(By.Id(OR.readingXMLFile("NonIETMemberRegistration", "BannerMessage", "TVWebPortalOR.xml"))).Text.Equals("EmailAddress already present.OK"));

                element = driver.FindElement(By.Id(OR.readingXMLFile("NonIETMemberRegistration", "BannerMessage", "TVWebPortalOR.xml")));

                String email_already_Message = element.Text;

                Assert.AreEqual("EmailAddress already present.OK", email_already_Message);

                //click on Ok button of banner message
                element = driver.FindElement(By.Id(OR.readingXMLFile("NonIETMemberRegistration", "BannerMessageOkButton", "TVWebPortalOR.xml")));
               
                executor.ExecuteScript("arguments[0].click();", element);
                log.Info("Existing_Email_Address_Validation test completed" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());
            }
            catch (Exception e)
            {
                log.Error(e.Message + "\n" + e.StackTrace + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());
                Assert.AreEqual(true, false);

            }
        }

         
        public void writingIntoexcel(Boolean result, int currentRecord, String fileName, String SheetName)
        {
            #region Getting_Excel_Control
            Microsoft.Office.Interop.Excel.Application excelApp = new Microsoft.Office.Interop.Excel.Application();
            excelApp.Visible = false;
            string ExcelDataPath = Environment.CurrentDirectory + "\\TestData_TV\\" + fileName;
            Workbook workBook = excelApp.Workbooks.Open(ExcelDataPath, 0, false, 5, "", "", false, XlPlatform.xlWindows, "", true, false, 0, true, false, false);
            Sheets Sheets = workBook.Worksheets;
            Worksheet currentSheet = null;
            currentSheet = (Worksheet)Sheets.get_Item(SheetName);
 
            #endregion

            //As per result writing pass and fail in the Excel record
            if (result)
                currentSheet.Cells[currentRecord, 11] = "Pass";
            else if (!result)
                currentSheet.Cells[currentRecord, 11] = "Fail";

            workBook.Save();
            workBook.Close();
            excelApp.Quit();

        }

        [Test]
        public void TVWeb_004_Email_Address_Matching_Validation()  
        {
            try
            {
                log.Info("Email_Address_Matching_Validation test Started" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());
                RegistrationPage();              //calling registration Process

                WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(60));
                wait.Until(ExpectedConditions.ElementExists(By.Id(OR.readingXMLFile("NonIETMemberRegistration", "NonMemberButton", "TVWebPortalOR.xml"))));
                log.Info("wait is over" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

                element = driver.FindElement(By.Id(OR.readingXMLFile("NonIETMemberRegistration", "NonMemberButton", "TVWebPortalOR.xml")));                                     //Clicking on No button for registration of Not IET member
                executor.ExecuteScript("arguments[0].click();", element);

                #region filling all the data field in registration page
                element = wait.Until<IWebElement>((d) =>
                {
                    return driver.FindElement(By.Id(OR.readingXMLFile("NonIETMemberRegistration", "Title", "TVWebPortalOR.xml")));
                });

                Thread.Sleep(3000);

                SelectElement titleSelector = new SelectElement(element);
                titleSelector.SelectByIndex(2);

                driver.FindElement(By.Id(OR.readingXMLFile("NonIETMemberRegistration", "Forenames", "TVWebPortalOR.xml"))).SendKeys("John");
                driver.FindElement(By.Id(OR.readingXMLFile("NonIETMemberRegistration", "Surname", "TVWebPortalOR.xml"))).SendKeys("Miller");
                driver.FindElement(By.Id(OR.readingXMLFile("NonIETMemberRegistration", "EmailAddress", "TVWebPortalOR.xml"))).SendKeys("JohnMillerx123@gmail.com");
                driver.FindElement(By.Id(OR.readingXMLFile("NonIETMemberRegistration", "ConfirmEmailAddress", "TVWebPortalOR.xml"))).SendKeys("JohnMiller12345@gmail.com");
                driver.FindElement(By.Id(OR.readingXMLFile("NonIETMemberRegistration", "Username", "TVWebPortalOR.xml"))).SendKeys("Johnxmiller");
                driver.FindElement(By.Id(OR.readingXMLFile("NonIETMemberRegistration", "Password", "TVWebPortalOR.xml"))).SendKeys("Rave1234");
                driver.FindElement(By.Id(OR.readingXMLFile("NonIETMemberRegistration", "ConfirmPassword", "TVWebPortalOR.xml"))).SendKeys("Rave1234");

                element = driver.FindElement(By.Id(OR.readingXMLFile("NonIETMemberRegistration", "Country", "TVWebPortalOR.xml")));
                SelectElement countrySelector = new SelectElement(element);
                countrySelector.SelectByIndex(3);
                #endregion

                //click on continue button
                element = driver.FindElement(By.Id(OR.readingXMLFile("NonIETMemberRegistration", "ContinueButton", "TVWebPortalOR.xml")));                                 //Clicking on Continue button for registration of Not IET member
                executor.ExecuteScript("arguments[0].click();", element);

                WebDriverWait waitStyle = new WebDriverWait(driver, TimeSpan.FromSeconds(20));
                waitStyle.Until(d => d.FindElement(By.Id(OR.readingXMLFile("NonIETMemberRegistration", "BannerMessage", "TVWebPortalOR.xml"))).Text.Equals("Email address do not matchOK"));

                //banner message is verified
                element = driver.FindElement(By.Id(OR.readingXMLFile("NonIETMemberRegistration", "BannerMessage", "TVWebPortalOR.xml")));

                String Donotmatch_email_message = element.Text;

                Assert.AreEqual(Donotmatch_email_message, "Email address do not matchOK");

                //Clicking on Ok button of banner message
                element = driver.FindElement(By.Id(OR.readingXMLFile("NonIETMemberRegistration", "BannerMessageOkButton", "TVWebPortalOR.xml")));
                executor.ExecuteScript("arguments[0].click();", element);

                log.Info("Email_Address_Matching_Validation test completed" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());
            }
            catch (Exception e)
            {
                log.Error(e.Message + "\n" + e.StackTrace + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());
                Assert.AreEqual(true, false);

            }
        }


        [Test]
        public void TVWeb_005_Password_Matching_Validation()
        {
            try
            {
                log.Info("Password_Matching_Validation test started" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

                RegistrationPage();                //funtion to redirect to registration page

                WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(60));
                wait.Until(ExpectedConditions.ElementExists(By.Id(OR.readingXMLFile("NonIETMemberRegistration", "NonMemberButton", "TVWebPortalOR.xml"))));

                element = driver.FindElement(By.Id(OR.readingXMLFile("NonIETMemberRegistration", "NonMemberButton", "TVWebPortalOR.xml")));
                executor.ExecuteScript("arguments[0].click();", element);

                #region Filling all the details
                element = wait.Until<IWebElement>((d) =>
                {
                    return driver.FindElement(By.Id(OR.readingXMLFile("NonIETMemberRegistration", "Title", "TVWebPortalOR.xml")));
                });
                Thread.Sleep(3000);
                SelectElement titleSelector = new SelectElement(element);
                titleSelector.SelectByIndex(2);


                driver.FindElement(By.Id(OR.readingXMLFile("NonIETMemberRegistration", "Forenames", "TVWebPortalOR.xml"))).SendKeys("John");
                driver.FindElement(By.Id(OR.readingXMLFile("NonIETMemberRegistration", "Surname", "TVWebPortalOR.xml"))).SendKeys("Miller");
                driver.FindElement(By.Id(OR.readingXMLFile("NonIETMemberRegistration", "EmailAddress", "TVWebPortalOR.xml"))).SendKeys("JohnMiller123@gmail.com");
                driver.FindElement(By.Id(OR.readingXMLFile("NonIETMemberRegistration", "ConfirmEmailAddress", "TVWebPortalOR.xml"))).SendKeys("JohnMiller123@gmail.com");
                driver.FindElement(By.Id(OR.readingXMLFile("NonIETMemberRegistration", "Username", "TVWebPortalOR.xml"))).SendKeys("Johnxmiller");
                driver.FindElement(By.Id(OR.readingXMLFile("NonIETMemberRegistration", "Password", "TVWebPortalOR.xml"))).SendKeys("Rave1234");
                driver.FindElement(By.Id(OR.readingXMLFile("NonIETMemberRegistration", "ConfirmPassword", "TVWebPortalOR.xml"))).SendKeys("Rave1234567");

                element = driver.FindElement(By.Id(OR.readingXMLFile("NonIETMemberRegistration", "Country", "TVWebPortalOR.xml")));
                SelectElement countrySelector = new SelectElement(element);
                countrySelector.SelectByIndex(3);

                #endregion

                element = driver.FindElement(By.Id(OR.readingXMLFile("NonIETMemberRegistration", "ContinueButton", "TVWebPortalOR.xml")));                                 //Clicking on Continue button for registration of Not IET member
                executor.ExecuteScript("arguments[0].click();", element);

                WebDriverWait waitStyle = new WebDriverWait(driver, TimeSpan.FromSeconds(20));
                waitStyle.Until(d => d.FindElement(By.Id(OR.readingXMLFile("NonIETMemberRegistration", "BannerMessage", "TVWebPortalOR.xml"))).Text.Equals("Password does not matchOK"));

                element = driver.FindElement(By.Id(OR.readingXMLFile("NonIETMemberRegistration", "BannerMessage", "TVWebPortalOR.xml")));

                String Donotmatch_Password_message = element.Text;

                //Password does not match banner message
                Assert.AreEqual("Password does not matchOK", Donotmatch_Password_message);

                //Click on ok button of banner mesage
                element = driver.FindElement(By.Id(OR.readingXMLFile("NonIETMemberRegistration", "BannerMessageOkButton", "TVWebPortalOR.xml")));
                executor.ExecuteScript("arguments[0].click();", element);

                log.Info("Password_Matching_Validation test completed" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());
            }

            catch (Exception e)
            {
                log.Error(e.Message + "\n" + e.StackTrace + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());
                Assert.AreEqual(true, false);

            }
        }

        [Test]
        public void TVWeb_007_Validate_Exist_Username_On_Cont()
        {
            try
            {
                log.Info("Validate_Exist_Username_On_Cont test Started" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

                TVWeb_001_Registration();              // registration process

                RegistrationPage();                   // funtion to redirect to registration page

                WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(60));
                wait.Until(ExpectedConditions.ElementExists(By.Id(OR.readingXMLFile("NonIETMemberRegistration", "NonMemberButton", "TVWebPortalOR.xml"))));

                element = driver.FindElement(By.Id(OR.readingXMLFile("NonIETMemberRegistration", "NonMemberButton", "TVWebPortalOR.xml")));
                executor.ExecuteScript("arguments[0].click();", element);

                wait = new WebDriverWait(driver, TimeSpan.FromSeconds(60));

                wait.Until(ExpectedConditions.ElementExists(By.ClassName("overlaySpinnerImage")));             //waiting for spinner loader



                //Read last record data and put into UI-Registration Page
                element = wait.Until<IWebElement>((d) =>
                {
                    return driver.FindElement(By.Id(OR.readingXMLFile("NonIETMemberRegistration", "Title", "TVWebPortalOR.xml")));
                });
                Thread.Sleep(3000);
                SelectElement titleSelector = new SelectElement(element);
                titleSelector.SelectByIndex(Convert.ToInt32(excelDataList[0]));
                selectedTitle = titleSelector.SelectedOption.Text;

                driver.FindElement(By.Id(OR.readingXMLFile("NonIETMemberRegistration", "Forenames", "TVWebPortalOR.xml"))).SendKeys(excelDataList[1]);
                Thread.Sleep(1000);

                driver.FindElement(By.Id(OR.readingXMLFile("NonIETMemberRegistration", "Surname", "TVWebPortalOR.xml"))).SendKeys(excelDataList[2]);
                Thread.Sleep(1000);

                String username_surname = driver.FindElement(By.Id(OR.readingXMLFile("NonIETMemberRegistration", "Forenames", "TVWebPortalOR.xml"))).GetAttribute("value") + " " + driver.FindElement(By.Id(OR.readingXMLFile("NonIETMemberRegistration", "Surname", "TVWebPortalOR.xml"))).GetAttribute("value");  //storing forename and surname

                driver.FindElement(By.Id(OR.readingXMLFile("NonIETMemberRegistration", "EmailAddress", "TVWebPortalOR.xml"))).SendKeys(excelDataList[3]);
                Thread.Sleep(1000);

                driver.FindElement(By.Id(OR.readingXMLFile("NonIETMemberRegistration", "ConfirmEmailAddress", "TVWebPortalOR.xml"))).SendKeys(excelDataList[3]);
                Thread.Sleep(1000);

                driver.FindElement(By.Id(OR.readingXMLFile("NonIETMemberRegistration", "Username", "TVWebPortalOR.xml"))).SendKeys(excelDataList[4]);
                Thread.Sleep(1000);

                driver.FindElement(By.Id(OR.readingXMLFile("NonIETMemberRegistration", "Password", "TVWebPortalOR.xml"))).SendKeys(excelDataList[5]);
                Thread.Sleep(1000);

                driver.FindElement(By.Id(OR.readingXMLFile("NonIETMemberRegistration", "ConfirmPassword", "TVWebPortalOR.xml"))).SendKeys(excelDataList[6]);
                Thread.Sleep(1000);

                element = driver.FindElement(By.Id(OR.readingXMLFile("NonIETMemberRegistration", "Country", "TVWebPortalOR.xml")));
                SelectElement countrySelector = new SelectElement(element);

                countrySelector.SelectByIndex(Convert.ToInt32(excelDataList[7]));
                selectedCountry = countrySelector.SelectedOption.Text;
                Thread.Sleep(1000);


                element = driver.FindElement(By.Id(OR.readingXMLFile("NonIETMemberRegistration", "ContinueButton", "TVWebPortalOR.xml")));                                 //Clicking on Continue button for registration of Not IET member
                executor.ExecuteScript("arguments[0].click();", element);

                //Existing username validation message
                iWait = new OpenQA.Selenium.Support.UI.WebDriverWait(driver, TimeSpan.FromSeconds(30.00));
                iWait.Until(ExpectedConditions.ElementExists(By.Id(OR.readingXMLFile("NonIETMemberRegistration", "BannerMessage", "TVWebPortalOR.xml"))));

                Assert.AreEqual("Registration failed as username already exists. Please try with other username.OK", driver.FindElement(By.Id(OR.readingXMLFile("NonIETMemberRegistration", "BannerMessage", "TVWebPortalOR.xml"))).Text);

                element = driver.FindElement(By.Id(OR.readingXMLFile("NonIETMemberRegistration", "BannerMessageOkButton", "TVWebPortalOR.xml")));
                executor.ExecuteScript("arguments[0].click();", element);

                log.Info("Validate_Exist_Username_On_Cont test completed" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());
            }
            catch (Exception e)
            {
                log.Error(e.Message + "\n" + e.StackTrace + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());
                Assert.AreEqual(true, false);

            }
        }


        [Test]
        public void TVWeb_008_Validate_Exist_Username_On_ChkUser()
        {
            try
            {
                log.Info("Validate_Exist_Username_On_ChkUser test started" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

                TVWeb_001_Registration();              //registration process

                RegistrationPage();                    //funtion to redirect to registration page

                WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(60));
                wait.Until(ExpectedConditions.ElementExists(By.Id(OR.readingXMLFile("NonIETMemberRegistration", "NonMemberButton", "TVWebPortalOR.xml"))));
                log.Info("wait is over" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

                element = driver.FindElement(By.Id(OR.readingXMLFile("NonIETMemberRegistration", "NonMemberButton", "TVWebPortalOR.xml")));
                executor.ExecuteScript("arguments[0].click();", element);


                driver.FindElement(By.Id(OR.readingXMLFile("NonIETMemberRegistration", "Username", "TVWebPortalOR.xml"))).SendKeys(excelDataList[4]);
                Thread.Sleep(1000);

                element = driver.FindElement(By.Id(OR.readingXMLFile("NonIETMemberRegistration", "CheckUserButton", "TVWebPortalOR.xml")));       //Clicking on Check User button
                executor.ExecuteScript("arguments[0].click();", element);

                iWait = new OpenQA.Selenium.Support.UI.WebDriverWait(driver, TimeSpan.FromSeconds(30.00));
                iWait.Until(ExpectedConditions.ElementExists(By.Id(OR.readingXMLFile("NonIETMemberRegistration", "BannerMessage", "TVWebPortalOR.xml"))));

                Assert.AreEqual("Username already exist. Please try with other usernameOK", driver.FindElement(By.Id(OR.readingXMLFile("NonIETMemberRegistration", "BannerMessage", "TVWebPortalOR.xml"))).Text);

                element = driver.FindElement(By.Id(OR.readingXMLFile("NonIETMemberRegistration", "BannerMessageOkButton", "TVWebPortalOR.xml")));
                executor.ExecuteScript("arguments[0].click();", element);

                log.Info("Validate_Exist_Username_On_ChkUser test completed" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());
            }
            catch (Exception e)
            {
                log.Error(e.Message + "\n" + e.StackTrace + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());
                Assert.AreEqual(true, false);

            }
        }



        [Test]
        public void TVWeb_009_Chrome_Validating_Mandatory_Fields()
        {
            try
            {
                log.Info("Chrome_Validating_Mandatory_Fields test completed" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

                String expectedStyle = "border: 1px solid rgb(197, 0, 0); padding: 2px 1px;";
                String currentStyle = "";

                RegistrationPage();

                WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(60));

                wait.Until(ExpectedConditions.ElementExists(By.Id(OR.readingXMLFile("NonIETMemberRegistration", "NonMemberButton", "TVWebPortalOR.xml"))));
                log.Info("wait is over before no button click" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

                //clicking on NO button
                element = driver.FindElement(By.Id(OR.readingXMLFile("NonIETMemberRegistration", "NonMemberButton", "TVWebPortalOR.xml")));
                executor.ExecuteScript("arguments[0].click();", element);

                wait.Until(ExpectedConditions.ElementExists(By.Id(OR.readingXMLFile("NonIETMemberRegistration", "ContinueButton", "TVWebPortalOR.xml"))));
                log.Info("wait is over aFTER nO Button click" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

                Thread.Sleep(2000);

                element = driver.FindElement(By.Id(OR.readingXMLFile("NonIETMemberRegistration", "ContinueButton", "TVWebPortalOR.xml")));
                executor.ExecuteScript("arguments[0].click();", element);

                WebDriverWait waitStyle = new WebDriverWait(driver, TimeSpan.FromSeconds(20));
                waitStyle.Until(d => d.FindElement(By.Id(OR.readingXMLFile("NonIETMemberRegistration", "Title", "TVWebPortalOR.xml"))).GetAttribute("style").Equals(expectedStyle));

                currentStyle = Stylegetid(OR.readingXMLFile("NonIETMemberRegistration", "Title", "TVWebPortalOR.xml"), expectedStyle);
                Assert.AreEqual(expectedStyle, currentStyle);

                currentStyle = Stylegetid(OR.readingXMLFile("NonIETMemberRegistration", "Forenames", "TVWebPortalOR.xml"), expectedStyle);

                Assert.AreEqual(expectedStyle, currentStyle);

                currentStyle = Stylegetid(OR.readingXMLFile("NonIETMemberRegistration", "Surname", "TVWebPortalOR.xml"), expectedStyle);

                Assert.AreEqual(expectedStyle, currentStyle);

                currentStyle = Stylegetid(OR.readingXMLFile("NonIETMemberRegistration", "EmailAddress", "TVWebPortalOR.xml"), expectedStyle);

                Assert.AreEqual(expectedStyle, currentStyle);

                currentStyle = Stylegetid(OR.readingXMLFile("NonIETMemberRegistration", "ConfirmEmailAddress", "TVWebPortalOR.xml"), expectedStyle);

                Assert.AreEqual(expectedStyle, currentStyle);

                currentStyle = Stylegetid(OR.readingXMLFile("NonIETMemberRegistration", "Username", "TVWebPortalOR.xml"), expectedStyle);

                Assert.AreEqual(expectedStyle, currentStyle);

                currentStyle = Stylegetid(OR.readingXMLFile("NonIETMemberRegistration", "Password", "TVWebPortalOR.xml"), expectedStyle);

                Assert.AreEqual(expectedStyle, currentStyle);


                currentStyle = Stylegetid(OR.readingXMLFile("NonIETMemberRegistration", "ConfirmPassword", "TVWebPortalOR.xml"), expectedStyle);

                Assert.AreEqual(expectedStyle, currentStyle);

                String CountryexpectedStyle = "border-radius: 4px; text-align: left; color: rgb(0, 0, 0); font-weight: normal; border: 1px solid rgb(197, 0, 0); padding: 2px 1px;";
                currentStyle = Stylegetid(OR.readingXMLFile("NonIETMemberRegistration", "Country", "TVWebPortalOR.xml"), CountryexpectedStyle);
                Assert.AreEqual(CountryexpectedStyle, currentStyle);

                Thread.Sleep(6000);

                log.Info("Chrome_Validating_Mandatory_Fields test completed" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

            }
            catch (Exception e)
            {
                log.Error(e.Message + "\n" + e.StackTrace + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());
                Assert.AreEqual(true, false);

            }
        }

        //This function gets the style of all fields at registration page
        public string Stylegetid(String id, String expectedStyle)
        {
                            
             return driver.FindElement(By.Id(id)).GetAttribute("style");

        }

        //This function store the data to be filled on registration page
        public void prepareData()
        {

            Random random = new Random();                                                      //0-247 0-7
            int randomSelector = random.Next(0, 7);
            while (randomSelector == 0)
            {
                randomSelector = random.Next(0, 7);
            }

            excelDataList.Add(randomSelector.ToString());                                             //Adding Title
            excelDataList.Add("Rave");                                                               //adding Forename
            excelDataList.Add("Tech");                                                               //Adding Surname
            excelDataList.Add("email" + uf.getGuid() + "@gmail.com");                                //Adding Email address
            excelDataList.Add("User" +uf.getGuid());                                                 //Username
            excelDataList.Add("Rave12345");                                                          //Passowrd
            excelDataList.Add("Rave12345");                                                          //Confirm Passowrd

            randomSelector = random.Next(0, 247);
            while (randomSelector == 0)
            {
                randomSelector = random.Next(0, 247);
            }
            excelDataList.Add(randomSelector.ToString());                                           //country
            excelDataList.Add("true");                                                              //checkbox
        }


        [Test]
        [TestCaseSource("EmailDataSource")]
        public void TVWeb_003_Email_Address_Negative_Testing(DataRow testData)
        {
            try
            {
                log.Info("Email_Address_Negative_Testing test started" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

                RegistrationPage();            //funtion to redirect to registration page

                WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(60));
                wait.Until(ExpectedConditions.ElementExists(By.Id(OR.readingXMLFile("NonIETMemberRegistration", "NonMemberButton", "TVWebPortalOR.xml"))));
                log.Info("wait is over" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

                element = driver.FindElement(By.Id(OR.readingXMLFile("NonIETMemberRegistration", "NonMemberButton", "TVWebPortalOR.xml")));                                //Clicking on NO button
                executor.ExecuteScript("arguments[0].click();", element);

                element = wait.Until<IWebElement>((d) =>
                {
                    return driver.FindElement(By.Id(OR.readingXMLFile("NonIETMemberRegistration", "Title", "TVWebPortalOR.xml")));
                });

                Thread.Sleep(3000);
                SelectElement titleSelector = new SelectElement(element);
                titleSelector.SelectByText(testData["Column1"].ToString());

                driver.FindElement(By.Id(OR.readingXMLFile("NonIETMemberRegistration", "Forenames", "TVWebPortalOR.xml"))).SendKeys(testData["Column2"].ToString());
                driver.FindElement(By.Id(OR.readingXMLFile("NonIETMemberRegistration", "Surname", "TVWebPortalOR.xml"))).SendKeys(testData["Column3"].ToString());
                driver.FindElement(By.Id(OR.readingXMLFile("NonIETMemberRegistration", "EmailAddress", "TVWebPortalOR.xml"))).SendKeys(testData["Column4"].ToString());
                driver.FindElement(By.Id(OR.readingXMLFile("NonIETMemberRegistration", "ConfirmEmailAddress", "TVWebPortalOR.xml"))).SendKeys(testData["Column5"].ToString());
                driver.FindElement(By.Id(OR.readingXMLFile("NonIETMemberRegistration", "Username", "TVWebPortalOR.xml"))).SendKeys(testData["Column6"].ToString());
                driver.FindElement(By.Id(OR.readingXMLFile("NonIETMemberRegistration", "Password", "TVWebPortalOR.xml"))).SendKeys(testData["Column7"].ToString());
                driver.FindElement(By.Id(OR.readingXMLFile("NonIETMemberRegistration", "ConfirmPassword", "TVWebPortalOR.xml"))).SendKeys(testData["Column8"].ToString());

                element = driver.FindElement(By.Id(OR.readingXMLFile("NonIETMemberRegistration", "Country", "TVWebPortalOR.xml")));
                SelectElement countrySelector = new SelectElement(element);
                countrySelector.SelectByText(testData["Column9"].ToString());

                element = driver.FindElement(By.Id(OR.readingXMLFile("NonIETMemberRegistration", "ContinueButton", "TVWebPortalOR.xml")));       //Clicking on Continue button for registration of Not IET member
                executor.ExecuteScript("arguments[0].click();", element);

                WebDriverWait waitStyle = new WebDriverWait(driver, TimeSpan.FromSeconds(20));
                waitStyle.Until(d => d.FindElement(By.Id(OR.readingXMLFile("NonIETMemberRegistration", "BannerMessage", "TVWebPortalOR.xml"))).Text.Equals("The email address is invalidOK"));

                element = driver.FindElement(By.Id(OR.readingXMLFile("NonIETMemberRegistration", "BannerMessage", "TVWebPortalOR.xml")));

                String invalid_email_message = element.Text;


                Thread.Sleep(2000);
                if (invalid_email_message.Equals("The email address is invalidOK"))
                    writingIntoexcel(true, currentRecord, "NormalDataset.xlsx", "Invalid_Email_Format");
                else
                    writingIntoexcel(false, currentRecord, "NormalDataset.xlsx", "Invalid_Email_Format");

                currentRecord++;

                element = driver.FindElement(By.Id(OR.readingXMLFile("NonIETMemberRegistration", "BannerMessageOkButton", "TVWebPortalOR.xml")));
                executor.ExecuteScript("arguments[0].click();", element);
                Assert.AreEqual(invalid_email_message, "The email address is invalidOK");

                log.Info("Email_Address_Negative_Testing test completed" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

            }
            catch (Exception e)
            {
                log.Error(e.Message + "\n" + e.StackTrace + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

                Assert.AreEqual(true, false);

            }
        }

        [Test]
        [TestCaseSource("PasswordDataSource")]
        public void TVWeb_006_Password_Format_Validation(DataRow testData)
        {
            try
            {
                log.Info("Password_Format_Validation test started" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());
                RegistrationPage();     //funtion to redirect to registration page

                WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(60));
                wait.Until(ExpectedConditions.ElementExists(By.Id(OR.readingXMLFile("NonIETMemberRegistration", "NonMemberButton", "TVWebPortalOR.xml"))));
                log.Info("wait is over" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

                element = driver.FindElement(By.Id(OR.readingXMLFile("NonIETMemberRegistration", "NonMemberButton", "TVWebPortalOR.xml")));                                 //Clicking on No button for registration of Not IET member
                executor.ExecuteScript("arguments[0].click();", element); ;


                //Filling all the details at registration page from XML file
                element = wait.Until<IWebElement>((d) =>
                {
                    return driver.FindElement(By.Id(OR.readingXMLFile("NonIETMemberRegistration", "Title", "TVWebPortalOR.xml")));
                });
                Thread.Sleep(3000);
                SelectElement titleSelector = new SelectElement(element);
                titleSelector.SelectByText(testData["Column1"].ToString());

                driver.FindElement(By.Id(OR.readingXMLFile("NonIETMemberRegistration", "Forenames", "TVWebPortalOR.xml"))).SendKeys(testData["Column2"].ToString());
                driver.FindElement(By.Id(OR.readingXMLFile("NonIETMemberRegistration", "Surname", "TVWebPortalOR.xml"))).SendKeys(testData["Column3"].ToString());
                driver.FindElement(By.Id(OR.readingXMLFile("NonIETMemberRegistration", "EmailAddress", "TVWebPortalOR.xml"))).SendKeys(testData["Column4"].ToString());
                driver.FindElement(By.Id(OR.readingXMLFile("NonIETMemberRegistration", "ConfirmEmailAddress", "TVWebPortalOR.xml"))).SendKeys(testData["Column5"].ToString());
                driver.FindElement(By.Id(OR.readingXMLFile("NonIETMemberRegistration", "Username", "TVWebPortalOR.xml"))).SendKeys(testData["Column6"].ToString());
                driver.FindElement(By.Id(OR.readingXMLFile("NonIETMemberRegistration", "Password", "TVWebPortalOR.xml"))).SendKeys(testData["Column7"].ToString());
                driver.FindElement(By.Id(OR.readingXMLFile("NonIETMemberRegistration", "ConfirmPassword", "TVWebPortalOR.xml"))).SendKeys(testData["Column8"].ToString());

                element = driver.FindElement(By.Id(OR.readingXMLFile("NonIETMemberRegistration", "Country", "TVWebPortalOR.xml")));
                SelectElement countrySelector = new SelectElement(element);
                countrySelector.SelectByText(testData["Column9"].ToString());


                element = driver.FindElement(By.Id(OR.readingXMLFile("NonIETMemberRegistration", "ContinueButton", "TVWebPortalOR.xml")));                                 //Clicking on Continue button for registration of Not IET member
                executor.ExecuteScript("arguments[0].click();", element);


                //banner message verification
                WebDriverWait waitStyle = new WebDriverWait(driver, TimeSpan.FromSeconds(20));
                waitStyle.Until(d => d.FindElement(By.Id(OR.readingXMLFile("NonIETMemberRegistration", "BannerMessage", "TVWebPortalOR.xml"))).Text.Equals("Please enter password in valid formatOK"));

                element = driver.FindElement(By.Id(OR.readingXMLFile("NonIETMemberRegistration", "BannerMessage", "TVWebPortalOR.xml")));

                String Incorrect_Password_message = element.Text;


                Assert.AreEqual(Incorrect_Password_message, "Please enter password in valid formatOK");


                //click on ok button of banner message
                element = driver.FindElement(By.Id(OR.readingXMLFile("NonIETMemberRegistration", "BannerMessageOkButton", "TVWebPortalOR.xml")));
                executor.ExecuteScript("arguments[0].click();", element);

                log.Info("Password_Format_Validation test completed" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());
            }
            catch (Exception e)
            {
                log.Error(e.Message + "\n" + e.StackTrace + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

                Assert.AreEqual(true, false);
            }
        }


        //This function read the data from Excel file for Email negative test
        public static IEnumerable<object> EmailDataSource
        {
            
            get
           
            {
                int start_cnt = 0;

                log.Info("inside EmailDataSsource" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());
   
                string ExcelDataPath = Environment.CurrentDirectory + "\\TestData_TV\\" + "NormalDataset.xlsx";

                FileStream stream = File.Open(ExcelDataPath, FileMode.Open, FileAccess.Read);

                // 2. Reading from a OpenXml Excel file (2007 format; *.xlsx)
                Excel.IExcelDataReader excelReader = Excel.ExcelReaderFactory.CreateOpenXmlReader(stream);

                // 3. DataSet - The result of each spreadsheet will be created in the result.Tables

                DataSet result = excelReader.AsDataSet();

                // 4. DataSet - Create column names from first row
                excelReader.IsFirstRowAsColumnNames = true;

                System.Data.DataTable currentSheet = result.Tables[1];

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

        //This function read the data from Excel file for Password test
        public static IEnumerable<object> PasswordDataSource
        {

            get
            {
                int start_cnt = 0;

                log.Info("inside password data source" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

                string ExcelDataPath = Environment.CurrentDirectory + "\\TestData_TV\\" + "NormalDataset.xlsx";

                FileStream stream = File.Open(ExcelDataPath, FileMode.Open, FileAccess.Read);

                Excel.IExcelDataReader excelReader = Excel.ExcelReaderFactory.CreateOpenXmlReader(stream);

                DataSet result = excelReader.AsDataSet();

                excelReader.IsFirstRowAsColumnNames = true;

                System.Data.DataTable currentSheet = result.Tables[0];

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

        [TestFixtureTearDown]
        public void TearDown()
        {
            st.Chrome_TearDown(driver, log);                        // Calling Chrome Teardown
        }


    }
}
