using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Library.Infrastructure.Interfaces
{
    public interface IRepository<T> where T : class
    {
        T GetById(int id);
        IEnumerable<T> ListAll();
        IEnumerable<T> List(Expression<Func<T, bool>> criteria);
        T Add(T entity);
        T Update(T entity);
        void Delete(T entity);
    }
}
