using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Linq;
namespace FairfaxCounty.JCAS_Public_MVC.Models
{
    /// <summary>Extension of the JcasCourtDateNoteMetadata class from the DefaultDatabase LinqToSql DataContext.</summary>
    [MetadataType(typeof(JcasCourtDateNoteMetadata))]
    public partial class JcasCourtDateNote : IHasUserLoginIdUpdate
    {
    }

    /// <summary>Metadata for the JcasCourtDateNoteMetadata class.</summary>
    /// <remarks>Extra class is necessary because partial properties are not supported.</remarks>
    [Description("Notes to display on the calendar for each court date, where applicable.")]
    public class JcasCourtDateNoteMetadata
    {
        /// <summary>Metadata for Id</summary>
        [Key]
        [Required]
        [Display(Name = "Id", Description = "System generated number to uniquely identify each row.  Must be completed.")]
        public object Id { get; set; }
        /// <summary>Metadata for FromDate</summary>
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}", ApplyFormatInEditMode = true, ConvertEmptyStringToNull = true, NullDisplayText = "")]
        [Required]
        [Display(Name = "From Date", Description = "Begin of the date range for the court date note.  Must be completed.")]
        public object FromDate { get; set; }
        /// <summary>Metadata for ToDate</summary>
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}", ApplyFormatInEditMode = true, ConvertEmptyStringToNull = true, NullDisplayText = "")]
        [Required]
        [Display(Name = "To Date", Description = "End of the date range for the court date note.  Must be completed.")]
        public object ToDate { get; set; }
        /// <summary>Metadata for Note</summary>
        [Required]
        [Display(Name = "Note", Description = "Note about the court date(s).  Must be 500 characters or less.  Must be completed.")]
        public object Note { get; set; }
        /// <summary>Metadata for DisplayOrder</summary>
        [Display(Name = "Display Order", Description = "Order to display this note on the calendar.  Must be an integer.")]
        [Required()]
        [DefaultValue(0)]
        [Range(0, 1000)]
        public object DisplayOrder { get; set; }
        /// <summary>Metadata for UserLoginIdUpdate</summary>
        [Display(Name = "UserLoginIdUpdate", Description = "Identifies the last user to update this row.  Must be 10 characters or less.  Must be completed.")]
        public object UserLoginIdUpdate { get; set; }
        /// <summary>Metadata for RowVersionId</summary>
        [Display(Name = "RowVersionId", Description = "Identifies the current version of this row.  Must be completed.")]
        public object RowVersionId { get; set; }
    }
}
