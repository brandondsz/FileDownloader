using System;
using System.Configuration;

namespace FileDownloader
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                Downloader.ProgressUpdateEvent += print;
                string retry;
                do
                {
                    print("How many parallel downloads do you want to execute?");

                    DownloadResult result = Downloader.Download(ConfigurationManager.AppSettings["fileUrl"], ConfigurationManager.AppSettings["downloadLocation"], int.Parse(Console.ReadLine()));

                    print($"Download Summary:\n FileSize: {DisplayFormatHelper.FormatSize(result.Size)}\n Number of chunks: {result.ParallelDownloads}" +
                        $"\n Chunk size: {DisplayFormatHelper.FormatSize(result.ChunkSize)}\n Time taken : {DisplayFormatHelper.TimeSpanDisplayFormat(result.TimeTaken)} \n Downloaded File: {result.FilePath}");

                    print("Try again? (Y/N)");
                    retry = Console.ReadLine();
                } while (!string.IsNullOrWhiteSpace(retry) && retry.ToLower() == "y");

            }
            catch (FriendlyException ex)
            {
                Console.WriteLine(ex.Message);
            }
            Console.Read();
        }

        private static void print(string s)
        {
            Console.Out.WriteLineAsync(s);
        }

    }
}
