using System.Collections.Concurrent;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.PublicSectorOrganisations.Domain.Configuration;
using SFA.DAS.PublicSectorOrganisations.Domain.Interfaces;
using SFA.DAS.PublicSectorOrganisations.Domain.NhsClientResponse;
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

            var dbRepositoryMock = new Mock<IPublicSectorOrganisationRepository>();
            dbRepositoryMock.Setup(x => x.GetPublicSectorOrganisationsFor(DataSource.Nhs)).ReturnsAsync(new List<PublicSectorOrganisationEntity>());

            var sut = new Data.Nhs.NhsImporterService(nhsClientMock.Object, config, dbRepositoryMock.Object, logger);
            await sut.ImportData();

            dbRepositoryMock.Verify(x=>x.UpdateAndAddPublicSectorOrganisationsFor(DataSource.Nhs, 
                It.Is<ConcurrentBag<PublicSectorOrganisationEntity>>(p=>p.Count == 0), It.IsAny<ConcurrentBag<PublicSectorOrganisationEntity>>()));

            dbRepositoryMock.Verify(x => x.UpdateAndAddPublicSectorOrganisationsFor(DataSource.Nhs, It.IsAny<ConcurrentBag<PublicSectorOrganisationEntity>>(),
                It.Is<ConcurrentBag<PublicSectorOrganisationEntity>>(p => p.Count == 6 &&
                                                                          VerifyMappedRecordHasExpectedValues(p, response1.Organisations[0], organisationsForResponse1[0]) &&
                                                                          VerifyMappedRecordHasExpectedValues(p, response1.Organisations[1], organisationsForResponse1[1]) &&
                                                                          VerifyMappedRecordHasExpectedValues(p, response1.Organisations[2], organisationsForResponse1[2]) &&
                                                                          VerifyMappedRecordHasExpectedValues(p, response2.Organisations[0], organisationsForResponse2[0]) &&
                                                                          VerifyMappedRecordHasExpectedValues(p, response2.Organisations[1], organisationsForResponse2[1]) &&
                                                                          VerifyMappedRecordHasExpectedValues(p, response2.Organisations[2], organisationsForResponse2[2]))));
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

            var dbRepositoryMock = new Mock<IPublicSectorOrganisationRepository>();
            dbRepositoryMock.Setup(x => x.GetPublicSectorOrganisationsFor(DataSource.Nhs)).ReturnsAsync(SetupExistingDataAsExisting(response2));

            var sut = new Data.Nhs.NhsImporterService(nhsClientMock.Object, config, dbRepositoryMock.Object, logger);
            await sut.ImportData();

            dbRepositoryMock.Verify(x => x.UpdateAndAddPublicSectorOrganisationsFor(DataSource.Nhs,
                It.Is<ConcurrentBag<PublicSectorOrganisationEntity>>(p => p.Count == 3), 
                It.Is<ConcurrentBag<PublicSectorOrganisationEntity>>(p=> p.Count == 3)));

            dbRepositoryMock.Verify(x => x.UpdateAndAddPublicSectorOrganisationsFor(DataSource.Nhs,
                It.Is<ConcurrentBag<PublicSectorOrganisationEntity>>(p =>
                    VerifyMappedRecordHasExpectedValues(p, response2.Organisations[0], organisationsForResponse2[0]) &&
                    VerifyMappedRecordHasExpectedValues(p, response2.Organisations[1], organisationsForResponse2[1]) &&
                    VerifyMappedRecordHasExpectedValues(p, response2.Organisations[2], organisationsForResponse2[2])),
                It.Is<ConcurrentBag<PublicSectorOrganisationEntity>>(p =>
                    VerifyMappedRecordHasExpectedValues(p, response1.Organisations[0], organisationsForResponse1[0]) &&
                    VerifyMappedRecordHasExpectedValues(p, response1.Organisations[1], organisationsForResponse1[1]) &&
                    VerifyMappedRecordHasExpectedValues(p, response1.Organisations[2], organisationsForResponse1[2]))));
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

        private bool VerifyMappedRecordHasExpectedValues(ConcurrentBag<PublicSectorOrganisationEntity> records, OrganisationSummary header, GetSingleOrganisationResponse detail)
        {
            records.First(x => x.OrganisationCode == header.OrgId).Should().BeEquivalentTo(
                new 
                {
                    header.Name,
                    Source = DataSource.Nhs,
                    detail.AddressLine1,
                    detail.AddressLine2,
                    detail.AddressLine3,
                    detail.Town,
                    detail.PostCode,
                    detail.Country,
                    detail.UPRN,
                });
            return true;
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
    }
}
