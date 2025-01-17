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
    
    public partial class JcasDateType
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public JcasDateType()
        {
            this.JcasRules = new HashSet<JcasRule>();
            this.JcasRules1 = new HashSet<JcasRule>();
            this.JcasRules2 = new HashSet<JcasRule>();
            this.JcasRules3 = new HashSet<JcasRule>();
        }
    
        public int Id { get; set; }
        public string DateTypeName { get; set; }
        public Nullable<int> MaximumSequence { get; set; }
        public string UserLoginIdUpdate { get; set; }
        public byte[] RowVersionId { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<JcasRule> JcasRules { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<JcasRule> JcasRules1 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<JcasRule> JcasRules2 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<JcasRule> JcasRules3 { get; set; }
    }
}
