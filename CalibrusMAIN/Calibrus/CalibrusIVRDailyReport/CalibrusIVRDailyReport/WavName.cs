using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CalibrusIVRDailyReport
{
    public class WavName
    {
        private string _wavName;

        public WavName(string wavname)
        {
            _wavName = wavname;
        }
        public string WaveName
        {
            get { return _wavName; }
        }
    }
}
