using System;
using System.Data.Entity;

namespace AnonymousBidder.Data.Infrastructure
{
    public interface IUnitOfWork : IDisposable
    {
        void Commit();
        
        AnonymousBidderDataContext Db { get; set; }
        
        void StartTransaction();
    }
}
