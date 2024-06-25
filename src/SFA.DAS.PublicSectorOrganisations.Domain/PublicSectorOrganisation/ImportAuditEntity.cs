namespace SFA.DAS.PublicSectorOrganisations.Domain.PublicSectorOrganisation;

public class ImportAuditEntity
{
    public long? Id { get; set; }
    public DateTime TimeStarted { get; set; }
    public DateTime TimeFinished { get; set; }
    public long RowsUpdated { get; set; }
    public long RowsAdded { get; set; }
    public DataSource Source { get; set; }
}