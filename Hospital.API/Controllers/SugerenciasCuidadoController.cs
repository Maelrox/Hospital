using Hospital.Domain.Entities;
using Hospital.Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Hospital.Domain.Enums;

namespace Hospital.API.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class SugerenciasCuidadoController : ControllerBase
    {
        private readonly IRepositorioGenerico<SugerenciaCuidado> _repositorio;
        private readonly IRepositorioGenerico<Paciente> _repositorioPaciente;

        public SugerenciasCuidadoController(
            IRepositorioGenerico<SugerenciaCuidado> repositorio,
            IRepositorioGenerico<Paciente> repositorioPaciente)
        {
            _repositorio = repositorio;
            _repositorioPaciente = repositorioPaciente;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<SugerenciaCuidado>>> ObtenerTodos()
        {
            var tipoUsuarioStr = User.FindFirst(ClaimTypes.Role)?.Value;
            if (!Enum.TryParse<TipoUsuario>(tipoUsuarioStr, out var tipoUsuario))
            {
                return Forbid();
            }

            var idUsuario = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

            if (tipoUsuario == TipoUsuario.Medico)
            {
                var entidades = await _repositorio.ObtenerTodosAsync(s => s.Paciente, s => s.Paciente.Usuario);
                return Ok(entidades);
            }
            else if (tipoUsuario == TipoUsuario.Paciente)
            {
                var paciente = await _repositorioPaciente.ObtenerPorCampoAsync(p => p.IdUsuario == idUsuario);
                if (paciente == null)
                {
                    return Forbid();
                }
                var entidades = await _repositorio.ObtenerTodosAsync(s => s.Paciente, s => s.Paciente.Usuario);
                var sugerenciasPaciente = entidades.Where(s => s.IdPaciente == paciente.IdPaciente);
                return Ok(sugerenciasPaciente);
            }
            else if (tipoUsuario == TipoUsuario.Familiar)
            {
                var entidades = await _repositorio.ObtenerTodosAsync(s => s.Paciente, s => s.Paciente.Usuario);
                return Ok(entidades);
            }
            else
            {
                return Forbid();
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<SugerenciaCuidado>> ObtenerPorId(int id)
        {
            var tipoUsuarioStr = User.FindFirst(ClaimTypes.Role)?.Value;
            if (!Enum.TryParse<TipoUsuario>(tipoUsuarioStr, out var tipoUsuario))
            {
                return Forbid();
            }

            var idUsuario = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

            var entidades = await _repositorio.ObtenerTodosAsync(s => s.Paciente, s => s.Paciente.Usuario);
            var entidad = entidades.FirstOrDefault(s => s.IdSugerencia == id);
            if (entidad == null)
                return NotFound();

            if (tipoUsuario == TipoUsuario.Medico || 
                (tipoUsuario == TipoUsuario.Paciente && entidad.IdPaciente == idUsuario) ||
                tipoUsuario == TipoUsuario.Familiar)
            {
                return Ok(entidad);
            }

            return Forbid();
        }

        [HttpPost]
        public async Task<ActionResult> Crear([FromBody] SugerenciaCuidado entidad)
        {
            var tipoUsuarioStr = User.FindFirst(ClaimTypes.Role)?.Value;
            if (!Enum.TryParse<TipoUsuario>(tipoUsuarioStr, out var tipoUsuario) || tipoUsuario != TipoUsuario.Medico)
            {
                return Forbid();
            }

            await _repositorio.CrearAsync(entidad);
            return CreatedAtAction(nameof(ObtenerPorId), new { id = entidad.IdSugerencia }, entidad);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Actualizar(int id, [FromBody] SugerenciaCuidado entidad)
        {
            var tipoUsuarioStr = User.FindFirst(ClaimTypes.Role)?.Value;
            if (!Enum.TryParse<TipoUsuario>(tipoUsuarioStr, out var tipoUsuario) || tipoUsuario != TipoUsuario.Medico)
            {
                return Forbid();
            }

            if (id != entidad.IdSugerencia)
                return BadRequest();
            await _repositorio.ActualizarAsync(entidad);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Eliminar(int id)
        {
            var tipoUsuarioStr = User.FindFirst(ClaimTypes.Role)?.Value;
            if (!Enum.TryParse<TipoUsuario>(tipoUsuarioStr, out var tipoUsuario) || tipoUsuario != TipoUsuario.Medico)
            {
                return Forbid();
            }

            await _repositorio.EliminarAsync(id);
            return NoContent();
        }
    }
}
