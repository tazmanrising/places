using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using CalibrusTPV.Data.ScriptsDb;

namespace CalibrusTPV.Data.Repository
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly ScriptsContext _ctx;

        protected Repository(ScriptsContext ctx)
        {
            _ctx = ctx;
        }

        protected DbSet<T> DbSet => _ctx.Set<T>();

        public T Create(T entity)
        {
            return DbSet.Add(entity);
        }

        public T Update(T entity)
        {
            _ctx.Entry(entity).State = EntityState.Modified;
            return entity;
        }

        public T Delete(T entity)
        {
            DbSet.Remove(entity);
            return entity;
        }

        public List<T> Filter(Expression<Func<T, bool>> filter, params Expression<Func<T, object>>[] includes)
        {
            var db = DbSet.Where(filter);

            if (includes != null)
            {
                db = includes.Aggregate(db, (current, include) => current.Include(include));
            }

            return db.ToList();
        }

        public List<T> Filter<TSort>(Expression<Func<T, bool>> filter, Expression<Func<T, TSort>> sort, SortOrder sortOrder, params Expression<Func<T, object>>[] includes)
        {
            IQueryable<T> result = DbSet.Where(filter);
            result = (sortOrder == SortOrder.Ascending) ? result.OrderBy(sort) : result.OrderByDescending(sort);

            if (includes != null)
            {
                result = includes.Aggregate(result, (current, include) => current.Include(include));
            }

            return result.ToList();
        }

        public List<T> Filter<TSort>(Expression<Func<T, bool>> filter, out int total, Expression<Func<T, TSort>> sort, SortOrder sortOrder, int page = 1, int size = 50, params Expression<Func<T, object>>[] includes)
        {
            if (page < 1)
            {
                page = 1;
            }

            if (size > 250)
            {
                throw new ArgumentOutOfRangeException("size", size, "size can not be more than 250");
            }

            total = Count();

            IQueryable<T> result = DbSet.Where(filter);
            result = (sortOrder == SortOrder.Ascending) ? result.OrderBy(sort) : result.OrderByDescending(sort);
            result = result.Skip((page - 1) * size).Take(size);

            return result.ToList();
        }

        public List<T> All(params Expression<Func<T, object>>[] includes)
        {
            var db = DbSet.AsQueryable();

            if (includes != null)
            {
                db = includes.Aggregate(db, (current, include) => current.Include(include));
            }

            return db.ToList();
        }

        public List<T> All<TSort>(Expression<Func<T, TSort>> sort, SortOrder sortOrder, params Expression<Func<T, object>>[] includes)
        {
            var db = DbSet.AsQueryable();
            db = (sortOrder == SortOrder.Ascending) ? db.OrderBy(sort) : db.OrderByDescending(sort);

            if (includes != null)
            {
                db = includes.Aggregate(db, (current, include) => current.Include(include));
            }

            return db.ToList();
        }

        public List<T> All<TSort>(out int total, Expression<Func<T, TSort>> sort, SortOrder sortOrder, int page = 1, int size = 50, params Expression<Func<T, object>>[] includes)
        {
            if (page < 1)
            {
                page = 1;
            }

            if (size > 250)
            {
                throw new ArgumentOutOfRangeException("size", size, "size can not be more than 250");
            }

            total = Count();

            IQueryable<T> result = DbSet.AsQueryable();
            result = (sortOrder == SortOrder.Ascending) ? result.OrderBy(sort) : result.OrderByDescending(sort);
            result = result.Skip((page - 1) * size).Take(size);

            return result.ToList();
        }

        public T Find(int id)
        {
            return DbSet.Find(id);
        }

        public T Find(Expression<Func<T, bool>> filter, params Expression<Func<T, object>>[] includes)
        {
            var db = DbSet.AsQueryable();

            if (includes != null)
            {
                db = includes.Aggregate(db, (current, include) => current.Include(include));
            }

            return db.FirstOrDefault(filter);
        }

        public int Count()
        {
            return DbSet.Count();
        }

        public int Count(Expression<Func<T, bool>> filter)
        {
            return DbSet.Count(filter);
        }

    }

}
