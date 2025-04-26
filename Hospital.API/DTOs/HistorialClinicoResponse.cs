namespace Hospital.API.DTOs
{
    public class HistorialClinicoResponse
    {
        public int IdHistorial { get; set; }
        public int IdPaciente { get; set; }
        public int IdMedico { get; set; }
        public DateTime FechaRegistro { get; set; }
        public string Diagnostico { get; set; }
        public string Observaciones { get; set; }
        public string Tratamiento { get; set; }
        public bool SeguimientoRequerido { get; set; }
        
        // Patient information
        public string NombrePaciente { get; set; }
        public string ApellidoPaciente { get; set; }
        public string Documento { get; set; }
    }
} 