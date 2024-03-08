namespace SFA.DAS.PublicSectorOrganisations.Domain.Interfaces;

public interface IImporterService
{
    Task ImportData();
}

public interface INhsImporterService : IImporterService 
{
}