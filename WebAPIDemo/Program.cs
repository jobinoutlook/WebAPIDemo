using Asp.Versioning;
using Asp.Versioning.ApiExplorer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi;
using WebAPIDemo.Data;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("ShirtStoreManagement"));
});
// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddOpenApi("v1", options =>
{
    options.AddDocumentTransformer((document, context, cancellationToken) =>
    {
        document.Components ??= new OpenApiComponents();
        document.Components.SecuritySchemes = new Dictionary<string, IOpenApiSecurityScheme>
        {
            ["Bearer"] = new OpenApiSecurityScheme
            {
                Scheme = "Bearer",
                Type = SecuritySchemeType.Http,
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
            }
        };

        document.Security ??= [];
        document.Security.Add(new OpenApiSecurityRequirement
        {
            [new OpenApiSecuritySchemeReference("Bearer", document, null)] = []
        });

        return Task.CompletedTask;
    });
});

builder.Services.AddOpenApi("v2", options =>
{
    options.AddDocumentTransformer((document, context, cancellationToken) =>
    {
        document.Components ??= new OpenApiComponents();
        document.Components.SecuritySchemes = new Dictionary<string, IOpenApiSecurityScheme>
        {
            ["Bearer"] = new OpenApiSecurityScheme
            {
                Scheme = "Bearer",
                Type = SecuritySchemeType.Http,
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
            }
        };

        document.Security ??= [];
        document.Security.Add(new OpenApiSecurityRequirement
        {
            [new OpenApiSecuritySchemeReference("Bearer", document, null)] = []
        });

        return Task.CompletedTask;
    });
});

builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ReportApiVersions = true;
    options.ApiVersionReader = ApiVersionReader.Combine(
        new UrlSegmentApiVersionReader(),
        new HeaderApiVersionReader("x-api-version"),
        new QueryStringApiVersionReader("api-version")
        );
})
.AddApiExplorer(options =>
{
    options.GroupNameFormat = "'v'VVV"; // e.g., v1, v2
    options.SubstituteApiVersionInUrl = true;
    options.AddApiVersionParametersWhenVersionNeutral = true; // 👈 important
});

var app = builder.Build();
var apiVersionDescriptionProvider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();
if (app.Environment.IsDevelopment())
{
    Console.WriteLine("Dicovered API Versions:");
    foreach (var description in apiVersionDescriptionProvider.ApiVersionDescriptions)
    {
        Console.WriteLine($"- {description.GroupName} (v{description.ApiVersion})");
    }
}
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "WebAPI API v1");
        options.SwaggerEndpoint("/openapi/v2.json", "WebAPI API v2");
    });
}
app.UseHttpsRedirection();
app.MapControllers();

app.Run();


/*
 [ApiVersion("2.0")]
[ApiExplorerSettings(GroupName = "v2")]

in V2

[ApiVersion("1.0")]
[ApiExplorerSettings(GroupName = "v1")]

in V1 
 
 */