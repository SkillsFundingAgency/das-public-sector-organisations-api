namespace SFA.DAS.PublicSectorOrganisations.Domain.Interfaces;

public interface IDateTimeProvider
{
    public DateTime UtcNow { get; }
}