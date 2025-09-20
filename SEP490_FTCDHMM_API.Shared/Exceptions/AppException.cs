namespace SEP490_FTCDHMM_API.Shared.Exceptions
{
    public class AppException : Exception
    {
        public AppResponseCode ResponseCode { get; }

        public AppException(AppResponseCode code) : base(code.Message)
        {
            ResponseCode = code;
        }

        public AppException(AppResponseCode code, string? customMessage) : base(customMessage ?? code.Message)
        {
            ResponseCode = new AppResponseCode(code.Status, customMessage ?? code.Message);
        }
    }
}
