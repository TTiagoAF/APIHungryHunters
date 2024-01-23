using PetaPoco;
using System.ComponentModel.DataAnnotations.Schema;

namespace APIHungryHunters.Models
{
    public class PlantaRestauranteDTO
    {
        public int Id_planta { get; set; }
        public int RestauranteId { get; set; }
        [ForeignKey("RestauranteId")]
        public string Planta_titulo { get; set; }
        public IFormFile Planta_image { get; set; }
    }
}
