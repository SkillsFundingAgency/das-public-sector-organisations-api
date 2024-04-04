using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SFA.DAS.PublicSectorOrganisations.Domain.PublicSectorOrganisation;

namespace SFA.DAS.PublicSectorOrganisations.Data.PublicSectorOrganisation;

public class ImportAuditEntityConfiguration : IEntityTypeConfiguration<ImportAuditEntity>
{
    public void Configure(EntityTypeBuilder<ImportAuditEntity> builder)
    {
        builder.ToTable("ImportAudit");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id).HasColumnName("Id").HasColumnType("int").IsRequired().ValueGeneratedOnAdd();
        builder.Property(x => x.TimeStarted).HasColumnName("TimeStarted").HasColumnType("DateTime").IsRequired();
        builder.Property(x => x.TimeFinished).HasColumnName("TimeFinished").HasColumnType("DateTime").IsRequired();
        builder.Property(x => x.RowsUpdated).HasColumnName("RowsUpdated").HasColumnType("int").IsRequired();
        builder.Property(x => x.RowsAdded).HasColumnName("RowsAdded").HasColumnType("int").IsRequired();
        builder.Property(x => x.Source).HasColumnName("Source").HasColumnType("tinyint").IsRequired();

        builder.HasIndex(x => x.Id).IsUnique();
    }
}   