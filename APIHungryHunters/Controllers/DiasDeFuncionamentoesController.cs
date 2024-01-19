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
        public async Task<ActionResult<IEnumerable<DiasDeFuncionamentoDTO>>> ObterTodosDias()
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
                    if (diasDeFuncionamentoDTO.Segunda != "false" && diasDeFuncionamentoDTO.Segunda != "true")
                    {
                        var erro1 = new { Mensagem = "Resposta inválida" };
                        return BadRequest(erro1);
                    }
                    if (diasDeFuncionamentoDTO.Terca != "false" && diasDeFuncionamentoDTO.Terca != "true")
                    {
                        var erro1 = new { Mensagem = "Resposta inválida" };
                        return BadRequest(erro1);
                    }
                    if (diasDeFuncionamentoDTO.Quarta != "false" && diasDeFuncionamentoDTO.Quarta != "true")
                    {
                        var erro1 = new { Mensagem = "Resposta inválida" };
                        return BadRequest(erro1);
                    }
                    if (diasDeFuncionamentoDTO.Quinta != "false" && diasDeFuncionamentoDTO.Quinta != "true")
                    {
                        var erro1 = new { Mensagem = "Resposta inválida" };
                        return BadRequest(erro1);
                    }
                    if (diasDeFuncionamentoDTO.Sexta != "false" && diasDeFuncionamentoDTO.Sexta != "true")
                    {
                        var erro1 = new { Mensagem = "Resposta inválida" };
                        return BadRequest(erro1);
                    }
                    if (diasDeFuncionamentoDTO.Sabado != "false" && diasDeFuncionamentoDTO.Sabado != "true")
                    {
                        var erro1 = new { Mensagem = "Resposta inválida" };
                        return BadRequest(erro1);
                    }
                    if (diasDeFuncionamentoDTO.Domingo != "false" && diasDeFuncionamentoDTO.Domingo != "true")
                    {
                        var erro1 = new { Mensagem = "Resposta inválida" };
                        return BadRequest(erro1);
                    }
                    diasDeFuncionamentoDTO.Id_dias = ObterIdDias(diasDeFuncionamentoDTO.RestauranteId);
                    var produtoExistente = await db.SingleOrDefaultAsync<DiasDeFuncionamento>("SELECT * FROM diasfuncionamento WHERE Id_dias = @0", diasDeFuncionamentoDTO.Id_dias);

                    if (produtoExistente == null)
                    {
                        diasDeFuncionamentoDTO.Id_dias = 0;
                        var novodia = mapper.Map<DiasDeFuncionamento>(diasDeFuncionamentoDTO);
                        await db.InsertAsync("diasfuncionamento", "Id_dias", true, novodia);
                    }
                    else
                    {
                        var novodia2 = mapper.Map<DiasDeFuncionamento>(diasDeFuncionamentoDTO);
                        await db.UpdateAsync("diasfuncionamento", "Id_dias", novodia2);
                    }
                }
            }
            return Ok();
        }

        [HttpGet("ObterIdDias")]
        public int ObterIdDias(int restauranteid)
        {
            using (var db = new Database(conexaodb, "MySql.Data.MySqlClient"))
            {
                var usuario = db.FirstOrDefault<DiasDeFuncionamento>("SELECT Id_dias FROM diasfuncionamento WHERE RestauranteId = @0", restauranteid);
                if(usuario == null)
                {
                    return 0;
                }
                else
                {
                    return usuario.Id_dias;
                }
                
            }
        }

        private bool DiasDeFuncionamentoExists(int id)
        {
            return (_context.DiasDeFuncionamentos?.Any(e => e.Id_dias == id)).GetValueOrDefault();
        }
    }
}
