using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

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

