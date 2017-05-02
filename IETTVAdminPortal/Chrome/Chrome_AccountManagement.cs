using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using log4net;
using OpenQA.Selenium.Support.UI;
using System.Reflection;
using NUnit.Framework;
using System.IO;
using Utility_Classes;
using OpenQA.Selenium.Chrome;
using System.Diagnostics;
using IETTVAdminportal.Reusable;
using IETTVWebportal.Chrome;
using Utilities.Object_Repository;
using System.Threading;
using Utilities.Config;
using OpenQA.Selenium.PhantomJS;

namespace IETTVAdminPortal.Chrome
{
    [TestFixture]
    public class Chrome_AccountManagement
    {
        private static ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        #region variable declaration and object initialisation

        internal IWebDriver driver = null;

        IJavaScriptExecutor executor;

        IWait<IWebDriver> iWait = null;

        Utility_Functions uf = new Utility_Functions();

        string driverName = "", driverPath, appURL;

        Chrome_AdminSetupTearDown st = new Chrome_AdminSetupTearDown();

        Object_Repository_Class OR = new Object_Repository_Class();

        Configuration cf = new Configuration();

        Chrome_AccountManagementVerification accountResult;

        #endregion

        public Chrome_AccountManagement()
        {

        }

        public Chrome_AccountManagement(IWebDriver driver, ILog log1, IJavaScriptExecutor Executor, IWait<IWebDriver> iWait)
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
            accountResult = new Chrome_AccountManagementVerification(driver, log, executor, iWait);            // Creating a object for calling IETTVWebPortal project

            appURL = st.Chrome_Setup(driver, log, executor);                                               // Calling Chrome Setup  
        }

        #endregion

        #region Reusable elements

        public IWebElement accountCreateTab()
        {
            iWait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy((OR.GetElement("AccountManagement", "AccountCreateTab", "TVAdminPortalOR.xml"))));

