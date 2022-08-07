using System;

namespace Express.Interop.Exceptions
{
    public class InvalidPageRangeException : Exception
    {
        public InvalidPageRangeException(string message) : base(message)
        {
        }
    }
}
