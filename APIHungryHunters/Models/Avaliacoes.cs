using PetaPoco;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace APIHungryHunters.Models
{
    public class Avaliacoes
    {
        [Key]
        public int Id_avaliacao { get; set; }
        public int RestauranteId { get; set; }
        [ForeignKey("RestauranteId")]
        [ResultColumn]
        public virtual Restaurantes Restaurante { get; set; }
        [ResultColumn]
        public string NomeRestaurante { get; set; }
        public int ContaId { get; set; }
        [ResultColumn]
        public virtual Contas Contas { get; set; }
        [ResultColumn]
        public string NomeCliente { get; set; }
        public Decimal Comida { get; set; }
        public Decimal Conforto { get; set; }
        public Decimal Beleza { get; set; }
        public Decimal Atendimento { get; set; }
        public Decimal Velocidade { get; set; }
        public string Comentario { get; set; }
        [ResultColumn]
        public double MediaConforto { get; set; }
    }
}
