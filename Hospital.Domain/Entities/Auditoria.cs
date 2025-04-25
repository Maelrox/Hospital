using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hospital.Domain.Entities
{
    [Table("auditoria")]
    public class Auditoria
    {
        [Key]
        [Column("id_auditoria")]
        public int IdAuditoria { get; set; }

        [Column("id_usuario")]
        public int IdUsuario { get; set; }

        [Column("tipo_operacion")]
        public string TipoOperacion { get; set; }

        [Column("fecha_hora")]
        public DateTime FechaHora { get; set; }

        [Column("tabla_afectada")]
        public string TablaAfectada { get; set; }

        [Column("descripcion_cambio")]
        public string DescripcionCambio { get; set; }

        [Column("ip_acceso")]
        public string IpAcceso { get; set; }

        // Relaciones
        [ForeignKey("IdUsuario")]
        public Usuario Usuario { get; set; }
    }
}
