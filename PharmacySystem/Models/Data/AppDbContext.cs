using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PharmacySystem.Interface;
using PharmacySystem.Models.DBModels;

namespace PharmacySystem.Models.Data
{
    public class AppDbContext : IdentityDbContext<ApplicationUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        // ================= DbSets =================
        public DbSet<Category> Categories { get; set; }
        public DbSet<Medicine> Medicines { get; set; }
        public DbSet<Supplier> Suppliers { get; set; }
        public DbSet<PurchaseOrder> PurchaseOrders { get; set; }
        public DbSet<PurchaseOrderItem> PurchaseOrderItems { get; set; }
        public DbSet<Sale> Sales { get; set; }
        public DbSet<SaleItem> SaleItems { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // 2. إضافة الأدوار الافتراضية
            var adminRoleId = "a182b8a0-2f22-49f3-8b1e-0d12e3456781";
            var pharmacistRoleId = "b273c9b1-3f33-40a4-9c2f-1e23f4567892";
            var cashierRoleId = "c384d0c2-4f44-51b5-ad30-2f34a5678903";

            modelBuilder.Entity<IdentityRole>().HasData(
                new IdentityRole
                {
                    Id = adminRoleId,
                    Name = "Admin",
                    NormalizedName = "ADMIN",
                    ConcurrencyStamp = "ac9965c6-60b5-481a-adfe-b0df2561251b"
                },
                new IdentityRole
                {
                    Id = pharmacistRoleId,
                    Name = "Pharmacist",
                    NormalizedName = "PHARMACIST",
                    ConcurrencyStamp = "8d769b15-2f7b-44c2-bae7-b8009de9565f"
                },
                new IdentityRole
                {
                    Id = cashierRoleId,
                    Name = "Cashier",
                    NormalizedName = "CASHIER",
                    ConcurrencyStamp = "fb37521b-7185-4103-840e-11a0163bea49"
                }
            );

            // إيقاف الحذف المتتالي كقاعدة عامة
            foreach (var relationship in modelBuilder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
            {
                relationship.DeleteBehavior = DeleteBehavior.Restrict;
            }

            // استثناء الـ Master-Details عشان يمسح البنود تلقائياً عند حذف الفاتورة
            modelBuilder.Entity<SaleItem>()
                .HasOne(si => si.Sale)
                .WithMany(s => s.SaleItems)
                .HasForeignKey(si => si.SaleId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<PurchaseOrderItem>()
                .HasOne(poi => poi.PurchaseOrder)
                .WithMany(po => po.PurchaseOrderItems)
                .HasForeignKey(poi => poi.PurchaseOrderId)
                .OnDelete(DeleteBehavior.Cascade);

            // ضبط الدقة للأرقام العشرية لتجنب تحذيرات SQL Server
            foreach (var property in modelBuilder.Model.GetEntityTypes()
                .SelectMany(t => t.GetProperties())
                .Where(p => p.ClrType == typeof(decimal) || p.ClrType == typeof(decimal?)))
            {
                property.SetPrecision(18);
                property.SetScale(2);
            }

            // تفعيل الـ Soft Delete Query Filter لأي Entity تطبق ISoftDelete
            modelBuilder.Entity<Medicine>().HasQueryFilter(m => !m.IsDeleted);
            modelBuilder.Entity<Supplier>().HasQueryFilter (m => !m.IsDeleted);
            modelBuilder.Entity<Category>().HasQueryFilter(m => !m.IsDeleted);

        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            ApplySoftDelete();
            return base.SaveChangesAsync(cancellationToken);
        }

        public override int SaveChanges()
        {
            ApplySoftDelete();
            return base.SaveChanges();
        }

        private void ApplySoftDelete()
        {
            // فحص أي الكيانات التي تطبق ISoftDelete ومطلوب حذفها
            foreach (var entry in ChangeTracker.Entries<ISoftDelete>())
            {
                if (entry.State == EntityState.Deleted)
                {
                    entry.State = EntityState.Modified;
                    entry.Entity.IsDeleted = true;
                }
            }
        }
    }
}