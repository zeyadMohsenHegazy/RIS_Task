using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace SmartInventorySystem.API.Swagger;

public class PaginationOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        var actionName = context.ApiDescription.ActionDescriptor.RouteValues["action"];
        var isPaginatedList = actionName is "GetAll" or "GetHistory";

        if (!isPaginatedList)
        {
            return;
        }

        operation.Parameters ??= new List<OpenApiParameter>();

        SetParameterExample(operation, "pageNumber", new OpenApiInteger(1));
        SetParameterExample(operation, "pageSize", new OpenApiInteger(10));
        SetParameterExample(operation, "search", new OpenApiString("laptop"));
    }

    private static void SetParameterExample(
        OpenApiOperation operation,
        string name,
        IOpenApiAny example)
    {
        var parameter = operation.Parameters.FirstOrDefault(p =>
            p.Name.Equals(name, StringComparison.OrdinalIgnoreCase));

        if (parameter is not null)
        {
            parameter.Example = example;
            parameter.Description ??= name switch
            {
                "pageNumber" => "Page number (1-based)",
                "pageSize" => "Items per page (1-100)",
                "search" => "Search by product name",
                _ => parameter.Description
            };
        }
    }
}
