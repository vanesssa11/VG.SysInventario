using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VG.SysInventario.EN
{
    public class Venta
    {
        [Key]
        public int Id { get; set; }
        [Required(ErrorMessage = "La fecha de la venta es obligatoria.")]
        public DateTime FechaVenta { get; set; }

        [Required(ErrorMessage = "El cliente es obligatorio.")]
        [ForeignKey("Cliente")]
        public int IdCliente { get; set; }

        [Required(ErrorMessage = "El total de la venta es obligatorio.")]
        [Range(0.01, 999999.99, ErrorMessage = "El total debe ser mayor a 0 y menor a 1,000,000.")]
        [Column(TypeName = "decimal(10,2)")]
        public decimal Total { get; set; }
        public byte Estado { get; set; }

        //Relacion con el cliente
        public virtual Cliente? Cliente { get; set; }
        public virtual ICollection<DetalleVenta> DetalleVentas { get; set; } = new List<DetalleVenta>();

        public enum EnumEstadoVenta
        {
            Activa = 1,
            Anulada = 2
        }
    }
}
