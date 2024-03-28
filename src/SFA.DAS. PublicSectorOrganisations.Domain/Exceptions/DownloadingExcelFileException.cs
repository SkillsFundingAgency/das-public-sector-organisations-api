using System.Runtime.Serialization;

namespace SFA.DAS.PublicSectorOrganisations.Domain.Exceptions;

public class DownloadingExcelFileException : Exception
{
    public DownloadingExcelFileException()
    {
    }

    public DownloadingExcelFileException(string message, Exception e) : base(message, e)
    {
    }

    public DownloadingExcelFileException(string message) : base(message)
    {
    }

    protected DownloadingExcelFileException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}