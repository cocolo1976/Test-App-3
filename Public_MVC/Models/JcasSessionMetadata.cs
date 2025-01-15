using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Linq;
using System.Web;
using System.Data.Entity;
using System.Data.Entity.SqlServer;

namespace FairfaxCounty.JCAS_Public_MVC.Models
{
    /// <summary>Extension of the JcasSessionMetadata class from the DefaultDatabase LinqToSql DataContext.</summary>
    [MetadataType(typeof(JcasSessionMetadata))]
    public partial class JcasSession : IHasUserLoginIdUpdate
    {
    }

    /// <summary>Metadata for the JcasSessionMetadata class.</summary>
    /// <remarks>Extra class is necessary because partial properties are not supported.</remarks>
    [Description("List of dates the court is in session.")]
    public class JcasSessionMetadata
    {
        /// <summary>Metadata for Id</summary>
        [Key]
        [Required]
        [Display(Name = "Id", Description = "System generated number to uniquely identify each row.  Must be completed.")]
        public object Id { get; set; }
        /// <summary>Metadata for SessionDate</summary>
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}", ApplyFormatInEditMode = true, ConvertEmptyStringToNull = true, NullDisplayText = "")]
        [Required]
        [Display(Name = "Session Date *", Description = "Identify the date the court is in session.  Must be completed.")]
        public object SessionDate { get; set; }
        /// <summary>Metadata for AttorneyId</summary>
        [Display(Name = "Attorney", Description = "Identifies the attorney scheduled for this court session and hearing type.")]
        public object AttorneyId { get; set; }
        /// <summary>Metadata for HearingTypeId</summary>
        [Required]
        [Display(Name = "Hearing Type *", Description = "Identifies the hearing type for the court session.  Must be completed.")]
        public object HearingTypeId { get; set; }
        /// <summary>Metadata for CourtroomId</summary>
        [Display(Name = "Courtroom", Description = "Identifies the courtroom assigned to this session and hearing type (if any).")]
        public object CourtroomId { get; set; }
        /// <summary>Metadata for SubmittedDate</summary>
        [Display(Name = "Submitted Date", Description = "Date the attorney is scheduled for the session.")]
        public object SubmittedDate { get; set; }
        /// <summary>Metadata for UserLoginIdUpdate</summary>
        [Display(Name = "User LoginId Update", Description = "Identifies the last user to update this row.  Must be 10 characters or less.  Must be completed.")]
        public object UserLoginIdUpdate { get; set; }
        /// <summary>Metadata for RowVersionId</summary>
        [Display(Name = "RowVersionId", Description = "Identifies the current version of this row.  Must be completed.")]
        public object RowVersionId { get; set; }
    }
}
