using Microsoft.EntityFrameworkCore;
using StockControlProject.Entities.Entities;
using StockControlProject.Repository.Abstract;
using StockControlProject.Repository.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace StockControlProject.Repository.Concrete
{
    public class GenericRepository<T> : IGenericRepository<T> where T : BaseEntity
    {
        private readonly StockControlContext context;

        public GenericRepository(StockControlContext _context)
        {
            context = _context;
        }

        public bool Activate(int id)
        {
            T item = GetById(id);
            item.IsActive = true;
            return Update(item);
        }

        public bool Add(T entity)
        {
            try
            {
                context.Set<T>().Add(entity);
                return Save() > 0;
            }
            catch 
            {
                return false;
            }
        }

        public bool Add(List<T> entities)
        {
            try
            {
                using (TransactionScope ts = new TransactionScope())
                {
                    foreach(var item in entities)
                    {
                        context.Set<T>().Add(item);
                    }
                    ts.Complete();
                    return Save() > 0;
                }
            }
            catch (Exception)
            {

                return false;
            }
        }

        public bool Any(Expression<Func<T, bool>> exp) => context.Set<T>().Any(exp);

        public List<T> GetActive()=>context.Set<T>().Where(x => x.IsActive==true).ToList();

        public IQueryable<T> GetActive(params Expression<Func<T, object>>[] includes)
        {
            var query=context.Set<T>().Where(x=>x.IsActive==true);
            if(includes != null) 
            {
                query = includes.Aggregate(query, (current, include) => current.Include(include));
            }
            return query;
        }

        public List<T> GetAll()=>context.Set<T>().ToList();

        public IQueryable<T> GetAll(params Expression<Func<T, object>>[] includes)
        {
            var query = context.Set<T>().AsQueryable();
            if(includes != null)
            {
                query=includes.Aggregate(query,(current,include)=>current.Include(include));
            }
            return query;
        }

        public IQueryable<T> GetAll(Expression<Func<T, bool>> exp, params Expression<Func<T, object>>[] includes)
        {
            var query = context.Set<T>().Where(exp);
            if(includes != null)
            {
                query=includes.Aggregate(query,(current, include)=>current.Include(include));
            }
            return query;
        }

        public T GetByDefault(Expression<Func<T, bool>> exp)=>context.Set<T>().FirstOrDefault(exp);

        public T GetById(int id) => context.Set<T>().Find(id);

        public IQueryable<T> GetById(int id, params Expression<Func<T, object>>[] includes)
        {
            var query = context.Set<T>().Where(x=>x.Id==id);
            if(includes!= null)
            {
                query=includes.Aggregate(query,(current,includes)=>current.Include(includes));
            }
            return query;
        }

        public List<T> GetDefault(Expression<Func<T, bool>> expression)=>context.Set<T>().Where(expression).ToList();

        public bool Remove(T entity)
        {
            entity.IsActive = false;
            return Update(entity);
        }

        public bool Remove(int id)
        {
            try
            {
                using(TransactionScope ts = new TransactionScope())
                {
                    T item = GetById(id);
                    item.IsActive = false;
                    ts.Complete();
                    return Update(item);
                }
            }
            catch (Exception)
            {

                return false;
            }
        }

        public bool RemoveAll(Expression<Func<T, bool>> exp)
        {
            try
            {
                using(TransactionScope ts = new TransactionScope())
                {
                    int count = 0;
                    var collection = GetDefault(exp);//verilen şarta uyanları collectiona attık
                    foreach(var item in collection)
                    {
                        item.IsActive = false;
                        bool result = Update(item);
                        if (result) count++;
                    }
                    if (collection.Count() == count) ts.Complete();
                    else return false;
                }
                return true;
            }
            catch(Exception)
            {
                return false;
            }
        }

        public int Save()
        {
            return context.SaveChanges();
        }

        public bool Update(T entity)
        {
            try
            {
                entity.ModifiedDate = DateTime.Now;
                context.Set<T>().Update(entity);
                return Save() > 0;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
