using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FairfaxCounty.JCAS_Public_MVC.Models;

namespace FairfaxCounty.JCAS_Public_MVC.ViewModels
{
    /// <summary>View model for attorney hearing types.</summary>
    public class AttorneyHearingTypeViewModel
    {
        /// <summary>Object that represents the attorney.</summary>
        public JcasAttorney Attorney { get; set; }

        /// <summary>Comma delimited list of Id's that identify the HearingTypes this Attorney has.</summary>
        [Required(ErrorMessage = "At least one hearing type is required.")]
        [Display(Name = "Hearing Types *")]
        public string AttorneyHearingTypes { get; set; }

        /// <summary>List of possible values for HearingTypes used to populate the checkbox list.</summary>
        public Dictionary<string, string> HearingTypeLookup { get; set; }

        /// <summary>New instance of the view model.</summary>
        public AttorneyHearingTypeViewModel()
        {
        }

        /// <summary>Get all hearing types the attorney is cetified to provide service.</summary>
        /// <param name="db">Database context object that represents the database for this application.</param>
        /// <param name="AttorneyId">The attorney to list the hearing types for.</param>
        public AttorneyHearingTypeViewModel(JcasEntities db, int? AttorneyId)
        {
            RefreshLookups(db);
            if (AttorneyId.HasValue)
            {
                Attorney = db.JcasAttorneys.Find(AttorneyId);
                // get a comma delimited list of the id's that identify the hearing types this attorney has.
                AttorneyHearingTypes = string.Join(",", db.JcasAttorneyHearingTypes.Where(c => c.AttorneyId == AttorneyId.Value).Select(c => c.HearingTypeId.ToString()).ToList());
            }
            else
                Attorney = new JcasAttorney();            
        }

        /// <summary>Refresh the hearing types list.</summary>
        /// <param name="db">Database context object that represents the database for this application.</param>
        public void RefreshLookups(JcasEntities db)
        {
            // add the HearingTypes from the database to the dictionary used to populate the list.
            HearingTypeLookup = db.JcasHearingTypes.OrderBy(a => a.HearingTypeCode).ThenBy(a => a.HearingTypeName).ToDictionary(a => a.Id.ToString(), a => a.HearingTypeName, StringComparer.OrdinalIgnoreCase);

        }
        /// <summary>Updates the HearingTypes that this Attorney has.</summary>
        /// <param name="db">Database Context object that represents the database for this application.</param>
        /// <param name="modelState">Object that represents the state of model binding to a property.</param>
        public void UpdateAttorneyHearingTypes(JcasEntities db, ModelStateDictionary modelState)
        {
            // if there are no validation errors...
            if (modelState.IsValid)
            {
                // for each AttorneyHearingType entry already in the database for this Attorney...
                foreach (var attorneyHearingType in db.JcasAttorneyHearingTypes.Where(c => c.AttorneyId == Attorney.Id))
                {
                    // if the AttorneyHearingType entry in the database is not in the list of AttorneyHearingTypes in the model...
                    if (string.IsNullOrEmpty(AttorneyHearingTypes) || !("," + AttorneyHearingTypes + ",").Contains("," + attorneyHearingType.HearingTypeId.ToString() + ","))
                    {
                        // mark the custody entry for removal from the database.
                        db.JcasAttorneyHearingTypes.Remove(attorneyHearingType);
                    }
                }
                // remove any AttorneyHearingType entries marked above.
                db.SaveChanges();
            }
            // if there are no validation errors or errors removing AttorneyHearingType entries from the database above
            // and the list of AttorneyHearingTypes in the model isn't empty...
            if (modelState.IsValid && !string.IsNullOrEmpty(AttorneyHearingTypes))
            {
                // for each HearingType in the list of HearingTypes for the Attorney in the model...
                foreach (string hearingtypeId in AttorneyHearingTypes.Split(','))
                {
                    // check to see if the AttorneyHearingType entry for this HearingType is already in the database for this Attorney
                    int intHearingTypeId = Convert.ToInt32(hearingtypeId);
                    JcasAttorneyHearingType attorneyHearingType = db.JcasAttorneyHearingTypes.Where(c => c.AttorneyId == Attorney.Id && c.HearingTypeId == intHearingTypeId).FirstOrDefault();
                    // if the AttorneyHearingType entry is not already in the database...
                    if (attorneyHearingType == null)
                    {
                        // add the new AttorneyHearingType entry to the database.
                        attorneyHearingType = new JcasAttorneyHearingType();
                        attorneyHearingType.AttorneyId = Attorney.Id;
                        attorneyHearingType.HearingTypeId = intHearingTypeId;
                        attorneyHearingType.UserLoginIdUpdate = HttpContext.Current.User.Identity.Name.ToString();
                        db.JcasAttorneyHearingTypes.Add(attorneyHearingType);
                        db.SaveChanges(modelState);
                    }
                }
            }
        }
    }
}