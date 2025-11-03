using crud_dotnet.Interfaces;
using crud_dotnet.Models;
using Microsoft.EntityFrameworkCore;

namespace crud_dotnet.Repository
{
    public class OrderRepository : IOrderRepository
    {
        private readonly AppDbContext _context;

        public OrderRepository(AppDbContext context)
        {
            _context = context;
        }
        public async Task<List<int>> GetExistingProductIdsAsync(List<int> productIds)
        {
            return await _context.Products
                .Where(p => productIds.Contains(p.Id))
                .Select(p => p.Id)
                .ToListAsync();
        }

        public async Task<List<Product>> GetProductsByIdsAsync(List<int> productIds)
        {
            return await _context.Products
            .Where(p => productIds.Contains(p.Id))
            .ToListAsync();
        }
        public async Task<List<string>> GetProductCategoriesAsync(List<int> productIds)
        {
            if (productIds == null || !productIds.Any())
                return new List<string>();

            return await _context.Products
                .Where(p => productIds.Contains(p.Id))
                .Select(p => p.Category.ToString())
                .Distinct()
                .ToListAsync();
        }
        public async Task<IEnumerable<Order>> GetAllOrdersAsync()
        {
            return await _context.Orders
                .Include(o => o.Items!)
                    .ThenInclude(i => i.Product)
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();
        }
        public async Task<Order?> GetOrderByIdAsync(int id)
        {
            return await _context.Orders
                .Include(o => o.Items!)
                    .ThenInclude(i => i.Product)
                .FirstOrDefaultAsync(o => o.Id == id);
        }
        public async Task AddOrderAsync(Order order)
        {
            order.TotalValue = order.Items?.Sum(i => i.UnitPrice * i.Quantity) ?? 0;

            order.CreatedAt = DateTime.UtcNow;

            if (order.Items != null)
            {
                foreach (var item in order.Items)
                {
                    item.Order = null!;
                }
            }

            await _context.Orders.AddAsync(order);
        }
        public void UpdateOrder(Order order)
        {
            _context.Orders.Update(order);
        }

        public void DeleteOrder(Order order)
        {
            _context.Orders.Remove(order);
        }

        public async Task<bool> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
