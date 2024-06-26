using Azure.Identity;
using Microsoft.EntityFrameworkCore;
using SFA.DAS.PublicSectorOrganisations.Domain.Configuration;
using SFA.DAS.PublicSectorOrganisations.Data;

namespace SFA.DAS.PublicSectorOrganisations.Api.AppStart;

public static class DatabaseExtensions
{
    public static void AddDatabaseRegistration(this IServiceCollection services, PublicSectorOrganisationsConfiguration config, string? environmentName)
    {
        services.AddHttpContextAccessor();
        if (environmentName.Equals("DEV", StringComparison.CurrentCultureIgnoreCase))
        {
            services.AddDbContext<PublicSectorOrganisationDataContext>(options =>
            {
                options.UseInMemoryDatabase("SFA.DAS.PublicSectorOrganisations");
            }, ServiceLifetime.Transient);
        }
        else if (environmentName.Equals("LOCAL", StringComparison.CurrentCultureIgnoreCase))
        {
            services.AddDbContext<PublicSectorOrganisationDataContext>(options=>options.UseSqlServer(config.ConnectionString),ServiceLifetime.Transient);
        }
        else
        {
            services.AddDbContext<PublicSectorOrganisationDataContext>(ServiceLifetime.Transient);    
        }
            
        services.AddSingleton(new EnvironmentConfiguration(environmentName));

        services.AddTransient<IPublicSectorOrganisationDataContext, PublicSectorOrganisationDataContext>(provider => provider.GetService<PublicSectorOrganisationDataContext>()!);
        services.AddTransient(provider => new Lazy<PublicSectorOrganisationDataContext>(provider.GetService<PublicSectorOrganisationDataContext>()!));
        services.AddSingleton(new ChainedTokenCredential(
            new ManagedIdentityCredential(),
            new AzureCliCredential(),
            new VisualStudioCodeCredential(),
            new VisualStudioCredential())
        );
    }
}