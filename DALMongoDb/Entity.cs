namespace DALLinq2db.DataModels
{
    public abstract class EntityBase<T>
    {
        protected abstract T Key { get; }

        public override bool Equals(object obj)
        {
            return obj != null && (GetType() == obj.GetType() && Key.Equals(((EntityBase<T>)obj).Key));
        }

        public override int GetHashCode()
        {
            return Key.GetHashCode();
        }
    }
}
