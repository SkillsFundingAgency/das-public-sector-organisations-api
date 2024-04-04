namespace SFA.DAS.PublicSectorOrganisations.Domain.Interfaces;

public interface IOnsDownloadService
{
    Task<string> CreateLatestOnsExcelFile();
    Task<bool> DownloadFile(string url, string targetPath, string targetFilename);
}