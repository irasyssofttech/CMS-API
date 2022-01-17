using System.Collections.Generic;

namespace MyPlushBuddy.Api.Services
{
    public interface IPropertyMappingService
    {
        Dictionary<string, PropertyMappingValue> GetPropertyMapping<TSource, TDestination>();

        bool ValidMappingExistsFor<TSource, TDestination>(string fields);
    }
}