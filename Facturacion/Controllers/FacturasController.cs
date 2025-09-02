using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Facturacion.Models.Data;
using Facturacion.Models.Entities;

namespace Facturacion.Controllers
{
    public class FacturasController : Controller
    {
        private readonly FacturacionDbContext _context;

        public FacturasController(FacturacionDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string searchTerm, string sortBy, string sortOrder, DateTime? fechaInicio, DateTime? fechaFin)
        {
            ViewBag.CurrentFilter = searchTerm;
            ViewBag.CurrentSort = sortBy;
            ViewBag.CurrentOrder = sortOrder;
            ViewBag.FechaInicio = fechaInicio?.ToString("yyyy-MM-dd");
            ViewBag.FechaFin = fechaFin?.ToString("yyyy-MM-dd");
            
            var facturas = _context.Facturas.Include(f => f.Cliente).AsQueryable();

            // Filtrado por rango de fechas
            if (fechaInicio.HasValue)
            {
                facturas = facturas.Where(f => f.Fecha >= fechaInicio.Value);
            }
            if (fechaFin.HasValue)
            {
                facturas = facturas.Where(f => f.Fecha <= fechaFin.Value);
            }

            // Filtrado por término de búsqueda
            if (!string.IsNullOrEmpty(searchTerm))
            {
                facturas = facturas.Where(f => 
                    f.NumeroFactura.Contains(searchTerm) ||
                    f.Cliente.Nombre.Contains(searchTerm) ||
                    f.Cliente.Apellido.Contains(searchTerm) ||
                    f.Cliente.Email.Contains(searchTerm));
            }

            // Ordenamiento
            switch (sortBy?.ToLower())
            {
                case "numero":
                    facturas = sortOrder == "desc" ? facturas.OrderByDescending(f => f.NumeroFactura) : facturas.OrderBy(f => f.NumeroFactura);
                    break;
                case "fecha":
                    facturas = sortOrder == "desc" ? facturas.OrderByDescending(f => f.Fecha) : facturas.OrderBy(f => f.Fecha);
                    break;
                case "cliente":
                    facturas = sortOrder == "desc" ? facturas.OrderByDescending(f => f.Cliente.Nombre) : facturas.OrderBy(f => f.Cliente.Nombre);
                    break;
                case "total":
                    facturas = sortOrder == "desc" ? facturas.OrderByDescending(f => f.Total) : facturas.OrderBy(f => f.Total);
                    break;
                default:
                    facturas = facturas.OrderByDescending(f => f.Fecha);
                    break;
            }

            return View(await facturas.ToListAsync());
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var factura = await _context.Facturas
                .Include(f => f.Cliente)
                .Include(f => f.FacturaDetalles)
                    .ThenInclude(fd => fd.Producto)
                        .ThenInclude(p => p.Categoria)
                .FirstOrDefaultAsync(m => m.Id == id);
            
            if (factura == null)
            {
                return NotFound();
            }

            return View(factura);
        }

