using Azure.Core;
using Azure.Identity;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SFA.DAS.PublicSectorOrganisations.Data.PublicSectorOrganisation;
using SFA.DAS.PublicSectorOrganisations.Domain.Configuration;
using SFA.DAS.PublicSectorOrganisations.Domain.PublicSectorOrganisation;

namespace SFA.DAS.PublicSectorOrganisations.Data;

public interface IPublicSectorOrganisationDataContext
{
    DbSet<PublicSectorOrganisationEntity> PublicSectorOrganisationEntities { get; set; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken  = default (CancellationToken));
}

public class PublicSectorOrganisationDataContext : DbContext, IPublicSectorOrganisationDataContext
{
    private const string AzureResource = "https://database.windows.net/";
    private readonly ChainedTokenCredential _azureServiceTokenProvider;
    private readonly EnvironmentConfiguration _environmentConfiguration;

    public DbSet<PublicSectorOrganisationEntity> PublicSectorOrganisationEntities { get; set; }

    private readonly PublicSectorOrganisationsConfiguration? _configuration;

    public PublicSectorOrganisationDataContext()
    {
    }

    public PublicSectorOrganisationDataContext(DbContextOptions options) : base(options)
    {
            
    }
    public PublicSectorOrganisationDataContext(IOptions<PublicSectorOrganisationsConfiguration> config, DbContextOptions options, ChainedTokenCredential azureServiceTokenProvider, EnvironmentConfiguration environmentConfiguration) :base(options)
    {
        _azureServiceTokenProvider = azureServiceTokenProvider;
        _environmentConfiguration = environmentConfiguration;
        _configuration = config.Value;
    }
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseLazyLoadingProxies();
            
        if (_configuration == null 
            || _environmentConfiguration.EnvironmentName.Equals("DEV", StringComparison.CurrentCultureIgnoreCase)
            || _environmentConfiguration.EnvironmentName.Equals("LOCAL", StringComparison.CurrentCultureIgnoreCase))
        {
            return;
        }

        var connection = new SqlConnection
        {
            ConnectionString = _configuration.ConnectionString,
            AccessToken = _azureServiceTokenProvider.GetTokenAsync(new TokenRequestContext(scopes: new string[] { AzureResource })).Result.Token
        };
            
        optionsBuilder.UseSqlServer(connection,options=>
            options.EnableRetryOnFailure(
                5,
                TimeSpan.FromSeconds(20),
                null
            ));

    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new PublicSectorOrganisationEntityConfiguration());
        
        //base.OnModelCreating(modelBuilder);
    }
}