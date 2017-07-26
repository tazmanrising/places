using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace consoleappTester
{
    class Program
    {
        static void Main(string[] args)
        {


            Console.WriteLine("----------- START ------------");

            //foreach (string b in args)
            //    Console.WriteLine(b + "   ");



            for (int i = 0; i < args.Length; i++)
            {
                string flag = args.GetValue(i).ToString();
                if (flag == "bla")
                {
                    //Bla();
                }
                Console.WriteLine(flag);

            }



            //Console.WriteLine(args);


            if (args == null)
            {
                Console.WriteLine("args is null"); // Check for null array
            }
            else
            {
                args = new string[2];
                args[0] = "welcome in";
                args[1] = "www.overflow.com";
                Console.Write("args length is ");
                Console.WriteLine(args.Length); // Write array length
                for (int i = 0; i < args.Length; i++) // Loop through array
                {
                    string argument = args[i];
                    Console.Write("args index ");
                    Console.Write(i); // Write index
                    Console.Write(" is [");
                    Console.Write(argument); // Write string
                    Console.WriteLine("]");
                }
            }


           // Console.ReadLine();

        }
    }
}
