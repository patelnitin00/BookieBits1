
using BookieBits.DataAccess;
using BookieBits.DataAccess.Repository;
using BookieBits.DataAccess.Repository.IRepository;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using BookieBits.Utility;
using Stripe;

var builder = WebApplication.CreateBuilder(args);

//auto added
//map default Identity with entity framework
//We will use Default IDentity for identity purpose
//options - only signin if email confirmed  - we will remove it for now

/*builder.Services.AddDefaultIdentity<IdentityUser>()
 .AddEntityFrameworkStores<ApplicationDbContext>();*/

builder.Services.AddIdentity<IdentityUser, IdentityRole>().AddDefaultTokenProviders()
    .AddEntityFrameworkStores<ApplicationDbContext>();


// Add services to the container.
builder.Services.AddControllersWithViews();

//my defined services

//1. AddDB Context 
builder.Services.AddDbContext<ApplicationDbContext>(options=> options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")
    ));

//to configure stripe - we will use same builder.config class
builder.Services.Configure<StripeSettings>(builder.Configuration.GetSection("Stripe"));




//2.for Razor page compilation
//builder.Services.AddRazorPages();

//3.We need implemetion of ICategoryRepository so we need to register it
//We need to define lifetime (Singleton, Scoped, Transient)
//for database we use Scoped
//builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();

//4. We no longer need individual repositories - we will use UnitOfWork
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddSingleton<IEmailSender, EmailSender>();
builder.Services.AddRazorPages();

//custom routes to redirect to pages
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = $"/Identity/Account/Login";
    options.LogoutPath = $"/Identity/Account/Logout";
    options.AccessDeniedPath = $"/Identity/Account/AccessDenied";
});


var app = builder.Build();

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

StripeConfiguration.ApiKey = builder.Configuration.GetSection("Stripe:SecretKey").Get<string>();

app.UseAuthentication();

app.UseAuthorization();

app.MapRazorPages();

app.MapControllerRoute(
    name: "default",
    pattern: "{area=Customer}/{controller=Home}/{action=Index}/{id?}");

app.Run();
