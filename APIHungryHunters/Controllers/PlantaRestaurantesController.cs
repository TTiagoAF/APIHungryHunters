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
using Microsoft.AspNetCore.Cors;

namespace APIHungryHunters.Controllers
{
    
    [Route("api/[controller]")]
    [ApiController]
    public class PlantaRestaurantesController : ControllerBase
    {
        private readonly TodoContext _context;

        public PlantaRestaurantesController(TodoContext context)
        {
            _context = context;
        }

        string conexaodb = "Server=localhost;Port=3306;Database=hungryhunters;Uid=root;";

        [HttpPost("AdicionarPlanta")]
        public async Task<IActionResult> AdicionarPlantaRestaurante([FromForm] PlantaRestauranteDTO plantaRestauranteDto)
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<PlantaRestauranteDTO, PlantaRestaurante>();
            });
            AutoMapper.IMapper mapper = config.CreateMapper();

            using (var db = new Database(conexaodb, "MySql.Data.MySqlClient"))
            {
                if (plantaRestauranteDto.Planta_image != null && plantaRestauranteDto.Planta_image.Length > 0)
                {
                    string nomeArquivo = $@"{Guid.NewGuid()}{plantaRestauranteDto.Planta_image.FileName}";

                    string caminhoArquivo = Path.Combine(".\\ImagensPlanta", nomeArquivo);

                    using (var stream = new FileStream(caminhoArquivo, FileMode.Create))
                    {
                        await plantaRestauranteDto.Planta_image.CopyToAsync(stream);
                    }
                    plantaRestauranteDto.Id_planta = ObterIdDias(plantaRestauranteDto.RestauranteId);
                    var produtoExistente = await db.SingleOrDefaultAsync<PlantaRestaurante>("SELECT * FROM PlantaRestaurante WHERE Id_planta = @0", plantaRestauranteDto.Id_planta);

                    if (produtoExistente == null)
                    {
                        plantaRestauranteDto.Id_planta = 0;
                        var plantaRestaurante = mapper.Map<PlantaRestaurante>(plantaRestauranteDto);
                        plantaRestaurante.Planta_titulo = nomeArquivo;

                        await db.InsertAsync("PlantaRestaurante", "Id_planta", true, plantaRestaurante);
                    }
                    else
                    {
                        var imagem = Path.Combine(".\\ImagensPlanta", produtoExistente.Planta_titulo);
                        if (System.IO.File.Exists(imagem))
                            System.IO.File.Delete(imagem);
                        var novodia2 = mapper.Map<PlantaRestaurante>(plantaRestauranteDto);
                        novodia2.Planta_titulo = nomeArquivo;
                        await db.UpdateAsync("PlantaRestaurante", "Id_planta", novodia2);
                    }
                    return Ok("PlantaRestaurante adicionado com sucesso!");
                }
            }
            return BadRequest("A imagem não foi fornecida ou é inválida.");
        }

        [HttpGet("ObterIdDias")]
        public int ObterIdDias(int restauranteid)
        {
            using (var db = new Database(conexaodb, "MySql.Data.MySqlClient"))
            {
                var usuario = db.FirstOrDefault<PlantaRestaurante>("SELECT Id_planta FROM PlantaRestaurante WHERE RestauranteId = @0", restauranteid);
                if (usuario == null)
                {
                    return 0;
                }
                else
                {
                    return usuario.Id_planta;
                }

            }
        }

        [HttpGet("ObterPlanta/{restauranteId}")]
        public IActionResult ObterPlanta(int restauranteId)
        {
            string pastaImagens = Path.Combine(".\\ImagensPlanta\\");

            using (var db = new Database(conexaodb, "MySql.Data.MySqlClient"))
            {
                var plantaRestaurante = db.Fetch<PlantaRestaurante>("SELECT * FROM PlantaRestaurante WHERE RestauranteId = @RestauranteId", new { RestauranteId = restauranteId });

                if (plantaRestaurante == null || plantaRestaurante.Count() == 0)
                {
                    return NotFound("Nenhuma imagem encontrada para este restaurante.");
                }

                List<string> caminhosImagens = new List<string>();

                foreach (var plantasRestaurante in plantaRestaurante)
                {
                    string caminhoImagem = Path.GetFullPath(pastaImagens + plantasRestaurante.Planta_titulo);
                    caminhosImagens.Add(caminhoImagem);
                }

                return Ok(new { caminhosImagens });
            }
        }
    }
}

