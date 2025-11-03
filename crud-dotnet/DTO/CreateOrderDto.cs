using crud_dotnet.Models;

public class CreateOrderDto
{
    public string Status { get; set; }
    public List<CreateOrderItemDto> Items { get; set; } = new();
}