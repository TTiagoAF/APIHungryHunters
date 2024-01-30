using PetaPoco;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace APIHungryHunters.Models
{
    public class MesasDTO
    {
        [Key]
        public int Id_mesa { get; set; }
        public int RestauranteId { get; set; }
        [ForeignKey("RestauranteId")]
        public string Nome { get; set; }
        public int Maximo_pessoas { get; set; }
        public string Notas { get; set; }
    }
}
