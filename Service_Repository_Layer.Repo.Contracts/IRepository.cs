namespace Service_Repository_Layer.Repo.Contracts
{
    public interface IRepository<T>
    {
        T GetById(int id);
        T Create(T obj);
        T Update(T obj);
        void Delete(int id);
    }
}
