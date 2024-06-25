using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace SFA.DAS.PublicSectorOrganisations.Api.Infrastructure;

public class JsonPatchDocumentFilter: IDocumentFilter
{
    public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
    {
        var patchOperation = swaggerDoc.Components.Schemas.AsEnumerable()
            .FirstOrDefault(s => s.Key.ToLower() == "operation");

        if (patchOperation.Key != default)
            patchOperation.Value.Properties.Remove("operationType");
    }
}