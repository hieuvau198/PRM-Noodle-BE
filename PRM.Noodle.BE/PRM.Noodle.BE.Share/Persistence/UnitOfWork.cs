using Microsoft.EntityFrameworkCore.Storage;
using PRM.Noodle.BE.Share.Data;
using PRM.Noodle.BE.Share.Interfaces;
using PRM.Noodle.BE.Share.Models;
using PRM.Noodle.BE.Share.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PRM.Noodle.BE.Share.Persistence
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly SpicyNoodleDbContext _context;

        private IDbContextTransaction _transaction;

        public IUserRepository Users { get; private set; }
        public IGenericRepository<Product> Products { get; private set; }
        public IGenericRepository<Topping> Toppings { get; private set; }
        public IGenericRepository<Combo> Combos { get; private set; }
        public IGenericRepository<Order> Orders { get; private set; }
        public IGenericRepository<OrderItem> OrderItems { get; private set; }
        public IGenericRepository<OrderCombo> OrderCombos { get; private set; }
        public IGenericRepository<DailyRevenue> DailyRevenues { get; private set; }
        public IGenericRepository<OrderItemTopping> OrderItemTopping { get; private set; }

        public UnitOfWork(SpicyNoodleDbContext context,
                          IUserRepository userRepository)
        {
            _context = context;

            Users = userRepository;
            Products = new GenericRepository<Product>(_context);
            Toppings = new GenericRepository<Topping>(_context);
            Combos = new GenericRepository<Combo>(_context);
            Orders = new GenericRepository<Order>(_context);
            OrderItems = new GenericRepository<OrderItem>(_context);
            OrderCombos = new GenericRepository<OrderCombo>(_context);
            OrderItemTopping = new GenericRepository<OrderItemTopping>(_context);
            DailyRevenues = new GenericRepository<DailyRevenue>(_context);
        }

        public int Complete()
        {
            return _context.SaveChanges();
        }

        public async Task<int> CompleteAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public async Task BeginTransactionAsync()
        {
            _transaction = await _context.Database.BeginTransactionAsync();
        }

        public async Task CommitTransactionAsync()
        {
            if (_transaction != null)
            {
                await _transaction.CommitAsync();
                await _transaction.DisposeAsync();
            }
        }

        public async Task RollbackTransactionAsync()
        {
            if (_transaction != null)
            {
                await _transaction.RollbackAsync();
                await _transaction.DisposeAsync();
            }
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
