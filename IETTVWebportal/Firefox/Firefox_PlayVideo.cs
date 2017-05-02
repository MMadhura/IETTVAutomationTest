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
using log4net;
using log4net.Config;
using System.Reflection;
using System.IO;
using System.Diagnostics;
using System.Text.RegularExpressions;
using NSoup.Nodes;  // for nsoup documents
using NSoup.Select;  //for nsoup element
//using IETTVAdminPortal.Firefox;
using IETTVWebportal.Reusable_Functions;

namespace IETTVWebportal.Firefox
{

    [TestFixture]
    public class Firefox_PlayVideo
    {
        // This is to configure logger mechanism for Utilities.Config
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        #region Variable Decration and object initialistaion

        internal IWebDriver driver = null;

        IWait<IWebDriver> iWait = null;

        IWebElement element;

        string appURL;

        Boolean flag;

        IJavaScriptExecutor executor = null;

        Utility_Functions uf = new Utility_Functions();                                  // Instantiate object for Utility Functons

        Configuration cf = new Configuration();                                          // Instantiate object for Configuration

        Object_Repository_Class or = new Object_Repository_Class();                      // Instantiate object for object repository

        Firefox_WebSetupTearDown st = new Firefox_WebSetupTearDown();                         // Instantiate object for Firefox Setup Teardown

        #endregion


        [TestFixtureSetUp]
        public void FixtureSetUp()
        {
            log4net.Config.XmlConfigurator.Configure(new FileInfo(AppDomain.CurrentDomain.BaseDirectory + "WebPortal.config"));         // To configure logger funtionality

            log.Info("Inside Fixture Setup of Firefox - Play Video Test" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

            driver = st.Firefox_PlayVideo_Fixture();

            uf.CreateOrReplaceVideoFolder();

            iWait = new WebDriverWait(driver, TimeSpan.FromSeconds(60));

            executor = (IJavaScriptExecutor)driver;
        }

        [SetUp]
        public void setup()
        {
            appURL = st.Firefox_Setup(driver, log, executor);                                   // Calling Firefox Setup
        }

        public void handleEmergencyPopUp()
        {
            //Handling pop up message
            iWait.Until(ExpectedConditions.ElementIsVisible(By.Id("div_emergency")));

            driver.FindElement(By.CssSelector("div#div_emergency > div > div.modal-content > div.modal-footer > div > button.btn.btn1.btn-success.ok_btn_size")).Click();

            iWait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.Id("div_emergency")));
        }

        [Test]
        public void TVWeb_001_VerifyVideoPlay()
        {

            try
            {

                log.Info("Verify Video Play Test Started" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

                uf.isJqueryActive(driver);

                iWait.Until(ExpectedConditions.ElementExists(By.Id("searchtextbox")));

                //search the required video

                IWebElement SearchTextField = driver.FindElement(By.Id("searchtextbox"));

                // Get the Video management node list  

                //String videoname = "video 1 7 may1";

               List<String> videoname = cf.readSysConfigFile("AdminPortal", "VideoManagement", "SysConfig.xml");

                Console.WriteLine("videoname    " + videoname.ElementAt(0).ToString());

                SearchTextField.SendKeys(videoname.ElementAt(0).ToString());

                iWait.Until(ExpectedConditions.ElementToBeClickable(By.Id("searchicon")));

                //Click on searchIcon

                IWebElement SearchIcon = driver.FindElement(By.Id("searchicon"));

                SearchIcon.Click();

                handleEmergencyPopUp();

                uf.isJqueryActive(driver);

                IWebElement searchresultDetails = driver.FindElement(By.CssSelector("div.video-description-clear-both > span > a"));

                IWebElement serachResultLink = driver.FindElement(By.CssSelector("div.margin-left-148.margin-right.video-description-clear-both > span > a"));

                String webvideoTitle = searchresultDetails.Text.Trim();

                log.Info("Web Video Title:" + webvideoTitle + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

                Assert.AreEqual(webvideoTitle, videoname.ElementAt(0).ToString());

                serachResultLink.Click();
                uf.isJqueryActive(driver);

                //  Thread.Sleep(2000);

                element = iWait.Until<IWebElement>((d) =>
                {
                    return d.FindElement(By.Id("playBtn"));
                });

                driver.FindElement(By.Id("playBtn")).Click();

                log.Info("Video play button clicked" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

                // Wait for 50 sec before collecting data

                Thread.Sleep(50000);

                driver.Navigate().GoToUrl("http://www.google.com");

                // Wait for 9 sec

                Thread.Sleep(9000);

                iWait.Until(driver1 => ((IJavaScriptExecutor)driver).ExecuteScript("return document.readyState").Equals("complete"));

                // Read generated HAR file and verify if the video play url exists

                string[] filePaths = Directory.GetFiles(@"C:\Project\IETTVWebPortal\Firebug\", "*.har");

                String fileContent;

                Boolean playfound = false;

                String HarPath = null;

                System.IO.StreamReader file = null;

                log.Info("File path count " + filePaths.Length + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

                if (filePaths.Length > 0)
                {

                    for (int i = 0; i < filePaths.Length; i++)
                    {
                        HarPath = filePaths[i].ToString();

                        log.Info("Path of HAR file is " + HarPath + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

                        file = new System.IO.StreamReader(HarPath);

                        fileContent = file.ReadToEnd();

                        MatchCollection matches = Regex.Matches(fileContent, "/SessionUpdate?");

                        int sessionCount = matches.Count;

                        Console.WriteLine("Total Session Count:" + sessionCount);

                        log.Info("Total Session Count:" + sessionCount);

                        if (sessionCount > 2)
                        {
                            log.Info("Video is playing" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

                            playfound = true;
                        }

                        file.Dispose();

                        file.Close();

                        // Delete generated file

                        File.Delete(HarPath);

                        log.Info(HarPath + " file is deleted" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

                    }

                    Assert.AreEqual(true, playfound);
                }
                else
                {
                    log.Info("HAR file could not be generated" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message + "\n" + e.StackTrace + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());
                log.Error(e.Message + "\n" + e.StackTrace + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

                Assert.AreEqual(true, false);
            }

        }

       
        [TestFixtureTearDown]
        public void TearDown()
        {
            st.Firefox_TearDown(driver, log);                                                   // Calling Firefox Teardown
        }

    }


}
