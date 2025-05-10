using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.OpenApi.Models;
using testback.Data;
using testback.Services;
using Microsoft.Extensions.Configuration;

var builder = WebApplication.CreateBuilder(args);

// Cargar configuración
builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

// Registrar servicios
builder.Services.AddControllers();
builder.Services.AddScoped<CalculadoraJornada>();
builder.Services.AddHttpClient<FestivoApiService>();

// DbContext para SQL Server
string conn = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContextPool<ApplicationDbContext>(options =>
    options.UseSqlServer(conn)
);

// Swagger/OpenAPI
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

// CORS: permitir Angular en localhost
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngularClient", policy =>
    {
        policy
            .WithOrigins("http://localhost:4200")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

var app = builder.Build();

// Middlware pipeline

// Necesario para que UseCors funcione correctamente
app.UseRouting();

// Aplicar CORS antes de MapControllers
app.UseCors("AllowAngularClient");

// HTTPS
app.UseHttpsRedirection();

// Autorización (si la usas)
app.UseAuthorization();

// Swagger siempre disponible en la raíz "/"
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "API Picso v1");
    c.RoutePrefix = string.Empty;
});

// Archivos estáticos para Docspermisos
var docFolder = Path.Combine(builder.Environment.ContentRootPath, "Docspermisos");
if (Directory.Exists(docFolder))
{
    app.UseStaticFiles(new StaticFileOptions
    {
        FileProvider = new PhysicalFileProvider(docFolder),
        RequestPath = "/Docspermisos"
    });
}

// Rutas de los controladores
app.MapControllers();

app.Run();
