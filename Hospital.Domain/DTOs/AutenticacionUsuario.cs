using System;
using System.ComponentModel.DataAnnotations;

namespace Hospital.Domain.DTOs
{
    public class AutenticacionUsuario
    {
        public string? Direccion { get; set; }
        public string? Telefono { get; set; }
        public string Token { get; set; } = string.Empty;
    }
} 