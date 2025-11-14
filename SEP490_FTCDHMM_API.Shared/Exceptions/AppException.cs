namespace SEP490_FTCDHMM_API.Shared.Exceptions
{
    public class AppException : Exception
    {
        public AppResponseCode ResponseCode { get; }

        public AppException(AppResponseCode responseCode)
            : base(responseCode.Message)
        {
            ResponseCode = responseCode;
        }

        public AppException(AppResponseCode responseCode, string message)
        : base(message)
        {
            ResponseCode = responseCode;
        }
    }
}
