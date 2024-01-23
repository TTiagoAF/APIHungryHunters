using PetaPoco;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace APIHungryHunters.Models
{
    public class FotosRestauranteDTO
    {
        [Key]
        public int Id_fotos { get; set; }
        public int RestauranteId { get; set; }
        [ForeignKey("RestauranteId")]
        public string Foto_titulo { get; set; }
        public IFormFile FotoRestaurante { get; set; }
    }
}
