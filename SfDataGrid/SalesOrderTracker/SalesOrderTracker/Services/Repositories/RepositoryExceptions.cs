using System;

namespace SalesOrderTracker.Services.Repositories
{
    public class RepositoryException : Exception
    {
        public RepositoryException(string? message, Exception? inner = null) : base(message, inner) { }
    }

    public class ConcurrencyException : RepositoryException
    {
        public ConcurrencyException(string? message, Exception? inner = null) : base(message, inner) { }
    }
}
