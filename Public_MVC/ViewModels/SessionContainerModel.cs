using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FairfaxCounty.JCAS_Public_MVC.ViewModels
{
    public class SessionContainerModel
    {
        public int Id { get; set; }
        public System.DateTime SessionDate { get; set; }
        public string SessionDateString { get; set; }
        public Nullable<int> AttorneyId { get; set; }
        public String AttorneyName { get; set; }
        public int HearingTypeId { get; set; }
        public string HearingTypeName { get; set; }
        public Nullable<int> CourtroomId { get; set; }
        public string Courtroom { get; set; }
        public int SessionRequestId { get; set; }
        public Nullable<System.DateTime> SubmittedDate { get; set; }
        public string UserLoginIdUpdate { get; set; }
        public byte[] RowVersionId { get; set; }
    }
}