namespace SFA.DAS.PublicSectorOrganisations.Api.AppStart;

public static class AddServiceRegistrationExtension
{
    public static void AddServiceRegistration(this IServiceCollection services)
    {
        // services.AddScoped<IPublicSectorOrganisationRepository, PublicSectorOrganisationRepository>();
        // services.AddMediatR(configuration => configuration.RegisterServicesFromAssembly(typeof(GetAllPublicSectorOrganisationsQuery).Assembly));
    }
}