using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace PlayGround
{
    public static class Downloader
    {
        private static float downloadedChunks = 0;

        public delegate void ProgressUpdate(string message);
        public static event ProgressUpdate ProgressUpdateEvent;

        public static DownloadResult Download(String fileUrl, String destinationFolderPath, int numberOfParallelDownloads)
        {
            downloadedChunks = 0;
            Uri uri = new Uri(fileUrl);
            //Input validation
            if (!Directory.Exists(destinationFolderPath))
            {
                throw new Exception($"Invalid value for destinationFolderPath. Directory {destinationFolderPath} does not exist.");
            }
            if (numberOfParallelDownloads <= 0)
            {
                throw new Exception("Invalid value for numberOfParallelDownloads. Please enter a value greater than zero.");
            }
            //Calculate destination path  
            String destinationFilePath = Path.Combine(destinationFolderPath, uri.Segments.Last());

            DownloadResult result = new DownloadResult() { FilePath = destinationFilePath };

            #region Get file size  
            WebRequest webRequest = HttpWebRequest.Create(fileUrl);
            webRequest.Method = "HEAD";
            long responseLength;
            using (WebResponse webResponse = webRequest.GetResponse())
            {
                if (!webResponse.Headers.AllKeys.Contains("Content-Length"))
                {
                    throw new Exception("Unable to download file. Content-Length not present.");
                }
                responseLength = long.Parse(webResponse.Headers.Get("Content-Length"));
                result.Size = responseLength;
            }
            #endregion
            UpdateProgress($"File Size:{responseLength} bytes");
            if (File.Exists(destinationFilePath))
            {
                File.Delete(destinationFilePath);
            }
            if (responseLength < numberOfParallelDownloads)
            {
                throw new Exception($"The file is too small to be divided into chunks to have {numberOfParallelDownloads} parallel downloads. Please select a value less than {numberOfParallelDownloads}.");
            }
            UpdateProgress("Dividing in to chunks...");

            ConcurrentDictionary<long, string> tempFilesDictionary = new ConcurrentDictionary<long, string>();

            #region Calculate ranges  
            List<Range> readRanges = new List<Range>();
            for (int chunk = 0; chunk < numberOfParallelDownloads - 1; chunk++)
            {
                var range = new Range()
                {
                    Start = chunk * (responseLength / numberOfParallelDownloads),
                    End = ((chunk + 1) * (responseLength / numberOfParallelDownloads)) - 1
                };
                readRanges.Add(range);
            }

            readRanges.Add(new Range()
            {
                Start = readRanges.Any() ? readRanges.Last().End + 1 : 0,
                End = responseLength - 1
            });
            result.ChunkSize = readRanges[0].End - readRanges[0].Start;
            result.ParallelDownloads = numberOfParallelDownloads;
            #endregion
            UpdateProgress($"Divided into {numberOfParallelDownloads} chunks of {readRanges[0].End - readRanges[0].Start} bytes each.");
            DateTime startTime = DateTime.Now;

            #region Parallel download  

            long total = readRanges.Count();
            UpdateProgress("Starting downloads...");
            Parallel.ForEach(readRanges, new ParallelOptions() { MaxDegreeOfParallelism = numberOfParallelDownloads }, readRange =>
            {
                var chunkStart = DateTime.Now;
                HttpWebRequest httpWebRequest = HttpWebRequest.Create(fileUrl) as HttpWebRequest;
                httpWebRequest.Method = "GET";
                httpWebRequest.AddRange(readRange.Start, readRange.End);
                using (HttpWebResponse httpWebResponse = httpWebRequest.GetResponse() as HttpWebResponse)
                {
                    String tempFilePath = Path.GetTempFileName();
                    using (var fileStream = new FileStream(tempFilePath, FileMode.Create, FileAccess.Write, FileShare.Write))
                    {
                        httpWebResponse.GetResponseStream().CopyTo(fileStream);
                        tempFilesDictionary.TryAdd(readRange.Start, tempFilePath);
                    }
                }

                downloadedChunks++;
                UpdateProgress(readRange.End - readRange.Start, DateTime.Now - chunkStart, DateTime.Now - startTime, total, numberOfParallelDownloads);
            });


            #endregion

            result.TimeTaken = DateTime.Now.Subtract(startTime);
            UpdateProgress($"Total time for downloading : {DisplayFormatHelper.TimeSpanDisplayFormat(result.TimeTaken)}");
            UpdateProgress("Merging chunks..");
            #region Merge to single file  
            using (FileStream destinationStream = new FileStream(destinationFilePath, FileMode.Append))
            {
                foreach (var tempFile in tempFilesDictionary.OrderBy(b => b.Key))
                {
                    byte[] tempFileBytes = File.ReadAllBytes(tempFile.Value);
                    destinationStream.Write(tempFileBytes, 0, tempFileBytes.Length);
                    File.Delete(tempFile.Value);
                }
                #endregion
            }
            result.TimeTaken = DateTime.Now.Subtract(startTime);
            UpdateProgress("Process complete!");
            return result;
        }

        private static void UpdateProgress(string message)
        {
            ProgressUpdateEvent?.Invoke(message);
        }
        private static void UpdateProgress(long chunkSize, TimeSpan chunkTime, TimeSpan totalTime, long total, int parallelDownloads)
        {
            if (ProgressUpdateEvent == null)
                return;
            var sb = new StringBuilder();
            //Given the size of the chunk just downloaded and the chunks remaining, calculate the time remaining for the download to complete.
            sb.Append($"{downloadedChunks * 100 / total}% - Speed {DisplayFormatHelper.FormatSize((long)(chunkSize / chunkTime.TotalSeconds))}ps");
            if (total > downloadedChunks)
            {
                //Assuming all the downloads start at relatively the same time ((total - downloadedChunks) * chunkTime.Ticks) - chunkTime.Ticks))
                sb.Append($" - Estimated time remaining {DisplayFormatHelper.TimeSpanDisplayFormat((long)((total - downloadedChunks) * chunkTime.Ticks) - chunkTime.Ticks)}");
            }
            ProgressUpdateEvent(sb.ToString());
        }
    }
}