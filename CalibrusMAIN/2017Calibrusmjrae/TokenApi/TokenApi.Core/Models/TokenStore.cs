using System;
using System.ComponentModel.DataAnnotations;

namespace TokenApi.Core.Models
{
    public class TokenStore
    {
        [Key]
        public int TokenId { get; set; }
        public string ClientName { get; set; }
        public string IssuedTo { get; set; }
        public string Domain { get; set; }
        public string Token { get; set; }
        public DateTime? IssueDate { get; set; }
        public DateTime? TimeStamp { get; set; }


    }

}
