using System;
using System.Collections.Generic;
using System.Linq;
using FairfaxCounty.JCAS_Public_MVC.Models;
using System.Text;
using System.Net.Mail;
using System.Web.Configuration;
using System.Web.Mvc;
using System.IO;
using System.Web;
using System.Web.Routing;
using System.Web.Mvc.Html;

namespace FairfaxCounty.JCAS_Public_MVC.Helpers
{
    /// <summary>Utility helpers to use within the application.</summary>
    public class Utility
    {
        /// <summary>Gets value of the specified SettingName from table Setting.</summary>
        /// <param name="settingName">Identifies the setting name to get the value.</param>
        public static string GetSettingValue(string settingName)
        {
            if (!string.IsNullOrEmpty(settingName))
            {
                JcasEntities db = new JcasEntities();
                return db.JcasSettings.Where(w => w.SettingName == settingName).Select(s => s.SettingValue).FirstOrDefault().ToString();
            }
            else
            {
                return string.Empty;
            }
        }

        /// <summary>Generates a random password of the specified length.</summary>
        /// <param name="length">Length of the password to generate.</param>
        /// <returns>Random password of the specified length.</returns>
        public static string RandomCode(int length)
        {
            Random objRand = new Random();
            char[] arrAllowableCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLOMNOPQRSTUVWXYZ0123456789".ToCharArray();
            string randomCode = string.Empty;
            // for each number between 0 and the specified length...
            for (int intI = 0; intI < length; intI++)
            {
                // randomly select a character from the allowable characters and add it to the return value.
                randomCode = randomCode + arrAllowableCharacters[objRand.Next(arrAllowableCharacters.Length - 1)];
            }
            return randomCode;
        }

        /// <summary>Sends the email registration receipt to an external user's email address.</summary>
        /// <param name="db">Database Context object that represents the database for this application.</param>
        /// <param name="userEmailAddress">Identifies the email address to send.</param>
        public static void EmailRegistrationReceipt(JcasEntities db, string userEmailAddress)
        {
            string from = GetSettingValue("JcasFromEmailAddress");
            string distribution = userEmailAddress;
            string subject = GetSettingValue("EmailRegistrationReceiptSubject");
            string body = GetSettingValue("EmailRegistrationReceiptBody");
            SendEmail(from, distribution, subject, body, null, true);
        }

        /// <summary>Sends the email registration approval needed to all SysAdmin users.</summary>
        /// <param name="db">Database Context object that represents the database for this application.</param>
        /// <param name="attorneyId">Identifies the attorney registered.</param>
        public static void EmailRegistrationApprovalNeeded(JcasEntities db, int attorneyId)
        {
            string from = GetSettingValue("JcasFromEmailAddress");
            string distribution = string.Join(",", db.JcasUsers.Where(u => u.UserRole == "SysAdmin").Select(u => u.UserLoginId + "@fairfaxcounty.gov"));
            string subject = GetSettingValue("EmailRegistrationApprovalNeededSubject");
            string body = GetSettingValue("EmailRegistrationApprovalNeededBody");
            body = body.Replace("{AttorneyId}", attorneyId.ToString());
            SendEmail(from, distribution, subject, body, null, true);
        }

        /// <summary>Sends the email confirmation code link to an external user's email address to allow them to select a password.</summary>
        /// <param name="db">Database Context object that represents the database for this application.</param>
        /// <param name="userEmailAddress">Identifies the email address to send.</param>
        /// <param name="emailConfirmationCode">Identifies the confirmation code to send.</param>
        public static void EmailConfirmationCode(JcasEntities db, string userEmailAddress, string emailConfirmationCode)
        {
            string from = GetSettingValue("JcasFromEmailAddress");
            string distribution = userEmailAddress;
            string subject = GetSettingValue("EmailConfirmationCodeSubject");
            string body = GetSettingValue("EmailConfirmationCodeBody");
            body = body.Replace("{EmailConfirmationCode}", emailConfirmationCode);
            SendEmail(from, distribution, subject, body, null, true);
        }

