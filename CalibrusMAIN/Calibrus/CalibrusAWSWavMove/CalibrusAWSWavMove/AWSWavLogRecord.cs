using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalibrusAWSWavMove
{
    public class AWSWavLogRecord
    {
        public int? Id { get; set; }
        public string Client { get; set; }
        public int MainId { get; set; }
        public string AwsUrlIn { get; set; }
        public string AwsUrlOut { get; set; }
    }
}
