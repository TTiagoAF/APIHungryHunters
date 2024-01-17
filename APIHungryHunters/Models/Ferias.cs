using PetaPoco;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace APIHungryHunters.Models
{
    public class Ferias
    {
        [Key]
        public int Id_ferias { get; set; }
        public int RestauranteId { get; set; }
        [ForeignKey("RestauranteId")]
        [ResultColumn]
        public virtual Restaurantes Restaurante { get; set; }
        public DateTime InicioFerias { get; set; }
        public DateTime FimFerias { get; set; }
    }
}
