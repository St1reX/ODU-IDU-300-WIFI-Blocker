using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;
using System.Collections;
using System.Net;
using Csv;

namespace WiFi_Blocker
{
    internal class Blocker
    {
        ChromeDriver ChromeInstance { get; set; }
        int Interval { get; set; }
        string Login { get; set; }
        string Password { get; set; }
        List<string> WhiteList  = new List<string>();


        public Blocker(string login, string password, int interval)
        {
            Login = login;
            Password = password;
            Interval = interval;

            WhiteListManagment();
            //BlockDevices(WhiteList);
        }

        private void WhiteListManagment()
        {
            char userAction = ' ';

            FetchWhiteList();

            if (WhiteList.Count == 0)
            {
                Logger.InfoMessage("White list is currently empty. \nDo you want to continue. It will cause blocking every device connected to WI-FI");
            }
            else
            {
                foreach (string MAC in WhiteList)
                {
                    Console.WriteLine($"MAC: {MAC}");
                }

                Console.WriteLine("Do you want to add new device (ENTER) or block deviced instantly (ESCAPE)");
            }
        }

        private void FetchWhiteList()
        {
            string whiteListPath = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), "..", "..", "..", "WhiteList"));


            try
            {
                if (!Directory.Exists(whiteListPath))
                {
                    Directory.CreateDirectory(whiteListPath);
                    whiteListPath = Path.Combine(whiteListPath, "devices.csv");

                    using (File.Create(whiteListPath))
                    {
                        
                    }

                    Logger.SuccessMessage("Directory WhiteList created");
                }
                else
                {
                    var csvFile = File.ReadAllText(whiteListPath);
                    foreach (var line in CsvReader.ReadFromText(csvFile))
                    {
                        WhiteList.Add(line[0]);
                        Console.WriteLine(line[0]);
                    }             
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private void AddToWhiteList(string deviceMac)
        {

        }

        private void BlockDevices(List <string> whiteList)
        {
            LoginUser();

            IWebElement DOMElement;
            IWebElement IFrame;

            IFrame = ChromeInstance.FindElement(By.Id("header"));
            ChromeInstance.SwitchTo().Frame(IFrame);
            Logger.InfoMessage("Context switched to header IFrame");

            DOMElement = ChromeInstance.FindElement(By.Id("menu3"));
            DOMElement.Click();

            ChromeInstance.SwitchTo().DefaultContent();



            IFrame = ChromeInstance.FindElement(By.Id("webbody"));
            ChromeInstance.SwitchTo().Frame(IFrame);
            Logger.InfoMessage("Context switched to body IFrame");

            IFrame = ChromeInstance.FindElement(By.Id("bodysetting_menu"));
            ChromeInstance.SwitchTo().Frame(IFrame);
            Logger.InfoMessage("Context switched to SideNav IFrame");

            DOMElement = ChromeInstance.FindElement(By.Id("menu12"));
            DOMElement.Click();

            DOMElement = ChromeInstance.FindElement(By.Id("menu13"));
            DOMElement.Click();


        }

        private void LoginUser()
        {
            try
            {
                CreateChromeInstance("--start-maximized", "--disable-search-engine-choice-screen", "--mute-audio");
                ChromeInstance.Navigate().GoToUrl("http://192.168.0.1");

                Wait(Interval);

                IWebElement DOMElement;

                DOMElement = ChromeInstance.FindElement(By.Name("loginName"));
                DOMElement.SendKeys(Login);

                DOMElement = ChromeInstance.FindElement(By.Name("loginPassword"));
                DOMElement.SendKeys(Password);

                DOMElement = ChromeInstance.FindElement(By.Id("td_buttonlogin"));
                DOMElement.Click();

                Wait(Interval);

                if (ChromeInstance.Url != "http://192.168.0.1/idu/home.shtml")
                {
                    throw new Exception("Error occurred during login process");
                }

                Logger.SuccessMessage($"User '{Login}' logged");


            }
            catch (Exception ex)
            {
                Logger.ErrorMessage(ex.Message);
            }

        }

        private void CreateChromeInstance(params string[] chromeStartOption)
        {
            try
            {
                ChromeOptions options = new ChromeOptions();
                options.AddArguments(chromeStartOption);

                ChromeInstance = new ChromeDriver(options);
                ChromeInstance.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(1);

                Logger.SuccessMessage("Created chrome instance.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Failed to initialize ChromeDriver: " + ex.Message);
                throw;
            }
        }

        void Wait(int seconds)
        {
            System.Threading.Thread.Sleep(seconds);
        }
    }
}
