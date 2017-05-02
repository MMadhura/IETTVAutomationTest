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
using Utilities.Object_Repository;
using Utilities.Config;
using System.Drawing;
using System.Data;
using System.IO;
using IETTVWebportal.Chrome;
using System.Xml;
using IETTVWebportal.Reusable_Functions;
using log4net;
using log4net.Config;
using System.Reflection;
using System.Diagnostics;
using Microsoft.Expression.Encoder.ScreenCapture;

namespace IETTVWebportal.Chrome
{
    [TestFixture]
    class Chrome_IETMemberRegistration
    {
        // This is to configure logger mechanism for Utilities.Config
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
 
        #region variable_Declaration

        static int counter = 1;    //

        int screenHeight, screenWidth;

        String membershipId = null;   //Decalre Globally bacause once created and used by more than one test

        List<String> completeDataList = new List<String>();

        IWebElement element;

        IJavaScriptExecutor executor;

        IWait<IWebDriver> iWait;

        WebPortalReusableMethods reusableMethods = new WebPortalReusableMethods();

        internal IWebDriver driver = null;

        string driverName = "", driverPath, appURL;

        Utility_Functions uf = new Utility_Functions();                                         // Instantiate object for Utility Functons

        Configuration cf = new Configuration();                                                 // Instantiate object for Configuration

        Object_Repository_Class or = new Object_Repository_Class();                             // Instantiate object for object repository

        Chrome_WebSetupTearDown st = new Chrome_WebSetupTearDown();                             // Instantiate object for Chrome Setup Teardown

        #endregion

        #region Setup

