using System.ComponentModel.DataAnnotations;
using FairfaxCounty.JCAS_Public_MVC.Models;

namespace FairfaxCounty.JCAS_Public_MVC.ViewModels
{
    /// <summary>View Model to allow a user to change their password.</summary>
    public class ChangePasswordViewModel
    {
        /// <summary>Message that appears at the top of the Change Password page.</summary>
        public string ChangePasswordMessage { get; set; }

        /// <summary>Email address of the user (used as the User Name for login).</summary>
        [Required(ErrorMessage = "Email Address: must be completed.")]
        [Display(Name = "Email Address *")]
        [MaxLength(300, ErrorMessage = "Email Address must be 300 characters or less.")]
        [EmailAddress]
        public string EmailAddress { get; set; }

        /// <summary>New password selected by the user.</summary>
        [Required(ErrorMessage = "New Password: must be completed.")]
        [MinLength(8, ErrorMessage = "New Password: must be at least 8 characters.")]
        [RegularExpression("^((?=.*[a-z])(?=.*[A-Z])(?=.*[0-9])(?=.*[!@#\\$%\\^&\\*_ ])).*$", ErrorMessage = "New Password: must have at least one special character, at least one lower case letter, at least one upper case letter, and at least one number.")]
        public string NewPassword { get; set; }

        /// <summary>New password selected by the user repeated (to verify the password).</summary>
        [Required(ErrorMessage = "Repeat New Password: must be completed.")]
        [Compare("NewPassword", ErrorMessage = "Repeat New Password: must match New Password.")]
        public string RepeatPassword { get; set; }

        /// <summary>Confirmation code in the url sent to the email address.</summary>
        public string EmailConfirmationCode { get; set; }

        /// <summary>Populates the Change Password message.</summary>
        /// <param name="db">Database Context object that represents the database for this application.</param>
        public void RefreshLookups(JcasEntities db)
        {
            ChangePasswordMessage = Helpers.Utility.GetSettingValue("ChangePasswordMessage");
        }

        /// <summary>Creates an instance of the model with the message populated.</summary>
        public ChangePasswordViewModel(JcasEntities db)
        {
            RefreshLookups(db);
        }

        /// <summary>Creates an empty instance of the model.</summary>
        public ChangePasswordViewModel()
        {
        }
    }
}