using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PedalHub.Data;
using PedalHub.Areas.Identity.Data;
using Amazon.XRay.Recorder.Core;
using Amazon.XRay.Recorder.Handlers.AwsSdk;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("PedalHubContextConnection") ?? throw new InvalidOperationException("Connection string 'PedalHubContextConnection' not found.");

AWSSDKHandler.RegisterXRayForAllServices();

builder.Services.AddDbContext<PedalHubContext>(options => options.UseSqlServer(connectionString));

builder.Services.AddDefaultIdentity<PedalHubUser>(
    options => options.SignIn.RequireConfirmedAccount = true)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<PedalHubContext>();

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseXRay("PedalHub");
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();
