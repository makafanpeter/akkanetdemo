using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace AkkaNetDemo.Persistance
{
    public interface IRepository<T> where T : IAggregateRoot
    {
        IUnitOfWork UnitOfWork { get; }


        void Add(T entity);
        void Add(IEnumerable<T> entities);
        void Update(T entity);
        void Update(IEnumerable<T> entities);
        void Delete(T entity);
        void Delete(Expression<Func<T, bool>> where);
        void Delete(IEnumerable<T> entities);
        void Delete(int id);
        IQueryable<T> Get(Expression<Func<T, bool>> filter = null,
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
            string includeProperties = "");
        T GetById(long id);
        T GetById(string id);
        T Get(Expression<Func<T, bool>> where);
        IQueryable<T> GetAll();
        IQueryable<T> GetMany(Expression<Func<T, bool>> where);

        IQueryable<T> GetWithRawSql(string query, params object[] parameters);

        void ExecuteWithRawSql(string query, object[] parameters);
    }
}