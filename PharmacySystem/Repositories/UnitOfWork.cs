using Microsoft.EntityFrameworkCore.Storage;
using PharmacySystem.Models;
using PharmacySystem.Models.DBModels;
using PharmacySystem.Interface;
using PharmacySystem.Models.Data;
using PharmacyManagement.Core.Interfaces;

namespace PharmacyManagement.Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;

        public IBaseRepository<Category> Categories { get; private set; }
        public IBaseRepository<Medicine> Medicines { get; private set; }
        public IBaseRepository<Supplier> Suppliers { get; private set; }
        public IBaseRepository<PurchaseOrder> PurchaseOrders { get; private set; }
        public IBaseRepository<PurchaseOrderItem> PurchaseOrderItems { get; private set; }
        public IBaseRepository<Sale> Sales { get; private set; }
        public IBaseRepository<SaleItem> SaleItems { get; private set; }

        public UnitOfWork(AppDbContext context)
        {
            _context = context;

            Categories = new BaseRepository<Category>(_context);
            Medicines = new BaseRepository<Medicine>(_context);
            Suppliers = new BaseRepository<Supplier>(_context);
            PurchaseOrders = new BaseRepository<PurchaseOrder>(_context);
            PurchaseOrderItems = new BaseRepository<PurchaseOrderItem>(_context);
            Sales = new BaseRepository<Sale>(_context);
            SaleItems = new BaseRepository<SaleItem>(_context);
        }

        public async Task<int> CompleteAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public async Task<IDbContextTransaction> BeginTransactionAsync()
        {
            return await _context.Database.BeginTransactionAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}