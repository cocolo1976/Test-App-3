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
    
    public partial class JcasHearingType
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public JcasHearingType()
        {
            this.JcasAttorneyHearingTypes = new HashSet<JcasAttorneyHearingType>();
            this.JcasSessions = new HashSet<JcasSession>();
        }
    
        public int Id { get; set; }
        public string HearingTypeCode { get; set; }
        public string HearingTypeName { get; set; }
        public string UserLoginIdUpdate { get; set; }
        public byte[] RowVersionId { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<JcasAttorneyHearingType> JcasAttorneyHearingTypes { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<JcasSession> JcasSessions { get; set; }
    }
}
