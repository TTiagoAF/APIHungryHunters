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
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;

namespace APIHungryHunters.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmpresasController : ControllerBase
    {
        private readonly TodoContext _context;
        private readonly IConfiguration _configuration;

        public EmpresasController(TodoContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        string conexaodb = "Server=localhost;Port=3306;Database=hungryhunters;Uid=root;";

        [HttpGet("ListadeEmpresas")]
        public async Task<ActionResult<IEnumerable<TodasEmpresasDTO>>> GetEmpresas()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Empresas, TodasEmpresasDTO>();

                cfg.CreateMap<Restaurantes, RestaurantesDTO>();
                   
                cfg.CreateMap<RestauranteMenu, RestauranteMenuDTO>();
            });

            AutoMapper.IMapper mapper = config.CreateMapper();

            using (var db = new Database(conexaodb, "MySql.Data.MySqlClient"))
            {
                var empresas = await db.FetchAsync<Empresas>("SELECT * FROM empresa");

                foreach (var empresa in empresas)
                {
                    var restaurantes = await db.FetchAsync<Restaurantes>("SELECT * FROM restaurantes WHERE NipcEmpresa = @0", empresa.Nipc);

                    foreach (var restaurante in restaurantes)
                    {
                        var menus = await db.FetchAsync<RestauranteMenu>("SELECT * FROM restaurantepratos WHERE RestauranteId = @0", restaurante.Id_restaurante);
                        restaurante.RestauranteMenus = menus;
                    }

                    empresa.Restaurantes = restaurantes;
                }

                var responseItems = mapper.Map<List<TodasEmpresasDTO>>(empresas);

                return Ok(responseItems);
            }
        }

        [HttpGet("BuscarEmpresaspor{Nipc}")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<TodasEmpresasDTO>>> GetEmpresas(string Nipc)
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Empresas, TodasEmpresasDTO>();

                cfg.CreateMap<Restaurantes, RestaurantesDTO>();

                cfg.CreateMap<RestauranteMenu, RestauranteMenuDTO>();
            });
            AutoMapper.IMapper mapper = config.CreateMapper();

            using (var db = new Database(conexaodb, "MySql.Data.MySqlClient"))
            {
                var empresas = await db.FetchAsync<Empresas>("SELECT * FROM empresa WHERE Nipc = @0", Nipc);

                foreach (var empresa in empresas)
                {
                    var restaurantes = await db.FetchAsync<Restaurantes>("SELECT * FROM restaurantes WHERE NipcEmpresa = @0", Nipc);

                    foreach (var restaurante in restaurantes)
                    {
                        var menus = await db.FetchAsync<RestauranteMenu>("SELECT * FROM restaurantepratos WHERE RestauranteId = @0", restaurante.Id_restaurante);
                        restaurante.RestauranteMenus = menus;
                    }

                    empresa.Restaurantes = restaurantes;
                }

                var responseItems = mapper.Map<List<TodasEmpresasDTO>>(empresas);

                return Ok(responseItems);
            }

        }

        [HttpGet("Empresaspor{Razao_social}")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<EmpresasDTO>>> GetEmpresasNome(string Razao_social)
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Empresas, EmpresasDTO>();
            });
            AutoMapper.IMapper mapper = config.CreateMapper();

            using (var db = new Database(conexaodb, "MySql.Data.MySqlClient"))
            {
                var empresas = await db.FetchAsync<Empresas>("SELECT * FROM empresa WHERE Razao_social = @0", Razao_social);

                if (empresas == null)
                {
                    return NotFound($"Não foi encontrada nenhuma empresa com essa razão social: {Razao_social}. Insira outra razão social.");
                }

                var empresasDTO = mapper.Map<List<EmpresasDTO>>(empresas);

                return Ok(empresasDTO);
            }
        }

        [HttpPost("AdicionarEmpresa")]
        public async Task<ActionResult> AddEmpresa([FromBody] List<EmpresasDTO> empresasDTOs)
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<EmpresasDTO, Empresas>().ForMember(dest => dest.Nipc, opt => opt.MapFrom(src => src.Nipc));
            });
            AutoMapper.IMapper mapper = config.CreateMapper();

            using (var db = new Database(conexaodb, "MySql.Data.MySqlClient"))
            {
                foreach (var empresasDTO in empresasDTOs)
                {
                    if (empresasDTO.Nipc.IndexOf("0") == 0 || empresasDTO.Nipc.IndexOf("4") == 0 || empresasDTO.Nipc.IndexOf("7") == 0 || empresasDTO.Nipc.IndexOf("8") == 0 || empresasDTO.Nipc.Length != 9 || string.IsNullOrWhiteSpace(empresasDTO.Nipc))
                    {
                        var erro1 = new { Mensagem = "Nipc inválido" };
                        return BadRequest(erro1);
                    }

                    var existingNipc = await db.FirstOrDefaultAsync<string>("SELECT Nipc FROM empresa WHERE Nipc = @Nipc", new { Nipc = empresasDTO.Nipc });
                    if (!string.IsNullOrEmpty(existingNipc))
                    {
                        var erro1 = new { Mensagem = "Este Nipc já está a ser utilizado." };
                        return BadRequest(erro1);
                    }

                    if (string.IsNullOrWhiteSpace(empresasDTO.Razao_social))
                    {
                        var erro1 = new { Mensagem = "Preencha a razão social" };
                        return BadRequest(erro1);
                    }

                    var existingRazao = await db.FirstOrDefaultAsync<string>("SELECT Razao_social FROM empresa WHERE Razao_social = @Razao_social", new { Razao_social = empresasDTO.Razao_social });
                    if (!string.IsNullOrEmpty(existingRazao))
                    {
                        var erro1 = new { Mensagem = "Esta Razão Social já está a ser utilizado." };
                        return BadRequest(erro1);
                    }

                    var emailRegex = new System.Text.RegularExpressions.Regex(@"^[a-zA-Z0-9._-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,6}$");
                    if (string.IsNullOrWhiteSpace(empresasDTO.Email) || !emailRegex.IsMatch(empresasDTO.Email))
                    {
                        var erro4 = new { Mensagem = "Email inválido." };
                        return BadRequest(erro4);
                    }

                    var existingEmail = await db.FirstOrDefaultAsync<string>("SELECT Email FROM empresa WHERE Email = @Email", new { Email = empresasDTO.Email });
                    if (!string.IsNullOrEmpty(existingEmail))
                    {
                        var erro1 = new { Mensagem = "Este email já está a ser utilizado." };
                        return BadRequest(erro1);
                    }

                    if (empresasDTO.Num_Restaurante <= 0)
                    {
                        var erro1 = new { Mensagem = "Adicione o número de restaurantes" };
                        return BadRequest(erro1);
                    }

                    if (empresasDTO.Telemovel.Length != 9 || empresasDTO.Telemovel == "")
                    {
                        var erro1 = new { Mensagem = "Telemóvel inválido" };
                        return BadRequest(erro1);
                    }

                    var passwordregex = new System.Text.RegularExpressions.Regex(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z])");
                    if (string.IsNullOrWhiteSpace(empresasDTO.Password) || empresasDTO.Password.Length < 5 || empresasDTO.Password.Length > 20 || empresasDTO.Password.Contains(' ') || !passwordregex.IsMatch(empresasDTO.Password))
                    {
                        var erro6 = new { Mensagem = "A senha deve ter entre 5 e 20 caracteres, não deve conter espaços e deve conter letras maiúsculas, minúsculas, número e caractere especial." };
                        return BadRequest(erro6);
                    }


                    var novaEmpresa = mapper.Map<Empresas>(empresasDTO);
                    novaEmpresa.Password = BCrypt.Net.BCrypt.HashPassword(empresasDTO.Password);
                    await db.InsertAsync("empresa", "Nipc", false, novaEmpresa);
                }
            }
            return Ok();
        }

        [HttpPost("LoginEmpresas")]
        public async Task<IActionResult> LoginEmpresas([FromBody] LoginEmpresasDTO loginEmpresasDTO)
        {
            using (var db = new Database(conexaodb, "MySql.Data.MySqlClient"))
            {
                if (string.IsNullOrWhiteSpace(loginEmpresasDTO.Nipc) || string.IsNullOrWhiteSpace(loginEmpresasDTO.Password))
                {
                    var erro1 = new { Mensagem = "Campos obrigatórios" };
                    return BadRequest(erro1);
                }
                var existingConta = await db.SingleOrDefaultAsync<Empresas>("SELECT Nipc, Password FROM empresa WHERE Nipc = @0", loginEmpresasDTO.Nipc);

                if (existingConta == null)
                {
                    var naoautorizado = new { Mensagem = "Credenciais inválidas" };
                    return Unauthorized(naoautorizado);
                }

                bool isPasswordValid = BCrypt.Net.BCrypt.Verify(loginEmpresasDTO.Password, existingConta.Password);

                if (!isPasswordValid)
                {
                    var naoautorizado2 = new { Mensagem = "Credenciais inválidas" };
                    return Unauthorized(naoautorizado2);
                }
            }
            var razaosocial = ObterRazaoSocial(loginEmpresasDTO.Nipc);
            var token = GenerateJwtToken(loginEmpresasDTO.Nipc);
            return Ok(new { Token = token, Razao = razaosocial});
        }

        [HttpGet("ObterRazaoSocial")]
        public string ObterRazaoSocial(string nipc)
        {
            using (var db = new Database(conexaodb, "MySql.Data.MySqlClient"))
            {
                var usuario = db.FirstOrDefault<Empresas>("SELECT Razao_social FROM empresa WHERE Nipc = @0", nipc);

                return usuario.Razao_social;
            }
        }

        private string GenerateJwtToken(string nipc)
        {

            var issuer = _configuration["Jwt:Issuer"];
            var audience = _configuration["Jwt:Audience"];
            var key = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]);
            var signingCredentials = new SigningCredentials(
                                    new SymmetricSecurityKey(key),
                                    SecurityAlgorithms.HmacSha512Signature
                                );

            var subject = new ClaimsIdentity(new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, nipc),
                new Claim(JwtRegisteredClaimNames.Email, nipc),
            });
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = subject,
                Issuer = issuer,
                Audience = audience,
                SigningCredentials = signingCredentials
            };
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var jwtToken = tokenHandler.WriteToken(token);
            return jwtToken;
        }
        
        [HttpPost("MaisRestaurante/{Razao_social}")]
        [Authorize]
        public async Task<ActionResult> MaisRestaurante(string Razao_social)
        {
            try
            {
                using (var db = new Database(conexaodb, "MySql.Data.MySqlClient"))
                {
                    var num = await db.SingleOrDefaultAsync<Empresas>("SELECT * FROM empresa WHERE Razao_social = @0", Razao_social);

                    if (num == null)
                    {
                        return NotFound($"Não foi encontrado nenhuma empresa com a razão social: {Razao_social}. Insira outra razão social.");
                    }

                    num.Num_Restaurante += 1;

                    await db.UpdateAsync("empresa", "Razao_social", num);

                    return NoContent();
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Erro ao atualizar");
            }
        }

        [HttpPost("MenosRestaurante/{Razao_social}")]
        [Authorize]
        public async Task<ActionResult> MenosRestaurante(string Razao_social)
        {
            try
            {
                using (var db = new Database(conexaodb, "MySql.Data.MySqlClient"))
                {
                    var num = await db.SingleOrDefaultAsync<Empresas>("SELECT * FROM empresa WHERE Razao_social = @0", Razao_social);

                    if (num == null)
                    {
                        return NotFound($"Não foi encontrado nenhuma empresa com a razão social: {Razao_social}. Insira outra razão social.");
                    }

                    num.Num_Restaurante -= 1;

                    await db.UpdateAsync("empresa", "Razao_social", num);

                    return NoContent();
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Erro ao atualizar");
            }
        }

        [HttpPost("MenosRestauranteporNipc/{Nipc}")]
        [Authorize]
        public async Task<ActionResult> MenosRestauranteNipc(string Nipc)
        {
            try
            {
                using (var db = new Database(conexaodb, "MySql.Data.MySqlClient"))
                {
                    var num = await db.SingleOrDefaultAsync<Empresas>("SELECT * FROM empresa WHERE Nipc = @0", Nipc);

                    if (num == null)
                    {
                        return NotFound($"Não foi encontrado nenhuma empresa com o Nipc: {Nipc}. Insira outro Nipc.");
                    }

                    num.Num_Restaurante -= 1;

                    await db.UpdateAsync("empresa", "Nipc", num);

                    return NoContent();
                }
            }
            catch (Exception ex)
            {
                // Retorna uma resposta de erro interno do servidor
                return StatusCode(StatusCodes.Status500InternalServerError, "Erro ao atualizar");
            }
        }

        private bool EmpresasExists(string nipc)
        {
            return (_context.Empresas?.Any(e => e.Nipc == nipc)).GetValueOrDefault();
        }
    }
}
