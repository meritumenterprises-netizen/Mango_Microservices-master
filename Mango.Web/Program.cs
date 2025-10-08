using Xango.Web.Service;
using Xango.Web.Service.IService;
using Xango.Services;
using Xango.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.Cookies;
using Xango.Models.Dto;
using Xango.Web.BaseService;
using AutoMapper;
using Xango.Web.Mapping;
using Xango.Services.Server.Utility;
using Xango.Services.Dto;
using Xango.Services.Client.Utility;
using Xango.Service.CouponAPI.Client;
using Xango.Service.InventoryAPI.Client;
using Xango.Service.ProductAPI.Client;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllersWithViews();
builder.Services.AddHttpContextAccessor();
builder.Services.AddHttpClient();


builder.Services.AddHttpClient<ICartService, CartService>();
builder.Services.AddHttpClient<IAuthService, AuthService>();
builder.Services.AddHttpClient<IOrderService, OrderService>();
//builder.Services.AddHttpClient<IInventoryService, InventoryService>();
SD.CouponAPIBase = builder.Configuration["ServiceUrls:CouponAPI"];
SD.OrderAPIBase = builder.Configuration["ServiceUrls:OrderAPI"];
SD.ShoppingCartAPIBase = builder.Configuration["ServiceUrls:ShoppingCartAPI"];
SD.AuthAPIBase = builder.Configuration["ServiceUrls:AuthAPI"];
SD.ProductAPIBase = builder.Configuration["ServiceUrls:ProductAPI"];
SD.InventoryAPIBase = builder.Configuration["ServiceUrls:InventoryAPI"];
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<ICartService, CartService>();
builder.Services.AddScoped<IAuthService, AuthService>();
//builder.Services.AddScoped<ICouponService, CouponService>();
builder.Services.AddScoped<ITokenProvider, TokenProvider>();
builder.Services.AddScoped<IBaseService, BaseService>();
builder.Services.AddScoped<ICouponHttpClient, CouponHttpClient>();
builder.Services.AddScoped<IInventoryttpClient, InventoryHttpClient>();
builder.Services.AddScoped<IProductHttpClient, ProductHttpClient>();
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
