using System.Collections.Concurrent;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.PublicSectorOrganisations.Domain.Interfaces;
using SFA.DAS.PublicSectorOrganisations.Domain.PoliceApiClientResponse;
using SFA.DAS.PublicSectorOrganisations.Domain.PublicSectorOrganisation;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.PublicSectorOrganisations.Data.Tests.Police.PoliceImporterService;

public class WhenImportingPoliceData
{
    [Test, MoqAutoData]
    public async Task Then_calls_apis_and_saves_new_records_in_database(
        PoliceForce[] response,
        ILogger<Data.Police.PoliceImporterService> logger
        )
    {

        var policeApiClientMock = new Mock<IPoliceApiClient>();
        policeApiClientMock.Setup(x => x.GetAllPoliceForces()).ReturnsAsync(response);

        var dbRepositoryMock = new Mock<IPublicSectorOrganisationRepository>();
        dbRepositoryMock.Setup(x => x.GetPublicSectorOrganisationsForDataSource(DataSource.Police)).ReturnsAsync(new List<PublicSectorOrganisationEntity>());

        var sut = new Data.Police.PoliceImporterService(policeApiClientMock.Object,dbRepositoryMock.Object, logger);
        await sut.ImportData();

        dbRepositoryMock.Verify(x=>x.UpdateAndAddPublicSectorOrganisationsFor(DataSource.Police, 
            It.Is<ConcurrentBag<PublicSectorOrganisationEntity>>(p=>p.Count == 0), It.IsAny<ConcurrentBag<PublicSectorOrganisationEntity>>(), It.IsAny<DateTime>()));

        dbRepositoryMock.Verify(x => x.UpdateAndAddPublicSectorOrganisationsFor(DataSource.Police, It.IsAny<ConcurrentBag<PublicSectorOrganisationEntity>>(),
            It.Is<ConcurrentBag<PublicSectorOrganisationEntity>>(p => p.Count == 3 &&
                                                                      VerifyMappedRecordHasExpectedValues(p, response[0]) &&
                                                                      VerifyMappedRecordHasExpectedValues(p, response[1]) &&
                                                                      VerifyMappedRecordHasExpectedValues(p, response[2])), It.IsAny<DateTime>()));
    }

    [Test, MoqAutoData]
    public async Task Then_calls_apis_and_adds_new_records_and_saves_updated_records_in_database(
        PoliceForce[] response,
        ILogger<Data.Police.PoliceImporterService> logger
        )
    {
        var policeApiClientMock = new Mock<IPoliceApiClient>();
        policeApiClientMock.Setup(x => x.GetAllPoliceForces()).ReturnsAsync(response);

        var dbRepositoryMock = new Mock<IPublicSectorOrganisationRepository>();
        dbRepositoryMock.Setup(x => x.GetPublicSectorOrganisationsForDataSource(DataSource.Police))
            .ReturnsAsync(new List<PublicSectorOrganisationEntity>
            {
                new PublicSectorOrganisationEntity
                {
                    OrganisationCode = response[0].Id,
                    Source = DataSource.Police,
                    Name = "Old Name"
                }
            });

        var sut = new Data.Police.PoliceImporterService(policeApiClientMock.Object, dbRepositoryMock.Object, logger);
        await sut.ImportData();

        dbRepositoryMock.Verify(x => x.UpdateAndAddPublicSectorOrganisationsFor(DataSource.Police,
            It.Is<ConcurrentBag<PublicSectorOrganisationEntity>>(p => p.Count == 1), 
            It.Is<ConcurrentBag<PublicSectorOrganisationEntity>>(p=>p.Count == 2), It.IsAny<DateTime>()));

        dbRepositoryMock.Verify(x => x.UpdateAndAddPublicSectorOrganisationsFor(DataSource.Police,
            It.Is<ConcurrentBag<PublicSectorOrganisationEntity>>(p =>
                VerifyMappedRecordHasExpectedValues(p, response[0])),
            It.Is<ConcurrentBag<PublicSectorOrganisationEntity>>(p =>
                VerifyMappedRecordHasExpectedValues(p, response[1]) &&
                VerifyMappedRecordHasExpectedValues(p, response[2])), It.IsAny<DateTime>()));
    }

    private bool VerifyMappedRecordHasExpectedValues(ConcurrentBag<PublicSectorOrganisationEntity> records, PoliceForce detail)
    {
        records.First(x => x.OrganisationCode == detail.Id).Should().BeEquivalentTo(
            new 
            {
                detail.Name
            });
        return true;
    }
}