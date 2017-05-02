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
using System.Diagnostics;
using Utilities.Config;
using Sikuli4Net.sikuli_JSON;
using Sikuli4Net.sikuli_REST;
using Sikuli4Net.sikuli_UTIL;


namespace IETTVAdminportal.Reusable
{
    
    class AdminAuth
    {
        
        Process pro;

        Boolean adminLogin;

        string authPath, username, password;

        Configuration cf = new Configuration();                                                                 // Instantiate object for Configuration        

        Sikuli4Net.sikuli_REST.Screen s;

        String authImagePath = Environment.CurrentDirectory + "\\Upload\\Sikuli\\";      
                 
        public Boolean authLogin(String Browser)
        {

            adminLogin = false;

            try
            {
                authPath = AppDomain.CurrentDomain.BaseDirectory + @"AuthHandle.exe";

                username =  cf.readingXMLFile("AdminPortal","Login", "userName", "Config.xml");                  // Get configured username

                password = cf.readingXMLFile("AdminPortal", "Login", "passWord", "Config.xml");                  // Get configured password

                pro = new Process();

                ProcessStartInfo startInfo = new ProcessStartInfo(authPath);

                startInfo.Arguments = "AuthHandle.au3" + " " + username + " " + password + " " + Browser;

                pro.StartInfo = startInfo;

                pro.Start();

                adminLogin = true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);

                adminLogin = false;
            }

            return adminLogin;
        }


        public Boolean sikuliAuthLogin(String Browser)
        {

            Pattern ImgChromeUsername = new Pattern(authImagePath + "\\ChromeAuthUsername.png");
            Pattern ImgChromePassword = new Pattern(authImagePath+"\\ChromeAuthPassword.png");
            Pattern ImgChromeLogIn = new Pattern(authImagePath+"\\ChromeAuthLogIn.png");

            adminLogin = false;

            s = new Sikuli4Net.sikuli_REST.Screen();

            Thread.Sleep(5000);

            try
            {
                username = cf.readingXMLFile("AdminPortal", "Login", "userName", "Config.xml");

                password = cf.readingXMLFile("AdminPortal", "Login", "passWord", "Config.xml");

                if (Browser.Equals("Chrome"))
                {
                    s.Wait(ImgChromeUsername, 60);

                    s.Type(ImgChromeUsername, username);

                    s.Type(ImgChromePassword, password);

                    s.Click(ImgChromeLogIn, true);
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);

                adminLogin = false;                
            }

            return adminLogin;
        }
                  
    }

}
