using MyPlushBuddy.Api.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;

namespace MyPlushBuddy.Api.Helpers
{
    public static class IQueryableExtensions
    {
        public static IQueryable<T> ApplySort<T>(this IQueryable<T> source, string orderBy,
            Dictionary<string, PropertyMappingValue> mappingDictionary)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (mappingDictionary == null)
            {
                throw new ArgumentNullException(nameof(mappingDictionary));
            }

            if(string.IsNullOrWhiteSpace(orderBy))
            {
                return source;
            }

            var orderByString = string.Empty;

            // the orderBy string is seperated by ",", so we split it.
            var orderByAfterSplit = orderBy.Split(',');

            // apply each orderby clause in reverse order - otherwise, the 
            // IQueryable will be ordered in the wrong order
            foreach(var orderByClause in orderByAfterSplit.Reverse())
            {
                // trim the orderBy clasue, as it might contain leading
                // or trailing space. Can't trimg the var in foreach,
                // so use another var
                var trimmedOrderByClause = orderByClause.Trim();

                // if the sort option ends with "desc", we order
                // descending, otherwise ascending
                var orderDescending = trimmedOrderByClause.EndsWith(" desc");

                // remove " asc" or " desc" from the orderByClause, so we
                // get the property name to look for in the mapping dictionary
                var indexOfFirstSpace = trimmedOrderByClause.IndexOf(" ");
                var propertyName = indexOfFirstSpace == -1 ?
                    trimmedOrderByClause : trimmedOrderByClause.Remove(indexOfFirstSpace);

                if(!mappingDictionary.ContainsKey(propertyName))
                {
                    throw new ArgumentException($"Key mappng for {propertyName} is missing");
                }

                // get the PropertyMappingValue
                var propertyMappingValue = mappingDictionary[propertyName];

                if (propertyMappingValue == null)
                {
                    throw new ArgumentNullException("propertyMappingValue");
                }

                // Run through the property names
                // so the orderby clauses are applied in the correct order
                foreach(var destinationProperty in 
                    propertyMappingValue.DestinationProperties)
                {
                    // revert sort order if necessary
                    if (propertyMappingValue.Revert)
                    {
                        orderDescending = !orderDescending;
                    }

                    orderByString = orderByString +
                        (string.IsNullOrWhiteSpace(orderByString) ? string.Empty : ", ")
                        + destinationProperty
                        + (orderDescending ? " descending" : " ascending");
                }

                
            }
            return source.OrderBy(orderByString);
        }
    }
}
