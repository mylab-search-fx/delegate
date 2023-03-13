using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;
using Nest;

namespace MyLab.Search.Searcher.Tools
{
    static class MappingPropertyTools
    {
        private static readonly object PropCacheSync = new ();
        private static readonly Dictionary<string, PropertyInfo> PropertyCache = new();

        public static bool IsIndexed(IProperty property)
        {
            if (property == null) throw new ArgumentNullException(nameof(property));

            var propType = property.GetType();

            if (propType.FullName == null) throw new InvalidOperationException("Property reflection type name is null");

            PropertyInfo indexPropInfo;

            lock (PropCacheSync)
            {
                if (!PropertyCache.TryGetValue(propType.FullName, out indexPropInfo))
                {
                    indexPropInfo = propType.GetProperty("Index", BindingFlags.Instance | BindingFlags.Public);

                    bool goodProperty = indexPropInfo?.PropertyType == typeof(bool) || indexPropInfo?.PropertyType == typeof(bool?);

                    PropertyCache.Add(propType.FullName, 
                        goodProperty ? indexPropInfo : null
                    );
                }
            }

            if (indexPropInfo == null) return false;

            var propVal = indexPropInfo.GetValue(property);

            return propVal == null || (bool)propVal;
        }
    }
}
