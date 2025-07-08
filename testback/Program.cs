using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.OpenApi.Models;
using testback.Data;
using testback.Services;
using Microsoft.Extensions.Configuration;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

builder.Services.AddControllers()
    .AddJsonOptions(x =>
        x.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles);

builder.Services.AddScoped<CalculadoraJornada>();
builder.Services.AddHttpClient<FestivoApiService>();

string conn = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContextPool<ApplicationDbContext>(options =>
    options.UseSqlServer(conn)
);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "API Picso",
        Version = "v1",
        Description = "Documentación de la API Picso"
    });
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontendClients", policy =>
    {
        policy
            .WithOrigins(
                "http://localhost:4200",
                "https://lively-meadow-0b592d31e.6.azurestaticapps.net"
            )
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

var app = builder.Build();

app.UseRouting();

app.UseCors("AllowFrontendClients"); 

app.UseAuthentication();
app.UseAuthorization();

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "API Picso v1");
    c.RoutePrefix = string.Empty;
});

var docFolder = Path.Combine(builder.Environment.ContentRootPath, "Docspermisos");
if (Directory.Exists(docFolder))
{
    app.UseStaticFiles(new StaticFileOptions
    {
        FileProvider = new PhysicalFileProvider(docFolder),
        RequestPath = "/Docspermisos"
    });
}

app.MapControllers();
app.Run();
