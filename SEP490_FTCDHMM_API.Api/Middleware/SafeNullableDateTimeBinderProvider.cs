using Microsoft.AspNetCore.Mvc.ModelBinding;

public class SafeNullableDateTimeBinderProvider : IModelBinderProvider
{
    public IModelBinder GetBinder(ModelBinderProviderContext context)
    {
        if (context.Metadata.ModelType == typeof(DateTime?) ||
            context.Metadata.ModelType == typeof(DateTime))
        {
            return new SafeNullableDateTimeBinder();
        }

        return null!;
    }
}
