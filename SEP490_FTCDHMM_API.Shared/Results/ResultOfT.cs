namespace SEP490_FTCDHMM_API.Shared.Results
{
    public class Result<T> : Result
    {
        public T Data { get; }

        private Result(bool success, string message, T data)
            : base(success, message)
        {
            Data = data;
        }

        public static Result<T> OK(T data, string message = "") =>
            new(true, message, data);

        public static new Result<T> Failure(string message) =>
            new(false, message, default!);
    }
}
