using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FairfaxCounty.JCAS_Public_MVC.ViewModels
{
    /// <summary>View Model for allowing a user to indicate that they have forgotten their password.</summary>
    /// <remarks>Only external users may reset their passwords using the app.</remarks>
    public class ForgotPasswordViewModel
    {
        /// <summary>The email address that uniquely identifies the external user.</summary>
        [Required]
        [Display(Name = "Email Address *")]
        public string EmailAddress { get; set; }

    }
}