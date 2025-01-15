using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
namespace FairfaxCounty.JCAS_Public_MVC.ViewModels
{
    /// <summary>View Model for the partial view that allows editing of comma delimited lists.</summary>
    public class CommaDelimitedListViewModel
    {
        /// <summary>Name of the field used to store the comma delimited list.</summary>
        public string FieldName { get; set; }
        /// <summary>Current value of the comma delimited list.</summary>
        public string CommaDelimitedList { get; set; }
        /// <summary>Lookup for the values that may be added to the list.</summary>
        public Dictionary<string, string> ListLookup { get; set; }
        /// <summary>Creates a new empty instance of the model.</summary>
        public CommaDelimitedListViewModel()
        {
        }
    }
}
