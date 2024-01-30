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
                        "AND DAY(FimFerias) >= @Dia",
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
                            if(validDia == null)
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
                        "AND Horario = @Horario",
                            new
                            {
                                reservaDTO.ContaId,
                                Ano = reservaDTO.Data_reserva.Year,
                                Mes = reservaDTO.Data_reserva.Month,
                                Dia = reservaDTO.Data_reserva.Day,
                                Horario = reservaDTO.Horario,
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
                        "AND Mesa = @Mesa",
                        new
                        {
                            reservaDTO.RestauranteId,
                            Mesa = reservaDTO.MesaId,
                        });

                    if (validReserva != null)
                    {
                        var erro5 = new { Mensagem = "Essa mesa já está ocupada" };
                        return BadRequest(erro5);
                    }

                    var novaReserva = mapper.Map<Reservas>(reservaDTO);

                    await db.InsertAsync("reservas", "Id_reserva", true, novaReserva);
                }
            }
            return Ok();
        }
    }
}

