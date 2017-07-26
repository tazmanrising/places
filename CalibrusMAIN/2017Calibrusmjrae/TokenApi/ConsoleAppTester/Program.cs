using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleAppTester
{
    class Program
    {
        static void Main(string[] args)
        {

            //todo  fix nlog to work  , not writing to file,  but need to write to db table
            new LogTester().ManageErrors();





            // Create a new Token

            var tokenCreator = new TokenCreator();
            var token = tokenCreator.RandomString(15);

            Console.WriteLine(token);

            


            Console.ReadLine();





        }

    }

}
