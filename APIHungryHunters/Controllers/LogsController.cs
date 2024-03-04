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
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class LogsController : ControllerBase
    {
        private readonly TodoContext _context;

        public LogsController(TodoContext context)
        {
            _context = context;
        }

        string conexaodb = "Server=localhost;Port=3306;Database=hungryhunters;Uid=root;";

        [HttpGet("ListadeLogscom")]
        public async Task<ActionResult<IEnumerable<TodasLogs>>> ObterLogscomRestauranteId([FromQuery] int RestauranteId)
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Logs, TodasLogs>();
            });
            AutoMapper.IMapper mapper = config.CreateMapper();

            using (var db = new Database(conexaodb, "MySql.Data.MySqlClient"))
            {
                var LogsRestauranteId = await db.FetchAsync<Logs>("SELECT * FROM logs WHERE RestauranteId = @0 ORDER BY teste DESC", RestauranteId);

                if (LogsRestauranteId == null || LogsRestauranteId.Count == 0)
                {
                    return NotFound($"Não foi encontrada nenhuma avaliação com o Id: {RestauranteId}. Insira outro Id.");
                }

                var responseItems = new List<TodasLogs>();

                foreach (var logs in LogsRestauranteId)
                {
                    var IdContas = ObterNomeDoRestaurante(logs.RestauranteId);
                    var Cliente = ObterNomeDoCliente(logs.ContaId);

					logs.NomeRestaurante = IdContas;
					logs.NomeCliente = Cliente;

                    var responseItem = mapper.Map<TodasLogs>(logs);
                    responseItems.Add(responseItem);
                }

                return Ok(responseItems);
            }
        }

		[HttpGet("ObterNomeDoCliente")]
		public string ObterNomeDoCliente(int idconta)
		{
			using (var db = new Database(conexaodb, "MySql.Data.MySqlClient"))
			{
				var nome = "Reserva feita pelo restaurante";
				if (idconta == 0)
				{
					return nome;
				}
				var usuario = db.FirstOrDefault<Contas>("SELECT Username FROM contas WHERE Id_conta = @0", idconta);

				return usuario.Username;
			}
		}

		[HttpGet("ObterNomeDoRestaurante")]
        public string ObterNomeDoRestaurante(int idrestaurante)
        {
            using (var db = new Database(conexaodb, "MySql.Data.MySqlClient"))
            {
                var usuario = db.FirstOrDefault<Restaurantes>("SELECT Nome FROM restaurantes WHERE Id_restaurante = @0", idrestaurante);

                return usuario.Nome;
            }
        }

        [HttpPost("AdicionarLogs")]
        public async Task<ActionResult> AdicionarLogs([FromBody] List<LogsDTO> logsDTOs)
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<LogsDTO, Logs>();
            });
            AutoMapper.IMapper mapper = config.CreateMapper();

            using (var db = new Database(conexaodb, "MySql.Data.MySqlClient"))
            {
                foreach (var logsDTO in logsDTOs)
                {                    
                    var novaLog = mapper.Map<Logs>(logsDTO);
                    await db.InsertAsync("logs", "Id_log", true, novaLog);
                }
            }
            return Ok();
        }
    }
}