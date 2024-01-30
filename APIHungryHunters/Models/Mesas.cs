using PetaPoco;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace APIHungryHunters.Models
{
    public class Mesas
    {
        [Key]
        public int Id_mesa { get; set; }
        public int RestauranteId { get; set; }
        [ForeignKey("RestauranteId")]
        [ResultColumn]
        public virtual Restaurantes Restaurante { get; set; }
        public string Nome { get; set; }
        public int Maximo_pessoas { get; set; }
        public string Notas { get; set; }
        [ResultColumn]
        public List<Reservas> Reservas { get; set; }
    }
}
