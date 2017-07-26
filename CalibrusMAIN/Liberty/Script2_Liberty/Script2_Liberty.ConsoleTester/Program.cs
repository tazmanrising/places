using Script2_Liberty.Data.Models;
using Script2_Liberty.Data.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Script2_Liberty.ConsoleTester
{
    class Program
    {
        static void Main(string[] args)
        {
            var market = new Market();
            var contractterm = new List<ContractTerm>();
            contractterm = market.TestMarketUtilities(97);
            Console.ReadLine();
        }
    }
}
