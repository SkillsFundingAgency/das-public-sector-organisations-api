using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.PublicSectorOrganisations.Domain.Configuration;
using SFA.DAS.PublicSectorOrganisations.Domain.Entities;
using SFA.DAS.PublicSectorOrganisations.Domain.Interfaces;
using SFA.DAS.PublicSectorOrganisations.Domain.Models;
using SFA.DAS.PublicSectorOrganisations.Domain.PublicSectorOrganisation;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.PublicSectorOrganisations.Data.Tests.Nhs.NhsImporterService
{
    public class WhenImportingData
    {
        [Test, MoqAutoData]
        public async Task Then_calls_apis_and_saves_new_records_in_database(
            GetAllOrganisationsResponse response1,
            GetAllOrganisationsResponse response2,
            List<GetSingleOrganisationResponse> organisationsForResponse1,
            List<GetSingleOrganisationResponse> organisationsForResponse2,
            ILogger<Data.Nhs.NhsImporterService> logger
            )
        {
            var config = new PublicSectorOrganisationsConfiguration
            {
                ConnectionString = "https://xxxx.com",
                NhsSectors = (new List<NhsSector> {new() {ExternalCode = "E1", InternalCode = "I1"}, new() { ExternalCode = "E2", InternalCode = "I2" }}).ToArray()
            };

            var nhsClientMock = new Mock<INhsClient>();
            nhsClientMock.Setup(x => x.GetAllOrganisations(config.NhsSectors[0].InternalCode)).ReturnsAsync(response1);
            nhsClientMock.Setup(x => x.GetAllOrganisations(config.NhsSectors[1].InternalCode)).ReturnsAsync(response2);

            SetupDetailResponsesForSummary(nhsClientMock, response1, organisationsForResponse1);
            SetupDetailResponsesForSummary(nhsClientMock, response2, organisationsForResponse2);

            var dbContextMock = new Mock<IPublicSectorOrganisationDataContext>();
            var dbSetMock = new MockDbSet<PublicSectorOrganisationEntity>();

            dbContextMock.Setup(x => x.PublicSectorOrganisationEntities)
                .Returns(dbSetMock);


            var sut = new Data.Nhs.NhsImporterService(nhsClientMock.Object, config, dbContextMock.Object, logger);
            await sut.ImportData();

            sut.NumberOfRecordsAdded.Should().Be(organisationsForResponse2.Count + organisationsForResponse1.Count);
            sut.NumberOfRecordsUpdated.Should().Be(0);
        }

        [Test, MoqAutoData]
        public async Task Then_calls_apis_and_adds_new_records_and_saves_updated_records_in_database(
            GetAllOrganisationsResponse response1,
            GetAllOrganisationsResponse response2,
            List<GetSingleOrganisationResponse> organisationsForResponse1,
            List<GetSingleOrganisationResponse> organisationsForResponse2,
            ILogger<Data.Nhs.NhsImporterService> logger
            )
        {
            var config = new PublicSectorOrganisationsConfiguration
            {
                ConnectionString = "https://xxxx.com",
                NhsSectors = (new List<NhsSector> { new() { ExternalCode = "E1", InternalCode = "I1" }, new() { ExternalCode = "E2", InternalCode = "I2" } }).ToArray()
            };

            var nhsClientMock = new Mock<INhsClient>();
            nhsClientMock.Setup(x => x.GetAllOrganisations(config.NhsSectors[0].InternalCode)).ReturnsAsync(response1);
            nhsClientMock.Setup(x => x.GetAllOrganisations(config.NhsSectors[1].InternalCode)).ReturnsAsync(response2);

            SetupDetailResponsesForSummary(nhsClientMock, response1, organisationsForResponse1);
            SetupDetailResponsesForSummary(nhsClientMock, response2, organisationsForResponse2);

            var dbContextMock = new Mock<IPublicSectorOrganisationDataContext>();
            var dbSetMock = new MockDbSet<PublicSectorOrganisationEntity>(SetupExistingDataAsExisting(response2));
            dbContextMock.Setup(x => x.PublicSectorOrganisationEntities)
                .Returns(dbSetMock);

            var sut = new Data.Nhs.NhsImporterService(nhsClientMock.Object, config, dbContextMock.Object, logger);
            await sut.ImportData();

            sut.NumberOfRecordsAdded.Should().Be(organisationsForResponse1.Count);
            sut.NumberOfRecordsUpdated.Should().Be(organisationsForResponse2.Count);
        }

        private static PublicSectorOrganisationDataContext CreateInMemoryPublicSectorOrganisationDataContext()
        {
            var dbOptions = new DbContextOptionsBuilder<PublicSectorOrganisationDataContext>();
            dbOptions.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTrackingWithIdentityResolution);
            dbOptions.UseInMemoryDatabase(Guid.NewGuid().ToString());
            dbOptions.EnableSensitiveDataLogging();
            var db = new PublicSectorOrganisationDataContext(dbOptions.Options);
            db.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTrackingWithIdentityResolution;

            return db;
        }

        private List<PublicSectorOrganisationEntity> SetupExistingDataAsExisting(GetAllOrganisationsResponse response2)
        {
            var list = new List<PublicSectorOrganisationEntity>();
            foreach (var summary in response2.Organisations)
            {
                list.Add(new PublicSectorOrganisationEntity
                {
                    Id = Guid.NewGuid(),
                    OrganisationCode = summary.OrgId,
                    Source = DataSource.Nhs,
                    Name = "Anything",
                    AddressLine1 = "Anything"
                });
            }

            return list;
        }


        private async Task SetupExistingDataFrom(PublicSectorOrganisationDataContext dbContext, GetAllOrganisationsResponse response2)
        {
            foreach (var summary in response2.Organisations)
            {
                dbContext.PublicSectorOrganisationEntities.Add(new PublicSectorOrganisationEntity
                {
                    Id = Guid.NewGuid(),
                    OrganisationCode = summary.OrgId,
                    Source = DataSource.Nhs,
                    Name = "Anything",
                    AddressLine1 = "Anything"
                });
            }

            await dbContext.SaveChangesAsync();
        }

        private void VerifyMappedRecordInDatabaseHasExpectedValues(PublicSectorOrganisationDataContext db, OrganisationSummary header, GetSingleOrganisationResponse detail)
        {
            db.PublicSectorOrganisationEntities.First(x => x.OrganisationCode == header.OrgId).Should().BeEquivalentTo(
                new PublicSectorOrganisationEntity
                {
                    Name = header.Name,
                    Source = DataSource.Nhs,
                    AddressLine1 = detail.AddressLine1,
                    AddressLine2 = detail.AddressLine2,
                    AddressLine3 = detail.AddressLine3,
                    Town = detail.Town,
                    PostCode = detail.PostCode,
                    Country = detail.Country,
                    UPRN = detail.UPRN,
                });
        }

        private void SetupDetailResponsesForSummary(Mock<INhsClient> mock, GetAllOrganisationsResponse summaryResponse, List<GetSingleOrganisationResponse> detailsResponse)
        {
            var i = 0;
            foreach (var organisation in summaryResponse.Organisations)
            {
                mock.Setup(x => x.GetOrganisation(organisation.OrgId)).ReturnsAsync(detailsResponse[i]);
                i++;
            }
        }

        private IEnumerable<PublicSectorOrganisationEntity> GetListOfPublicSectorOrganisationEntities()
        {
            return new List<PublicSectorOrganisationEntity>
            {
                new PublicSectorOrganisationEntity {Id = Guid.NewGuid(), Name = "Name 1", Source = DataSource.Nhs},
            };
        }

    }
}
