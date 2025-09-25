using System.ComponentModel.DataAnnotations;

namespace SEP490_FTCDHMM_API.Api.Attributes
{
    public class AllowedValuesAttribute : ValidationAttribute
    {
        private readonly object[] _allowedValues;

        public AllowedValuesAttribute(object[] allowedValues)
        {
            _allowedValues = allowedValues;
        }

        public override bool IsValid(object? value)
        {
            return value != null && _allowedValues.Contains(value);
        }
    }
}
