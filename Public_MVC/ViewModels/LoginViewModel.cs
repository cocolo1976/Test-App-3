using FairfaxCounty.JCAS_Public_MVC.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using FairfaxCounty.JCAS_Public_MVC.Helpers;
using System.Data.Entity;
using System.Web.Mvc;

namespace FairfaxCounty.JCAS_Public_MVC.ViewModels
{
    /// <summary>View Model used to login a User.</summary>
    public class LoginViewModel
    {
        /// <summary>If this is an internal user, the User Id that the employee uses to login to the FFX domain.  If this is an external user, the email address that uniquely identifies the user.</summary>
        [Required]
        [Display(Name = "User name *")]
        public string UserLoginId { get; set; }
        /// <summary>The password entered by the user.</summary>
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password *")]
        public string Password { get; set; }
        /// <summary>Included with county template.  Not used in this app.</summary>
        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }
        /// <summary>Message that appears on the login page (from the setting in the JcasSetting table).</summary>
        public string LoginMessage { get; set; }
        /// <summary>Any timed messages that are currently in effect (from the JcasMessage table).</summary>
        public string MessageText { get; set; }
        /// <summary>Whether the app should be available.  False if there is a timed outage currently in effect (from the JcasMessage table).</summary>
        public bool AppAvailable { get; set; }

        /// <summary>Refreshes the lookup lists of possible values used to populate dropdowns and gets items from settings.</summary>
        /// <param name="db">Database Context object that represents the database for this application.</param>
        public void RefreshLookups(JcasEntities db)
        {
            // get intro message from setting.
            LoginMessage = Utility.GetSettingValue("LoginMessage");
            MessageText = "";
            AppAvailable = true;
            foreach (JcasMessage message in db.JcasMessages.Where(m => m.StartDateTime <= DateTime.Now && m.EndDateTime >= DateTime.Now))
            {
                MessageText = MessageText + message.MessageText;
                if (!message.ApplicationAvailable)
                {
                    AppAvailable = false;
                }
            }
        }

        /// <summary>Email the monthly scheduling reminder, if needed.</summary>
        /// <param name="db">Database Context object that represents the database for this application.</param>
        /// <param name="modelState">Object to represent the model state of model binding to a property.</param>
        public void EmailSchedulingReminder(JcasEntities db, ModelStateDictionary modelState)
        {
            //if the scheduling reminder is turned on
            if (Convert.ToBoolean(Utility.GetSettingValue("EmailSchedulingReminderOn")) == true)
            {
                //get the setting value of the calendar date to email.  If not available, default to the 25th
                string calendarDate = Utility.GetSettingValue("EmailSchedulingReminderCalendarDate") ?? "25";
                //if the current date is on or after the calendar date to send email
                if (DateTime.Today.Day >= Convert.ToInt32(calendarDate))
                {
                    //get the last date the email was sent.  If not available, default to the 25th of the previous month
                    string dateSentString = Utility.GetSettingValue("EmailSchedulingReminderDateSent") ?? 
                        DateTime.Today.AddMonths(-1).ToString("MM/" + calendarDate + "/yyyy");
                    //if date sent was before the first day of this month,
                    if (DateTime.Parse(dateSentString) < DateTime.Parse(DateTime.Today.ToString("MM/01/yyyy")))
                    {
                        string from = Utility.GetSettingValue("JcasFromEmailAddress");
                        try
                        {
                            //send the email to all active attorneys except Public Defender
                            int pdAttorneyId = Convert.ToInt32(Utility.GetSettingValue("PDAttorneyId"));
                            string distribution = string.Join(",", db.JcasAttorneys.Where(a => a.IsActive == true
                                && a.Id != pdAttorneyId).Select(a => a.EmailAddress));
                            string subject = Utility.GetSettingValue("EmailSchedulingReminderSubject");
                            string body = Utility.GetSettingValue("EmailSchedulingReminderBody");
                            int sessionMonths = Convert.ToInt32(Utility.GetSettingValue("AttorneySessionMonths"));
                            body = body.Replace("{SessionMonth}", DateTime.Today.AddMonths(sessionMonths).ToString("MMMM yyyy"));
                            body = body.Replace("{DateBegin}", DateTime.Today.AddMonths(1).ToString("MM/01/yyyy"));
                            Utility.SendEmail(from, distribution, subject, body, null, true);
                            //update the date sent to current date
                            JcasSetting settingToUpdate = db.JcasSettings.Where(a => a.SettingName == "EmailSchedulingReminderDateSent").FirstOrDefault();
                            settingToUpdate.SettingValue = DateTime.Today.ToShortDateString();
                            db.Entry(settingToUpdate).State = EntityState.Modified;
                            db.SaveChanges(modelState);
                        }
                        //if error, email the support staff
                        catch (Exception ex)
                        {
                            Utility.SendEmail(from, 
                                Utility.GetSettingValue("JcasSupportStaff"),
                                Utility.GetSettingValue("EmailSchedulingReminderSubject") + " - Error",
                                ex.Message, 
                                null, 
                                true);
                        }
                    }
                }
            }
        }
        /// <summary>Creates a new instance of the model populated with the LoginMessage.</summary>
        /// <param name="db">Database Context object that represents the database for this application.</param>
        public LoginViewModel(JcasEntities db)
        {
            RefreshLookups(db);
        }

        /// <summary>Creates an empty instance of the model.</summary>
        public LoginViewModel()
        {
        }
    }
}