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
    public class AvaliacoesController : ControllerBase
    {
        private readonly TodoContext _context;

        public AvaliacoesController(TodoContext context)
        {
            _context = context;
        }

        string conexaodb = "Server=localhost;Port=3306;Database=hungryhunters;Uid=root;";

        [HttpGet("ListadeAvaliacoespor{ContaId}")]
        public async Task<ActionResult<IEnumerable<TodasAvaliacoes>>> ObterAvaliacoesporContaId(int ContaId)
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Avaliacoes, TodasAvaliacoes>();
            });
            AutoMapper.IMapper mapper = config.CreateMapper();

            using (var db = new Database(conexaodb, "MySql.Data.MySqlClient"))
            {
                var AvaliacoesporContaId = await db.FetchAsync<Avaliacoes>("SELECT * FROM avaliacoes WHERE ContaId = @0", ContaId);

                if (AvaliacoesporContaId == null || AvaliacoesporContaId.Count == 0)
                {
                    return NotFound($"Não foi encontrada nenhuma avaliação com o Id: {ContaId}. Insira outro Id.");
                }

                var responseItems = new List<TodasAvaliacoes>();

                foreach (var avaliacoes in AvaliacoesporContaId)
                {
                    var IdContas = ObterNomeDoRestaurante(avaliacoes.RestauranteId);
                    var Cliente = ObterNomeDoCliente(avaliacoes.ContaId);

                    avaliacoes.NomeRestaurante = IdContas;
                    avaliacoes.NomeCliente = Cliente;

                    var responseItem = mapper.Map<TodasAvaliacoes>(avaliacoes);
                    responseItems.Add(responseItem);
                }

                return Ok(responseItems);
            }
        }

        [HttpGet("ListadeAvaliacoesLimitadacom")]
        public async Task<ActionResult<IEnumerable<TodasAvaliacoes>>> ObterAvaliacoesLimitadacomRestauranteId(int RestauranteId)
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Avaliacoes, TodasAvaliacoes>();
            });
            AutoMapper.IMapper mapper = config.CreateMapper();

            using (var db = new Database(conexaodb, "MySql.Data.MySqlClient"))
            {
                var AvaliacoesRestauranteId = await db.FetchAsync<Avaliacoes>("SELECT * FROM avaliacoes WHERE RestauranteId = @0 ORDER BY Id_avaliacao ASC LIMIT 3", RestauranteId);

                if (AvaliacoesRestauranteId == null || AvaliacoesRestauranteId.Count == 0)
                {
                    return NotFound($"Não foi encontrada nenhuma avaliação com o Id: {RestauranteId}. Insira outro Id.");
                }

                var responseItems = new List<TodasAvaliacoes>();

                foreach (var avaliacoes in AvaliacoesRestauranteId)
                {
                    var IdContas = ObterNomeDoRestaurante(avaliacoes.RestauranteId);
                    var Cliente = ObterNomeDoCliente(avaliacoes.ContaId);

                    avaliacoes.NomeRestaurante = IdContas;
                    avaliacoes.NomeCliente = Cliente;

                    var responseItem = mapper.Map<TodasAvaliacoes>(avaliacoes);
                    responseItems.Add(responseItem);
                }

                return Ok(responseItems);
            }
        }

        [HttpGet("ListadeAvaliacoescom{RestauranteId}")]
        public async Task<ActionResult<IEnumerable<TodasAvaliacoes>>> ObterAvaliacoescomRestauranteId(int RestauranteId)
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Avaliacoes, TodasAvaliacoes>();
            });
            AutoMapper.IMapper mapper = config.CreateMapper();

            using (var db = new Database(conexaodb, "MySql.Data.MySqlClient"))
            {
                var AvaliacoesRestauranteId = await db.FetchAsync<Avaliacoes>("SELECT * FROM avaliacoes WHERE RestauranteId = @0 ORDER BY Id_avaliacao ASC", RestauranteId);

                if (AvaliacoesRestauranteId == null || AvaliacoesRestauranteId.Count == 0)
                {
                    return NotFound($"Não foi encontrada nenhuma avaliação com o Id: {RestauranteId}. Insira outro Id.");
                }

                var responseItems = new List<TodasAvaliacoes>();

                foreach (var avaliacoes in AvaliacoesRestauranteId)
                {
                    var IdContas = ObterNomeDoRestaurante(avaliacoes.RestauranteId);
                    var Cliente = ObterNomeDoCliente(avaliacoes.ContaId);

                    avaliacoes.NomeRestaurante = IdContas;
                    avaliacoes.NomeCliente = Cliente;

                    var responseItem = mapper.Map<TodasAvaliacoes>(avaliacoes);
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

        [HttpPost("AdicionarAvaliacao")]
        public async Task<ActionResult> AdicionarAvaliacao([FromBody] List<AvaliacoesDTO> avaliacoesDTOs)
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<AvaliacoesDTO, Avaliacoes>();
            });
            AutoMapper.IMapper mapper = config.CreateMapper();

            using (var db = new Database(conexaodb, "MySql.Data.MySqlClient"))
            {
                foreach (var avaliacoesDTO in avaliacoesDTOs)
                {
                    if (string.IsNullOrWhiteSpace(avaliacoesDTO.Comentario))
                    {
                        var erro1 = new { Mensagem = "Preencha o comentário" };
                        return BadRequest(erro1);
                    }
                    avaliacoesDTO.Id_avaliacao = ObterIdAvaliacoes(avaliacoesDTO.ContaId);

                    var TemAvaliacao = await db.FirstOrDefaultAsync<Avaliacoes>(
                        "SELECT * FROM avaliacoes WHERE ContaId = @ContaId " +
                        "AND RestauranteId = @RestauranteId",
                            new
                            {
                                avaliacoesDTO.ContaId,
                                RestauranteId = avaliacoesDTO.RestauranteId,
                            });

                    if (TemAvaliacao != null)
                    {
                        var novaAvaliacao2 = mapper.Map<Avaliacoes>(avaliacoesDTO);
                        await db.UpdateAsync("avaliacoes", "Id_avaliacao", novaAvaliacao2);
                    }
                    else if(TemAvaliacao == null)
                    {
                        avaliacoesDTO.Id_avaliacao = 0;
                        var novaAvaliacao = mapper.Map<Avaliacoes>(avaliacoesDTO);
                        await db.InsertAsync("avaliacoes", "Id_avaliacao", true, novaAvaliacao);
                    }
                    
                }
            }
            return Ok();
        }

        [HttpGet("ObterIdAvaliacoes")]
        public int ObterIdAvaliacoes(int ContaId)
        {
            using (var db = new Database(conexaodb, "MySql.Data.MySqlClient"))
            {
                var usuario = db.FirstOrDefault<Avaliacoes>("SELECT Id_avaliacao FROM avaliacoes WHERE ContaId = @0", ContaId);
                if (usuario == null)
                {
                    return 0;
                }
                else
                {
                    return usuario.Id_avaliacao;
                }

            }
        }
    }
}