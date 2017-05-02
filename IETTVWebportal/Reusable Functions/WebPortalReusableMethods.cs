using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using System.Threading;
using OpenQA.Selenium.Support.UI;
using Utility_Classes;

namespace IETTVWebportal.Reusable_Functions
{
    /// <summary>
    /// A test class for ...
    /// </summary>
    [TestFixture]
    public class WebPortalReusableMethods
    {
        IWebDriver mozilladriver = null;
        IWebElement element = null;
        IWait<IWebDriver> iWait;

        Utility_Functions uf = new Utility_Functions();

        public String darwinCRM_CreateCustomer(List<String> completeDataList)
        {

            string customerId = create_IETMember(completeDataList);   //Calling to create customer and retrieve customer ID
            Console.WriteLine("Customer ID ::: " + customerId);

            String membershipId = search(customerId);    //Calling to get Member Ship Id 


            for (int i = 1; i < 10; i++)
            {
                if (membershipId.Equals(""))
                {
                    Thread.Sleep(20000);
                    membershipId = search(customerId);
                }
                else
                    break;
            }

            Console.WriteLine("MemberShip ID :: " + membershipId);

            IWebDriver mozilladriver = getDriver();
            mozilladriver.Quit();

            return membershipId;

        }

        public String create_IETMember(List<String> completeDataList)
        {
            
             mozilladriver = new FirefoxDriver();
             iWait = new WebDriverWait(mozilladriver, TimeSpan.FromSeconds(60));
            // mozilladriver.Manage().Timeouts().ImplicitlyWait(new Timespan(0, 0, 30));
            mozilladriver.Manage().Window.Maximize();
            mozilladriver.Navigate().GoToUrl("http://sg1d-crm-st-ap.dev.mfh/psp/CRMTST1/EMPLOYEE/CRM/?cmd=logout");

            //loging into crm
            iWait.Until(ExpectedConditions.ElementIsVisible(By.Id("userid")));
            mozilladriver.FindElement(By.Id("userid")).SendKeys("ASALVI");
            mozilladriver.FindElement(By.Id("pwd")).SendKeys("Travel1@");
            mozilladriver.FindElement(By.ClassName("psloginbutton")).Click();

            mozilladriver.FindElement(By.Name("CR_CUSTOMER")).Click();

            element = mozilladriver.FindElement(By.Name("NAV"));  //Customers CRM
            mozilladriver.SwitchTo().Frame(element);
            mozilladriver.FindElement(By.LinkText("Customers CRM")).Click();
            mozilladriver.SwitchTo().DefaultContent();

            element = mozilladriver.FindElement(By.Name("TargetContent"));
            mozilladriver.SwitchTo().Frame(element);
            mozilladriver.FindElement(By.LinkText("Add Person")).Click();
            mozilladriver.SwitchTo().DefaultContent();


            element = mozilladriver.FindElement(By.Name("TargetContent"));
            mozilladriver.SwitchTo().Frame(element);
            mozilladriver.FindElement(By.Id("BO_NAME_FIRST_NAME$0")).SendKeys(completeDataList[0]);
            mozilladriver.FindElement(By.Id("BO_NAME_LAST_NAME$0")).SendKeys(completeDataList[1]);
            mozilladriver.FindElement(By.Id("RD_PERSON_BIRTHDATE")).SendKeys(completeDataList[2] + completeDataList[3] + completeDataList[4]);
            mozilladriver.FindElement(By.Id("ABE_ADD_WRK_POSTAL$0")).SendKeys("sg12ay");
            mozilladriver.FindElement(By.Id("UFL_FUNCLIB_U_ACE_LOOKUP$0")).Click();
            Thread.Sleep(2000);
            mozilladriver.FindElement(By.Id("#ICSave")).Click();
            Thread.Sleep(2000);

            mozilladriver.FindElement(By.Id("tab12")).Click();                                    //member App
            element = mozilladriver.FindElement(By.Id("IEE_MEMBER_APP_IEE_MEM_TYPE_APPL$0"));      //member type
            SelectElement MemberType = new SelectElement(element);
            MemberType.SelectByIndex(1);

            iWait.Until(ExpectedConditions.ElementIsVisible(By.Id("IEE_MEMBER_APP_EMPLOYMENT_STATUS$0")));

            element = mozilladriver.FindElement(By.Id("IEE_MEMBER_APP_EMPLOYMENT_STATUS$0"));      //emp status
            SelectElement EmpStatus = new SelectElement(element);
            EmpStatus.SelectByIndex(0);

            iWait.Until(ExpectedConditions.ElementIsVisible(By.Id("IEE_MEMBER_APP_IEE_MEM_SOURCE$0")));

            element = mozilladriver.FindElement(By.Id("IEE_MEMBER_APP_IEE_MEM_SOURCE$0"));      //Membership source
            SelectElement Memsource = new SelectElement(element);
            Memsource.SelectByIndex(4);
            mozilladriver.FindElement(By.Id("#ICSave")).Click();

            iWait.Until(ExpectedConditions.ElementIsVisible(By.Id("IEE_MEM_APP_WRK_PAYMENT_METHOD$0")));

            element = mozilladriver.FindElement(By.Id("IEE_MEM_APP_WRK_PAYMENT_METHOD$0"));      //payment mode
            SelectElement payment_status = new SelectElement(element);
            payment_status.SelectByIndex(3);
            mozilladriver.FindElement(By.Id("IEE_MEM_APP_WRK_IEE_MEM_APP_SUB_O$0")).Click();   //submit order 


            iWait.Until(ExpectedConditions.ElementIsVisible(By.Id("H#H#DI4")));
            String CustomerID = mozilladriver.FindElement(By.Id("H#H#DI4")).Text;
            Console.WriteLine("Cus_id:::" + CustomerID);


            return CustomerID;
        }