        [TestFixtureSetUp]
        public void FixtureSetUp()
        {
            log4net.Config.XmlConfigurator.Configure(new FileInfo(AppDomain.CurrentDomain.BaseDirectory + "WebPortal.config"));

            log.Info("Inside Fixture Setup of chrome - NonIETmemberRegistration Test" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

            uf.CreateOrReplaceVideoFolder();

            string baseDir = AppDomain.CurrentDomain.BaseDirectory;                        // Get path till Base Directory

            driverName = "webdriver.chrome.driver";                                        // Driver name for Chrome

            driverPath = baseDir + "/chromedriver.exe";                                    // Path for ChromeDriver

            System.Environment.SetEnvironmentVariable(driverName, driverPath);

            driver = new ChromeDriver();                                                   // Initialize chrome driver 

            executor = (IJavaScriptExecutor)driver;

            iWait = new WebDriverWait(driver, TimeSpan.FromSeconds(60));
        }

            

        [SetUp]
        public void SetUp()
        {
            appURL = st.Chrome_Setup(driver, log, executor);                                // Calling Chrome Setup 
        }

       


        #endregion

        #region Reusable function for this module

        // This function moves the control to Registration page
        public void RedirectToRegistrationPage()
        {
            

            //clicking on Login button
            iWait.Until(ExpectedConditions.ElementToBeClickable(By.LinkText(or.readingXMLFile("IETMemberRegistration", "LoginLink", "TVWebPortalOR.xml"))));
            //element = driver.FindElement(By.LinkText("Please Log in"));
            element = driver.FindElement(By.LinkText(or.readingXMLFile("IETMemberRegistration", "LoginLink", "TVWebPortalOR.xml")));
            executor.ExecuteScript("arguments[0].click();", element);

            //clicking on Registration link
            iWait = new OpenQA.Selenium.Support.UI.WebDriverWait(driver, TimeSpan.FromSeconds(30.00));
            iWait.Until(ExpectedConditions.ElementToBeClickable(By.LinkText(or.readingXMLFile("IETMemberRegistration", "NotRegistered", "TVWebPortalOR.xml"))));
            element = driver.FindElement(By.LinkText(or.readingXMLFile("IETMemberRegistration", "RegistrationLink", "TVWebPortalOR.xml")));
            executor.ExecuteScript("arguments[0].click();", element);
        }

        //This funxcton Prepare details with unique GUID
        public List<String> GenerateCompleteData()
        {
           
                completeDataList.Add("Rave");            //adding Firstname 0
                completeDataList.Add("Tech");            //adding Lastname 1
                completeDataList.Add("19");            //adding Day 2
                completeDataList.Add("11");            //adding Month 3
                completeDataList.Add("1994");            //adding Year 4
                completeDataList.Add("user" +uf.getGuid());            //adding Username 5
                completeDataList.Add("Rave1234");            //adding Password 6
                completeDataList.Add("rave" +uf.getGuid() + "@email.com");            //adding Email 7
           
            return completeDataList;
        }

        //  This function select Date value from Day dropdown
        public void SetDayValue(String dateFromCRM)
        {
            //driver.FindElement(By.Id("Day")).Click();
            driver.FindElement(By.Id(or.readingXMLFile("IETMemberRegistration", "Day", "TVWebPortalOR.xml"))).Click();

            //IList<IWebElement> dayList = (IList<IWebElement>)driver.FindElements(By.CssSelector("ul.DAY > li"));
            IList<IWebElement> dayList = (IList<IWebElement>)driver.FindElements(By.CssSelector(or.readingXMLFile("IETMemberRegistration", "DayCSS", "TVWebPortalOR.xml")));
          
            foreach (IWebElement currentDay in dayList)
            {
                string dayValue = currentDay.FindElement(By.TagName("a")).Text.Trim();
                if (dayValue.Equals(dateFromCRM))
                {
                    currentDay.FindElement(By.TagName("a")).Click();
                    break;
                }
            }
        }

        //  This function select Month value from Month dropdown
        public void SetMonthValue(String monthFromCRM)
        {
            //driver.FindElement(By.Id("Month")).Click();
            driver.FindElement(By.Id(or.readingXMLFile("IETMemberRegistration", "Month", "TVWebPortalOR.xml"))).Click();


            //IList<IWebElement> monthList = (IList<IWebElement>)driver.FindElements(By.CssSelector("ul.MONTH > li"));
            IList<IWebElement> monthList = (IList<IWebElement>)driver.FindElements(By.CssSelector(or.readingXMLFile("IETMemberRegistration", "MonthCSS", "TVWebPortalOR.xml")));
         
            foreach (IWebElement currentMonth in monthList)
            {
                string monthvalue = currentMonth.FindElement(By.TagName("a")).GetAttribute("Id");
                if (monthvalue.Equals(monthFromCRM))
                {
                    currentMonth.FindElement(By.TagName("a")).Click();
                    break;
                }
            }
        }

        //  This function select Year value from Year dropdown
        public void SetYearValue(String yearFromCRM)
        {
            //driver.FindElement(By.Id("year")).Click();
            driver.FindElement(By.Id(or.readingXMLFile("IETMemberRegistration", "Year", "TVWebPortalOR.xml"))).Click();
            //IList<IWebElement> yearList = (IList<IWebElement>)driver.FindElements(By.CssSelector("ul.YEAR > li"));
            IList<IWebElement> yearList = (IList<IWebElement>)driver.FindElements(By.CssSelector(or.readingXMLFile("IETMemberRegistration", "YearCSS", "TVWebPortalOR.xml")));
         
            List<IWebElement> tempyearList = new List<IWebElement>();
            foreach (IWebElement ele in yearList)
            {
                tempyearList.Add(ele);
            }

            tempyearList.RemoveAt(0);    //here removing first element because first element does not contain year value

            foreach (IWebElement ele in tempyearList)
            {
                string yearvalue = ele.FindElement(By.TagName("a")).GetAttribute("Id");
               if (yearvalue.Equals(yearFromCRM))
                {
                    ele.FindElement(By.TagName("a")).Click();
                    break;
                }
            }
        }
       
        #endregion


        [Test]
        public void TVWeb_001_IETMemberRegistration()
        {

            try
            {
                log.Info("TVWeb_001_IETMemberRegistration test started" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

                // Generating details with unique GUID
                List<String> completeDataList = GenerateCompleteData();

                #region Region to create membership Id

                membershipId = reusableMethods.darwinCRM_CreateCustomer(completeDataList);

                #endregion

                RedirectToRegistrationPage();  // Moving to Registration Page.

                #region Region to Fill Surname, MemberShip ID, DOB

                uf.isJqueryActive(driver);

                //iWait.Until(ExpectedConditions.ElementIsVisible(By.Id("yes-member-btn")));
                iWait.Until(ExpectedConditions.ElementIsVisible(By.Id(or.readingXMLFile("IETMemberRegistration", "BtnYes", "TVWebPortalOR.xml"))));
                //driver.FindElement(By.Id("yes-member-btn")).Click();   // Clicking on YES button
                driver.FindElement(By.Id(or.readingXMLFile("IETMemberRegistration", "BtnYes", "TVWebPortalOR.xml"))).Click();   // Clicking on YES button

                //iWait.Until(ExpectedConditions.ElementIsVisible(By.Id("IEtSurname")));
                iWait.Until(ExpectedConditions.ElementIsVisible(By.Id(or.readingXMLFile("IETMemberRegistration", "IEtSurname", "TVWebPortalOR.xml"))));
                //driver.FindElement(By.Id("IEtSurname")).SendKeys(completeDataList[1]);     // Entering Surname 
                driver.FindElement(By.Id(or.readingXMLFile("IETMemberRegistration", "IEtSurname", "TVWebPortalOR.xml"))).SendKeys(completeDataList[1]);     // Entering Surname 
                //driver.FindElement(By.Id("MembershipNumber")).SendKeys(membershipId);     //Entering membership number
                driver.FindElement(By.Id(or.readingXMLFile("IETMemberRegistration", "MembershipNumber", "TVWebPortalOR.xml"))).SendKeys(membershipId);     //Entering membership number

                SetDayValue(completeDataList[2]);     // Selecting Date value from Day dropdown
                SetMonthValue(completeDataList[3]);   // Selecting Month value from Month dropdown  
                SetYearValue(completeDataList[4]);      // Selecting Year value from Year dropdown

                //driver.FindElement(By.Id("Check")).Click();  // Clicking on Continue Button
                driver.FindElement(By.Id(or.readingXMLFile("IETMemberRegistration", "Check", "TVWebPortalOR.xml"))).Click();  // Clicking on Continue Button


                #endregion


                #region Region to fill other details

                iWait.Until(ExpectedConditions.ElementIsVisible(By.Id(or.readingXMLFile("IETMemberRegistration", "IEtUserName", "TVWebPortalOR.xml"))));

                 driver.FindElement(By.Id(or.readingXMLFile("IETMemberRegistration", "IEtUserName", "TVWebPortalOR.xml"))).SendKeys(completeDataList[5]);           //Entering Username
                driver.FindElement(By.Id(or.readingXMLFile("IETMemberRegistration", "IEtPassword", "TVWebPortalOR.xml"))).SendKeys(completeDataList[6]);           //Entering password
                driver.FindElement(By.Id(or.readingXMLFile("IETMemberRegistration", "IEtConfirmPassword", "TVWebPortalOR.xml"))).SendKeys(completeDataList[6]);   //Entering confirmPassword
                driver.FindElement(By.Id(or.readingXMLFile("IETMemberRegistration", "IEtEmailAddrees", "TVWebPortalOR.xml"))).SendKeys(completeDataList[7]);     //Entering Email
                driver.FindElement(By.Id(or.readingXMLFile("IETMemberRegistration", "ConfirmEmail", "TVWebPortalOR.xml"))).SendKeys(completeDataList[7]);        //Entering Confirm Email
                driver.FindElement(By.Id(or.readingXMLFile("IETMemberRegistration", "IetMemberContinue", "TVWebPortalOR.xml"))).Click();                         // Clickcing on Continue button 

                //iWait.Until(ExpectedConditions.ElementIsVisible(By.Id("FinalContinueMember")));
                iWait.Until(ExpectedConditions.ElementIsVisible(By.Id(or.readingXMLFile("IETMemberRegistration", "FinalMember", "TVWebPortalOR.xml"))));
                //driver.FindElement(By.Id("FinalContinueMember")).Click();
                driver.FindElement(By.Id(or.readingXMLFile("IETMemberRegistration", "FinalMember", "TVWebPortalOR.xml"))).Click();

                #endregion

                // Verfying the content of Banner Message and clicking on OK button
                iWait.Until(ExpectedConditions.ElementIsVisible(By.Id(or.readingXMLFile("IETMemberRegistration", "BannerMessageOkButton", "TVWebPortalOR.xml"))));
                //Assert.AreEqual("You have registered successfully.", driver.FindElement(By.Id("Sucess_Message")).Text.Trim());
                Assert.AreEqual("You have registered successfully.", driver.FindElement(By.Id(or.readingXMLFile("IETMemberRegistration", "SuccessMsg", "TVWebPortalOR.xml"))).Text.Trim());
                //driver.FindElement(By.Id("okButtonId")).Click();
                driver.FindElement(By.Id(or.readingXMLFile("IETMemberRegistration", "BannerMessageOkButton", "TVWebPortalOR.xml"))).Click();


                #region Code to Logout Comment


                //Waiting untill please wait text is displayed
                iWait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector("div > h3")));

                iWait.Until(ExpectedConditions.ElementExists(By.Id(or.readingXMLFile("IETMemberRegistration", "UserName", "TVWebPortalOR.xml"))));

                // Entering Username
                driver.FindElement(By.Id(or.readingXMLFile("IETMemberRegistration", "UserName", "TVWebPortalOR.xml"))).SendKeys(completeDataList[5]);

                //Entering password
                driver.FindElement(By.Id(or.readingXMLFile("IETMemberRegistration", "Password", "TVWebPortalOR.xml"))).SendKeys(completeDataList[6]);

                // Clicking on Login button
                driver.FindElement(By.ClassName(or.readingXMLFile("IETMemberRegistration", "LoginButton", "TVWebPortalOR.xml"))).Click();

                //Terms and condition page-Positive test

                element = iWait.Until<IWebElement>((d) =>
                {
                    return driver.FindElement(By.ClassName(or.readingXMLFile("IETMemberRegistration", "AcceptButton", "TVWebPortalOR.xml")));                                //accept button of T&C
                });

                executor = (IJavaScriptExecutor)driver;

                executor.ExecuteScript("arguments[0].click();", element);

                iWait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector(or.readingXMLFile("IETMemberRegistration", "button.ok_btn_size", "TVWebPortalOR.xml"))));  // Waiting for Popup window to appear after clicking on accept button

