using Microsoft.Xrm.Sdk;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Pg.DataverseSync.Engine.Application
{
    internal static partial class DataverseValueConverter
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
                DateTimeOffset dateTimeOffset => dateTimeOffset.UtcDateTime,
                string stringValue when TryConvertSerializedDateTime(stringValue, out DateTime convertedDateTime) => convertedDateTime,
                _ => value,
            };
        }

        private static bool TryConvertSerializedDateTime(string value, out DateTime convertedDateTime)
        {
            if (TryConvertDataContractDateTime(value, out convertedDateTime))
            {
                return true;
            }

            if (TryConvertIsoDateTime(value, out convertedDateTime))
            {
                return true;
            }

            convertedDateTime = default;
            return false;
        }

        private static bool TryConvertDataContractDateTime(string value, out DateTime convertedDateTime)
        {
            var match = DataContractDateTimeRegex().Match(value);
            if (!match.Success)
            {
                convertedDateTime = default;
                return false;
            }

            var millisecondsSinceUnixEpoch = long.Parse(match.Groups[1].Value, CultureInfo.InvariantCulture);
            convertedDateTime = DateTimeOffset.FromUnixTimeMilliseconds(millisecondsSinceUnixEpoch).UtcDateTime;
            return true;
        }

        private static bool TryConvertIsoDateTime(string value, out DateTime convertedDateTime)
        {
            if (!value.Contains('T'))
            {
                convertedDateTime = default;
                return false;
            }

            if (!DateTimeOffset.TryParse(
                value,
                CultureInfo.InvariantCulture,
                DateTimeStyles.RoundtripKind | DateTimeStyles.AllowWhiteSpaces,
                out var parsedDateTimeOffset))
            {
                convertedDateTime = default;
                return false;
            }

            convertedDateTime = parsedDateTimeOffset.UtcDateTime;
            return true;
        }

        [GeneratedRegex(@"^\\?/Date\(([-+]?\d+)(?:[-+]\d{4})?\)\\?/$", RegexOptions.CultureInvariant)]
        private static partial Regex DataContractDateTimeRegex();
    }
}
