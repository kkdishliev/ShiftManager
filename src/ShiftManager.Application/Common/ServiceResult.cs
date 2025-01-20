namespace ShiftManager.Application.Common
{
    public class ServiceResult
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
        public List<string> Errors { get; set; } = new List<string>();

        public static ServiceResult Success(string message = "Operation completed successfully")
        {
            return new ServiceResult
            {
                IsSuccess = true,
                Message = message
            };
        }

        public static ServiceResult Failure(List<string> errors, string message = "Operation failed")
        {
            return new ServiceResult
            {
                IsSuccess = false,
                Errors = errors,
                Message = message
            };
        }

        public static ServiceResult Failure(string error, string message = "Operation failed")
        {
            return Failure(new List<string> { error }, message);
        }
    }

    public class ServiceResult<T> : ServiceResult
    {
        public T Data { get; set; }

        public static ServiceResult<T> Success(T data, string message = "Operation completed successfully")
        {
            return new ServiceResult<T>
            {
                IsSuccess = true,
                Data = data,
                Message = message
            };
        }

        public new static ServiceResult<T> Failure(List<string> errors, string message = "Operation failed")
        {
            return new ServiceResult<T>
            {
                IsSuccess = false,
                Errors = errors,
                Message = message
            };
        }

        public new static ServiceResult<T> Failure(string error, string message = "Operation failed")
        {
            return Failure(new List<string> { error }, message);
        }
    }
}