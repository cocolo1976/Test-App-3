using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Linq;
namespace FairfaxCounty.JCAS_Public_MVC.Models
{
    /// <summary>Extension of the JcasCourtClosedMetadata class from the DefaultDatabase LinqToSql DataContext.</summary>
    [MetadataType(typeof(JcasCourtClosedMetadata))]
    public partial class JcasCourtClosed : IHasUserLoginIdUpdate
    {
    }

    /// <summary>Metadata for the JcasCourtClosedMetadata class.</summary>
    /// <remarks>Extra class is necessary because partial properties are not supported.</remarks>
    [Description("List of dates the court is closed.")]
    public class JcasCourtClosedMetadata
    {
        /// <summary>Metadata for Id</summary>
        [Key]
        [Required]
        [Display(Name = "Id", Description = "System generated number to uniquely identify each row.  Must be completed.")]
        public object Id { get; set; }
        /// <summary>Metadata for CourtClosedDate</summary>
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}", ApplyFormatInEditMode = true, ConvertEmptyStringToNull = true, NullDisplayText = "")]
        [Required]
        [Display(Name = "Court Closed Date *", Description = "Date the court is closed.  Must be completed.")]
        public object CourtClosedDate { get; set; }
        /// <summary>Metadata for CourtClosedReason</summary>
        [Required]
        [Display(Name = "Court Closed Reason *", Description = "Reason the court is closed.  Must be 50 characters or less.  Must be completed.")]
        public object CourtClosedReason { get; set; }
        /// <summary>Metadata for UserLoginIdUpdate</summary>
        [Display(Name = "User LoginId Update", Description = "Identifies the last user to update this row.  Must be 10 characters or less.  Must be completed.")]
        public object UserLoginIdUpdate { get; set; }
        /// <summary>Metadata for RowVersionId</summary>
        [Display(Name = "RowVersionId", Description = "Identifies the current version of this row.  Must be completed.")]
        public object RowVersionId { get; set; }
    }
}
