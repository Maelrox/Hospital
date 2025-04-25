using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hospital.Domain.Entities
{
    [Table("familiares")]
    public class Familiar
    {
        [Key]
        [Column("id_familiar")]
        public int IdFamiliar { get; set; }

        [Column("id_usuario")]
        public int IdUsuario { get; set; }

        [Column("id_paciente")]
        public int IdPaciente { get; set; }

        [Column("parentesco")]
        public string Parentesco { get; set; }

        [Column("autorizado_registro_signos")]
        public bool AutorizadoRegistroSignos { get; set; }

        // Relaciones de navegación
        [ForeignKey("IdUsuario")]
        public Usuario Usuario { get; set; }

        [ForeignKey("IdPaciente")]
        public Paciente Paciente { get; set; }
    }


}
