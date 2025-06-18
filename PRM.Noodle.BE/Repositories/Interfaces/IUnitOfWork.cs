using Repositories.Models;
using Repositories.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IUserRepository Users { get; }
        IGenericRepository<Product> Products { get; }
        IGenericRepository<Topping> Toppings { get; }
        IGenericRepository<Combo> Combos { get; }
        IGenericRepository<Order> Orders { get; }
        IGenericRepository<OrderItem> OrderItems { get; }
        IGenericRepository<OrderCombo> OrderCombos { get; }
        IGenericRepository<DailyRevenue> DailyRevenues { get; }

        int Complete();
        Task<int> CompleteAsync();
        Task BeginTransactionAsync();
        Task CommitTransactionAsync();
        Task RollbackTransactionAsync();
    }
}
