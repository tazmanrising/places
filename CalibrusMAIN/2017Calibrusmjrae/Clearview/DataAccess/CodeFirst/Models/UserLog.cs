using System;
using System.ComponentModel.DataAnnotations;


namespace Calibrus.ClearviewPortal.DataAccess.CodeFirst.Models
{
    public class UserLog
    {
        [Key]
        public int UserLogId { get; set; }
        public int UserId { get; set; }
        public bool IsActive { get; set; }
        public string Note { get; set; }
        public DateTime CreatedDateTime { get; set; }
        public string CreatedBy { get; set; }

    }
}
