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
        if (dto == null || dto.Items == null || !dto.Items.Any())
            throw new Exception("O pedido não possui itens.");

        var productIds = dto.Items
            .Where(i => i.ProductId > 0)
            .Select(i => i.ProductId)
            .ToList();

        var existingProducts = await _repository.GetProductsByIdsAsync(productIds);
        var productsFromDb = new List<Product>(existingProducts);

        foreach (var item in dto.Items)
        {
            bool exists = productsFromDb.Any(p =>
                (item.ProductId > 0 && p.Id == item.ProductId) ||
                p.Name.Equals(item.Name, StringComparison.OrdinalIgnoreCase));

            if (!exists)
            {
                var newProduct = new Product
                {
                    Name = item.Name,
                    Category = Enum.TryParse<Category>(item.Category, true, out var cat)
                        ? cat
                        : Category.Books, // categoria padrão
                    Price = item.Price
                };

                _repository.AddProduct(newProduct);
                productsFromDb.Add(newProduct);
            }
        }

        await _repository.SaveChangesAsync();

        var orderItems = dto.Items.Select(i =>
        {
            var product = productsFromDb.FirstOrDefault(p =>
                (i.ProductId > 0 && p.Id == i.ProductId) ||
                p.Name.Equals(i.Name, StringComparison.OrdinalIgnoreCase));

            if (product == null)
                throw new Exception($"Produto '{i.Name}' não encontrado ou não foi criado.");

            return new OrderItem
            {
                ProductId = product.Id,
                Quantity = i.Quantity,
                UnitPrice = product.Price
            };
        }).ToList();

        decimal totalValue = orderItems.Sum(i => i.Quantity * i.UnitPrice);
        int distinctProductsCount = orderItems.Select(i => i.ProductId).Distinct().Count();
        decimal discount = 0;

        if (distinctProductsCount >= 5)
            discount += 0.10m * totalValue;

        if (totalValue > 500)
            discount += 0.15m * totalValue;

        var categories = productsFromDb.Select(p => p.Category.ToString()).Distinct().ToList();
        if (categories.Contains("Books"))
            discount += 0.05m * totalValue;

        var order = new Order
        {
            Items = orderItems,
            DiscountValue = discount,
            TotalValue = totalValue - discount,
            Status = OrderStatus.Pending,
            CreatedAt = DateTime.UtcNow
        };

        await _repository.AddOrderAsync(order);
        await _repository.SaveChangesAsync();

        return order;
    }
}
