using Microsoft.AspNetCore.Mvc;

namespace Hospital.API.Controllers
{
    using Hospital.Domain.Entities;
    using Hospital.Domain.Interfaces;
    using Microsoft.AspNetCore.Mvc;

    [ApiController]
    [Route("api/[controller]")]
    public class PacientesController : ControllerBase
    {
        private readonly IRepositorioGenerico<Paciente> _repositorio;

        public PacientesController(IRepositorioGenerico<Paciente> repositorio)
        {
            _repositorio = repositorio;
        }

            [HttpGet]
            public async Task<ActionResult<IEnumerable<Paciente>>> ObtenerTodos()
            {
                var pacientes = await _repositorio.ObtenerTodosAsync(p => p.Usuario);
                return Ok(pacientes);
            }

        [HttpGet("{id}")]
        public async Task<ActionResult<Paciente>> ObtenerPorId(int id)
        {
            var paciente = await _repositorio.ObtenerPorIdAsync(id);
            if (paciente == null)
                return NotFound();
            return Ok(paciente);
        }

        [HttpPost]
        public async Task<ActionResult> Crear([FromBody] Paciente paciente)
        {
            await _repositorio.CrearAsync(paciente);
            return CreatedAtAction(nameof(ObtenerPorId), new { id = paciente.IdPaciente }, paciente);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Actualizar(int id, [FromBody] Paciente paciente)
        {
            if (id != paciente.IdPaciente)
                return BadRequest();

            await _repositorio.ActualizarAsync(paciente);
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
