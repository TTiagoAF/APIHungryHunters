using PetaPoco;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace APIHungryHunters.Models
{
    public class ImagemMenu
    {
        [Key]
        public int Id_imagemmenu { get; set; }
        public int RestauranteId { get; set; }
        [ForeignKey("RestauranteId")]
        [ResultColumn]
        public virtual Restaurantes Restaurante { get; set; }
        public string Imagem_titulo { get; set; }
        public IFormFile Menu_imagem { get; set; }
    }
}
