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
    
    public partial class JcasCourtClosed
    {
        public int Id { get; set; }
        public System.DateTime CourtClosedDate { get; set; }
        public string CourtClosedReason { get; set; }
        public string UserLoginIdUpdate { get; set; }
        public byte[] RowVersionId { get; set; }
    }
}
