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
    [Authorize]
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

        [HttpGet("ListadeRestaurantesAutorizados")]
        public async Task<ActionResult<IEnumerable<RestaurantesDTO>>> GetAutorizados()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Restaurantes, RestaurantesDTO>();
            });
            AutoMapper.IMapper mapper = config.CreateMapper();

            using (var db = new Database(conexaodb, "MySql.Data.MySqlClient"))
            {
                var restaurantes = await db.FetchAsync<Restaurantes>("SELECT * FROM restaurantes WHERE Autorizado = @0", "true");

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
                    return NotFound($"Não foi encontrada nenhum restaurante com o Id: {id}. Insira outro Id.");
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
                return NotFound($"Não foi encontrada nenhum restaurante com o Nome: {nome}. Insira outro Nome.");
            }

            var restaurantesDTO = mapper.Map<List<RestaurantesDTO>>(restaurantes);

            return Ok(restaurantesDTO);
       }
    }

        [HttpGet("RestaurantesporNipc/{nipc}")]
        public async Task<ActionResult<IEnumerable<RestaurantesDTO>>> RestaurantesporNipc(string nipc)
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Restaurantes, RestaurantesDTO>();
            });
            AutoMapper.IMapper mapper = config.CreateMapper();

            using (var db = new Database(conexaodb, "MySql.Data.MySqlClient"))
            {
                var restaurantes = await db.FetchAsync<Restaurantes>("SELECT * FROM restaurantes WHERE NipcEmpresa = @0", nipc);

                if (restaurantes == null)
                {
                    return NotFound($"Não foi encontrada nenhum restaurante com o Nipc: {nipc}. Insira outro Nipc.");
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
                        var erro1 = new { Mensagem = "Preencha o nome" };
                        return BadRequest(erro1);
                    }
                    
                    var existingNome = await db.FirstOrDefaultAsync<string>("SELECT Nome FROM restaurantes WHERE Nome = @Nome", new { Nome = restaurantesDTO.Nome });
                    if (!string.IsNullOrEmpty(existingNome))
                    {
                        var erro1 = new { Mensagem = "Este nome já está a ser utilizado" };
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
                        var erro1 = new { Mensagem = "Coordenadas inválidas" };
                        return BadRequest(erro1);
                    }

                    if (string.IsNullOrWhiteSpace(restaurantesDTO.Descricao))
                    {
                        var erro1 = new { Mensagem = "Descrição inválida" };
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

        [HttpPost("DeleteRestaurantes/{nome}")]
        public async Task<ActionResult> DeleteRestaurantes(string nome)
        {
            try
            {
                using (var db = new Database(conexaodb, "MySql.Data.MySqlClient"))
                {
                        var todosBrinquedos = await db.SingleOrDefaultAsync<Restaurantes>("SELECT * FROM restaurantes WHERE Nome = @0", nome);

                        if (todosBrinquedos == null)
                        {
                            return NotFound($"Não foi encontrado nenhum Brinquedo com o Id: {nome}. Insira outro Id.");
                        }
                        else
                        {
                            await db.DeleteAsync("restaurantes", "Nome", todosBrinquedos);
                        }
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Erro ao excluir brinquedo(s)");
            }
        }

        [HttpPost("MenosMesasporId/{IdRestaurante}")]
        public async Task<ActionResult> MenosMesasId(string IdRestaurante)
        {
            try
            {
                using (var db = new Database(conexaodb, "MySql.Data.MySqlClient"))
                {
                    var num = await db.SingleOrDefaultAsync<Restaurantes>("SELECT * FROM restaurantes WHERE Id_restaurante = @0", IdRestaurante);

                    if (num == null)
                    {
                        return NotFound($"Não foi encontrado nenhuma empresa com o Nipc: {IdRestaurante}. Insira outro Nipc.");
                    }

                    num.NumeroMesas -= 1;

                    await db.UpdateAsync("restaurantes", "Id_restaurante", num);

                    return NoContent();
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Erro ao atualizar");
            }
        }

        [HttpPost("MaisMesasporId/{IdRestaurante}")]
        public async Task<ActionResult> MaisMesasId(string IdRestaurante)
        {
            try
            {
                using (var db = new Database(conexaodb, "MySql.Data.MySqlClient"))
                {
                    var num = await db.SingleOrDefaultAsync<Restaurantes>("SELECT * FROM restaurantes WHERE Id_restaurante = @0", IdRestaurante);

                    if (num == null)
                    {
                        return NotFound($"Não foi encontrado nenhuma empresa com o Nipc: {IdRestaurante}. Insira outro Nipc.");
                    }

                    num.NumeroMesas += 1;

                    await db.UpdateAsync("restaurantes", "Id_restaurante", num);

                    return NoContent();
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Erro ao atualizar");
            }
        }

        private bool RestaurantesExists(long id)
        {
            return (_context.Restaurantes?.Any(e => e.Id_restaurante == id)).GetValueOrDefault();
        }
    }
}
