using Microsoft.EntityFrameworkCore;
using testback.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddScoped<ApplicationDbContext>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

string _GetConnStringName = builder.Configuration.GetConnectionString("DefaultConnectionMySQL");
builder.Services.AddDbContextPool<ApplicationDbContext>(options =>
    options.UseMySql(_GetConnStringName, ServerVersion.AutoDetect(_GetConnStringName)));


builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngularClient", policy =>
    {
        policy.WithOrigins("http://localhost:4200") // Permitir Angular
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("AllowAngularClient");

app.UseAuthorization();

app.MapControllers();

app.Run();