                IList<IWebElement> btnOK = driver.FindElements(By.CssSelector(or.readingXMLFile("IETMemberRegistration", "button.ok_btn_size", "TVWebPortalOR.xml")));

                element = btnOK.ElementAt(0);

                executor.ExecuteScript("arguments[0].click();", element);

                element = iWait.Until<IWebElement>((d) =>
                {
                    return driver.FindElement(By.Id(or.readingXMLFile("IETMemberRegistration", "Message", "TVWebPortalOR.xml")));                      //getting username who logged-in 
                });
                
                element = driver.FindElement(By.Id(or.readingXMLFile("IETMemberRegistration", "MyIETDropDown", "TVWebPortalOR.xml")));                                       //Clicking myIET dropdown 
                executor.ExecuteScript("arguments[0].click();", element);

                element = driver.FindElement(By.Id(or.readingXMLFile("IETMemberRegistration", "LogoutLink", "TVWebPortalOR.xml")));                                     //clicking on log out link
                executor.ExecuteScript("arguments[0].click();", element);

                #endregion

                log.Info("TVWeb_001_IETMemberRegistration test completed" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

            }
            catch (Exception e)
            {
                log.Error(e.Message + "\n" + e.StackTrace + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());
                Assert.AreEqual(true, false);
            }
        }

