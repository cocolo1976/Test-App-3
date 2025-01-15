using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Linq;
namespace FairfaxCounty.JCAS_Public_MVC.Models
{
    /// <summary>Extension of the JcasRuleMetadata class from the DefaultDatabase LinqToSql DataContext.</summary>
    [MetadataType(typeof(JcasRuleMetadata))]
    public partial class JcasRule : IHasUserLoginIdUpdate
    {
    }

    /// <summary>Metadata for the JcasRuleMetadata class.</summary>
    /// <remarks>Extra class is necessary because partial properties are not supported.</remarks>
    [Description("List of all rules to process attorney requests for court sessions.")]
    public class JcasRuleMetadata
    {
        /// <summary>Metadata for Id</summary>
        [Key]
        [Required]
        [Display(Name = "Id", Description = "System generated number to uniquely identify each row.  Must be completed.")]
        public object Id { get; set; }
        /// <summary>Metadata for RuleName</summary>
        [Required]
        [Display(Name = "Rule Name *", Description = "Descriptive name of each rule.  Must be 100 characters or less.  Must be completed.")]
        public object RuleName { get; set; }
        /// <summary>Metadata for RuleDescription</summary>
        [Display(Name = "Rule Description", Description = "Detail description about this rule.  Must be 5000 characters or less.")]
        public object RuleDescription { get; set; }

        /// <summary>Metadata for SubmitFromDateTypeId</summary>
        [Required]
        [Display(Name = "Submit From *", Description = "Begin of the period attorneys may submit for court dates for this rule.  Must be completed.")]
        public object SubmitFromDateTypeId { get; set; }
        /// <summary>Metadata for SubmitFromSequence</summary>
        [Required]
        [Display(Name = "Submit From Sequence *", Description = "Identifies the sequence of the selected date type for SubmitFrom.  Must be completed.")]
        public object SubmitFromSequence { get; set; }
        /// <summary>Metadata for SubmitToDateTypeId</summary>
        [Required]
        [Display(Name = "Submit To *", Description = "End of the period attorneys may submit for court dates for this rule.  Must be completed.")]
        public object SubmitToDateTypeId { get; set; }
        /// <summary>Metadata for SubmitToSequence</summary>
        [Required]
        [Display(Name = "Submit To Sequence *", Description = "Identifies the sequence of the selected date type for SubmitTo.  Must be completed.")]
        public object SubmitToSequence { get; set; }
        /// <summary>Metadata for SessionMonthSequence</summary>
        [Required]
        [Display(Name = "Session Month Sequence *", Description = "Number of months ahead of the current month to display sessions to schedule.  Must be completed.")]
        public object SessionMonthSequence { get; set; }
        /// <summary>Metadata for SessionFromDateTypeId</summary>
        [Required]
        [Display(Name = "Session From *", Description = "Begin of the session date range attorneys may schedule for court dates for this rule.  Must be completed.")]
        public object SessionFromDateTypeId { get; set; }
        /// <summary>Metadata for SessionFromSequence</summary>
        [Required]
        [Display(Name = "Session From Sequence *", Description = "Identifies the sequence of the selected date type for SessionFrom.  Must be completed.")]
        public object SessionFromSequence { get; set; }
        /// <summary>Metadata for SessionToDateTypeId</summary>
        [Required]
        [Display(Name = "Session To *", Description = "End of the session date range attorneys may schedule for court dates for this rule.  Must be completed.")]
        public object SessionToDateTypeId { get; set; }
        /// <summary>Metadata for SessionToSequence</summary>
        [Required]
        [Display(Name = "Session To Sequence *", Description = "Identifies the sequence of the selected date type for SessionTo.  Must be completed.")]
        public object SessionToSequence { get; set; }
        /// <summary>Metadata for HearingTypeCount</summary>
        [Required]
        [Display(Name = "Hearing Type Count *", Description = "Maximum number of court sessions each attorney may schedule for each hearing type for this rule.  Must be completed.")]
        public object HearingTypeCount { get; set; }
        /// <summary>Metadata for UserGroup</summary>
        [Required]
        [Display(Name = "User Group *", Description = "Identifies the user group (Attorney or Internal) this rule is for.  Must be 20 characters or less.  Must be completed.")]
        [UIHint("UserGroup")]
        public object UserGroup { get; set; }
        /// <summary>Metadata for UserLoginIdUpdate</summary>
        [Display(Name = "User LoginId Update", Description = "Identifies the last user to update this row.  Must be 10 characters or less.  Must be completed.")]
        public object UserLoginIdUpdate { get; set; }
        /// <summary>Metadata for RowVersionId</summary>
        [Display(Name = "RowVersionId", Description = "Identifies the current version of this row.  Must be completed.")]
        public object RowVersionId { get; set; }
    }
}
