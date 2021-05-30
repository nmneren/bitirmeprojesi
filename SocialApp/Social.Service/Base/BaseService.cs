using Social.Core.Entity;
using Social.Core.Entity.Enums;
using Social.Core.Service;
using Social.Model.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Social.Service.Base
{
    public class BaseService<T> : ICoreService<T> where T : CoreEntity
    {
        private readonly SocialContext _context;

        public BaseService(SocialContext context)
        {
            _context = context;
        }

        public bool Add(T model)
        {
            try
            {
                _context.Set<T>().Add(model);
                return Save() > 0;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public bool Update(T model)
        {
            try
            {
                _context.Set<T>().Update(model);
                return Save() > 0;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public bool Delete(T model)
        {
            try
            {
                _context.Set<T>().Remove(model);
                return Save() > 0;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool Delete(int? id)
        {
            try
            {
                _context.Set<T>().Remove(_context.Set<T>().Find(id));
                return Save() > 0;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool Activate(int? id)
        {
            T model = GetById(id);
            model.Status = Status.Active;
            return Update(model);
        }

        public bool Passive(int? id)
        {
            T model = GetById(id);
            model.Status = Status.Passive;
            return Update(model);
        }

        public bool Any(Expression<Func<T, bool>> expression) => _context.Set<T>().Any(expression);
        public List<T> GetActive() => _context.Set<T>().Where(x => x.Status == Status.Active).ToList();
        public List<T> GetPassive() => _context.Set<T>().Where(x => x.Status == Status.Passive).ToList();
        public T GetByDefault(Expression<Func<T, bool>> exception) => _context.Set<T>().FirstOrDefault(exception);
        public T GetById(int? id) => _context.Set<T>().Find(id);
        public List<T> GetAll() => _context.Set<T>().ToList();
        public List<T> GetDefaultList(Expression<Func<T, bool>> expression) => _context.Set<T>().Where(expression).ToList();
        public int Save() => _context.SaveChanges();

        
    }
}
