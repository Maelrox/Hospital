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
    [Table("historialclinico")]
    public class HistorialClinico
    {
        [Key]
        [Column("id_historial")]
        public int IdHistorial { get; set; }

        [Column("id_paciente")]
        public int IdPaciente { get; set; }

        [Column("id_medico")]
        public int IdMedico { get; set; }

        [Column("fecha_registro", TypeName = "timestamp without time zone")]
        public DateTime FechaRegistro { get; set; }

        [Column("diagnostico")]
        public string Diagnostico { get; set; }

        [Column("observaciones")]
        public string Observaciones { get; set; }

        [Column("tratamiento")]
        public string Tratamiento { get; set; }

        [Column("seguimiento_requerido")]
        public bool SeguimientoRequerido { get; set; }

        // Relaciones de navegación
        [ForeignKey("IdPaciente")]
        [ValidateNever]
        public Paciente Paciente { get; set; }

        [ForeignKey("IdMedico")]
        [ValidateNever]
        public Medico Medico { get; set; }
    }

}
