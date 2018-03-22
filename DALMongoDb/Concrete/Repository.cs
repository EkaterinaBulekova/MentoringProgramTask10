using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using DALMongoDb.Abstract;
using MongoDB.Bson;
using MongoDB.Driver;

namespace DALMongoDb.Concrete
{
    public class Repository<T> : IRepository<T, ObjectId> where T : IEntity<ObjectId>
    {
        private readonly IMongoCollection<T> _collection;

        public Repository(DataContext context)
        {
            _collection = context.GetDatabase().GetCollection<T>(typeof(T).Name.ToLower());
        }

        private IQueryable<T> CreateSet()
        {
            return _collection.AsQueryable();
        }

        public T Insert(T instance)
        {
            try
            {
                instance.Id = ObjectId.GenerateNewId();
                _collection.InsertOne(instance);

                return instance;
            }
            catch (Exception ex)
            {
                Console.WriteLine(Message.CannotInsert);
                throw new DataException(Message.CannotInsert, ex);
            }
        }

        public void InsertMany(List<T> instances)
        {
            try
            {
                instances.ForEach(_=>_.Id = ObjectId.GenerateNewId());
                _collection.InsertMany(instances);
            }
            catch (Exception ex)
            {
                Console.WriteLine(Message.CannotInsert);
                throw new DataException(Message.CannotInsert, ex);
            }
        }

        public void Update(T instance)
        {
            try
            {
                var query = Builders<T>.Filter.Eq(o => o.Id, instance.Id);
                _collection.ReplaceOne(query, instance);
            }
            catch (Exception ex)
            {
                Console.WriteLine(Message.CannotUpdate);
                throw new DataException(Message.CannotUpdate, ex);
            }
        }

        public void UpdateMany<TProperty>(Expression<Func<T, bool>> condition, Expression<Func<T, TProperty>> predicate, TProperty value)
        {
            try
            {
                var update = Builders<T>.Update.Set(predicate, value);
                _collection.UpdateMany(condition, update);
            }
            catch (Exception ex)
            {
                Console.WriteLine(Message.CannotUpdate);
                throw new DataException(Message.CannotUpdate, ex);
            }
        }

        public void Delete(ObjectId id)
        {
            try
            {
                _collection.DeleteOne(Builders<T>.Filter.Eq(p => p.Id, id));
            }
            catch (Exception ex)
            {
                Console.WriteLine(Message.CannotDelete);
                throw new DataException(Message.CannotDelete, ex);
            }
        }

        public void DeleteMany(Expression<Func<T, bool>> condition = null)
        {
            var setIds = (condition != null) ? List(condition).Select(_ => _.Id) : List().Select(_ => _.Id);
            try
            {
                _collection.DeleteMany(Builders<T>.Filter.In(p => p.Id, setIds));
            }
            catch (Exception ex)
            {
                Console.WriteLine(Message.CannotDelete);
                throw new DataException(Message.CannotDelete, ex);
            }
        }

        public T GetById(ObjectId id)
        {
            return Single(o => o.Id == id);
        }

        public T Single(Expression<Func<T, bool>> predicate = null)
        {
            var set = CreateSet();
            var query = (predicate == null ? set : set.Where(predicate));

            return query.SingleOrDefault();
        }

        public IReadOnlyList<TProperty> ListPropertyDist<TProperty>(Expression<Func<T, TProperty>> predicate)
        {
            var set = CreateSet().Select(predicate).Where(_ => _ != null).Distinct();
            return set.ToList();
        }

        public IReadOnlyList<T> List(Expression<Func<T, bool>> condition = null, Func<T, string> order = null, int? take = null)
        {
            var set = CreateSet();
            if (condition != null)
            {
                set = set.Where(condition);
            }

            if (order != null)
            {
                return take == null 
                    ? set.AsEnumerable().OrderBy(order).ToList() 
                    : set.AsEnumerable().OrderBy(order).Take(Convert.ToInt32(take)).ToList();
            }

            return take == null ? set.ToList() : set.Take(Convert.ToInt32(take)).ToList();
        }

        public int Count(Expression<Func<T, bool>> predicate = null)
        {
            var set = CreateSet();

            return predicate == null ? set.Count() : set.Count(predicate);
        }

        

        public int Max(Expression<Func<T, int>> predicate)
        {
            var max = CreateSet().Max(predicate);
            return max;
        }

        public int Min(Expression<Func<T, int>> predicate)
        {
            var min = CreateSet().Min(predicate);
            return min;
        }

        public bool Exists(Expression<Func<T, bool>> predicate)
        {
            var set = CreateSet();
            return set.Any(predicate);
        }
    }
}
