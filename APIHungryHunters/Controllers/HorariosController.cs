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
    public class HorariosController : ControllerBase
    {
        private readonly TodoContext _context;

        public HorariosController(TodoContext context)
        {
            _context = context;
        }
        string conexaodb = "Server=localhost;Port=3306;Database=hungryhunters;Uid=root;";

        [HttpGet("ListadeHorarios")]
        public async Task<ActionResult<IEnumerable<HorariosDTO>>> ObterTodosHorararios()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Horarios, HorariosDTO>();
            });
            AutoMapper.IMapper mapper = config.CreateMapper();
            using (var db = new Database(conexaodb, "MySql.Data.MySqlClient"))
            {
                var Todoshorarios = await db.FetchAsync<Horarios>("SELECT * FROM horariosreservas");
                var responseItems = mapper.Map<List<HorariosDTO>>(Todoshorarios);
                return Ok(responseItems);
            }
        }

        [HttpGet("ListadeHorariospor{RestauranteId}")]
        public async Task<ActionResult<IEnumerable<HorariosDTO>>> ObterHorariosporIdRestaurante(int RestauranteId)
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Horarios, HorariosDTO>();
            });
            AutoMapper.IMapper mapper = config.CreateMapper();
            using (var db = new Database(conexaodb, "MySql.Data.MySqlClient"))
            {
                var HorariosporRestauranteId = await db.FetchAsync<Horarios>("SELECT * FROM horariosreservas WHERE RestauranteId = @0", RestauranteId);
                if (HorariosporRestauranteId == null)
                {
                    return NotFound($"Não foi encontrada nenhum horário com o Id: {RestauranteId}. Insira outro Id.");
                }
                var responseItems = mapper.Map<List<HorariosDTO>>(HorariosporRestauranteId);
                return Ok(responseItems);
            }
        }

        [HttpGet("ListadeHorariosporIdHorario/{IdHorario}")]
        public async Task<ActionResult<IEnumerable<HorariosDTO>>> ObterHorariosporId(int IdHorario)
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Horarios, HorariosDTO>();
            });
            AutoMapper.IMapper mapper = config.CreateMapper();
            using (var db = new Database(conexaodb, "MySql.Data.MySqlClient"))
            {
                var HorariosporId = await db.FetchAsync<Horarios>("SELECT * FROM horariosreservas WHERE Id_horarios = @0", IdHorario);
                if (HorariosporId == null)
                {
                    return NotFound($"Não foi encontrada nenhum horário com o Id: {IdHorario}. Insira outro Id.");
                }
                var responseItems = mapper.Map<List<HorariosDTO>>(HorariosporId);
                return Ok(responseItems);
            }
        }

        [HttpPost("AdicionarHorarios")]
        public async Task<ActionResult> AdicionarHorarios([FromBody] List<HorariosDTO> horariosDTOs)
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<HorariosDTO, Horarios>();
            });
            AutoMapper.IMapper mapper = config.CreateMapper();

            using (var db = new Database(conexaodb, "MySql.Data.MySqlClient"))
            {
                foreach (var horariosDTO in horariosDTOs)
                {
                    var existingHorario = await db.SingleOrDefaultAsync<Horarios>(
                   "SELECT * FROM horariosreservas WHERE RestauranteId = @RestauranteId AND HoraReserva = @HoraReserva",
                   new
                   {
                       horariosDTO.RestauranteId,
                       HoraReserva = horariosDTO.HoraReserva
                   });

                    if (existingHorario != null)
                    {
                        var erro5 = new { Mensagem = "Já existe um horario para este restaurante nesta data." };
                        return BadRequest(erro5);
                    }

                    var novohorario = mapper.Map<Horarios>(horariosDTO);

                    await db.InsertAsync("horariosreservas", "Id_horarios", true, novohorario);
                }
            }
            return Ok();
        }

        [HttpPost("DeleteHora/{horaReserva}/{RestauranteId}")]
        public async Task<ActionResult> DeleteHora(string horaReserva, int RestauranteId)
        {
            try
            {
                using (var db = new Database(conexaodb, "MySql.Data.MySqlClient"))
                {
                    var todoshorarios = await db.SingleOrDefaultAsync<Horarios>("SELECT * FROM horariosreservas WHERE RestauranteId = @0 AND HoraReserva = @1", RestauranteId, horaReserva);

                    if (todoshorarios == null)
                    {
                        return NotFound($"Não foi encontrado nenhum Brinquedo com o Id: {horaReserva}. Insira outro Id.");
                    }
                    else
                    {
                        await db.DeleteAsync("horariosreservas", "Id_horarios", todoshorarios);
                    }
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Erro ao excluir brinquedo(s)");
            }
        }

        

        private bool HorariosExists(int id)
        {
            return (_context.Horarios?.Any(e => e.Id_horarios == id)).GetValueOrDefault();
        }
    }
}
