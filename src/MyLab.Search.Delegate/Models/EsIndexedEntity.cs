using System.Collections.Generic;

namespace MyLab.Search.Delegate.Models
{
    public class EsIndexedEntity 
    {
        public EsIndexedEntityContent Content { get; set; }
        public double? Score { get; set; }
    }

    public class EsIndexedEntityContent : Dictionary<string, object>
    {

    }
}
