using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
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

        public DbSet<Compra> Compras { get; set; }
        public DbSet<DetalleCompra> DetalleCompras { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DetalleCompra>()
                .HasOne(d => d.Compra)
                .WithMany(c => c.DetalleCompras)
                .HasForeignKey(d => d.IdCompra);


            base.OnModelCreating(modelBuilder);
        }

    }
}