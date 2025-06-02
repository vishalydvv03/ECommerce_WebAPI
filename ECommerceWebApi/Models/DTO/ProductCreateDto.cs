namespace ECommerceWebApi.Models.DTO
{
    public class ProductCreateDto
    {
        public string Name { get; set; }
        public decimal Price { get; set; }
        public int CategoryId { get; set; }
    }
}
