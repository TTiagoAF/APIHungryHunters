using PetaPoco;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace APIHungryHunters.Models
{
    public class HorariosDTO
    {
        [Key]
        public int Id_horarios { get; set; }
        public int RestauranteId { get; set; }
        [ForeignKey("RestauranteId")]
        public string HoraReserva { get; set; }
    }
}
