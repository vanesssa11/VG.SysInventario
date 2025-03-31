using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using Rotativa.AspNetCore;
using VG.SysInventario.BL;
using VG.SysInventario.EN;

namespace VG.SysInventario.AppWeb.Controllers
{
    public class ProductoController : Controller
    {
        private readonly ProductoBL _productoBL;
        public ProductoController (ProductoBL pProductoBL)
        {
            _productoBL = pProductoBL;
        }

        // GET: ProductoController
        public async Task<ActionResult> Index()
        {
            var productos = await _productoBL.ObtenerTodosAsync();
            return View(productos);
        }

        // GET: ProductoController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: ProductoController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: ProductoController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(Producto pProducto)
        {
            try
            {
                await _productoBL.CrearAsync(pProducto);
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: ProductoController/Edit/5
        public async Task<ActionResult> Edit(int id)
        {
            var producto = await _productoBL.ObtenerPorIdAsync(new Producto { Id = id });
            return View(producto);
        }

        // POST: ProductoController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(Producto producto)
        {
            try
            {
                await _productoBL.ModificarAsync(producto);    
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: ProductoController/Delete/5
        public async Task<ActionResult> Delete(int id)
        {
            var producto = await _productoBL.ObtenerPorIdAsync(new Producto {   Id = id });
            return View(producto);
        }

        // POST: ProductoController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(Producto pProducto)
        {
            try
            {
                await _productoBL.EliminarAsync(new Producto { Id = pProducto.Id});
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            } 
        }
        public async Task<ActionResult> ReporteProductos()
        {
            var productos = await _productoBL.ObtenerTodosAsync();
            return new ViewAsPdf("rpProducto",productos);
        }

        public async Task<JsonResult> ProductosJson()
        {
            var productos = await _productoBL.ObtenerTodosAsync();

            var productosData = productos
                .Select(p => new
                { 
                    nombre = p.Nombre,
                    stock = p.CantidadDisponible
                })
                .ToList();
            return Json(productosData);
        }
        public async Task<JsonResult> ProductosJsonPrecio()
        {
            var productos = await _productoBL.ObtenerTodosAsync();
            var productosData = productos
                .Select(p => new
                {
                    fechaCreacion = p.FechaCreación.ToString("yyyy - MM - dd"),
                    precio = p.Precio
                })
                .ToList();

            var groupedData = productosData
                .GroupBy(p => p.fechaCreacion)
                .Select(g => new
                {
                    fecha = g.Key,
                    precioPromedio = g.Average(p => p.precio) //calcular precio promedio
                })
                .OrderBy(g => g.fecha)
                .ToList();
            return Json(groupedData);
        }
        public async Task<IActionResult> ReporteProductosExcel()
        {
            var productos = await _productoBL.ObtenerTodosAsync();
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using (var packge = new ExcelPackage())
            {

                var hojaExcel = packge.Workbook.Worksheets.Add("Productos");

                hojaExcel.Cells["A1"].Value = "Nombre";
                hojaExcel.Cells["B1"].Value = "Precio";
                hojaExcel.Cells["C1"].Value = "Cantidad Disponible";
                hojaExcel.Cells["D1"].Value = "Fecha Creacion";

                int row = 2;

                foreach (var producto in productos)
                {
                    hojaExcel.Cells[row, 1].Value = producto.Nombre;
                    hojaExcel.Cells[row, 2].Value = producto.Precio;
                    hojaExcel.Cells[row, 3].Value = producto.CantidadDisponible;
                    hojaExcel.Cells[row, 4].Value = producto.FechaCreación.ToString("yyyy-MM-dd");
                    row++;
                }
                hojaExcel.Cells["A:D"].AutoFitColumns();

                var stream  =  new MemoryStream();
                packge.SaveAs(stream);
                stream.Position = 0;

                return File(stream, "application/vnd.openxmlFormarts-officedocument.spreadsheet.sheet","ReporteProductosExcel.xlsx");
            }
        }
        public async Task<IActionResult> SubirExcelProductos(IFormFile archivoExcel)
        {
            if (archivoExcel == null || archivoExcel.Length == 0)
            {
                return RedirectToAction("Index");
            }
            var productos = new List<Producto>();

            using (var stream = new MemoryStream())
            {
                await archivoExcel.CopyToAsync(stream);
                using (var packge = new ExcelPackage())
                {
                    var hojaExcel = packge.Workbook.Worksheets[0];

                    int rowCount = hojaExcel.Dimension.Rows;

                    for (int row = 2; row <= rowCount; row++)
                    {
                        var nombre = hojaExcel.Cells[row, 1].Text;
                        var precio = hojaExcel.Cells[row, 2].GetValue<decimal>();
                        var cantidad = hojaExcel.Cells[row, 3].GetValue<int>();
                        var fecha = hojaExcel.Cells[row, 4].GetValue<DateTime>();

                        if (String.IsNullOrWhiteSpace(nombre) || precio <= 0 || cantidad < 0)
                                continue;
                        productos.Add(new Producto
                        {
                            Nombre = nombre,
                            Precio = precio,
                            CantidadDisponible = cantidad,
                            FechaCreación = fecha
                        });
                    }
                }
                if (productos.Count > 0)
                {
                    await _productoBL.AgregarTodosAsync(productos);
                }
                return RedirectToAction("Index");
            }
            
        }
    }
}

