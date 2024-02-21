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

        [HttpGet("ListadeRestaurantesComCategorias")]
        public async Task<ActionResult<IEnumerable<TodosRestaurantes>>> GetRestauranteseCategorias()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Restaurantes, TodosRestaurantes>();
                cfg.CreateMap<Categorias, CategoriasDTO>();
            });
            AutoMapper.IMapper mapper = config.CreateMapper();

            using (var db = new Database(conexaodb, "MySql.Data.MySqlClient"))
            {
                var restaurantes = await db.FetchAsync<Restaurantes>("SELECT * FROM restaurantes ORDER BY RAND() LIMIT 5");

                foreach(var restaurante in restaurantes)
                {
                    var categorias = await db.FetchAsync<Categorias>("SELECT * FROM restaurantecategorias WHERE RestauranteId = @0", restaurante.Id_restaurante);
                    restaurante.Categorias = categorias;
                }
                
                var responseItems = mapper.Map<List<TodosRestaurantes>>(restaurantes);

                return Ok(responseItems);
            }
        }

        [HttpGet("TodaListadeRestaurantesComCategorias")]
        public async Task<ActionResult<IEnumerable<TodosRestaurantes>>> GetTodosRestauranteseCategorias()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Restaurantes, TodosRestaurantes>();
                cfg.CreateMap<Categorias, CategoriasDTO>();
            });
            AutoMapper.IMapper mapper = config.CreateMapper();

            using (var db = new Database(conexaodb, "MySql.Data.MySqlClient"))
            {
                var restaurantes = await db.FetchAsync<Restaurantes>("SELECT * FROM restaurantes ORDER BY RAND()");

                foreach (var restaurante in restaurantes)
                {
                    var categorias = await db.FetchAsync<Categorias>("SELECT * FROM restaurantecategorias WHERE RestauranteId = @0", restaurante.Id_restaurante);
                    restaurante.Categorias = categorias;
                }

                var responseItems = mapper.Map<List<TodosRestaurantes>>(restaurantes);

                return Ok(responseItems);
            }
        }

        [HttpGet("ListadeRestaurantesComCategoriasLisboa")]
        public async Task<ActionResult<IEnumerable<TodosRestaurantes>>> GetRestauranteseCategoriasLisboa()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Restaurantes, TodosRestaurantes>();
                cfg.CreateMap<Categorias, CategoriasDTO>();
            });
            AutoMapper.IMapper mapper = config.CreateMapper();

            using (var db = new Database(conexaodb, "MySql.Data.MySqlClient"))
            {
                var restaurantes = await db.FetchAsync<Restaurantes>("SELECT * FROM restaurantes WHERE Distrito = @0 ORDER BY RAND() LIMIT 5", "Lisboa");

                foreach(var restaurante in restaurantes)
                {
                    var categorias = await db.FetchAsync<Categorias>("SELECT * FROM restaurantecategorias WHERE RestauranteId = @0", restaurante.Id_restaurante);
                    restaurante.Categorias = categorias;
                }
                
                var responseItems = mapper.Map<List<TodosRestaurantes>>(restaurantes);

                return Ok(responseItems);
            }
        }

        [HttpGet("ListadeRestaurantesComCategoriasPorto")]
        public async Task<ActionResult<IEnumerable<TodosRestaurantes>>> GetRestauranteseCategoriasPorto()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Restaurantes, TodosRestaurantes>();
                cfg.CreateMap<Categorias, CategoriasDTO>();
            });
            AutoMapper.IMapper mapper = config.CreateMapper();

            using (var db = new Database(conexaodb, "MySql.Data.MySqlClient"))
            {
                var restaurantes = await db.FetchAsync<Restaurantes>("SELECT * FROM restaurantes WHERE Distrito = @0 ORDER BY RAND() LIMIT 5", "Porto");

                foreach (var restaurante in restaurantes)
                {
                    var categorias = await db.FetchAsync<Categorias>("SELECT * FROM restaurantecategorias WHERE RestauranteId = @0", restaurante.Id_restaurante);
                    restaurante.Categorias = categorias;
                }

                var responseItems = mapper.Map<List<TodosRestaurantes>>(restaurantes);

                return Ok(responseItems);
            }
        }

        [HttpGet("ListadeRestaurantesMelhorComida")]
        public async Task<ActionResult<IEnumerable<TodosRestaurantes>>> GetRestauranteseMelhorComida()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Restaurantes, TodosRestaurantes>();
                cfg.CreateMap<Categorias, CategoriasDTO>();
                cfg.CreateMap<Avaliacoes, AvaliacoesDTO>();
            });
            AutoMapper.IMapper mapper = config.CreateMapper();

            using (var db = new Database(conexaodb, "MySql.Data.MySqlClient"))
            {
                var query = "SELECT r.*, AVG(a.Comida) AS MediaComida " +
                            "FROM restaurantes r " +
                            "LEFT JOIN avaliacoes a ON r.Id_restaurante = a.RestauranteId " +
                            "GROUP BY r.Id_restaurante " +
                            "ORDER BY MediaComida DESC LIMIT 5";

                var restaurantes = await db.FetchAsync<TodosRestaurantes>(query);

                foreach (var restaurante in restaurantes)
                {
                    var categorias = await db.FetchAsync<Categorias>("SELECT * FROM restaurantecategorias WHERE RestauranteId = @0", restaurante.Id_restaurante);
                    restaurante.Categorias = categorias;
                }

                return Ok(restaurantes);
            }
        }

        [HttpGet("ListadeRestaurantesMelhorConforto")]
        public async Task<ActionResult<IEnumerable<TodosRestaurantes>>> GetRestauranteseMelhorConforto()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Restaurantes, TodosRestaurantes>();
                cfg.CreateMap<Categorias, CategoriasDTO>();
                cfg.CreateMap<Avaliacoes, AvaliacoesDTO>();
            });
            AutoMapper.IMapper mapper = config.CreateMapper();

            using (var db = new Database(conexaodb, "MySql.Data.MySqlClient"))
            {
                var query = "SELECT r.*, AVG(a.Conforto) AS MediaConforto " +
                            "FROM restaurantes r " +
                            "LEFT JOIN avaliacoes a ON r.Id_restaurante = a.RestauranteId " +
                            "GROUP BY r.Id_restaurante " +
                            "ORDER BY MediaConforto DESC LIMIT 5";

                var restaurantes = await db.FetchAsync<TodosRestaurantes>(query);

                foreach (var restaurante in restaurantes)
                {
                    var categorias = await db.FetchAsync<Categorias>("SELECT * FROM restaurantecategorias WHERE RestauranteId = @0", restaurante.Id_restaurante);
                    restaurante.Categorias = categorias;
                }

                return Ok(restaurantes);
            }
        }

        [HttpGet("ListadeRestaurantesMaisBonito")]
        public async Task<ActionResult<IEnumerable<TodosRestaurantes>>> GetRestauranteseMaisBonito()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Restaurantes, TodosRestaurantes>();
                cfg.CreateMap<Categorias, CategoriasDTO>();
                cfg.CreateMap<Avaliacoes, AvaliacoesDTO>();
            });
            AutoMapper.IMapper mapper = config.CreateMapper();

            using (var db = new Database(conexaodb, "MySql.Data.MySqlClient"))
            {
                var query = "SELECT r.*, AVG(a.Beleza) AS MediaBeleza " +
                            "FROM restaurantes r " +
                            "LEFT JOIN avaliacoes a ON r.Id_restaurante = a.RestauranteId " +
                            "GROUP BY r.Id_restaurante " +
                            "ORDER BY MediaBeleza DESC LIMIT 5";

                var restaurantes = await db.FetchAsync<TodosRestaurantes>(query);

                foreach (var restaurante in restaurantes)
                {
                    var categorias = await db.FetchAsync<Categorias>("SELECT * FROM restaurantecategorias WHERE RestauranteId = @0", restaurante.Id_restaurante);
                    restaurante.Categorias = categorias;
                }

                return Ok(restaurantes);
            }
        }

        [HttpGet("ListadeRestaurantesMelhorAtendimento")]
        public async Task<ActionResult<IEnumerable<TodosRestaurantes>>> GetRestauranteseMelhorAtendimento()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Restaurantes, TodosRestaurantes>();
                cfg.CreateMap<Categorias, CategoriasDTO>();
                cfg.CreateMap<Avaliacoes, AvaliacoesDTO>();
            });
            AutoMapper.IMapper mapper = config.CreateMapper();

            using (var db = new Database(conexaodb, "MySql.Data.MySqlClient"))
            {
                var query = "SELECT r.*, AVG(a.Atendimento) AS MediaAtendimento " +
                            "FROM restaurantes r " +
                            "LEFT JOIN avaliacoes a ON r.Id_restaurante = a.RestauranteId " +
                            "GROUP BY r.Id_restaurante " +
                            "ORDER BY MediaAtendimento DESC LIMIT 5";

                var restaurantes = await db.FetchAsync<TodosRestaurantes>(query);

                foreach (var restaurante in restaurantes)
                {
                    var categorias = await db.FetchAsync<Categorias>("SELECT * FROM restaurantecategorias WHERE RestauranteId = @0", restaurante.Id_restaurante);
                    restaurante.Categorias = categorias;
                }

                return Ok(restaurantes);
            }
        }

        [HttpGet("ListadeRestaurantesMaisRapido")]
        public async Task<ActionResult<IEnumerable<TodosRestaurantes>>> GetRestauranteseMaisRapido()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Restaurantes, TodosRestaurantes>();
                cfg.CreateMap<Categorias, CategoriasDTO>();
                cfg.CreateMap<Avaliacoes, AvaliacoesDTO>();
            });
            AutoMapper.IMapper mapper = config.CreateMapper();

            using (var db = new Database(conexaodb, "MySql.Data.MySqlClient"))
            {
                var query = "SELECT r.*, AVG(a.Velocidade) AS MediaVelocidade " +
                            "FROM restaurantes r " +
                            "LEFT JOIN avaliacoes a ON r.Id_restaurante = a.RestauranteId " +
                            "GROUP BY r.Id_restaurante " +
                            "ORDER BY MediaVelocidade DESC LIMIT 5";

                var restaurantes = await db.FetchAsync<TodosRestaurantes>(query);

                foreach (var restaurante in restaurantes)
                {
                    var categorias = await db.FetchAsync<Categorias>("SELECT * FROM restaurantecategorias WHERE RestauranteId = @0", restaurante.Id_restaurante);
                    restaurante.Categorias = categorias;
                }

                return Ok(restaurantes);
            }
        }

        [HttpGet("ListadeRestaurantesComCategoriasFaro")]
        public async Task<ActionResult<IEnumerable<TodosRestaurantes>>> GetRestauranteseCategoriasFaro()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Restaurantes, TodosRestaurantes>();
                cfg.CreateMap<Categorias, CategoriasDTO>();
            });
            AutoMapper.IMapper mapper = config.CreateMapper();

            using (var db = new Database(conexaodb, "MySql.Data.MySqlClient"))
            {
                var restaurantes = await db.FetchAsync<Restaurantes>("SELECT * FROM restaurantes WHERE Distrito = @0 ORDER BY RAND() LIMIT 5", "Faro");

                foreach (var restaurante in restaurantes)
                {
                    var categorias = await db.FetchAsync<Categorias>("SELECT * FROM restaurantecategorias WHERE RestauranteId = @0", restaurante.Id_restaurante);
                    restaurante.Categorias = categorias;
                }

                var responseItems = mapper.Map<List<TodosRestaurantes>>(restaurantes);

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
        public async Task<ActionResult<IEnumerable<TodosRestaurantes>>> GetRestaurantes(long id)
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Restaurantes, TodosRestaurantes>();
                cfg.CreateMap<RestauranteMenu, RestauranteMenuDTO>();
                cfg.CreateMap<Categorias, CategoriasDTO>();
            });
            AutoMapper.IMapper mapper = config.CreateMapper();

            using (var db = new Database(conexaodb, "MySql.Data.MySqlClient"))
            {
                var restaurantes = await db.FetchAsync<Restaurantes>("SELECT * FROM restaurantes WHERE Id_restaurante = @0", id);

                foreach (var restaurante in restaurantes)
                {
                    var menu = await db.FetchAsync<RestauranteMenu>("SELECT * FROM restaurantepratos WHERE RestauranteId = @0", id);
                    restaurante.RestauranteMenus = menu;

                    var categorias = await db.FetchAsync<Categorias>("SELECT * FROM restaurantecategorias WHERE RestauranteId = @0", id);
                    restaurante.Categorias = categorias;
                }

                var restaurantesDTO = mapper.Map<List<TodosRestaurantes>>(restaurantes);

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

        [HttpGet("PesquisaDeRestaurantes{nome}")]
        public async Task<ActionResult<IEnumerable<TodosRestaurantes>>> PesquisaDeRestaurantes(string nome)
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Restaurantes, TodosRestaurantes>();
                cfg.CreateMap<Categorias, CategoriasDTO>();
            });
            AutoMapper.IMapper mapper = config.CreateMapper();

            using (var db = new Database(conexaodb, "MySql.Data.MySqlClient"))
            {
                var restaurantes = await db.FetchAsync<Restaurantes>("SELECT * FROM restaurantes WHERE Nome LIKE @0 ORDER BY Nome",'%' + nome + '%');

                foreach (var restaurante in restaurantes)
                {
                    var categorias = await db.FetchAsync<Categorias>("SELECT * FROM restaurantecategorias WHERE RestauranteId = @0", restaurante.Id_restaurante);
                    restaurante.Categorias = categorias;
                }

                var restaurantesDTO = mapper.Map<List<TodosRestaurantes>>(restaurantes);

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

                    if (restaurantesDTO.Telemovel.Length != 9 || restaurantesDTO.Telemovel == "")
                    {
                        var erro1 = new { Mensagem = "Telemóvel inválido" };
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
                            return NotFound($"Não foi encontrado nenhum Restaurante com o nome: {nome}. Insira outro nome.");
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

        [HttpPost("MenosPessoasporId/{IdRestaurante}")]
        public async Task<ActionResult> MenosPessoasId(string IdRestaurante)
        {
            try
            {
                using (var db = new Database(conexaodb, "MySql.Data.MySqlClient"))
                {
                    var num = await db.SingleOrDefaultAsync<Restaurantes>("SELECT * FROM restaurantes WHERE Id_restaurante = @0", IdRestaurante);

                    if (num == null)
                    {
                        return NotFound($"Não foi encontrado nenhum restaurante com o Id: {IdRestaurante}. Insira outro Id.");
                    }

                    num.CapacidadeGrupo -= 1;

                    await db.UpdateAsync("restaurantes", "Id_restaurante", num);

                    return NoContent();
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpPost("MaisPessoasporId/{IdRestaurante}")]
        public async Task<ActionResult> MaisPessoasId(string IdRestaurante)
        {
            try
            {
                using (var db = new Database(conexaodb, "MySql.Data.MySqlClient"))
                {
                    var num = await db.SingleOrDefaultAsync<Restaurantes>("SELECT * FROM restaurantes WHERE Id_restaurante = @0", IdRestaurante);

                    if (num == null)
                    {
                        return NotFound($"Não foi encontrado nenhum restaurante com o Id: {IdRestaurante}. Insira outro Id.");
                    }

                    num.CapacidadeGrupo += 1;

                    await db.UpdateAsync("restaurantes", "Id_restaurante", num);

                    return NoContent();
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        private bool RestaurantesExists(long id)
        {
            return (_context.Restaurantes?.Any(e => e.Id_restaurante == id)).GetValueOrDefault();
        }
    }
}
