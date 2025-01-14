using Microsoft.EntityFrameworkCore;
using myShop.Entities.Models;
using myShop.DataAccess.Data;
using myShop.Entities.Repositories;
using myShop.DataAccess.RepositoriesImplementation;
using Microsoft.AspNetCore.Identity;
using myShop.Utilities;
using Microsoft.AspNetCore.Identity.UI.Services;
using Stripe;

namespace myShop.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();
            builder.Services.AddRazorPages().AddRazorRuntimeCompilation();
            builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(
            builder.Configuration.GetConnectionString("DefaultConnection")
            ));
			builder.Services.Configure<StripeData>(builder.Configuration.GetSection("stripe"));

			builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
            options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromHours(24))
                .AddDefaultTokenProviders().AddDefaultUI()
                 .AddEntityFrameworkStores<ApplicationDbContext>();
            builder.Services.AddSingleton<IEmailSender, EmailSender>();
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
            }
            app.UseStaticFiles();

            app.UseRouting();
			StripeConfiguration.ApiKey = builder.Configuration.GetSection("stripe:Secretkey").Get<string>();


			app.UseAuthorization();
            app.MapRazorPages();

            app.MapControllerRoute(
                name: "default",
                pattern: "{area=Admin}/{controller=Home}/{action=Index}/{id?}");

            app.MapControllerRoute(
                name: "Customer",
                pattern: "{area=Customer}/{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}