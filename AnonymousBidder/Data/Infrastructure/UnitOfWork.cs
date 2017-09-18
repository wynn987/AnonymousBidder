using System;
using System.Data.Entity;
using System.Transactions;
namespace AnonymousBidder.Data.Infrastructure
{
    public class UnitOfWork : IUnitOfWork
    {
        private TransactionScope _transaction;
        private AnonymousBidderDataContext _dataContext;

        public UnitOfWork()
        {
            _dataContext = new AnonymousBidderDataContext();
        }

        public UnitOfWork(AnonymousBidderDataContext context)
        {
            _dataContext = context;
        }

        public AnonymousBidderDataContext Db
        {
            get { return _dataContext; }
            set { _dataContext = value; }
        }

        public void Commit()
        {
            _dataContext.SaveChanges();

            if (_transaction != null)
                _transaction.Complete();
        }

        public void StartTransaction()
        {
            _transaction = new TransactionScope();
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposing)
            {
                return;
            }

            if (this._dataContext == null)
            {
                return;
            }

            this._dataContext.Dispose();
            this._dataContext = null;
        }
    }
}
