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
    public class FeriasController : ControllerBase
    {
        private readonly TodoContext _context;

        public FeriasController(TodoContext context)
        {
            _context = context;
        }
        string conexaodb = "Server=localhost;Port=3306;Database=hungryhunters;Uid=root;";

        [HttpGet("ListadeFerias")]
        public async Task<ActionResult<IEnumerable<FeriasDTO>>> ObterTodasFerias()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Ferias, FeriasDTO>();
            });
            AutoMapper.IMapper mapper = config.CreateMapper();
            using (var db = new Database(conexaodb, "MySql.Data.MySqlClient"))
            {
                var TodasFerias = await db.FetchAsync<Ferias>("SELECT * FROM ferias");
                var responseItems = mapper.Map<List<FeriasDTO>>(TodasFerias);
                return Ok(responseItems);
            }
        }

        [HttpGet("ListadeFeriaspor{RestauranteId}")]
        public async Task<ActionResult<IEnumerable<FeriasDTO>>> ObterFeriasporIdRestaurante(int RestauranteId)
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Ferias, FeriasDTO>();
            });
            AutoMapper.IMapper mapper = config.CreateMapper();
            using (var db = new Database(conexaodb, "MySql.Data.MySqlClient"))
            {
                var FeriasporRestauranteId = await db.FetchAsync<Ferias>("SELECT * FROM ferias WHERE RestauranteId = @0", RestauranteId);
                if (FeriasporRestauranteId == null)
                {
                    return NotFound($"Não foi encontrada nenhumas ferias com o Id: {RestauranteId}. Insira outro Id.");
                }
                var responseItems = mapper.Map<List<FeriasDTO>>(FeriasporRestauranteId);
                return Ok(responseItems);
            }
        }

        [HttpGet("ListadeFeriasporIdFerias/{IdFerias}")]
        public async Task<ActionResult<IEnumerable<FeriasDTO>>> ObterFeriasporId(int IdFerias)
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Ferias, FeriasDTO>();
            });
            AutoMapper.IMapper mapper = config.CreateMapper();
            using (var db = new Database(conexaodb, "MySql.Data.MySqlClient"))
            {
                var CategoriaporId = await db.FetchAsync<Ferias>("SELECT * FROM ferias WHERE Id_ferias = @0", IdFerias);
                if (CategoriaporId == null)
                {
                    return NotFound($"Não foi encontrada nenhumas ferias com o Id: {IdFerias}. Insira outro Id.");
                }
                var responseItems = mapper.Map<List<FeriasDTO>>(CategoriaporId);
                return Ok(responseItems);
            }
        }

        [HttpPost("AdicionarFerias")]
        public async Task<ActionResult> AdicionarFerias([FromBody] List<FeriasDTO> feriasDTOs)
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<FeriasDTO, Ferias>();
            });
            AutoMapper.IMapper mapper = config.CreateMapper();

            using (var db = new Database(conexaodb, "MySql.Data.MySqlClient"))
            {
                foreach (var feriasDTO in feriasDTOs)
                {
                    if(feriasDTO.InicioFerias.Year != feriasDTO.FimFerias.Year )
                    {
                        var erro5 = new { Mensagem = "Data inválida" };
                        return BadRequest(erro5);
                    }

                    if (feriasDTO.InicioFerias.Year == feriasDTO.FimFerias.Year)
                    {
                        if (feriasDTO.InicioFerias.Month > feriasDTO.FimFerias.Month)
                        {
                            var erro5 = new { Mensagem = "Data inválida" };
                            return BadRequest(erro5);
                        }
                    }
                    if (feriasDTO.InicioFerias.Year == feriasDTO.FimFerias.Year)
                    {
                        if (feriasDTO.InicioFerias.Month <= feriasDTO.FimFerias.Month)
                        {
                            if (feriasDTO.InicioFerias.Day > feriasDTO.FimFerias.Day)
                            {
                                var erro5 = new { Mensagem = "Data inválida" };
                                return BadRequest(erro5);
                            }
                        }
                    }

                    if (feriasDTO.InicioFerias.Year != DateTime.Now.Year || feriasDTO.FimFerias.Year != DateTime.Now.Year)
                    {
                        var erro5 = new { Mensagem = "Marque as férias neste ano" };
                        return BadRequest(erro5);
                    }
                    
                    if (feriasDTO.InicioFerias.Year == DateTime.Now.Year || feriasDTO.FimFerias.Year == DateTime.Now.Year)
                    {
                        if (feriasDTO.InicioFerias.Month < DateTime.Now.Month || feriasDTO.FimFerias.Month < DateTime.Now.Month)
                        {
                            var erro5 = new { Mensagem = "Esse mês já passou" };
                            return BadRequest(erro5);
                        }
                    }
                    if (feriasDTO.InicioFerias.Year == DateTime.Now.Year || feriasDTO.FimFerias.Year == DateTime.Now.Year)
                    {
                        if (feriasDTO.InicioFerias.Month <= DateTime.Now.Month || feriasDTO.FimFerias.Month <= DateTime.Now.Month)
                        {
                            if (feriasDTO.InicioFerias.Day < DateTime.Now.Day || feriasDTO.FimFerias.Day < DateTime.Now.Day)
                            {
                                var erro5 = new { Mensagem = "Esse dia já passou" };
                                return BadRequest(erro5);
                            }
                        }
                    }

                    var novaferias = mapper.Map<Ferias>(feriasDTO);

                    await db.InsertAsync("ferias", "Id_ferias", true, novaferias);
                }
            }
            return Ok();
        }

        private bool FeriasExists(int id)
        {
            return (_context.Ferias?.Any(e => e.Id_ferias == id)).GetValueOrDefault();
        }
    }
}
