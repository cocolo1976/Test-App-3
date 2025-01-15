using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Linq;
namespace FairfaxCounty.JCAS_Public_MVC.Models
{
    /// <summary>Extension of the JcasSettingMetadata class from the DefaultDatabase LinqToSql DataContext.</summary>
    [MetadataType(typeof(JcasSettingMetadata))]
    public partial class JcasSetting : IHasUserLoginIdUpdate
    {
    }

    /// <summary>Metadata for the JcasSettingMetadata class.</summary>
    /// <remarks>Extra class is necessary because partial properties are not supported.</remarks>
    [Description("Configuration settings for the application that Sys Admin users may adjust.")]
    public class JcasSettingMetadata
    {
        /// <summary>Metadata for Id</summary>
        [Key]
        [Required]
        [Display(Name = "Id", Description = "Unique number assigned by the system to identify this setting.  Must be completed.")]
        public object Id { get; set; }
        /// <summary>Metadata for SettingName</summary>
        [Required]
        [Display(Name = "Setting Name *", Description = "Descriptive name for the setting.  Must be 50 characters or less.  Must be completed.")]
        public object SettingName { get; set; }
        /// <summary>Metadata for SettingValue</summary>
        [Display(Name = "Setting Value", Description = "Current value of the configuration setting.")]
        public object SettingValue { get; set; }
        /// <summary>Metadata for SettingLabelText</summary>
        [Display(Name = "Setting Label Text", Description = "Text to display to the left of the control when prompting for this setting value.  Must be 50 characters or less.")]
        public object SettingLabelText { get; set; }
        /// <summary>Metadata for SettingDataType</summary>
        [Required]
        [Display(Name = "Setting DataType *", Description = "Type of data to accept as the value of this setting.  Must be 50 characters or less.  Must be completed.")]
        public object SettingDataType { get; set; }
        /// <summary>Metadata for SettingMaxLength</summary>
        [Required]
        [Display(Name = "Setting Max Length *", Description = "Maximum number of characters to accept for the value.  Must be completed.")]
        public object SettingMaxLength { get; set; }
        /// <summary>Metadata for SettingRequired</summary>
        [Required]
        [Display(Name = "Setting Required *", Description = "Whether this setting value is required.  Must be completed.")]
        public object SettingRequired { get; set; }
        /// <summary>Metadata for SettingTooltip</summary>
        [Display(Name = "Setting Tooltip", Description = "Tooltip to display when the cursor is over the control for this setting.  Must be 300 characters or less.")]
        public object SettingTooltip { get; set; }
        /// <summary>Metadata for SettingOrder</summary>
        [Required]
        [Display(Name = "Setting Order *", Description = "Identifies the order in which items are listed.  Must be completed.")]
        public object SettingOrder { get; set; }
        /// <summary>Metadata for SettingVisible</summary>
        [Required]
        [Display(Name = "Setting Visible *", Description = "Whether this setting is visible to users.  Must be completed.")]
        public object SettingVisible { get; set; }
        /// <summary>Metadata for UserLoginIdUpdate</summary>
        [Display(Name = "User LoginId Update", Description = "Identifies the last user to update this row.  Must be 10 characters or less.  Must be completed.")]
        public object UserLoginIdUpdate { get; set; }
        /// <summary>Metadata for RowVersionId</summary>
        [Display(Name = "RowVersionId", Description = "Identifies the current version of this row.  Must be completed.")]
        public object RowVersionId { get; set; }
    }
}
