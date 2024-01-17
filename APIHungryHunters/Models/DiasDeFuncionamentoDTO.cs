using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace APIHungryHunters.Models
{
    public class DiasDeFuncionamentoDTO
    {
        [Key]
        public int Id_dias { get; set; }
        public int RestauranteId { get; set; }
        [ForeignKey("RestauranteId")]
        public string DiasDaSemana { get; set; }
    }
}
