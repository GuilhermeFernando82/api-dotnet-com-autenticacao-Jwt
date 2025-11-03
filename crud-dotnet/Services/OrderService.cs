using crud_dotnet.Interfaces;
using crud_dotnet.Models;
using crud_dotnet.Repository;

public class OrderService : IOrderService
{
    private readonly IOrderRepository _repository;

    public OrderService(IOrderRepository repository)
    {
        _repository = repository;
    }

    public async Task<Order> CreateOrderAsync(CreateOrderDto dto, OrderStatus status)
    {
        var productIds = dto.Items.Select(i => i.ProductId).ToList();
        var existingProducts = await _repository.GetExistingProductIdsAsync(productIds);
        var missingProducts = productIds.Except(existingProducts).ToList();
        if (missingProducts.Any())
            throw new Exception($"Não existem produtos com esse id: {string.Join(", ", missingProducts)}");

        var productsFromDb = await _repository.GetProductsByIdsAsync(productIds);

        var orderItems = dto.Items.Select(i =>
        {
            var product = productsFromDb.First(p => p.Id == i.ProductId);
            return new OrderItem
            {
                ProductId = product.Id,
                Quantity = i.Quantity,
                UnitPrice = product.Price
            };
        }).ToList();

        decimal totalValue = orderItems.Sum(i => i.Quantity * i.UnitPrice);
        int totalQuantity = orderItems.Sum(i => i.Quantity);
        decimal discount = 0;

        if (totalQuantity >= 5) discount += 0.10m * totalValue;
        if (totalValue > 500) discount += 0.15m * totalValue;

        var categories = productsFromDb.Select(p => p.Category.ToString()).Distinct().ToList();
        if (categories.Contains("Books")) discount += 0.05m * totalValue;

        var order = new Order
        {
            Items = orderItems,
            DiscountValue = discount,
            Status = status
        };

        await _repository.AddOrderAsync(order);
        await _repository.SaveChangesAsync();

        return order;
    }
}
