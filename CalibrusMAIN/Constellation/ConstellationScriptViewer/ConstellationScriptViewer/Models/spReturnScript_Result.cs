//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ConstellationScriptViewer.Models
{
    using System;
    
    public partial class spReturnScript_Result
    {
        public int ScriptId { get; set; }
        public Nullable<int> ScriptOrder { get; set; }
        public bool Active { get; set; }
        public string Verbiage { get; set; }
        public string VerbiageSpanish { get; set; }
        public string Condition { get; set; }
        public Nullable<bool> YesNo { get; set; }
        public Nullable<bool> TextBox { get; set; }
        public string TextBoxValue { get; set; }
        public Nullable<bool> BlueNote { get; set; }
        public string BlueNoteVerbiage { get; set; }
        public string NoVerbiage { get; set; }
        public string NoVerbiageSpanish { get; set; }
        public string NoConcern { get; set; }
        public string NoConcernCode { get; set; }
        public int History { get; set; }
    }
}
