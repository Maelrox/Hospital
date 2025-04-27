using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Hospital.Domain.Entities
{

    [Table("signosvitales")]
    public class SignosVitales
    {
        [Key]
        [Column("id_signos")]
        public int IdSignos { get; set; }

        [Column("id_paciente")]
        public int IdPaciente { get; set; }
        
        [Column("fecha_registro", TypeName = "timestamp without time zone")]
        public DateTime FechaRegistro { get; set; }

        [Column("oximetria")]
        public int Oximetria { get; set; }

        [Column("frecuencia_respiratoria")]
        public int FrecuenciaRespiratoria { get; set; }

        [Column("frecuencia_cardiaca")]
        public int FrecuenciaCardiaca { get; set; }

        [Column("temperatura")]
        public int Temperatura { get; set; }

        [Column("presion_arterial_sistolica")]
        public int PresionArterialSistolica { get; set; }

        [Column("presion_arterial_diastolica")]
        public int PresionArterialDiastolica { get; set; }

        [Column("glicemia")]
        public int Glicemia { get; set; }

        [Column("id_registrador")]
        public int IdRegistrador { get; set; }

        [Column("tipo_registrador")]
        public string TipoRegistrador { get; set; }

        // Relación de navegación
        [ForeignKey("IdPaciente")]
        [ValidateNever]
        public Paciente Paciente { get; set; }
    }

}
