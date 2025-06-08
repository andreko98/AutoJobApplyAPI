using AutoJobApplyAPI.Models;
using AutoJobApplyAPI.Services;
using AutoJobApplyAPI.Services.Interface;
using AutoJobApplyDatabase.Context;
using AutoJobApplyDatabase.Repositories;
using AutoJobApplyDatabase.Repositories.ApiKey;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Configurações de conexão com banco (appsettings.json)
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection"); 

// Add services to the container.
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddControllers();

// Swagger (se desejar)
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Injeção dos repositórios
builder.Services.AddScoped<IApiKeyRepository, ApiKeyRepository>();
builder.Services.AddScoped<IApplicationRepository, ApplicationRepository>();
builder.Services.AddScoped<IEmailRepository, EmailRepository>();
builder.Services.AddScoped<IJobRepository, JobRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();

// Configurações de email
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));

// Injeção dos serviços
builder.Services.AddScoped<IApplicationService, ApplicationService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IJobService, JobService>();
builder.Services.AddScoped<IUserService, UserService>();

builder.Services.AddDataProtection();
builder.Services.AddScoped<DataProtectionService>();

builder.Services.AddHttpClient<IExternalApiService, ExternalApiService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
