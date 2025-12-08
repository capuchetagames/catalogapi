using Core.Entity;

namespace Core.Repository;

public interface IRepository<T> where T : EntityBase
{
    IList<T> GetAll();
    
    T GetById(int id);
    
    void Add(T entity);
    
    void Update(T entity);
    
    void Delete(int id);
}