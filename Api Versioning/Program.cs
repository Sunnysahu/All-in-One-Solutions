using Asp.Versioning;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerUI;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "My API V1",
        Version = "v1"
    });

    options.SwaggerDoc("v2", new OpenApiInfo
    {
        Title = "My API V2",
        Version = "v2"
    });
});

builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(1, 0); // Default API version

    options.AssumeDefaultVersionWhenUnspecified = true; // If client does not specify version

    options.ReportApiVersions = true; // Sends supported versions in response header

    options.ApiVersionReader = new UrlSegmentApiVersionReader();  // Read version from URL segment (e.g., /api/v1/products) -- > If you do not mention this line, the API versioning package uses its default version readers.

    /*
    // You can also combine multiple readers:
    options.ApiVersionReader = ApiVersionReader.Combine(
    new UrlSegmentApiVersionReader(),
    new QueryStringApiVersionReader("api-version"),
    new HeaderApiVersionReader("x-api-version")
    );
     */
}).AddApiExplorer(options =>
{
    options.GroupNameFormat = "'v'VVV"; // Format for grouping API versions in Swagger UI (e.g., v1, v2)
    options.SubstituteApiVersionInUrl = true; // Substitute the API version in the URL template
});


// Use for .Net 9.0 and above

//builder.Services.AddApiExplorer(options =>
//{
//    options.GroupNameFormat = "'v'VVV";
//    options.SubstituteApiVersionInUrl = true;
//});
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
        options.SwaggerEndpoint("/swagger/v2/swagger.json", "My API V2");
    });
}

app.UseStatusCodePages(async context =>
{
    var response = context.HttpContext.Response;

    if (response.StatusCode == 404)
    {
        response.ContentType = "application/json";

        await response.WriteAsJsonAsync(new
        {
            Status = StatusCodes.Status404NotFound,
            Message = "API version is invalid or unsupported",
            SupportedVersions = new[] { "1.0", "2.0" }
        });
    }
});



app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();