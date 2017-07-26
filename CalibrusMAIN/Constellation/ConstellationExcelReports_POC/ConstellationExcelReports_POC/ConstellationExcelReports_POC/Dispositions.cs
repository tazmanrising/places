using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConstellationExcelReports_POC
{
    class Dispositions
    {
        
        private int _count;
        private string _disposition;

        public Dispositions(int count, string disposition)
        {
            _count = count;
            _disposition = disposition;
        }

        public int Count
        {
            get { return _count; }
        }
        public string Disposition
        {
            get { return _disposition; }
        }
    }
}
