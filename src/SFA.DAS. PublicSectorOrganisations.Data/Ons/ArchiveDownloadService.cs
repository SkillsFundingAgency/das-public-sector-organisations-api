//using Microsoft.Extensions.Logging;
//using SFA.DAS.PublicSectorOrganisations.Domain.Interfaces;
//using System.IO.Compression;

//namespace SFA.DAS.PublicSectorOrganisations.Data.Ons
//{
//    public class ArchiveDownloadService : IArchiveDownloadService
//    {
//        private readonly ILogger<ArchiveDownloadService> _logger;

//        public ArchiveDownloadService(ILogger<ArchiveDownloadService> logger)
//        {
//            _logger = logger;
//        }

//        public async Task<bool> DownloadFile(string url, string targetPath, string targetFilename)
//        {
//            Directory.CreateDirectory(targetPath);

//            var filenameAndPath = Path.Combine(targetPath, targetFilename);

//            _logger.LogInformation("Downloading {url} to {filenameAndPath}...", url, filenameAndPath);

//            using (var apiClient = new HttpClient())
//            {
//                // Add a User-Agent header to mimic a web browser
//                apiClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/58.0.3029.110 Safari/537.36");

//                using (var response = await apiClient.GetAsync(url, HttpCompletionOption.ResponseHeadersRead))
//                {
//                    if (!response.IsSuccessStatusCode)
//                    {
//                        _logger.LogError(new Exception($"Status code {response.StatusCode} returned"), $"Status code {response.StatusCode} returned");
//                        return false;
//                    }

//                    using (var streamToReadFrom = await response.Content.ReadAsStreamAsync())
//                    {
//                        using (Stream streamToWriteTo = File.Open(filenameAndPath, FileMode.Create, FileAccess.ReadWrite))
//                        {
//                            await streamToReadFrom.CopyToAsync(streamToWriteTo);
//                        }
//                    }
//                }
//            }
//            _logger.LogInformation("Download complete");
//            return true;
//        }

//        public void UnzipFile(string zipFile, string targetPath)
//        {
//            //var dirInfo = new DirectoryInfo(targetPath);  
//            //if (dirInfo.Exists)
//            //{
//            //    _logger.Warn($"Extract folder {targetPath} already exists - deleting");
//            //    try
//            //    {
//            //        dirInfo.Delete(true);
//            //    }
//            //    catch (Exception ex)
//            //    {
//            //        _logger.Error(ex, "Error deleting folder");
//            //        throw;
//            //    }
//            //}

//            //try
//            //{
//            //    _logger.Warn($"Extracting {zipFile} to {targetPath}");
//            //    ZipFile.ExtractToDirectory(zipFile, targetPath);
//            //}
//            //catch (Exception ex)
//            //{
//            //    _logger.Error(ex, "Error extracting archive");
//            //    throw;
//            //}
//        }
//    }
//}
