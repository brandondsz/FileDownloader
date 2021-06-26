// <copyright file="Program.cs" company="BDZ Corp">
// Copyright (c) BDZ Corp. All rights reserved.
// </copyright>
namespace FileDownloader
{
    using System;
    using System.Configuration;
    using FileDownloader.Models;

    /// <summary>
    /// Program class
    /// </summary>
    public class Program
    {
        /// <summary>
        /// Main entry point
        /// </summary>
        /// <param name="args">Arguments to be passed to the application</param>
        public static void Main(string[] args)
        {
            try
            {
                Downloader.ProgressUpdateEvent += Print;
                string retry;
                do
                {
                    Print("How many parallel downloads do you want to execute?");
                    int numOfParallelDownloads = int.Parse(Console.ReadLine());
                    DownloadResult result = Downloader.Download(ConfigurationManager.AppSettings["fileUrl"], ConfigurationManager.AppSettings["downloadLocation"], numOfParallelDownloads);

                    Print($"Download Summary:\n FileSize: {DisplayFormatHelper.FormatSize(result.Size)}\n Number of chunks: {numOfParallelDownloads}" +
                        $"\n Chunk size: {DisplayFormatHelper.FormatSize(result.ChunkSize)}\n Time taken : {DisplayFormatHelper.TimeSpanDisplayFormat(result.TimeTaken)} \n Downloaded File: {result.FilePath}");

                    Print("Try again? (Y/N)");
                    retry = Console.ReadLine();
                } 
                while (!string.IsNullOrWhiteSpace(retry) && retry.ToLower() == "y");
            }
            catch (FriendlyException ex)
            {
                Console.WriteLine(ex.Message);
            }

            Console.Read();
        }

        /// <summary>
        /// Print a message to the console
        /// </summary>
        /// <param name="message">input string to be printed</param>
        private static void Print(string message)
        {
            Console.Out.WriteLineAsync(message);
        }
    }
}
