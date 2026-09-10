using Microsoft.EntityFrameworkCore.Storage;
using PharmacySystem.Models;
using PharmacySystem.Models.DBModels;

namespace PharmacyManagement.Core.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IBaseRepository<Category> Categories { get; }
        IBaseRepository<Medicine> Medicines { get; }
        IBaseRepository<Supplier> Suppliers { get; }
        IBaseRepository<PurchaseOrder> PurchaseOrders { get; }
        IBaseRepository<PurchaseOrderItem> PurchaseOrderItems { get; }
        IBaseRepository<Sale> Sales { get; }
        IBaseRepository<SaleItem> SaleItems { get; }

        Task<int> CompleteAsync();
        Task<IDbContextTransaction> BeginTransactionAsync();
    }
}