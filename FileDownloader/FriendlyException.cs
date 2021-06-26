// <copyright file="FriendlyException.cs" company="BDZ Corp">
// Copyright (c) BDZ Corp. All rights reserved.
// </copyright>
namespace FileDownloader
{
    using System;

    /// <summary>
    /// Exceptions that are actually friendly error messages
    /// </summary>
    [Serializable]
    public class FriendlyException : 
        Exception
    {
        /// <summary>
        ///  Initializes a new instance of the <see cref="FriendlyException" /> class.
        /// </summary>
        public FriendlyException() : base()
        {
        }
        
        /// <summary>
        ///  Initializes a new instance of the <see cref="FriendlyException" /> class.
        /// </summary>
        /// <param name="message">Exception message</param>
        public FriendlyException(string message) : base(message)
        {
        }
    }
}
