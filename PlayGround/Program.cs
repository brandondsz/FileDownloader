using System;

namespace PlayGround
{
    class Program
    {
        static void print(string s)
        {
            Console.Out.WriteLineAsync(s);
        }
        static void Main(string[] args)
        {
            try
            {
                Downloader.ProgressUpdateEvent += print;

                Console.WriteLine("How many parallel downloads do you want to execute?");

                var result = Downloader.Download("http://www.ovh.net/files/10Mio.dat",
                    System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location),
                    int.Parse(Console.ReadLine()));

                Console.WriteLine($"Download Summary:\n FileSize: {DisplayFormatHelper.FormatSize(result.Size)}\n Number of chunks: {result.ParallelDownloads}" +
                    $"\n Chunk size: {DisplayFormatHelper.FormatSize(result.ChunkSize)}\n Time taken : {DisplayFormatHelper.TimeSpanDisplayFormat(result.TimeTaken)} \n Downloaded File: {result.FilePath}");
                Console.Read();

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

     
    }
}
