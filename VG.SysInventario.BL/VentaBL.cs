using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VG.SysInventario.DAL;
using VG.SysInventario.EN;
using VG.SysInventario.EN.Filtros;

namespace VG.SysInventario.BL
{
    public class VentaBL
    {
        readonly VentaDAL ventaDAL;

        public VentaBL(VentaDAL pVentaDAL)
        {
            ventaDAL = pVentaDAL;
        }

        public async Task<int> CrearAsync(Venta pVenta)
        {
            return await ventaDAL.CrearAsync(pVenta);
        }

        public async Task<int> AnularAsync(int IdVenta)
        {
            return await ventaDAL.AnularAsync(IdVenta);
        }

        public async Task<Venta> ObtenerPorIdAsync(int IdVenta)
        {
            return await ventaDAL.ObtenerPorIdAsync(IdVenta);
        }

        public async Task<List<Venta>> ObtenerTodosAsync()
        {
            return await ventaDAL.ObtenerTodosAsync();
        }

        public async Task<List<Venta>> ObtenerPorEstadoAsync(byte estado)
        {
            return await ventaDAL.ObtenerPorEstadoAsync(estado);
        }

        public async Task<List<Venta>> ObtenerReporteVentasAsync(VentaFiltro filtro)
        {
            return await ventaDAL.ObtenerReporteVentasAsync(filtro);
        }
    }
}
