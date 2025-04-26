using Hospital.Domain.Entities;
using Hospital.Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Hospital.API.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class HistorialesClinicosController : ControllerBase
    {
        private readonly IRepositorioGenerico<HistorialClinico> _repositorio;

        public HistorialesClinicosController(IRepositorioGenerico<HistorialClinico> repositorio)
        {
            _repositorio = repositorio;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<HistorialClinico>>> ObtenerTodos()
        {
            var entidades = await _repositorio.ObtenerTodosAsync();
            return Ok(entidades);
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
