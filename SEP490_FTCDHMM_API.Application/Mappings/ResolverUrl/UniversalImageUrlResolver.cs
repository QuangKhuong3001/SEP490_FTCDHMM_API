using AutoMapper;
using SEP490_FTCDHMM_API.Application.Interfaces.ExternalServices;
using SEP490_FTCDHMM_API.Domain.Entities;

public class UniversalImageUrlResolver<TSource, TDestination>
    : IValueResolver<TSource, TDestination, string?>
{
    private readonly IS3ImageService _s3;

    public UniversalImageUrlResolver(IS3ImageService s3)
    {
        _s3 = s3;
    }

    public string? Resolve(TSource src, TDestination dest, string? destMember, ResolutionContext context)
    {
        if (src == null)
            return null;

        var imageProp = src.GetType().GetProperty("Image")
                      ?? src.GetType().GetProperty("Avatar");

        if (imageProp == null)
        {
            // Try to get Avatar through User property (for Comment entity)
            var userProp = src.GetType().GetProperty("User");
            if (userProp != null)
            {
                var userObj = userProp.GetValue(src);
                if (userObj != null)
                {
                    imageProp = userObj.GetType().GetProperty("Avatar");
                    if (imageProp != null)
                    {
                        var imageObj = imageProp.GetValue(userObj) as Image;
                        if (imageObj == null)
                            return null;
                        var key = imageObj.Key;
                        return _s3.GeneratePreSignedUrl(key);
                    }
                }
            }
            return null;
        }

        var image = imageProp.GetValue(src) as Image;
        if (image == null)
            return null;
        var imageKey = image.Key;
        return _s3.GeneratePreSignedUrl(imageKey);
    }
}
