namespace Hospital.Data
{
    using Hospital.Domain.Entities;
    using Microsoft.EntityFrameworkCore;
    using System.Collections.Generic;
    using System.Reflection.Emit;

    public class HospitalDbContext : DbContext
    {
        public HospitalDbContext(DbContextOptions<HospitalDbContext> options) : base(options) { }

        public DbSet<Paciente> Pacientes { get; set; }
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Familiar> Familiares { get; set; }
        public DbSet<Medico> Medicos { get; set; }
        public DbSet<RelacionMedicoPaciente> RelacionesMedicoPaciente { get; set; }
        public DbSet<HistorialClinico> HistorialesClinicos { get; set; }
        public DbSet<SugerenciaCuidado> SugerenciasCuidado { get; set; }
        public DbSet<SignosVitales> SignosVitales { get; set; }
        public DbSet<Auditoria> Auditorias { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Aquí puedes configurar relaciones, restricciones, índices, etc. personalizados.
        }
    }

}
