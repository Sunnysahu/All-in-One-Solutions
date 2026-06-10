namespace GlobalAnnotationUtils.Common
{
    public class Result<T>
    {
        public bool IsSuccess { get; }
        public T? Value { get; }
        public Error? Error { get; }

        private Result(bool isSuccess, T? value, Error? error)
        {
            IsSuccess = isSuccess;
            Value = value;
            Error = error;
        }

        public static Result<T> Success(Error error, T value)
            => new(true, value, error);

        public static Result<T> Failure(Error error)
            => new(false, default, error);
    }
}
