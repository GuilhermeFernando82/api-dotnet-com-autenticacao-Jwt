namespace crud_dotnet.Models
{
    public enum OrderStatus
    {
        Pending,
        Approved,
        Canceled
    }
    public class Order
    {
        public int Id { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public decimal TotalValue { get; set; }
        public decimal DiscountValue { get; set; }
        public OrderStatus Status { get; set; }
        public List<OrderItem> Items { get; set; } = new();
    }
}