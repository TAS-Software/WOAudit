//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace WOAudit
{
    using System;
    using System.Collections.Generic;
    
    public partial class WOAuditDump
    {
        public long WODump_ID { get; set; }
        public int WOPart { get; set; }
        public int WOTID { get; set; }
        public string WorksOrderNumber { get; set; }
        public string WorksOrderType { get; set; }
        public string WOMethod { get; set; }
        public decimal BatchQuantity { get; set; }
        public decimal QuantityStored { get; set; }
        public string WORespCode { get; set; }
        public bool OnHold { get; set; }
        public bool Exclude { get; set; }
        public string WOIssue { get; set; }
        public System.DateTime CompletionDate { get; set; }
        public Nullable<System.DateTime> ActualReceiptDate { get; set; }
        public string WorksOrderStatus { get; set; }
        public Nullable<int> CompPartID { get; set; }
        public string CompPartNumber { get; set; }
        public Nullable<decimal> PlannedIssueQuantity { get; set; }
        public Nullable<decimal> ActualIssueQuantity { get; set; }
        public Nullable<System.DateTime> PlannedIssueDate { get; set; }
        public string WOCompIssueStatus { get; set; }
        public Nullable<bool> CompIssued { get; set; }
        public Nullable<bool> IsStoreRequest { get; set; }
        public Nullable<int> SequenceNo { get; set; }
        public Nullable<bool> MRPIgnore { get; set; }
        public Nullable<System.DateTime> WOPLastModifiedDate { get; set; }
        public Nullable<System.DateTime> WOLastModifiedDate { get; set; }
    }
}
