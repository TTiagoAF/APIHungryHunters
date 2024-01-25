using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace APIHungryHunters.Models
{
    public class TituloFotosDTO
    {
        [Key]
        public int Id_fotos { get; set; }
        public int RestauranteId { get; set; }
        [ForeignKey("RestauranteId")]
        public string Foto_titulo { get; set; }
    }
}
