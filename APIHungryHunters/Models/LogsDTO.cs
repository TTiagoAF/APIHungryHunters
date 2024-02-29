using PetaPoco;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace APIHungryHunters.Models
{
	public class LogsDTO
	{
		[Key]
		public int Id_log { get; set; }
		public int RestauranteId { get; set; }
		[ForeignKey("RestauranteId")]
		public int ContaId { get; set; }
		public string Descricao { get; set; }
		public string Log_Data { get; set; }
	}
}
