using LCDSArtGallery.DataAccess.Repository;
using LCDSArtGallery.DataAccess.Repository.IRepository;
using LCDSArtGallery.Utility;
using LCDSArtGalleryWeb.DataAccess.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Stripe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LCDSArtGalleryWeb
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();
            services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(
                Configuration.GetConnectionString("DefaultConnection")));
            //DS: mapping for Stripe keys & properties from StripeSettings
            //DS: this will go to Stripe section from appsettings.json, will bind the values of the keys
            //to the properties from StripeSettings class
            services.Configure<StripeSettings>(Configuration.GetSection("Stripe"));
            services.AddIdentity<IdentityUser, IdentityRole>(options =>
            {
                options.SignIn.RequireConfirmedAccount = false;

                options.User.RequireUniqueEmail = true;

                options.Lockout.AllowedForNewUsers = true;

                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(10);

                options.Lockout.MaxFailedAccessAttempts = 3;
            })
                .AddDefaultTokenProviders()

                .AddEntityFrameworkStores<ApplicationDbContext>();
           services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = $"/Identity/Account/Login";
                options.LogoutPath = $"/Identity/Account/Logout";
                options.AccessDeniedPath = $"/Identity/Account/AccessDenied";
            });
            services.AddDistributedMemoryCache();
            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(100);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });
            services.AddSingleton<IEmailSender, EmailSender>();
            // below line is used for mapping stripe keys with the properties from the class StripeSettings
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddRazorPages();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            //DS: Configure the HTTP request pipeline.
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

           //DS: stripe config at the global level
           //DS: we assign the value of the secret key to the api key
           //DS: the code after the = sign looks for the secret key inside section "Stripe" from appsettings.json
            StripeConfiguration.ApiKey = Configuration.GetSection("Stripe:SecretKey").Get<string>();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{area=Customer}/{controller=Home}/{action=Index}/{id?}");
                endpoints.MapRazorPages();
            });
        }
    }
}
