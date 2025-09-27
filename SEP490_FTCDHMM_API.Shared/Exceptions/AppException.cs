namespace SEP490_FTCDHMM_API.Shared.Exceptions
{
    public class AppException : Exception
    {
        public AppResponseCode ResponseCode { get; }

        public AppException(AppResponseCode code)
        {
            ResponseCode = new AppResponseCode(code.StatusCode, code.Code);
        }
    }
}
