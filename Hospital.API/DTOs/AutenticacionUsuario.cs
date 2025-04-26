namespace Hospital.Controllers.DTOs
{
    public class AutenticacionUsuario
    {
        public int IdUsuario { get; set; }
        public string TipoUsuario { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string DocumentoIdentidad { get; set; }
        public string Direccion { get; set; }
        public string Telefono { get; set; }
        public string CorreoElectronico { get; set; }
        public string NombreUsuario { get; set; }
        public bool Estado { get; set; }
        public string Token { get; internal set; }

    }
}