using Hospital.Domain.Entities;
using Hospital.Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Hospital.API.DTOs;

namespace Hospital.API.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class HistorialesClinicosController : ControllerBase
    {
        private readonly IRepositorioGenerico<HistorialClinico> _repositorio;
        private readonly IRepositorioGenerico<Paciente> _repositorioPaciente;

        public HistorialesClinicosController(
            IRepositorioGenerico<HistorialClinico> repositorio,
            IRepositorioGenerico<Paciente> repositorioPaciente)
        {
            _repositorio = repositorio;
            _repositorioPaciente = repositorioPaciente;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<HistorialClinicoResponse>>> ObtenerTodos()
        {
            var historiales = await _repositorio.ObtenerTodosAsync(h => h.Paciente, h => h.Paciente.Usuario);
            var response = historiales.Select(h => new HistorialClinicoResponse
            {
                IdHistorial = h.IdHistorial,
                IdPaciente = h.IdPaciente,
                IdMedico = h.IdMedico,
                FechaRegistro = h.FechaRegistro,
                Diagnostico = h.Diagnostico,
                Observaciones = h.Observaciones,
                Tratamiento = h.Tratamiento,
                SeguimientoRequerido = h.SeguimientoRequerido,
                NombrePaciente = h.Paciente.Usuario.Nombre,
                ApellidoPaciente = h.Paciente.Usuario.Apellido,
                Documento = h.Paciente.Usuario.DocumentoIdentidad
            });

            return Ok(response);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<HistorialClinico>> ObtenerPorId(int id)
        {
            var entidad = await _repositorio.ObtenerPorIdAsync(id);
            if (entidad == null)
                return NotFound();
            return Ok(entidad);
        }

        [HttpPost]
        public async Task<ActionResult> Crear([FromBody] HistorialClinico entidad)
        {
            await _repositorio.CrearAsync(entidad);
            return CreatedAtAction(nameof(ObtenerPorId), new { id = entidad.IdHistorial }, entidad);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Actualizar(int id, [FromBody] HistorialClinico entidad)
        {
            if (id != entidad.IdHistorial)
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
    }
}
