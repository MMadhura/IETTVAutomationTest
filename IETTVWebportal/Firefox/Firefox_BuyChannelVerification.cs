//using NUnit.Framework;
//using OpenQA.Selenium;
//using OpenQA.Selenium.Chrome;
//using OpenQA.Selenium.Firefox;
//using OpenQA.Selenium.IE;
//using OpenQA.Selenium.Interactions;
//using OpenQA.Selenium.Support.UI;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading;
//using System.Threading.Tasks;
//using System.Windows.Forms;
//using System.Drawing;
//using Utility_Classes;
//using Utilities.Config;
//using Utilities.Object_Repository;
//using log4net;
//using log4net.Config;
//using System.Reflection;
//using System.IO;
//using System.Diagnostics;
//using NSoup.Nodes;  // for nsoup documents
//using NSoup.Select;  //for nsoup element
//using IETTVAdminPortal.Firefox;
//using Microsoft.Expression.Encoder.ScreenCapture;
//using IETTVWebportal.Reusable_Functions;

//namespace IETTVWebportal.Firefox
//{
//    [TestFixture]
//    class Firefox_BuyChannelVerification
//    {
//        #region Variable Decration and object initialistaion

//        internal IWebDriver driver = null;

//        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

//        string driverName = "", driverPath, appURL;

//        IJavaScriptExecutor executor;

//        IWait<IWebDriver> iWait;

//        String columData = null;
        
//        Boolean flag;

//        Utility_Functions uf = new Utility_Functions();                                         // Instantiate object for Utility Functons

//        Configuration cf = new Configuration();                                                 // Instantiate object for Configuration

//        Object_Repository_Class or = new Object_Repository_Class();                             // Instantiate object for object repository

//        // Instantiate object of admin portal
//        Firefox_IndividualChannelPricing icpAdmin = new Firefox_IndividualChannelPricing();

//        Firefox_WebSetupTearDown st = new Firefox_WebSetupTearDown();                           // Instantiate object for Firefox Setup Teardown

//        #endregion


//        #region SetUp

//        [TestFixtureSetUp]
//        public void FixtureSetUp()
//        {
//            log4net.Config.XmlConfigurator.Configure(new FileInfo(AppDomain.CurrentDomain.BaseDirectory + "WebPortal.config"));

//            log.Info("Inside Fixture Setup of Firefox - BottomBar Verification Test" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

//            uf.CreateOrReplaceVideoFolder();

//            driver = new FirefoxDriver();                                                       // Initialize Firefox driver 

//            iWait = new WebDriverWait(driver, TimeSpan.FromSeconds(60));

//            executor = (IJavaScriptExecutor)driver;
//        }

//        [SetUp]
//        public void SetUp()
//        {
//            appURL = st.Firefox_Setup(driver, log, executor);                                   // Calling Firefox Setup            
//        }

//        #endregion

//        #region Reusable Functions

//        public void RedirectToBuyChannel()
//        {
           
//            //iWait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy(By.Id("BuySubscription")));
//            iWait.Until(ExpectedConditions.ElementExists(By.Id(or.readingXMLFile("BuyChannelVerification", "BuySubscription", "TVWebPortalOR.xml"))));

//            //driver.FindElement(By.Id("BuySubscription")).FindElement(By.TagName("a")).Click();
//            driver.FindElement(By.Id(or.readingXMLFile("BuyChannelVerification", "BuySubscription", "TVWebPortalOR.xml"))).FindElement(By.TagName("a")).Click();            

//            //iWait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("div.text-center > button.btn.ok_btn_size")));  // Waiting for Popup window to appear after clicking on accept button
//            iWait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector(or.readingXMLFile("BuyChannelVerification", "AcceptButtonCss", "TVWebPortalOR.xml"))));  // Waiting for Popup window to appear after clicking on accept button            

