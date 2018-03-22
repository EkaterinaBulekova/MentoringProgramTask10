namespace DALMongoDb.Abstract
{
    public interface IEntity<T>
    {
        T Id { get; set; }
        string ToString();
    }
}