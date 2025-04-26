using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Hospital.Domain.Entities
{

    [Table("pacientes")]
    public class Paciente
    {
        [Key]
        [Column("id_paciente")]
        public int IdPaciente { get; set; }

        [Column("id_usuario")]
        public int IdUsuario { get; set; }

        [Column("fecha_nacimiento")]
        public DateTime FechaNacimiento { get; set; }

        [Column("sexo")]
        public string Sexo { get; set; }

        [Column("tipo_sangre")]
        public string TipoSangre { get; set; }

        [Column("alergias")]
        public string Alergias { get; set; }

        [Column("antecedentes")]
        public string Antecedentes { get; set; }

        [Column("direccion_residencia")]
        public string DireccionResidencia { get; set; }

        [Column("contacto_emergencia")]
        public string ContactoEmergencia { get; set; }

        [Column("parentesco_contacto")]
        public string ParentescoContacto { get; set; }

        [Column("telefono_emergencia")]
        public string TelefonoEmergencia { get; set; }

        // Relaciones de navegación (no llevan [Column])
        [ForeignKey("IdUsuario")]
        [ValidateNever]
        public Usuario Usuario { get; set; }
        public ICollection<Familiar> Familiares { get; set; }

        public ICollection<SignosVitales> SignosVitales { get; set; }

    }
}
