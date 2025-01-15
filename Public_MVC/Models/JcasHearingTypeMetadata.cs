using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Linq;
namespace FairfaxCounty.JCAS_Public_MVC.Models
{
    /// <summary>Extension of the JcasHearingTypeMetadata class from the DefaultDatabase LinqToSql DataContext.</summary>
    [MetadataType(typeof(JcasHearingTypeMetadata))]
    public partial class JcasHearingType : IHasUserLoginIdUpdate
    {
    }

    /// <summary>Metadata for the JcasHearingTypeMetadata class.</summary>
    /// <remarks>Extra class is necessary because partial properties are not supported.</remarks>
    [Description("List of hearing types attorneys may request for their schedule.")]
    [DisplayName("Hearing Types")]
    public class JcasHearingTypeMetadata
    {
        /// <summary>Metadata for Id</summary>
        [Key]
        [Required]
        [Display(Name = "Id", Description = "System generated number to uniquely identify each row.  Must be completed.")]
        public object Id { get; set; }
        /// <summary>Metadata for HearingTypeCode</summary>
        [Required]
        [Display(Name = "Hearing Type Code *", Description = "Code to uniquely identify the hearing type.  Must be 5 characters or less.  Must be completed.")]
        public object HearingTypeCode { get; set; }
        /// <summary>Metadata for HearingTypeName</summary>
        [Required]
        [Display(Name = "Hearing Type Name *", Description = "Description of the hearing type.  Must be 50 characters or less.  Must be completed.")]
        public object HearingTypeName { get; set; }
        /// <summary>Metadata for UserLoginIdUpdate</summary>
        [Display(Name = "User LoginId Update", Description = "Identifies the last user to update this row.  Must be 10 characters or less.  Must be completed.")]
        public object UserLoginIdUpdate { get; set; }
        /// <summary>Metadata for RowVersionId</summary>
        [Display(Name = "RowVersionId", Description = "Identifies the current version of this row.  Must be completed.")]
        public object RowVersionId { get; set; }
    }
}
