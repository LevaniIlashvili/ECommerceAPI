namespace ECommerceAPI.Helpers
{
    public enum ErrorType
    {
        ValidationError,
        Conflict,
        NotFound,
        DatabaseError,
        Unauthorized,
        Forbid,
        BadRequest
    }

    public class Result<T>
    {
        public bool Success { get; private set; }
        public T? Data { get; private set; }
        public string? ErrorMessage { get; private set; }
        public ErrorType? ErrorType { get; private set; }

        private Result(bool success, T? data, string? errorMessage, ErrorType? errorType)
        {
            Success = success;
            Data = data;
            ErrorMessage = errorMessage;
            ErrorType = errorType;
        }

        public static Result<T> SuccessResult(T data) =>
            new Result<T>(true, data, null, null);

        public static Result<T> Failure(string errorMessage, ErrorType errorType) =>
            new Result<T>(false, default, errorMessage, errorType);
    }
}