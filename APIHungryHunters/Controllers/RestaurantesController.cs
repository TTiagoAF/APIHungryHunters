using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using APIHungryHunters.Models;
using Newtonsoft.Json;
using System.Text;
using PetaPoco;
using System.Data;
using MySql.Data.MySqlClient;
using Humanizer;
using AutoMapper;
using Org.BouncyCastle.Crypto.Generators;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace APIHungryHunters.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RestaurantesController : ControllerBase
    {
        private readonly TodoContext _context;
        private readonly IConfiguration _configuration;

        public RestaurantesController(TodoContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        string conexaodb = "Server=localhost;Port=3306;Database=hungryhunters;Uid=root;";

        [HttpGet("ListadeRestaurantes")]
        public async Task<ActionResult<IEnumerable<RestaurantesDTO>>> GetRestaurantes()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Restaurantes, RestaurantesDTO>();
            });
            AutoMapper.IMapper mapper = config.CreateMapper();

            using (var db = new Database(conexaodb, "MySql.Data.MySqlClient"))
            {
                var restaurantes = await db.FetchAsync<Restaurantes>("SELECT * FROM restaurantes");

                var responseItems = mapper.Map<List<RestaurantesDTO>>(restaurantes);

                return Ok(responseItems);
            }
        }
        [HttpGet("BuscarRestaurantepor{id}")]
        public async Task<ActionResult<IEnumerable<RestaurantesDTO>>> GetRestaurantes(long id)
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Restaurantes, RestaurantesDTO>();
            });
            AutoMapper.IMapper mapper = config.CreateMapper();

            using (var db = new Database(conexaodb, "MySql.Data.MySqlClient"))
            {
                var restaurantes = await db.FetchAsync<Restaurantes>("SELECT * FROM restaurantes WHERE Id_restaurante = @0", id);

                if (restaurantes == null)
                {
                    return NotFound($"Não foi encontrada nenhuma Conta com o Id: {id}. Insira outro Id.");
                }

                var restaurantesDTO = mapper.Map<List<RestaurantesDTO>>(restaurantes);

                return Ok(restaurantesDTO);
            }
        }

        [HttpGet("Restaurantespor{nome}")]
        public async Task<ActionResult<IEnumerable<RestaurantesDTO>>> GetRestaurantesNome(string nome)
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Restaurantes, RestaurantesDTO>();
            });
            AutoMapper.IMapper mapper = config.CreateMapper();

            using (var db = new Database(conexaodb, "MySql.Data.MySqlClient"))
            {
                var restaurantes = await db.FetchAsync<Restaurantes>("SELECT * FROM restaurantes WHERE Nome = @0", nome);

                if (restaurantes == null)
                {
                    return NotFound($"Não foi encontrada nenhuma Conta com o Id: {nome}. Insira outro Id.");
                }

                var restaurantesDTO = mapper.Map<List<RestaurantesDTO>>(restaurantes);

                return Ok(restaurantesDTO);
            }
        }
        [HttpPost("AdicionarRestaurante")]
        public async Task<ActionResult> AddRestaurante([FromBody] List<RestaurantesDTO> restaurantesDTOs)
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<RestaurantesDTO, Restaurantes>();
            });
            AutoMapper.IMapper mapper = config.CreateMapper();

            using (var db = new Database(conexaodb, "MySql.Data.MySqlClient"))
            {
                foreach (var restaurantesDTO in restaurantesDTOs)
                {
                    var restaurantes = await db.FirstOrDefaultAsync<string>("SELECT Nipc FROM empresa WHERE Nipc = @Nipc", new { Nipc = restaurantesDTO.NipcEmpresa });
                    if (string.IsNullOrEmpty(restaurantes))
                    {
                        return NotFound($"Não foi encontrada nenhuma Empresa com esse Nipc: {restaurantesDTO.NipcEmpresa}. Insira outro Nipc.");
                    }

                    if (string.IsNullOrWhiteSpace(restaurantesDTO.Nome))
                    {
                        var erro1 = new { Mensagem = "Preencha a razão social" };
                        return BadRequest(erro1);
                    }
                    
                    var existingNome = await db.FirstOrDefaultAsync<string>("SELECT Nome FROM restaurantes WHERE Nome = @Nome", new { Nome = restaurantesDTO.Nome });
                    if (!string.IsNullOrEmpty(existingNome))
                    {
                        var erro1 = new { Mensagem = "Este Razão Social já está a ser utilizado." };
                        return BadRequest(erro1);
                    }

                    if (restaurantesDTO.PrecoMedio <= 0)
                    {
                        var erro4 = new { Mensagem = "Preço inválido." };
                        return BadRequest(erro4);
                    }

                    if (restaurantesDTO.NumeroMesas <= 0)
                    {
                        var erro1 = new { Mensagem = "Adicione o número de mesas" };
                        return BadRequest(erro1);
                    }

                    if (string.IsNullOrWhiteSpace(restaurantesDTO.Distrito))
                    {
                        var erro1 = new { Mensagem = "Distrito inválido" };
                        return BadRequest(erro1);
                    }

                    if (string.IsNullOrWhiteSpace(restaurantesDTO.Coordenadas))
                    {
                        var erro1 = new { Mensagem = "Coordenadas inválido" };
                        return BadRequest(erro1);
                    }

                    if (string.IsNullOrWhiteSpace(restaurantesDTO.Descricao))
                    {
                        var erro1 = new { Mensagem = "Descrição inválido" };
                        return BadRequest(erro1);
                    }
                    if (restaurantesDTO.CapacidadeGrupo <= 0)
                    {
                        var erro1 = new { Mensagem = "Adicione o número de capacidade por grupo" };
                        return BadRequest(erro1);
                    }

                    var novoRestaurante = mapper.Map<Restaurantes>(restaurantesDTO);
                    await db.InsertAsync("restaurantes", "Id_restaurante", true, novoRestaurante);
                }
            }
            return Ok();
        }

        [HttpGet("ObterIdDoRestaurantePorEmail")]
        public int? ObterIdDoRestaurantePorEmail(string email)
        {
            using (var db = new Database(conexaodb, "MySql.Data.MySqlClient"))
            {
                var user = db.FirstOrDefault<Restaurantes>("SELECT Id_restaurante FROM restaurantes WHERE Email = @0", email);

                return user?.Id_restaurante;
            }
        }
        // DELETE: api/Restaurantes/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRestaurantes(long id)
        {
            if (_context.Restaurantes == null)
            {
                return NotFound();
            }
            var restaurantes = await _context.Restaurantes.FindAsync(id);
            if (restaurantes == null)
            {
                return NotFound();
            }

            _context.Restaurantes.Remove(restaurantes);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool RestaurantesExists(long id)
        {
            return (_context.Restaurantes?.Any(e => e.Id_restaurante == id)).GetValueOrDefault();
        }
    }
}
