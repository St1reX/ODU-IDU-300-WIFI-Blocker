using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiFi_Blocker
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Logger.InfoMessage("ODU IDU 300 from PLUS (Polkomtel sp. z o.o.) WI-FI devices blocker.");

            Blocker blocker1 = new Blocker("admin", "Kuba2007", 2000);
        }
    }
}
