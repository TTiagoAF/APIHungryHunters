using PetaPoco;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace APIHungryHunters.Models
{
    public class FotosRestaurante
    {
        [Key]
        public int Id_fotos { get; set; }
        public int RestauranteId { get; set; }
        [ForeignKey("RestauranteId")]
        [ResultColumn]
        public virtual Restaurantes Restaurante { get; set; }
        public string Foto_titulo { get; set; }
    }
}
