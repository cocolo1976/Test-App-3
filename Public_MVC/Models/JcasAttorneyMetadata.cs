using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Linq;

namespace FairfaxCounty.JCAS_Public_MVC.Models
{
    /// <summary>Extension of the JcasAttorneyMetadata class from the DefaultDatabase LinqToSql DataContext.</summary>
    [MetadataType(typeof(JcasAttorneyMetadata))]
    public partial class JcasAttorney : IHasUserLoginIdUpdate
    {
        /// <summary>Returns the JcasAttorneys to display on the specified page of a filtered, sorted, paged list.</summary>
        /// <param name="db">Database Context object that represents the database for this application.</param>
        /// <param name="viewBag">The dynamic view data dictionary for the page displaying the list.</param>
        /// <param name="filter">If specified, only items that contain this text are included in the list.</param>
        /// <param name="orderBy">If specified, the list is sorted in this order.</param>
        /// <param name="page">The number of the page within the list to display.</param>
        /// <param name="pageSize">The number of records per page.</param>
        /// <returns>JcasAttorneys to display on the specified page of a filtered, sorted, paged list.</returns>
        public static IQueryable<JcasAttorney> FilteredSortedPagedList(JcasEntities db, dynamic viewBag, string filter, string orderBy, int page = 1, int pageSize = 10)
        {
            viewBag.FullNameSortParm = string.IsNullOrEmpty(orderBy) ? "FullName_desc" : "";
            viewBag.BarNumberSortParm = orderBy == "BarNumber" ? "BarNumber_desc" : "BarNumber";
            viewBag.EmailAddressSortParm = orderBy == "EmailAddress" ? "EmailAddress_desc" : "EmailAddress";
            viewBag.LawFirmNameSortParm = orderBy == "LawFirmName" ? "LawFirmName_desc" : "LawFirmName";
            viewBag.PhoneNumberSortParm = orderBy == "PhoneNumber" ? "PhoneNumber_desc" : "PhoneNumber";
            viewBag.IsActiveSortParm = orderBy == "IsActive" ? "IsActive_desc" : "IsActive";

            page = page > 0 ? page : 1;
            pageSize = pageSize > 0 ? pageSize : 10;

            IQueryable<JcasAttorney> query = db.JcasAttorneys;
            if (!string.IsNullOrEmpty(filter))
            {
                query = query.Where(u => u.FullName.ToUpper().Contains(filter.ToUpper()) 
                    || u.BarNumber.ToUpper().Contains(filter.ToUpper()) 
                    || u.EmailAddress.ToUpper().Contains(filter.ToUpper()) 
                    || u.LawFirmName.ToUpper().Contains(filter.ToUpper())
                    || u.PhoneNumber.ToUpper().Contains(filter.ToUpper())
                    || u.IsActive.ToString().Contains(filter.ToUpper()));
            }
            switch (orderBy)
            {
                case "FullName": query = query.OrderBy(u => u.FullName); break;
                case "FullName_desc": query = query.OrderByDescending(u => u.FullName); break;
                case "BarNumber": query = query.OrderBy(u => u.BarNumber); break;
                case "BarNumber_desc": query = query.OrderByDescending(u => u.BarNumber); break;
                case "EmailAddress": query = query.OrderBy(u => u.EmailAddress); break;
                case "EmailAddress_desc": query = query.OrderByDescending(u => u.EmailAddress); break;
                case "LawFirmName": query = query.OrderBy(u => u.LawFirmName); break;
                case "LawFirmName_desc": query = query.OrderByDescending(u => u.LawFirmName); break;
                case "PhoneNumber": query = query.OrderBy(u => u.PhoneNumber); break;
                case "PhoneNumber_desc": query = query.OrderByDescending(u => u.PhoneNumber); break;
                case "IsActive": query = query.OrderBy(u => u.IsActive); break;
                case "IsActive_desc": query = query.OrderByDescending(u => u.IsActive); break;
                default:
                    query = query.OrderBy(u => u.FullName);
                    break;
            }
            double totalCount = query.Count();
            viewBag.TotalPages = Convert.ToInt32(Math.Ceiling(totalCount / pageSize));
            query = query.Skip((page - 1) * pageSize).Take(pageSize);
            return query;
        }
    }

