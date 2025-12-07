using Microsoft.AspNetCore.Mvc.ModelBinding;

public class SafeNullableDateTimeBinder : IModelBinder
{
    public Task BindModelAsync(ModelBindingContext context)
    {
        var value = context.ValueProvider.GetValue(context.ModelName).FirstValue;

        if (string.IsNullOrWhiteSpace(value))
        {
            context.Result = ModelBindingResult.Success(null);
            return Task.CompletedTask;
        }

        if (DateTime.TryParse(value, out var dt))
        {
            context.Result = ModelBindingResult.Success(dt);
            return Task.CompletedTask;
        }

        context.Result = ModelBindingResult.Success(null);
        return Task.CompletedTask;
    }
}
