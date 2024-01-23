using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using APIHungryHunters.Models;
using System.Drawing;
using AutoMapper;
using PetaPoco;
using Microsoft.AspNetCore.Authorization;

namespace APIHungryHunters.Controllers
{
    
    [Route("api/[controller]")]
    [ApiController]
    public class FotosRestaurantesController : ControllerBase
    {
        private readonly TodoContext _context;

        public FotosRestaurantesController(TodoContext context)
        {
            _context = context;
        }

        string conexaodb = "Server=localhost;Port=3306;Database=hungryhunters;Uid=root;";

        [HttpPost("AdicionarFotos")]
        public async Task<ActionResult> AdicionarFotos([FromForm] FotosRestauranteDTO fotosRestauranteDTO)
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<FotosRestauranteDTO, FotosRestaurante>();
            });
            AutoMapper.IMapper mapper = config.CreateMapper();

            using (var db = new Database(conexaodb, "MySql.Data.MySqlClient"))
            {
                if (fotosRestauranteDTO.FotoRestaurante != null && fotosRestauranteDTO.FotoRestaurante.Length > 0)
                {
                    string nomeArquivo = fotosRestauranteDTO.FotoRestaurante.FileName;

                    string caminhoArquivo = Path.Combine(".\\Imagens", nomeArquivo);

                    using (var stream = new FileStream(caminhoArquivo, FileMode.Create))
                    {
                        await fotosRestauranteDTO.FotoRestaurante.CopyToAsync(stream);
                    }

                    var imagemMenu = mapper.Map<FotosRestaurante>(fotosRestauranteDTO);
                    imagemMenu.Foto_titulo = nomeArquivo;

                    await db.InsertAsync("fotosrestaurante", "Id_fotos", true, imagemMenu);

                    return Ok("FotoRestaurante adicionado com sucesso!");
                }
            }

            return BadRequest("A imagem não foi fornecida ou é inválida.");
        }


    }
}

