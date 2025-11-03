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

    public async Task<Order> CreateOrderAsync(CreateOrderDto dto)
    {
        // Validar produtos
        var productIds = dto.Items.Select(i => i.ProductId).ToList();
        var existingProducts = await _repository.GetExistingProductIdsAsync(productIds);
        var missingProducts = productIds.Except(existingProducts).ToList();
        if (missingProducts.Any())
            throw new Exception($"Produtos não existem: {string.Join(", ", missingProducts)}");

        // Criar itens
        var orderItems = dto.Items.Select(i => new OrderItem
        {
            ProductId = i.ProductId,
            Quantity = i.Quantity,
            UnitPrice = i.UnitPrice
        }).ToList();

        // Total e desconto
        decimal totalValue = orderItems.Sum(i => i.Quantity * i.UnitPrice);
        int totalQuantity = orderItems.Sum(i => i.Quantity);
        decimal discount = 0;

        if (totalQuantity >= 5) discount += 0.10m * totalValue;
        if (totalValue > 500) discount += 0.15m * totalValue;

        var categories = await _repository.GetProductCategoriesAsync(productIds);
        if (categories.Contains("Livros")) discount += 0.05m * totalValue;

        // Criar pedido
        var order = new Order
        {
            Items = orderItems,
            DiscountValue = discount,
            Status = dto.Status
        };

        await _repository.AddOrderAsync(order);
        await _repository.SaveChangesAsync();

        return order;
    }
}
