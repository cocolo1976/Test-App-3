using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Linq;
namespace FairfaxCounty.JCAS_Public_MVC.Models
{
    /// <summary>Extension of the JcasCourtroomMetadata class from the DefaultDatabase LinqToSql DataContext.</summary>
    [MetadataType(typeof(JcasCourtroomMetadata))]
    public partial class JcasCourtroom : IHasUserLoginIdUpdate
    {
    }

    /// <summary>Metadata for the JcasCourtroomMetadata class.</summary>
    /// <remarks>Extra class is necessary because partial properties are not supported.</remarks>
    [Description("List of all JDC courtrooms that can be assigned to hearings.")]
    [DisplayName("Courtroom")]
    [DisplayColumn("Courtroom", sortColumn: "Courtroom")]
    public class JcasCourtroomMetadata
    {
        /// <summary>Metadata for Id</summary>
        [Key]
        [Required]
        [Display(Name = "Id", Description = "System generated number to uniquely identify each row.  Must be completed.")]
        public object Id { get; set; }
        /// <summary>Metadata for Courtroom</summary>
        [Required]
        [Display(Name = "Courtroom *", Description = "Identify the courtroom used for hearing assignments.  Must be 20 characters or less.  Must be completed.")]
        public object Courtroom { get; set; }
        /// <summary>Metadata for UserLoginIdUpdate</summary>
        [Display(Name = "User LoginId Update", Description = "Identifies the last user to update this row.  Must be 10 characters or less.  Must be completed.")]
        public object UserLoginIdUpdate { get; set; }
        /// <summary>Metadata for RowVersionId</summary>
        [Display(Name = "RowVersionId", Description = "Identifies the current version of this row.  Must be completed.")]
        public object RowVersionId { get; set; }
    }
}
