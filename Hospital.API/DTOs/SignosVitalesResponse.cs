namespace Hospital.API.DTOs
{
    public class SignosVitalesResponse
    {
        public int IdSignos { get; set; }
        public int IdPaciente { get; set; }
        public DateTime FechaRegistro { get; set; }
        public int Oximetria { get; set; }
        public int FrecuenciaRespiratoria { get; set; }
        public int FrecuenciaCardiaca { get; set; }
        public int Temperatura { get; set; }
        public int PresionArterialSistolica { get; set; }
        public int PresionArterialDiastolica { get; set; }
        public int Glicemia { get; set; }
        public int IdRegistrador { get; set; }
        public string TipoRegistrador { get; set; }
        
        // Patient information
        public string NombrePaciente { get; set; }
        public string ApellidoPaciente { get; set; }
        public string Documento { get; set; }
    }
} 