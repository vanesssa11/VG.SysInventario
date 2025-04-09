using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VG.SysInventario.EN;
using VG.SysInventario.EN.Filtros;

namespace VG.SysInventario.DAL
{
    public class VentaDAL
    {
        readonly SysInventarioDBContext dbContext;

        public VentaDAL(SysInventarioDBContext sysInventarioDB)
        {
            dbContext = sysInventarioDB;
        }

        public async Task<int> CrearAsync(Venta pVenta)
        {
            // Agregar la venta con sus detalles 
            dbContext.ventas.Add(pVenta);
            int result = await dbContext.SaveChangesAsync();
            if (result > 0)
            {
                // Actualizar Stock de productos
                foreach (var detalle in pVenta.DetalleVentas)
                {
                    var producto = await dbContext.productos.FirstOrDefaultAsync(p => p.Id == detalle.IdProducto);
                    if (producto != null)
                    {
                        producto.CantidadDisponible += detalle.Cantidad;
                    }
                }
            }
            return await dbContext.SaveChangesAsync();
        }

        public async Task<int> AnularAsync(int IdVenta)
        {
            var venta = await dbContext.ventas
                .Include(c => c.DetalleVentas)
                .FirstOrDefaultAsync(c => c.Id == IdVenta);

            if (venta != null & venta.Estado != (byte)Venta.EnumEstadoVenta.Anulada)
            {
                // Marcar la venta como anulada 
                venta.Estado = (byte)Venta.EnumEstadoVenta.Anulada;

                // Restar la cantidad de los productos vendidos
                foreach (var detalle in venta.DetalleVentas)
                {
                    var producto = await dbContext.productos.FirstOrDefaultAsync(p => p.Id == detalle.IdProducto);
                    if (producto != null)
                    {
                        producto.CantidadDisponible -= detalle.Cantidad;
                    }
                }
                return await dbContext.SaveChangesAsync();
            }

            return 0; // Si ya estaba anulada, no hacer nada
        }

        public async Task<Venta> ObtenerPorIdAsync(int IdVenta)
        {
            var venta = await dbContext.ventas
                .Include(c => c.DetalleVentas)
                .Include(c => c.Cliente)
                .FirstOrDefaultAsync(c => c.Id == IdVenta);

            return venta ?? new Venta();
        }

        public async Task<List<Venta>> ObtenerTodosAsync()
        {
            var venta = await dbContext.ventas
                .Include(c => c.DetalleVentas)
                .Include(c => c.Cliente)
                .ToListAsync();

            return venta ?? new List<Venta>();
        }

        public async Task<List<Venta>> ObtenerPorEstadoAsync(byte estado)
        {
            var ventaQuery = dbContext.ventas.AsQueryable();

            if (estado != 0)
            {
                ventaQuery = ventaQuery.Where(c => c.Estado == estado);
            }

            ventaQuery = ventaQuery
                .Include(c => c.DetalleVentas)
                .Include(c => c.Cliente);

            var venta = await ventaQuery.ToListAsync();
            return venta ?? new List<Venta>();
        }

        public async Task<List<Venta>> ObtenerReporteVentasAsync(VentaFiltro filtro)
        {
            var ventaQuery = dbContext.ventas
                .Include(c => c.DetalleVentas)
                .ThenInclude(dc => dc.productos)
                .Include(c => c.Cliente)
                .AsQueryable();

            if (filtro.FechaInicio.HasValue)
            {
                DateTime fechaInicio = filtro.FechaInicio.Value.Date; // Eliminar la hora, dejar solo la fecha
                ventaQuery = ventaQuery.Where(c => c.FechaVenta >= fechaInicio);
            }

            if (filtro.FechaFin.HasValue)
            {
                DateTime fechaFin = filtro.FechaFin.Value.Date.AddDays(1).AddSeconds(-1); // Incluir hasta el final del día
                ventaQuery = ventaQuery.Where(c => c.FechaVenta <= fechaFin);
            }

            return await ventaQuery.ToListAsync();
        }
    }
}
