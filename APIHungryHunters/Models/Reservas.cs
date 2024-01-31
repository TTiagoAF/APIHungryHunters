using PetaPoco;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace APIHungryHunters.Models
{
    public class Reservas
    {
        [Key]
        public int Id_reserva { get; set; }
        public int RestauranteId { get; set; }
        [ForeignKey("RestauranteId")]
        [ResultColumn]
        public virtual Restaurantes Restaurante { get; set; }
        [ResultColumn]
        public string NomeRestaurante { get; set; }
        public int ContaId { get; set; }
        [ForeignKey("ContaId")]
        [ResultColumn]
        public virtual Contas Contas { get; set; }
        [ResultColumn]
        public string NomeCliente { get; set; }
        public int MesaId { get; set; }
        [ForeignKey("MesaId")]
        [ResultColumn]
        public virtual Mesas Mesas { get; set; }
        [ResultColumn]
        public string NomeMesa { get; set; }
        public DateTime Data_reserva { get; set; }
        public string Horario { get; set; }
        public int Quantidade_pessoa { get; set; }
        public string Estado { get; set; }
    }
}
