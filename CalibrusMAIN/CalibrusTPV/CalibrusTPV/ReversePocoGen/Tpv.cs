// <auto-generated>
// ReSharper disable ConvertPropertyToExpressionBody
// ReSharper disable DoNotCallOverridableMethodsInConstructor
// ReSharper disable InconsistentNaming
// ReSharper disable PartialMethodWithSinglePart
// ReSharper disable PartialTypeWithSinglePart
// ReSharper disable RedundantNameQualifier
// ReSharper disable RedundantOverridenMember
// ReSharper disable UseNameofExpression
// TargetFrameworkVersion = 4.5

using Newtonsoft.Json;

#pragma warning disable 1591    //  Ignore "Missing XML Comment" warning


namespace CalibrusTPV.ReversePocoGen
{

    // TPV
    [System.CodeDom.Compiler.GeneratedCode("EF.Reverse.POCO.Generator", "2.28.0.0")]
    public class Tpv
    {
        public int Id { get; set; } // Id (Primary key)
        public int TpvAgentId { get; set; } // tpvAgentId
        public int UserId { get; set; } // UserId
        public string Dnis { get; set; } // Dnis (length: 10)
        public string Verified { get; set; } // Verified (length: 1)
        public string Btn { get; set; } // Btn (length: 10)
        public string ConcernCode { get; set; } // ConcernCode (length: 50)

        // Reverse navigation
        [JsonIgnore]
        public virtual System.Collections.Generic.ICollection<OrderDetail> OrderDetails { get; set; } // Many to many mapping
        [JsonIgnore]
        public virtual System.Collections.Generic.ICollection<Recording> Recordings { get; set; } // Recordings.FK_Recordings_TPV

        public Tpv()
        {
            OrderDetails = new System.Collections.Generic.List<OrderDetail>();
            Recordings = new System.Collections.Generic.List<Recording>();
        }
    }

}
// </auto-generated>
