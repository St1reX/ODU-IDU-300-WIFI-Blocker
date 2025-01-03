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
using OpenQA.Selenium.BiDi.Modules.Log;
using System.Net.Http.Headers;

namespace WiFi_Blocker
{
    internal class Blocker
    {
        ChromeDriver ChromeInstance { get; set; }
        int Interval { get; set; }
        string Login { get; set; }
        string Password { get; set; }
        string WhiteListPath { get; set; }
        List<string> WhiteList  = new List<string>();


        public Blocker(string login, string password, int interval)
        {
            Login = login;
            Password = password;
            Interval = interval;

            WhiteListManagment();
            //BlockDevices(WhiteList);
        }


        //WHITE LIST MANAGMENT
        private void WhiteListManagment()
        { 
            int selectedOption = 0;
            List<string> communicates = new List<string> { };
            List<string> options = new List<string> {"Add new MAC to WhiteList", "Delete MAC from WhiteList", "Display WhiteList"};

            try
            {
                FetchWhiteList();

                if (WhiteList.Count == 0)
                {
                    Logger.InfoMessage("White list is currently empty. \nDo you want to continue. It will cause blocking every device connected to WI-FI.");
                }

                DisplayWhiteList();

                Console.Clear();

                while (selectedOption != -1)
                {
                    selectedOption = SelectFromGivenOptions(options, communicates);

                    switch (selectedOption)
                    {
                        case 0:
                            AddDeviceToWhiteList();
                            break;
                        case 1:
                            DeleteDeviceFromWhiteList();
                            break;
                        case 2:
                            DisplayWhiteList();
                            break;
                        default:
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.ErrorMessage("Unknown error ocurred while managin WhiteList: " + ex.Message);
                Environment.Exit(0);
            }
        }
        private void FetchWhiteList()
        {
            WhiteListPath = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), "..", "..", "..", "WhiteList"));

