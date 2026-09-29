using Microsoft.Xrm.Sdk;

namespace Pg.DataverseSync.Engine.Application
{
    internal static class DataverseValueConverter
    {
        public static object? Convert(object? value)
        {
            return value switch
            {
                null => null,
                AliasedValue aliasedValue => Convert(aliasedValue.Value),
                OptionSetValue optionSetValue => optionSetValue.Value,
                OptionSetValueCollection optionSetValues => string.Join(",", optionSetValues.Select(optionSetValue => optionSetValue.Value)),
                Money money => money.Value,
                EntityReference entityReference => entityReference.Id,
                BooleanManagedProperty booleanManagedProperty => booleanManagedProperty.Value,
                _ => value,
            };
        }
    }
}
