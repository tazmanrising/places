//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Calibrus.SparkPortal.DataAccess.Infrastructure
{
    using System;
    using System.Collections.Generic;
    
    public partial class UserLog
    {
        public int UserLogId { get; set; }
        public int UserId { get; set; }
        public bool IsActive { get; set; }
        public string Note { get; set; }
        public System.DateTime CreatedDateTime { get; set; }
        public string CreatedBy { get; set; }
    
        public virtual User User { get; set; }
    }
}
