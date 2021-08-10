using System.Collections.Generic;

namespace MyLab.Search.Delegate.Models
{
    public class TokenRequest
    {
        public  FiltersCall Filters { get; set; }
    }

    public class FiltersCall : Dictionary<string, FilterArgs>
    {
        
    }

    public class FilterArgs : Dictionary<string, string>
    {

    }
}