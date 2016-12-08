using System;

namespace OpenClassic.Server.Concurrency
{
    public class WrongThreadException : Exception
    {
        public WrongThreadException() { }

        public WrongThreadException(string message) : base(message) { }

        public WrongThreadException(string message, Exception innerException) : base(message, innerException) { }
    }
}