        public IActionResult Create()
        {
            ViewData["ClienteId"] = new SelectList(_context.Clientes.Select(c => new { 
                Id = c.Id, 
                NombreCompleto = c.Nombre + " " + c.Apellido 
            }), "Id", "NombreCompleto");
            
            ViewBag.Productos = _context.Productos.Include(p => p.Categoria).ToList();
            
            var factura = new Factura
            {
                Fecha = DateTime.Now,
                NumeroFactura = GenerarNumeroFactura()
            };
            
            return View(factura);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Factura factura, List<int> ProductoIds, List<int> Cantidades, List<decimal> PreciosUnitarios)
        {
            try
            {
                // Debug: Log de los datos recibidos
                var debug = $"ProductoIds: {(ProductoIds != null ? string.Join(",", ProductoIds) : "null")} | " +
                           $"Cantidades: {(Cantidades != null ? string.Join(",", Cantidades) : "null")} | " +
                           $"Precios: {(PreciosUnitarios != null ? string.Join(",", PreciosUnitarios) : "null")} | " +
                           $"IVA: {factura.IVA} | Observaciones: '{factura.Observaciones}' | FormaPago: '{factura.FormaPago}'";
                
                Console.WriteLine($"DEBUG CREATE FACTURA: {debug}");

                // Validaciones básicas
                if (ProductoIds == null || ProductoIds.Count == 0 || ProductoIds.All(p => p == 0))
                {
                    ModelState.AddModelError("", "Debe seleccionar al menos un producto válido.");
                }

                if (Cantidades == null || Cantidades.Count == 0 || Cantidades.All(c => c <= 0))
                {
                    ModelState.AddModelError("", "Debe ingresar cantidades válidas.");
                }

                if (PreciosUnitarios == null || PreciosUnitarios.Count == 0 || PreciosUnitarios.All(p => p <= 0))
                {
                    ModelState.AddModelError("", "Los precios unitarios deben ser válidos.");
                }

                // Remover validaciones de campos que se manejan manualmente
                ModelState.Remove("FacturaDetalles");
                ModelState.Remove("Subtotal");
                ModelState.Remove("Total");
                ModelState.Remove("IRPF");
                ModelState.Remove("IVA");
                ModelState.Remove("Observaciones");
                ModelState.Remove("FormaPago");
                ModelState.Remove("Cliente"); // Remover validación de la entidad Cliente
                
                // Remover cualquier validación de propiedades de navegación
                var keysToRemove = ModelState.Keys.Where(k => 
                    k.Contains("Cliente.") || 
                    k.Contains("Producto.") || 
                    k.Contains("Categoria.") ||
                    k.Contains("FacturaDetalles.")
                ).ToList();
                
                foreach (var key in keysToRemove)
                {
                    ModelState.Remove(key);
                }
                
                // Si el número de factura está vacío, no validarlo ahora
                if (string.IsNullOrEmpty(factura.NumeroFactura))
                {
                    ModelState.Remove("NumeroFactura");
                }
                
                // Validar ClienteId manualmente
                if (factura.ClienteId == 0)
                {
                    ModelState.AddModelError("ClienteId", "Debe seleccionar un cliente.");
                }

                // Validar IVA manualmente
                if (factura.IVA < 0 || factura.IVA > 100)
                {
                    ModelState.AddModelError("IVA", "El IVA debe estar entre 0 y 100.");
                }

                // Debug: Revisar errores del ModelState
                if (!ModelState.IsValid)
                {
                    Console.WriteLine("DEBUG: ModelState no es válido. Errores:");
                    foreach (var modelError in ModelState)
                    {
                        foreach (var error in modelError.Value.Errors)
                        {
                            Console.WriteLine($"Campo: {modelError.Key}, Error: {error.ErrorMessage}");
                        }
                    }
                }

                if (ModelState.IsValid)
                {
                    // Validación adicional de seguridad
                    if (ProductoIds == null || Cantidades == null || PreciosUnitarios == null)
                    {
                        ModelState.AddModelError("", "Datos de productos inválidos.");
                        throw new InvalidOperationException("Listas de productos null");
                    }

                    // Generar número de factura
                    if (string.IsNullOrEmpty(factura.NumeroFactura))
                    {
                        factura.NumeroFactura = GenerarNumeroFactura();
                    }

                    // Inicializar colección
                    factura.FacturaDetalles = new List<FacturaDetalle>();

                    // Crear detalles válidos
                    for (int i = 0; i < ProductoIds.Count; i++)
                    {
                        if (ProductoIds != null && Cantidades != null && PreciosUnitarios != null &&
                            i < Cantidades.Count && i < PreciosUnitarios.Count && 
                            ProductoIds[i] > 0 && Cantidades[i] > 0 && PreciosUnitarios[i] > 0)
                        {
                            var detalle = new FacturaDetalle
                            {
                                ProductoId = ProductoIds[i],
                                Cantidad = Cantidades[i],
                                PrecioUnitario = PreciosUnitarios[i],
                                Subtotal = Cantidades[i] * PreciosUnitarios[i]
                            };
                            factura.FacturaDetalles.Add(detalle);
                        }
                    }

                    if (factura.FacturaDetalles.Count == 0)
                    {
                        ModelState.AddModelError("", "No se pudieron procesar las líneas de productos.");
                        throw new InvalidOperationException("Sin detalles válidos");
                    }

                    // Calcular totales
                    CalcularTotales(factura);

                    // Guardar en base de datos
                    _context.Facturas.Add(factura);
                    var result = await _context.SaveChangesAsync();
                    
                    Console.WriteLine($"DEBUG: Factura guardada, registros afectados: {result}");

                    TempData["SuccessMessage"] = $"Factura {factura.NumeroFactura} creada exitosamente.";
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ERROR CREATE FACTURA: {ex.Message}");
                ModelState.AddModelError("", $"Error al crear la factura: {ex.Message}");
            }

            ViewData["ClienteId"] = new SelectList(_context.Clientes.Select(c => new { 
                Id = c.Id, 
                NombreCompleto = c.Nombre + " " + c.Apellido 
            }), "Id", "NombreCompleto", factura.ClienteId);
            
            ViewBag.Productos = _context.Productos.Include(p => p.Categoria).ToList();
            
            return View(factura);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var factura = await _context.Facturas
                .Include(f => f.FacturaDetalles)
                    .ThenInclude(fd => fd.Producto)
                .FirstOrDefaultAsync(f => f.Id == id);
            
            if (factura == null)
            {
                return NotFound();
            }

            ViewData["ClienteId"] = new SelectList(_context.Clientes.Select(c => new { 
                Id = c.Id, 
                NombreCompleto = c.Nombre + " " + c.Apellido 
            }), "Id", "NombreCompleto", factura.ClienteId);
            
            ViewBag.Productos = _context.Productos.Include(p => p.Categoria).ToList();
            
            return View(factura);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Factura factura, List<int> ProductoIds, List<int> Cantidades, List<decimal> PreciosUnitarios)
        {
            if (id != factura.Id)
            {
                return NotFound();
            }

            try
            {
                // Validaciones básicas
                if (ProductoIds == null || ProductoIds.Count == 0 || ProductoIds.All(p => p == 0))
                {
                    ModelState.AddModelError("", "Debe seleccionar al menos un producto válido.");
                }

                if (Cantidades == null || Cantidades.Count == 0 || Cantidades.All(c => c <= 0))
                {
                    ModelState.AddModelError("", "Debe ingresar cantidades válidas.");
                }

                if (PreciosUnitarios == null || PreciosUnitarios.Count == 0 || PreciosUnitarios.All(p => p <= 0))
                {
                    ModelState.AddModelError("", "Los precios unitarios deben ser válidos.");
                }

                // Remover validaciones de campos que se manejan manualmente
                ModelState.Remove("FacturaDetalles");
                ModelState.Remove("Subtotal");
                ModelState.Remove("Total");
                ModelState.Remove("IRPF");
                ModelState.Remove("IVA");
                ModelState.Remove("Observaciones");
                ModelState.Remove("FormaPago");
                ModelState.Remove("Cliente");
                
                // Remover cualquier validación de propiedades de navegación
                var keysToRemove = ModelState.Keys.Where(k => 
                    k.Contains("Cliente.") || 
                    k.Contains("Producto.") || 
                    k.Contains("Categoria.") ||
                    k.Contains("FacturaDetalles.")
                ).ToList();
                
                foreach (var key in keysToRemove)
                {
                    ModelState.Remove(key);
                }

                // Validar ClienteId manualmente
                if (factura.ClienteId == 0)
                {
                    ModelState.AddModelError("ClienteId", "Debe seleccionar un cliente.");
                }

                // Validar IVA manualmente
                if (factura.IVA < 0 || factura.IVA > 100)
                {
                    ModelState.AddModelError("IVA", "El IVA debe estar entre 0 y 100.");
                }

                if (ModelState.IsValid)
                {
                    // Validación adicional de seguridad
                    if (ProductoIds == null || Cantidades == null || PreciosUnitarios == null)
                    {
                        ModelState.AddModelError("", "Datos de productos inválidos.");
                        throw new InvalidOperationException("Listas de productos null");
                    }

                    // Eliminar detalles existentes
                    var detallesExistentes = _context.FacturaDetalles.Where(fd => fd.FacturaId == id);
                    _context.FacturaDetalles.RemoveRange(detallesExistentes);

                    // Inicializar colección
                    factura.FacturaDetalles = new List<FacturaDetalle>();

                    // Crear nuevos detalles válidos
                    for (int i = 0; i < ProductoIds.Count; i++)
                    {
                        if (ProductoIds != null && Cantidades != null && PreciosUnitarios != null &&
                            i < Cantidades.Count && i < PreciosUnitarios.Count && 
                            ProductoIds[i] > 0 && Cantidades[i] > 0 && PreciosUnitarios[i] > 0)
                        {
                            var detalle = new FacturaDetalle
                            {
                                FacturaId = factura.Id,
                                ProductoId = ProductoIds[i],
                                Cantidad = Cantidades[i],
                                PrecioUnitario = PreciosUnitarios[i],
                                Subtotal = Cantidades[i] * PreciosUnitarios[i]
                            };
                            factura.FacturaDetalles.Add(detalle);
                        }
                    }

                    if (factura.FacturaDetalles.Count == 0)
                    {
                        ModelState.AddModelError("", "No se pudieron procesar las líneas de productos.");
                        throw new InvalidOperationException("Sin detalles válidos");
                    }

                    // Calcular totales
                    CalcularTotales(factura);

                    _context.Update(factura);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Factura actualizada exitosamente.";
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!FacturaExists(factura.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ERROR EDIT FACTURA: {ex.Message}");
                ModelState.AddModelError("", $"Error al actualizar la factura: {ex.Message}");
            }

            ViewData["ClienteId"] = new SelectList(_context.Clientes.Select(c => new { 
                Id = c.Id, 
                NombreCompleto = c.Nombre + " " + c.Apellido 
            }), "Id", "NombreCompleto", factura.ClienteId);
            
            ViewBag.Productos = _context.Productos.Include(p => p.Categoria).ToList();
            
            return View(factura);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var factura = await _context.Facturas
                .Include(f => f.Cliente)
                .Include(f => f.FacturaDetalles)
                    .ThenInclude(fd => fd.Producto)
                .FirstOrDefaultAsync(m => m.Id == id);
            
            if (factura == null)
            {
                return NotFound();
            }

            return View(factura);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var factura = await _context.Facturas.FindAsync(id);
            if (factura != null)
            {
                _context.Facturas.Remove(factura);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Factura eliminada exitosamente.";
            }

            return RedirectToAction(nameof(Index));
        }

        private bool FacturaExists(int id)
        {
            return _context.Facturas.Any(e => e.Id == id);
        }

        private string GenerarNumeroFactura()
        {
            var year = DateTime.Now.Year;
            var lastNumber = _context.Facturas
                .Where(f => f.NumeroFactura.StartsWith($"FACT-{year}-"))
                .Count() + 1;
            
            return $"FACT-{year}-{lastNumber:D4}";
        }

        private void CalcularTotales(Factura factura)
        {
            factura.Subtotal = factura.FacturaDetalles.Sum(fd => fd.Subtotal);
            
            // Calcular IRPF (15%)
            var importeIRPF = factura.Subtotal * (factura.IRPF / 100);
            
            // Calcular IVA si es mayor que 0
            var importeIVA = factura.IVA > 0 ? factura.Subtotal * (factura.IVA / 100) : 0;
            
            // Total = Subtotal - IRPF + IVA
            factura.Total = factura.Subtotal - importeIRPF + importeIVA;
        }
    }
}