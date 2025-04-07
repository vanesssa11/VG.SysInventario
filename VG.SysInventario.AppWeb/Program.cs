using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using VG.SysInventario.BL;
using VG.SysInventario.DAL;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<SysInventarioDBContext>( options =>
{
    var conexionString = builder.Configuration.GetConnectionString("Conn");

    options.UseMySql(conexionString,ServerVersion.AutoDetect(conexionString));
});
builder.Services.AddScoped<ProductoDAL>();
builder.Services.AddScoped<ProductoBL>();

builder.Services.AddScoped<ProveedorDAL>();
builder.Services.AddScoped<ProveedorBL>();

builder.Services.AddScoped<ClienteDAL>();
builder.Services.AddScoped<ClienteBL>();

builder.Services.AddScoped<CompraDAL>();
builder.Services.AddScoped<CompraBL>();



// Add services to the container.
builder.Services.AddControllersWithViews();

ExcelPackage.LicenseContext = LicenseContext.NonCommercial;


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

IWebHostEnvironment env = app.Environment;
Rotativa.AspNetCore.RotativaConfiguration.
    Setup(env.WebRootPath, "../wwwroot/Rotativa");

app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
