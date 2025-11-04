using crud_dotnet.Models;

public class CreateOrderDto
{
    public List<CreateOrderItemDto> Items { get; set; } = new();
}