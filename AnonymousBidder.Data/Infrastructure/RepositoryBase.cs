using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using AnonymousBidder.Data.Infrastructure;

namespace AnonymousBidder.Data.Infrastructure
{
    public abstract class RepositoryBase<T> where T : class
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IDbSet<T> dbSet;

        protected RepositoryBase(IUnitOfWork unitOfWork)
        {
            if (unitOfWork == null) throw new ArgumentNullException("unitOfWork");
            _unitOfWork = unitOfWork;
            this.dbSet = _unitOfWork.Db.Set<T>();
        }
        public IUnitOfWork UnitOfWork { get { return _unitOfWork; } }
        internal DbContext Database { get { return _unitOfWork.Db; } }

        #region Implementation For Generic Repository
        public virtual int GetNewID()
        {
            return 1;
        }

        public IEnumerable<T> FindBy(Expression<Func<T, bool>> predicate)
        {
            IEnumerable<T> query = dbSet.Where(predicate).AsEnumerable();
            return query;
        }

        public virtual T Add(T entity)
        {
            return dbSet.Add(entity);
        }

        public virtual void Update(T entity)
        {
            dbSet.Attach(entity);
            _unitOfWork.Db.Entry(entity).State = EntityState.Modified;
        }

        public virtual bool Delete(int id)
        {
            try
            {
                var entity = GetById(id);
                if (entity != null)
                    dbSet.Remove(entity);

                return true;
            }
            catch
            {
                return false;
            }

        }
        public virtual bool Delete(Guid id)
        {
            try
            {
                var entity = GetById(id);
                if (entity != null)
                    dbSet.Remove(entity);

                return true;
            }
            catch
            {
                return false;
            }

        }

        public virtual bool Delete(T entity)
        {
            try
            {
                if (_unitOfWork.Db.Entry(entity).State == EntityState.Detached)
                {
                    dbSet.Attach(entity);
                }
                dynamic obj = dbSet.Remove(entity);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public virtual bool Delete(Expression<Func<T, bool>> where)
        {
            try
            {
                IEnumerable<T> objects = dbSet.Where<T>(where).AsEnumerable();
                foreach (T obj in objects)
                    dbSet.Remove(obj);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public virtual T GetById(int id)
        {
            return dbSet.Find(id);
        }
        public virtual T GetById(Guid id)
        {
            return dbSet.Find(id);
        }
        public virtual IEnumerable<T> GetAll()
        {
            return dbSet.ToList();
        }

        public virtual IEnumerable<T> GetMany(Expression<Func<T, bool>> where)
        {
            return dbSet.Where(where).ToList();
        }

        public T Get(Expression<Func<T, bool>> where)
        {
            return dbSet.Where(where).FirstOrDefault<T>();
        }

        #endregion
    }
}
