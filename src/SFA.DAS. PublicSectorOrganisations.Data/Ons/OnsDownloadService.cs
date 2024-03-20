using Microsoft.Extensions.Logging;
using SFA.DAS.PublicSectorOrganisations.Domain.Interfaces;
using SFA.DAS.PublicSectorOrganisations.Domain.Configuration;
using SFA.DAS.PublicSectorOrganisations.Domain.Exceptions;

namespace SFA.DAS.PublicSectorOrganisations.Data.Ons
{
    public class OnsDownloadService : IOnsDownloadService
    {
        private readonly IOnsDownloadClient _client;
        private readonly PublicSectorOrganisationsConfiguration _configuration;
        private readonly ILogger<OnsDownloadService> _logger;
        private readonly DateTime _startDateTime;

        public OnsDownloadService(IOnsDownloadClient client, IDateTimeProvider dateTimeProvider, PublicSectorOrganisationsConfiguration configuration, ILogger<OnsDownloadService> logger)
        {
            _client = client;
            _startDateTime = dateTimeProvider.UtcNow;
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<string> CreateLatestOnsExcelFile()
        {
            var workingFolder = Path.GetTempPath();
            var fileName = "publicsectorclassificationguidelatest";

            var maxHistoricFileAttempts = 4;
            var attempt = 0;
            var downloadSuccess = false;

            while (attempt < maxHistoricFileAttempts)
            {
                var url = GetDownloadUrlForMonthYear(attempt);
                _logger.LogInformation("Downloading ONS from {url}", url);

                downloadSuccess = await DownloadFile(url, workingFolder, fileName);

                if (downloadSuccess) break;
                attempt++;
            }

            if (!downloadSuccess)
            {
                const string errorMessage = "Failed to download ONS from current and previous month, potential URL format change";
                _logger.LogError(errorMessage);
                throw new DownloadingExcelFileException(errorMessage);
            }

            return Path.Combine(workingFolder, fileName);
        }

        public async Task<bool> DownloadFile(string url, string targetPath, string targetFilename)
        {
            Directory.CreateDirectory(targetPath);

            var filenameAndPath = Path.Combine(targetPath, targetFilename);

            _logger.LogInformation("Downloading {url} to {filenameAndPath}...", url, filenameAndPath);

            using (var response = await _client.GetAsync(url))
            {
                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError("Status code {statusCode} returned", response.StatusCode);
                    return false;
                }

                try
                {
                    await using var streamToReadFrom = await response.Content.ReadAsStreamAsync();
                    await using Stream streamToWriteTo =
                        File.Open(filenameAndPath, FileMode.Create, FileAccess.ReadWrite);
                    await streamToReadFrom.CopyToAsync(streamToWriteTo);
                }
                catch (Exception ex)
                {
                    throw new DownloadingExcelFileException("Problem extracting Excel file into Temp", ex);
                }
            }
            _logger.LogInformation("Download complete");
            return true;
        }

        private string GetDownloadUrlForMonthYear(int minusMonths)
        {
            var urlpattern = _configuration.OnsUrl;
            var datePattern = _configuration.OnsUrlDateFormat;

            var now = _startDateTime.AddMonths(-minusMonths);

            var url = string.Format(urlpattern, now.ToString(datePattern).ToLower());
            return url;
        }
    }
}
