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

        [HttpGet("ListadePlantas")]
        public async Task<ActionResult<IEnumerable<PlantaRestauranteDTO>>> ObterTodasAsPlantas()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<PlantaRestaurante, PlantaRestauranteDTO>();
            });
            AutoMapper.IMapper mapper = config.CreateMapper();
            using (var db = new Database(conexaodb, "MySql.Data.MySqlClient"))
            {
                var TodasPlantas = await db.FetchAsync<PlantaRestaurante>("SELECT * FROM plantarestaurante");
                var responseItems = mapper.Map<List<PlantaRestauranteDTO>>(TodasPlantas);
                return Ok(responseItems);
            }
        }

        [HttpGet("ListadePlantaspor{RestauranteId}")]
        public async Task<ActionResult<IEnumerable<PlantaRestauranteDTO>>> ObterPlantaporIdRestaurante(int RestauranteId)
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<PlantaRestaurante, PlantaRestauranteDTO>();
            });
            AutoMapper.IMapper mapper = config.CreateMapper();
            using (var db = new Database(conexaodb, "MySql.Data.MySqlClient"))
            {
                var PlantasporRestauranteId = await db.FetchAsync<PlantaRestaurante>("SELECT * FROM plantarestaurante WHERE RestauranteId = @0", RestauranteId);
                if (PlantasporRestauranteId == null)
                {
                    return NotFound($"Não foi encontrada nenhuma Conta com o Id: {RestauranteId}. Insira outro Id.");
                }
                var responseItems = mapper.Map<List<PlantaRestauranteDTO>>(PlantasporRestauranteId);
                return Ok(responseItems);
            }
        }

        [HttpGet("ListadePlantasporIdPlanta/{IdPlanta}")]
        public async Task<ActionResult<IEnumerable<PlantaRestauranteDTO>>> ObterPlantaporId(int IdPlanta)
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<PlantaRestaurante, PlantaRestauranteDTO>();
            });
            AutoMapper.IMapper mapper = config.CreateMapper();
            using (var db = new Database(conexaodb, "MySql.Data.MySqlClient"))
            {
                var PlantaporId = await db.FetchAsync<PlantaRestaurante>("SELECT * FROM plantarestaurante WHERE Id_planta = @0", IdPlanta);
                if (PlantaporId == null)
                {
                    return NotFound($"Não foi encontrada nenhuma Conta com o Id: {IdPlanta}. Insira outro Id.");
                }
                var responseItems = mapper.Map<List<PlantaRestauranteDTO>>(PlantaporId);
                return Ok(responseItems);
            }
        }

        [HttpPost("upload-imagem/")]
        public async Task<IActionResult> UploadImagem([FromForm] IFormFile file, [FromBody] List<PlantaRestauranteDTO> plantaRestauranteDTOs)
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<PlantaRestauranteDTO, PlantaRestaurante>();
            });
            AutoMapper.IMapper mapper = config.CreateMapper();
            using (var db = new Database(conexaodb, "MySql.Data.MySqlClient"))
            { 
                 using (var ms = new MemoryStream())
                 {
                    foreach (var plantaRestauranteDTO in plantaRestauranteDTOs)
                    {
                        await file.CopyToAsync(ms);
                        var imagemBytes = ms.ToArray();

                        var novorestaurante = mapper.Map<PlantaRestaurante>(plantaRestauranteDTO);

                        await db.InsertAsync("plantarestaurante", "Id_planta", true, novorestaurante);
                        return Ok("g");
                    }
                    return Ok("f");
                 }                                     
            }
        }
    }
}
