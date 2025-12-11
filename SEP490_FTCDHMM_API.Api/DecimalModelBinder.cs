using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Globalization;

namespace SEP490_FTCDHMM_API.Api.ModelBinders
{
    public class DecimalModelBinder : IModelBinder
    {
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            if (bindingContext == null)
            {
                throw new ArgumentNullException(nameof(bindingContext));
            }

            var modelName = bindingContext.ModelName;
            var valueProviderResult = bindingContext.ValueProvider.GetValue(modelName);

            if (valueProviderResult == ValueProviderResult.None)
            {
                return Task.CompletedTask;
            }

            bindingContext.ModelState.SetModelValue(modelName, valueProviderResult);

            var value = valueProviderResult.FirstValue;

            if (string.IsNullOrEmpty(value))
            {
                return Task.CompletedTask;
            }

            // Try to parse with invariant culture (uses period as decimal separator)
            if (decimal.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out var result))
            {
                bindingContext.Result = ModelBindingResult.Success(result);
                return Task.CompletedTask;
            }

            // If invariant culture fails, try current culture
            if (decimal.TryParse(value, NumberStyles.Any, CultureInfo.CurrentCulture, out result))
            {
                bindingContext.Result = ModelBindingResult.Success(result);
                return Task.CompletedTask;
            }

            bindingContext.ModelState.TryAddModelError(
                modelName,
                $"The value '{value}' is not a valid decimal."
            );

            return Task.CompletedTask;
        }
    }
}
