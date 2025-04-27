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

    [Table("sugerenciascuidado")]
    public class SugerenciaCuidado
    {
        [Key]
        [Column("id_sugerencia")]
        public int IdSugerencia { get; set; }

        [Column("id_paciente")]
        public int IdPaciente { get; set; }

        [Column("id_medico")]
        public int IdMedico { get; set; }

        [Column("fecha_registro")]
        public DateTime FechaRegistro { get; set; }

        [Column("prioridad")]
        public string Prioridad { get; set; }

        [Column("descripcion")]
        public string Descripcion { get; set; }

        [Column("duracion_tratamiento")]
        public int DuracionTratamiento { get; set; }

        [Column("activo")]
        public bool Activo { get; set; }

        // Relaciones de navegación
        [ForeignKey("IdPaciente")]
        [ValidateNever]
        public Paciente Paciente { get; set; }

        [ForeignKey("IdMedico")]
        [ValidateNever]
        public Medico Medico { get; set; }
    }

}