    /// <summary>Metadata for the JcasAttorneyMetadata class.</summary>
    /// <remarks>Extra class is necessary because partial properties are not supported.</remarks>
    [Description("List of attorneys who may request court sessions within the JCAS application.")]
    public class JcasAttorneyMetadata
    {
        /// <summary>Metadata for Id</summary>
        [Key]
        [Required]
        [Display(Name = "Id", Description = "System generated number to uniquely identify each row.  Must be completed.")]
        public object Id { get; set; }
        /// <summary>Metadata for LastName</summary>
        [Required]
        [Display(Name = "Last Name *", Description = "Last name of the attorney.  Must be 50 characters or less.  Must be completed.")]
        public object LastName { get; set; }
        /// <summary>Metadata for FirstName</summary>
        [Required]
        [Display(Name = "First Name *", Description = "First name of the attorney.  Must be 50 characters or less.  Must be completed.")]
        public object FirstName { get; set; }
        /// <summary>Metadata for MiddleName</summary>
        [Display(Name = "Middle Name", Description = "Middle name of the attorney.  Must be 50 characters or less.")]
        public object MiddleName { get; set; }
        /// <summary>Metadata for FullName</summary>
        [Display(Name = "Attorney Name", Description = "Full name of the attorney.")]
        public object FullName { get; set; }
        /// <summary>Metadata for BarNumber</summary>
        [Required]
        [StringLength(maximumLength: 10, ErrorMessage = "Bar number must be between 4 and 10 digits.", MinimumLength = 4)]
        [Display(Name = "Bar Number *", Description = "Number issued by the Virginia State Bar to uniquely identify the attorney.  Must be 10 characters or less.  Must be completed.")]
        public object BarNumber { get; set; }
        /// <summary>Metadata for EmailAddress</summary>
        [Required]
        [EmailAddress]
        [Display(Name = "Email Address *", Description = "Email address where the attorney may be contacted.  Must be 300 characters or less.  Must be completed.")]
        public object EmailAddress { get; set; }
        /// <summary>Metadata for FirmName</summary>
        [Display(Name = "Law Firm Name", Description = "Name of the law firm where the attorney provides legal service.  Must be 100 characters or less.")]
        public object LawFirmName { get; set; }
        /// <summary>Metadata for PhoneNumber</summary>
        [Phone]
        [Required]
        [StringLength(maximumLength: 13, ErrorMessage = "Phone number is invalid.", MinimumLength = 13)]
        [Display(Name = "Phone Number *", Description = "Phone number where the attorney may be contacted.")]
        public object PhoneNumber { get; set; }
        /// <summary>Metadata for IsActive</summary>
        [Required]
        [DefaultValue(true)]
        [Display(Name = "Is Active", Description = "Whether the attorney is active in JCAS application.  Must be completed.")]
        public object IsActive { get; set; }
        /// <summary>Metadata for Password</summary>
        [Display(Name = "Password", Description = "Password the attorney will use to login to the application (encrypted).")]
        public object Password { get; set; }
        /// <summary>Metadata for PasswordExpiration</summary>
        [DisplayFormat(DataFormatString = "{0: MM/dd/yyyy}", ApplyFormatInEditMode = true, ConvertEmptyStringToNull = true, NullDisplayText = "")]
        [Display(Name = "Password Expires", Description = "Date the password expires and need to be changed.")]
        public object PasswordExpires { get; set; }
        /// <summary>Metadata for FailedLoginAttempts</summary>
        [Required]
        [DefaultValue(0)]
        [Display(Name = "Failed Login Attempts", Description = "Number of failed login attempts since last successful login.  Must be completed.")]
        public object FailedLoginAttempts { get; set; }
        /// <summary>Metadata for LastPasswordChange</summary>
        [DisplayFormat(DataFormatString = "{0: MM/dd/yyyy}", ApplyFormatInEditMode = true, ConvertEmptyStringToNull = true, NullDisplayText = "")]
        [Display(Name = "Last Password Change", Description = "Last date that the password was changed.")]
        public object LastPasswordChange { get; set; }
        /// <summary>Metadata for EmailIsConfirmed</summary>
        [Required]
        [DefaultValue(false)]
        [Display(Name = "Email Is Confirmed", Description = "Whether or not the email address has been confirmed.  Must be completed.")]
        public object EmailIsConfirmed { get; set; }
        /// <summary>Metadata for EmailConfirmationCode</summary>
        [Display(Name = "Email Confirmation Code", Description = "Randomly generated code used to confirm the email address.")]
        public object EmailConfirmationCode { get; set; }
        /// <summary>Metadata for EmailConfirmationExpires</summary>
        [DisplayFormat(DataFormatString = "{0: MM/dd/yyyy}", ApplyFormatInEditMode = true, ConvertEmptyStringToNull = true, NullDisplayText = "")]
        [Display(Name = "Email Confirmation Expires", Description = "Date that the randomly generated code used to confirm the email address expires.")]
        public object EmailConfirmationExpires { get; set; }
        /// <summary>Metadata for UserLoginIdUpdate</summary>
        [Display(Name = "User LoginId Update", Description = "Identifies the last user to update this row.  Must be 10 characters or less.  Must be completed.")]
        public object UserLoginIdUpdate { get; set; }
        /// <summary>Metadata for RowVersionId</summary>
        [Display(Name = "RowVersionId", Description = "Identifies the current version of this row.  Must be completed.")]
        public object RowVersionId { get; set; }
        /// <summary>Metadata for JcasAttorneyHearingTypes</summary>
        [Display(Name = "Hearing Types *", Description = "Identifies the hearing types the attorney is ceritfied to provide service.  Must be completed.")]
        public object JcasAttorneyHearingTypes{ get; set; }
    }
}
