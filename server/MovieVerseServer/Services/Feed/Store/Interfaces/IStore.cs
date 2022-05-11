using System.Threading.Tasks;

namespace Feed.Store.Interfaces
{
    public interface IStore<E>
    {
        Task<E> GetById(string id);
        Task<bool> CreateEntity(E entity);

        Task<bool> UpdateEntity(E entity);

        Task<bool> DeleteEntity(string id);
    }
}
