using PetaPoco;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace APIHungryHunters.Models
{
    public class DiasFestivosDTO
    {
        [Key]
        public int Id_festivo { get; set; }
        public int RestauranteId { get; set; }
        [ForeignKey("RestauranteId")]
        public DateTime DiaFestivo { get; set; }
    }
}
