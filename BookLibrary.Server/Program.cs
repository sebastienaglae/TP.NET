using System.Threading.RateLimiting;
using BookLibrary.Server;
using BookLibrary.Server.Database;
using BookLibrary.Server.Dtos;
using BookLibrary.Server.Services;
using BookLibrary.Server.Services.Plugins;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<LibraryDbContext>(options => options.UseInMemoryDatabase("LibraryDatabase"));
builder.Services.AddControllersWithViews().AddNewtonsoftJson().AddRazorRuntimeCompilation();
builder.Services.AddSwaggerDocument();

builder.Services.AddSingleton<DtoMapper>();
builder.Services.AddSingleton<OpenLibraryService>();
builder.Services.AddScoped<AuthenticationService>();

builder.Services.AddPlugins();
builder.Services.AddMemoryCache();

builder.Services.AddRateLimiter(rateLimiterOptions =>
{
    rateLimiterOptions.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
    rateLimiterOptions.AddFixedWindowLimiter("fixed-window", options =>
    {
        options.PermitLimit = 10;
        options.Window = System.TimeSpan.FromSeconds(30);
        options.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
        options.QueueLimit = 5;
    });
});

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Authentication/Login";
        options.LogoutPath = "/Authentication/Logout";
        options.AccessDeniedPath = "/Home/AccessDenied";
        options.ExpireTimeSpan = System.TimeSpan.FromDays(1d);
        options.Cookie.IsEssential = true;
    });

builder.Services.AddMvc();

WebApplication app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseStatusCodePagesWithReExecute("/Home/StatusCode", "?code={0}");
    
    app.UseHsts();
}
app.UseHttpsRedirection();
app.UseRateLimiter();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();
app.UseOpenApi();
app.UseSwaggerUi();
app.UsePlugins();
app.MapControllerRoute(
    "default",
    "{controller=Home}/{action=Index}/{id?}");
app.UseAuthentication();

using (IServiceScope scope = app.Services.CreateScope())
{
    DbInitializer.Initialize(scope.ServiceProvider.GetRequiredService<LibraryDbContext>());
}

app.Run();