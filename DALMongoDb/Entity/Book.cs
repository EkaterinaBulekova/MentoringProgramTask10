using DALMongoDb.Abstract;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace DALMongoDb.Entity
{
    public class Book : IEntity<ObjectId>
    {
        private const string Format = "|{0,-20}|{1,-10}|{2,5:d}|{3,-20}|{4,5:d}|";
        [BsonId] public ObjectId Id { get; set; }
        public override string ToString() => string.Format(Format, Name, Author, Count, string.Join(",", Genre), Year);

        [BsonElement] public string Name { get; set; }

        [BsonElement] public string Author { get; set; }

        [BsonElement] public int Count { get; set; }

        [BsonElement] public string[] Genre { get; set; }

        [BsonElement] public int Year { get; set; }
    }
}
