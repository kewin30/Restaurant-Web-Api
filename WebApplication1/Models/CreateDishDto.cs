using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models
{
    public class CreateDishDto
    {
        [Required]
        public string Name { get; set; }
        public int Description { get; set; }
        public decimal Price { get; set; }

        public int RestaurantId { get; set; }
    }
}
