﻿//------------------------------------------------------------------------------
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
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    using System.Data.Entity.Core.Objects;
    using System.Linq;
    
    public partial class liveThas01Entities : DbContext
    {
        public liveThas01Entities()
            : base("name=liveThas01Entities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
    
        public virtual int THAS_CONNECT_CopyLiveWODatasetToTable()
        {
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("THAS_CONNECT_CopyLiveWODatasetToTable");
        }
    
        public virtual int THAS_CONNECT_CopyWODatasetToTable()
        {
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("THAS_CONNECT_CopyWODatasetToTable");
        }
    
        public virtual ObjectResult<Nullable<int>> THAS_CONNECT_GetWOPNumber(Nullable<int> wOPartID)
        {
            var wOPartIDParameter = wOPartID.HasValue ?
                new ObjectParameter("WOPartID", wOPartID) :
                new ObjectParameter("WOPartID", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<Nullable<int>>("THAS_CONNECT_GetWOPNumber", wOPartIDParameter);
        }
    
        public virtual int THAS_CONNECT_CopyWO_WOP_NumbersToTable()
        {
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("THAS_CONNECT_CopyWO_WOP_NumbersToTable");
        }
    
        public virtual ObjectResult<Nullable<int>> THAS_CONNECT_GetLiveWOPNumber(Nullable<int> wOPartID)
        {
            var wOPartIDParameter = wOPartID.HasValue ?
                new ObjectParameter("WOPartID", wOPartID) :
                new ObjectParameter("WOPartID", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<Nullable<int>>("THAS_CONNECT_GetLiveWOPNumber", wOPartIDParameter);
        }
    
        public virtual ObjectResult<Nullable<int>> THAS_CONNECT_GetLiveWONumber(Nullable<int> wOTID)
        {
            var wOTIDParameter = wOTID.HasValue ?
                new ObjectParameter("WOTID", wOTID) :
                new ObjectParameter("WOTID", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<Nullable<int>>("THAS_CONNECT_GetLiveWONumber", wOTIDParameter);
        }
    
        public virtual ObjectResult<Nullable<int>> THAS_CONNECT_GetWONumber(Nullable<int> wOTID)
        {
            var wOTIDParameter = wOTID.HasValue ?
                new ObjectParameter("WOTID", wOTID) :
                new ObjectParameter("WOTID", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<Nullable<int>>("THAS_CONNECT_GetWONumber", wOTIDParameter);
        }
    
        public virtual int THAS_CONNECT_CopyWO_NumbersToTable()
        {
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("THAS_CONNECT_CopyWO_NumbersToTable");
        }
    
        public virtual int THAS_CONNECT_CopyWOP_NumbersToTable()
        {
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("THAS_CONNECT_CopyWOP_NumbersToTable");
        }
    }
}
