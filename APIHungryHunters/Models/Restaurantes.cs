using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection.Metadata;
using System.Runtime.Serialization;

namespace APIHungryHunters.Models
{
    public class Restaurantes
    {
        [Key]
        public int Id_restaurante { get; set; }
        public string NipcEmpresa { get; set; }
        [ForeignKey("NipcEmpresa")]
        public virtual Empresas Empresas { get; set; }
        public string Nome { get; set; }
        public Decimal PrecoMedio { get; set; }
        public int NumeroMesas { get; set; }
        public string Distrito { get; set; }
        public string Coordenadas { get; set; }
        public string Descricao { get; set; }
        public int CapacidadeGrupo { get; set; }
        public Boolean Autorizado { get; set; }
        public List<PlantaRestaurante> PlantaRestaurantes { get; set; }
        public List<RestauranteMenu> RestauranteMenus { get; set; }
    }
}
