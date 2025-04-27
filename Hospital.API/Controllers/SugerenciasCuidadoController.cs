using Hospital.Domain.Entities;
using Hospital.Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Hospital.API.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class SugerenciasCuidadoController : ControllerBase
    {
        private readonly IRepositorioGenerico<SugerenciaCuidado> _repositorio;

        public SugerenciasCuidadoController(IRepositorioGenerico<SugerenciaCuidado> repositorio)
        {
            _repositorio = repositorio;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<SugerenciaCuidado>>> ObtenerTodos()
        {
            var tipoUsuario = User.FindFirst(ClaimTypes.Role)?.Value;
            var idUsuario = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

            if (tipoUsuario == "Médico")
            {
                var entidades = await _repositorio.ObtenerTodosAsync();
                return Ok(entidades);
            }
            else if (tipoUsuario == "Paciente")
            {
                var entidades = await _repositorio.ObtenerTodosAsync();
                var sugerenciasPaciente = entidades.Where(s => s.IdPaciente == idUsuario);
                return Ok(sugerenciasPaciente);
            }
            else if (tipoUsuario == "Familiar")
            {
                var entidades = await _repositorio.ObtenerTodosAsync();
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
            var tipoUsuario = User.FindFirst(ClaimTypes.Role)?.Value;
            var idUsuario = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

            var entidad = await _repositorio.ObtenerPorIdAsync(id);
            if (entidad == null)
                return NotFound();

            if (tipoUsuario == "Médico" || 
                (tipoUsuario == "Paciente" && entidad.IdPaciente == idUsuario) ||
                tipoUsuario == "Familiar")
            {
                return Ok(entidad);
            }

            return Forbid();
        }

        [HttpPost]
        public async Task<ActionResult> Crear([FromBody] SugerenciaCuidado entidad)
        {
            var tipoUsuario = User.FindFirst(ClaimTypes.Role)?.Value;
            if (tipoUsuario != "Médico")
            {
                return Forbid();
            }

            await _repositorio.CrearAsync(entidad);
            return CreatedAtAction(nameof(ObtenerPorId), new { id = entidad.IdSugerencia }, entidad);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Actualizar(int id, [FromBody] SugerenciaCuidado entidad)
        {
            var tipoUsuario = User.FindFirst(ClaimTypes.Role)?.Value;
            if (tipoUsuario != "Médico")
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
            var tipoUsuario = User.FindFirst(ClaimTypes.Role)?.Value;
            if (tipoUsuario != "Médico")
            {
                return Forbid();
            }

            await _repositorio.EliminarAsync(id);
            return NoContent();
        }
    }
}