        public String create_IETMember(String surname, String day, String month, String year)
        {

            mozilladriver = new FirefoxDriver();
            iWait = new WebDriverWait(mozilladriver, TimeSpan.FromSeconds(60));
            // mozilladriver.Manage().Timeouts().ImplicitlyWait(new Timespan(0, 0, 30));
            mozilladriver.Manage().Window.Maximize();
            mozilladriver.Navigate().GoToUrl("http://sg1d-crm-st-ap.dev.mfh/psp/CRMTST1/EMPLOYEE/CRM/?cmd=logout");

            //loging into crm
            iWait.Until(ExpectedConditions.ElementIsVisible(By.Id("userid")));
            mozilladriver.FindElement(By.Id("userid")).SendKeys("ASALVI");
            mozilladriver.FindElement(By.Id("pwd")).SendKeys("Travel1@");
            mozilladriver.FindElement(By.ClassName("psloginbutton")).Click();

            mozilladriver.FindElement(By.Name("CR_CUSTOMER")).Click();

            element = mozilladriver.FindElement(By.Name("NAV"));  //Customers CRM
            mozilladriver.SwitchTo().Frame(element);
            mozilladriver.FindElement(By.LinkText("Customers CRM")).Click();
            mozilladriver.SwitchTo().DefaultContent();

            element = mozilladriver.FindElement(By.Name("TargetContent"));
            mozilladriver.SwitchTo().Frame(element);
            mozilladriver.FindElement(By.LinkText("Add Person")).Click();
            mozilladriver.SwitchTo().DefaultContent();


            element = mozilladriver.FindElement(By.Name("TargetContent"));
            mozilladriver.SwitchTo().Frame(element);
            mozilladriver.FindElement(By.Id("BO_NAME_FIRST_NAME$0")).SendKeys("Firstname");
            mozilladriver.FindElement(By.Id("BO_NAME_LAST_NAME$0")).SendKeys(surname);
            mozilladriver.FindElement(By.Id("RD_PERSON_BIRTHDATE")).SendKeys(day + month + year);
            mozilladriver.FindElement(By.Id("ABE_ADD_WRK_POSTAL$0")).SendKeys("sg12ay");
            mozilladriver.FindElement(By.Id("UFL_FUNCLIB_U_ACE_LOOKUP$0")).Click();
            Thread.Sleep(2000);
            mozilladriver.FindElement(By.Id("#ICSave")).Click();
            Thread.Sleep(2000);

            mozilladriver.FindElement(By.Id("tab12")).Click();                                    //member App
            element = mozilladriver.FindElement(By.Id("IEE_MEMBER_APP_IEE_MEM_TYPE_APPL$0"));      //member type
            SelectElement MemberType = new SelectElement(element);
            MemberType.SelectByIndex(1);

            iWait.Until(ExpectedConditions.ElementIsVisible(By.Id("IEE_MEMBER_APP_EMPLOYMENT_STATUS$0")));

            element = mozilladriver.FindElement(By.Id("IEE_MEMBER_APP_EMPLOYMENT_STATUS$0"));      //emp status
            SelectElement EmpStatus = new SelectElement(element);
            EmpStatus.SelectByIndex(0);

            iWait.Until(ExpectedConditions.ElementIsVisible(By.Id("IEE_MEMBER_APP_IEE_MEM_SOURCE$0")));

            element = mozilladriver.FindElement(By.Id("IEE_MEMBER_APP_IEE_MEM_SOURCE$0"));      //Membership source
            SelectElement Memsource = new SelectElement(element);
            Memsource.SelectByIndex(4);
            mozilladriver.FindElement(By.Id("#ICSave")).Click();

            iWait.Until(ExpectedConditions.ElementIsVisible(By.Id("IEE_MEM_APP_WRK_PAYMENT_METHOD$0")));

            element = mozilladriver.FindElement(By.Id("IEE_MEM_APP_WRK_PAYMENT_METHOD$0"));      //payment mode
            SelectElement payment_status = new SelectElement(element);
            payment_status.SelectByIndex(3);
            mozilladriver.FindElement(By.Id("IEE_MEM_APP_WRK_IEE_MEM_APP_SUB_O$0")).Click();   //submit order 


            iWait.Until(ExpectedConditions.ElementIsVisible(By.Id("H#H#DI4")));
            String CustomerID = mozilladriver.FindElement(By.Id("H#H#DI4")).Text;
            Console.WriteLine("Cus_id:::" + CustomerID);


            return CustomerID;
        }

        public String search(String CustomerID)
        {
            mozilladriver.SwitchTo().DefaultContent();
            element = mozilladriver.FindElement(By.Name("NAV"));                                //Search person
            mozilladriver.SwitchTo().Frame(element);
            mozilladriver.FindElement(By.LinkText("Search Person")).Click();



            // driver.SwitchTo().DefaultContent();   
            element = mozilladriver.FindElement(By.Name("TargetContent"));
            mozilladriver.SwitchTo().Frame(element);
            element = mozilladriver.FindElement(By.Name("RB_FLT_CRIT_WRK_RA_VALUE$1"));      //Customerid

            element.SendKeys(CustomerID);

            mozilladriver.FindElement(By.Id("PB_FILTER")).Click();

            String membershipId = mozilladriver.FindElement(By.Id("H#H#IEE_D1")).Text;
            Console.WriteLine("Mem id:::" + membershipId);

            return membershipId;

        }

        public IWebDriver getDriver()
        {
            return mozilladriver;
        }


    }
}