        /// <summary>Sends the email to inform the attorney of the scheduled session.</summary>
        /// <param name="db">Database Context object that represents the database for this application.</param>
        /// <param name="userEmailAddress">Identifies the email address to send.</param>
        /// <param name="sessionInfo">Information about the scheduled session.</param>
        /// <param name="sessionDate">Date of the scheduled session.</param>
        public static void EmailSessionScheduled(JcasEntities db, string userEmailAddress, string sessionInfo, DateTime sessionDate)
        {
            //setup the ics attachment. 
            string appRegion = Constants.AppRegion != "Production" ? Constants.AppRegion : string.Empty;
            byte[] icsBytes = Encoding.UTF8.GetBytes(BuildOutlookFile(sessionDate, 
                sessionDate.AddDays(1), 
                sessionInfo, 
                "Fairfax County JDR" + appRegion));
            MemoryStream stream = new MemoryStream(icsBytes);
            Attachment attachment = new Attachment(stream, "JcasSession" + appRegion + ".ics");
            List<Attachment> attachments = new List<Attachment> { attachment };
            //setup and send the email
            string from = GetSettingValue("JcasFromEmailAddress");
            string distribution = userEmailAddress;
            string subject = GetSettingValue("EmailSessionScheduledSubject");
            string body = GetSettingValue("EmailSessionScheduledBody");
            body = body.Replace("{SessionInfo}", sessionInfo);
            SendEmail(from, distribution, subject, body, null, true, attachments);
            stream.Dispose();
        }

        /// <summary>Sends the email to inform the attorney of the cancellation of the scheduled session.</summary>
        /// <param name="db">Database Context object that represents the database for this application.</param>
        /// <param name="userEmailAddress">Identifies the email address to send.</param>
        /// <param name="sessionInfo">Information about the cancellation of the scheduled session.</param>
        public static void EmailSessionUnscheduled(JcasEntities db, string userEmailAddress, string sessionInfo)
        {
            string from = GetSettingValue("JcasFromEmailAddress");
            string distribution = userEmailAddress;
            string subject = GetSettingValue("EmailSessionUnscheduledSubject");
            string body = GetSettingValue("EmailSessionUnscheduledBody");
            body = body.Replace("{SessionInfo}", sessionInfo);
            SendEmail(from, distribution, subject, body, null, true);
        }
        
        /// <summary>Sends the email to inform the attorney of the updated session.</summary>
        /// <param name="db">Database Context object that represents the database for this application.</param>
        /// <param name="userEmailAddress">Identifies the email address to send.</param>
        /// <param name="sessionInfo">Information about the updated session.</param>
        public static void EmailSessionUpdated(JcasEntities db, string userEmailAddress, string sessionInfo)
        {
            string from = GetSettingValue("JcasFromEmailAddress");
            string distribution = userEmailAddress;
            string subject = GetSettingValue("EmailSessionUpdatedSubject");
            string body = GetSettingValue("EmailSessionUpdatedBody");
            body = body.Replace("{SessionInfo}", sessionInfo);
            SendEmail(from, distribution, subject, body, null, true);
        }

        /// <summary>Sends the email to inform the attorney of the deleted session.</summary>
        /// <param name="db">Database Context object that represents the database for this application.</param>
        /// <param name="userEmailAddress">Identifies the email address to send.</param>
        /// <param name="sessionInfo">Information about the deleted session.</param>
        public static void EmailSessionDeleted(JcasEntities db, string userEmailAddress, string sessionInfo)
        {
            string from = GetSettingValue("JcasFromEmailAddress");
            string distribution = userEmailAddress;
            string subject = GetSettingValue("EmailSessionDeletedSubject");
            string body = GetSettingValue("EmailSessionDeletedBody");
            body = body.Replace("{SessionInfo}", sessionInfo);
            SendEmail(from, distribution, subject, body, null, true);
        }

        /// <summary>Sends the forgot password email to an external user's email address to allow them to select a new password.</summary>
        /// <param name="db">Database Context object that represents the database for this application.</param>
        /// <param name="userEmailAddress">Identifies the email address of the user.</param>
        /// <param name="emailConfirmationCode">Confirmation code being emailed to the user.</param>
        public static void ForgotPassword(JcasEntities db, string userEmailAddress, string emailConfirmationCode)
        {
            string from = GetSettingValue("JcasFromEmailAddress");
            string distribution = userEmailAddress;
            string subject = GetSettingValue("ForgotPasswordSubject");
            string body = GetSettingValue("ForgotPasswordBody");
            body = body.Replace("{EmailConfirmationCode}", emailConfirmationCode);
            SendEmail(from, distribution, subject, body, null, true);
        }

