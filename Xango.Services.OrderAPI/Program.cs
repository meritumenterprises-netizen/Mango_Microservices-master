using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Xango.Serrvices.Server.Utility;
using Xango.Service.InventoryAPI.Client;
using Xango.Services.OrderAPI;
using Xango.Services.OrderAPI.Data;
using Xango.Services.Server.Utility;
using Xango.Services.Server.Utility.Extensions;

var builder = WebApplication.CreateBuilder(args);
builder.WebHost.ConfigureKestrel(options =>
{
#if DEBUG
    CertificateInstaller.AddCertificateToTrustedRoot(Environment.GetEnvironmentVariable("CertificateName"), Environment.GetEnvironmentVariable("DevCertificatePassword"));
#endif

    options.ListenAnyIP(7004, listenOptions =>
    {
        listenOptions.UseHttps(Environment.GetEnvironmentVariable("CertificateName"), Environment.GetEnvironmentVariable("DevCertificatePassword"));
    });
});

builder.WebHost.UseUrls("https://0.0.0.0:7004");
IMapper mapper = MappingConfig.RegisterMaps().CreateMapper();
builder.Services.AddSingleton(mapper);

builder.Services.AddControllers();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<BackendApiAuthenticationHttpClientHandler>();
builder.Services.AddScoped<ITokenProvider, TokenProvider>();
//builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IInventoryttpClient, InventoryHttpClient>();


//builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

builder.Services.AddEndpointsApiExplorer();

// Add services to the container.

builder.Services.AddDbContext<AppDbContext>(option =>
{
    option.UseSqlServer(Environment.GetEnvironmentVariable("SQL_CONNECTION_STRING"));
});

builder.Services.AddHttpContextAccessor();

builder.Services.AddScoped<BackendApiAuthenticationHttpClientHandler>();

builder.Services.AddMemoryCache();
builder.Services.AddHttpClient("Product", u => u.BaseAddress =
new Uri(builder.Configuration["ServiceUrls:ProductAPI"])).AddHttpMessageHandler<BackendApiAuthenticationHttpClientHandler>();
builder.Services.AddControllers();
builder.Services.AddHttpClient("Inventory", u => u.BaseAddress =
new Uri(builder.Configuration["ServiceUrls:InventoryAPI"])).AddHttpMessageHandler<BackendApiAuthenticationHttpClientHandler>();


builder.Services.AddEndpointsApiExplorer();

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

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    if (app.Environment.IsDevelopment())
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Order API");
        c.RoutePrefix = string.Empty;
    }
});
Stripe.StripeConfiguration.ApiKey = builder.Configuration.GetSection("Stripe:SecretKey").Get<string>();

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
ApplyMigration();
app.Run();


void ApplyMigration()
{
    using (var scope = app.Services.CreateScope())
    {
        var _db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        if (_db.Database.GetPendingMigrations().Count() > 0)
        {
            _db.Database.Migrate();
        }
    }
}