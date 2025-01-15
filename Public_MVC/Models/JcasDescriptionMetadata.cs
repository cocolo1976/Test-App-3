using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlClient;

namespace FairfaxCounty.JCAS_Public_MVC.Models
{
    /// <summary>Extension of the JcasDescription class from the Entity Data Model.</summary>
    [MetadataType(typeof(JcasDescriptionMetadata))]
    public partial class JcasDescription
    {
        /// <summary>If a custom SQL exception was thrown, gets the message from the exception.</summary>
        /// <param name="db">Entity to represent the database.</param>
        /// <param name="exception">The custon SQL exception to get the message from.</param>
        /// <returns>The message from the custom SQL exception.</returns>
        public static string GetDatabaseMessage(JcasEntities db, Exception exception)
        {
            SqlException objSqlException;
            string strReturn = string.Empty;
            Exception expCurrent;
            expCurrent = exception;
            while (expCurrent != null && !(expCurrent is SqlException))
            {
                expCurrent = expCurrent.InnerException;
            }
            if (expCurrent != null)
            {
                if (expCurrent is SqlException)
                {
                    objSqlException = (SqlException)expCurrent;
                    if (objSqlException.Number >= 50000)
                    {
                        strReturn = objSqlException.Message;
                    }
                    else
                    {
                        string[] arrConstraint = objSqlException.Message.Replace("\"", "'").Split('\'');
                        string strConstraint = db.JcasDescriptions.Where(d => arrConstraint.Contains(d.Id)).Select(d => d.DescriptionText).FirstOrDefault();
                        if (!string.IsNullOrEmpty(strConstraint))
                        {
                            strReturn = strConstraint;
                        }
                    }
                }
            }
            return strReturn;
        }
    }
    /// <summary>Metadata for the JcasDescription class.</summary>
    public class JcasDescriptionMetadata
    {
    }
}