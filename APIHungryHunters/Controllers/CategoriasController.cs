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
    public class CategoriasController : ControllerBase
    {
        private readonly TodoContext _context;

        public CategoriasController(TodoContext context)
        {
            _context = context;
        }
        string conexaodb = "Server=localhost;Port=3306;Database=hungryhunters;Uid=root;";

        [HttpGet("ListadeCategorias")]
        public async Task<ActionResult<IEnumerable<CategoriasDTO>>> ObterTodasCategorias()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Categorias, CategoriasDTO>();
            });
            AutoMapper.IMapper mapper = config.CreateMapper();
            using (var db = new Database(conexaodb, "MySql.Data.MySqlClient"))
            {
                var TodasCategorias = await db.FetchAsync<Categorias>("SELECT * FROM restaurantecategorias");
                var responseItems = mapper.Map<List<CategoriasDTO>>(TodasCategorias);
                return Ok(responseItems);
            }
        }

        [HttpGet("ListadeCategoriaspor{RestauranteId}")]
        public async Task<ActionResult<IEnumerable<CategoriasDTO>>> ObterCategoriasporIdRestaurante(int RestauranteId)
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Categorias, CategoriasDTO>();
            });
            AutoMapper.IMapper mapper = config.CreateMapper();
            using (var db = new Database(conexaodb, "MySql.Data.MySqlClient"))
            {
                var CategoriasporRestauranteId = await db.FetchAsync<Categorias>("SELECT * FROM restaurantecategorias WHERE RestauranteId = @0", RestauranteId);
                if (CategoriasporRestauranteId == null)
                {
                    return NotFound($"Não foi encontrada nenhuma categoria com o Id: {RestauranteId}. Insira outro Id.");
                }
                var responseItems = mapper.Map<List<CategoriasDTO>>(CategoriasporRestauranteId);
                return Ok(responseItems);
            }
        }

        [HttpGet("ListadeCategoriasporIdCategorias/{IdCategorias}")]
        public async Task<ActionResult<IEnumerable<CategoriasDTO>>> ObterCategoriasporId(int IdCategorias)
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Categorias, CategoriasDTO>();
            });
            AutoMapper.IMapper mapper = config.CreateMapper();
            using (var db = new Database(conexaodb, "MySql.Data.MySqlClient"))
            {
                var CategoriaporId = await db.FetchAsync<Categorias>("SELECT * FROM restaurantecategorias WHERE Id_categoria = @0", IdCategorias);
                if (CategoriaporId == null)
                {
                    return NotFound($"Não foi encontrada nenhuma categoria com o Id: {IdCategorias}. Insira outro Id.");
                }
                var responseItems = mapper.Map<List<CategoriasDTO>>(CategoriaporId);
                return Ok(responseItems);
            }
        }

        [HttpPost("AdicionarCategorias")]
        public async Task<ActionResult> AdicionarCategoria([FromBody] List<CategoriasDTO> categoriasDTOs)
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<CategoriasDTO, Categorias>();
            });
            AutoMapper.IMapper mapper = config.CreateMapper();

            using (var db = new Database(conexaodb, "MySql.Data.MySqlClient"))
            {
                foreach (var categoriasDTO in categoriasDTOs)
                {
                    if (string.IsNullOrWhiteSpace(categoriasDTO.Categoria_Um))
                    {
                        var erro1 = new { Mensagem = "Preencha a categoria um" };
                        return BadRequest(erro1);
                    }

                    categoriasDTO.Id_categoria = ObterIdCategoria(categoriasDTO.RestauranteId);
                    var produtoExistente = await db.SingleOrDefaultAsync<Categorias>("SELECT * FROM restaurantecategorias WHERE Id_categoria = @0", categoriasDTO.Id_categoria);

                    if (produtoExistente == null)
                    {
                        categoriasDTO.Id_categoria = 0;
                        var novacategoria = mapper.Map<Categorias>(categoriasDTO);

                        await db.InsertAsync("restaurantecategorias", "Id_categoria", true, novacategoria);
                    }
                    else
                    {
                        var novacategoria = mapper.Map<Categorias>(categoriasDTO);

                        await db.UpdateAsync("restaurantecategorias", "Id_categoria", novacategoria);
                    }

                    
                }
            }
            return Ok();
        }

        [HttpGet("ObterIdCategoria")]
        public int ObterIdCategoria(int restauranteid)
        {
            using (var db = new Database(conexaodb, "MySql.Data.MySqlClient"))
            {
                var usuario = db.FirstOrDefault<Categorias>("SELECT Id_categoria FROM restaurantecategorias WHERE RestauranteId = @0", restauranteid);
                if (usuario == null)
                {
                    return 0;
                }
                else
                {
                    return usuario.Id_categoria;
                }

            }
        }

        private bool CategoriasExists(int id)
        {
            return (_context.Categorias?.Any(e => e.Id_categoria == id)).GetValueOrDefault();
        }
    }
}