        [Test]
        public void TVWeb_002_ExistingMemberShipIDValidation()
        {

            try
            {

                log.Info("TVWeb_002_ExistingMemberShipIDValidation test started" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

                TVWeb_001_IETMemberRegistration();     //Calling Registration Process

                RedirectToRegistrationPage();  //Redirecting to Registration Page.

                #region Region to Fill Surname, MemberShip ID, DOB

                uf.isJqueryActive(driver);

                iWait.Until(ExpectedConditions.ElementIsVisible(By.Id("yes-member-btn")));
                //driver.FindElement(By.Id("yes-member-btn")).Click();
                driver.FindElement(By.Id(or.readingXMLFile("IETMemberRegistration", "Check", "TVWebPortalOR.xml"))).Click();

                //iWait.Until(ExpectedConditions.ElementIsVisible(By.Id("IEtSurname")));                
                //driver.FindElement(By.Id("IEtSurname")).SendKeys(completeDataList[1]);     //entering  surName 
                //driver.FindElement(By.Id("MembershipNumber")).SendKeys(membershipId);       // entering membership number

                iWait.Until(ExpectedConditions.ElementIsVisible(By.Id(or.readingXMLFile("IETMemberRegistration", "IEtSurname", "TVWebPortalOR.xml"))));
                driver.FindElement(By.Id(or.readingXMLFile("IETMemberRegistration", "IEtSurname", "TVWebPortalOR.xml"))).SendKeys(completeDataList[1]);     //entering  surName 
                driver.FindElement(By.Id(or.readingXMLFile("IETMemberRegistration", "MembershipNumber", "TVWebPortalOR.xml"))).SendKeys(membershipId);       // entering membership number
                


               
                SetDayValue(completeDataList[2]);     // Selecting Date value from Day dropdown
                SetMonthValue(completeDataList[3]);   // Selecting Month value from Month dropdown  
                SetYearValue(completeDataList[4]);      // Selecting Year value from Year dropdown

                driver.FindElement(By.Id(or.readingXMLFile("IETMemberRegistration", "Check", "TVWebPortalOR.xml"))).Click();


                #endregion

                //iWait.Until(ExpectedConditions.ElementIsVisible(By.Id("okButtonId")));
                iWait.Until(ExpectedConditions.ElementIsVisible(By.Id(or.readingXMLFile("NonIETMemberRegistration", "BannerMessageOkButton", "TVWebPortalOR.xml"))));
                Assert.AreEqual("You are already been registeredOK", driver.FindElement(By.ClassName(or.readingXMLFile("IETMemberRegistration", "Warningmsg", "TVWebPortalOR.xml"))).Text.Trim());        //Verifying the Warning Message
                driver.FindElement(By.Id(or.readingXMLFile("NonIETMemberRegistration", "BannerMessageOkButton", "TVWebPortalOR.xml"))).Click();        //Clicking on OK button

                log.Info("TVWeb_002_ExistingMemberShipIDValidation test completed" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

            }
            catch (Exception e)
            {
                log.Error(e.Message + "\n" + e.StackTrace + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());
                Assert.AreEqual(true, false);
            }


        }
 
        [Test]
        public void TVWeb_003_ExistingUsernameValidation()
        {

            try
            {
                log.Info("TVWeb_003_ExistingUsernameValidation test started" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

                TVWeb_001_IETMemberRegistration();     //Calling Registration Process

                #region Region to create membership Id

                membershipId = null;

                membershipId = reusableMethods.darwinCRM_CreateCustomer(completeDataList);

                #endregion

                RedirectToRegistrationPage();  //Redirecting to Registration Page.

                #region Region to Fill Surname, MemberShip ID, DOB

                uf.isJqueryActive(driver);

                iWait.Until(ExpectedConditions.ElementIsVisible(By.Id(or.readingXMLFile("IETMemberRegistration", "BtnYes", "TVWebPortalOR.xml"))));
                driver.FindElement(By.Id(or.readingXMLFile("IETMemberRegistration", "BtnYes", "TVWebPortalOR.xml"))).Click();

                iWait.Until(ExpectedConditions.ElementIsVisible(By.Id(or.readingXMLFile("IETMemberRegistration", "IEtSurname", "TVWebPortalOR.xml"))));
                driver.FindElement(By.Id(or.readingXMLFile("IETMemberRegistration", "IEtSurname", "TVWebPortalOR.xml"))).SendKeys(completeDataList[1]);     //entering  surName with prefix
                driver.FindElement(By.Id(or.readingXMLFile("IETMemberRegistration", "MembershipNumber", "TVWebPortalOR.xml"))).SendKeys(membershipId);       //membership number  

                SetDayValue(completeDataList[2]);     // Selecting Date value from Day dropdown
                SetMonthValue(completeDataList[3]);   // Selecting Month value from Month dropdown  
                SetYearValue(completeDataList[4]);      // Selecting Year value from Year dropdown



                driver.FindElement(By.Id(or.readingXMLFile("IETMemberRegistration", "Check", "TVWebPortalOR.xml"))).Click();
                #endregion


                #region Region to fill other details

                iWait.Until(ExpectedConditions.ElementIsVisible(By.Id("IEtUserName")));

                driver.FindElement(By.Id(or.readingXMLFile("IETMemberRegistration", "IEtUserName", "TVWebPortalOR.xml"))).SendKeys(completeDataList[5]);           //Entering Username
                driver.FindElement(By.Id(or.readingXMLFile("IETMemberRegistration", "IEtPassword", "TVWebPortalOR.xml"))).SendKeys(completeDataList[6]);           //Entering password
                driver.FindElement(By.Id(or.readingXMLFile("IETMemberRegistration", "IEtConfirmPassword", "TVWebPortalOR.xml"))).SendKeys(completeDataList[6]);   //Entering confirmPassword
                driver.FindElement(By.Id(or.readingXMLFile("IETMemberRegistration", "IEtEmailAddrees", "TVWebPortalOR.xml"))).SendKeys(completeDataList[7]);     //Entering Email
                driver.FindElement(By.Id(or.readingXMLFile("IETMemberRegistration", "ConfirmEmail", "TVWebPortalOR.xml"))).SendKeys(completeDataList[7]);        //Entering Confirm Email
                driver.FindElement(By.Id(or.readingXMLFile("IETMemberRegistration", "IetMemberContinue", "TVWebPortalOR.xml"))).Click();                         // Clickcing on Continue button 



                #endregion

                iWait.Until(ExpectedConditions.ElementToBeClickable(By.Id(or.readingXMLFile("NonIETMemberRegistration", "BannerMessageOkButton", "TVWebPortalOR.xml"))));

                iWait.Until(ExpectedConditions.ElementIsVisible(By.Id(or.readingXMLFile("NonIETMemberRegistration", "BannerMessageOkButton", "TVWebPortalOR.xml"))));

                Assert.AreEqual("Username already exist. Please try with other usernameOK", driver.FindElement(By.ClassName(or.readingXMLFile("IETMemberRegistration", "Warningmsg", "TVWebPortalOR.xml"))).Text.Trim());        //Verifying the Warning Message
                driver.FindElement(By.Id(or.readingXMLFile("NonIETMemberRegistration", "BannerMessageOkButton", "TVWebPortalOR.xml"))).Click();        //Clicking on OK button

                log.Info("TVWeb_003_ExistingUsernameValidation test completed" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

            }
            catch (Exception e)
            {
                log.Error(e.Message + "\n" + e.StackTrace + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());
                Assert.AreEqual(true, false);
            }
        }

        [Test]
        public void TVWeb_004_ExistingEmailValidation()
        {

            try
            {
                log.Info("TVWeb_004_ExistingEmailValidation test started" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

                TVWeb_001_IETMemberRegistration();     //Calling Registration Process

                #region Region to create membership Id

                membershipId = null;


                membershipId = reusableMethods.darwinCRM_CreateCustomer(completeDataList);


                #endregion

                RedirectToRegistrationPage();  //Redirecting to Registration Page.

                #region Region to Fill Surname, MemberShip ID, DOB

                uf.isJqueryActive(driver);

                iWait.Until(ExpectedConditions.ElementIsVisible(By.Id(or.readingXMLFile("IETMemberRegistration", "BtnYes", "TVWebPortalOR.xml"))));
                driver.FindElement(By.Id(or.readingXMLFile("IETMemberRegistration", "BtnYes", "TVWebPortalOR.xml"))).Click();

                iWait.Until(ExpectedConditions.ElementIsVisible(By.Id(or.readingXMLFile("IETMemberRegistration", "IEtUserName", "TVWebPortalOR.xml"))));
                driver.FindElement(By.Id(or.readingXMLFile("IETMemberRegistration", "IEtSurname", "TVWebPortalOR.xml"))).SendKeys(completeDataList[1]);     //entering  surName with prefix
                driver.FindElement(By.Id(or.readingXMLFile("IETMemberRegistration", "MembershipNumber", "TVWebPortalOR.xml"))).SendKeys(membershipId);       //membership number  

                SetDayValue(completeDataList[2]);
                SetMonthValue(completeDataList[3]);
                SetYearValue(completeDataList[4]);


                driver.FindElement(By.Id(or.readingXMLFile("IETMemberRegistration", "Check", "TVWebPortalOR.xml"))).Click();
                #endregion

                #region Region to fill other details

                iWait.Until(ExpectedConditions.ElementIsVisible(By.Id(or.readingXMLFile("IETMemberRegistration", "IEtUserName", "TVWebPortalOR.xml"))));

                driver.FindElement(By.Id(or.readingXMLFile("IETMemberRegistration", "IEtUserName", "TVWebPortalOR.xml"))).SendKeys(completeDataList[5]);           //Entering Username
                driver.FindElement(By.Id(or.readingXMLFile("IETMemberRegistration", "IEtPassword", "TVWebPortalOR.xml"))).SendKeys(completeDataList[6]);           //Entering password
                driver.FindElement(By.Id(or.readingXMLFile("IETMemberRegistration", "IEtConfirmPassword", "TVWebPortalOR.xml"))).SendKeys(completeDataList[6]);   //Entering confirmPassword
                driver.FindElement(By.Id(or.readingXMLFile("IETMemberRegistration", "IEtEmailAddrees", "TVWebPortalOR.xml"))).SendKeys(completeDataList[7]);     //Entering Email
                driver.FindElement(By.Id(or.readingXMLFile("IETMemberRegistration", "ConfirmEmail", "TVWebPortalOR.xml"))).SendKeys(completeDataList[7]);        //Entering Confirm Email
                driver.FindElement(By.Id(or.readingXMLFile("IETMemberRegistration", "IetMemberContinue", "TVWebPortalOR.xml"))).Click();                         // Clickcing on Continue button 

                iWait.Until(ExpectedConditions.ElementIsVisible(By.Id(or.readingXMLFile("IETMemberRegistration", "FinalMember", "TVWebPortalOR.xml"))));
                driver.FindElement(By.Id(or.readingXMLFile("IETMemberRegistration", "FinalMember", "TVWebPortalOR.xml"))).Click();

                #endregion


                //Verifying the content of Warning Message
                iWait.Until(ExpectedConditions.ElementIsVisible(By.Id(or.readingXMLFile("IETMemberRegistration", "FinalMember", "TVWebPortalOR.xml"))));
                Assert.AreEqual("Registration failed as email address already existsOK", driver.FindElement(By.ClassName(or.readingXMLFile("IETMemberRegistration", "Warningmsg", "TVWebPortalOR.xml"))).Text.Trim());
                driver.FindElement(By.Id(or.readingXMLFile("IETMemberRegistration", "BannerMessageOkButton", "TVWebPortalOR.xml"))).Click();        //Clicking on OK button

                log.Info("TVWeb_004_ExistingEmailValidation test completed" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

            }
            catch (Exception e)
            {
                log.Error(e.Message + "\n" + e.StackTrace + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());
                Assert.AreEqual(true, false);
            }


        }


        //Not able to test because functionality is not working properly
        [Test]
        public void TVWeb_005_EmailMatchingValidation()
        {
            //String surname = "khan";
            //String day = "10";
            //String month = "10";
            //String year = "1994";

            //redirectToRegistrationPage();

            //#region Region to create membership Id

            //membershipId = null;


            //membershipId = reusableMethods.darwinCRM_CreateCustomer(completeDataList);

            //#endregion

            //#region Region to Fill Surname, MemberShip ID, DOB

            //uf.isJqueryActive(driver);

            //iWait.Until(ExpectedConditions.ElementIsVisible(By.Id("yes-member-btn")));
            //driver.FindElement(By.Id("yes-member-btn")).Click();

            //iWait.Until(ExpectedConditions.ElementIsVisible(By.Id("IEtSurname")));
            //driver.FindElement(By.Id("IEtSurname")).SendKeys(surname);     //entering  surName with prefix
            //driver.FindElement(By.Id("MembershipNumber")).SendKeys(membershipId);       //membership number  

            //#region selecting data value

            //driver.FindElement(By.Id("Day")).Click();

            //IList<IWebElement> dayList = (IList<IWebElement>)driver.FindElements(By.CssSelector("ul.day > li"));
          
            //foreach (IWebElement currentDay in dayList)
            //{
            //    string dayValue = currentDay.FindElement(By.TagName("a")).Text.Trim();
            //    if (dayValue.Equals(day))
            //    {
            //        currentDay.FindElement(By.TagName("a")).Click();
            //        break;
            //    }
            //}

            //#endregion

            //#region selecting month

            //driver.FindElement(By.Id("Month")).Click();


            //IList<IWebElement> monthList = (IList<IWebElement>)driver.FindElements(By.CssSelector("ul.month > li"));
            //foreach (IWebElement currentMonth in monthList)
            //{
            //    Console.WriteLine("insIde for month");
            //    string monthvalue = currentMonth.FindElement(By.TagName("a")).GetAttribute("Id");
            //    Console.WriteLine("month value :: " + monthvalue);
            //    if (monthvalue.Equals(month))
            //    {
            //        currentMonth.FindElement(By.TagName("a")).Click();
            //        break;
            //    }
            //}

            //#endregion

            //#region selecting year value

            //driver.FindElement(By.Id("year")).Click();
            //IList<IWebElement> yearList = (IList<IWebElement>)driver.FindElements(By.CssSelector("ul.year > li"));
            //Console.WriteLine("year Count :: " + yearList.Count);

            //List<IWebElement> tempyearList = new List<IWebElement>();
            //foreach (IWebElement ele in yearList)
            //{
            //    tempyearList.Add(ele);
            //}

            //tempyearList.RemoveAt(0);    //here removing first element because first element does not contain year value

            //foreach (IWebElement ele in tempyearList)
            //{
            //    Console.WriteLine("insIde for year");
            //    string yearvalue = ele.FindElement(By.TagName("a")).GetAttribute("Id");
            //    Console.WriteLine("year value :: " + yearvalue);
            //    if (yearvalue.Equals(year))
            //    {
            //        ele.FindElement(By.TagName("a")).Click();
            //        break;
            //    }
            //}

            //#endregion

            //driver.FindElement(By.Id("Check")).Click();
            //#endregion

            //#region Region to fill other details

            //iWait.Until(ExpectedConditions.ElementIsVisible(By.Id("IEtUserName")));

            //driver.FindElement(By.Id("IEtUserName")).SendKeys("AutomationSeleniumIETUser");           //Entering Username
            //driver.FindElement(By.Id("IEtPassword")).SendKeys("Rave1234");           //Entering password
            //driver.FindElement(By.Id("IEtConfirmPassword")).SendKeys("Rave1234");   //Entering confirmPassword
            //driver.FindElement(By.Id("IEtEmailAddrees")).SendKeys("autmationSelenium@email.com");   //Entering Email
            //driver.FindElement(By.Id("ConfirmEmail")).SendKeys("autmationSelenium12@email.com");   //Entering Confirm Email
            //driver.FindElement(By.Id("IetMemberContinue")).Click();

            //iWait.Until(ExpectedConditions.ElementIsVisible(By.Id("FinalContinueMember")));
            //driver.FindElement(By.Id("FinalContinueMember")).Click();

            //#endregion
        
        }


        [Test]
        public void TVWeb_006_PassowrdMatchingValidation()
        {
            String surname = "Tech";
            String day = "19";
            String month = "11";
            String year = "1994";

            try
            {
                log.Info("TVWeb_006_PassowrdMatchingValidation test started" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

                #region Region to create membership Id

                membershipId = null;

                membershipId = reusableMethods.darwinCRM_CreateCustomer(completeDataList);

                #endregion

                RedirectToRegistrationPage();  // moving to Registration page

                #region Region to Fill Surname, MemberShip ID, DOB

                uf.isJqueryActive(driver);

                iWait.Until(ExpectedConditions.ElementIsVisible(By.Id(or.readingXMLFile("IETMemberRegistration", "BtnYes", "TVWebPortalOR.xml"))));
                driver.FindElement(By.Id(or.readingXMLFile("IETMemberRegistration", "BtnYes", "TVWebPortalOR.xml"))).Click();

                iWait.Until(ExpectedConditions.ElementIsVisible(By.Id("IEtSurname")));
                driver.FindElement(By.Id(or.readingXMLFile("IETMemberRegistration", "IEtSurname", "TVWebPortalOR.xml"))).SendKeys(surname);     //entering  surName 
                driver.FindElement(By.Id(or.readingXMLFile("IETMemberRegistration", "MembershipNumber", "TVWebPortalOR.xml"))).SendKeys(membershipId);       //membership number  

                SetDayValue(day);
                SetMonthValue(month);
                SetYearValue(year);

                driver.FindElement(By.Id(or.readingXMLFile("IETMemberRegistration", "Check", "TVWebPortalOR.xml"))).Click();
                #endregion

                #region Region to fill other details

                iWait.Until(ExpectedConditions.ElementIsVisible(By.Id(or.readingXMLFile("IETMemberRegistration", "IEtUserName", "TVWebPortalOR.xml"))));

                driver.FindElement(By.Id(or.readingXMLFile("IETMemberRegistration", "IEtUserName", "TVWebPortalOR.xml"))).SendKeys("AutomationSeleniumIETUser");           //Entering Username
                driver.FindElement(By.Id(or.readingXMLFile("IETMemberRegistration", "IEtPassword", "TVWebPortalOR.xml"))).SendKeys("Rave1234");           //Entering password
                driver.FindElement(By.Id(or.readingXMLFile("IETMemberRegistration", "IEtConfirmPassword", "TVWebPortalOR.xml"))).SendKeys("Rave12344563");   //Entering confirmPassword
                driver.FindElement(By.Id(or.readingXMLFile("IETMemberRegistration", "IEtEmailAddrees", "TVWebPortalOR.xml"))).SendKeys("autmationSelenium@email.com");   //Entering Email
                driver.FindElement(By.Id(or.readingXMLFile("IETMemberRegistration", "ConfirmEmail", "TVWebPortalOR.xml"))).SendKeys("autmationSelenium@email.com");   //Entering Confirm Email
                driver.FindElement(By.Id(or.readingXMLFile("IETMemberRegistration", "IetMemberContinue", "TVWebPortalOR.xml"))).Click();



                #endregion

                //Verifying the content or banner message
                iWait.Until(ExpectedConditions.ElementIsVisible(By.Id(or.readingXMLFile("IETMemberRegistration", "BannerMessageOkButton", "TVWebPortalOR.xml"))));
                Assert.AreEqual("Password does not matchOK", driver.FindElement(By.ClassName(or.readingXMLFile("IETMemberRegistration", "Warningmsg", "TVWebPortalOR.xml"))).Text.Trim());        //Verifying the Warning Message
                driver.FindElement(By.Id(or.readingXMLFile("IETMemberRegistration", "BannerMessageOkButton", "TVWebPortalOR.xml"))).Click();        //Clicking on OK button

                log.Info("TVWeb_006_PassowrdMatchingValidation test completed" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

            }
            catch (Exception e)
            {
                log.Error(e.Message + "\n" + e.StackTrace + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());
                Assert.AreEqual(true, false);
            }
        }

        [Test]
        [TestCaseSource("PasswordDataSource")]
        public void TVWeb_007_PasswordFormatValidation(DataRow testData)
        {
            String surname = "Tech";
            String day = "19";
            String month = "11";
            String year = "1994";

            try
            {
                log.Info("TVWeb_007_PasswordFormatValidation test started" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

                //Preparing data into required form
                List<String> completeDataList = GenerateCompleteData();

                if (counter == 1)   // Here by using counter we are restricting that only one time Membership ID should be created.
                {
                    #region Region to create membership Id

                    membershipId = null;

                    membershipId = reusableMethods.darwinCRM_CreateCustomer(completeDataList);


                    #endregion

                    counter++;
                }


                RedirectToRegistrationPage();  //Moving to Registration Page.

                #region Region to Fill Surname, MemberShip ID, DOB

                uf.isJqueryActive(driver);

                iWait.Until(ExpectedConditions.ElementIsVisible(By.Id(or.readingXMLFile("IETMemberRegistration", "BtnYes", "TVWebPortalOR.xml"))));
                driver.FindElement(By.Id(or.readingXMLFile("IETMemberRegistration", "BtnYes", "TVWebPortalOR.xml"))).Click();

                iWait.Until(ExpectedConditions.ElementIsVisible(By.Id(or.readingXMLFile("IETMemberRegistration", "IEtSurname", "TVWebPortalOR.xml"))));
                driver.FindElement(By.Id(or.readingXMLFile("IETMemberRegistration", "IEtSurname", "TVWebPortalOR.xml"))).SendKeys(surname);     //entering  surName with prefix
                driver.FindElement(By.Id(or.readingXMLFile("IETMemberRegistration", "MembershipNumber", "TVWebPortalOR.xml"))).SendKeys(membershipId); //membership number

                SetDayValue(day);
                SetMonthValue(month);
                SetYearValue(year);

                driver.FindElement(By.Id(or.readingXMLFile("IETMemberRegistration", "Check", "TVWebPortalOR.xml"))).Click();


                #endregion


                #region Region to fill other details

                iWait.Until(ExpectedConditions.ElementIsVisible(By.Id(or.readingXMLFile("IETMemberRegistration", "IEtUserName", "TVWebPortalOR.xml"))));

                driver.FindElement(By.Id(or.readingXMLFile("IETMemberRegistration", "IEtUserName", "TVWebPortalOR.xml"))).SendKeys(completeDataList[5]);           //Entering Username
                driver.FindElement(By.Id(or.readingXMLFile("IETMemberRegistration", "IEtPassword", "TVWebPortalOR.xml"))).SendKeys(testData["Column1"].ToString());           //Entering password
                driver.FindElement(By.Id(or.readingXMLFile("IETMemberRegistration", "IEtConfirmPassword", "TVWebPortalOR.xml"))).SendKeys(testData["Column2"].ToString());   //Entering confirmPassword
                driver.FindElement(By.Id(or.readingXMLFile("IETMemberRegistration", "IEtEmailAddrees", "TVWebPortalOR.xml"))).SendKeys(completeDataList[7]);   //Entering Email
                driver.FindElement(By.Id(or.readingXMLFile("IETMemberRegistration", "ConfirmEmail", "TVWebPortalOR.xml"))).SendKeys(completeDataList[7]);   //Entering Confirm Email
                driver.FindElement(By.Id(or.readingXMLFile("IETMemberRegistration", "IetMemberContinue", "TVWebPortalOR.xml"))).Click();



                iWait.Until(ExpectedConditions.ElementIsVisible(By.Id(or.readingXMLFile("IETMemberRegistration", "BannerMessageOkButton", "TVWebPortalOR.xml"))));
                Assert.AreEqual("Please enter password in valid formatOK", driver.FindElement(By.ClassName(or.readingXMLFile("IETMemberRegistration", "Warningmsg", "TVWebPortalOR.xml"))).Text.Trim());        //Verifying the Warning Message
                driver.FindElement(By.Id(or.readingXMLFile("IETMemberRegistration", "BannerMessageOkButton", "TVWebPortalOR.xml"))).Click();        //Clicking on OK button


                #endregion

                log.Info("TVWeb_007_PasswordFormatValidation test completed" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

            }
            catch (Exception e)
            {
                log.Error(e.Message + "\n" + e.StackTrace + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());
                Assert.AreEqual(true, false);
            }
        }


        [Test]
        [TestCaseSource("EmailDataSource")]
        public void TVWeb_008_EmailFormatValidation(DataRow testData)
        {
            String surname = "Tech";
            String day = "19";
            String month = "11";
            String year = "1994";

            try
            {
                log.Info("TVWeb_008_EmailFormatValidation test started" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

                //Preparing data into required form
                List<String> completeDataList = GenerateCompleteData();

                if (counter == 1) // Here by using counter we are restricting that only one time Membership ID should be created.
                {
                    #region Region to create membership Id

                    membershipId = null;


                    membershipId = reusableMethods.darwinCRM_CreateCustomer(completeDataList);

                    #endregion
                    counter++;
                }

                RedirectToRegistrationPage();  //Moving to Registration Page.

                #region Region to Fill Surname, MemberShip ID, DOB

                uf.isJqueryActive(driver);

                iWait.Until(ExpectedConditions.ElementIsVisible(By.Id(or.readingXMLFile("IETMemberRegistration", "BtnYes", "TVWebPortalOR.xml"))));
                driver.FindElement(By.Id(or.readingXMLFile("IETMemberRegistration", "BtnYes", "TVWebPortalOR.xml"))).Click();

                iWait.Until(ExpectedConditions.ElementIsVisible(By.Id(or.readingXMLFile("IETMemberRegistration", "IEtSurname", "TVWebPortalOR.xml"))));
                driver.FindElement(By.Id(or.readingXMLFile("IETMemberRegistration", "IEtSurname", "TVWebPortalOR.xml"))).SendKeys(surname);     //entering  surName with prefix
                driver.FindElement(By.Id(or.readingXMLFile("IETMemberRegistration", "MembershipNumber", "TVWebPortalOR.xml"))).SendKeys(membershipId); //membership number

                SetDayValue(day);
                SetMonthValue(month);
                SetYearValue(year);

                driver.FindElement(By.Id(or.readingXMLFile("IETMemberRegistration", "Check", "TVWebPortalOR.xml"))).Click();


                #endregion


                #region Region to fill other details

                iWait.Until(ExpectedConditions.ElementIsVisible(By.Id(or.readingXMLFile("IETMemberRegistration", "IEtUserName", "TVWebPortalOR.xml"))));

                driver.FindElement(By.Id(or.readingXMLFile("IETMemberRegistration", "IEtUserName", "TVWebPortalOR.xml"))).SendKeys(completeDataList[5]);           //Entering Username
                driver.FindElement(By.Id(or.readingXMLFile("IETMemberRegistration", "IEtPassword", "TVWebPortalOR.xml"))).SendKeys(completeDataList[6]);           //Entering password
                driver.FindElement(By.Id(or.readingXMLFile("IETMemberRegistration", "IEtConfirmPassword", "TVWebPortalOR.xml"))).SendKeys(completeDataList[6]);   //Entering confirmPassword
                driver.FindElement(By.Id(or.readingXMLFile("IETMemberRegistration", "IEtEmailAddrees", "TVWebPortalOR.xml"))).SendKeys(testData["Column1"].ToString());   //Entering Email
                driver.FindElement(By.Id(or.readingXMLFile("IETMemberRegistration", "ConfirmEmail", "TVWebPortalOR.xml"))).SendKeys(testData["Column2"].ToString());   //Entering Confirm Email
                driver.FindElement(By.Id(or.readingXMLFile("IETMemberRegistration", "IetMemberContinue", "TVWebPortalOR.xml"))).Click();

                iWait.Until(ExpectedConditions.ElementIsVisible(By.Id(or.readingXMLFile("IETMemberRegistration", "BannerMessageOkButton", "TVWebPortalOR.xml"))));
                Assert.AreEqual("The email address is invalidOK", driver.FindElement(By.ClassName(or.readingXMLFile("IETMemberRegistration", "Warningmsg", "TVWebPortalOR.xml"))).Text.Trim());        //Verifying the Warning Message
                driver.FindElement(By.Id(or.readingXMLFile("IETMemberRegistration", "BannerMessageOkButton", "TVWebPortalOR.xml"))).Click();        //Clicking on OK button

                #endregion

                log.Info("TVWeb_008_EmailFormatValidation test completed" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

            }
            catch (Exception e)
            {
                log.Error(e.Message + "\n" + e.StackTrace + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());
                Assert.AreEqual(true, false);
            }
        }

        public static IEnumerable<object> EmailDataSource
        {

            get
            {
                int start_cnt = 0;

                log.Info("inside EmailDataSsource" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

                string ExcelDataPath = Environment.CurrentDirectory + "\\TestData_TV\\" + "NormalDataset.xlsx";

                FileStream stream = File.Open(ExcelDataPath, FileMode.Open, FileAccess.Read);

                log.Info("excel file open " + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());


                // 2. Reading from a OpenXml Excel file (2007 format; *.xlsx)
                Excel.IExcelDataReader excelReader = Excel.ExcelReaderFactory.CreateOpenXmlReader(stream);

                // 3. DataSet - The result of each spreadsheet will be created in the result.Tables

                DataSet result = excelReader.AsDataSet();

                // 4. DataSet - Create column names from first row
                excelReader.IsFirstRowAsColumnNames = true;

                System.Data.DataTable currentSheet = result.Tables[3];

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

        [Test]
        public void TVWeb_009_MandatoryFieldsValidation()
        {
            String expectedStyle = "border: 1px solid rgb(197, 0, 0); padding: 2px 1px;";
            String actualStyle = "";

            String surname = "Tech";
            String day = "19";
            String month = "11";
            String year = "1994";

            try
            {
                log.Info("TVWeb_009_MendatroyFieldsValidation test started" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

                #region Region to create membership Id

                membershipId = null;

                //Preparing data into required form
                List<String> completeDataList = GenerateCompleteData();

                membershipId = reusableMethods.darwinCRM_CreateCustomer(completeDataList);

                #endregion

                RedirectToRegistrationPage();  //Moving to Registration Page.

                uf.isJqueryActive(driver);

                iWait.Until(ExpectedConditions.ElementIsVisible(By.Id(or.readingXMLFile("IETMemberRegistration", "BtnYes", "TVWebPortalOR.xml"))));
                driver.FindElement(By.Id(or.readingXMLFile("IETMemberRegistration", "BtnYes", "TVWebPortalOR.xml"))).Click();   // Clicking on YES button

                iWait.Until(ExpectedConditions.ElementIsVisible(By.Id(or.readingXMLFile("IETMemberRegistration", "Check", "TVWebPortalOR.xml"))));
                driver.FindElement(By.Id(or.readingXMLFile("IETMemberRegistration", "Check", "TVWebPortalOR.xml"))).Click();   //Clicking on Continue button

                uf.isJqueryActive(driver);

                actualStyle = driver.FindElement(By.Id(or.readingXMLFile("IETMemberRegistration", "IEtSurname", "TVWebPortalOR.xml"))).GetAttribute("style");
                Assert.AreEqual(expectedStyle, actualStyle);                    //Checking red border appears around Surname field.

                actualStyle = driver.FindElement(By.Id(or.readingXMLFile("IETMemberRegistration", "MembershipNumber", "TVWebPortalOR.xml"))).GetAttribute("style");
                Assert.AreEqual(expectedStyle, actualStyle);                    //Checking red border appears around Member Ship ID field.

                iWait.Until(ExpectedConditions.ElementIsVisible(By.Id(or.readingXMLFile("IETMemberRegistration", "IEtSurname", "TVWebPortalOR.xml"))));
                driver.FindElement(By.Id(or.readingXMLFile("IETMemberRegistration", "IEtSurname", "TVWebPortalOR.xml"))).SendKeys(surname);     //entering  surName 
                driver.FindElement(By.Id(or.readingXMLFile("IETMemberRegistration", "MembershipNumber", "TVWebPortalOR.xml"))).SendKeys(membershipId); //entring membership number

                driver.FindElement(By.Id(or.readingXMLFile("IETMemberRegistration", "Check", "TVWebPortalOR.xml"))).Click();   //Clicking on Continue button

                //Verfiying Warning message for Date is displayed.
                iWait.Until(ExpectedConditions.VisibilityOfAllElementsLocatedBy(By.Id(or.readingXMLFile("IETMemberRegistration", "BannerMessageOkButton", "TVWebPortalOR.xml"))));

                Assert.AreEqual("Please enter a valid date.OK", driver.FindElement(By.ClassName(or.readingXMLFile("IETMemberRegistration", "Warningmsg", "TVWebPortalOR.xml"))).Text.Trim());        //Verifying the Warning Message

                element = driver.FindElement(By.Id(or.readingXMLFile("IETMemberRegistration", "BannerMessageOkButton", "TVWebPortalOR.xml")));

                Actions act = new Actions(driver);
                act.MoveToElement(element).Click().Build().Perform();


                // executor.ExecuteScript("arguments[0].click();", element);

                // driver.FindElement(By.Id("okButtonId")).Click();        //Clicking on OK button

                iWait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.Id(or.readingXMLFile("IETMemberRegistration", "BannerMessageOkButton", "TVWebPortalOR.xml"))));

                //Seleting DOB so that we can move to next page of registration
                SetDayValue(day);
                SetMonthValue(month);
                SetYearValue(year);

                driver.FindElement(By.Id(or.readingXMLFile("IETMemberRegistration", "Check", "TVWebPortalOR.xml"))).Click();   //Clicking on Continue button


                #region Region to Mandatory fields validation for second page of registration

                iWait.Until(ExpectedConditions.ElementIsVisible(By.Id(or.readingXMLFile("IETMemberRegistration", "IEtUserName", "TVWebPortalOR.xml"))));

                driver.FindElement(By.Id("CheckIetMemberUserName")).Click();    //CLicknig on Check USer button

                //Verfiying Warning message for Check USer button  is displayed.
                iWait.Until(ExpectedConditions.ElementIsVisible(By.Id(or.readingXMLFile("IETMemberRegistration", "BannerMessageOkButton", "TVWebPortalOR.xml"))));
                Assert.AreEqual("Please enter User NameOK", driver.FindElement(By.ClassName(or.readingXMLFile("IETMemberRegistration", "Warningmsg", "TVWebPortalOR.xml"))).Text.Trim());        //Verifying the Warning Message
                driver.FindElement(By.Id(or.readingXMLFile("IETMemberRegistration", "BannerMessageOkButton", "TVWebPortalOR.xml"))).Click();        //Clicking on OK button

                iWait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.Id(or.readingXMLFile("IETMemberRegistration", "BannerMessageOkButton", "TVWebPortalOR.xml"))));

                driver.FindElement(By.Id(or.readingXMLFile("IETMemberRegistration", "IetMemberContinue", "TVWebPortalOR.xml"))).Click();

                uf.isJqueryActive(driver);

                actualStyle = driver.FindElement(By.Id(or.readingXMLFile("IETMemberRegistration", "IEtUserName", "TVWebPortalOR.xml"))).GetAttribute("style");
                Assert.AreEqual(expectedStyle, actualStyle);                    //Checking red border appears around Surname field.

                actualStyle = driver.FindElement(By.Id(or.readingXMLFile("IETMemberRegistration", "IEtPassword", "TVWebPortalOR.xml"))).GetAttribute("style");
                Assert.AreEqual(expectedStyle, actualStyle);                    //Checking red border appears around Member Ship ID field.

                actualStyle = driver.FindElement(By.Id(or.readingXMLFile("IETMemberRegistration", "IEtConfirmPassword", "TVWebPortalOR.xml"))).GetAttribute("style");
                Assert.AreEqual(expectedStyle, actualStyle);                    //Checking red border appears around Member Ship ID field.


                actualStyle = driver.FindElement(By.Id(or.readingXMLFile("IETMemberRegistration", "IEtEmailAddrees", "TVWebPortalOR.xml"))).GetAttribute("style");
                Assert.AreEqual(expectedStyle, actualStyle);                    //Checking red border appears around Member Ship ID field.


                actualStyle = driver.FindElement(By.Id("ConfirmEmail")).GetAttribute("style");
                Assert.AreEqual(expectedStyle, actualStyle);                    //Checking red border appears around Member Ship ID field.


                #endregion

                log.Info("TVWeb_009_MendatroyFieldsValidation test completed" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());


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
            st.Chrome_TearDown(driver, log);                        // Calling Chrome Teardown
        }

    }
}
