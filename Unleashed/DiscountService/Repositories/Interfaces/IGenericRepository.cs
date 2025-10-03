namespace DiscountService.Repositories.Interfaces
{
    public interface IGenericRepository<Tid, T>
    {
        IQueryable<T> All();
        Task<T?> FindAsync(Tid id);
        Task<bool> IsAny(Tid id);
        bool Update(T entity);
        bool Delete(T entity);
        Task<bool> CreateAsync(T entity);
        Task<bool> SaveAsync();
    }
}