            try
            {
                if (!Directory.Exists(WhiteListPath))
                {
                    Directory.CreateDirectory(WhiteListPath);
                    WhiteListPath = Path.Combine(WhiteListPath, "devices.txt");

                    using (File.Create(WhiteListPath))
                    {
                        
                    }

                    Logger.SuccessMessage("Directory WhiteList created");
                }
                else
                {
                    WhiteListPath = Path.Combine(WhiteListPath, "devices.txt");
                }

                StreamReader reader;

                using (reader = new StreamReader(WhiteListPath, new System.Text.UTF8Encoding(true)))
                {
                    string deviceMAC = "";
                    char character = ' ';

                    while (reader.Peek() != -1)
                    {
                        character = (char)reader.Read();

                        if(character == ';')
                        {
                            WhiteList.Add(deviceMAC);
                            deviceMAC = "";
                            continue;
                        }

                        deviceMAC += character;
                    }
                }

            }
            catch (Exception ex)
            {
                Logger.ErrorMessage(ex.Message);
                Environment.Exit(0);
            }
        }
        private void AddDeviceToWhiteList()
        {
            Console.Clear();

            string MAC = "";
            Console.WriteLine("Provide device MAC Adress: ");
            MAC = Console.ReadLine();

            try
            {
                if (WhiteList.Contains(MAC))
                {
                    Logger.InfoMessage("Provided MAC adress is already added to the WhiteList. Press any key to continue...");
                    Console.ReadKey();
                }
                else
                {
                    WhiteList.Add(MAC);

                    StreamWriter writer;

                    using (writer = File.AppendText(WhiteListPath))
                    {
                        foreach (var letter in MAC)
                        {
                            writer.Write(letter);
                        }
                        writer.Write(";");
                    }

                    Logger.SuccessMessage("Added device successfully. Press any key to continue...");
                    Console.ReadKey();
                }
            }
            catch (Exception ex)
            {
                Logger.ErrorMessage(ex.Message);
                Environment.Exit(0);
            }
        }
        private void DeleteDeviceFromWhiteList()
        {
            Console.Clear();

            int selectedOption = 0;
            List<string> communicates = new List<string> {"Select device you want to delete."};
            List<string> options = WhiteList;

            try
            {

                while (selectedOption != -1)
                {
                   selectedOption = SelectFromGivenOptions(options, communicates);

                    if (selectedOption != -1)
                    {
                        options.RemoveAt(selectedOption);

                        StreamWriter writer;

                        using (writer = File.CreateText(WhiteListPath))
                        {
                            foreach (var device in options)
                            {
                                foreach (var letter in device)
                                {
                                    writer.Write(letter);
                                }
                                writer.Write(";");
                            }
                        }

                        Logger.SuccessMessage("Deleted device successfully. Press any key to continue...");
                        Console.ReadKey();
                    }
                    else
                    {
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.ErrorMessage(ex.Message);
                Environment.Exit(0);
            }
        }
        private void DisplayWhiteList()
        {
            Console.WriteLine("\n******************************");

            Console.WriteLine("WHITE LIST");

            foreach (string MAC in WhiteList)
            {
                Console.WriteLine($"MAC: {MAC}");
            }

            Console.WriteLine("******************************\n");

            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }



        //BROWSER MANAGMENT
        private void BlockDevices(List <string> whiteList)
        {
            try
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

                ChromeInstance.SwitchTo().DefaultContent();



                IFrame = ChromeInstance.FindElement(By.Id("webbody"));
                ChromeInstance.SwitchTo().Frame(IFrame);

                IFrame = ChromeInstance.FindElement(By.Id("bodysetting_content"));
                ChromeInstance.SwitchTo().Frame(IFrame);
                Logger.InfoMessage("Context switched to Device List IFrame");


                DOMElement = ChromeInstance.FindElement(By.Id("div_info_normal"));
                IEnumerable<IWebElement> devicesList = DOMElement.FindElements(By.TagName("tr"));

                foreach (var device in devicesList)
                {
                    string MAC = device.FindElements(By.TagName("td"))[3].Text;
                    IWebElement checkBox = device.FindElements(By.TagName("td"))[7];

                    if (whiteList.Contains(MAC))
                    {
                        continue;
                    }
                    checkBox.Click();
                }

                DOMElement = ChromeInstance.FindElements(By.TagName("table"))[1];
                DOMElement = DOMElement.FindElements(By.TagName("td"))[2];
                DOMElement.Click();
            }
            catch (Exception ex)
            {
                Logger.ErrorMessage("Unknown error ocurred while blocking the devices on router admin panel: " + ex.Message);
                Environment.Exit(0);
            }
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
                Logger.ErrorMessage("Error occured while loggin the user to the admin panel: " + ex.Message);
                Environment.Exit(0);
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
                Logger.ErrorMessage("Failed to initialize ChromeDriver: " + ex.Message);
                Environment.Exit(0);
            }
        }


        //HELPER METHODS
        private void DisplayCommunicates(List<string> communicates)
        {
            foreach (string c in communicates)
            {
                Logger.InfoMessage(c);
            }
        }
        private int SelectFromGivenOptions(List<string> options, List<string> comunicates)
        {
            int selectedOption = 0;
            ConsoleKey actionKey;

            var tmpCommunicates = comunicates.ToList();
            tmpCommunicates.Add("Use {↑ and ↓} to change selected option, ENTER to choose.");
            tmpCommunicates.Add("Press ESC to exit.");
            tmpCommunicates.Add("\n");

            while (true)
            {
                Console.Clear();

                foreach (var communicate in tmpCommunicates)
                {
                    Console.WriteLine(communicate);
                }


                for (int i = 0; i < options.Count; i++)
                {
                    if (i == selectedOption)
                    {
                        Console.ForegroundColor = ConsoleColor.Green;

                        Console.WriteLine(" > " + options[i]);

                        Console.ResetColor();
                        continue;
                    }

                    Console.WriteLine(options[i]);
                }

                actionKey = Console.ReadKey().Key;

                switch (actionKey)
                {
                    case ConsoleKey.DownArrow:
                        selectedOption = selectedOption + 1 >= options.Count ? 0 : selectedOption + 1;
                        continue;
                    case ConsoleKey.UpArrow:
                        selectedOption = selectedOption - 1 < 0 ? options.Count - 1 : selectedOption - 1;
                        continue;
                    case ConsoleKey.Enter:
                        tmpCommunicates.Clear();
                        return selectedOption;
                    case ConsoleKey.Escape:
                        tmpCommunicates.Clear();
                        return -1;
                    default:
                        continue;
                }
            }
        }
        void Wait(int seconds)
        {
            System.Threading.Thread.Sleep(seconds);
        }
    }
}
