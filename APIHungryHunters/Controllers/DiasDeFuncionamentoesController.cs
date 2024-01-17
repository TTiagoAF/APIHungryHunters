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
using Microsoft.AspNetCore.Authorization;

namespace APIHungryHunters.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class DiasDeFuncionamentoesController : ControllerBase
    {
        private readonly TodoContext _context;

        public DiasDeFuncionamentoesController(TodoContext context)
        {
            _context = context;
        }
        string conexaodb = "Server=localhost;Port=3306;Database=hungryhunters;Uid=root;";

        [HttpGet("ListadeDias")]
        public async Task<ActionResult<IEnumerable<CategoriasDTO>>> ObterTodosDias()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<DiasDeFuncionamento, DiasDeFuncionamentoDTO>();
            });
            AutoMapper.IMapper mapper = config.CreateMapper();
            using (var db = new Database(conexaodb, "MySql.Data.MySqlClient"))
            {
                var TodosDias = await db.FetchAsync<DiasDeFuncionamento>("SELECT * FROM diasfuncionamento");
                var responseItems = mapper.Map<List<DiasDeFuncionamentoDTO>>(TodosDias);
                return Ok(responseItems);
            }
        }

        [HttpGet("ListadeDiaspor{RestauranteId}")]
        public async Task<ActionResult<IEnumerable<DiasDeFuncionamentoDTO>>> ObterDiasporIdRestaurante(int RestauranteId)
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<DiasDeFuncionamento, DiasDeFuncionamentoDTO>();
            });
            AutoMapper.IMapper mapper = config.CreateMapper();
            using (var db = new Database(conexaodb, "MySql.Data.MySqlClient"))
            {
                var DiasporRestauranteId = await db.FetchAsync<DiasDeFuncionamento>("SELECT * FROM diasfuncionamento WHERE RestauranteId = @0", RestauranteId);
                if (DiasporRestauranteId == null)
                {
                    return NotFound($"Não foi encontrado nenhum dia com o Id: {RestauranteId}. Insira outro Id.");
                }
                var responseItems = mapper.Map<List<DiasDeFuncionamentoDTO>>(DiasporRestauranteId);
                return Ok(responseItems);
            }
        }

        [HttpGet("ListadeDiasporIdDias/{IdDias}")]
        public async Task<ActionResult<IEnumerable<DiasDeFuncionamentoDTO>>> ObterCategoriasporId(int IdDias)
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<DiasDeFuncionamento, DiasDeFuncionamentoDTO>();
            });
            AutoMapper.IMapper mapper = config.CreateMapper();
            using (var db = new Database(conexaodb, "MySql.Data.MySqlClient"))
            {
                var DiasporId = await db.FetchAsync<DiasDeFuncionamento>("SELECT * FROM diasfuncionamento WHERE Id_dias = @0", IdDias);
                if (DiasporId == null)
                {
                    return NotFound($"Não foi encontrado nenhum dia com o Id: {DiasporId}. Insira outro Id.");
                }
                var responseItems = mapper.Map<List<DiasDeFuncionamentoDTO>>(DiasporId);
                return Ok(responseItems);
            }
        }

        [HttpPost("AdicionarDias")]
        public async Task<ActionResult> AdicionarDia([FromBody] List<DiasDeFuncionamentoDTO> diasDeFuncionamentoDTOs)
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<DiasDeFuncionamentoDTO, DiasDeFuncionamento>();
            });
            AutoMapper.IMapper mapper = config.CreateMapper();

            using (var db = new Database(conexaodb, "MySql.Data.MySqlClient"))
            {
                foreach (var diasDeFuncionamentoDTO in diasDeFuncionamentoDTOs)
                {
                    if (string.IsNullOrWhiteSpace(diasDeFuncionamentoDTO.DiasDaSemana))
                    {
                        var erro1 = new { Mensagem = "Insira um dia da semana" };
                        return BadRequest(erro1);
                    }

                    var novodia = mapper.Map<DiasDeFuncionamento>(diasDeFuncionamentoDTO);

                    await db.InsertAsync("diasfuncionamento", "Id_dias", true, novodia);
                }
            }
            return Ok();
        }

        private bool DiasDeFuncionamentoExists(int id)
        {
            return (_context.DiasDeFuncionamentos?.Any(e => e.Id_dias == id)).GetValueOrDefault();
        }
    }
}
