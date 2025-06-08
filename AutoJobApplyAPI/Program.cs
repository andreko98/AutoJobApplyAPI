using AutoJobApplyAPI.Models;
using AutoJobApplyAPI.Services;
using AutoJobApplyAPI.Services.Interface;
using AutoJobApplyDatabase.Context;
using AutoJobApplyDatabase.Repositories;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Configura��es de conex�o com banco (appsettings.json)
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection"); 

builder.Services.Configure<OpenAiSettings>(builder.Configuration.GetSection("OpenAI"));

// Add services to the container.
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddControllers();

// Swagger (se desejar)
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Inje��o dos reposit�rios
builder.Services.AddScoped<IApplicationRepository, ApplicationRepository>();
builder.Services.AddScoped<IEmailRepository, EmailRepository>();
builder.Services.AddScoped<IJobRepository, JobRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();

// Configura��es da OpenAI
builder.Services.AddHttpClient();
builder.Services.Configure<OpenAiSettings>(builder.Configuration.GetSection("OpenAI"));

// Configura��es de email
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));

// Inje��o dos servi�os
builder.Services.AddScoped<IApplicationService, ApplicationService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IJobService, JobService>();
builder.Services.AddScoped<IOpenAIService, OpenAiService>();
builder.Services.AddScoped<IUserService, UserService>();

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
