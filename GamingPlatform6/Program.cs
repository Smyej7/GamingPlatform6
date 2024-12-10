using GamingPlatform6.Data;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;

//Program.cs===========================================================================
namespace GamingPlatform6
{
    public class Program
    {
        public static void Main(string[] args)
        {
            //=====Middleware and Services=============================================
            var builder = WebApplication.CreateBuilder(args);

            // Configure Entity Framework avec la base de données SQLite ou SQL Serverg
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            // Configure MVC
            builder.Services.AddControllersWithViews();

            //adding multi-language support
            AddingMultiLanguageSupportServices(builder);

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            //====App===================================================================
            var app = builder.Build();

            //adding multi-language support
            AddingMultiLanguageSupport(app);

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
            }
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=ChangeLanguage}/{id?}");

            app.Run();
        }

        private static void AddingMultiLanguageSupportServices(WebApplicationBuilder? builder)
        {
            if (builder == null) { throw new Exception("builder==null"); };

            builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");
            builder.Services.AddMvc()
                    .AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix);
            builder.Services.Configure<RequestLocalizationOptions>(options =>
            {
                var supportedCultures = new[] { "en", "fr" };
                options.SetDefaultCulture(supportedCultures[0])
                    .AddSupportedCultures(supportedCultures)
                    .AddSupportedUICultures(supportedCultures);
            });
        }

        private static void AddingMultiLanguageSupport(WebApplication? app)
        {
            app?.UseRequestLocalization();
        }
    }
}
