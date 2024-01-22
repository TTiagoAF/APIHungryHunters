using PetaPoco;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace APIHungryHunters.Models
{
    public class CategoriasDTO
    {
        [Key]
        public int Id_categoria { get; set; }
        public int RestauranteId { get; set; }
        [ForeignKey("RestauranteId")]
        public string Categoria_Um { get; set; }
        public string Categoria_Dois { get; set; }
        public string Categoria_Tres { get; set; }
    }
}
