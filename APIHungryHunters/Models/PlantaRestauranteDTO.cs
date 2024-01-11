using System.ComponentModel.DataAnnotations.Schema;

namespace APIHungryHunters.Models
{
    public class PlantaRestauranteDTO
    {
        public int Id_planta { get; set; }
        public int RestauranteId { get; set; }
        [ForeignKey("RestauranteId")]
        public virtual RestaurantesDTO Restaurante { get; set; }
        public string Planta_titulo { get; set; }
        public byte[] Planta_binary { get; set; }
    }
}
