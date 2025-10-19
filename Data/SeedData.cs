using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using CinemaApp.Models;
using Microsoft.AspNetCore.Identity;
using System;

namespace CinemaApp.Data
{
    public static class SeedData
    {
        public static async Task EnsureAdminExistsAsync(IServiceProvider services)
        {
            var db = services.GetRequiredService<ApplicationDbContext>();

            // تأكد إن المايجريشن مطبّق
            await db.Database.MigrateAsync();

            // هل في admin أصلاً؟
            var exists = await db.Users.AnyAsync(u => u.Role == "Admin");
            if (exists) return;

            // لو مفيش - انشئ حساب أدمين
            var admin = new User
            {
                Username = "admin",
                Email = "admin@cinema.local",
                Role = "Admin",
                CreatedAt = DateTime.UtcNow
            };

            // استخدم PasswordHasher عشان نولّد hash للباسورد
            var hasher = new PasswordHasher<User>();
            var hashed = hasher.HashPassword(admin, "Admin@123"); // <- هذا الباسورد الافتراضي
            admin.PasswordHash = hashed;

            db.Users.Add(admin);
            await db.SaveChangesAsync();
        }
    }
}