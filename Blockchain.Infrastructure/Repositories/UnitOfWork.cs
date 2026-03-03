using Blockchain.Domain.Entities;
using Blockchain.Domain.Interfaces;
using Blockchain.Infrastructure.Data;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blockchain.Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;
        private IDbContextTransaction? _transaction;

        public IGenericRepository<Wallet> Wallets { get; }
        public IGenericRepository<Transaction> Transactions { get; }
        public IGenericRepository<Block> Blocks { get; }
        public IGenericRepository<SmartContract> SmartContracts { get; }
        public IGenericRepository<ContractInteraction> ContractInteractions { get; }
        public IGenericRepository<AuditLog> AuditLogs { get; }
        public IGenericRepository<Notification> Notifications { get; }

        public UnitOfWork(AppDbContext context)
        {
            _context = context;
            Wallets = new GenericRepository<Wallet>(context);
            Transactions = new GenericRepository<Transaction>(context);
            Blocks = new GenericRepository<Block>(context);
            SmartContracts = new GenericRepository<SmartContract>(context);
            ContractInteractions = new GenericRepository<ContractInteraction>(context);
            AuditLogs = new GenericRepository<AuditLog>(context);
            Notifications = new GenericRepository<Notification>(context);
        }

        public async Task<int> SaveChangesAsync(CancellationToken ct = default)
            => await _context.SaveChangesAsync(ct);

        public async Task BeginTransactionAsync(CancellationToken ct = default)
            => _transaction = await _context.Database.BeginTransactionAsync(ct);

        public async Task CommitTransactionAsync(CancellationToken ct = default)
        {
            if (_transaction != null)
            {
                await _transaction.CommitAsync(ct);
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }

        public async Task RollbackTransactionAsync(CancellationToken ct = default)
        {
            if (_transaction != null)
            {
                await _transaction.RollbackAsync(ct);
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }

        public void Dispose()
        {
            _transaction?.Dispose();
            _context.Dispose();
        }
    }
}
