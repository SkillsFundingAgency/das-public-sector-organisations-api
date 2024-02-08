using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SFA.DAS.PublicSectorOrganisations.Domain.PublicSectorOrganisation;

namespace SFA.DAS.PublicSectorOrganisations.Data.PublicSectorOrganisation;

public class PublicSectorOrganisationEntityConfiguration : IEntityTypeConfiguration<PublicSectorOrganisationEntity>
{
    public void Configure(EntityTypeBuilder<PublicSectorOrganisationEntity> builder)
    {
        builder.ToTable("PublicSectorOrganisation");
        // builder.HasKey(x => x.Id);
    }
}   