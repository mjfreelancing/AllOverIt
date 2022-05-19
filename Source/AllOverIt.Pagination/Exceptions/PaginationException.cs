using System;

namespace AllOverIt.Pagination.Exceptions
{
    public class PaginationException : Exception
    {
        public PaginationException()
        {
        }

        public PaginationException(string message)
            : base(message)
        {
        }

        public PaginationException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