            return driver.FindElement((OR.GetElement("AccountManagement", "AccountCreateTab", "TVAdminPortalOR.xml")));
        }

        public IWebElement accountTypeDropdown()
        {
            iWait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy((OR.GetElement("AccountManagement", "AccountTypeDropdown", "TVAdminPortalOR.xml"))));

            return driver.FindElement((OR.GetElement("AccountManagement", "AccountTypeDropdown", "TVAdminPortalOR.xml")));
        }

        public IWebElement accountNameField()
        {
            iWait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy((OR.GetElement("AccountManagement", "AccountName", "TVAdminPortalOR.xml"))));

            return driver.FindElement((OR.GetElement("AccountManagement", "AccountName", "TVAdminPortalOR.xml")));
        }

        public IWebElement accountFirstNameField()
        {
            iWait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy((OR.GetElement("AccountManagement", "AccountFName", "TVAdminPortalOR.xml"))));

            return driver.FindElement((OR.GetElement("AccountManagement", "AccountFName", "TVAdminPortalOR.xml")));
        }

        public IWebElement accountLastNameField()
        {
            iWait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy((OR.GetElement("AccountManagement", "AccountLName", "TVAdminPortalOR.xml"))));

            return driver.FindElement((OR.GetElement("AccountManagement", "AccountLName", "TVAdminPortalOR.xml")));
        }

        public IWebElement accountContactEmailField()
        {
            iWait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy((OR.GetElement("AccountManagement", "ContactEmail", "TVAdminPortalOR.xml"))));

            return driver.FindElement((OR.GetElement("AccountManagement", "ContactEmail", "TVAdminPortalOR.xml")));
        }

        public IWebElement IETAccountfield()
        {
            iWait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy((OR.GetElement("AccountManagement", "IETAccountName", "TVAdminPortalOR.xml"))));

            return driver.FindElement((OR.GetElement("AccountManagement", "IETAccountName", "TVAdminPortalOR.xml")));
        }

        public IWebElement IETAccountEmailField()
        {
            iWait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy((OR.GetElement("AccountManagement", "IETAccountEmail", "TVAdminPortalOR.xml"))));

            return driver.FindElement((OR.GetElement("AccountManagement", "IETAccountEmail", "TVAdminPortalOR.xml")));
        }

        public IWebElement IPRangerdbfield()
        {
            iWait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy((OR.GetElement("AccountManagement", "IPRangeRadionBtn", "TVAdminPortalOR.xml"))));

            return driver.FindElement((OR.GetElement("AccountManagement", "IPRangeRadionBtn", "TVAdminPortalOR.xml")));
        }

        public IWebElement AccountSaveBtn()
        {
            iWait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy((OR.GetElement("AccountManagement", "AccountSaveBtn", "TVAdminPortalOR.xml"))));

            return driver.FindElement((OR.GetElement("AccountManagement", "AccountSaveBtn", "TVAdminPortalOR.xml")));
        }

        public IWebElement accountSearchTypeDRD()
        {
            iWait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy((OR.GetElement("AccountManagement", "AccountSearchtype", "TVAdminPortalOR.xml"))));

            return driver.FindElement((OR.GetElement("AccountManagement", "AccountSearchtype", "TVAdminPortalOR.xml")));
        }

        public IWebElement SearchAccountNameField()
        {
            iWait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy((OR.GetElement("AccountManagement", "SearchAccountName", "TVAdminPortalOR.xml"))));

            return driver.FindElement((OR.GetElement("AccountManagement", "SearchAccountName", "TVAdminPortalOR.xml")));
        }

        public IWebElement AccountSearchBTN()
        {
            iWait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy((OR.GetElement("AccountManagement", "AccountSearchBTN", "TVAdminPortalOR.xml"))));

            return driver.FindElement((OR.GetElement("AccountManagement", "AccountSearchBTN", "TVAdminPortalOR.xml")));
        }

        public IWebElement ChannelList()
        {
            iWait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy((OR.GetElement("AccountManagement", "ChannelList", "TVAdminPortalOR.xml"))));

            return driver.FindElement((OR.GetElement("AccountManagement", "ChannelList", "TVAdminPortalOR.xml")));
        }

        public IWebElement SelectCatalogueList()
        {
            iWait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy((OR.GetElement("AccountManagement", "SelectCatalogue", "TVAdminPortalOR.xml"))));

            return driver.FindElement((OR.GetElement("AccountManagement", "SelectCatalogue", "TVAdminPortalOR.xml")));
        }

        public IWebElement SelectSubscriptionType()
        {
            iWait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy((OR.GetElement("AccountManagement", "SelectSubscriptionType", "TVAdminPortalOR.xml"))));

            return driver.FindElement((OR.GetElement("AccountManagement", "SelectSubscriptionType", "TVAdminPortalOR.xml")));
        }

        public IWebElement SubscriptionAddBTN()
        {
            iWait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy((OR.GetElement("AccountManagement", "SubscriptionAddBTN", "TVAdminPortalOR.xml"))));

            return driver.FindElement((OR.GetElement("AccountManagement", "SubscriptionAddBTN", "TVAdminPortalOR.xml")));
        }

        public IWebElement SubscriptionSaveBtn()
        {
            iWait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy((OR.GetElement("AccountManagement", "SubscriptionSaveBtn", "TVAdminPortalOR.xml"))));

            return driver.FindElement((OR.GetElement("AccountManagement", "SubscriptionSaveBtn", "TVAdminPortalOR.xml")));
        }

        public IWebElement AccessDetailsTab()
        {
            iWait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy((OR.GetElement("AccountManagement", "AccessDetailsTab", "TVAdminPortalOR.xml"))));

            return driver.FindElement((OR.GetElement("AccountManagement", "AccessDetailsTab", "TVAdminPortalOR.xml")));
        }

        //public IWebElement mySubcriptionContent()
        //{
        //    iWait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy((OR.GetElement("AccountManagement", "SubscriptionContent", "TVAdminPortalOR.xml"))));

        //    return driver.FindElement((OR.GetElement("AccountManagement", "SubscriptionContent", "TVAdminPortalOR.xml")));
        //}

        #endregion

        #region Reusable function

        public void OverlayWait()
        {
            iWait.Until(ExpectedConditions.InvisibilityOfElementLocated((OR.GetElement("SeriesManagement", "OverlayWait", "TVAdminPortalOR.xml"))));
        }

        public void verifySuccessBannerMessage(String message)
        {
            iWait.Until(ExpectedConditions.ElementIsVisible((OR.GetElement("SeriesManagement", "SuccessOkButton", "TVAdminPortalOR.xml"))));

            iWait.Until(d => d.FindElement((OR.GetElement("SeriesManagement", "SucessMessage", "TVAdminPortalOR.xml"))).Text.Equals(message));

            Assert.AreEqual(message, driver.FindElement((OR.GetElement("SeriesManagement", "SucessMessage", "TVAdminPortalOR.xml"))).Text.Trim());

            //Click on ok button banner message

            Thread.Sleep(2000);

            driver.FindElement((OR.GetElement("SeriesManagement", "SuccessOkButton", "TVAdminPortalOR.xml"))).Click();

            OverlayWait();

        }

        #endregion

        [Test]
        public void TVAdmin_001_AccManagementandSubscription()
        {
            log.Info("TVAdmin_001_AccManagementandSubscription test started" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

            accountCreateTab().Click();

            AccountDetails();

            OverlayWait();

            Thread.Sleep(3000);

            recentAccountSearch();

            createRecentAccountAccess();

            verifyRecentSubscriptionDetails();

            uf.OpenNewTab(driver);

            String browsertype = uf.getRunningBrowser(driver, driver.FindElement(By.TagName("html")));

            uf.SwitchToWebTab(driver, browsertype);

            //yopmail
            uf.NavigateYopMail(cf, driver);

            getPasswordFromMail();

            uf.SwitchToWebTab(driver, browsertype);

            accountResult.switchToWebPortal();

            handlePopup();

            accountResult.AccountLogin();

            accountResult.ClickYesButton();

            OverlayWait();

            accountResult.WelcomeMessage();

            accountResult.ClickMyIETTVSubscription();

            checkMySubscription();

            accountResult.Logout();

            log.Info("TVAdmin_001_AccManagementandSubscription test completed" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());
        }

        public void AccountDetails()
        {
            log.Info("inside AccountDetails " + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());
            //select account type from dropdown
            SelectElement AccountTypeSelector = new SelectElement(accountTypeDropdown());

            String accountType = cf.readingXMLFile("AdminPortal", "Account Management", "accountTypeDrd", "Config.xml");

            AccountTypeSelector.SelectByText(accountType);

            // enter value in account name field
            string accountName = "AutoAccount" + uf.getShortGuid();

            accountNameField().SendKeys(accountName);

            cf.writingIntoXML("Admin", "Account Management", "accountname", accountName, "TestCopy.xml");

            //enter value in first name field
            string accountFirstName = "Auto FName" + uf.getShortGuid();

            accountFirstNameField().SendKeys(accountFirstName);

            //enter value in last name field
            string accountLastName = "Auto LName" + uf.getShortGuid();

            accountLastNameField().SendKeys(accountLastName);

            //enter value in contact email field
            string accountEmail = cf.readingXMLFile("Admin", "Account Management", "accountname", "TestCopy.xml");

            string UserEmailAddress = accountEmail + "@yopmail.com";

            cf.writingIntoXML("Admin", "Account Management", "UserEmailAddress", UserEmailAddress, "TestCopy.xml");

            accountContactEmailField().SendKeys(UserEmailAddress);

            //enter value in IET Account Manager Name
            string IETAccountName = "IETAutoAccount" + uf.getShortGuid();

            IETAccountfield().SendKeys(IETAccountName);

            cf.writingIntoXML("Admin", "Account Management", "IETAccountName", IETAccountName, "TestCopy.xml");

            string IETAccountEmail = cf.readingXMLFile("Admin", "Account Management", "IETAccountName", "TestCopy.xml") + "@yopmail.com";

            IETAccountEmailField().SendKeys(IETAccountEmail);

            //select Non-static radio button
            IPRangerdbfield().Click();

            OverlayWait();

            // click save btn
            AccountSaveBtn().Click();

            Thread.Sleep(3000);

            accountResult.VerifySuccessBannerMsg("Account created successfully");

        }

        public void recentAccountSearch()
        {
            log.Info("inside recentAccountSearch " + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());
            //search account name
            SelectElement AccountSearchdrd = new SelectElement(accountSearchTypeDRD());

            String accountSearchType = cf.readingXMLFile("AdminPortal", "Account Management", "accountTypeDrd", "Config.xml");

            AccountSearchdrd.SelectByText(accountSearchType);

            SearchAccountNameField().SendKeys(cf.readingXMLFile("Admin", "Account Management", "accountname", "TestCopy.xml"));

            //click search button
            AccountSearchBTN().Click();

            recentAccountSubscription();

        }

        public void recentAccountSubscription()
        {
            log.Info("inside recentAccountSubscription " + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

            IList<IWebElement> accountRowList;

            Thread.Sleep(3000);
            iWait.Until(ExpectedConditions.ElementExists((OR.GetElement("AccountManagement", "RecentAccountListing", "TVAdminPortalOR.xml"))));

            try
            {
                Thread.Sleep(3000);
                IWebElement tblAccountListing = driver.FindElement((OR.GetElement("AccountManagement", "RecentAccountListing", "TVAdminPortalOR.xml")));

                iWait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy(By.TagName("tr")));

                accountRowList = (IList<IWebElement>)tblAccountListing.FindElements(By.TagName("tr"));
            }
            catch (Exception e)
            {
                log.Info("Row detection failed at first instance for My video table");

                Thread.Sleep(2000);  // Row detection failed at first instance thus retrying after wait

                IWebElement tblAccountListing = driver.FindElement((OR.GetElement("AccountManagement", "RecentAccountListing", "TVAdminPortalOR.xml")));

                iWait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy(By.TagName("tr")));

                accountRowList = (IList<IWebElement>)tblAccountListing.FindElements(By.TagName("tr"));

            }

            int i = 0;

            foreach (IWebElement currentrow in accountRowList)
            {
                if (currentrow.GetAttribute("class").Equals("GridRowStyle") || currentrow.GetAttribute("class").Equals("AltGridStyle"))
                {
                    string columnAccountName = driver.FindElements(By.TagName("td"))[1].FindElement(By.ClassName("lblLink")).Text.Trim();

                    string SearchAccountName = cf.readingXMLFile("Admin", "Account Management", "accountname", "TestCopy.xml");

                    if (SearchAccountName.Equals(columnAccountName))
                    {
                        iWait.Until(ExpectedConditions.ElementToBeClickable(OR.GetElement("AccountManagement", "SubscriptionBtn", "TVAdminPortalOR.xml")));

                        Thread.Sleep(5000);

                        IWebElement SubscriptionBtn = driver.FindElement(OR.GetElement("AccountManagement", "SubscriptionBtn", "TVAdminPortalOR.xml"));
                        SubscriptionBtn.Click();

                        break;
                    }
                    i++;

                }

            }

        }

        public void createRecentAccountAccess()
        {
            log.Info("inside createRecentAccountAccess " + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

            accountCreateTab().Click();

            //SelectElement from channel list
            SelectElement selectChannel = new SelectElement(ChannelList());
            String selectChannelName = cf.readingXMLFile("Admin", "Channel Management", "channelname", "TestCopy.xml");
            selectChannel.SelectByText(selectChannelName);

            //select catalogue
            Thread.Sleep(5000);
            SelectElement selectCatalogue = new SelectElement(SelectCatalogueList());
            String selectCatalogueYear = "2002";
            selectCatalogue.SelectByText(selectCatalogueYear);

            //select subscription type
            Thread.Sleep(5000);
            SelectElement selectSubscription = new SelectElement(SelectSubscriptionType());
            String subscriptionType = cf.readingXMLFile("AdminPortal", "Account Management", "SubscriptionType", "Config.xml");
            selectSubscription.SelectByText(subscriptionType);

            Thread.Sleep(3000);
            SubscriptionAddBTN().Click();

            OverlayWait();

            SubscriptionSaveBtn().Click();

            Thread.Sleep(2000);

            //Verify Subscription saved message
            verifySuccessBannerMessage("Record saved successfully");

        }

        public void verifyRecentSubscriptionDetails()
        {
            log.Info("inside verifyRecentSubscriptionDetails " + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

            IList<IWebElement> subscriptionList;

            Thread.Sleep(2000);

            iWait.Until(ExpectedConditions.ElementExists((OR.GetElement("AccountManagement", "grdSubscriptionDetails", "TVAdminPortalOR.xml"))));

            IWebElement tblSubscriptionListing = driver.FindElement((OR.GetElement("AccountManagement", "grdSubscriptionDetails", "TVAdminPortalOR.xml")));

            iWait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy(By.TagName("tr")));

            subscriptionList = (IList<IWebElement>)tblSubscriptionListing.FindElements(By.TagName("tr"));

            foreach (IWebElement currentrow in subscriptionList)
            {
                if (currentrow.GetAttribute("class").Equals("GridRowStyle") || currentrow.GetAttribute("class").Equals("AltGridStyle"))
                {
                    string columnChannelName = driver.FindElements(By.TagName("td"))[1].FindElement(By.TagName("li")).Text.Trim();

                    string ChannelName = cf.readingXMLFile("Admin", "Channel Management", "channelname", "TestCopy.xml");

                    Assert.AreEqual(ChannelName, columnChannelName);

                    break;
                }
            }

        }

        public void getPasswordFromMail()
        {
            log.Info("inside getPasswordFromMail " + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());
            #region getting password from mail

            iWait.Until(ExpectedConditions.ElementIsVisible(By.Id("login")));

            string accountEmailAddress = cf.readingXMLFile("Admin", "Account Management", "UserEmailAddress", "TestCopy.xml");

            IWebElement YopUsername = driver.FindElement(By.Id("login"));
            YopUsername.SendKeys(accountEmailAddress);
            
            //YopUsername.SendKeys("madhura123@yopmail.com");

            IWebElement submitBtn = driver.FindElement(By.CssSelector("input.sbut"));
            submitBtn.Click();

            //Thread.Sleep(150000);
            WebDriverWait wait = new WebDriverWait(driver, new TimeSpan(0, 2, 30));
            wait.Until(ExpectedConditions.ElementIsVisible(By.Id("ifmail")));

            IWebElement mailFrame = driver.FindElement(By.Id("ifmail"));
            driver.SwitchTo().Frame(mailFrame);

            Thread.Sleep(3000);

            IWebElement mailText = driver.FindElement(By.Id("mailmillieu"));

            string[] passwordText = mailText.Text.Split(':');

            string finalPassword = passwordText[3].Substring(1, 12);

            cf.writingIntoXML("Admin", "Account Management", "Password", finalPassword, "TestCopy.xml");

            #endregion
        }

        public void checkMySubscription()
        {
            log.Info("inside checkMySubscription " + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

            Thread.Sleep(5000);

            iWait.Until(ExpectedConditions.ElementExists((OR.GetElement("AccountManagement", "SubscriptionContent", "TVWebPortalOR.xml"))));

            iWait.Until(ExpectedConditions.ElementExists((OR.GetElement("AccountManagement", "SubscriptionTable", "TVWebPortalOR.xml"))));

            IList<IWebElement> accountRecordList;

            IWebElement subscriptionListing = driver.FindElement(OR.GetElement("AccountManagement", "SubscriptionTable", "TVWebPortalOR.xml"));

            iWait.Until(ExpectedConditions.ElementExists(By.TagName("tr")));

            accountRecordList = subscriptionListing.FindElements(By.TagName("tr"));

            int counter = 0;
            foreach (IWebElement subscriptionList in accountRecordList)
            {
                if (counter >= 1)
                {
                    String columData = subscriptionList.FindElements((OR.GetElement("UserGeneratedContent", "StatusColumnData", "TVWebPortalOR.xml")))[0].FindElement(By.TagName("a")).Text.Trim();


                    string ChannelName = cf.readingXMLFile("Admin", "Channel Management", "channelname", "TestCopy.xml");

                    Assert.AreEqual(ChannelName, columData);

                    IWebElement a = driver.FindElements(OR.GetElement("UserGeneratedContent", "StatusColumnData", "TVWebPortalOR.xml"))[0];
                    a.Click();

                    Thread.Sleep(3000);

                    break;
                }
                else
                {
                    counter++;
                }
            }


        }

        public void handlePopup()
        {
            log.Info("inside handlePopup " + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

            Thread.Sleep(4000);

            accountResult.handlePromotionalPopup();

            accountResult.HandleEmergencyPopUp();
        }

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
