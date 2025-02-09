namespace ECommerceAPI.Helpers
{
    public enum RepositoryErrorType
    {
        ValidationError,
        Conflict,
        NotFound,
        DatabaseError,
        Unauthorized,
        Forbid,
        BadRequest
    }

    public class RepositoryResult<T>
    {
        public bool Success { get; private set; }
        public T? Data { get; private set; }
        public string? ErrorMessage { get; private set; }
        public RepositoryErrorType? ErrorType { get; private set; }

        private RepositoryResult(bool success, T? data, string? errorMessage, RepositoryErrorType? errorType)
        {
            Success = success;
            Data = data;
            ErrorMessage = errorMessage;
            ErrorType = errorType;
        }

        public static RepositoryResult<T> SuccessResult(T data) =>
            new RepositoryResult<T>(true, data, null, null);

        public static RepositoryResult<T> Failure(string errorMessage, RepositoryErrorType errorType) =>
            new RepositoryResult<T>(false, default, errorMessage, errorType);
    }
}