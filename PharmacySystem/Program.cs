using Microsoft.AspNetCore.Identity; // تأكد من وجود هذا الـ directive
using Microsoft.EntityFrameworkCore;
using PharmacyManagement.Core.Interfaces;
using PharmacyManagement.Infrastructure.Repositories;
using PharmacySystem.Models.Data;

using Microsoft.AspNetCore.Identity.UI.Services;
using PharmacySystem.Services;

namespace PharmacySystem
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();
            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            builder.Services.AddScoped(typeof(IBaseRepository<>), typeof(BaseRepository<>));
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

            // --- إضافة خدمة إرسال الإيميلات ---
            builder.Services.AddTransient<IEmailSender,EmailSender>();

            // --- إعدادات Identity مع تفعيل تأكيد الإيميل ---
            builder.Services.AddDefaultIdentity<ApplicationUser>(options => {
                options.SignIn.RequireConfirmedAccount = false; // <-- غيرناها هنا لـ true
                options.Password.RequireDigit = true;
                options.Password.RequiredLength = 6;
                options.Password.RequireNonAlphanumeric = false;
                options.Tokens.EmailConfirmationTokenProvider = TokenOptions.DefaultEmailProvider;
            })
            .AddRoles<IdentityRole>()
            .AddEntityFrameworkStores<AppDbContext>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }
            app.UseStaticFiles();
            app.UseHttpsRedirection();
            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();
            app.UseStaticFiles();
            app.MapStaticAssets();
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}")
                .WithStaticAssets();

            app.MapRazorPages();
            app.Run();
        }
    }
}