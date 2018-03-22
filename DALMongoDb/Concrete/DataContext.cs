using System;
using MongoDB.Driver;

namespace DALMongoDb.Concrete
{
    public class DataContext : IDisposable
    {
        private readonly string _mongoDbName;
        MongoClient _client;

        public DataContext(string dburl, string dbname)
        {
            _mongoDbName = dbname;
            _client = new MongoClient(dburl);
        }

        public IMongoDatabase GetDatabase() { return _client.GetDatabase(_mongoDbName); }

        public void DropDatabase(string dbName)
        {
            _client.DropDatabase(dbName);
        }

        public void DropCollection<T>()
        {
            var database = GetDatabase();
            var collectionName = typeof(T).Name.ToLower();
            database.DropCollection(collectionName);
        }

        public void Dispose()
        {
            _client = null;
        }
    }
}