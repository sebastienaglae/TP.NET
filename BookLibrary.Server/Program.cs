using BookLibrary.Server;
using BookLibrary.Server.Database;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<LibraryDbContext>(options => options.UseInMemoryDatabase("LibraryDatabase"));
builder.Services.AddControllersWithViews().AddNewtonsoftJson().AddRazorRuntimeCompilation();
builder.Services.AddSwaggerDocument();

builder.Services.AddAutoMapper(typeof(MappingProfile));

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
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();
app.UseOpenApi();
app.UseSwaggerUi();
app.MapControllerRoute(
    "default",
    "{controller=Home}/{action=Index}/{id?}");

using (IServiceScope scope = app.Services.CreateScope())
{
    DbInitializer.Initialize(scope.ServiceProvider.GetRequiredService<LibraryDbContext>());
}

app.Run();