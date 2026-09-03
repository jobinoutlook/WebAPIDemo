using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using WebAPIDemo.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ApplicationDbContext>(options => {
    options.UseSqlServer(builder.Configuration.GetConnectionString("ShirtStoreManagement"));
});

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddOpenApi();

var app = builder.Build();



if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(options =>
    {
        options.Layout = ScalarLayout.Classic;
    });
}



// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.MapControllers();


app.Run();

