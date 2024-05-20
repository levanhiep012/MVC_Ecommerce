using ecommerce_temp.Models;

public class CartItem
{
    public string CartItemId { get; set; }
    public string CartId { get; set; }
    public string ProductId { get; set; }
    public int Quantity { get; set; }

    public virtual Product Product { get; set; }
    public virtual Cart Cart { get; set; }
}
