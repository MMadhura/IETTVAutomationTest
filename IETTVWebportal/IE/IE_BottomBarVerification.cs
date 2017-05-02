using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.IE;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using OpenQA.Selenium.Remote;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using Utility_Classes;
using Utilities.Config;
using Utilities.Object_Repository;
using log4net;
using log4net.Config;
using System.Reflection;
using System.IO;
using System.Diagnostics;
using Microsoft.Expression.Encoder.ScreenCapture;
using IETTVWebportal.Reusable_Functions;

namespace IETTVWebportal.IE
{
    [TestFixture]
    class IE_BottomBarVerification
    {
        internal IWebDriver driver = null;

        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        string driverName = "", driverPath, appURL;

        IWait<IWebDriver> iWait = null;

        IJavaScriptExecutor executor;

        Utility_Functions uf = new Utility_Functions();                                         // Instantiate object for Utility Functons

        Configuration cf = new Configuration();                                                 // Instantiate object for Configuration

        Object_Repository_Class or = new Object_Repository_Class();                             // Instantiate object for object repository.

        IE_WebSetupTearDown st = new IE_WebSetupTearDown();                                     // Instantiate object for IE Setup Teardown

        [TestFixtureSetUp]
        public void FixtureSetUp()
        {
           
             log4net.Config.XmlConfigurator.Configure(new FileInfo(AppDomain.CurrentDomain.BaseDirectory + "WebPortal.config"));

             log.Info("Inside Fixture Setup of IE - BottomBar Verification Test" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

             uf.CreateOrReplaceVideoFolder();

             string baseDir = AppDomain.CurrentDomain.BaseDirectory;            

             driverName = "webdriver.ie.driver";                                                // Driver name for IE

             driverPath = baseDir + "/IEDriverServer.exe";                                      // path for IE Driver
             
             System.Environment.SetEnvironmentVariable(driverName, driverPath);

             InternetExplorerOptions opt = new InternetExplorerOptions();                       // Ensuring Clean IE session

             opt.EnsureCleanSession = true;

             driver = new InternetExplorerDriver(opt);                                          // Initialize IE driver              

             if (uf.getRunningBrowser(driver, driver.FindElement(By.TagName("html"))).ToString().Equals("IE11"))
             {
                  
                 log.Info("IE11 detected in setup" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

                 // Check if Registry Entry is present for IE 11 browser

                 if (uf.checkIE11RegistryPresence().Equals("true"))
                 {
                       log.Info("Registry Created successfully / Present for IE 11" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());
                 }
                 else
                 {
                        log.Info("Registry couldn't be created. Test may not run properly in IE 11. Please contact administrator" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());
                 }

            }

            iWait = new WebDriverWait(driver, TimeSpan.FromSeconds(60));

            executor = (IJavaScriptExecutor)driver;   
        }


        [SetUp]
        public void SetUp()
        {
            appURL = st.IE_Setup(driver, log, executor);    
        }

        [Test]
        public void TVWeb_001_VerifyBottomBar()
        {

            try
            {

                IWebElement backToTop;

                IJavaScriptExecutor executor;

                String myCommunityURL, faceBookURL, twitterURL, linkedinURL, youTubeURL, myCommunityStatus, facebookStatus, twitterStatus, linkedinStatus, youTubeStatus, myIETHeader,
                IETTvHelp, TvAccType, othIETSites, abtLinksURL, othLinksURL, bottomFooterURL, abtLinkStatus, othLinkStatus, IEThelpURL, IEThelpStatus, IETAccTypeURL, IETAccTypeStatus, bottomLinkStatus, copyRightText;

                IList<IWebElement> myCommunityLinks, myCommunity, facebook, twitter, linkedin, youTube, footerSection, footerFourSections, abtIET, abtHelp, abtAccType, abtOthSites, bottomFooter, bottomFooterList;

                String[] arrAbtIET = { "Vision, mission & values", "People", "Our offices & venues", "Savoy Place upgrade", "Working at the IET" };
                String[] arrIETHelp = { "FAQ", "Institution", "Technical requirements", "Contact", "Corporate", "Forgot password", "Delegate / Speaker access code" };
                String[] arrAccType = { "Institution", "Members", "Visitor", "Corporate", "Individual" };
                String[] arrOthSites = { "The IET", "E&T Jobs", "E&T Magazine", "", "IET Connect", "IET Digital Library", "IET Electrical", "IET Faraday", "IET Venues" };
                String[] arrBottomFooter = { "Cookies", "Privacy Statement", "Accessibility", "Legal Notices", "T&C" };


                int ScrollTop, cntFooterSections, cntGlobal = 0;

                WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(60));

                wait.Until(ExpectedConditions.ElementExists(By.ClassName(or.readingXMLFile("BottomBar", "overLaySpinner", "TVWebPortalOR.xml"))));

                Thread.Sleep(2000);

                Boolean resScrollDown = uf.scrollDown(driver);

                log.Info("Scroll Down Result=" + resScrollDown + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

                Thread.Sleep(5000);

                // Verify scroll down is performed

                Assert.AreEqual(true, resScrollDown);

                backToTop = driver.FindElement(By.Id(or.readingXMLFile("BottomBar", "backToTop", "TVWebPortalOR.xml"))).FindElement(By.TagName("a"));

                // Verify Back to Top anchor is present

                Assert.AreEqual("Click to Go Top", backToTop.GetAttribute("title").ToString());

                // Click on Back to Top anchor

                executor = (IJavaScriptExecutor)driver;
                executor.ExecuteScript("arguments[0].click();", backToTop);

                Thread.Sleep(2000);

                ScrollTop = uf.getScrollTop(driver);

                // Verify Back to Top functionality

                Assert.AreEqual(0, ScrollTop);

                Thread.Sleep(2000);

                // Once again scroll down for link verification 

                uf.scrollDown(driver);

                Thread.Sleep(2000);

                //////// Verify My Community Icon Presence and Status ///////

                myCommunityLinks = driver.FindElement(By.ClassName(or.readingXMLFile("BottomBar", "myCommunityLinks", "TVWebPortalOR.xml"))).FindElements(By.TagName("li"));

                log.Info("Total icon List" + myCommunityLinks.Count + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

                // Verify Total No. of My community icons

                Assert.AreEqual(5, myCommunityLinks.Count);

                // Verify MyCommunity section is present

                myCommunity = myCommunityLinks.ElementAt(0).FindElements(By.TagName("a"));

                Assert.AreEqual(1, myCommunity.Count);

                myCommunityURL = myCommunity.ElementAt(0).GetAttribute("href").ToString();

                log.Info("My Community URL:" + myCommunityURL + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

                // Verify MyCommunity URL

                Assert.AreEqual("http://mycommunity.theiet.org/?origin=foot-social", myCommunityURL);

                // Verify MyCommunity Request Status                  

                myCommunityStatus = uf.getStatusCode(new Uri(myCommunityURL.ToString()));

                Assert.AreEqual("OK", myCommunityStatus);

                // Verify MyCommunity icon title text

                Assert.AreEqual("My Community icon", myCommunity.ElementAt(0).FindElement(By.TagName("img")).GetAttribute("title").ToString());

                // Verify MyCommunity text

                Assert.AreEqual("MyCommunity", myCommunity.ElementAt(0).FindElement(By.TagName("span")).Text.ToString());

                // Verify Facebook icon is present and it's status

                facebook = myCommunityLinks.ElementAt(1).FindElements(By.TagName("a"));

                faceBookURL = facebook.ElementAt(0).GetAttribute("href").ToString();

                log.Info("Facebook URL:" + faceBookURL + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

                // Verify Facebook URL

                Assert.AreEqual("http://www.theiet.org/policy/media/follow/facebook.cfm?origin=foot-social", faceBookURL);

                // Verify Facebook Request Status

                facebookStatus = uf.getStatusCode(new Uri(faceBookURL.ToString()));

                Assert.AreEqual("OK", facebookStatus);

                // Verify Facebook icon title text

                Assert.AreEqual("Facebook icon", facebook.ElementAt(0).FindElement(By.TagName("img")).GetAttribute("title").ToString());

                // Verify Twitter icon is present and it's status

                twitter = myCommunityLinks.ElementAt(2).FindElements(By.TagName("a"));

                twitterURL = twitter.ElementAt(0).GetAttribute("href").ToString();

                log.Info("Twitter URL:" + twitterURL + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

                // Verify Twitter URL

                Assert.AreEqual("http://www.theiet.org/policy/media/follow/twitter.cfm?origin=foot-social", twitterURL);

                // Verify Twitter Request Status

                twitterStatus = uf.getStatusCode(new Uri(twitterURL.ToString()));

                Assert.AreEqual("OK", twitterStatus);

                // Verify Twitter icon title text

                Assert.AreEqual("Twitter icon", twitter.ElementAt(0).FindElement(By.TagName("img")).GetAttribute("title").ToString());

                // Verify Linkedin icon is present and it's status

                linkedin = myCommunityLinks.ElementAt(3).FindElements(By.TagName("a"));

                linkedinURL = linkedin.ElementAt(0).GetAttribute("href").ToString();

                log.Info("Linkedin URL:" + linkedinURL + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

                // Verify Linkedin URL

                Assert.AreEqual("http://www.theiet.org/policy/media/follow/linkedin.cfm?origin=foot-social", linkedinURL);

                // Verify Linkedin Request Status

                linkedinStatus = uf.getStatusCode(new Uri(linkedinURL.ToString()));

                Assert.AreEqual("OK", linkedinStatus);

                // Verify Linkedin icon title text

                Assert.AreEqual("LinkedIn icon", linkedin.ElementAt(0).FindElement(By.TagName("img")).GetAttribute("title").ToString());

                // Verify Youtube icon is present and it's status

                youTube = myCommunityLinks.ElementAt(4).FindElements(By.TagName("a"));

                youTubeURL = youTube.ElementAt(0).GetAttribute("href").ToString();

                log.Info("YouTube URL:" + youTubeURL + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

                // Verify YouTube URL

                Assert.AreEqual("http://www.theiet.org/policy/media/follow/youtube.cfm?origin=foot-social", youTubeURL);

                // Verify YouTube Request Status

                youTubeStatus = uf.getStatusCode(new Uri(youTubeURL.ToString()));

                Assert.AreEqual("OK", youTubeStatus);

                // Verify four sections are present in footer

                footerSection = driver.FindElements(By.CssSelector(or.readingXMLFile("BottomBar", "footerFourSections", "TVWebPortalOR.xml")));

                cntFooterSections = footerSection.Count();

                Assert.AreEqual(1, cntFooterSections);

                footerFourSections = footerSection.ElementAt(0).FindElements(By.TagName("div"));

                log.Info("Total Footer Sections:" + footerFourSections.Count + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

                Assert.AreEqual(4, footerFourSections.Count);

                // {This needs to be taken from Test Data - Currently it is mentioned in test script}

                // Verify About the IET section

                myIETHeader = footerFourSections.ElementAt(0).FindElement(By.TagName("h4")).Text.ToString();

                log.Info("My IET Header:" + myIETHeader + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

                // Verify My IET section header text

                Assert.AreEqual("About the IET", myIETHeader);

                abtIET = footerFourSections.ElementAt(0).FindElement(By.TagName("ul")).FindElements(By.TagName("li"));

                // Verify expected no. of links are present under About the IET

                Assert.AreEqual(5, abtIET.Count());

                // Verifying all links present under 'About the IET' section                                                         

                foreach (String val in arrAbtIET)
                {

                    log.Info("Verifying " + val + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

                    Assert.AreEqual(abtIET.ElementAt(cntGlobal).FindElement(By.TagName("a")).Text, val);

                    abtLinksURL = abtIET.ElementAt(cntGlobal).FindElement(By.TagName("a")).GetAttribute("href").ToString();

                    abtLinkStatus = uf.getStatusCode(new Uri(myCommunityURL.ToString()));

                    Assert.AreEqual("OK", abtLinkStatus);

                    cntGlobal++;
                }

                // Verify IET.tV help section

                IETTvHelp = footerFourSections.ElementAt(1).FindElement(By.TagName("h4")).Text.ToString();

                log.Info("IET TV Help:" + IETTvHelp + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

                // Verify IET TV. help section header text

                Assert.AreEqual("IET.tv help", IETTvHelp);

                cntGlobal = 0;

                abtHelp = footerFourSections.ElementAt(1).FindElement(By.TagName("ul")).FindElements(By.TagName("li"));

                // Verify expected no. of links are present under 'IET.tv help'

                Assert.AreEqual(7, abtHelp.Count());

                // Verifying all links present under 'IET.tv help' section                                                  

                foreach (String val in arrIETHelp)
                {
                    log.Info("Verifying " + val + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

                    Assert.AreEqual(abtHelp.ElementAt(cntGlobal).FindElement(By.TagName("a")).Text, val);

                    String newVal = val.Replace(" ", "-");

                    if (cntGlobal == 0)
                    {

                        IEThelpURL = driver.Url.ToString() + "?" + newVal.ToLower();
                    }
                    else if (cntGlobal == 1 || cntGlobal == 4)
                    {
                        IEThelpURL = driver.Url.ToString() + "?services=h" + newVal.ToLower();
                    }
                    else
                    {
                        IEThelpURL = driver.Url.ToString() + "?services=" + newVal.ToLower();
                    }

                    log.Info("IET HELP URL:= " + IEThelpURL + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

                    IEThelpStatus = uf.getStatusCode(new Uri(IEThelpURL.ToString()));

                    Assert.AreEqual("OK", IEThelpStatus);

                    cntGlobal++;
                }

                // Verify IET.tv account types section

                TvAccType = footerFourSections.ElementAt(2).FindElement(By.TagName("h4")).Text.ToString();

                log.Info("IET TV Account Types:" + TvAccType + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

                // Verify IET TV. account types header text

                Assert.AreEqual("IET.tv account types", TvAccType);

                cntGlobal = 0;

                abtAccType = footerFourSections.ElementAt(2).FindElement(By.TagName("ul")).FindElements(By.TagName("li"));

                // Verify expected no. of links are present under 'IET.tv account types'

                Assert.AreEqual(5, abtAccType.Count());

                // Verifying all links present under 'IET.tv help' section

                foreach (String val in arrAccType)
                {
                    log.Info("Verifying " + val + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

                    Assert.AreEqual(abtAccType.ElementAt(cntGlobal).FindElement(By.TagName("a")).Text, val);

                    String newVal = val.Replace(" ", "-");

                    if (cntGlobal == 0 || cntGlobal == 3)
                    {

                        IETAccTypeURL = driver.Url.ToString() + "?services=a" + newVal.ToLower();
                    }
                    else
                    {
                        IETAccTypeURL = driver.Url.ToString() + "?services=" + newVal.ToLower();
                    }

                    log.Info("IET Acc Type URL:= " + IETAccTypeURL + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

                    IETAccTypeStatus = uf.getStatusCode(new Uri(IETAccTypeURL.ToString()));

                    Assert.AreEqual("OK", IETAccTypeStatus);

                    cntGlobal++;
                }


                // Verify Other IET websites section

                othIETSites = footerFourSections.ElementAt(3).FindElement(By.TagName("h4")).Text.ToString();

                log.Info("Other IET WebSites:" + othIETSites + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

                // Verify IET TV. help section header text

                Assert.AreEqual("Other IET websites", othIETSites);

                cntGlobal = 0;

                abtOthSites = footerFourSections.ElementAt(3).FindElement(By.TagName("ul")).FindElements(By.TagName("li"));

                Assert.AreEqual(9, abtOthSites.Count());

                // Verifying all links present under 'About the IET' section 

                foreach (String val in arrOthSites)
                {

                    if (cntGlobal != 3)
                    {
                        log.Info("Verifying " + val + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

                        Assert.AreEqual(val, abtOthSites.ElementAt(cntGlobal).FindElement(By.TagName("a")).Text);

                        othLinksURL = abtOthSites.ElementAt(cntGlobal).FindElement(By.TagName("a")).GetAttribute("href").ToString();

                        log.Info("Other IET Websites:= " + othLinksURL + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

                        othLinkStatus = uf.getStatusCode(new Uri(othLinksURL.ToString()));

                        Assert.AreEqual("OK", othLinkStatus);

                    }

                    cntGlobal++;
                }

                // Verify Bottom Footer - Cookies, Privacy statement etc.

                cntGlobal = 0;

                bottomFooter = driver.FindElements(By.CssSelector(or.readingXMLFile("BottomBar", "bottomFooter", "TVWebPortalOR.xml")));

                // Verify Bottom Footer is present

                Assert.AreEqual(1, bottomFooter.Count());

                bottomFooterList = bottomFooter.ElementAt(0).FindElement(By.TagName("div")).FindElements(By.TagName("a"));

                foreach (String val in arrBottomFooter)
                {

                    log.Info("Verifying " + val + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

                    // Verify bottom footer value is present as expected

                    Assert.AreEqual(val, bottomFooterList.ElementAt(cntGlobal).Text);

                    bottomFooterURL = bottomFooterList.ElementAt(cntGlobal).GetAttribute("href").ToString();

                    log.Info("Bottom Footer URL := " + bottomFooterURL + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

                    bottomLinkStatus = uf.getStatusCode(new Uri(bottomFooterURL.ToString()));

                    Assert.AreEqual("OK", bottomLinkStatus);

                    cntGlobal++;
                }

                // Verify Copyright text

                copyRightText = driver.FindElement(By.CssSelector(or.readingXMLFile("BottomBar", "copyRight", "TVWebPortalOR.xml"))).FindElement(By.TagName("p")).Text.ToString();

                log.Info("Copyright text:=" + copyRightText + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

                Assert.AreEqual("© 2015 The Institution of Engineering and Technology is registered as a Charity in England & Wales (no 211014) and Scotland (no SC038698)", copyRightText);
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
            st.IE_TearDown(driver, log);
        }

    }

}
