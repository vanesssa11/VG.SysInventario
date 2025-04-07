using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VG.SysInventario.EN
{
    public class Cliente
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre del cliente es obligatorio.")]
        [StringLength(255, ErrorMessage = "El nombre no puede tener mas de 255 caracteres.")]
        public string Nombre { get; set; }

        [StringLength(50, ErrorMessage = "El DUI no puede tener mas de 50 caracteres.")]
        public string? DUI { get; set; }

        [StringLength(80, ErrorMessage = "La direccion no puede tener mas de 80 caracteres.")]
        public string? Dirección { get; set; }

        [StringLength(20, ErrorMessage = "El telefono no puede tener mas de 20 caracteres.")]
        public string? Telefono { get; set; }
    }
}

