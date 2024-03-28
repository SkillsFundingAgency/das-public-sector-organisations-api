using System.Runtime.Serialization;

namespace SFA.DAS.PublicSectorOrganisations.Domain.Exceptions;

public class WithinTransactionException : Exception
{
    public WithinTransactionException()
    {
    }

    public WithinTransactionException(string message, Exception e) : base(message, e)
    {
    }

    public WithinTransactionException(string message) : base(message)
    {
    }

    protected WithinTransactionException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }

}