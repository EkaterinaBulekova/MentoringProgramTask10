using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace DALMongoDb.Abstract
{
    public interface IRepository<T, in TId>
    {
        T Insert(T instance);

        void InsertMany(List<T> instances);

        void Update(T instance);

        void UpdateMany<TProperty>(Expression<Func<T, bool>> condition, 
            Expression<Func<T, TProperty>> predicate,
            TProperty value);

        void Delete(TId id);

        void DeleteMany(Expression<Func<T, bool>> predicate = null);

        T GetById(TId id);

        T Single(Expression<Func<T, bool>> predicate = null);

        IReadOnlyList<T> List(Expression<Func<T, bool>> condition = null, Func<T, string> order = null, int? take = null);

        int Count(Expression<Func<T, bool>> predicate = null);

        bool Exists(Expression<Func<T, bool>> predicate);

        int Max(Expression<Func<T, int>> predicate);

        int Min(Expression<Func<T, int>> predicate);

        IReadOnlyList<TProperty> ListPropertyDist<TProperty>(Expression<Func<T, TProperty>> predicate);
    }
}