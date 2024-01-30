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
    public class ImagemMenusController : ControllerBase
    {
        private readonly TodoContext _context;

        public ImagemMenusController(TodoContext context)
        {
            _context = context;
        }

        string conexaodb = "Server=localhost;Port=3306;Database=hungryhunters;Uid=root;";

        [HttpPost("AdicionarImagemMenu")]
        public async Task<ActionResult> AdicionarImagemMenu([FromForm] ImagemMenuDTO imagemMenuDTO)
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<ImagemMenuDTO, ImagemMenu>();
            });
            AutoMapper.IMapper mapper = config.CreateMapper();

            using (var db = new Database(conexaodb, "MySql.Data.MySqlClient"))
            {
                if (imagemMenuDTO.Menu_imagem != null && imagemMenuDTO.Menu_imagem.Count > 0)
                {
                    foreach (var arquivo in imagemMenuDTO.Menu_imagem)
                    {
                        string nomeArquivo = $@"{Guid.NewGuid()}{arquivo.FileName}";

                        string caminhoArquivo = Path.Combine(".\\ImagensMenu", nomeArquivo);

                        using (var stream = new FileStream(caminhoArquivo, FileMode.Create))
                        {
                            await arquivo.CopyToAsync(stream);
                        }

                        var imagemMenu = mapper.Map<ImagemMenu>(imagemMenuDTO);
                        imagemMenu.Imagem_titulo = nomeArquivo;

                        await db.InsertAsync("imagemmenu", "Id_imagemmenu", true, imagemMenu);
                    }

                    return Ok("ImagemMenu adicionado com sucesso!");
                }
            }
            return BadRequest("A imagem não foi fornecida ou é inválida.");
        }
    }
}
