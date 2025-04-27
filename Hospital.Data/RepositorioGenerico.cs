using Hospital.Data;
using Hospital.Domain.Interfaces;
using Hospital.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace Hospital.Domain
{
    public class RepositorioGenerico<T> : IRepositorioGenerico<T> where T : class
    {
        private readonly HospitalDbContext _context;
        private readonly DbSet<T> _dbSet;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public RepositorioGenerico(HospitalDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _dbSet = context.Set<T>();
            _httpContextAccessor = httpContextAccessor;
        }

        private async Task RegistrarAuditoria(string tipoOperacion, string descripcion)
        {
            var userIdClaim = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier);
            int? userId = userIdClaim != null ? int.Parse(userIdClaim.Value) : null;

            var auditoria = new Auditoria
            {
                IdUsuario = userId,
                TipoOperacion = tipoOperacion,
                FechaHora = DateTime.Now,
                TablaAfectada = typeof(T).Name,
                DescripcionCambio = descripcion,
                IpAcceso = _httpContextAccessor.HttpContext?.Connection?.RemoteIpAddress?.ToString() ?? "0.0.0.0"
            };

            await _context.Auditorias.AddAsync(auditoria);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<T>> ObtenerTodosAsync()
        {
            return await _dbSet.ToListAsync();
        }

        public async Task<T> ObtenerPorIdAsync(object id)
        {
            return await _dbSet.FindAsync(id);
        }

        public async Task CrearAsync(T entidad)
        {
            await _dbSet.AddAsync(entidad);
            await _context.SaveChangesAsync();
            await RegistrarAuditoria("INSERT", $"Creación de nuevo registro en {typeof(T).Name}");
        }

        public async Task ActualizarAsync(T entidad)
        {
            _dbSet.Update(entidad);
            await _context.SaveChangesAsync();
            await RegistrarAuditoria("UPDATE", $"Actualización de registro en {typeof(T).Name}");
        }

        public async Task EliminarAsync(object id)
        {
            var entidad = await _dbSet.FindAsync(id);
            if (entidad != null)
            {
                _dbSet.Remove(entidad);
                await _context.SaveChangesAsync();
                await RegistrarAuditoria("DELETE", $"Eliminación de registro en {typeof(T).Name}");
            }
        }

        public async Task<IEnumerable<T>> ObtenerTodosAsync(params Expression<Func<T, object>>[] includes)
        {
            IQueryable<T> query = _dbSet;

            foreach (var include in includes)
            {
                query = query.Include(include);
            }

            return await query.ToListAsync();
        }
    }
}
