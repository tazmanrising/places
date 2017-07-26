using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CalibrusIVRDailyReport
{
   public class DataStore
    {
        private List<WavName> _wavNames = new List<WavName>();

        public int CallTotal { get; set; }
        public int? CallSeconds { get; set; }

        public List<WavName> WavNames
        {
            get { return _wavNames; }
        }
        
    }
}
