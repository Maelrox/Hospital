using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Hospital.Domain.Entities
{

    [Table("usuarios")]
    public class Usuario
    {
        [Key]
        [Column("id_usuario")]
        public int IdUsuario { get; set; }

        [Column("tipo_usuario")]
        public string TipoUsuario { get; set; }

        [Column("nombre")]
        public string Nombre { get; set; }

        [Column("apellido")]
        public string Apellido { get; set; }

        [Column("documento_identidad")]
        public string DocumentoIdentidad { get; set; }

        [Column("direccion")]
        public string Direccion { get; set; }

        [Column("telefono")]
        public string Telefono { get; set; }

        [Column("correo_electronico")]
        public string CorreoElectronico { get; set; }

        [Column("nombre_usuario")]
        public string NombreUsuario { get; set; }

        [Column("contraseña")]
        [JsonIgnore]
        public string Contraseña { get; set; }

        [Column("estado")]
        public bool Estado { get; set; }

        [Column("fecha_registro")]
        public DateTime FechaRegistro { get; set; }

        // Relaciones de navegación
        public ICollection<Paciente> Pacientes { get; set; }
        public ICollection<Familiar> Familiares { get; set; }
        public ICollection<Medico> Medicos { get; set; }
        public ICollection<Auditoria> Auditorias { get; set; }
    }
}
