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

        [HttpGet("ListadeFotosDoRestauranteDaBase")]
        public async Task<ActionResult<IEnumerable<TituloFotosDTO>>> ObterTodasFotosDoRestauranteDaBase()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<FotosRestaurante, TituloFotosDTO>();
            });
            AutoMapper.IMapper mapper = config.CreateMapper();
            using (var db = new Database(conexaodb, "MySql.Data.MySqlClient"))
            {
                var todasFotos = await db.FetchAsync<FotosRestaurante>("SELECT * FROM fotosrestaurante");
                var responseItems = mapper.Map<List<TituloFotosDTO>>(todasFotos);
                return Ok(responseItems);
            }
        }

        [HttpGet("ListadeFotosDoRestauranteDaPasta")]
        public async Task<ActionResult> ObterTodasFotosDoRestauranteDaPasta()
        {
            string[] fyles = Directory.GetFiles(".\\Imagens");
            return Ok(fyles);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<IEnumerable<HorariosDTO>>> ObterHorariosporId(int id)
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<FotosRestaurante, FotosRestauranteDTO>();
            });
            AutoMapper.IMapper mapper = config.CreateMapper();
            using (var db = new Database(conexaodb, "MySql.Data.MySqlClient"))
            {
                var fotosRestaurante = await db.FetchAsync<FotosRestaurante>("SELECT * FROM fotosrestaurante WHERE Id_fotos = @0", id);

                if (fotosRestaurante == null)
                {
                    return NotFound("PlantaRestaurante não encontrado.");
                }

                var fotosRestauranteDto = mapper.Map<List<FotosRestauranteDTO>>(fotosRestaurante);

                return Ok(fotosRestauranteDto);
            }
        }
    

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
                if (fotosRestauranteDTO.FotoRestaurante != null && fotosRestauranteDTO.FotoRestaurante.Count > 0)
                {
                    foreach (var arquivo in fotosRestauranteDTO.FotoRestaurante)
                    {
                        string nomeArquivo = $@"{Guid.NewGuid()}{arquivo.FileName}";

                        string caminhoArquivo = Path.Combine(".\\Imagens", nomeArquivo);

                        using (var stream = new FileStream(caminhoArquivo, FileMode.Create))
                        {
                            await arquivo.CopyToAsync(stream);
                        }

                        var imagemMenu = mapper.Map<FotosRestaurante>(fotosRestauranteDTO);
                        imagemMenu.Foto_titulo = nomeArquivo;

                        await db.InsertAsync("fotosrestaurante", "Id_fotos", true, imagemMenu);
                    }

                    return Ok("FotosRestaurante adicionadas com sucesso!");
                }
            }
            
            return BadRequest("A imagem não foi fornecida ou é inválida.");
        }
    }
}

