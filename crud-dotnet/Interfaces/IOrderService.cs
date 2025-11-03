using crud_dotnet.Models;

namespace crud_dotnet.Repository
{
    public interface IOrderService
    {
        Task<Order> CreateOrderAsync(CreateOrderDto dto, OrderStatus status);
    }
}