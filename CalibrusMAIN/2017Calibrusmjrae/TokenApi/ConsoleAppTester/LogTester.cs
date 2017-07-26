using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Logging;

namespace ConsoleAppTester
{
    public class LogTester
    {

        public void ManageErrors()
        {
            try
            {
                Logger.Error("test", "adfadfaf");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

             

        }

     
    }

}
