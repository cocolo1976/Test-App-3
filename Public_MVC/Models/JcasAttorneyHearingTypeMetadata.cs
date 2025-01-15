using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Linq;
namespace FairfaxCounty.JCAS_Public_MVC.Models
{
    /// <summary>Extension of the JcasAttorneyHearingTypeMetadata class from the DefaultDatabase LinqToSql DataContext.</summary>
    [MetadataType(typeof(JcasAttorneyHearingTypeMetadata))]
    public partial class JcasAttorneyHearingType : IHasUserLoginIdUpdate
    {
    }

    /// <summary>Metadata for the JcasAttorneyHearingTypeMetadata class.</summary>
    /// <remarks>Extra class is necessary because partial properties are not supported.</remarks>
    [Description("List of hearing types each attorney is ceritfied to provide service.")]
    [DisplayName("Hearing Types")]
    public class JcasAttorneyHearingTypeMetadata
    {
        /// <summary>Metadata for Id</summary>
        [Key]
        [Required]
        [Display(Name = "Id", Description = "System generated number to uniquely identify each row.  Must be completed.")]
        public object Id { get; set; }
        /// <summary>Metadata for AttorneyId</summary>
        [Required]
        [Display(Name = "Attorney", Description = "Identify the attorney for each case type.  Must be completed.")]
        public object AttorneyId { get; set; }
        /// <summary>Metadata for HearingTypeId</summary>
        [Required]
        [Display(Name = "Hearing Type", Description = "Identify each hearing type the attorney is certified to provide service.  Must be completed.")]
        public object HearingTypeId { get; set; }
        /// <summary>Metadata for UserLoginIdUpdate</summary>
        [Display(Name = "User LoginId Update", Description = "Identifies the last user to update this row.  Must be 10 characters or less.  Must be completed.")]
        public object UserLoginIdUpdate { get; set; }
        /// <summary>Metadata for RowVersionId</summary>
        [Display(Name = "RowVersionId", Description = "Identifies the current version of this row.  Must be completed.")]
        public object RowVersionId { get; set; }
    }
}
