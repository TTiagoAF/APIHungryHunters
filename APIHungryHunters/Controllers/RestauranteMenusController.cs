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
    public class RestauranteMenusController : ControllerBase
    {
        private readonly TodoContext _context;

        public RestauranteMenusController(TodoContext context)
        {
            _context = context;
        }
        string conexaodb = "Server=localhost;Port=3306;Database=hungryhunters;Uid=root;";

        [HttpGet("ListadeMenus")]
        public async Task<ActionResult<IEnumerable<RestauranteMenuDTO>>> ObterTodasOsMenus()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<RestauranteMenu, RestauranteMenuDTO>();
            });
            AutoMapper.IMapper mapper = config.CreateMapper();
            using (var db = new Database(conexaodb, "MySql.Data.MySqlClient"))
            {
                var TodosMenus = await db.FetchAsync<RestauranteMenu>("SELECT * FROM restaurantepratos");
                var responseItems = mapper.Map<List<RestauranteMenuDTO>>(TodosMenus);
                return Ok(responseItems);
            }
        }

        [HttpGet("ListadeMenuspor{RestauranteId}")]
        public async Task<ActionResult<IEnumerable<RestauranteMenuDTO>>> ObterMenusporIdRestaurante(int RestauranteId)
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<RestauranteMenu, RestauranteMenuDTO>();
            });
            AutoMapper.IMapper mapper = config.CreateMapper();
            using (var db = new Database(conexaodb, "MySql.Data.MySqlClient"))
            {
                var MenusporRestauranteId = await db.FetchAsync<RestauranteMenu>("SELECT * FROM restaurantepratos WHERE RestauranteId = @0", RestauranteId);
                if (MenusporRestauranteId == null)
                {
                    return NotFound($"Não foi encontrada nenhuma prato com o Id: {RestauranteId}. Insira outro Id.");
                }
                var responseItems = mapper.Map<List<RestauranteMenuDTO>>(MenusporRestauranteId);
                return Ok(responseItems);
            }
        }

        [HttpGet("Lista/{RestauranteId}/{Categoria}")]
        public async Task<ActionResult<IEnumerable<RestauranteMenuDTO>>> Categorias(int RestauranteId, string Categoria)
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<RestauranteMenu, RestauranteMenuDTO>();
            });
            AutoMapper.IMapper mapper = config.CreateMapper();
            using (var db = new Database(conexaodb, "MySql.Data.MySqlClient"))
            {
                var MenusporRestauranteId = await db.FetchAsync<RestauranteMenu>("SELECT * FROM restaurantepratos WHERE RestauranteId = @0 AND CategoriaPrato = @1", RestauranteId, Categoria);
                if (MenusporRestauranteId == null)
                {
                    return NotFound($"Não foi encontrada nenhuma prato com o Id: {RestauranteId}. Insira outro Id.");
                }
                var responseItems = mapper.Map<List<RestauranteMenuDTO>>(MenusporRestauranteId);
                return Ok(responseItems);
            }
        }

        [HttpGet("ListadeMenusporIdMenus/{IdMenu}")]
        public async Task<ActionResult<IEnumerable<RestauranteMenuDTO>>> ObterMenusporId(int IdMenu)
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<RestauranteMenu, RestauranteMenuDTO>();
            });
            AutoMapper.IMapper mapper = config.CreateMapper();
            using (var db = new Database(conexaodb, "MySql.Data.MySqlClient"))
            {
                var MenuporId = await db.FetchAsync<RestauranteMenu>("SELECT * FROM restaurantepratos WHERE Id_pratos = @0", IdMenu);
                if (MenuporId == null)
                {
                    return NotFound($"Não foi encontrada nenhum prato com o Id: {IdMenu}. Insira outro Id.");
                }
                var responseItems = mapper.Map<List<RestauranteMenuDTO>>(MenuporId);
                return Ok(responseItems);
            }
        }

        [HttpPost("AdicionarMenu")]
        public async Task<ActionResult> AdicionarMenu([FromBody] List<RestauranteMenuDTO> restauranteMenuDTOs)
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<RestauranteMenuDTO, RestauranteMenu>();
            });
            AutoMapper.IMapper mapper = config.CreateMapper();

            using (var db = new Database(conexaodb, "MySql.Data.MySqlClient"))
            {
                foreach (var restauranteMenuDTO in restauranteMenuDTOs)
                {
                    if (string.IsNullOrWhiteSpace(restauranteMenuDTO.Nome))
                    {
                        var erro1 = new { Mensagem = "Nome inválido" };
                        return BadRequest(erro1);
                    }
                    if (restauranteMenuDTO.Preco <= 0)
                    {
                        var erro1 = new { Mensagem = "Adicione o preço" };
                        return BadRequest(erro1);
                    }
                    if (string.IsNullOrWhiteSpace(restauranteMenuDTO.Desc_prato))
                    {
                        var erro1 = new { Mensagem = "Descrição inválida" };
                        return BadRequest(erro1);
                    }
                    if (string.IsNullOrWhiteSpace(restauranteMenuDTO.CategoriaPrato))
                    {
                        var erro1 = new { Mensagem = "Categoria inválida inválida" };
                        return BadRequest(erro1);
                    }

                    var existingPrato = await db.SingleOrDefaultAsync<RestauranteMenu>(
                    "SELECT * FROM restaurantepratos WHERE RestauranteId = @RestauranteId " +
                    "AND Nome = @Nome",
                     new
                     {
                         restauranteMenuDTO.RestauranteId,
                         Nome = restauranteMenuDTO.Nome,
                     });

                    if (existingPrato != null)
                    {
                        var erro5 = new { Mensagem = "Já existe este prato para este restaurante elimina o prato primeiro para atualizar." };
                        return BadRequest(erro5);
                    }

                    var novoPrato = mapper.Map<RestauranteMenu>(restauranteMenuDTO);

                    await db.InsertAsync("restaurantepratos", "Id_pratos", true, novoPrato);
                }
            }
            return Ok();
        }

        [HttpPost("EliminarPratos/{PratosId}")]
        public async Task<ActionResult> DeletePratos(int PratosId)
        {
            try
            {
                using (var db = new Database(conexaodb, "MySql.Data.MySqlClient"))
                {
                    var todospratos = await db.SingleOrDefaultAsync<RestauranteMenu>("SELECT * FROM restaurantepratos WHERE Id_pratos = @0", PratosId);

                    if (todospratos == null)
                    {
                        return NotFound($"Não foi encontrado nenhum prato com o id: {PratosId}.");
                    }
                    else
                    {
                        await db.DeleteAsync("restaurantepratos", "Id_pratos", todospratos);
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
