using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace APIHungryHunters.Models
{
    public class TodasReservasDTO
    {
        [Key]
        public int Id_reserva { get; set; }
        public int RestauranteId { get; set; }
        [ForeignKey("RestauranteId")]
        public string NomeRestaurante { get; set; }
        public int ContaId { get; set; }
        [ForeignKey("ContaId")]
        public string NomeCliente { get; set; }
        public int MesaId { get; set; }
        [ForeignKey("MesaId")]
        public string NomeMesa { get; set; }
        public DateTime Data_reserva { get; set; }
        public string Horario { get; set; }
        public int Quantidade_pessoa { get; set; }
        public string Estado { get; set; }
    }
}
