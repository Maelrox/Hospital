using Hospital.Domain.Entities;
using Hospital.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Hospital.API.Controllers
{
    [ApiController]
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
            var entidades = await _repositorio.ObtenerTodosAsync();
            return Ok(entidades);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<SugerenciaCuidado>> ObtenerPorId(int id)
        {
            var entidad = await _repositorio.ObtenerPorIdAsync(id);
            if (entidad == null)
                return NotFound();
            return Ok(entidad);
        }

        [HttpPost]
        public async Task<ActionResult> Crear([FromBody] SugerenciaCuidado entidad)
        {
            await _repositorio.CrearAsync(entidad);
            return CreatedAtAction(nameof(ObtenerPorId), new { id = entidad.IdSugerencia }, entidad);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Actualizar(int id, [FromBody] SugerenciaCuidado entidad)
        {
            if (id != entidad.IdSugerencia)
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
