﻿using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection.Metadata;

namespace APIHungryHunters.Models
{
    public class RestaurantesDTO
    {
        public int Id_restaurante { get; set; }
        public string NipcEmpresa { get; set; }
        [ForeignKey("NipcEmpresa")]
        public virtual EmpresasDTO Empresas { get; set; }
        public string Nome { get; set; }
        public Decimal PrecoMedio { get; set; }
        public int NumeroMesas { get; set; }
        public string Distrito { get; set; }
        public string Coordenadas { get; set; }
        public string Descricao { get; set; }
        public int CapacidadeGrupo { get; set; }
        public Boolean Autorizado { get; set; }
    }
}
