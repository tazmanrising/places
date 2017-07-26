using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TokenApi.Core.Models
{
    public class ApiLog
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ApiLogId { get; set; }
        public string Token { get; set; }
        [NotMapped]
        public DateTime TimeStamp { get; set; }
        public string IpAddress { get; set; }
        public string RequestUrl { get; set; }
        public string Header { get; set; }
        public bool AccessGranted { get; set; }
    }

}
