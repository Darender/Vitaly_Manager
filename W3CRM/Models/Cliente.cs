﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace W3CRM.Controllers
{
    public class Cliente
    {
		[Key]
		public int ID { get; set; }

		[Required]
		public required string Nombres { get; set; }
		public string? Apellidos { get; set; }

		[Required]
		public required string Telefono { get; set; }
		public int? Edad { get; set; }
		public string? Genero { get; set; }
		public DateTime? Ultima_Conusulta { get; set; }
		[Required]
		public required DateTime Ingreso { get; set; }
	}
}