        /// <summary>Sends the specified email.</summary>
        /// <param name="from">Email address the email is sent from.</param>
        /// <param name="distribution">Email addresses the email is sent to.</param>
        /// <param name="subject">Subject line of the email.</param>
        /// <param name="body">Message to be included in the body of the email.</param>
        /// <param name="cc">Email address that is cc'ed on the email.</param>
        /// <param name="isBodyHtml">Whether or not the body of the email is in HTML format.</param>
        /// <param name="attachments">Documents to attach to the email.</param>
        public static void SendEmail(string from,
            string distribution,
            string subject,
            string body,
            string cc = null,
            bool isBodyHtml = true,
            List<Attachment> attachments = null)
        {
            MailAddress objFrom = new MailAddress(from);
            MailAddress objcc = null;
            if (!string.IsNullOrEmpty(cc))
            {
                objcc = new MailAddress(cc);
            }
            MailAddressCollection objDistribution = new MailAddressCollection();
            foreach (string address in distribution.Replace(",", ";").Split(';'))
            {
                if (!string.IsNullOrEmpty(address))
                {
                    objDistribution.Add(address);
                }
            }
            SendEmail(objFrom, objDistribution, subject, body, objcc, isBodyHtml, attachments);
        }

        /// <summary>Sends the specified email.</summary>
        /// <param name="fromEmail">Email address the email is sent from.</param>
        /// <param name="distribution">Email addresses the email is sent to.</param>
        /// <param name="emailSubject">Subject line of the email.</param>
        /// <param name="emailBody">Message to be included in the body of the email.</param>
        /// <param name="ccEmail">Email address that is cc'ed on the email.</param>
        /// <param name="bodyIsHtml">Whether or not the body of the email is in HTML format.</param>
        /// <param name="emailAttachments">Documents to attach to the email.</param>
        public static void SendEmail(MailAddress fromEmail, MailAddressCollection distribution, string emailSubject, string emailBody,
        MailAddress ccEmail = null, bool bodyIsHtml = true, List<Attachment> emailAttachments = null)
        {
            //Attachment emailAttachment = new Attachment;
            if (distribution.Count > 0)
            {
                MailMessage mailMessage = new MailMessage();
                mailMessage.IsBodyHtml = Convert.ToBoolean(bodyIsHtml);
                MailAddressCollection testAddress = TestEmailAddress();
                if (testAddress.Count > 0)
                {
                    emailBody = TestVersionHeader(distribution, fromEmail, ccEmail) + System.Environment.NewLine + emailBody;
                    for (int i = 0; i < testAddress.Count; i++)
                    {
                        mailMessage.To.Insert(i, testAddress.ElementAt(i));
                    }
                    mailMessage.From = testAddress.FirstOrDefault();
                }
                else
                {
                    for (int i = 0; i < distribution.Count; i++)
                    {
                        mailMessage.To.Insert(i, distribution.ElementAt(i));
                    }
                    mailMessage.From = fromEmail;
                    if (ccEmail != null)
                    {
                        mailMessage.CC.Insert(0, ccEmail);
                    }
                }
                if (bodyIsHtml == true)
                {
                    emailBody = emailBody.Replace(System.Environment.NewLine, "<br />");
                    emailBody = emailBody.Replace("\r", "<br />");
                    emailBody = emailBody.Replace("&lt;br /&gt;", "<br />");
                }
                mailMessage.Subject = emailSubject;
                mailMessage.Body = emailBody;
                if (emailAttachments != null)
                {
                    foreach (Attachment att in emailAttachments)
                    {
                        mailMessage.Attachments.Add(att);
                    }
                }
                using (var smtpMail = new SmtpClient())
                {
                    smtpMail.UseDefaultCredentials = true;
                    smtpMail.DeliveryMethod = SmtpDeliveryMethod.Network;
                    smtpMail.EnableSsl = false;
                    smtpMail.Host = WebConfigurationManager.AppSettings["smtpServer"];
                    smtpMail.Send(mailMessage);
                }
            }
        }
        
