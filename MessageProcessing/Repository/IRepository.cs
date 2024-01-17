namespace MessageProcessing.Repository
{
    public interface IRepository<T>
    {
        Task SaveAsync(T data);
    }
}