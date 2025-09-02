using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Facturacion.Models.Data;
using Facturacion.Models.ViewModels;
using System.Globalization;

namespace Facturacion.Controllers
{
    public class EstadisticasController : Controller
    {
        private readonly FacturacionDbContext _context;

        public EstadisticasController(FacturacionDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(int? año)
        {
            var añoSeleccionado = año ?? DateTime.Now.Year;
            
            // Obtener años disponibles con facturas
            var añosDisponibles = await _context.Facturas
                .Select(f => f.Fecha.Year)
                .Distinct()
                .OrderByDescending(a => a)
                .ToListAsync();

            if (!añosDisponibles.Contains(añoSeleccionado))
            {
                añoSeleccionado = añosDisponibles.FirstOrDefault();
            }

            var estadisticas = new EstadisticasViewModel
            {
                Año = añoSeleccionado,
                AñosDisponibles = añosDisponibles
            };

            if (añoSeleccionado > 0)
            {
                // Obtener facturas del año seleccionado
                var facturas = await _context.Facturas
                    .Where(f => f.Fecha.Year == añoSeleccionado)
                    .ToListAsync();

                // Agrupar por mes y calcular estadísticas
                var estadisticasMensuales = facturas
                    .GroupBy(f => f.Fecha.Month)
                    .Select(g => new EstadisticasMes
                    {
                        Mes = g.Key,
                        NombreMes = CultureInfo.GetCultureInfo("es-ES").DateTimeFormat.GetMonthName(g.Key),
                        TotalFacturado = g.Sum(f => f.Total),
                        CantidadFacturas = g.Count(),
                        PromedioFactura = g.Average(f => f.Total)
                    })
                    .OrderBy(e => e.Mes)
                    .ToList();

                // Completar meses sin facturas
                for (int mes = 1; mes <= 12; mes++)
                {
                    if (!estadisticasMensuales.Any(e => e.Mes == mes))
                    {
                        estadisticasMensuales.Add(new EstadisticasMes
                        {
                            Mes = mes,
                            NombreMes = CultureInfo.GetCultureInfo("es-ES").DateTimeFormat.GetMonthName(mes),
                            TotalFacturado = 0,
                            CantidadFacturas = 0,
                            PromedioFactura = 0
                        });
                    }
                }

                estadisticas.FacturacionMensual = estadisticasMensuales.OrderBy(e => e.Mes).ToList();
                estadisticas.TotalAño = estadisticas.FacturacionMensual.Sum(e => e.TotalFacturado);
                estadisticas.TotalFacturas = estadisticas.FacturacionMensual.Sum(e => e.CantidadFacturas);
                estadisticas.PromedioMensual = estadisticas.TotalFacturas > 0 ? estadisticas.TotalAño / 12 : 0;

                var mesMayorFacturacion = estadisticas.FacturacionMensual.OrderByDescending(e => e.TotalFacturado).First();
                estadisticas.MesMayorFacturacion = mesMayorFacturacion.NombreMes;
                estadisticas.MayorFacturacionMes = mesMayorFacturacion.TotalFacturado;
            }

            return View(estadisticas);
        }

        public async Task<IActionResult> Resumen()
        {
            var totalFacturas = await _context.Facturas.CountAsync();
            var totalFacturado = totalFacturas > 0 ? await _context.Facturas.SumAsync(f => f.Total) : 0;
            var facturaPromedio = totalFacturas > 0 ? await _context.Facturas.AverageAsync(f => f.Total) : 0;
            
            var resumen = new DashboardViewModel
            {
                TotalFacturas = totalFacturas,
                TotalFacturado = totalFacturado,
                TotalClientes = await _context.Clientes.CountAsync(),
                TotalProductos = await _context.Productos.CountAsync(),
                FacturaPromedio = facturaPromedio,
                UltimasFacturas = await _context.Facturas
                    .Include(f => f.Cliente)
                    .OrderByDescending(f => f.Fecha)
                    .Take(5)
                    .ToListAsync()
            };

            return View(resumen);
        }
    }
}