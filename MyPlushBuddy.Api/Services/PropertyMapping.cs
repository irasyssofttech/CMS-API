using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyPlushBuddy.Api.Services
{
    public class PropertyMapping<TSource, TDesination>: IPropertyMapping
    {
        public Dictionary<string, PropertyMappingValue> _mappingDictionary { get; private set; }
        public PropertyMapping(Dictionary<string, PropertyMappingValue> mappingDictionary)
        {
            _mappingDictionary = mappingDictionary ??
                throw new ArgumentNullException(nameof(mappingDictionary));

        }
    }
}
