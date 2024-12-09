using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using PasswordManagerApplication.Data;
using PasswordManagerApplication.Models;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDistributedMemoryCache();

// Add session services
builder.Services.AddSession(options =>
{
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true; // This ensures session cookie is always included, even in GDPR-regulated environments
});

builder.Services.AddDbContext<ApplicationDbContext>(optiopns =>
               optiopns.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();



builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Password Manager API",
        Version = "v1",
        Description = "An API to manage user accounts and password entries"
    });
});

var app = builder.Build();

app.UseSwagger();

// Enable middleware to serve
// Swagger UI (HTML, JS, CSS, etc.),
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "Password Manager API V1");
    options.RoutePrefix = string.Empty; // To serve the Swagger UI at the root
});
// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseSession();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=AccountPage}/{id?}");

app.Run();