//            //IList<IWebElement> btnOK = driver.FindElements(By.CssSelector("div.text-center > button.btn.ok_btn_size"));
//            IList<IWebElement> btnOK = driver.FindElements(By.CssSelector(or.readingXMLFile("BuyChannelVerification", "AcceptButtonCss", "TVWebPortalOR.xml")));
//            IWebElement element = btnOK.ElementAt(0);

//            executor.ExecuteScript("arguments[0].click();", element);

//            //iWait.Until(ExpectedConditions.TextToBePresentInElementLocated(By.ClassName("header-purchase"), "Purchase Channel Access"));
//            iWait.Until(ExpectedConditions.TextToBePresentInElementLocated(By.ClassName(or.readingXMLFile("BuyChannelVerification", "PurchaseChannel", "TVWebPortalOR.xml")), "Purchase Channel Access"));
//        }

//        //This function store all the container of the Individual Pricing 

//        public IList<IWebElement> GetRowListSelenium()
//        {
//            //IList<IWebElement> rowListSelenium = (IList<IWebElement>)driver.FindElements(By.ClassName("divPurchaseChannels"))[0].FindElements(By.ClassName("channel-widget-container"));
//            IList<IWebElement> rowListSelenium = (IList<IWebElement>)driver.FindElements(By.ClassName(or.readingXMLFile("BuyChannelVerification", "PurchaseDiv", "TVWebPortalOR.xml")))[0].FindElements(By.ClassName(or.readingXMLFile("BuyChannelVerification", "ChannelWidContainer", "TVWebPortalOR.xml")));
//            log.Info("count  : " + rowListSelenium.Count);
//            return rowListSelenium;

//       }

//        //This function search for the required Channel
//        public int FindRequiredChannel(Elements rowListNsoup, String channelName)
//        {

//            flag = false;

//            int rowcounter = 0;
//            foreach (Element currentRow in rowListNsoup)
//            {
//                log.Info("Row Counter :: " + rowcounter);

//                columData = currentRow.GetElementsByClass("channel-widget-border")[0].GetElementsByTag("div")[1].GetElementById("ChannelBlock" + rowcounter).OwnText().Trim();

//                if (columData.Equals(channelName))
//                {
//                    flag = true;
//                    log.Info("required channel name  : " + channelName);
//                    break;
//                }
//                rowcounter++;
//            }
//            return rowcounter;
//        }


//        #endregion

//        [Test]
//        public void TVWeb_001_VerifyChannelPriceDisplayed()
//        {
//            try
//            {
//                log.Info("TVWeb_001_VerifyChannelPriceDisplayed Started" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());


//                uf.isJqueryActive(driver);

//                RedirectToBuyChannel();

//                //Assert.AreEqual("Purchase Channel Access", driver.FindElement(By.ClassName("header-purchase")).Text.ToString());
//                Assert.AreEqual("Purchase Channel Access", driver.FindElement(By.ClassName(or.readingXMLFile("BuyChannelVerification", "PurchaseChannel", "TVWebPortalOR.xml"))).Text.ToString());


//                //need to open new tab pending
//                #region Call Admin portal to Update Channel price

//                //calling setup function
//                icpAdmin.FixtureSetUp();
//                icpAdmin.SetUp();

//                icpAdmin.RedirectToIndividualChannelPricing();

//                Elements rowListNsoup = icpAdmin.GetRowListNsoup();

//                log.Info("Getting Channel Name" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

//                String channelName = icpAdmin.GetChannelName();
//                log.Info("Channel Name  " + channelName);

//                icpAdmin.FindRequiredChannel(rowListNsoup, channelName);

//                //Enter random Archive price value 
//                string expectedArchive = icpAdmin.GetRandomPrice();
//                string expectedFront = icpAdmin.GetRandomPrice();
//                string expectedAll = icpAdmin.GetRandomPrice();

//                log.Info("Updating channel price" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

//                icpAdmin.UpdatingPrice(expectedArchive, expectedFront, expectedAll);

//                icpAdmin.VerifyBannerMessage();

