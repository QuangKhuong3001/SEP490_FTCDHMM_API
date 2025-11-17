using AutoMapper;
using SEP490_FTCDHMM_API.Application.Dtos.Common;

public class FormFileToUploadConverter
    : ITypeConverter<IFormFile, FileUploadModel?>
{
    public FileUploadModel? Convert(IFormFile src, FileUploadModel? dest, ResolutionContext ctx)
    {
        if (src == null)
            return null;

        return new FileUploadModel
        {
            FileName = src.FileName,
            Content = src.OpenReadStream(),
            ContentType = src.ContentType
        };
    }
}
