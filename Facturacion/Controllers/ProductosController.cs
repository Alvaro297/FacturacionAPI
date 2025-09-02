using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Facturacion.Models.Data;
using Facturacion.Models.Entities;

namespace Facturacion.Controllers
{
    public class ProductosController : Controller
    {
        private readonly FacturacionDbContext _context;

        public ProductosController(FacturacionDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string searchTerm, string sortBy, string sortOrder, int? categoriaId)
        {
            ViewBag.CurrentFilter = searchTerm;
            ViewBag.CurrentSort = sortBy;
            ViewBag.CurrentOrder = sortOrder;
            ViewBag.CurrentCategoria = categoriaId;
            ViewBag.Categorias = new SelectList(_context.Categorias, "Id", "Nombre");
            
            var productos = _context.Productos.Include(p => p.Categoria).AsQueryable();

            // Filtrado por categoría
            if (categoriaId.HasValue)
            {
                productos = productos.Where(p => p.CategoriaId == categoriaId);
            }

            // Filtrado por término de búsqueda
            if (!string.IsNullOrEmpty(searchTerm))
            {
                productos = productos.Where(p => 
                    p.Nombre.Contains(searchTerm) ||
                    (p.Descripcion != null && p.Descripcion.Contains(searchTerm)) ||
                    (p.Categoria != null && p.Categoria.Nombre.Contains(searchTerm)));
            }

            // Ordenamiento
            switch (sortBy?.ToLower())
            {
                case "nombre":
                    productos = sortOrder == "desc" ? productos.OrderByDescending(p => p.Nombre) : productos.OrderBy(p => p.Nombre);
                    break;
                case "categoria":
                    productos = sortOrder == "desc" ? productos.OrderByDescending(p => p.Categoria.Nombre) : productos.OrderBy(p => p.Categoria.Nombre);
                    break;
                case "precio":
                    productos = sortOrder == "desc" ? productos.OrderByDescending(p => p.PrecioUnitario) : productos.OrderBy(p => p.PrecioUnitario);
                    break;
                default:
                    productos = productos.OrderBy(p => p.Nombre);
                    break;
            }

            return View(await productos.ToListAsync());
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var producto = await _context.Productos
                .Include(p => p.Categoria)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (producto == null)
            {
                return NotFound();
            }

            return View(producto);
        }

        public IActionResult Create()
        {
            ViewData["CategoriaId"] = new SelectList(_context.Categorias, "Id", "Nombre");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Nombre,Descripcion,PrecioUnitario,CategoriaId")] Producto producto)
        {
            if (ModelState.IsValid)
            {
                _context.Add(producto);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Producto creado exitosamente.";
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoriaId"] = new SelectList(_context.Categorias, "Id", "Nombre", producto.CategoriaId);
            return View(producto);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var producto = await _context.Productos.FindAsync(id);
            if (producto == null)
            {
                return NotFound();
            }
            ViewData["CategoriaId"] = new SelectList(_context.Categorias, "Id", "Nombre", producto.CategoriaId);
            return View(producto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Nombre,Descripcion,PrecioUnitario,CategoriaId")] Producto producto)
        {
            if (id != producto.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(producto);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Producto actualizado exitosamente.";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductoExists(producto.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoriaId"] = new SelectList(_context.Categorias, "Id", "Nombre", producto.CategoriaId);
            return View(producto);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var producto = await _context.Productos
                .Include(p => p.Categoria)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (producto == null)
            {
                return NotFound();
            }

            return View(producto);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var producto = await _context.Productos.FindAsync(id);
            if (producto != null)
            {
                try
                {
                    _context.Productos.Remove(producto);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Producto eliminado exitosamente.";
                }
                catch (DbUpdateException)
                {
                    TempData["ErrorMessage"] = "No se puede eliminar el producto porque está siendo usado en facturas.";
                    return RedirectToAction(nameof(Index));
                }
            }

            return RedirectToAction(nameof(Index));
        }

        private bool ProductoExists(int id)
        {
            return _context.Productos.Any(e => e.Id == id);
        }
    }
}