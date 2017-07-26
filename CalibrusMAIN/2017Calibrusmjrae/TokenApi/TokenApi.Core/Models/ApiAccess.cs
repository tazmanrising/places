using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TokenApi.Core.Models
{
    public class ApiAccess
    {
        [Key]
        public int ApiAccessId { get; set; }
        public int TokenId { get; set; }
        public string ApiAddress { get; set; }
        public DateTime TimeStamp { get; set; }
       

    }

}
