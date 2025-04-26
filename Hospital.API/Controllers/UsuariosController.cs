using Hospital.Domain.Entities;
using Hospital.Domain.Interfaces;
using Hospital.Domain.Services;
using Microsoft.AspNetCore.Mvc;
using Hospital.Controllers.DTOs;

namespace Hospital.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsuariosController : ControllerBase
    {
        private readonly IRepositorioGenerico<Usuario> _repositorio;
        private readonly IJwtService _jwtService;

        public UsuariosController(IRepositorioGenerico<Usuario> repositorio, IJwtService jwtService)
        {
            _repositorio = repositorio;
            _jwtService = jwtService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Usuario>>> ObtenerTodos()
        {
            var entidades = await _repositorio.ObtenerTodosAsync();
            return Ok(entidades);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Usuario>> ObtenerPorId(int id)
        {
            var entidad = await _repositorio.ObtenerPorIdAsync(id);
            if (entidad == null)
                return NotFound();
            return Ok(entidad);
        }

        [HttpPost]
        public async Task<ActionResult> Crear([FromBody] CrearUsuario crearUsuario)
        {
            // Encriptar la contraseña
            string passwordHash = BCrypt.Net.BCrypt.HashPassword(crearUsuario.Contraseña);

            var usuario = new Usuario
            {
                TipoUsuario = crearUsuario.TipoUsuario,
                Nombre = crearUsuario.Nombre,
                Apellido = crearUsuario.Apellido,
                DocumentoIdentidad = crearUsuario.DocumentoIdentidad,
                Direccion = crearUsuario.Direccion,
                Telefono = crearUsuario.Telefono,
                CorreoElectronico = crearUsuario.CorreoElectronico,
                NombreUsuario = crearUsuario.NombreUsuario,
                Contraseña = passwordHash,
                Estado = true, // Activo por defecto
                FechaRegistro = DateTime.UtcNow,
                Pacientes = new List<Paciente>(),
                Familiares = new List<Familiar>(),
                Medicos = new List<Medico>(),
                Auditorias = new List<Auditoria>()
            };

            await _repositorio.CrearAsync(usuario);

            // Generate JWT token
            var token = _jwtService.GenerateToken(usuario.IdUsuario, usuario.CorreoElectronico);

            var usuarioResponse = new AutenticacionUsuario
            {
                IdUsuario = usuario.IdUsuario,
                NombreUsuario = usuario.NombreUsuario,
                Nombre = usuario.Nombre,
                Apellido = usuario.Apellido,
                CorreoElectronico = usuario.CorreoElectronico,
                TipoUsuario = usuario.TipoUsuario,
                Estado = usuario.Estado,
                Direccion = usuario.Direccion,
                Telefono = usuario.Telefono,
                Token = token
            };

            return CreatedAtAction(nameof(ObtenerPorId), new { id = usuario.IdUsuario }, usuarioResponse);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Actualizar(int id, [FromBody] Usuario entidad)
        {
            if (id != entidad.IdUsuario)
                return BadRequest();
            await _repositorio.ActualizarAsync(entidad);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Eliminar(int id)
        {
            await _repositorio.EliminarAsync(id);
            return NoContent();
        }

        [HttpPost("login")]
        public async Task<ActionResult<AutenticacionUsuario>> Login([FromBody] LoginUsuario loginUsuario)
        {
            var usuarios = await _repositorio.ObtenerTodosAsync();
            var usuario = usuarios.FirstOrDefault(u => u.NombreUsuario == loginUsuario.NombreUsuario);

            if (usuario == null)
                return Unauthorized("Usuario no encontrado");

            bool contraseñaValida = BCrypt.Net.BCrypt.Verify(loginUsuario.Contraseña, usuario.Contraseña);

            if (!contraseñaValida)
                return Unauthorized("Contraseña incorrecta");

            // Generate JWT token
            var token = _jwtService.GenerateToken(usuario.IdUsuario, usuario.CorreoElectronico);

            var usuarioResponse = new AutenticacionUsuario
            {
                IdUsuario = usuario.IdUsuario,
                NombreUsuario = usuario.NombreUsuario,
                Nombre = usuario.Nombre,
                Apellido = usuario.Apellido,
                CorreoElectronico = usuario.CorreoElectronico,
                TipoUsuario = usuario.TipoUsuario,
                Estado = usuario.Estado,
                Direccion = usuario.Direccion,
                Telefono = usuario.Telefono,
                Token = token
            };

            return Ok(usuarioResponse);
        }
    }
}
