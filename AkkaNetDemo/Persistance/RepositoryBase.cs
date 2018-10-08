using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

namespace AkkaNetDemo.Persistance
{
    public abstract class RepositoryBase<T> where T : class
    {
        private ProductContext _dataContext;
        private readonly DbSet<T> _dbset;


        protected RepositoryBase(IDatabaseFactory databaseFactory)
        {
            DatabaseFactory = databaseFactory;
            _dbset = DataContext.Set<T>();
        }

        protected IDatabaseFactory DatabaseFactory
        {
            get;
            private set;
        }
        protected ProductContext DataContext
        {
            get { return _dataContext ?? (_dataContext = DatabaseFactory.Get()); }
        }
        public IUnitOfWork UnitOfWork => DataContext;

        public virtual void Add(T entity)
        {
            if (entity == null) throw new ArgumentNullException("entity");
            _dbset.Add(entity);



        }

        public void Add(IEnumerable<T> entities)
        {
            if (entities == null) throw new ArgumentNullException("entities");
            foreach (var entity in entities)
                _dbset.Add(entity);

        }

        public virtual void Update(T entity)
        {
            if (entity == null) throw new ArgumentNullException("entity");
            _dbset.Attach(entity);
            _dataContext.Entry(entity).State = EntityState.Modified;
        }

        public void Update(IEnumerable<T> entities)
        {
            if (entities == null) throw new ArgumentNullException("entities");
            foreach (var dbObj in entities.Select(entity => _dataContext.Entry(entity)))
            {
                dbObj.State = EntityState.Modified;
            }

        }

        public virtual void Delete(T entity)
        {
            if (entity == null) throw new ArgumentNullException("entity");
            _dbset.Remove(entity);

        }

        public void Delete(int id)
        {
            var entity = _dbset.Find(id);
            if (entity == null) return;

            Delete(entity);
        }

        public void Delete(IEnumerable<T> entities)
        {
            if (entities == null) throw new ArgumentNullException("entities");
            foreach (var dbObj in entities.Select(entity => _dataContext.Entry(entity)))
            {
                dbObj.State = EntityState.Deleted;
            }
        }

        public void Delete(IEnumerable<int> ids)
        {
            if (ids == null) throw new ArgumentNullException("ids");
            var entities = ids.Select(id => _dbset.Find(id)).ToList();
            Delete(entities);
        }
        public virtual void Delete(Expression<Func<T, bool>> where)
        {
            IEnumerable<T> objects = _dbset.Where<T>(where).AsEnumerable();
            foreach (T obj in objects)
                _dbset.Remove(obj);
        }

        public virtual IQueryable<T> Get(
            Expression<Func<T, bool>> filter = null,
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
            string includeProperties = "")
        {
            IQueryable<T> query = _dbset;

            if (filter != null)
            {
                query = query.Where(filter);
            }
            //if (orderBy == null) throw new ArgumentNullException("orderBy");

            foreach (var includeProperty in includeProperties.Split
                (new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(includeProperty);
            }

            if (orderBy != null)
            {
                return orderBy(query);
            }
            else
            {
                return query;
            }
        }

        public virtual T GetById(long id)
        {
            return _dbset.Find(id);
        }

        public virtual T GetById(string id)
        {
            return _dbset.Find(id);
        }

        public virtual IQueryable<T> GetAll()
        {
            return _dbset;
        }

        public virtual IQueryable<T> GetMany(Expression<Func<T, bool>> where)
        {
            return _dbset.Where(where);
        }

        public T Get(Expression<Func<T, bool>> where)
        {
            return _dbset.Where(where).FirstOrDefault<T>();
        }

        public virtual IQueryable<T> GetWithRawSql(string query, params object[] parameters)
        {
            return _dbset.FromSql(query, parameters).AsQueryable();
        }

        public virtual void ExecuteWithRawSql(string query, params object[] parameters)
        {
            _dataContext.Database.ExecuteSqlCommand(query, parameters);
        }
    }
}