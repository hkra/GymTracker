using System;

namespace GymTracker.Exceptions
{
    public class GitSourceException : Exception
    {
        public GitSourceException(string message) : base(message)
        {
        }
    }
}