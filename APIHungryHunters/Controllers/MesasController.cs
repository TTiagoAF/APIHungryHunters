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
    public class MesasController : ControllerBase
    {
        private readonly TodoContext _context;

        public MesasController(TodoContext context)
        {
            _context = context;
        }
        string conexaodb = "Server=localhost;Port=3306;Database=hungryhunters;Uid=root;";

        [HttpGet("ListadeMesas")]
        public async Task<ActionResult<IEnumerable<MesasDTO>>> ObterTodasOsMenus()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Mesas, MesasDTO>();
            });
            AutoMapper.IMapper mapper = config.CreateMapper();
            using (var db = new Database(conexaodb, "MySql.Data.MySqlClient"))
            {
                var TodasMesas = await db.FetchAsync<Mesas>("SELECT * FROM mesas");
                var responseItems = mapper.Map<List<MesasDTO>>(TodasMesas);
                return Ok(responseItems);
            }
        }

        [HttpGet("ListadeMesaspor{RestauranteId}")]
        public async Task<ActionResult<IEnumerable<MesasDTO>>> ObterMesasporIdRestaurante(int RestauranteId)
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Mesas, MesasDTO>();
            });
            AutoMapper.IMapper mapper = config.CreateMapper();
            using (var db = new Database(conexaodb, "MySql.Data.MySqlClient"))
            {
                var MesasporRestauranteId = await db.FetchAsync<Mesas>("SELECT * FROM mesas WHERE RestauranteId = @0", RestauranteId);
                if (MesasporRestauranteId == null)
                {
                    return NotFound($"Não foi encontrada nenhuma mesa com o Id: {RestauranteId}. Insira outro Id.");
                }
                var responseItems = mapper.Map<List<MesasDTO>>(MesasporRestauranteId);
                return Ok(responseItems);
            }
        }

        [HttpGet("ListadeMesasporIdMesa/{IdMesa}")]
        public async Task<ActionResult<IEnumerable<MesasDTO>>> ObterMesaporId(int IdMesa)
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Mesas, MesasDTO>();
            });
            AutoMapper.IMapper mapper = config.CreateMapper();
            using (var db = new Database(conexaodb, "MySql.Data.MySqlClient"))
            {
                var MesaporId = await db.FetchAsync<Mesas>("SELECT * FROM mesas WHERE Id_mesa = @0", IdMesa);
                if (MesaporId == null)
                {
                    return NotFound($"Não foi encontrada nenhum mesa com o Id: {IdMesa}. Insira outro Id.");
                }
                var responseItems = mapper.Map<List<MesasDTO>>(MesaporId);
                return Ok(responseItems);
            }
        }

        [HttpPost("AdicionarMesa")]
        public async Task<ActionResult> AdicionarMesas([FromBody] List<MesasDTO> mesasDTOs)
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<MesasDTO, Mesas>();
            });
            AutoMapper.IMapper mapper = config.CreateMapper();

            using (var db = new Database(conexaodb, "MySql.Data.MySqlClient"))
            {
                foreach (var mesasDTO in mesasDTOs)
                {
                    if (string.IsNullOrWhiteSpace(mesasDTO.Nome))
                    {
                        var erro1 = new { Mensagem = "Nome inválido" };
                        return BadRequest(erro1);
                    }
                    if (mesasDTO.Maximo_pessoas <= 0)
                    {
                        var erro1 = new { Mensagem = "Adicione o máximo de pessoas" };
                        return BadRequest(erro1);
                    }
                    if (string.IsNullOrWhiteSpace(mesasDTO.Notas))
                    {
                        var erro1 = new { Mensagem = "Notas inválida" };
                        return BadRequest(erro1);
                    }

                    var novaMesa = mapper.Map<Mesas>(mesasDTO);

                    await db.InsertAsync("mesas", "Id_mesa", true, novaMesa);
                }
            }
            return Ok();
        }

        [HttpPost("EliminarMesas/{MesasId}")]
        public async Task<ActionResult> DeleteMesas(int MesasId)
        {
            try
            {
                using (var db = new Database(conexaodb, "MySql.Data.MySqlClient"))
                {
                    var todasMesas = await db.SingleOrDefaultAsync<Mesas>("SELECT * FROM mesas WHERE Id_mesa = @0", MesasId);

                    if (todasMesas == null)
                    {
                        return NotFound($"Não foi encontrado nenhuma mesa com o id: {MesasId}.");
                    }
                    else
                    {
                        await db.DeleteAsync("mesas", "Id_mesa", todasMesas);
                    }
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Erro ao excluir horario");
            }
        }
    }
}
