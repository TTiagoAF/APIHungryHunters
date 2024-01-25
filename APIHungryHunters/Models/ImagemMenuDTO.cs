using PetaPoco;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace APIHungryHunters.Models
{
    public class ImagemMenuDTO
    {
        [Key]
        public int Id_imagemmenu { get; set; }
        public int RestauranteId { get; set; }
        [ForeignKey("RestauranteId")]
        public string Imagem_titulo { get; set; }
        public List<IFormFile> Menu_imagem { get; set; }
    }
}
