using Hospital.Domain.Entities;
using Hospital.Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Hospital.API.DTOs;
using System.Security.Claims;

namespace Hospital.API.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class SignosVitalesController : ControllerBase
    {
        private readonly IRepositorioGenerico<SignosVitales> _repositorio;
        private readonly IRepositorioGenerico<RelacionMedicoPaciente> _repositorioRelacion;
        private readonly IRepositorioGenerico<Familiar> _repositorioFamiliar;

        public SignosVitalesController(
            IRepositorioGenerico<SignosVitales> repositorio,
            IRepositorioGenerico<RelacionMedicoPaciente> repositorioRelacion,
            IRepositorioGenerico<Familiar> repositorioFamiliar)
        {
            _repositorio = repositorio;
            _repositorioRelacion = repositorioRelacion;
            _repositorioFamiliar = repositorioFamiliar;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<SignosVitalesResponse>>> ObtenerTodos()
        {
            var tipoUsuario = User.FindFirst(ClaimTypes.Role)?.Value;
            var idUsuario = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

            var signosVitales = await _repositorio.ObtenerTodosAsync(s => s.Paciente, s => s.Paciente.Usuario);
            
            if (tipoUsuario == "Paciente")
            {
                signosVitales = signosVitales.Where(s => s.IdPaciente == idUsuario);
            }
            else if (tipoUsuario == "Familiar")
            {
                var familiar = await _repositorioFamiliar.ObtenerPorCampoAsync(f => f.IdUsuario == idUsuario);
                if (familiar == null)
                {
                    return Forbid();
                }
                signosVitales = signosVitales.Where(s => s.IdPaciente == familiar.IdPaciente);
            }

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
            var tipoUsuario = User.FindFirst(ClaimTypes.Role)?.Value;
            var idUsuario = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

            var signosVitales = await _repositorio.ObtenerPorIdAsync(id);
            if (signosVitales == null)
                return NotFound();

            if (tipoUsuario == "Médico")
            {
                // Médicos can see all records
            }
            else if (tipoUsuario == "Paciente" && signosVitales.IdPaciente != idUsuario)
            {
                return Forbid();
            }
            else if (tipoUsuario == "Familiar")
            {
                var familiar = await _repositorioFamiliar.ObtenerPorCampoAsync(f => f.IdUsuario == idUsuario);
                if (familiar == null || familiar.IdPaciente != signosVitales.IdPaciente)
                {
                    return Forbid();
                }
            }

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
            var tipoUsuario = User.FindFirst(ClaimTypes.Role)?.Value;
            var idUsuario = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

            if (tipoUsuario == "Médico")
            {
                entidad.IdRegistrador = idUsuario;
                entidad.TipoRegistrador = "Médico";
            }
            else if (tipoUsuario == "Familiar")
            {
                var familiar = await _repositorioFamiliar.ObtenerPorCampoAsync(f => f.IdUsuario == idUsuario);
                if (familiar == null || !familiar.AutorizadoRegistroSignos)
                {
                    return BadRequest(new { error = "No está autorizado para registrar signos vitales." });
                }
                entidad.IdRegistrador = idUsuario;
                entidad.TipoRegistrador = "Familiar";
            }
            else
            {
                return Forbid();
            }

            try
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
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Actualizar(int id, [FromBody] SignosVitales entidad)
        {
            var tipoUsuario = User.FindFirst(ClaimTypes.Role)?.Value;
            if (tipoUsuario != "Médico")
            {
                return Forbid();
            }

            if (id != entidad.IdSignos)
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
