using AspNetCoreHero.ToastNotification;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ProjectThoiTrang.Models;
using ProjectThoiTrang.RequestModel;
using ProjectThoiTrang.Service;
using System;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages().AddRazorRuntimeCompilation();
builder.Services.AddControllersWithViews();
builder.Services.AddHttpClient();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddCookie(options =>
            {
                options.LoginPath = "/Account/Login";
                options.LogoutPath = "/Account/Logout";
            });
builder.Services.AddScoped<ServiceCart>();
builder.Services.AddSingleton<IVnPayService, VnPayService>();
// Configure Notyf for toast notifications
builder.Services.AddNotyf(config =>
{
    config.DurationInSeconds = 10;
    config.IsDismissable = true;
    config.Position = NotyfPosition.TopRight;
});
//connect database
builder.Services.AddDbContext<WebFashionContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("WebDataBase")
));

ConfigureServices(builder.Services);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}


app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();
app.MapGet("/cart_amount/{user_id:int}", (int user_id, ServiceCart service) =>
{
    return service.GetCartDeTails(user_id);
});
app.MapPost("/add_cart", async ([FromBody] AddCartRequest request, ServiceCart service) =>
{
    return await service.AddToCart(request.UserId, request.ProductId, request.Quantity);
});
app.MapPost("/delete_cart", async ([FromBody] DeleteCartRequest request, ServiceCart service) =>
{
    return await service.DeleteProduct(request.UserId, request.ProductId);
});


app.UseEndpoints(endpoints =>
{
    endpoints.MapRazorPages();    
});

app.Run();

void ConfigureServices(IServiceCollection services)
{
    services.AddRazorPages(options =>
    {
        options.Conventions.AddAreaPageRoute("Admin", "/AdminHome/Index", "/Admin/Home");
    });
}