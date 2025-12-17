using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Xango.Services.Server.Utility;
using Xango.Service.InventoryAPI.Client;
using Xango.Services.OrderAPI;
using Xango.Services.OrderAPI.Data;
using Xango.Services.Server.Utility.Extensions;
using Xango.Service.QueueAPI.Client;

var builder = WebApplication.CreateBuilder(args);
builder.WebHost.ConfigureKestrel(options =>
{
    options.Configure(builder.Configuration.GetSection("Kestrel"));
});

#if DEBUG
builder.Services.AddCors(options =>
{
	options.AddPolicy("AllowAll",
		policy =>
		{
			policy.AllowAnyOrigin()
				  .AllowAnyHeader()
				  .AllowAnyMethod();
		});
});
#endif


IMapper mapper = MappingConfig.RegisterMaps().CreateMapper();
builder.Services.AddSingleton(mapper);

builder.Services.AddControllers();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<BackendApiAuthenticationHttpClientHandler>();
builder.Services.AddScoped<ITokenProvider, TokenProvider>();
builder.Services.AddScoped<IInventoryttpClient, InventoryHttpClient>();
builder.Services.AddScoped<IQueueHttpClient, QueueHttpClient>();


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
new Uri(Environment.GetEnvironmentVariable("ProductAPI"))).AddHttpMessageHandler<BackendApiAuthenticationHttpClientHandler>();
builder.Services.AddControllers();
builder.Services.AddHttpClient("Inventory", u => u.BaseAddress =
new Uri(Environment.GetEnvironmentVariable("InventoryAPI"))).AddHttpMessageHandler<BackendApiAuthenticationHttpClientHandler>();


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
#if DEBUG
app.UseCors("AllowAll");
#endif


app.UseSwagger();
app.UseSwaggerUI();


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