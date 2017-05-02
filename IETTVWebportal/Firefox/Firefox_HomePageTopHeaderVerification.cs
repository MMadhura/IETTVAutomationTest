using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.IE;
using OpenQA.Selenium.Support.UI;
using System.Windows.Forms;
using System.Threading;
using System.Net;
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


namespace IETTVWebportal.Firefox
{
    
    [TestFixture]
    public class Firefox_HomePageTopHeaderVerification
    {

        // This is to configure logger mechanism for Utilities.Config
       private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
 
       internal IWebDriver driver = null;

       IWait<IWebDriver> iWait;

       IJavaScriptExecutor executor;

       string appURL;

       Utility_Functions uf = new Utility_Functions();                                         // Instantiate object for Utility Functons

       Configuration cf = new Configuration();                                                 // Instantiate object for Configuration

       Object_Repository_Class OR = new Object_Repository_Class();

       Firefox_WebSetupTearDown st = new Firefox_WebSetupTearDown();                           // Instantiate object for Firefox Setup Teardown

       #region Setup 

       [TestFixtureSetUp]
       public void FixtureSetUp()
       {
           log4net.Config.XmlConfigurator.Configure(new FileInfo(AppDomain.CurrentDomain.BaseDirectory + "WebPortal.config"));

           log.Info("Inside Fixture Setup of Firefox - BottomBar Verification Test" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

           driver = new FirefoxDriver();                                                       // Initialize Firefox driver  

           uf.CreateOrReplaceVideoFolder();

           iWait = new WebDriverWait(driver, TimeSpan.FromSeconds(60));

           executor = (IJavaScriptExecutor)driver;       
       }

       [SetUp]
       public void SetUp()
       {
           appURL = st.Firefox_Setup(driver, log, executor);                                   // Calling Firefox Setup
       }



       #endregion


        // This function return value of href attribute.
        public String getHrefValue(String cssValue)
        {
            return driver.FindElement(By.CssSelector(cssValue)).GetAttribute("href");
        }

        [Test]
        public void TVWeb_001_VerifyTopHeaderLinkTest()
        {
            String hrefValue = null;
            String Status = null;
            String expectedStatusCode = "OK";
            int size=0, cntSize=0;;

            try
            {
                log.Info("TVWeb_001_VerifyTopHeaderLinkTest test started" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

                #region Other IET Website Link Testing

                hrefValue = getHrefValue("ul.OtheeIETWebList > li#Li2 > a");
                Status = getStatusCode(new Uri(hrefValue));
                Assert.AreEqual(expectedStatusCode, Status);  // checking status of E&T jobs

                // Verifying that E&T Jobs present or not
                Assert.AreNotEqual(driver.FindElements(By.CssSelector(OR.readingXMLFile("HomePageTopHeader", "ETJobsIETWebList", "TVWebPortalOR.xml"))).Count, 0);

                hrefValue = getHrefValue("ul.OtheeIETWebList > li#Li1 > a");
                Status = getStatusCode(new Uri(hrefValue));
                Assert.AreEqual(expectedStatusCode, Status);    // checking status of IET Web List

                // Verifying that IET Web List present or not
                Assert.AreNotEqual(driver.FindElements(By.CssSelector(OR.readingXMLFile("HomePageTopHeader", "TheIETWebList", "TVWebPortalOR.xml"))).Count, 0);


                hrefValue = getHrefValue("ul.OtheeIETWebList > li#Li3 > a");
                Status = getStatusCode(new Uri(hrefValue));
                Assert.AreEqual(expectedStatusCode, Status);    // checking status of IET Connect Web List

                // Verifying that IET Connect Web List present or not
                Assert.AreNotEqual(driver.FindElements(By.CssSelector(OR.readingXMLFile("HomePageTopHeader", "IETConnectIETWebList", "TVWebPortalOR.xml"))).Count, 0);


                hrefValue = getHrefValue("ul.OtheeIETWebList > li#Li4 > a");
                Status = getStatusCode(new Uri(hrefValue));
                Assert.AreEqual(expectedStatusCode, Status);    // checking status of Digital Library

                // Verifying that Digital Library present or not
                Assert.AreNotEqual(driver.FindElements(By.CssSelector(OR.readingXMLFile("HomePageTopHeader", "DigitalLibraryIETWebList", "TVWebPortalOR.xml"))).Count, 0);


                hrefValue = getHrefValue("ul.OtheeIETWebList > li#Li5 > a");
                Status = getStatusCode(new Uri(hrefValue));
                Assert.AreEqual(expectedStatusCode, Status);    // checking status of Electrical IET

                // Verifying that Electrical IET present or not
                Assert.AreNotEqual(driver.FindElements(By.CssSelector(OR.readingXMLFile("HomePageTopHeader", "ElectricalIETWebList", "TVWebPortalOR.xml"))).Count, 0);


                hrefValue = getHrefValue("ul.OtheeIETWebList > li#Li6 > a");
                Status = getStatusCode(new Uri(hrefValue));
                Assert.AreEqual(expectedStatusCode, Status);    // checking status of Faraday IEt Web List

                // Verifying that Faraday IEt present or not
                Assert.AreNotEqual(driver.FindElements(By.CssSelector(OR.readingXMLFile("HomePageTopHeader", "FaradayIETWebList", "TVWebPortalOR.xml"))).Count, 0);


                hrefValue = getHrefValue("ul.OtheeIETWebList > li#Li7 > a");
                Status = getStatusCode(new Uri(hrefValue));
                Assert.AreEqual(expectedStatusCode, Status);    // checking status of Venues

                // Verifying that Venues present or not
                Assert.AreNotEqual(driver.FindElements(By.CssSelector(OR.readingXMLFile("HomePageTopHeader", "VenuesIETWebList", "TVWebPortalOR.xml"))).Count, 0);


                hrefValue = getHrefValue("ul.OtheeIETWebList > li#Reports > a");
                Status = getStatusCode(new Uri(hrefValue));
                Assert.AreEqual(expectedStatusCode, Status);    // checking status of Magazines

                // Verifying that Magazines present or not
                Assert.AreNotEqual(driver.FindElements(By.CssSelector(OR.readingXMLFile("HomePageTopHeader", "MagazineIETWebList", "TVWebPortalOR.xml"))).Count, 0);


                #endregion

                #region Login Link Testing

                IWebElement loginLink = driver.FindElement(By.LinkText(OR.readingXMLFile("HomePageTopHeader", "LoginLink", "TVWebPortalOR.xml")));
                Assert.AreEqual(true, loginLink.Displayed);  // Verfying that login link is present

                IJavaScriptExecutor executor = (IJavaScriptExecutor)driver;
                executor.ExecuteScript("arguments[0].click();", loginLink);

                iWait.Until(ExpectedConditions.ElementIsVisible((By.LinkText(OR.readingXMLFile("HomePageTopHeader", "ContactLink", "TVWebPortalOR.xml")))));

                #endregion

                #region Contact Link

                hrefValue = driver.FindElement(By.LinkText(OR.readingXMLFile("HomePageTopHeader", "ContactLink", "TVWebPortalOR.xml"))).GetAttribute("href");
                Status = getStatusCode(new Uri(hrefValue));  // getting status of Contact link
                Assert.AreEqual(expectedStatusCode, Status);    // verfying that statsu of Contact link 

                // verfying that  Contact link is displayed
                Assert.AreEqual(driver.FindElement(By.LinkText(OR.readingXMLFile("HomePageTopHeader", "ContactLink", "TVWebPortalOR.xml"))).Displayed, true);

                #endregion

                #region checking Logo link and clicking on it

                //checking Status of Logo link
                hrefValue = getHrefValue(OR.readingXMLFile("HomePageTopHeader", "Logolink", "TVWebPortalOR.xml"));
                Status = getStatusCode(new Uri(hrefValue));
                Assert.AreEqual(expectedStatusCode, Status);
                Assert.AreEqual(driver.FindElement(By.CssSelector(OR.readingXMLFile("HomePageTopHeader", "LogoImage", "TVWebPortalOR.xml"))).Displayed, true);

                //Clicking on logo Image
                IWebElement logoImage = driver.FindElement(By.CssSelector(OR.readingXMLFile("HomePageTopHeader", "Logolink", "TVWebPortalOR.xml")));
                executor.ExecuteScript("arguments[0].click();", logoImage);

              //  Thread.Sleep(2000);

                uf.isJavaScriptActive(driver);

                #endregion

                #region Testing Advertise Banner Images

                driver.SwitchTo().Frame(0);               

                while (cntSize < 30)
                {
                    size = driver.FindElements(By.CssSelector("body > a > img")).Count;

                    Console.WriteLine("Size:" + size);

                    if (size == 1)
                    {
                        Assert.AreEqual(1, size);

                        break;
                    }

                    cntSize++;

                    Thread.Sleep(1000);

                    driver.Navigate().Refresh();

                    driver.SwitchTo().DefaultContent();

                    driver.SwitchTo().Frame(0);                    
                }

                if (size == 0)
                {
                    Assert.AreEqual(true, false);
                }

                #endregion

                log.Info("TVWeb_001_VerifyTopHeaderLinkTest test completed" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

            }
            catch (Exception e)
            {
                log.Error(e.Message + "\n" + e.StackTrace + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());
                Assert.AreEqual(true, false);
            }

        }

        // This function returns status code of URL
        public String getStatusCode(Uri homePageLink)
        {
            try
            {
                HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(homePageLink); // Creating connection with URL
                webRequest.Timeout = 10000;     // Thisi is timeout
                webRequest.AllowAutoRedirect = false;
                HttpWebResponse wResp = (HttpWebResponse)webRequest.GetResponse();  // Getting response from server
                Thread.Sleep(2000);
                string wRespStatusCode = wResp.StatusCode.ToString();   // getting status code from the response
                return wRespStatusCode.Trim();
            }
            catch (WebException we)
            {
                string wRespStatusCode = ((HttpWebResponse)we.Response).StatusCode.ToString(); // getting the status code from response
                Thread.Sleep(2000);
                we.Response.Close();  // closing the connection
                return wRespStatusCode.Trim();
            }
        }

       [TestFixtureTearDown]
       public void TearDown()
       {
           st.Firefox_TearDown(driver, log);                                                   // Calling Firefox Teardown
       }

    }
}
