using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SparkProviderBatchXLSReport
{
    public class BrandObject
    {
        public List<BrandObjectID> IDList { get; set; }
        public string Name { get; set; }
    }

    public class BrandObjectID
    {
        public int id { get; set; }

    }
}
