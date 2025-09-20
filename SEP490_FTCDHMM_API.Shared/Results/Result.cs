namespace SEP490_FTCDHMM_API.Shared.Results
{
    public class Result
    {
        public bool Success { get; }
        public string Message { get; }

        protected Result(bool success, string message)
        {
            Success = success;
            Message = message;
        }

        public static Result OK(string message = "") => new(true, message);
        public static Result Failure(string message) => new(false, message);
    }
}
