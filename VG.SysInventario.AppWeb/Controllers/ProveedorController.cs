using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using Rotativa.AspNetCore;
using VG.SysInventario.BL;
using VG.SysInventario.EN;

namespace VG.SysInventario.AppWeb.Controllers
{
    public class ProveedorController : Controller
    {
        readonly ProveedorBL _proveedorBL;
        public ProveedorController(ProveedorBL pProveedorBL)
        {
            _proveedorBL = pProveedorBL;
        }

        // GET: ProveedorController
        public async Task<ActionResult> Index()
        {
            var proveedores = await _proveedorBL.ObtenerTodosAsync();
            return View(proveedores);
        }

        // GET: ProveedorController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: ProveedorController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: ProveedorController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(Proveedor pProveedor)
        {
            try
            {
                await _proveedorBL.CrearAsync(pProveedor);
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: ProveedorController/Edit/5
        public async Task<ActionResult> Edit(int id)
        {
            var proveedor = await _proveedorBL.ObtenerPorIdAsync(new Proveedor { Id = id });
            return View(proveedor);
        }

        // POST: ProveedorController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(Proveedor pProveedor)
        {
            try
            {
                var result = await _proveedorBL.ModificarAsync(pProveedor);
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: ProveedorController/Delete/5
        public async Task<ActionResult> Delete(int id)
        {
            var proveedor = await _proveedorBL.ObtenerPorIdAsync(new Proveedor { Id = id });
            return View(proveedor);
        }

        // POST: ProveedorController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteProveedor(int id)
        {
            try
            {
                var resul = await _proveedorBL.EliminarAsync(new Proveedor { Id = id });
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
        public async Task<ActionResult> ReporteProveedor()
        {
            var proveedor = await _proveedorBL.ObtenerTodosAsync();
            return new ViewAsPdf("rpProveedor", proveedor);
        }
      
        public async Task<IActionResult> ReporteProveedoresExcel()
        {
            var proveedores = await _proveedorBL.ObtenerTodosAsync();
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using (var packge = new ExcelPackage())
            {

                var hojaExcel = packge.Workbook.Worksheets.Add("Proveedor");

                hojaExcel.Cells["A1"].Value = "Nombre";
                hojaExcel.Cells["B1"].Value = "NRC";
                hojaExcel.Cells["C1"].Value = "Direccion";
                hojaExcel.Cells["D1"].Value = "Telefono";
                hojaExcel.Cells["E1"].Value = "Email";

                int row = 2;

                foreach (var proveedor in proveedores)
                {
                    hojaExcel.Cells[row, 1].Value = proveedor.Nombre;
                    hojaExcel.Cells[row, 2].Value = proveedor.NRC;
                    hojaExcel.Cells[row, 3].Value = proveedor.Direccion;
                    hojaExcel.Cells[row, 4].Value = proveedor.Telefono;
                    hojaExcel.Cells[row, 5].Value = proveedor.Email;
                    row++;
                }
                hojaExcel.Cells["A:E"].AutoFitColumns();

                var stream = new MemoryStream();
                packge.SaveAs(stream);
                stream.Position = 0;

                return File(stream, "application/vnd.openxmlFormarts-officedocument.spreadsheet.sheet", "ReporteProveedoresExcel.xlsx");
            }
        }
        public async Task<IActionResult> SubirExcelProveedores(IFormFile archivoExcel)
        {
            if (archivoExcel == null || archivoExcel.Length == 0)
            {
                return RedirectToAction("Index");
            }
            var proveedores = new List<Proveedor>();

            using (var stream = new MemoryStream())
            {
                await archivoExcel.CopyToAsync(stream);
                using (var package = new ExcelPackage())
                {
                    var hojaExcel = package.Workbook.Worksheets[0];
                    int rowCount = hojaExcel.Dimension.Rows;

                    for (int row = 2; row <= rowCount; row++)
                    {
                        var nombre = hojaExcel.Cells[row, 1].Text;
                        var nrc = hojaExcel.Cells[row, 2].Text;
                        var direccion = hojaExcel.Cells[row, 3].Text;
                        var telefono = hojaExcel.Cells[row, 4].Text;
                        var email = hojaExcel.Cells[row, 5].Text;

                        if (string.IsNullOrEmpty(nombre) || string.IsNullOrEmpty(nrc) || string.IsNullOrEmpty(direccion) ||
                            string.IsNullOrEmpty(telefono) || string.IsNullOrEmpty(email))
                            continue;
                        proveedores.Add(new Proveedor
                        {
                            Nombre = nombre,
                            NRC = nrc,
                            Direccion = direccion,
                            Telefono = telefono,
                            Email = email
                        });
                    }
                }
                if (proveedores.Count > 0)
                    {
                    await _proveedorBL.AgregarTodosAsync(proveedores);
                    }
                return RedirectToAction("Index");
                }
            }

        }
    }