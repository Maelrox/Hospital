using Hospital.Domain.Entities;
using Hospital.Domain.Interfaces;
using Hospital.Domain.Services;
using Microsoft.AspNetCore.Mvc;
using Hospital.Controllers.DTOs;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hospital.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsuariosController : ControllerBase
    {
        private readonly IRepositorioGenerico<Usuario> _repositorio;
        private readonly IRepositorioGenerico<Medico> _repositorioMedico;
        private readonly IRepositorioGenerico<Paciente> _repositorioPaciente;
        private readonly IRepositorioGenerico<Familiar> _repositorioFamiliar;
        private readonly IJwtService _jwtService;

        public UsuariosController(
            IRepositorioGenerico<Usuario> repositorio,
            IRepositorioGenerico<Medico> repositorioMedico,
            IRepositorioGenerico<Paciente> repositorioPaciente,
            IRepositorioGenerico<Familiar> repositorioFamiliar,
            IJwtService jwtService)
        {
            _repositorio = repositorio;
            _repositorioMedico = repositorioMedico;
            _repositorioPaciente = repositorioPaciente;
            _repositorioFamiliar = repositorioFamiliar;
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
            // Validar campos únicos
            var usuarios = await _repositorio.ObtenerTodosAsync();
            
            if (usuarios.Any(u => u.NombreUsuario == crearUsuario.NombreUsuario))
            {
                return BadRequest(new { error = "El nombre de usuario ya está en uso." });
            }

            if (usuarios.Any(u => u.DocumentoIdentidad == crearUsuario.DocumentoIdentidad))
            {
                return BadRequest(new { error = "El documento de identidad ya está registrado." });
            }

            if (usuarios.Any(u => u.CorreoElectronico == crearUsuario.CorreoElectronico))
            {
                return BadRequest(new { error = "El correo electrónico ya está registrado." });
            }

            // Validar tipo de usuario
            if (!new[] { "Médico", "Paciente", "Familiar" }.Contains(crearUsuario.TipoUsuario))
            {
                return BadRequest(new { error = "Tipo de usuario inválido. Debe ser 'Médico', 'Paciente' o 'Familiar'." });
            }

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
                FechaRegistro = DateTime.Now
            };

            await _repositorio.CrearAsync(usuario);

            // Generate JWT token
            var token = _jwtService.GenerateToken(usuario.IdUsuario, usuario.CorreoElectronico, usuario.TipoUsuario);

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
            var token = _jwtService.GenerateToken(usuario.IdUsuario, usuario.CorreoElectronico, usuario.TipoUsuario);

            var usuarioResuesta = new AutenticacionUsuario
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

            switch (usuario.TipoUsuario)
            {
                case "Medico":
                    var medico = (await _repositorioMedico.ObtenerTodosAsync())
                        .FirstOrDefault(m => m.IdUsuario == usuario.IdUsuario);
                    usuarioResuesta.IdMedico = medico?.IdMedico;
                    break;
                case "Paciente":
                    var paciente = (await _repositorioPaciente.ObtenerTodosAsync())
                        .FirstOrDefault(p => p.IdUsuario == usuario.IdUsuario);
                    usuarioResuesta.IdPaciente = paciente?.IdPaciente;
                    break;
                case "Familiar":
                    var familiar = (await _repositorioFamiliar.ObtenerTodosAsync())
                        .FirstOrDefault(f => f.IdUsuario == usuario.IdUsuario);
                    usuarioResuesta.IdFamiliar = familiar?.IdFamiliar;
                    usuarioResuesta.IdPaciente = familiar?.IdPaciente;
                    break;
            }

            return Ok(usuarioResuesta);
        }
    }
}
