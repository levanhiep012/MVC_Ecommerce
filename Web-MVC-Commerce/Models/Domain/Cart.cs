namespace MVC_Ecommerce.Models
{
    public class Cart
    {
        public int CartId { get; set; }
        public string UserId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
    }
}