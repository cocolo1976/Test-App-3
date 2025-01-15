using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FairfaxCounty.JCAS_Public_MVC.Models
{
    /// <summary>Extension of Entities class from the Entity Data Model.</summary>
    public partial class JcasEntities
    {
        /// <summary>Saves all changes made in this context to the underlying database and adds any errors to the specified ModelState.</summary>
        /// <param name="modelState">Object that represents the state of model binding to a property.</param>
        /// <returns>The number of objects written to the underlying database.</returns>
        public int SaveChanges(ModelStateDictionary modelState)
        {
            try
            {
                return SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                DbEntityEntry dbEntry = this.ChangeTracker.Entries().Where(p => p.State == EntityState.Modified).FirstOrDefault();
                SetCorrectOriginalValues(dbEntry);

                foreach (string propertyName in dbEntry.OriginalValues.PropertyNames)
                {
                    if (!object.Equals(dbEntry.OriginalValues.GetValue<object>(propertyName), dbEntry.CurrentValues.GetValue<object>(propertyName)))
                    {
                        modelState.AddModelError(propertyName, "Stored Value: " + dbEntry.OriginalValues.GetValue<object>(propertyName).ToString());
                    }
                }
                modelState.AddModelError(string.Empty, "Unable to update.  Already updated by another user.");
                dbEntry.Reload();
            }
            catch (DbEntityValidationException ex)
            {
                foreach (var eve in ex.EntityValidationErrors)
                {
                    foreach (var ve in eve.ValidationErrors)
                    {
                        modelState.AddModelError(ve.PropertyName, ve.ErrorMessage);
                    }
                }
            }
            catch (Exception ex)
            {
                string strMessage = JcasDescription.GetDatabaseMessage(this, ex);
                if (!string.IsNullOrEmpty(strMessage))
                {
                    modelState.AddModelError(string.Empty, strMessage);
                }
                else
                {
                    throw;
                }
            }
            return 0;
        }
        /// <summary>For modified entries, gets the original values from the database.</summary>
        /// <param name="Modified">Entry that was modified.</param>
        void SetCorrectOriginalValues(DbEntityEntry Modified)
        {
            var values = Modified.CurrentValues.Clone();
            Modified.Reload();
            Modified.CurrentValues.SetValues(values);
            try
            {
                Modified.OriginalValues["RowVersionId"] = values["RowVersionId"];
            }
            finally
            {
                Modified.State = EntityState.Modified;
            }
        }

        /// <summary>Saves all changes made in this context to the underlying database.  
        /// Prior to saving, for items that implement the IHasUserLoginIdUpdate interface, sets the UserLoginIdUpdate property to the current user,
        /// so that triggers in the database can log who was responsible for making the changes.</summary>
        /// <returns>The number of objects written to the underlying database.</returns>
        public override int SaveChanges()
        {
            List<DbEntityEntry> delEntries = new List<DbEntityEntry>();

            // Get all Added/Deleted/Modified entities (not Unmodified or Detached)
            foreach (var ent in this.ChangeTracker.Entries().Where(p => p.State == EntityState.Added || p.State == EntityState.Deleted || p.State == EntityState.Modified))
            {
                if (ent.Entity is IHasUserLoginIdUpdate)
                {
                    ((IHasUserLoginIdUpdate)ent.Entity).UserLoginIdUpdate = HttpContext.Current.User.Identity.Name.ToString();
                    if (ent.State == EntityState.Deleted)
                    {
                        delEntries.Add(ent);
                        ent.State = EntityState.Modified;
                    }
                }
            }
            if (delEntries.Count > 0)
            {
                base.SaveChanges();
                foreach (DbEntityEntry ent in delEntries)
                {
                    ent.Reload();
                    ent.State = EntityState.Deleted;
                }
            }
            return base.SaveChanges();
        }
    }

    /// <summary>Interface implemented by items in the model that have a UserLoginIdUpdate property that is
    /// used by triggers in the database to log who was responsible for making changes to the item.</summary>
    public interface IHasUserLoginIdUpdate
    {
        string UserLoginIdUpdate { get; set; }
    }
}