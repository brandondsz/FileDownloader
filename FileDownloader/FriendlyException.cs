using System;
/// <summary>
/// Exception that is meant to be displayed in a user friendly messages
/// </summary>
namespace FileDownloader
{
    [Serializable]
    public class FriendlyException : Exception
    {
        public FriendlyException() : base() { }
        public FriendlyException(string message) : base(message) { }
        public FriendlyException(string message, Exception inner) : base(message, inner) { }
    }
}