//                icpAdmin.OverlayWait();

//                log.Info("verifying updated channel price" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

//                #endregion

//                #region Verify the Price changes on web portal

//                driver.Navigate().Refresh();

//                uf.isJqueryActive(driver);

//                IList<IWebElement> rowlistSelenium = GetRowListSelenium();

//                flag = false;

//                int rowcounter = 0;
//                foreach (IWebElement currentRow in rowlistSelenium)
//                {
//                    log.Info("Row Counter :: " + rowcounter);

//                    //columData = currentRow.FindElement(By.CssSelector("div.channel-widget-border > div.channel-widget-header")).Text.ToString();
//                    columData = currentRow.FindElement(By.CssSelector(or.readingXMLFile("BuyChannelVerification", "ChannelWidget", "TVWebPortalOR.xml"))).Text.ToString();

//                    log.Info("Channel Name  :" + columData);


//                    if (columData.Equals(channelName))  
//                    {
//                        flag = true;
                     

//                        String allValue = driver.FindElement(By.Id("ChannelBlock" + rowcounter + "btn1")).GetAttribute("price");
//                        log.Info("allValue :" + allValue);
//                        Assert.AreEqual(true, expectedAll.Contains(allValue)); //need to see

//                        String archiveValue = driver.FindElement(By.Id("ChannelBlock" + rowcounter + "btn2")).GetAttribute("price");
//                        Assert.AreEqual(true,expectedArchive.Contains(archiveValue));

//                        String frontValue = driver.FindElement(By.Id("ChannelBlock" + rowcounter + "btn3")).GetAttribute("price");
//                        Assert.AreEqual(true, expectedFront.Contains(frontValue));
                        

//                        break;
//                    }
//                    rowcounter++;
//                }

//                #endregion


//                log.Info("TVWeb_001_VerifyChannelPriceDisplayed completed" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

//                icpAdmin.TearDown();
//            }

//            catch (Exception e)
//            {
//                log.Error(e.Message + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());
//                Assert.AreEqual(true, false);
//            }
//        }
        
//        [Test]
//        public void TVWeb_002_VerifyBuyMoreMessage()
//        {
//            try
//            {
//                log.Info("TVWeb_002_VerifyBuyMoreMessage Started" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());


//                String TotalPriceContent = icpAdmin.GetTotalRandomPrice();
//                log.Info("Total price from admin : " + TotalPriceContent);

//                String MessageContent = "To gain FREE access to all IET.tv Technology Channels, why not become a member? IET Members receive free access as part of their membership package. Join the IET here or continue to Checkout.";

//                String MembershipUrlContent = cf.readingXMLFile("AdminPortal", "IndividualChannelPricing", "membershipUrl", "Config.xml");

//                uf.isJqueryActive(driver);

//                RedirectToBuyChannel();

//                //Assert.AreEqual("Purchase Channel Access", driver.FindElement(By.ClassName("header-purchase")).Text.ToString());
//                Assert.AreEqual("Purchase Channel Access", driver.FindElement(By.ClassName(or.readingXMLFile("BuyChannelVerification", "PurchaseChannel", "TVWebPortalOR.xml"))).Text.ToString());

//                #region Update the Total price and buy more message from admin portal

//                //calling setup function
//                icpAdmin.FixtureSetUp();
//                icpAdmin.SetUp();

//                icpAdmin.RedirectToIndividualChannelPricing();

//                //Enter content 
//                icpAdmin.EnterBuyMoreData(TotalPriceContent, MessageContent, MembershipUrlContent);

//                icpAdmin.VerifyBannerMessage();

//                #endregion

//                IList<IWebElement> rowlistSelenium = GetRowListSelenium();

//                flag = false;

//                int rowcounter = 0;
//                Decimal preAllValue = 0;

//                foreach (IWebElement currentRow in rowlistSelenium)
//                {

//                    log.Info("Row Counter :: " + rowcounter);

