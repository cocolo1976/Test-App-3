//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace FairfaxCounty.JCAS_Public_MVC.Models
{
    using System;
    
    public partial class pJcasSessionGetCalendar_Result3
    {
        public Nullable<System.DateTime> CalendarDate { get; set; }
        public Nullable<int> CalendarWeek { get; set; }
        public Nullable<int> CalendarDay { get; set; }
        public Nullable<int> CalendarWeekday { get; set; }
        public string CalendarWeekdayName { get; set; }
        public string CourtClosed { get; set; }
        public string HearingType { get; set; }
        public Nullable<int> SessionId { get; set; }
        public Nullable<int> AttorneyId { get; set; }
        public string AttorneyName { get; set; }
        public string Courtroom { get; set; }
    }
}