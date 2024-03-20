using AutoFixture.NUnit3;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.PublicSectorOrganisations.Application.Queries.GetMatchingPublicSectorOrganisations;
using SFA.DAS.PublicSectorOrganisations.Domain.Interfaces;
using SFA.DAS.PublicSectorOrganisations.Domain.PublicSectorOrganisation;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.PublicSectorOrganisations.Application.Tests.Queries.GetMatchingPublicSectorOrganisations;

public class WhenGettingMatchingPublicSectorOrganisations
{
    [Test, MoqAutoData]
    public async Task And_no_matching_records_Then_returns_empty_list(
        GetMatchingPublicSectorOrganisationsQuery query,
        [Frozen] Mock<IPublicSectorOrganisationRepository> repo,
        [Greedy] GetMatchingPublicSectorOrganisationsHandler handler
        )
    {
        repo.Setup(x => x.GetMatchingActivePublicSectorOrganisations(query.SearchTerm)).ReturnsAsync(new List<PublicSectorOrganisationEntity>());

        var response = await handler.Handle(query, CancellationToken.None);

        response.PublicSectorOrganisations.Length.Should().Be(0);
        repo.Verify(x=>x.GetAllActivePublicSectorOrganisations(), Times.Never);
    }

    [Test, MoqAutoData]
    public async Task And_no_search_term_Then_returns_full_list(
        List<PublicSectorOrganisationEntity> fullList,
        [Frozen] Mock<IPublicSectorOrganisationRepository> repo,
        [Greedy] GetMatchingPublicSectorOrganisationsHandler handler
    )
    {

        fullList = fullList.OrderBy(x => x.Name).ToList();
        repo.Setup(x => x.GetAllActivePublicSectorOrganisations()).ReturnsAsync(fullList);

        var response = await handler.Handle(new GetMatchingPublicSectorOrganisationsQuery(), CancellationToken.None);

        response.PublicSectorOrganisations.Length.Should().Be(3);
        ResponseMatchesEntity(response.PublicSectorOrganisations[0], fullList[0]);
        ResponseMatchesEntity(response.PublicSectorOrganisations[1], fullList[1]);
        ResponseMatchesEntity(response.PublicSectorOrganisations[2], fullList[2]);
        repo.Verify(x => x.GetMatchingActivePublicSectorOrganisations(It.IsAny<string>()), Times.Never);
    }

    [Test, MoqAutoData]
    public async Task And_with_search_term_Then_returns_full_list(
        List<PublicSectorOrganisationEntity> fullList,
        string searchTerm, 
        [Frozen] Mock<IPublicSectorOrganisationRepository> repo,
        [Greedy] GetMatchingPublicSectorOrganisationsHandler handler
    )
    {
        fullList = fullList.OrderBy(x => x.Name).ToList();
        repo.Setup(x => x.GetMatchingActivePublicSectorOrganisations(searchTerm)).ReturnsAsync(fullList);

        var response = await handler.Handle(new GetMatchingPublicSectorOrganisationsQuery { SearchTerm = searchTerm }, CancellationToken.None);

        response.PublicSectorOrganisations.Length.Should().Be(3);
        ResponseMatchesEntity(response.PublicSectorOrganisations[0], fullList[0]);
        ResponseMatchesEntity(response.PublicSectorOrganisations[1], fullList[1]);
        ResponseMatchesEntity(response.PublicSectorOrganisations[2], fullList[2]);
        repo.Verify(x => x.GetAllActivePublicSectorOrganisations(), Times.Never);
    }

    private bool ResponseMatchesEntity(PublicSectorOrganisation response, PublicSectorOrganisationEntity entity)
    {
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
        return true;
    }
}