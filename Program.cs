using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using WebApplication2.Models;
using WebApplication2.Services;

namespace WebApplication2
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
          

            // Add services to the container.

            builder.Services.AddControllersWithViews();
            builder.Services.AddDbContext<LearningContext>(Options => Options.UseSqlServer(builder.Configuration.GetConnectionString("defaultconnection")));
            builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(option =>
            {
                option.ExpireTimeSpan = TimeSpan.FromMinutes(20);
                option.LoginPath = "/Account/Login";
                option.AccessDeniedPath = "/Account/Login";
            });

            builder.Services.AddSession(option =>
            {
                option.IdleTimeout = TimeSpan.FromMinutes(5);
                option.Cookie.HttpOnly = true;
                option.Cookie.IsEssential = true;
            });
            builder.Services.AddHttpClient<PaymentService>();
            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
            }
            
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseSession();
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
