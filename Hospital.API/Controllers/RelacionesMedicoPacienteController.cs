using Hospital.Domain.Entities;
using Hospital.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Hospital.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RelacionesMedicoPacienteController : ControllerBase
    {
        private readonly IRepositorioGenerico<RelacionMedicoPaciente> _repositorio;

        public RelacionesMedicoPacienteController(IRepositorioGenerico<RelacionMedicoPaciente> repositorio)
        {
            _repositorio = repositorio;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<RelacionMedicoPaciente>>> ObtenerTodos()
        {
            var entidades = await _repositorio.ObtenerTodosAsync();
            return Ok(entidades);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<RelacionMedicoPaciente>> ObtenerPorId(int id)
        {
            var entidad = await _repositorio.ObtenerPorIdAsync(id);
            if (entidad == null)
                return NotFound();
            return Ok(entidad);
        }

        [HttpPost]
        public async Task<ActionResult> Crear([FromBody] RelacionMedicoPaciente entidad)
        {
            await _repositorio.CrearAsync(entidad);
            return CreatedAtAction(nameof(ObtenerPorId), new { id = entidad.IdRelacion }, entidad);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Actualizar(int id, [FromBody] RelacionMedicoPaciente entidad)
        {
            if (id != entidad.IdRelacion)
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
