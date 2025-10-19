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
        var imageProp = src?.GetType().GetProperty("Image")
                      ?? src?.GetType().GetProperty("Avatar");

        if (imageProp == null)
            return null;

        var imageObj = imageProp.GetValue(src) as Image;
        var key = imageObj?.Key;
        return _s3.GeneratePreSignedUrl(key);
    }
}
