namespace ecommerce_temp.Data.Models
{
    public partial class Brand
    {
        public Brand()
        {
            this.Products = new HashSet<Product>();
        }

        public int BrandId { get; set; }
        public string BrandName { get; set; }
        public string? ImageUrl { get; set; }

        public virtual ICollection<Product> Products { get; set; }
    }
}
