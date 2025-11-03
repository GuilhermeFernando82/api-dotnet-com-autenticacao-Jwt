using crud_dotnet.Models;

namespace crud_dotnet.Interfaces
{
    public interface IOrderRepository
    {
        Task<IEnumerable<Order>> GetAllOrdersAsync();
        Task<Order?> GetOrderByIdAsync(int id);
        Task AddOrderAsync(Order order);
        void UpdateOrder(Order order);
        void DeleteOrder(Order order);
        Task<bool> SaveChangesAsync();
        Task<List<int>> GetExistingProductIdsAsync(List<int> productIds);
        Task<List<string>> GetProductCategoriesAsync(List<int> productIds);
    }
}