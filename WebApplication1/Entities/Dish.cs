namespace WebApplication1.Entities
{
    public class Dish
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Description { get; set; }
        public decimal Price { get; set; }

        public int RestaurantId { get; set; }
        public virtual Restaurant Restaurant { get; set; }
    }
}
