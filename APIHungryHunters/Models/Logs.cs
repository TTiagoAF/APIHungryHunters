using PetaPoco;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace APIHungryHunters.Models
{
	public class Logs
	{
		[Key]
		public int Id_log { get; set; }
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
		public string Descricao { get; set; }
		public string Log_Data { get; set; }
	}
}
