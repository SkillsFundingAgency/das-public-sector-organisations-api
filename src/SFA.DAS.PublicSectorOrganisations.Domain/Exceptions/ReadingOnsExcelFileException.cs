using System.Runtime.Serialization;

namespace SFA.DAS.PublicSectorOrganisations.Domain.Exceptions;

public class ReadingOnsExcelFileException : Exception
{
    public ReadingOnsExcelFileException()
    {
    }

    public ReadingOnsExcelFileException(string message, Exception e) : base(message, e)
    {
    }

    public ReadingOnsExcelFileException(string message) : base(message)
    {
    }

    protected ReadingOnsExcelFileException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }

}