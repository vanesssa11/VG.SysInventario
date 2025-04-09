using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using VG.SysInventario.BL;
using VG.SysInventario.EN;
using static VG.SysInventario.EN.Venta;
using OfficeOpenXml;
using Rotativa.AspNetCore;
using VG.SysInventario.EN.Filtros;
using static VG.SysInventario.EN.Filtros.VentaFiltro;

namespace VG.SysInventario.AppWeb.Controllers
{
    public class VentaController : Controller
    {
        readonly ClienteBL clienteBL;
        readonly VentaBL ventaBL;
        readonly ProductoBL productoBL;

        public VentaController(ClienteBL pClienteBL, VentaBL pVentaBL, ProductoBL pProductoBL)
        {
            clienteBL = pClienteBL;
            ventaBL = pVentaBL;
            productoBL = pProductoBL;
        }

        // GET: VentaController
        public async Task<IActionResult> Index(byte? estado)
        {
            var venta = await ventaBL.ObtenerPorEstadoAsync(estado ?? 0);

            var estados = new List<SelectListItem>
            {
                new SelectListItem { Value = "", Text = "Todos" },
                new SelectListItem { Value = "1", Text = "Activa" },
                new SelectListItem { Value = "2", Text = "Anulada" }
            };
            ViewBag.Estado = new SelectList(estados, "Value", "Text", estado?.ToString());
            return View(venta);
        }

        // GET: VentaController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: VentaController/Create
        public async Task<IActionResult> Create()
        {
            ViewBag.Clientes = new SelectList(await clienteBL.ObtenerTodosAsync(), "Id", "Nombre");
            ViewBag.Productos = await productoBL.ObtenerTodosAsync();

            return View();
        }

        // POST: VentaController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(Venta venta)
        {
            try
            {
                venta.Estado = (byte)EnumEstadoVenta.Activa;
                venta.FechaVenta = DateTime.Now;
                await ventaBL.CrearAsync(venta);

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: VentaController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: VentaController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: VentaController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: VentaController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        public async Task<IActionResult> Anular(int Id)
        {
            var venta = await ventaBL.ObtenerPorIdAsync(Id);
            if (venta == null)
            {
                return NotFound();
            }
            await ventaBL.AnularAsync(Id);
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> ReporteVentasExcel(List<Venta> venta)
        {
            using (var package = new ExcelPackage())
            {
                var hojaExcel = package.Workbook.Worksheets.Add("Reporte Ventas");

                // Emcabezados
                hojaExcel.Cells["A1"].Value = "Fecha de Venta";
                hojaExcel.Cells["B1"].Value = "Cliente";
                hojaExcel.Cells["C1"].Value = "Producto";
                hojaExcel.Cells["D1"].Value = "Cantidad";
                hojaExcel.Cells["E1"].Value = "Subtotal";
                hojaExcel.Cells["F1"].Value = "Total de la Venta";

                int row = 2;
                int totalCantidad = 0;
                decimal totalSubTotal = 0;
                decimal totalGeneral = 0;

                foreach (var ventas in venta)
                {
                    foreach (var detalle in ventas.DetalleVentas)
                    {
                        hojaExcel.Cells[row, 1].Value = ventas.FechaVenta.ToString("yyyy-MM-dd");
                        hojaExcel.Cells[row, 2].Value = ventas.Cliente?.Nombre ?? "N/A";
                        hojaExcel.Cells[row, 3].Value = detalle.productos?.Nombre ?? "N/A";
                        hojaExcel.Cells[row, 4].Value = detalle.Cantidad;
                        hojaExcel.Cells[row, 5].Value = detalle.SubTotal;
                        hojaExcel.Cells[row, 6].Value = ventas.Total;

                        // Acumular totales
                        totalCantidad += detalle.Cantidad;
                        totalSubTotal += detalle.SubTotal;
                        totalGeneral += ventas.Total;

                        row++;
                    }
                }

                // Fila de totales
                hojaExcel.Cells[row, 3].Value = "Totales:";
                hojaExcel.Cells[row, 4].Value = totalCantidad;
                hojaExcel.Cells[row, 5].Value = totalSubTotal;
                hojaExcel.Cells[row, 6].Value = totalGeneral;

                // Negrita para la fila de totales
                hojaExcel.Cells[row, 3, row, 6].Style.Font.Bold = true;

                hojaExcel.Cells["A:F"].AutoFitColumns();

                var stream = new MemoryStream();
                package.SaveAs(stream);
                stream.Position = 0;

                return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "ReporteVentasExcel.xlsx");
            }
        }

        [HttpGet]
        public async Task<IActionResult> DescargarReporte(VentaFiltro filtro)
        {
            var venta = await ventaBL.ObtenerReporteVentasAsync(filtro);

            if (filtro.TipoReporte == (byte)EnumTipoReporte.PDF)
            {
                return new ViewAsPdf("rpVentas", venta);
            }
            else if (filtro.TipoReporte == (byte)EnumTipoReporte.Excel)
            {
                return await ReporteVentasExcel(venta);
            }

            return BadRequest("Formate no Valido");
        }

        [HttpGet]
        public IActionResult ReporteCompras()
        {
            return View();
        }
    }
}
