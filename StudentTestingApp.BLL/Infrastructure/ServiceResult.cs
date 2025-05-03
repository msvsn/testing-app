using System.Collections.Generic;
using System.Linq;

namespace StudentTestingApp.BLL.Infrastructure
{
    public class ServiceResult<T>
    {
        public bool Success { get; private set; }
        public T Data { get; private set; } // Make setter private
        public List<string> Errors { get; private set; } = new List<string>();

        // Private constructor to enforce usage of static factory methods
        private ServiceResult(bool success, T data, List<string>? errors = null)
        {
            Success = success;
            Data = data;
            if (errors != null)
            {
                Errors = errors;
            }
        }

        // Static factory method for success
        public static ServiceResult<T> Ok(T data)
        {
            // Ensure data is not null for value types or provide default for reference types if needed
            return new ServiceResult<T>(true, data);
        }

        // Static factory method for failure with a list of errors
        public static ServiceResult<T> Failure(List<string> errors)
        {
            // Ensure T has a default value if needed for reference types
            return new ServiceResult<T>(false, default!, errors ?? new List<string> { "An unknown error occurred." });
        }

        // Static factory method for failure with a single error
        public static ServiceResult<T> Failure(string error)
        {
            return Failure(new List<string> { error });
        }
    }
}
