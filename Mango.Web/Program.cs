using AutoMapper;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.Http;
using Xango.Models.Dto;
using Xango.Service.AuthenticationAPI.Client;
using Xango.Service.CouponAPI.Client;
using Xango.Service.InventoryAPI.Client;
using Xango.Service.OrderAPI.Client;
using Xango.Service.ProductAPI.Client;
using Xango.Service.ShoppingCartAPI.Client;
using Xango.Services;
using Xango.Services.Client.Utility;
using Xango.Services.Dto;
using Xango.Services.Interfaces;
using Xango.Services.Server.Utility;
using Xango.Web.Mapping;
using Xango.Web;
using Xango.Serrvices.Server.Utility;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.ConfigureKestrel(options =>
{
    options.Configure(builder.Configuration.GetSection("Kestrel"));
});


// Add services to the container.

builder.Services.AddControllersWithViews();
builder.Services.AddHttpContextAccessor();
builder.Services.AddHttpClient();


builder.Services.AddScoped<ITokenProvider, TokenProvider>();
builder.Services.AddScoped<ICouponHttpClient, CouponHttpClient>();
builder.Services.AddScoped<IInventoryttpClient, InventoryHttpClient>();
builder.Services.AddScoped<IProductHttpClient, ProductHttpClient>();
builder.Services.AddScoped<IShoppingCartHttpClient, ShoppingCartHttpClient>();
builder.Services.AddScoped<IOrderHttpClient, OrderHttpClient>();
builder.Services.AddScoped<IAuthenticationHttpClient, AuthenticationHttpClient>();
IMapper mapper = MappingConfig.RegisterMaps().CreateMapper();
builder.Services.AddSingleton(mapper);


builder.Services.AddControllersWithViews().AddRazorRuntimeCompilation();
builder.Services.AddRazorPages();

builder.Services.AddAuthorization();
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.ExpireTimeSpan = TimeSpan.FromHours(10);
        options.LoginPath = "/Auth/Login";
        options.AccessDeniedPath = "/Auth/AccessDenied";
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseRouting();
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();



app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
