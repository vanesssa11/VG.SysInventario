using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VG.SysInventario.EN
{
    public class DetalleVenta
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("venta")]
        public int IdVenta { get; set; }

        [Required(ErrorMessage = "El producto es obligatorio.")]
        [ForeignKey("productos")]
        public int IdProducto { get; set; }

        [Required(ErrorMessage = "La cantidad es obligatoria.")]
        [Range(1, int.MaxValue, ErrorMessage = "La cantidad debe ser al menos 1.")]
        public int Cantidad { get; set; }

        [Required(ErrorMessage = "El subtotal es obligatorio.")]
        [Range(0.01, 999999.99, ErrorMessage = "El subtotal debe ser mayor a 0.")]
        [Column(TypeName = "decimal(10,2)")]
        public decimal SubTotal { get; set; }

        [Required(ErrorMessage = "El total es obligatorio.")]
        [Range(0.01, 999999.99, ErrorMessage = "El total debe ser mayor a 0.")]
        [Column(TypeName = "decimal(10,2)")]
        public decimal TotalVenta { get; set; }

        // Navegación
        public virtual Venta? venta { get; set; }
        public virtual Producto? productos { get; set; }
    }
}
