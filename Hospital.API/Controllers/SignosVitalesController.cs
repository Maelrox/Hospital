using Hospital.Domain.Entities;
using Hospital.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Hospital.API.DTOs;

namespace Hospital.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SignosVitalesController : ControllerBase
    {
        private readonly IRepositorioGenerico<SignosVitales> _repositorio;
        private readonly IRepositorioGenerico<RelacionMedicoPaciente> _repositorioRelacion;

        public SignosVitalesController(
            IRepositorioGenerico<SignosVitales> repositorio,
            IRepositorioGenerico<RelacionMedicoPaciente> repositorioRelacion)
        {
            _repositorio = repositorio;
            _repositorioRelacion = repositorioRelacion;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<SignosVitalesResponse>>> ObtenerTodos()
        {
            var signosVitales = await _repositorio.ObtenerTodosAsync(s => s.Paciente, s => s.Paciente.Usuario);
            var response = signosVitales.Select(s => new SignosVitalesResponse
            {
                IdSignos = s.IdSignos,
                IdPaciente = s.IdPaciente,
                FechaRegistro = s.FechaRegistro,
                Oximetria = s.Oximetria,
                FrecuenciaRespiratoria = s.FrecuenciaRespiratoria,
                FrecuenciaCardiaca = s.FrecuenciaCardiaca,
                Temperatura = s.Temperatura,
                PresionArterialSistolica = s.PresionArterialSistolica,
                PresionArterialDiastolica = s.PresionArterialDiastolica,
                Glicemia = s.Glicemia,
                IdRegistrador = s.IdRegistrador,
                TipoRegistrador = s.TipoRegistrador,
                NombrePaciente = s.Paciente.Usuario.Nombre,
                ApellidoPaciente = s.Paciente.Usuario.Apellido,
                Documento = s.Paciente.Usuario.DocumentoIdentidad
            });

            return Ok(response);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<SignosVitalesResponse>> ObtenerPorId(int id)
        {
            var signosVitales = await _repositorio.ObtenerPorIdAsync(id);
            if (signosVitales == null)
                return NotFound();

            var response = new SignosVitalesResponse
            {
                IdSignos = signosVitales.IdSignos,
                IdPaciente = signosVitales.IdPaciente,
                FechaRegistro = signosVitales.FechaRegistro,
                Oximetria = signosVitales.Oximetria,
                FrecuenciaRespiratoria = signosVitales.FrecuenciaRespiratoria,
                FrecuenciaCardiaca = signosVitales.FrecuenciaCardiaca,
                Temperatura = signosVitales.Temperatura,
                PresionArterialSistolica = signosVitales.PresionArterialSistolica,
                PresionArterialDiastolica = signosVitales.PresionArterialDiastolica,
                Glicemia = signosVitales.Glicemia,
                IdRegistrador = signosVitales.IdRegistrador,
                TipoRegistrador = signosVitales.TipoRegistrador,
                NombrePaciente = signosVitales.Paciente.Usuario.Nombre,
                ApellidoPaciente = signosVitales.Paciente.Usuario.Apellido,
                Documento = signosVitales.Paciente.Usuario.DocumentoIdentidad
            };

            return Ok(response);
        }

        [HttpPost]
        public async Task<ActionResult> Crear([FromBody] SignosVitales entidad)
        {
            await _repositorio.CrearAsync(entidad);

            // Get all relationships for this patient
            var relaciones = await _repositorioRelacion.ObtenerTodosAsync();
            var relacionExistente = relaciones.FirstOrDefault(r => 
                r.IdPaciente == entidad.IdPaciente && 
                r.IdMedico == entidad.IdRegistrador && 
                r.Estado == true
            );

            if (relacionExistente == null)
            {
                var nuevaRelacion = new RelacionMedicoPaciente
                {
                    IdMedico = entidad.IdRegistrador,
                    IdPaciente = entidad.IdPaciente,
                    FechaAsignacion = DateTime.Now,
                    Estado = true
                };

                await _repositorioRelacion.CrearAsync(nuevaRelacion);
            }

            return CreatedAtAction(nameof(ObtenerPorId), new { id = entidad.IdSignos }, entidad);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Actualizar(int id, [FromBody] SignosVitales entidad)
        {
            if (id != entidad.IdSignos)
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
