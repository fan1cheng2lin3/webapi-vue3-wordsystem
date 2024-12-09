using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using MyWordStystemWebapi.Data;
using MyWordStystemWebapi.Helpers;
using MyWordStystemWebapi.Services.Implmentation;
using MyWordStystemWebapi.Services.Interfaces;
using System;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

//------------------------------------------------------

builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "ʹ��Bearer������JWT��Ȩ��ͷ",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new List<string>()
        }
    });
});

//------------------------------------------------------

// ע�� DbContext
builder.Services.AddDbContext<MywordDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

//------------------------------------------------------

// ���ÿ������
builder.Services.AddCors(options =>
{
    options.AddPolicy("cors", p =>
    {
        p.AllowAnyOrigin()
        .AllowAnyMethod()
        .AllowAnyHeader();
    });
});

//------------------------------------------------------

// ����AuthSettings��EmailSettings
builder.Services.Configure<AuthSettings>(builder.Configuration.GetSection("AuthSettings"));
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));

// ע��EmailService
builder.Services.AddSingleton<EmailService>(sp =>
{
    var emailSettings = sp.GetRequiredService<IOptions<EmailSettings>>().Value;
    return new EmailService(
        emailSettings.MailFromAddress,
        bool.Parse(emailSettings.UseSsl),
        emailSettings.Username,
        emailSettings.Password,
        emailSettings.ServerName,
        int.Parse(emailSettings.ServerPort)
    );
});


builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ICiKuService, CiKuService>();


//------------------------------------------------------

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("cors");
app.UseMiddleware<JwtMiddleware>();
app.UseAuthorization();
app.MapControllers();
app.Run();