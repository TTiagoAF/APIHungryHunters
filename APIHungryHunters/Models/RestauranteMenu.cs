﻿using PetaPoco;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace APIHungryHunters.Models
{
    public class RestauranteMenu
    {
        [Key]
        public int Id_pratos { get; set; }
        public int RestauranteId { get; set; }
        [ForeignKey("RestauranteId")]
        [ResultColumn]
        public virtual Restaurantes Restaurante { get; set; }
        public string Nome { get; set; }
        public Decimal Preco { get; set; }
        public string Desc_prato { get; set; }
        public string CategoriaPrato { get; set; }
    }
}
