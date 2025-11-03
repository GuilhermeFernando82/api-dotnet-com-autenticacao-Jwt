using crud_dotnet.Models;
using System.Text.Json.Serialization;
public enum Category
{
    Electronics,
    Clothing,
    Food,
    Books
}
public class Product
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public Category Category { get; set; }
    [JsonIgnore]
    public ICollection<OrderItem>? OrderItems { get; set; }
}