using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.SqlClient;
using System.Linq.Expressions;

namespace Calibrus.ClearviewPortal.DataAccess.Repository
{
    public interface IRepository<T> where T : class
    {
        T Create(T entity);
        T Update(T entity);
        T Delete(T entity);
        List<T> Filter(Expression<Func<T, bool>> filter, params Expression<Func<T, object>>[] includes);
        List<T> Filter<TSort>(Expression<Func<T, bool>> filter, Expression<Func<T, TSort>> sort, SortOrder sortOrder, params Expression<Func<T, object>>[] includes);
        List<T> Filter<TSort>(Expression<Func<T, bool>> filter, out int total, Expression<Func<T, TSort>> sort, SortOrder sortOrder, int page = 1, int size = 50, params Expression<Func<T, object>>[] includes);
        List<T> All(params Expression<Func<T, object>>[] includes);
        List<T> All<TSort>(Expression<Func<T, TSort>> sort, SortOrder sortOrder, params Expression<Func<T, object>>[] includes);
        List<T> All<TSort>(out int total, Expression<Func<T, TSort>> sort, SortOrder sortOrder, int page = 1, int size = 50, params Expression<Func<T, object>>[] includes);
        T Find(int id);
        T Find(Expression<Func<T, bool>> filter, params Expression<Func<T, object>>[] includes);
        int Count();
        int Count(Expression<Func<T, bool>> filter);
    }
}
