using PetaPoco;
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
        [ResultColumn]
        public virtual Empresas Empresas { get; set; }
        public string Nome { get; set; }
        public Decimal PrecoMedio { get; set; }
        public string Distrito { get; set; }
        public string Coordenadas { get; set; }
        public string Telemovel { get; set; }
        public string Descricao { get; set; }
        public int CapacidadeGrupo { get; set; }
        public string Autorizado { get; set; }
        [ResultColumn]
        public List<PlantaRestaurante> PlantaRestaurantes { get; set; }
        [ResultColumn]
        public List<RestauranteMenu> RestauranteMenus { get; set; }
        [ResultColumn]
        public List<Categorias> Categorias { get; set; }
        [ResultColumn]
        public List<DiasDeFuncionamento> DiasDeFuncionamentos { get; set; }
        [ResultColumn]
        public List<Ferias> Ferias { get; set; }
        [ResultColumn]
        public List<DiasFestivos> DiasFestivos { get; set; }
        [ResultColumn]
        public List<Horarios> Horarios { get; set; }
        [ResultColumn]
        public List<ImagemMenu> ImagemMenus { get; set; }
        [ResultColumn]
        public List<FotosRestaurante> FotosRestaurantes { get; set; }
        [ResultColumn]
        public List<Reservas> Reservas { get; set; }
        [ResultColumn]
        public List<Mesas> Mesas { get; set; }
        [ResultColumn]
        public List<Avaliacoes> Avaliacoes { get; set; }
    }
}
