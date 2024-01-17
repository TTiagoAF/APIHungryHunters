using PetaPoco;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace APIHungryHunters.Models
{
    public class FeriasDTO
    {
        [Key]
        public int Id_ferias { get; set; }
        public int RestauranteId { get; set; }
        [ForeignKey("RestauranteId")]
        public DateTime InicioFerias { get; set; }
        public DateTime FimFerias { get; set; }
    }
}
