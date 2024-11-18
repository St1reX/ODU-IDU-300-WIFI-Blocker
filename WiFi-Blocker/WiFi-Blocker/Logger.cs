using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiFi_Blocker
{
    internal class Logger
    {
        public static void ErrorMessage(string message)
        {
            string time = DateTime.Now.ToLongTimeString();

            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"ERROR: {time} || {message}");
            Console.ForegroundColor = ConsoleColor.White;
        }

        public static void InfoMessage(string message)
        {
            string time = DateTime.Now.ToLongTimeString();

            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine($"INFO: {time} || {message}");
            Console.ForegroundColor = ConsoleColor.White;
        }

        public static void SuccessMessage(string message)
        {
            string time = DateTime.Now.ToLongTimeString();

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"SUCCESS: {time} || {message}");
            Console.ForegroundColor = ConsoleColor.White;
        }


    }
}