//                    #region Click on 'All' price and verify Buy More text

//                    icpAdmin.driver.Manage().Window.Size = new Size(-2000, 0);

//                    String allValue = driver.FindElement(By.Id("ChannelBlock" + rowcounter + "btn1")).GetAttribute("price");
//                    log.Info("allValue :" + allValue);

//                    iWait.Until(ExpectedConditions.ElementIsVisible(By.Id("ChannelBlock" + rowcounter + "btn1")));

//                    executor.ExecuteScript("arguments[0].click();", driver.FindElement(By.Id("ChannelBlock" + rowcounter + "btn1")));

//                    //iWait.Until(ExpectedConditions.ElementIsVisible(By.Id("spnTotal")));
//                    iWait.Until(ExpectedConditions.ElementIsVisible(By.Id(or.readingXMLFile("BuyChannelVerification", "SpanTotal", "TVWebPortalOR.xml"))));

//                    //String totalPrice = driver.FindElement(By.Id("spnTotal")).Text.Trim().ToString();
//                    String totalPrice = driver.FindElement(By.Id(or.readingXMLFile("BuyChannelVerification", "SpanTotal", "TVWebPortalOR.xml"))).Text.Trim().ToString();


//                    log.Info("total price content from Webportal:" + totalPrice);

//                    Assert.AreEqual(Convert.ToDecimal(allValue) + preAllValue, Convert.ToDecimal(totalPrice.Substring(totalPrice.LastIndexOf("£") + 1)));

//                    preAllValue = preAllValue + Convert.ToDecimal(allValue);

//                    if (Convert.ToDecimal(totalPrice.Substring(totalPrice.LastIndexOf("£") + 1)) >= Convert.ToDecimal(TotalPriceContent))
//                    {
//                        uf.scrollDown(driver);

//                        // verify Message is displayed 
//                        Assert.AreEqual(true, driver.FindElement(By.Id("divSubscription_message")).Displayed);
//                        Assert.AreEqual(true, driver.FindElement(By.Id(or.readingXMLFile("BuyChannelVerification", "DivSubscriptionMsg", "TVWebPortalOR.xml"))).Displayed);

//                        //Verify the Buy more message Content
//                        //Assert.AreEqual(MessageContent, driver.FindElement(By.Id("divSubscription_message")).Text.Trim().ToString());
//                        Assert.AreEqual(MessageContent, driver.FindElement(By.Id(or.readingXMLFile("BuyChannelVerification", "DivSubscriptionMsg", "TVWebPortalOR.xml"))).Text.Trim().ToString());                        

//                        //Verify 'Ok' button
//                        //Assert.AreEqual(true, driver.FindElement(By.Id("btnredirectMembershipPage")).Enabled);
//                        Assert.AreEqual(true, driver.FindElement(By.Id(or.readingXMLFile("BuyChannelVerification", "BtrRedirectMembershipPage", "TVWebPortalOR.xml"))).Enabled);

//                        //verify Checkout button
//                        //Assert.AreEqual(true, driver.FindElement(By.Id("btnCheckout")).Enabled);
//                        Assert.AreEqual(true, driver.FindElement(By.Id(or.readingXMLFile("BuyChannelVerification", "BtnCheckOut", "TVWebPortalOR.xml"))).Enabled);

//                        break;
//                    }

//                    else
//                    {
//                        log.Info("Avalible Price amount not sufficient to check buy more message" + "at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());
//                    }

//                    #endregion

//                    rowcounter++;
//                }


//                log.Info("TVWeb_002_VerifyBuyMoreMessage completed" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());
//                icpAdmin.TearDown();
            
//            }

//            catch (Exception e)
//            {
//                log.Error(e.Message + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());
//                Assert.AreEqual(true, false);
//            }
//        }

        
//        [TestFixtureTearDown]
//        public void TearDown()
//        {
//            st.Firefox_TearDown(driver, log);                                                   // Calling Firefox Teardown
//        }
//    }

//}