        /// <summary>Address that all the test email is sent to.</summary>
        public static MailAddressCollection TestEmailAddress()
        {
            MailAddressCollection distribution = new MailAddressCollection();
            string testEmailAddress = GetSettingValue("TestEmailAddress");
            foreach (string itm in testEmailAddress.Replace(';', ',').Split(','))
            {
                if (!string.IsNullOrEmpty(itm))
                {
                    distribution.Add(itm);
                }
            }
            return distribution;
        }
        
        /// <summary>Builds the header that appears in emails sent by non-production versions.</summary>
        /// <param name="distribution">To addresses for the email.</param>
        /// <param name="fromEmail">From address for the email.</param>
        /// <param name="ccEmail">Cc address for the email.</param>
        /// <returns>Header that appears in emails sent by non-production versions.</returns>
        public static string TestVersionHeader(MailAddressCollection distribution, MailAddress fromEmail, MailAddress ccEmail)
        {
            StringBuilder headerString = new StringBuilder();
            headerString.Append("*** JCAS TEST VERSION ***");
            headerString.AppendLine("In the production version of JCAS, this email would have been addressed as follows:");
            headerString.Append("To: ");
            for (int i = 0; i < distribution.Count; i++)
            {
                if (i > 0)
                {
                    headerString.Append("; ");
                }
                headerString.Append(distribution.ElementAt(i).Address);
            }
            headerString.AppendLine();
            if (fromEmail != null)
            {
                headerString.Append("From: ");
                headerString.AppendLine(fromEmail.Address);
            }
            if (ccEmail != null)
            {
                headerString.Append("cc: ");
                headerString.AppendLine(ccEmail.Address);
                headerString.AppendLine();
            }

            headerString.AppendLine("************************");
            return headerString.ToString();
        }
        /// <summary>Build a calendar event to attach to an email.</summary>
        /// <param name="eventStart">Date and time the event starts.</param>
        /// <param name="eventEnd">Date and time the event ends.</param>
        /// <param name="eventName">Name/title of the event.</param>
        /// <param name="eventLocation">Location of the event.</param>
        /// <returns>String of the calendar event.</returns>
        private static string BuildOutlookFile(DateTime eventStart, DateTime eventEnd, string eventName, string eventLocation)
        {
            //string dateFormat = "yyyyMMddTHHmmss";
            string dateFormat = "yyyyMMdd";
            var dateCreated = DateTime.Now;
            var icsFile = new StringBuilder();

            icsFile.AppendLine("BEGIN:VCALENDAR");
            icsFile.AppendLine("METHOD:PUBLISH");
            icsFile.AppendLine("VERSION:2.0");

            icsFile.AppendLine("BEGIN:VTIMEZONE");
            icsFile.AppendLine("TZID:America/New_York");
            icsFile.AppendLine("END:VTIMEZONE");

            // Define the event.
            icsFile.AppendLine("BEGIN:VEVENT");
            //icsFile.AppendLine("PRIORITY:3");
            //icsFile.AppendLine("UID:" + Guid.NewGuid());

            // Adding the event datetimes
            icsFile.AppendLine(string.Format("DTSTAMP:{0}", dateCreated.ToString(dateFormat)));
            icsFile.AppendLine(string.Format("DTSTART:{0}", eventStart.ToString(dateFormat)));
            icsFile.AppendLine(string.Format("DTEND:{0}", eventEnd.ToString(dateFormat)));
            
            // Adding the summary and content
            icsFile.AppendLine("SUMMARY: Fairfax County JDR Court - Confirmed Attorney or GAL of the day");
            icsFile.AppendLine(string.Format("LOCATION:{0}", eventLocation));
            icsFile.AppendLine(string.Format("DESCRIPTION:{0}", eventName));

            // ADD ALARM
            icsFile.AppendLine("BEGIN:VALARM");
            icsFile.AppendLine("TRIGGER:-PT12H");
            icsFile.AppendLine("ACTION:DISPLAY");
            icsFile.AppendLine("DESCRIPTION:Reminder");
            icsFile.AppendLine("END:VALARM");

            // END EVENT
            icsFile.AppendLine("END:VEVENT");
            icsFile.AppendLine("END:VCALENDAR");

            return icsFile.ToString();
        }
    }
}