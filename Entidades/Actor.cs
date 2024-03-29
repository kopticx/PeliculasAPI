﻿using System.ComponentModel.DataAnnotations;

namespace PeliculasAPI.Entidades
{
    public class Actor
    {
        public int Id { get; set; }
        [StringLength(maximumLength: 200)]
        public string Nombre { get; set; }
        public string Biografia { get; set; }
        public DateTime FechaNacimiento { get; set; }
        public string FotoURL { get; set; }
        public List<PeliculasActores> PeliculasActores { get; set; }
    }
}
