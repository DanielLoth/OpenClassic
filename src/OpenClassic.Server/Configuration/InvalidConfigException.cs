using System;

namespace OpenClassic.Server.Configuration
{
    public class InvalidConfigException : Exception
    {
        public InvalidConfigException() { }

        public InvalidConfigException(string message) : base(message) { }

        public InvalidConfigException(string message, Exception innerException) : base(message, innerException) { }
    }
}
