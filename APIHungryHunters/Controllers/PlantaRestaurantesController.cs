﻿using System;
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
    public class PlantaRestaurantesController : ControllerBase
    {
        private readonly TodoContext _context;

        public PlantaRestaurantesController(TodoContext context)
        {
            _context = context;
        }

        string conexaodb = "Server=localhost;Port=3306;Database=hungryhunters;Uid=root;";

        [HttpPost("AdicionarPlanta")]
        public async Task<IActionResult> AdicionarPlantaRestaurante([FromForm] PlantaRestauranteDTO plantaRestauranteDto)
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<PlantaRestauranteDTO, PlantaRestaurante>();
            });
            AutoMapper.IMapper mapper = config.CreateMapper();

            using (var db = new Database(conexaodb, "MySql.Data.MySqlClient"))
            {
                if (plantaRestauranteDto.Planta_image != null && plantaRestauranteDto.Planta_image.Length > 0)
                {
                    string nomeArquivo = plantaRestauranteDto.Planta_image.FileName;

                    string caminhoArquivo = Path.Combine(".\\Imagens", nomeArquivo);

                    using (var stream = new FileStream(caminhoArquivo, FileMode.Create))
                    {
                        await plantaRestauranteDto.Planta_image.CopyToAsync(stream);
                    }

                    var plantaRestaurante = mapper.Map<PlantaRestaurante>(plantaRestauranteDto);
                    plantaRestaurante.Planta_titulo = nomeArquivo;

                    await db.InsertAsync("PlantaRestaurante", "Id_planta", true, plantaRestaurante);

                    return Ok("PlantaRestaurante adicionado com sucesso!");
                }
            }

            return BadRequest("A imagem não foi fornecida ou é inválida.");
        }


    }
}

