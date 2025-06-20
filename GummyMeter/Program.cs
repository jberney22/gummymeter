using GummyMeter.Data;
using GummyMeter.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddHttpClient<TmdbService>();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.LogoutPath = "/Account/Logout";
    });

builder.Services.AddSession();

var app = builder.Build();

// Configure middleware
if (!app.Environment.IsDevelopment())
{
    app.Use(async (context, next) =>
    {
        var request = context.Request;

        if (request.Host.Host.StartsWith("www.") == false)
        {
            var wwwHost = new HostString("www." + request.Host.Host);
            var newUrl = Uri.UriSchemeHttps + "://" + wwwHost + request.Path + request.QueryString;
            context.Response.Redirect(newUrl, permanent: true);
            return;
        }
        await next();
    });
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Movies}/{action=Index}/{id?}");

app.Run();
