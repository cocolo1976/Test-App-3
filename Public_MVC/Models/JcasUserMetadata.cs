using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Linq;
namespace FairfaxCounty.JCAS_Public_MVC.Models
{
    /// <summary>Extension of the JcasUserMetadata class from the DefaultDatabase LinqToSql DataContext.</summary>
    [MetadataType(typeof(JcasUserMetadata))]
    public partial class JcasUser : IHasUserLoginIdUpdate
    {
        /// <summary>Returns the JcasUsers to display on the specified page of a filtered, sorted, paged list.</summary>
        /// <param name="db">Database Context object that represents the database for this application.</param>
        /// <param name="viewBag">The dynamic view data dictionary for the page displaying the list.</param>
        /// <param name="filter">If specified, only items that contain this text are included in the list.</param>
        /// <param name="orderBy">If specified, the list is sorted in this order.</param>
        /// <param name="page">The number of the page within the list to display.</param>
        /// <param name="pageSize">The number of records per page.</param>
        /// <returns>JcasUsers to display on the specified page of a filtered, sorted, paged list.</returns>
        public static IQueryable<JcasUser> FilteredSortedPagedList(JcasEntities db, dynamic viewBag, string filter, string orderBy, int page = 1, int pageSize = 10)
        {
            viewBag.UserLoginIdSortParm = string.IsNullOrEmpty(orderBy) ? "UserLoginId_desc" : "";
            viewBag.UserNameSortParm = orderBy == "UserName" ? "UserName_desc" : "UserName";
            viewBag.UserRoleSortParm = orderBy == "UserRole" ? "UserRole_desc" : "UserRole";
            page = page > 0 ? page : 1;
            pageSize = pageSize > 0 ? pageSize : 10;

            IQueryable<JcasUser> query = db.JcasUsers;
            if (!string.IsNullOrEmpty(filter))
            {
                query = query.Where(u => u.UserLoginId.ToUpper().Contains(filter.ToUpper()) 
                    || u.UserName.ToUpper().Contains(filter.ToUpper()) 
                    || u.UserRole.ToUpper().Contains(filter.ToUpper()));
            }
            switch (orderBy)
            {
                case "UserLoginId": query = query.OrderBy(u => u.UserLoginId); break;
                case "UserLoginId_desc": query = query.OrderByDescending(u => u.UserLoginId); break;
                case "UserName": query = query.OrderBy(u => u.UserName); break;
                case "UserName_desc": query = query.OrderByDescending(u => u.UserName); break;
                case "UserRole": query = query.OrderBy(u => u.UserRole); break;
                case "UserRole_desc": query = query.OrderByDescending(u => u.UserRole); break;
                default:
                    query = query.OrderBy(u => u.UserLoginId);
                    break;
            }
            double totalCount = query.Count();
            viewBag.TotalPages = Convert.ToInt32(Math.Ceiling(totalCount / pageSize));
            query = query.Skip((page - 1) * pageSize).Take(pageSize);
            return query;
        }
    }

    /// <summary>Metadata for the JcasUserMetadata class.</summary>
    /// <remarks>Extra class is necessary because partial properties are not supported.</remarks>
    [Description("Users of the Jcas application.")]
    public class JcasUserMetadata
    {
        /// <summary>Metadata for Id</summary>
        [Key]
        [Required]
        [Display(Name = "Id", Description = "System generated number to uniquely identify each row.  Must be completed.")]
        public object Id { get; set; }
        /// <summary>Metadata for UserLoginId</summary>
        [Required]
        [Display(Name = "User Login Id *", Description = "Network Id the user uses to login to the application.  Must be 10 characters or less.  Must be completed.")]
        public object UserLoginId { get; set; }
        /// <summary>Metadata for UserName</summary>
        [Required]
        [Display(Name = "User Name *", Description = "Full name of the user (First Middle Last).  Must be 50 characters or less.  Must be completed.")]
        public object UserName { get; set; }
        /// <summary>Metadata for UserRole</summary>
        [Display(Name = "User Role", Description = "Role that determines what the user can do within the application.  Must be 50 characters or less.")]
        public object UserRole { get; set; }
        /// <summary>Metadata for UserLoginIdUpdate</summary>
        [Display(Name = "User LoginId Update", Description = "Identifies the last user to update this row.  Must be 10 characters or less.  Must be completed.")]
        public object UserLoginIdUpdate { get; set; }
        /// <summary>Metadata for RowVersionId</summary>
        [Display(Name = "RowVersionId", Description = "Identifies the current version of this row.  Must be completed.")]
        public object RowVersionId { get; set; }
    }
}
