using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using VG.SysInventario.EN;


namespace VG.SysInventario.DAL
{
    public class ProductoDAL
    {
        readonly SysInventarioDBContext dbContext;

        public ProductoDAL(SysInventarioDBContext Context)
        {
            dbContext = Context;
        }
        public async Task<int> CrearAsync(Producto pProducto)
        {
            Producto producto = new Producto()
            {
                Nombre = pProducto.Nombre,
                Precio = pProducto.Precio,
                CantidadDisponible = pProducto.CantidadDisponible,
                FechaCreación = pProducto.FechaCreación

            };
            dbContext.Add(producto);
            return await dbContext.SaveChangesAsync();
        }
        public async Task<int> ModificarAsync(Producto pProducto)
        {
            var producto = await dbContext.productos.FirstOrDefaultAsync(s => s.Id == pProducto.Id);
            if (producto != null)
            {
                producto.Nombre = pProducto.Nombre;
                producto.Precio = pProducto.Precio;
                producto.CantidadDisponible = pProducto.CantidadDisponible;
                producto.FechaCreación = pProducto.FechaCreación;
                dbContext.productos.Update(producto);
                return await dbContext.SaveChangesAsync();
            }
            else
                return 0;
        }
        public async Task<int> EliminarAsync(Producto pProducto)
        {
            var producto = dbContext.productos.FirstOrDefault(s => s.Id == pProducto.Id);
            if (producto != null)
            {
                producto.Nombre = pProducto.Nombre;
                producto.Precio = pProducto.Precio;
                producto.CantidadDisponible = pProducto.CantidadDisponible;
                producto.FechaCreación = pProducto.FechaCreación;
                dbContext.productos.Remove(producto);
                return await dbContext.SaveChangesAsync();
            }
            else
                return 0;
        }
        public async Task<Producto> ObtenerPorIdAsync(Producto pProducto)
        {
            var producto = await dbContext.productos.FirstOrDefaultAsync(s => s.Id == pProducto.Id);
            if (producto != null && producto.Id != 0)
            {
                return new Producto
                {
                    Id = producto.Id,
                    Nombre = producto.Nombre,
                    Precio = producto.Precio,
                    CantidadDisponible = producto.CantidadDisponible,
                    FechaCreación = producto.FechaCreación
                };
            }
            else
                return new Producto();
        }
        public async Task<List<Producto>> ObtenerTodosAsync()
        {
            var productos = await dbContext.productos.ToListAsync();
            if (productos != null && productos.Count > 0)
            {
                var list = new List<Producto>();
                productos.ForEach(s => list.Add(new Producto
                {
                    Id = s.Id,
                    Nombre = s.Nombre,
                    Precio = s.Precio,
                    CantidadDisponible = s.CantidadDisponible,
                    FechaCreación = s.FechaCreación
                }));
                return list;
            }
            else
                return new List<Producto>();
        }
        public async Task AgregarTodosAsync(List<Producto> pProductos)
        {
            await dbContext.productos.AddRangeAsync(pProductos);
            await dbContext.SaveChangesAsync();
        }
    }
}
