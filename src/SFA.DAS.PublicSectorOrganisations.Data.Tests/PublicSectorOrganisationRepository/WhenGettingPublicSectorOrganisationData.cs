using AutoFixture;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.PublicSectorOrganisations.Domain.PublicSectorOrganisation;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.PublicSectorOrganisations.Data.Tests.PublicSectorOrganisationRepository;

public class WhenGettingPublicSectorOrganisationData
{
    [Test, MoqAutoData]
    public async Task For_an_explicit_Id_and_its_not_found_Then_null_is_returned(
        Guid id
        )
    {
        var db = new PublicSectorOrganisationDataContext(new DbContextOptionsBuilder<PublicSectorOrganisationDataContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString(), b => b.EnableNullChecks(false))
            .Options);

        var sut = new Data.PublicSectorOrganisationRepository(new Lazy<PublicSectorOrganisationDataContext>(db),
            Mock.Of<ILogger<Data.PublicSectorOrganisationRepository>>());


        var entity = await sut.GetPublicSectorOrganisationById(id);

        entity.Should().BeNull();
    }

    [Test, MoqAutoData]
    public async Task For_an_explicit_Id_and_its_found_Then_entity_is_returned(
        PublicSectorOrganisationEntity existingEntity
        )
    {
        var db = new PublicSectorOrganisationDataContext(new DbContextOptionsBuilder<PublicSectorOrganisationDataContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString(), b => b.EnableNullChecks(false))
            .Options);

        db.PublicSectorOrganisationEntities.Add(existingEntity);
        await db.SaveChangesAsync();

        var sut = new Data.PublicSectorOrganisationRepository(new Lazy<PublicSectorOrganisationDataContext>(db),
            Mock.Of<ILogger<Data.PublicSectorOrganisationRepository>>());


        var entity = await sut.GetPublicSectorOrganisationById(existingEntity.Id.Value);

        entity.Should().NotBeNull();
    }

    [Test, MoqAutoData]
    public async Task For_all_organisations_Then_all_active_entities_are_returned()
    {

        var f = new Fixture();

        var activeList = f.CreateMany<PublicSectorOrganisationEntity>().OrderBy(x=>x.Name).ToList();
        activeList.ForEach(x => { x.Active = true;});

        var inactiveList = f.CreateMany<PublicSectorOrganisationEntity>().ToList();
        inactiveList.ForEach(x => { x.Active = false; });

        var db = new PublicSectorOrganisationDataContext(new DbContextOptionsBuilder<PublicSectorOrganisationDataContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString(), b => b.EnableNullChecks(false))
            .Options);

        db.PublicSectorOrganisationEntities.AddRange(activeList);
        db.PublicSectorOrganisationEntities.AddRange(inactiveList);
        await db.SaveChangesAsync();

        var sut = new Data.PublicSectorOrganisationRepository(new Lazy<PublicSectorOrganisationDataContext>(db),
            Mock.Of<ILogger<Data.PublicSectorOrganisationRepository>>());


        var entities = await sut.GetAllActivePublicSectorOrganisations();

        entities.Should().BeEquivalentTo(activeList);
    }


    [TestCase(DataSource.Ons, DataSource.Police)]
    [TestCase(DataSource.Nhs, DataSource.Police)]
    [TestCase(DataSource.Police, DataSource.Ons)]
    public async Task For_each_sector_organisations_Then_all_active_entities_are_returned(DataSource activeSource, DataSource inActiveSource)
    {

        var f = new Fixture();

        var activeList = f.CreateMany<PublicSectorOrganisationEntity>().OrderBy(x => x.Name).ToList();
        activeList.ForEach(x =>
        {
            x.Active = true;
            x.Source = activeSource;
        });

        var inactiveList = f.CreateMany<PublicSectorOrganisationEntity>().ToList();
        inactiveList.ForEach(x =>
        {
            x.Active = true;
            x.Source = inActiveSource;
        });

        var db = new PublicSectorOrganisationDataContext(new DbContextOptionsBuilder<PublicSectorOrganisationDataContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString(), b => b.EnableNullChecks(false))
            .Options);

        db.PublicSectorOrganisationEntities.AddRange(activeList);
        db.PublicSectorOrganisationEntities.AddRange(inactiveList);
        await db.SaveChangesAsync();

        var sut = new Data.PublicSectorOrganisationRepository(new Lazy<PublicSectorOrganisationDataContext>(db),
            Mock.Of<ILogger<Data.PublicSectorOrganisationRepository>>());


        var entities = await sut.GetPublicSectorOrganisationsForDataSource(activeSource);

        entities.Should().BeEquivalentTo(activeList);
    }


    [Test, MoqAutoData]
    public async Task Finding_organisations_by_searchTerm_Then_all_matching_namesare_returned()
    {

        var f = new Fixture();
        var searchTerm = "Hello";


        var matchList = f.CreateMany<PublicSectorOrganisationEntity>().OrderBy(x => x.Name).ToList();
        matchList.ForEach(x =>
        {
            x.Active = true;
            x.Name += searchTerm;
        });

        var noMatchList = f.CreateMany<PublicSectorOrganisationEntity>().ToList();
        noMatchList.ForEach(x => { x.Active = true; });

        var db = new PublicSectorOrganisationDataContext(new DbContextOptionsBuilder<PublicSectorOrganisationDataContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString(), b => b.EnableNullChecks(false))
            .Options);

        db.PublicSectorOrganisationEntities.AddRange(matchList);
        db.PublicSectorOrganisationEntities.AddRange(noMatchList);
        await db.SaveChangesAsync();

        var sut = new Data.PublicSectorOrganisationRepository(new Lazy<PublicSectorOrganisationDataContext>(db),
            Mock.Of<ILogger<Data.PublicSectorOrganisationRepository>>());


        var entities = await sut.GetMatchingActivePublicSectorOrganisations(searchTerm);

        entities.Should().BeEquivalentTo(matchList);
    }
}