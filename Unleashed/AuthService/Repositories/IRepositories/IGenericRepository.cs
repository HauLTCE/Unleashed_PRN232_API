namespace AuthService.Repositories.IRepositories
{
    public interface IGenericRepository<Tid,T>
    {
        Task<IEnumerable<T>> All();
        Task<(IEnumerable<T> entities, int totalCount)> GetPagedAsync(
        int pageNumber,
        int pageSize,
        string? searchQuery);
        Task<T?> FindAsync(Tid id);
        Task<bool> IsAny(Tid id);
        bool Update(T entity);
        bool Delete(T entity);
        Task<bool> CreateAsync(T entity);
        Task<bool> SaveAsync();
    }
}
