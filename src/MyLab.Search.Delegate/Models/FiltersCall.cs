using System;
using System.Collections.Generic;

#if IS_CLIENT
namespace MyLab.Search.Delegate.Client
#else
namespace MyLab.Search.Delegate.Models
#endif
{
    [Obsolete]
    public class FiltersCall : Dictionary<string, FilterArgs>
    {
        /// <summary>
        /// Initializes a new instance of <see cref="FiltersCall"/>
        /// </summary>
        public FiltersCall()
        {
            
        }

        /// <summary>
        /// Initializes a new instance of <see cref="FiltersCall"/>
        /// </summary>
        public FiltersCall(IEnumerable<KeyValuePair<string, FilterArgs>> initial) : base(initial)
        {
            
        }
    }

    [Obsolete]
    public class FilterArgs : Dictionary<string, string>
    {

    }
}