using crud_dotnet.Models;
using System.Text.Json.Serialization;
public class CategoryData
{
    public int Id { get; set; }
    public string Name { get; set; }
    public ICollection<Product> Products { get; set; }
}