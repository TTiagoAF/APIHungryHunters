﻿using PetaPoco;
using System.ComponentModel.DataAnnotations;

namespace APIHungryHunters.Models
{
    public class Contas
    {
        [Key]
        public int Id_conta { get; set; }

        public string Email { get; set; }
        public string Username { get; set; }
        public DateTime datadenascimento { get; set; }
        public int Pontos { get; set; }
        public string Password { get; set; }
        [ResultColumn]
        public List<Reservas> Reservas { get; set; }
        [ResultColumn]
        public List<Avaliacoes> Avaliacoes { get; set; }
		[ResultColumn]
		public List<Logs> Logs { get; set; }
	}
}
