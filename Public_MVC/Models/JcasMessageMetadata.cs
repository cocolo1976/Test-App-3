using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Linq;
namespace FairfaxCounty.JCAS_Public_MVC.Models
{
    /// <summary>Extension of the JcasMessageMetadata class from the DefaultDatabase LinqToSql DataContext.</summary>
    [MetadataType(typeof(JcasMessageMetadata))]
    public partial class JcasMessage : IHasUserLoginIdUpdate
    {
    }

    /// <summary>Metadata for the JcasMessageMetadata class.</summary>
    /// <remarks>Extra class is necessary because partial properties are not supported.</remarks>
    [Description("System messages to display on the login page.")]
    public class JcasMessageMetadata
    {
        /// <summary>Metadata for Id</summary>
        [Key]
        [Required]
        [Display(Name = "Id", Description = "Unique number assigned by the system to identify this message.  Must be completed.")]
        public int Id { get; set; }
        /// <summary>Metadata for MessageText</summary>
        [Required]
        [Display(Name = "Message Text *", Description = "Text of the message to display.  Must be 2000 characters or less.  Must be completed.")]
        public string MessageText { get; set; }
        /// <summary>Metadata for DisplayAsError</summary>
        [Required]
        [Display(Name = "Display As Error *", Description = "Whether this message should be displayed as an error.  Must be completed.")]
        public bool DisplayAsError { get; set; }
        /// <summary>Metadata for ApplicationAvailable</summary>
        [Required]
        [Display(Name = "Application Available *", Description = "Whether the application should be available when this message is displayed.  Must be completed.")]
        public bool ApplicationAvailable { get; set; }
        /// <summary>Metadata for StartDateTime</summary>
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy hh:mm tt}", ApplyFormatInEditMode = true, ConvertEmptyStringToNull = true, NullDisplayText = "")]
        [Required]
        [Display(Name = "Start DateTime *", Description = "Date and time this message should begin being displayed.  Must be a valid date and time.  Must be completed.")]
        public DateTime StartDateTime { get; set; }
        /// <summary>Metadata for EndDateTime</summary>
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy hh:mm tt}", ApplyFormatInEditMode = true, ConvertEmptyStringToNull = true, NullDisplayText = "")]
        [Required]
        [Display(Name = "End DateTime *", Description = "Date and time this message should stop being displayed.  Must be a valid date and time.  Must be completed.")]
        public DateTime EndDateTime { get; set; }
        /// <summary>Metadata for UserLoginIdUpdate</summary>
        [Display(Name = "User LoginId Update", Description = "Identifies the last user to update this row.  Must be 10 characters or less.  Must be completed.")]
        public string UserLoginIdUpdate { get; set; }
        /// <summary>Metadata for RowVersionId</summary>
        [Display(Name = "RowVersionId", Description = "Identifies the current version of this row.  Must be completed.")]
        public byte[] RowVersionId { get; set; }
    }
}
