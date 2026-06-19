using System.Text.Json.Serialization;

namespace Outbox_Pattern.Common
{
    public class Result<T>
    {
        public bool IsSuccess { get; }

        public T? Value { get; }

        public Response? Message { get; }

        private Result(bool isSuccess, T? value, Response? message)
        {
            IsSuccess = isSuccess;
            Value = value;
            Message = message;
        }

        public static Result<T> Success(Response success, T value)
            => new(true, value, success);

        public static Result<T> Success(Response success)
            => new(true, default, success);

        public static Result<T> Failure(Response error)
            => new(false, default, error);
    }
}
