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

    [Route("api/[controller]")]
    [ApiController]
    public class ReservasController : ControllerBase
    {
        private readonly TodoContext _context;

        public ReservasController(TodoContext context)
        {
            _context = context;
        }

        string conexaodb = "Server=localhost;Port=3306;Database=hungryhunters;Uid=root;";

        [HttpGet("ListadeReservaspor{ContaId}")]
        public async Task<ActionResult<IEnumerable<TodasReservasDTO>>> ObterReservasporContaId(int ContaId)
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Reservas, TodasReservasDTO>();
            });
            AutoMapper.IMapper mapper = config.CreateMapper();

            using (var db = new Database(conexaodb, "MySql.Data.MySqlClient"))
            {
                var ReservasporContaId = await db.FetchAsync<Reservas>("SELECT * FROM reservas WHERE ContaId = @0 ORDER BY Data_reserva", ContaId);

                if (ReservasporContaId == null || ReservasporContaId.Count == 0)
                {
                    return NotFound($"Não foi encontrada nenhuma reserva com o Id: {ContaId}. Insira outro Id.");
                }

                var responseItems = new List<TodasReservasDTO>();

                foreach (var reserva in ReservasporContaId)
                {
                    var IdContas = ObterNomeDoRestaurante(reserva.RestauranteId);
                    var Mesa = ObterNomeDaMesa(reserva.MesaId);
                    var Cliente = ObterNomeDoCliente(reserva.ContaId);

                    reserva.NomeRestaurante = IdContas;
                    reserva.NomeMesa = Mesa;
                    reserva.NomeCliente = Cliente;

                    var responseItem = mapper.Map<TodasReservasDTO>(reserva);
                    responseItems.Add(responseItem);
                }

                return Ok(responseItems);
            }
        }

        [HttpGet("ListadeReservascom{RestauranteId}")]
        public async Task<ActionResult<IEnumerable<TodasReservasDTO>>> ObterReservascomRestauranteId(int RestauranteId)
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Reservas, TodasReservasDTO>();
            });
            AutoMapper.IMapper mapper = config.CreateMapper();

            using (var db = new Database(conexaodb, "MySql.Data.MySqlClient"))
            {
                var ReservasporContaId = await db.FetchAsync<Reservas>("SELECT * FROM reservas WHERE RestauranteId = @0 ORDER BY Data_reserva", RestauranteId);

                if (ReservasporContaId == null || ReservasporContaId.Count == 0)
                {
                    return NotFound($"Não foi encontrada nenhuma reserva com o Id: {RestauranteId}. Insira outro Id.");
                }

                var responseItems = new List<TodasReservasDTO>();

                foreach (var reserva in ReservasporContaId)
                {
                    var IdContas = ObterNomeDoRestaurante(reserva.RestauranteId);
                    var Mesa = ObterNomeDaMesa(reserva.MesaId);
                    var Cliente = ObterNomeDoCliente(reserva.ContaId);

                    reserva.NomeRestaurante = IdContas;
                    reserva.NomeMesa = Mesa;
                    reserva.NomeCliente = Cliente;

                    var responseItem = mapper.Map<TodasReservasDTO>(reserva);
                    responseItems.Add(responseItem);
                }

                return Ok(responseItems);
            }
        }

        [HttpGet("ObterNomeDaMesa")]
        public string ObterNomeDaMesa(int idmesa)
        {
            using (var db = new Database(conexaodb, "MySql.Data.MySqlClient"))
            {
                var usuario = db.FirstOrDefault<Mesas>("SELECT Nome FROM mesas WHERE Id_mesa = @0", idmesa);

                return usuario.Nome;
            }
        }

        [HttpGet("ObterNomeDoCliente")]
        public string ObterNomeDoCliente(int idconta)
        {
            using (var db = new Database(conexaodb, "MySql.Data.MySqlClient"))
            {
                var nome = "Reserva feita pelo restaurante";
                if(idconta == 0)
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

        [HttpPost("AdicionarReserva")]
        public async Task<ActionResult> AdicionarReserva([FromBody] List<ReservasDTO> reservasDTOs)
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<ReservasDTO, Reservas>();
            });
            AutoMapper.IMapper mapper = config.CreateMapper();

            using (var db = new Database(conexaodb, "MySql.Data.MySqlClient"))
            {
                foreach (var reservaDTO in reservasDTOs)
                {
                    if (string.IsNullOrWhiteSpace(reservaDTO.Horario))
                    {
                        var erro1 = new { Mensagem = "Selecione um horário" };
                        return BadRequest(erro1);
                    }
                    if (reservaDTO.Data_reserva.Year != DateTime.Now.Year)
                    {
                        var erro5 = new { Mensagem = "Faça a reserva para este ano" };
                        return BadRequest(erro5);
                    }

                    if (reservaDTO.Data_reserva.Year == DateTime.Now.Year)
                    {
                        if (reservaDTO.Data_reserva.Month < DateTime.Now.Month)
                        {
                            var erro5 = new { Mensagem = "Esse mês já passou" };
                            return BadRequest(erro5);
                        }
                    }
                    if (reservaDTO.Data_reserva.Year == DateTime.Now.Year)
                    {
                        if (reservaDTO.Data_reserva.Month <= DateTime.Now.Month)
                        {
                            if (reservaDTO.Data_reserva.Day < DateTime.Now.Day)
                            {
                                var erro5 = new { Mensagem = "Esse dia já passou" };
                                return BadRequest(erro5);
                            }
                        }
                    }

                    if (reservaDTO.Data_reserva.Year <= DateTime.Now.Year)
                    {
                        if (reservaDTO.Data_reserva.Month <= DateTime.Now.Month)
                        {
                            if (reservaDTO.Data_reserva.Day <= DateTime.Now.Day)
                            {
                                int resultadoComparacao = string.Compare(DateTime.Now.ToString("HH:mm"), reservaDTO.Horario);

                                if (resultadoComparacao >= 0)
                                {
                                    var erro5 = new { Mensagem = "Esse horário já passou" };
                                    return BadRequest(erro5);
                                }
                            }
                        }
                    }

                    var validData = await db.FirstOrDefaultAsync<Reservas>(
                    "SELECT * FROM ferias WHERE RestauranteId = @RestauranteId " +
                    "AND YEAR(InicioFerias) = @Ano " +
                    "AND MONTH(InicioFerias) = @Mes " +
                    "AND DAY(InicioFerias) <= @Dia",
                     new
                     {
                         reservaDTO.RestauranteId,
                         Ano = reservaDTO.Data_reserva.Year,
                         Mes = reservaDTO.Data_reserva.Month,
                         Dia = reservaDTO.Data_reserva.Day,
                     });

                    var validData2 = await db.FirstOrDefaultAsync<Reservas>(
                       "SELECT * FROM ferias WHERE RestauranteId = @RestauranteId " +
                        "AND YEAR(FimFerias) = @Ano " +
                        "AND MONTH(FimFerias) = @Mes " +
                        "AND DAY(FimFerias) <= @Dia",
                         new
                         {
                             reservaDTO.RestauranteId,
                             Ano = reservaDTO.Data_reserva.Year,
                             Mes = reservaDTO.Data_reserva.Month,
                             Dia = reservaDTO.Data_reserva.Day,
                         });

                    var validDia = await db.FirstOrDefaultAsync<Reservas>(
                        "SELECT * FROM diasfestivos WHERE RestauranteId = @RestauranteId " +
                        "AND YEAR(DiaFestivo) = @Ano " +
                        "AND MONTH(DiaFestivo) = @Mes " +
                        "AND DAY(DiaFestivo) = @Dia",
                            new
                            {
                                reservaDTO.RestauranteId,
                                Ano = reservaDTO.Data_reserva.Year,
                                Mes = reservaDTO.Data_reserva.Month,
                                Dia = reservaDTO.Data_reserva.Day,
                            });

                    if (validData != null || validData2 != null)
                    {
                        var erro5 = new { Mensagem = "O restaurante está de férias nesta data" };
                        return BadRequest(erro5);

                    }

                    var diasemana = reservaDTO.Data_reserva.DayOfWeek;

                    var validSegunda = await db.FirstOrDefaultAsync<DiasDeFuncionamento>(
                    "SELECT * FROM diasfuncionamento WHERE RestauranteId = @RestauranteId",
                    new
                    {
                        reservaDTO.RestauranteId,
                    });

                    if (diasemana == DayOfWeek.Monday)
                    {
                        if (validSegunda.Segunda == "true")
                        {
                            if (validDia == null)
                            {
                                var erro5 = new { Mensagem = "O restaurante está de folga nesta data" };
                                return BadRequest(erro5);
                            }
                        }
                    }
                    else if (diasemana == DayOfWeek.Tuesday)
                    {
                        if (validSegunda.Terca == "true")
                        {
                            if (validDia == null)
                            {
                                var erro5 = new { Mensagem = "O restaurante está de folga nesta data" };
                                return BadRequest(erro5);
                            }
                        }
                    }
                    else if (diasemana == DayOfWeek.Wednesday)
                    {
                        if (validSegunda.Quarta == "true")
                        {
                            if (validDia == null)
                            {
                                var erro5 = new { Mensagem = "O restaurante está de folga nesta data" };
                                return BadRequest(erro5);
                            }
                        }
                    }
                    else if (diasemana == DayOfWeek.Thursday)
                    {
                        if (validSegunda.Quinta == "true")
                        {
                            if (validDia == null)
                            {
                                var erro5 = new { Mensagem = "O restaurante está de folga nesta data" };
                                return BadRequest(erro5);
                            }
                        }
                    }
                    else if (diasemana == DayOfWeek.Friday)
                    {
                        if (validSegunda.Sexta == "true")
                        {
                            if (validDia == null)
                            {
                                var erro5 = new { Mensagem = "O restaurante está de folga nesta data" };
                                return BadRequest(erro5);
                            }
                        }
                    }
                    else if (diasemana == DayOfWeek.Saturday)
                    {
                        if (validSegunda.Sabado == "true")
                        {
                            if (validDia == null)
                            {
                                var erro5 = new { Mensagem = "O restaurante está de folga nesta data" };
                                return BadRequest(erro5);
                            }
                        }
                    }
                    else if (diasemana == DayOfWeek.Sunday)
                    {
                        if (validSegunda.Domingo == "true")
                        {
                            if (validDia == null)
                            {
                                var erro5 = new { Mensagem = "O restaurante está de folga nesta data" };
                                return BadRequest(erro5);
                            }
                        }
                    }

                    var validReserva = await db.FirstOrDefaultAsync<Reservas>(
                        "SELECT * FROM reservas WHERE ContaId = @ContaId " +
                        "AND YEAR(Data_reserva) = @Ano " +
                        "AND MONTH(Data_reserva) = @Mes " +
                        "AND DAY(Data_reserva) = @Dia " +
                        "AND Horario = @Horario " +
                        "AND Estado != @Estado",
                            new
                            {
                                reservaDTO.ContaId,
                                Ano = reservaDTO.Data_reserva.Year,
                                Mes = reservaDTO.Data_reserva.Month,
                                Dia = reservaDTO.Data_reserva.Day,
                                Horario = reservaDTO.Horario,
                                Estado = "Cancelado",
                            });

                    if (validReserva != null)
                    {
                        var erro5 = new { Mensagem = "Já tens uma reserva feita para essa altura" };
                        return BadRequest(erro5);
                    }

                    var validMesa = await db.FirstOrDefaultAsync<Reservas>(
                        "SELECT * FROM reservas WHERE RestauranteId = @RestauranteId " +
                        "AND YEAR(Data_reserva) = @Ano " +
                        "AND MONTH(Data_reserva) = @Mes " +
                        "AND DAY(Data_reserva) = @Dia " +
                        "AND Horario = @Horario " +
                        "AND MesaId = @Mesa " +
                        "AND Estado != @Estado",
                        new
                        {
                            reservaDTO.RestauranteId,
                            Ano = reservaDTO.Data_reserva.Year,
                            Mes = reservaDTO.Data_reserva.Month,
                            Dia = reservaDTO.Data_reserva.Day,
                            Horario = reservaDTO.Horario,
                            Mesa = reservaDTO.MesaId,
                            Estado = "Cancelado",
                        });

                    if (validMesa != null)
                    {
                        var erro5 = new { Mensagem = "Essa mesa já está ocupada" };
                        return BadRequest(erro5);
                    }

                    var validgrupo = await db.FirstOrDefaultAsync<Mesas>(
                        "SELECT * FROM mesas WHERE RestauranteId = @RestauranteId " +
                        "AND Maximo_pessoas < @Maximo",
                        new
                        {
                            reservaDTO.RestauranteId,
                            Maximo = reservaDTO.Quantidade_pessoa,
                        });

                    if (validgrupo != null)
                    {
                        var erro5 = new { Mensagem = "Quantidade de pessoas inválidas para essa mesa" };
                        return BadRequest(erro5);
                    }

                    var novaReserva = mapper.Map<Reservas>(reservaDTO);
                    await db.InsertAsync("reservas", "Id_reserva", true, novaReserva);
                }
            }
            return Ok();
        }

        [HttpPost("AdicionarReservaDosRestaurantes")]
        public async Task<ActionResult> AdicionarReservaDosRestaurantes([FromBody] List<ReservasDTO> reservasDTOs)
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<ReservasDTO, Reservas>();
            });
            AutoMapper.IMapper mapper = config.CreateMapper();

            using (var db = new Database(conexaodb, "MySql.Data.MySqlClient"))
            {
                foreach (var reservaDTO in reservasDTOs)
                {
                    if (string.IsNullOrWhiteSpace(reservaDTO.Horario))
                    {
                        var erro1 = new { Mensagem = "Selecione um horário" };
                        return BadRequest(erro1);
                    }
                    if (reservaDTO.Data_reserva.Year != DateTime.Now.Year)
                    {
                        var erro5 = new { Mensagem = "Faça a reserva para este ano" };
                        return BadRequest(erro5);
                    }

                    if (reservaDTO.Data_reserva.Year == DateTime.Now.Year)
                    {
                        if (reservaDTO.Data_reserva.Month < DateTime.Now.Month)
                        {
                            var erro5 = new { Mensagem = "Esse mês já passou" };
                            return BadRequest(erro5);
                        }
                    }
                    if (reservaDTO.Data_reserva.Year == DateTime.Now.Year)
                    {
                        if (reservaDTO.Data_reserva.Month <= DateTime.Now.Month)
                        {
                            if (reservaDTO.Data_reserva.Day < DateTime.Now.Day)
                            {
                                var erro5 = new { Mensagem = "Esse dia já passou" };
                                return BadRequest(erro5);
                            }
                        }
                    }

                    if (reservaDTO.Data_reserva.Year <= DateTime.Now.Year)
                    {
                        if (reservaDTO.Data_reserva.Month <= DateTime.Now.Month)
                        {
                            if (reservaDTO.Data_reserva.Day <= DateTime.Now.Day)
                            {
                                int resultadoComparacao = string.Compare(DateTime.Now.ToString("HH:mm"), reservaDTO.Horario);

                                if (resultadoComparacao >= 0)
                                {
                                    var erro5 = new { Mensagem = "Esse horário já passou" };
                                    return BadRequest(erro5);
                                }
                            }
                        }
                    }

                    var validData = await db.FirstOrDefaultAsync<Reservas>(
                    "SELECT * FROM ferias WHERE RestauranteId = @RestauranteId " +
                    "AND YEAR(InicioFerias) = @Ano " +
                    "AND MONTH(InicioFerias) = @Mes " +
                    "AND DAY(InicioFerias) <= @Dia",
                     new
                     {
                         reservaDTO.RestauranteId,
                         Ano = reservaDTO.Data_reserva.Year,
                         Mes = reservaDTO.Data_reserva.Month,
                         Dia = reservaDTO.Data_reserva.Day,
                     });

                    var validData2 = await db.FirstOrDefaultAsync<Reservas>(
                       "SELECT * FROM ferias WHERE RestauranteId = @RestauranteId " +
                        "AND YEAR(FimFerias) = @Ano " +
                        "AND MONTH(FimFerias) = @Mes " +
                        "AND DAY(FimFerias) <= @Dia",
                         new
                         {
                             reservaDTO.RestauranteId,
                             Ano = reservaDTO.Data_reserva.Year,
                             Mes = reservaDTO.Data_reserva.Month,
                             Dia = reservaDTO.Data_reserva.Day,
                         });

                    var validDia = await db.FirstOrDefaultAsync<Reservas>(
                        "SELECT * FROM diasfestivos WHERE RestauranteId = @RestauranteId " +
                        "AND YEAR(DiaFestivo) = @Ano " +
                        "AND MONTH(DiaFestivo) = @Mes " +
                        "AND DAY(DiaFestivo) = @Dia",
                            new
                            {
                                reservaDTO.RestauranteId,
                                Ano = reservaDTO.Data_reserva.Year,
                                Mes = reservaDTO.Data_reserva.Month,
                                Dia = reservaDTO.Data_reserva.Day,
                            });

                    if (validData != null || validData2 != null)
                    {
                        var erro5 = new { Mensagem = "O restaurante está de férias nesta data" };
                        return BadRequest(erro5);

                    }

                    var diasemana = reservaDTO.Data_reserva.DayOfWeek;

                    var validSegunda = await db.FirstOrDefaultAsync<DiasDeFuncionamento>(
                    "SELECT * FROM diasfuncionamento WHERE RestauranteId = @RestauranteId",
                    new
                    {
                        reservaDTO.RestauranteId,
                    });

                    if (diasemana == DayOfWeek.Monday)
                    {
                        if (validSegunda.Segunda == "true")
                        {
                            if (validDia == null)
                            {
                                var erro5 = new { Mensagem = "O restaurante está de folga nesta data" };
                                return BadRequest(erro5);
                            }
                        }
                    }
                    else if (diasemana == DayOfWeek.Tuesday)
                    {
                        if (validSegunda.Terca == "true")
                        {
                            if (validDia == null)
                            {
                                var erro5 = new { Mensagem = "O restaurante está de folga nesta data" };
                                return BadRequest(erro5);
                            }
                        }
                    }
                    else if (diasemana == DayOfWeek.Wednesday)
                    {
                        if (validSegunda.Quarta == "true")
                        {
                            if (validDia == null)
                            {
                                var erro5 = new { Mensagem = "O restaurante está de folga nesta data" };
                                return BadRequest(erro5);
                            }
                        }
                    }
                    else if (diasemana == DayOfWeek.Thursday)
                    {
                        if (validSegunda.Quinta == "true")
                        {
                            if (validDia == null)
                            {
                                var erro5 = new { Mensagem = "O restaurante está de folga nesta data" };
                                return BadRequest(erro5);
                            }
                        }
                    }
                    else if (diasemana == DayOfWeek.Friday)
                    {
                        if (validSegunda.Sexta == "true")
                        {
                            if (validDia == null)
                            {
                                var erro5 = new { Mensagem = "O restaurante está de folga nesta data" };
                                return BadRequest(erro5);
                            }
                        }
                    }
                    else if (diasemana == DayOfWeek.Saturday)
                    {
                        if (validSegunda.Sabado == "true")
                        {
                            if (validDia == null)
                            {
                                var erro5 = new { Mensagem = "O restaurante está de folga nesta data" };
                                return BadRequest(erro5);
                            }
                        }
                    }
                    else if (diasemana == DayOfWeek.Sunday)
                    {
                        if (validSegunda.Domingo == "true")
                        {
                            if (validDia == null)
                            {
                                var erro5 = new { Mensagem = "O restaurante está de folga nesta data" };
                                return BadRequest(erro5);
                            }
                        }
                    }

                    var validMesa = await db.FirstOrDefaultAsync<Reservas>(
                        "SELECT * FROM reservas WHERE RestauranteId = @RestauranteId " +
                        "AND YEAR(Data_reserva) = @Ano " +
                        "AND MONTH(Data_reserva) = @Mes " +
                        "AND DAY(Data_reserva) = @Dia " +
                        "AND Horario = @Horario " +
                        "AND MesaId = @Mesa " +
                        "AND Estado != @Estado",
                        new
                        {
                            reservaDTO.RestauranteId,
                            Ano = reservaDTO.Data_reserva.Year,
                            Mes = reservaDTO.Data_reserva.Month,
                            Dia = reservaDTO.Data_reserva.Day,
                            Horario = reservaDTO.Horario,
                            Mesa = reservaDTO.MesaId,
                            Estado = "Cancelado",
                        });

                    if (validMesa != null)
                    {
                        var erro5 = new { Mensagem = "Essa mesa já está ocupada" };
                        return BadRequest(erro5);
                    }

                    var validgrupo = await db.FirstOrDefaultAsync<Mesas>(
                        "SELECT * FROM mesas WHERE RestauranteId = @RestauranteId " +
                        "AND Maximo_pessoas < @Maximo",
                        new
                        {
                            reservaDTO.RestauranteId,
                            Maximo = reservaDTO.Quantidade_pessoa,
                        });

                    if (validgrupo != null)
                    {
                        var erro5 = new { Mensagem = "Quantidade de pessoas inválidas para essa mesa" };
                        return BadRequest(erro5);
                    }

                    var novaReserva = mapper.Map<Reservas>(reservaDTO);

                    await db.InsertAsync("reservas", "Id_reserva", true, novaReserva);
                }
            }
            return Ok();
        }

        [HttpPost("EliminarReservas/{ReservaId}")]
        public async Task<ActionResult> DeleteReservas(int ReservaId)
        {
            try
            {
                using (var db = new Database(conexaodb, "MySql.Data.MySqlClient"))
                {
                    var todasreservas = await db.SingleOrDefaultAsync<Reservas>("SELECT * FROM reservas WHERE Id_reserva = @0", ReservaId);

                    if (todasreservas == null)
                    {
                        return NotFound($"Não foi encontrado nenhuma reserva com o id: {ReservaId}.");
                    }
                    else
                    {
                        await db.DeleteAsync("reservas", "Id_reserva", todasreservas);
                    }
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Erro ao excluir horario");
            }
        }

        [HttpPost("MudarEstado/{IdReserva}/{Estado}")]
        public async Task<ActionResult> MudarEstadoId(string IdReserva, string Estado)
        {
            try
            {
                using (var db = new Database(conexaodb, "MySql.Data.MySqlClient"))
                {
                    var estado = await db.SingleOrDefaultAsync<Reservas>("SELECT * FROM reservas WHERE Id_reserva = @0", IdReserva);

                    if (estado == null)
                    {
                        return NotFound($"Não foi encontrado nenhuma reserva com o Id: {IdReserva}. Insira outro Id.");
                    }

                    if (Estado == "Cancelado")
                    {
                        if (estado.Data_reserva.Year <= DateTime.Now.Year)
                        {
                            if (estado.Data_reserva.Month <= DateTime.Now.Month)
                            {
                                if (estado.Data_reserva.Day <= DateTime.Now.Day)
                                {
                                    int resultadoComparacao = string.Compare(DateTime.Now.ToString("HH:mm"), estado.Horario);

                                    if (resultadoComparacao <= 0)
                                    {
                                        var erro5 = new { Mensagem = "Já não podes cancelar esta reserva já passou do horário" };
                                        return BadRequest(erro5);
                                    }
                                }
                            }
                        }
                    }
                    estado.Estado = Estado;

                    await db.UpdateAsync("reservas", "Id_reserva", estado);

                    return NoContent();
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Erro ao atualizar");
            }
        }
    }
}