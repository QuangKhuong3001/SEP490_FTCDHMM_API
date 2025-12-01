namespace SEP490_FTCDHMM_API.Application.Interfaces.ExternalServices
{
    public interface ITranslateService
    {
        Task<string> TranslateToEnglishAsync(string vietnamese);
        Task<string> TranslateToVietnameseAsync(string english);
    }

}
