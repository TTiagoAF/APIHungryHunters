using PetaPoco;
using System.ComponentModel.DataAnnotations.Schema;

namespace APIHungryHunters.Models
{
    public class TodosRestaurantes
    {
        public int Id_restaurante { get; set; }
        public string NipcEmpresa { get; set; }
        [ForeignKey("NipcEmpresa")]
        public string Nome { get; set; }
        public Decimal PrecoMedio { get; set; }
        public int NumeroMesas { get; set; }
        public string Distrito { get; set; }
        public string Coordenadas { get; set; }
        public string Telemovel { get; set; }
        public string Descricao { get; set; }
        public int CapacidadeGrupo { get; set; }
        public string Autorizado { get; set; }
        public List<Categorias> Categorias { get; set; }
        public List<RestauranteMenu> RestauranteMenus { get; set; }
    }
}
