using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalibrusAWSBulkWavFileMove
{
    public class WavFileRecord
    {
        public int MainId { get; set; }
        public DateTime? CallDateTime { get; set; }
        public string WavName { get; set; }
        public string OutboundWavName { get; set; }
        public string Client { get; set; }
    }
}
