using System;

namespace StudentTestingApp.BLL.Exceptions
{
    public class AuthorizationException : Exception
    {
        public AuthorizationException(string message) : base(message)
        {
        }

        public AuthorizationException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
