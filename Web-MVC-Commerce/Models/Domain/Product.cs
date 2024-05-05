namespace MVC_Ecommerce.Models
{
    public class Product
    {
        public int ProductId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string ImageUrl { get; set; }
        public decimal Price { get; set; }
        public int? Quantity { get; set; }
        public int CategoryId { get; set; }
        public string? Vote { get; set; }

        // public virtual ICollection<
    }
}