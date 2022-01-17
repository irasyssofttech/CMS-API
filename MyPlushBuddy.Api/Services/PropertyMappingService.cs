using MyPlushBuddy.Api.Entities;
using MyPlushBuddy.Api.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyPlushBuddy.Api.Services
{
    public class PropertyMappingService : IPropertyMappingService
    {
        private Dictionary<string, PropertyMappingValue> _pageTagPropertyMapping =
            new Dictionary<string, PropertyMappingValue>(StringComparer.OrdinalIgnoreCase)
            {
                { "PageTagId", new PropertyMappingValue(new List<string>() { "PageTagId" }) },
                { "PageRoute", new PropertyMappingValue(new List<string>() { "PageRoute" }) },
                { "Robots", new PropertyMappingValue(new List<string>() { "Robots" }) },
                { "PageTitle", new PropertyMappingValue(new List<string>() { "PageTitle" }) },
                { "OGTitle", new PropertyMappingValue(new List<string>() { "OGTitle" }) },
                { "OGDescription", new PropertyMappingValue(new List<string>() { "OGDescription" }) },
                { "PageKeywords", new PropertyMappingValue(new List<string>() { "PageKeywords" }) },
                { "MetaNameDescription", new PropertyMappingValue(new List<string>() { "MetaNameDescription" }) },
            };

        private IList<IPropertyMapping> _propertyMappings = new List<IPropertyMapping>();

        public PropertyMappingService()
        {
            _propertyMappings.Add(new PropertyMapping<PageTagViewModel, PageTag>(_pageTagPropertyMapping));
        }

        public bool ValidMappingExistsFor<TSource, TDestination>(string fields)
        {
            var propertyMapping = GetPropertyMapping<TSource, TDestination>();

            if (string.IsNullOrWhiteSpace(fields))
            {
                return true;
            }

            // the string is seperated by ",", so we split it.
            var fieldAfterSplit = fields.Split(',');

            // run through the fields clauses
            foreach(var field in fieldAfterSplit)
            {
                // trim
                var trimmedField = field.Trim();

                // remove everythin after the first " " - if the fields
                // are coming from an orderBy string, this part must be
                // ignored
                var indexOfFirstSpace = trimmedField.IndexOf(" ");
                var propertyName = indexOfFirstSpace == -1 ?
                    trimmedField : trimmedField.Remove(indexOfFirstSpace);

                // find the matching property
                if (!propertyMapping.ContainsKey(propertyName))
                {
                    return false;
                }
            }
            return true;
        }

        public Dictionary<string, PropertyMappingValue> GetPropertyMapping
            <TSource, TDestination>()
        {
            // get matching mapping
            var matchingMapping = _propertyMappings
                .OfType<PropertyMapping<TSource, TDestination>>();

            if (matchingMapping.Count() == 1)
            {
                return matchingMapping.First()._mappingDictionary;
            }

            throw new Exception($"Cantnot find exact property mapping instance" +
                $"for {typeof(TSource)}, {typeof(TDestination)}");
        }
    }
}
