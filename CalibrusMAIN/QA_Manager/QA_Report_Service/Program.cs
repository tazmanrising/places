using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QA_Report_Service
{
    class Program
    {
        static void Main(string[] args)
        {
            var qa = new QA_Builder();
            qa.GetAllCalls();

        }
    }
}
