using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConstellationExcelReports_POC
{
    class Vendors
    {
        private int _vendorId;
        private string _vendorName;

        public Vendors(int id, string name)
        {
            _vendorId = id;
            _vendorName = name;
        }

        public int VendorId
        {
            get { return _vendorId; }
        }
        public string VendorName
        {
            get { return _vendorName; }
        }
    }
}
