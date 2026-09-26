using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace WebAPIDemo.Filters.OperationFilter
{
    //public class AuthorizationHeaderOperationFilter : IOperationFilter
    //{
    //    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    //    {
    //        // Ensure Security list is initialized
    //        operation.Security ??= new List<OpenApiSecurityRequirement>();

    //        // Define Bearer scheme reference
    //        var bearerScheme = new OpenApiSecurityScheme
    //        {
    //            Reference = new OpenApiReference
    //            {
    //                Type = ReferenceType.SecurityScheme,
    //                Id = "Bearer"
    //            }
    //        };

    //        // Add requirement using modern collection initializer
    //        operation.Security.Add(new OpenApiSecurityRequirement
    //        {
    //            [bearerScheme] = Array.Empty<string>() // empty scopes for JWT Bearer
    //        });
    //    }
    //}
}