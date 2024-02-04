namespace ePizzaHub.Services.Interfaces
{
    public interface IService<TEntity> where TEntity : class
    {
        IEnumerable<TEntity> GetAll();
        TEntity Find(object id);
        void Add(TEntity entity);
        void Update(TEntity entity);
        void Delete(int id);
    }
}
