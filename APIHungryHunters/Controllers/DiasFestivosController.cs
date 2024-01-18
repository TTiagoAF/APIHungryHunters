using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using APIHungryHunters.Models;
using AutoMapper;
using PetaPoco;

namespace APIHungryHunters.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DiasFestivosController : ControllerBase
    {
        private readonly TodoContext _context;

        public DiasFestivosController(TodoContext context)
        {
            _context = context;
        }
        string conexaodb = "Server=localhost;Port=3306;Database=hungryhunters;Uid=root;";

        [HttpGet("ListadeDiasFestivos")]
        public async Task<ActionResult<IEnumerable<DiasFestivosDTO>>> ObterTodosDiasFestivos()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<DiasFestivos, DiasFestivosDTO>();
            });
            AutoMapper.IMapper mapper = config.CreateMapper();
            using (var db = new Database(conexaodb, "MySql.Data.MySqlClient"))
            {
                var Todosfestivos = await db.FetchAsync<DiasFestivos>("SELECT * FROM diasfestivos");
                var responseItems = mapper.Map<List<DiasFestivosDTO>>(Todosfestivos);
                return Ok(responseItems);
            }
        }

        [HttpGet("ListadeDiasFestivospor{RestauranteId}")]
        public async Task<ActionResult<IEnumerable<DiasFestivosDTO>>> ObterDiasFestivosporIdRestaurante(int RestauranteId)
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<DiasFestivos, DiasFestivosDTO>();
            });
            AutoMapper.IMapper mapper = config.CreateMapper();
            using (var db = new Database(conexaodb, "MySql.Data.MySqlClient"))
            {
                var FestivosporRestauranteId = await db.FetchAsync<DiasFestivos>("SELECT * FROM diasfestivos WHERE RestauranteId = @0", RestauranteId);
                if (FestivosporRestauranteId == null)
                {
                    return NotFound($"Não foi encontrada nenhum dia festivo com o Id: {RestauranteId}. Insira outro Id.");
                }
                var responseItems = mapper.Map<List<DiasFestivosDTO>>(FestivosporRestauranteId);
                return Ok(responseItems);
            }
        }

        [HttpGet("ListadeDiasFestivosporIdFestivo/{IdFestivo}")]
        public async Task<ActionResult<IEnumerable<DiasFestivosDTO>>> ObterDiasFestivosporId(int IdFestivo)
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<DiasFestivos, DiasFestivosDTO>();
            });
            AutoMapper.IMapper mapper = config.CreateMapper();
            using (var db = new Database(conexaodb, "MySql.Data.MySqlClient"))
            {
                var FestivosporId = await db.FetchAsync<DiasFestivos>("SELECT * FROM diasfestivos WHERE Id_festivo = @0", IdFestivo);
                if (FestivosporId == null)
                {
                    return NotFound($"Não foi encontrada nenhum dia festivo com o Id: {IdFestivo}. Insira outro Id.");
                }
                var responseItems = mapper.Map<List<DiasFestivosDTO>>(FestivosporId);
                return Ok(responseItems);
            }
        }

        [HttpPost("AdicionarDiasFestivos")]
        public async Task<ActionResult> AdicionarDiasFestivos([FromBody] List<DiasFestivosDTO> diasFestivosDTOs)
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<DiasFestivosDTO, DiasFestivos>();
            });
            AutoMapper.IMapper mapper = config.CreateMapper();

            using (var db = new Database(conexaodb, "MySql.Data.MySqlClient"))
            {
                foreach (var diasFestivosDTO in diasFestivosDTOs)
                {
                    if (diasFestivosDTO.DiaFestivo.Year != DateTime.Now.Year)
                    {
                        var erro5 = new { Mensagem = "Marque um dia festivo deste ano" };
                        return BadRequest(erro5);
                    }

                    if (diasFestivosDTO.DiaFestivo.Year == DateTime.Now.Year)
                    {
                        if (diasFestivosDTO.DiaFestivo.Month < DateTime.Now.Month)
                        {
                            var erro5 = new { Mensagem = "Esse mês já passou" };
                            return BadRequest(erro5);
                        }
                    }
                    if (diasFestivosDTO.DiaFestivo.Year == DateTime.Now.Year)
                    {
                        if (diasFestivosDTO.DiaFestivo.Month >= DateTime.Now.Month)
                        {
                            if (diasFestivosDTO.DiaFestivo.Day < DateTime.Now.Day)
                            {
                                var erro5 = new { Mensagem = "Esse dia já passou" };
                                return BadRequest(erro5);
                            }
                        }
                    }

                    var novofestivo = mapper.Map<DiasFestivos>(diasFestivosDTO);

                    await db.InsertAsync("diasfestivos", "Id_festivo", true, novofestivo);
                }
            }
            return Ok();
        }

        private bool DiasFestivosExists(int id)
        {
            return (_context.DiasFestivos?.Any(e => e.Id_festivo == id)).GetValueOrDefault();
        }
    }
}
