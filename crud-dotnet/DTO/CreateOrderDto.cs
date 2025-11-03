using crud_dotnet.Models;

public class CreateOrderDto
{
    public OrderStatus Status { get; set; }
    public List<CreateOrderItemDto> Items { get; set; } = new();
}