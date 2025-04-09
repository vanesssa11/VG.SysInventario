using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using VG.SysInventario.EN;


namespace VG.SysInventario.DAL
{
    public class SysInventarioDBContext : DbContext
    {

        public SysInventarioDBContext(DbContextOptions<SysInventarioDBContext> options) : base(options)

        {

        }
        public DbSet<Producto> productos { get; set; }
     
        public DbSet<Proveedor> proveedores { get; set; }

        public DbSet<Cliente> clientes { get; set; }

        public DbSet<Compra> Compras { get; set; }
        public DbSet<Venta> ventas { get; set; }
        public DbSet<DetalleVenta> detalleventa { get; set; }
        public DbSet<DetalleCompra> DetalleCompras { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DetalleCompra>()
                .HasOne(d => d.Compra)
                .WithMany(c => c.DetalleCompras)
                .HasForeignKey(d => d.IdCompra);
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<DetalleVenta>()
                .HasOne(d => d.venta)
                .WithMany(c => c.DetalleVentas)
                .HasForeignKey(d => d.IdVenta);
            base.OnModelCreating(modelBuilder);
        }

    }
}