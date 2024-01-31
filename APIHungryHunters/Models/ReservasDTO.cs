using PetaPoco;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace APIHungryHunters.Models
{
    public class ReservasDTO
    {
        [Key]
        public int Id_reserva { get; set; }
        public int RestauranteId { get; set; }
        [ForeignKey("RestauranteId")]
        public int ContaId { get; set; }
        [ForeignKey("ContaId")]
        public int MesaId { get; set; }
        [ForeignKey("MesaId")]
        public DateTime Data_reserva { get; set; }
        public string Horario { get; set; }
        public int Quantidade_pessoa { get; set; }
        public string Estado { get; set; }
    }
}
