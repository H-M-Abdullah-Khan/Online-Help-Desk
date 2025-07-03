//using Microsoft.EntityFrameworkCore;
//using Microsoft.Extensions.FileSystemGlobbing.Internal.Patterns;
//using Microsoft.Extensions.Options;
//using Online_Help_Desk.Models;

//var builder = WebApplication.CreateBuilder(args);
//builder.Services.AddMvc();
//builder.Services.AddSession();
//builder.Services.AddDbContext<ApplicationDbContext>(Options =>
//Options.UseSqlServer(builder.Configuration.GetConnectionString("mycon")));
//var app = builder.Build();

//app.MapGet("/", () => "Hello World!");
//app.MapControllerRoute(
//    name: "default",
//    pattern: "{Controller=Auth}/{action=login}"   
//    );

//app.UseStaticFiles();
//app.UseSession();
//app.Run();
using Microsoft.EntityFrameworkCore;
using Online_Help_Desk.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews(); // Use AddControllersWithViews for MVC
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Set session timeout
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("mycon")));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection(); // Optional, but good practice for security
app.UseStaticFiles();
app.UseRouting();
app.UseSession(); // Must be after UseRouting and before UseEndpoints

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Auth}/{action=Login}"
);

app.Run();
