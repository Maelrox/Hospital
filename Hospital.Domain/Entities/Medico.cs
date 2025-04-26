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
    [Table("medicos")]
    public class Medico
    {
        [Key]
        [Column("id_medico")]
        public int IdMedico { get; set; }

        [Column("id_usuario")]
        public int IdUsuario { get; set; }

        [Column("especialidad")]
        public string Especialidad { get; set; }

        [Column("numero_licencia")]
        public string NumeroLicencia { get; set; }

        [Column("registro_profesional")]
        public string RegistroProfesional { get; set; }

        [Column("años_experiencia")]
        public int AniosExperiencia { get; set; }

        // Relaciones
        [ForeignKey("IdUsuario")]
        [ValidateNever]
        public Usuario Usuario { get; set; }
        [ValidateNever]
        public ICollection<RelacionMedicoPaciente> RelacionesMedicoPaciente { get; set; }
        [ValidateNever]
        public ICollection<SugerenciaCuidado> SugerenciasCuidado { get; set; }
        [ValidateNever]
        public ICollection<HistorialClinico> HistorialesClinicos { get; set; }
    }


}
