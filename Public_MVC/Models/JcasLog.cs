//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace FairfaxCounty.JCAS_Public_MVC.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class JcasLog
    {
        public int Id { get; set; }
        public string TableId { get; set; }
        public string ColumnId { get; set; }
        public int RowId { get; set; }
        public string NewValue { get; set; }
        public string LogAction { get; set; }
        public System.DateTime LogDateTime { get; set; }
        public string UserLoginIdUpdate { get; set; }
    }
}
