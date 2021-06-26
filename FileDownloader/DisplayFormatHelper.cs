// <copyright file="DisplayFormatHelper.cs" company="BDZ Corp">
// Copyright (c) BDZ Corp. All rights reserved.
// </copyright>
namespace FileDownloader
{
    using System;
    using System.Text;

    /// <summary>
    /// Display formatter for different data types
    /// </summary>
    public static class DisplayFormatHelper
    {
        #region Memory
        /// <summary>
        //  File format suffixes
        /// </summary>
        private static readonly string[] FileSizeSuffixes = { "Bytes", "KB", "MB", "GB", "TB", "PB" };
        /// <summary>
        /// File size formatting
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static string FormatSize(long bytes)
        {
            int counter = 0;
            decimal number = bytes;
            while (Math.Round(number / 1024) >= 1)
            {
                number = number / 1024;
                counter++;
            }
            return string.Format("{0:n1}{1}", number, FileSizeSuffixes[counter]);
        }
        #endregion

        #region Time
        /// <summary>
        /// Time Span formatting
        /// </summary>
        /// <param name="span"></param>
        /// <returns></returns>
        public static string TimeSpanDisplayFormat(TimeSpan span)
        {
            if (span == TimeSpan.Zero) return "0 minutes";

            StringBuilder sb = new StringBuilder();
            if (span.Days > 0)
                sb.AppendFormat("{0} day{1} ", span.Days, span.Days > 1 ? "s" : String.Empty);
            if (span.Hours > 0)
                sb.AppendFormat("{0} hour{1} ", span.Hours, span.Hours > 1 ? "s" : String.Empty);
            if (span.Minutes > 0)
                sb.AppendFormat("{0} minute{1} ", span.Minutes, span.Minutes > 1 ? "s" : String.Empty);
            if (span.Seconds > 0)
                sb.AppendFormat("{0} second{1} ", span.Seconds, span.Seconds > 1 ? "s" : String.Empty);
            if (span.Milliseconds > 0)
                sb.AppendFormat("{0} milisecond{1} ", span.Milliseconds, span.Milliseconds > 1 ? "s" : String.Empty);
            return sb.ToString();

        }

        /// <summary>
        /// Time Span formatting
        /// </summary>
        /// <param name="seconds"></param>
        /// <returns></returns>
        public static string TimeSpanDisplayFormat(long seconds)
        {
            return TimeSpanDisplayFormat(TimeSpan.FromTicks(seconds));
        }
        #endregion
    }
}
