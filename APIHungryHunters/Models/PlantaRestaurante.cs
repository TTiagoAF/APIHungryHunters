using PetaPoco;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace APIHungryHunters.Models
{
    public class PlantaRestaurante
    {
        [Key]
        public int Id_planta { get; set; }
        public int RestauranteId { get; set; }
        [ForeignKey("RestauranteId")]
        [ResultColumn]
        public virtual Restaurantes Restaurante { get; set; }
        public string Planta_titulo { get; set; }
    }
}
