using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using App.Models;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.AspNetCore.Identity.UI.Services;
using App.Services;
using App.Security.Requirements;
using Microsoft.AspNetCore.Authorization;

namespace App
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

            services.AddOptions();
            var mailSetting = Configuration.GetSection("MailSettings");
            services.Configure<MailSettings>(mailSetting);
            services.AddSingleton<IEmailSender, SendMailService>();

            services.AddRazorPages();
            services.AddDbContext<AppDbContext>(options =>
            {
                string connectionString = Configuration.GetConnectionString("AppDbConnection");
                options.UseSqlServer(connectionString);
            });
            // Dang ky identity
            services.AddIdentity<AppUser, IdentityRole>()
                    .AddEntityFrameworkStores<AppDbContext>()
                    .AddDefaultTokenProviders();
            // services.AddDefaultIdentity<AppUser>()
            //         .AddEntityFrameworkStores<AppDbContext>()
            //         .AddDefaultTokenProviders();

            services.Configure<IdentityOptions>(options =>
            {
                // Thiết lập về Password
                options.Password.RequireDigit = false; // Không bắt phải có số
                options.Password.RequireLowercase = false; // Không bắt phải có chữ thường
                options.Password.RequireNonAlphanumeric = false; // Không bắt ký tự đặc biệt
                options.Password.RequireUppercase = false; // Không bắt buộc chữ in
                options.Password.RequiredLength = 3; // Số ký tự tối thiểu của password
                options.Password.RequiredUniqueChars = 1; // Số ký tự riêng biệt

                // Cấu hình Lockout - khóa user
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5); // Khóa 5 phút
                options.Lockout.MaxFailedAccessAttempts = 3; // Thất bại 5 lầ thì khóa
                options.Lockout.AllowedForNewUsers = true;

                // Cấu hình về User.
                options.User.AllowedUserNameCharacters = // các ký tự đặt tên user
                    "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
                options.User.RequireUniqueEmail = true;  // Email là duy nhất

                // Cấu hình đăng nhập.
                options.SignIn.RequireConfirmedEmail = true;            // Cấu hình xác thực địa chỉ email (email phải tồn tại)
                options.SignIn.RequireConfirmedPhoneNumber = false;     // Xác thực số điện thoại

                options.SignIn.RequireConfirmedAccount = true;

            });

            services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = "/login";
                options.LogoutPath = "/logout";
                options.AccessDeniedPath = "/AccessDenied";
            });

            services.AddAuthentication()
                    .AddGoogle(options =>
                    {
                        var gConfig = Configuration.GetSection("Google");
                        options.ClientId = gConfig["ClientId"];
                        options.ClientSecret = gConfig["ClientSecret"]; ;
                        options.CallbackPath = "/google-login";//default signin-google
                    })
                    .AddFacebook(options =>
                    {
                        var fConfig = Configuration.GetSection("Facebook");
                        options.ClientId = fConfig["ClientId"];
                        options.ClientSecret = fConfig["ClientSecret"];
                        options.CallbackPath = "/facebook-login";
                    });
            services.AddSingleton<IdentityErrorDescriber, AppIdentityErrorDescriber>();

            services.AddAuthorization(options =>
            {
                options.AddPolicy("AllowEditRole", policyBuilder =>
                {
                    policyBuilder.RequireAuthenticatedUser();
                    // policyBuilder.RequireRole("Admin");
                    // policyBuilder.RequireRole("Editor");

                    policyBuilder.RequireClaim("canedit", "role");
                });

                options.AddPolicy("InGenz", policyBuilder =>
                {
                    policyBuilder.RequireAuthenticatedUser();

                    policyBuilder.Requirements.Add(new GenzRequirement());
                });

                options.AddPolicy("ShowAdminMenu", policyBuilder =>
                {
                    policyBuilder.RequireRole("Admin");
                });

                options.AddPolicy("CanUpdateArticle", policyBuilder =>
                {
                    policyBuilder.Requirements.Add(new ArticleUpdateRequirement());
                });
            });

            services.AddTransient<IAuthorizationHandler, AppAuthorizationHandler>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
            });


        }
    }
}

/*

dotnet aspnet-codegenerator identity -dc RazorWebApp.Models.AppDbContext

dotnet new page -n Index -o /Areas/Admin/Pages/Role/ -na RazorWebApp.Admin.Role

*/  