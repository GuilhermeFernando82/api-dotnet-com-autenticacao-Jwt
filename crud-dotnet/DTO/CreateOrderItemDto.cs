public class CreateOrderItemDto
{
    public int ProductId { get; set; } // opcional se for produto novo
    public string Name { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int Quantity { get; set; }
}