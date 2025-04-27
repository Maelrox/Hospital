using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Hospital.Domain.Interfaces
{
    public interface IRepositorioGenerico<T> where T : class
    {
        Task<IEnumerable<T>> ObtenerTodosAsync();
        Task<IEnumerable<T>> ObtenerTodosAsync(params Expression<Func<T, object>>[] includes);
        Task<T> ObtenerPorIdAsync(object id);
        Task CrearAsync(T entidad);
        Task ActualizarAsync(T entidad);
        Task EliminarAsync(object id);
        Task<T> ObtenerPorCampoAsync(Expression<Func<T, bool>> predicado, params Expression<Func<T, object>>[] includes);
    }
} 