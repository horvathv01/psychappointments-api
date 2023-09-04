namespace PsychAppointments_API.DAL;

public interface IRepository<T>
{
    Task<T> GetById(long id);
    Task<T> GetByEmail(string email); //implementation is optional based on entity type
    Task<IEnumerable<T>> GetAll();
    Task<bool> Add(T entity);
    Task<bool> Update(long id, T entity);
    Task<bool> Delete(long id);
}