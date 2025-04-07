using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VG.SysInventario.EN;

namespace VG.SysInventario.DAL
{
    public class ClienteDAL
    {
        readonly SysInventarioDBContext dbContext;
        public ClienteDAL(SysInventarioDBContext sysProductosDB)
        {
            dbContext = sysProductosDB;
        }
        public async Task<int> CrearAsync(Cliente pCliente)
        {
            Cliente cliente = new Cliente()
            {
                Nombre = pCliente.Nombre,
                DUI = pCliente.DUI,
                Dirección = pCliente.Dirección,
                Telefono = pCliente.Telefono
            };
            dbContext.clientes.Add(cliente);
            return await dbContext.SaveChangesAsync();
        }
        public async Task<int> EliminarAsync(Cliente pCliente)
        {
            var cliente = await dbContext.clientes.FirstOrDefaultAsync(s => s.Id == pCliente.Id);
            if (cliente != null && cliente.Id != 0)
            {
                dbContext.clientes.Remove(cliente);
                return await dbContext.SaveChangesAsync();
            }
            else
                return 0;
        }
        public async Task<int> ModificarAsync(Cliente pCliente)
        {
            var cliente = await dbContext.clientes.FirstOrDefaultAsync(s => s.Id == pCliente.Id);
            if (cliente != null && cliente.Id != 0)
            {
                cliente.Nombre = pCliente.Nombre;
                cliente.DUI = pCliente.DUI;
                cliente.Dirección = pCliente.Dirección;
                cliente.Telefono = pCliente.Telefono;

                dbContext.Update(cliente);
                return await dbContext.SaveChangesAsync();
            }
            else
                return 0;
        }
        public async Task<Cliente> ObtenerPorIdAsync(Cliente pCliente)
        {
            var cliente = await dbContext.clientes.FirstOrDefaultAsync(s => s.Id == pCliente.Id);
            if (cliente != null && cliente.Id != 0)
            {
                return new Cliente
                {
                    Id = cliente.Id,
                    Nombre = cliente.Nombre,
                    DUI = cliente.DUI,
                    Dirección = cliente.Dirección,
                    Telefono = cliente.Telefono
                };
            }
            else
                return new Cliente();
        }
        public async Task<List<Cliente>> ObtenerTodosAsync()
        {
            var clientes = await dbContext.clientes.ToListAsync();
            if (clientes != null && clientes.Count > 0)
            {
                var list = new List<Cliente>();
                clientes.ForEach(p => list.Add(new Cliente
                {
                    Id = p.Id,
                    Nombre = p.Nombre,
                    DUI = p.DUI,
                    Dirección = p.Dirección,
                    Telefono = p.Telefono
                }));
                return list;
            }
            else
                return new List<Cliente>();
        }
        public async Task AgregarTodosAsync(List<Cliente> pCliente)
        {
            await dbContext.clientes.AddRangeAsync(pCliente);
            await dbContext.SaveChangesAsync();
        }

    }
}
