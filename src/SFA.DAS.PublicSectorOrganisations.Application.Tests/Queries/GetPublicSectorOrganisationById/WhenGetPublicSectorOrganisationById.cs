using AutoFixture.NUnit3;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.PublicSectorOrganisations.Application.Queries.GetPublicSectorOrganisationById;
using SFA.DAS.PublicSectorOrganisations.Domain.Interfaces;
using SFA.DAS.PublicSectorOrganisations.Domain.PublicSectorOrganisation;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.PublicSectorOrganisations.Application.Tests.Queries.GetPublicSectorOrganisationById
{
    public class WhenGetPublicSectorOrganisationById
    {
        [Test, MoqAutoData]
        public async Task And_no_record_Then_returns_null(
            GetPublicSectorOrganisationByIdQuery query,
            [Frozen] Mock<IPublicSectorOrganisationRepository> repo,
            [Greedy] GetPublicSectorOrganisationByIdQueryHandler handler
            )
        {
            repo.Setup(x => x.GetPublicSectorOrganisationById(query.Id)).ReturnsAsync((PublicSectorOrganisationEntity)null);

            var response = await handler.Handle(query, CancellationToken.None);

            response.Should().BeNull();
        }

        [Test, MoqAutoData]
        public async Task And_record_found_Then_returns_mapped_values(
            GetPublicSectorOrganisationByIdQuery query,
            PublicSectorOrganisationEntity entity,
            [Frozen] Mock<IPublicSectorOrganisationRepository> repo,
            [Greedy] GetPublicSectorOrganisationByIdQueryHandler handler
        )
        {
            repo.Setup(x => x.GetPublicSectorOrganisationById(query.Id)).ReturnsAsync(entity);

            var response = await handler.Handle(query, CancellationToken.None);

            response.Should().BeEquivalentTo(new 
            {
                entity.Name,
                entity.Id,
                Source = entity.Source.ToString(),
                entity.AddressLine1,
                entity.AddressLine2,
                entity.AddressLine3,
                entity.Town,
                entity.PostCode,
                entity.Country,
                entity.UPRN,
                entity.OrganisationCode,
                entity.OnsSector,
                entity.Active
            });
        }

    }
}
