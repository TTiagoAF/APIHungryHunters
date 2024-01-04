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

namespace APIHungryHunters.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ContasController : ControllerBase
{
    private readonly TodoContext _contexto;
    private readonly IConfiguration _configuration;

    public ContasController(TodoContext contexto, IConfiguration configuration)
    {
        _contexto = contexto;
        _configuration = configuration;
    }

    // String de conexão com o banco de dados
    string conexaodb = "Server=localhost;Port=3306;Database=hungryhunters;Uid=root;";

    // GET: /api/TodasVendas/ListaDeVendas
    // Obtém todas as vendas da máquina de vendas a partir da base de dados
    [HttpGet("ListaDeContas")]
    public async Task<ActionResult<IEnumerable<ContasDTO>>> GetContas()
    {
        // Configuração do AutoMapper para mapear a classe TodasVendas para TodasVendasDTO
        var config = new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<Contas, ContasDTO>();
        });
        AutoMapper.IMapper mapper = config.CreateMapper();

        using (var db = new Database(conexaodb, "MySql.Data.MySqlClient"))
        {
            // Consulta todas as vendas na tabela vendas
            var contas = await db.FetchAsync<Contas>("SELECT * FROM contas");

            // Mapeia as vendas para a lista de DTOs
            var responseItems = mapper.Map<List<ContasDTO>>(contas);

            // Retorna a lista de vendas como resposta
            return Ok(responseItems);
        }
    }

    // GET: /api/TodasVendas/ListaDeVendasPor/{id}
    // Obtém uma venda específica da máquina de vendas a partir do ID fornecido
    [HttpGet("ContasPor/{id}")]
    public async Task<ActionResult<IEnumerable<ContasDTO>>> GetContas(long id)
    {
        // Configuração do AutoMapper para mapear a classe TodasVendas para TodasVendasDTO
        var config = new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<Contas, ContasDTO>();
        });
        AutoMapper.IMapper mapper = config.CreateMapper();

        using (var db = new Database(conexaodb, "MySql.Data.MySqlClient"))
        {
            // Consulta a venda pelo ID
            var conta = await db.FetchAsync<Contas>("SELECT * FROM contas WHERE Id_conta = @0", id);

            // Verifica se a venda foi encontrada
            if (conta == null)
            {
                return NotFound($"Não foi encontrada nenhuma Conta com o Id: {id}. Insira outro Id.");
            }

            // Mapeia a venda para o DTO
            var contasDTO = mapper.Map<List<ContasDTO>>(conta);

            // Retorna a venda como resposta
            return Ok(contasDTO);
        }
    }

    // POST: /api/TodasVendas/DeleteVendas
    // Exclui vendas da base de dados com base nos IDs fornecidos
    [HttpPost("DeleteContas")]
    public async Task<ActionResult> DeleteContas([FromBody] List<long> ids)
    {
        try
        {
            using (var db = new Database(conexaodb, "MySql.Data.MySqlClient"))
            {
                foreach (var id in ids)
                {
                    // Consulta a venda pelo ID
                    var conta = await db.SingleOrDefaultAsync<Contas>("SELECT * FROM contas WHERE Id_conta = @0", id);

                    // Verifica se a venda foi encontrada
                    if (conta == null)
                    {
                        return NotFound($"Não foi encontrada nenhuma conta com o Id: {id}. Insira outro Id.");
                    }
                    else
                    {
                        // Exclui a venda da tabela vendas
                        await db.DeleteAsync("contas", "Id_conta", conta);
                    }
                }
            }

            // Retorna uma resposta sem conteúdo
            return NoContent();
        }
        catch (Exception ex)
        {
            // Retorna uma resposta de erro interno do servidor
            return StatusCode(StatusCodes.Status500InternalServerError, "Erro ao excluir a conta");
        }
    }

    // POST: /api/TodasVendas/AddVendas
    // Insere vendas na base de dados
    [HttpPost("AddConta")]
    public async Task<ActionResult> AddConta([FromBody] List<ContasDTO> ContasDTO)
    {
        // Configuração do AutoMapper para mapear a classe TodasVendasDTO para TodasVendas
        var config = new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<ContasDTO, Contas>();
        });
        AutoMapper.IMapper mapper = config.CreateMapper();

        using (var db = new Database(conexaodb, "MySql.Data.MySqlClient"))
        {
            foreach (var contasDTO in ContasDTO)
            {
                var existingEmail = await db.FirstOrDefaultAsync<string>("SELECT Email FROM contas WHERE Email = @Email", new { Email = contasDTO.Email });
                if (!string.IsNullOrEmpty(existingEmail))
                {
                    var erro1 = new { Mensagem = "Este email já está a ser utilizado."};
                    return BadRequest(erro1);
                }

                var existingUsername = await db.FirstOrDefaultAsync<string>("SELECT Username FROM contas WHERE Username = @Username", new { Username = contasDTO.Username });
                if (!string.IsNullOrEmpty(existingUsername))
                {
                    var erro2 = new { Mensagem = "Este username já está a ser utilizado."};
                    return BadRequest(erro2);
                }

                if (string.IsNullOrWhiteSpace(contasDTO.Username) || contasDTO.Username.Length < 5 || contasDTO.Username.Length > 15 || contasDTO.Username.Contains(' '))
                {
                    var erro3 = new { Mensagem = "O username deve ter entre 5 e 15 caracteres e não deve conter espaços."};
                    return BadRequest(erro3);
                }

                var emailRegex = new System.Text.RegularExpressions.Regex(@"^[a-zA-Z0-9._-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,6}$");
                if (string.IsNullOrWhiteSpace(contasDTO.Email) || !emailRegex.IsMatch(contasDTO.Email))
                {
                    var erro4 = new { Mensagem = "Email inválido." };
                    return BadRequest(erro4);
                }

                var birthYear3 = DateTime.Now.Year - contasDTO.datadenascimento.Year;
                var birthYear1 = DateTime.Now.Month - contasDTO.datadenascimento.Month;
                var birthYear2 = DateTime.Now.Day - contasDTO.datadenascimento.Day;

                // Verifique se a data de nascimento está preenchida e se é uma data válida
                if (contasDTO.datadenascimento == DateTime.MinValue || contasDTO.datadenascimento.Year < DateTime.Now.Year - 125 || contasDTO.datadenascimento.Year > DateTime.Now.Year)
                { 
                    var erro5 = new { Mensagem = "Selecione uma data válida" };
                    return BadRequest(erro5);
                }

                if (birthYear3 < 18)
                {
                    var erro7 = new { Mensagem = "Tens de ter no minimo 18 anos" };
                    return BadRequest(erro7);
                }
                if (birthYear3 == 18)
                {
                    if (birthYear1 < 0)
                    {                        
                         var erro8 = new { Mensagem = "Tens de ter no minimo 18 anos" };
                         return BadRequest(erro8);                        
                    }
                }
                if (birthYear3 == 18)
                {
                    if (birthYear1 >= 0)
                    {
                        if (birthYear2 < 0)
                        {
                            var erro9 = new { Mensagem = "Tens de ter no minimo 18 anos" };
                            return BadRequest(erro9);
                        }
                    }
                }
                var passwordregex = new System.Text.RegularExpressions.Regex(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z])");

                // Verifique se a senha está preenchida e atende aos critérios
                if (string.IsNullOrWhiteSpace(contasDTO.Password) || contasDTO.Password.Length < 5 || contasDTO.Password.Length > 20 || contasDTO.Password.Contains(' ') || !passwordregex.IsMatch(contasDTO.Password))
                {
                    var erro6 = new { Mensagem = "A senha deve ter entre 5 e 20 caracteres, não deve conter espaços e deve conter letras maiúsculas, minúsculas, número e caractere especial." };
                    return BadRequest(erro6);
                }

                string hashedPassword = BCrypt.Net.BCrypt.HashPassword(contasDTO.Password);
                contasDTO.Password = hashedPassword;
                // Mapeia o DTO para a classe TodasVendas
                var novaconta = mapper.Map<Contas>(contasDTO);

                // Insere a venda na tabela vendas
                await db.InsertAsync("contas", "Id_conta", true, novaconta);
            }
        }

        // Retorna uma resposta de sucesso
        return Ok();
    }

    [HttpPost("Login")]
    public async Task<IActionResult> Login([FromBody] LoginDTO loginDTO)
    {
        using (var db = new Database(conexaodb, "MySql.Data.MySqlClient"))
        {
            if(string.IsNullOrWhiteSpace(loginDTO.Email) || string.IsNullOrWhiteSpace(loginDTO.Password))
            {
                var erro1 = new { Mensagem = "Campos obrigatórios" };
                return BadRequest(erro1);
            }
            var existingConta = await db.SingleOrDefaultAsync<Contas>("SELECT * FROM contas WHERE Email = @0", loginDTO.Email);

            if (existingConta == null)
            {
                var naoautorizado = new { Mensagem = "Credenciais inválidas" };
                return Unauthorized(naoautorizado);
            }

            bool isPasswordValid = BCrypt.Net.BCrypt.Verify(loginDTO.Password, existingConta.Password);

            if (!isPasswordValid)
            {
                var naoautorizado2 = new { Mensagem = "Credenciais inválidas" };
                return Unauthorized(naoautorizado2);
            }
        }

        var token = GenerateJwtToken(loginDTO.Email);
        return Ok(new { Token = token });
    }

    private string GenerateJwtToken(string email)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:SecretKey"]));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, email),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    [HttpPut("UpdateConta/{id}")]
    public async Task<ActionResult> UpdateConta(int id, [FromBody] ContasDTO contasDTO)
    {
        try
        {
            using (var db = new Database(conexaodb, "MySql.Data.MySqlClient"))
            {
                // Consulta a conta pelo ID
                var conta = await db.SingleOrDefaultAsync<Contas>("SELECT * FROM contas WHERE Id_conta = @0", id);

                if (conta == null)
                {
                    return NotFound($"Não foi encontrada nenhuma conta com o Id: {id}. Insira outro Id.");
                }

                // Atualize os campos necessários
                conta.Email = contasDTO.Email;
                conta.Username = contasDTO.Username;
                conta.Pontos = contasDTO.Pontos;
                conta.Password = contasDTO.Password;

                // Atualize a conta na tabela contas
                await db.UpdateAsync("contas", "Id_conta", conta);
            }

            return NoContent();
        }
        catch (Exception ex)
        {
            // Logue ou manipule a exceção conforme necessário
            return StatusCode(StatusCodes.Status500InternalServerError, "Erro ao atualizar a conta");
        }
    }

    // Método auxiliar para verificar se uma venda com o ID especificado existe
    private bool ContasExist(long id)
    {
        return _contexto.Contas.Any(e => e.Id_conta == id);
    }
}