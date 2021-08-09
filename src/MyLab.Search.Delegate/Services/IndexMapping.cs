using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace MyLab.Search.Delegate.Services
{
    class IndexMapping
    {
        public IReadOnlyCollection<IndexMappingProperty> Props { get; }

        public IndexMapping(IEnumerable<IndexMappingProperty> props)
        {
            Props = new ReadOnlyCollection<IndexMappingProperty>(props.ToArray());
        }
    }
}