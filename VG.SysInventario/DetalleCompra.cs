using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VG.SysInventario.EN
{
    public class DetalleCompra
    {
        [Key]
        public int Id { get; set; }
        public int IdCompra { get; set; }
        [Required(ErrorMessage = "El producto es obligatorio.")]
        [ForeignKey("Producto")]

        public int IdProducto { get; set; }

        [Required(ErrorMessage = "La cantidad es obligatoria.")]
        [Range(1, int.MaxValue, ErrorMessage = "La cantidad debe ser al menos 1.")]

        public int Cantidad { get; set; }

        [Required(ErrorMessage = "El precio unitario es obligatorio.")]
        [Column(TypeName = "decimal(10,2)")]

        public decimal PrecioUnitario { get; set; }

        [Required(ErrorMessage = "El subTotal es obligatorio")]
        [Range(0.01, 99999999.99, ErrorMessage = "El subTotal debe ser mayor a 0.")]

        [Column(TypeName = "decimal(10,2)")]

        public decimal SubTotal {  get; set; }

        //Relacion con compra (cada detalle pertenece a una compra)
        public virtual Compra? Compra { get; set; }

        //Relacion con producto (Cada detalle esta asociado a un producto)
        public virtual Producto? Producto { get; set; }
    }
}