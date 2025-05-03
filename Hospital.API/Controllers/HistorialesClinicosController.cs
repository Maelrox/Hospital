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
    public class HistorialesClinicosController : ControllerBase
    {
        private readonly IRepositorioGenerico<HistorialClinico> _repositorio;
        private readonly IRepositorioGenerico<RelacionMedicoPaciente> _repositorioRelacion;
        private readonly IRepositorioGenerico<Medico> _repositorioMedico;
        private readonly IRepositorioGenerico<Paciente> _repositorioPaciente;

        public HistorialesClinicosController(
            IRepositorioGenerico<HistorialClinico> repositorio,
            IRepositorioGenerico<RelacionMedicoPaciente> repositorioRelacion,
            IRepositorioGenerico<Medico> repositorioMedico,
            IRepositorioGenerico<Paciente> repositorioPaciente)
        {
            _repositorio = repositorio;
            _repositorioRelacion = repositorioRelacion;
            _repositorioMedico = repositorioMedico;
            _repositorioPaciente = repositorioPaciente;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<HistorialClinico>>> ObtenerTodos()
        {
            var tipoUsuarioStr = User.FindFirst(ClaimTypes.Role)?.Value;
            if (!Enum.TryParse<TipoUsuario>(tipoUsuarioStr, out var tipoUsuario))
            {
                return Forbid();
            }
    
            var idUsuario = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var paciente = await _repositorioPaciente.ObtenerPorCampoAsync(p => p.IdUsuario == idUsuario);
            var historiales = await _repositorio.ObtenerTodosAsync(h => h.Paciente, h => h.Paciente.Usuario);

            if (tipoUsuario == TipoUsuario.Paciente)
            {
                historiales = historiales.Where(h => h.IdPaciente == paciente.IdPaciente);
            }
            else if (tipoUsuario == TipoUsuario.Familiar)
            {
                return Forbid();
            }

            return Ok(historiales);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<HistorialClinico>> ObtenerPorId(int id)
        {
            var tipoUsuarioStr = User.FindFirst(ClaimTypes.Role)?.Value;
            if (!Enum.TryParse<TipoUsuario>(tipoUsuarioStr, out var tipoUsuario))
            {
                return Forbid();
            }

            var idUsuario = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

            var historial = await _repositorio.ObtenerPorIdAsync(id);
            if (historial == null)
                return NotFound();

            if (tipoUsuario == TipoUsuario.Paciente && historial.IdPaciente != idUsuario)
            {
                return Forbid();
            }
            else if (tipoUsuario == TipoUsuario.Familiar)
            {
                return Forbid();
            }

            return Ok(historial);
        }

        [HttpPost]
        public async Task<ActionResult> Crear([FromBody] HistorialClinico entidad)
        {
            var tipoUsuarioStr = User.FindFirst(ClaimTypes.Role)?.Value;
            if (!Enum.TryParse<TipoUsuario>(tipoUsuarioStr, out var tipoUsuario) || tipoUsuario != TipoUsuario.Medico)
            {
                return Forbid();
            }

            var idUsuario = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var medico = await _repositorioMedico.ObtenerPorCampoAsync(m => m.IdUsuario == idUsuario);
            if (medico == null)
            {
                return Forbid();
            }

            entidad.FechaRegistro = DateTime.Now;
            entidad.IdMedico = medico.IdMedico;

            try
            {
                await _repositorio.CrearAsync(entidad);

                // Get all relationships for this patient
                var relaciones = await _repositorioRelacion.ObtenerTodosAsync();
                var relacionExistente = relaciones.FirstOrDefault(r => 
                    r.IdPaciente == entidad.IdPaciente && 
                    r.IdMedico == medico.IdMedico && 
                    r.Estado == true
                );

                if (relacionExistente == null)
                {
                    var nuevaRelacion = new RelacionMedicoPaciente
                    {
                        IdMedico = medico.IdMedico,
                        IdPaciente = entidad.IdPaciente,
                        FechaAsignacion = DateTime.Now,
                        Estado = true
                    };

                    await _repositorioRelacion.CrearAsync(nuevaRelacion);
                }

                return CreatedAtAction(nameof(ObtenerPorId), new { id = entidad.IdHistorial }, entidad);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Actualizar(int id, [FromBody] HistorialClinico entidad)
        {
            var tipoUsuarioStr = User.FindFirst(ClaimTypes.Role)?.Value;
            if (!Enum.TryParse<TipoUsuario>(tipoUsuarioStr, out var tipoUsuario) || tipoUsuario != TipoUsuario.Medico)
            {
                return Forbid();
            }

            var idUsuario = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var medico = await _repositorioMedico.ObtenerPorCampoAsync(m => m.IdUsuario == idUsuario);
            if (medico == null)
            {
                return Forbid();
            }

            var historialExistente = await _repositorio.ObtenerPorIdAsync(id);
            if (historialExistente == null)
            {
                return NotFound();
            }

            // Update the existing entity's properties
            historialExistente.Diagnostico = entidad.Diagnostico;
            historialExistente.Tratamiento = entidad.Tratamiento;
            historialExistente.Observaciones = entidad.Observaciones;
            historialExistente.SeguimientoRequerido = entidad.SeguimientoRequerido;
            historialExistente.IdMedico = medico.IdMedico;
            historialExistente.IdPaciente = entidad.IdPaciente;

            try
            {
                await _repositorio.ActualizarAsync(historialExistente);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
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
