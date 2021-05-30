using Social.Core.Entity;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Social.Core.Service
{
    public interface ICoreService<T> where T : CoreEntity
    {
        bool Add(T model);
        bool Update(T model);
        bool Delete(T model);
        bool Delete(int? id);
        bool Activate(int? id);
        bool Passive(int? id);
        T GetById(int? id);
        T GetByDefault(Expression<Func<T, bool>> exception); // x=> x.name == "numan"
        List<T> GetAll();
        List<T> GetDefaultList(Expression<Func<T, bool>> expression);
        bool Any(Expression<Func<T, bool>> expression);
        List<T> GetActive();
        List<T> GetPassive();
        int Save();
    }
}
