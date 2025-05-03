using System;

namespace StudentTestingApp.BLL.Exceptions
{
    public class DataAccessException : Exception
    {
        public DataAccessException(string message) : base(message)
        {
        }

        public DataAccessException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
