// <copyright file="DownloadResult.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
namespace FileDownloader.Models
{
    using System;

    /// <summary>
    /// Data model - DownloadResult
    /// </summary>
    public class DownloadResult
    {
        /// <summary>
        /// Gets or sets the file size
        /// </summary>
        public long Size { get; set; }

        /// <summary>
        /// Gets or sets the file location path
        /// </summary>
        public string FilePath { get; set; }

        /// <summary>
        /// Gets or sets the time taken to download
        /// </summary>
        public TimeSpan TimeTaken { get; set; }

        /// <summary>
        /// Gets or sets the size of the chunk of file
        /// </summary>
        public long ChunkSize { get; set; }
    }
}
