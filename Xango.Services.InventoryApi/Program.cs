using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Xango.Serrvices.Server.Utility;
using Xango.Services.InventoryApi;
using Xango.Services.InventoryApi.Data;
using Xango.Services.InventoryApi.Service;
using Xango.Services.InventoryApi.Service.IService;
using Xango.Services.Server.Utility;
using Xango.Services.Server.Utility.Extensions;


var builder = WebApplication.CreateBuilder(args);
#if DEBUG
CertificateInstaller.AddCertificateToTrustedRoot(Environment.GetEnvironmentVariable("CertificateName"), Environment.GetEnvironmentVariable("DevCertificatePassword"));
#endif
builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(7010, listenOptions =>
    {
        listenOptions.UseHttps(Environment.GetEnvironmentVariable("CertificateName"), Environment.GetEnvironmentVariable("DevCertificatePassword"));
    });
});

builder.WebHost.UseUrls("https://0.0.0.0:7010");
// Add services to the container.

builder.Services.AddDbContext<AppDbContext>(option =>
{
    option.UseSqlServer(Environment.GetEnvironmentVariable("SQL_CONNECTION_STRING"));
});

IMapper mapper = MappingConfig.RegisterMaps().CreateMapper();
builder.Services.AddSingleton(mapper);
builder.Services.AddScoped<ITokenProvider, TokenProvider>();
builder.Services.AddScoped<IInventoryService, InventoryService>();
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());


builder.Services.AddMvc();
builder.Services.AddControllers();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddHttpContextAccessor();

builder.Services.AddScoped<IInventoryService, InventoryService>();


builder.Services.AddSwaggerGen(option =>
{
    option.AddSecurityDefinition(name: JwtBearerDefaults.AuthenticationScheme, securityScheme: new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Description = "Enter the Bearer Authorization string as following: `Bearer Generated-JWT-Token`",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
    option.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference= new OpenApiReference
                {
                    Type=ReferenceType.SecurityScheme,
                    Id=JwtBearerDefaults.AuthenticationScheme
                }
            }, new string[]{}
        }
    });
});

builder.AddAppAuthetication();
builder.Services.AddAuthorization();


var app = builder.Build();
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    if (!app.Environment.IsDevelopment())
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "InventoryService API");
        c.RoutePrefix = string.Empty;
    }
});


app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
