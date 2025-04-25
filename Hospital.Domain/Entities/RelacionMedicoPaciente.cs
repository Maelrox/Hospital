using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hospital.Domain.Entities
{

    [Table("relacionmedicopaciente")]
    public class RelacionMedicoPaciente
    {
        [Key]
        [Column("id_relacion")]
        public int IdRelacion { get; set; }

        [Column("id_medico")]
        public int IdMedico { get; set; }

        [Column("id_paciente")]
        public int IdPaciente { get; set; }

        [Column("fecha_asignacion")]
        public DateTime FechaAsignacion { get; set; }

        [Column("estado")]
        public bool Estado { get; set; }

        // Relaciones de navegación
        [ForeignKey("IdMedico")]
        public Medico Medico { get; set; }

        [ForeignKey("IdPaciente")]
        public Paciente Paciente { get; set; }
    }

}
