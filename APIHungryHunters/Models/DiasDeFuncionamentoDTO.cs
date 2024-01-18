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
        public string Segunda { get; set; }
        public string Terca { get; set; }
        public string Quarta { get; set; }
        public string Quinta { get; set; }
        public string Sexta { get; set; }
        public string Sabado { get; set; }
        public string Domingo { get; set; }
    }
}
