using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Facturacion.Models.Data;
using Facturacion.Models.Entities;

namespace Facturacion.Controllers
{
    public class ClientesController : Controller
    {
        private readonly FacturacionDbContext _context;

        public ClientesController(FacturacionDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string searchTerm, string sortBy, string sortOrder)
        {
            ViewBag.CurrentFilter = searchTerm;
            ViewBag.CurrentSort = sortBy;
            ViewBag.CurrentOrder = sortOrder;
            
            var clientes = _context.Clientes.AsQueryable();

            // Filtrado
            if (!string.IsNullOrEmpty(searchTerm))
            {
                clientes = clientes.Where(c => 
                    c.Nombre.Contains(searchTerm) ||
                    c.Apellido.Contains(searchTerm) ||
                    c.Email.Contains(searchTerm) ||
                    (c.NIF != null && c.NIF.Contains(searchTerm)) ||
                    (c.CIF != null && c.CIF.Contains(searchTerm)) ||
                    (c.Telefono != null && c.Telefono.Contains(searchTerm)));
            }

            // Ordenamiento
            switch (sortBy?.ToLower())
            {
                case "nombre":
                    clientes = sortOrder == "desc" ? clientes.OrderByDescending(c => c.Nombre) : clientes.OrderBy(c => c.Nombre);
                    break;
                case "apellido":
                    clientes = sortOrder == "desc" ? clientes.OrderByDescending(c => c.Apellido) : clientes.OrderBy(c => c.Apellido);
                    break;
                case "email":
                    clientes = sortOrder == "desc" ? clientes.OrderByDescending(c => c.Email) : clientes.OrderBy(c => c.Email);
                    break;
                case "nif":
                    clientes = sortOrder == "desc" ? clientes.OrderByDescending(c => c.NIF) : clientes.OrderBy(c => c.NIF);
                    break;
                case "cif":
                    clientes = sortOrder == "desc" ? clientes.OrderByDescending(c => c.CIF) : clientes.OrderBy(c => c.CIF);
                    break;
                default:
                    clientes = clientes.OrderBy(c => c.Nombre);
                    break;
            }

            return View(await clientes.ToListAsync());
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cliente = await _context.Clientes
                .FirstOrDefaultAsync(m => m.Id == id);
            if (cliente == null)
            {
                return NotFound();
            }

            return View(cliente);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Nombre,Apellido,NIF,CIF,Direccion,Telefono,Email")] Cliente cliente)
        {
            if (ModelState.IsValid)
            {
                // Validar que al menos NIF o CIF esté presente
                if (string.IsNullOrEmpty(cliente.NIF) && string.IsNullOrEmpty(cliente.CIF))
                {
                    ModelState.AddModelError("", "Debe proporcionar al menos un NIF o CIF.");
                    return View(cliente);
                }

                try
                {
                    _context.Add(cliente);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Cliente creado exitosamente.";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateException)
                {
                    ModelState.AddModelError("", "Ya existe un cliente con ese email, NIF o CIF.");
                }
            }
            return View(cliente);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cliente = await _context.Clientes.FindAsync(id);
            if (cliente == null)
            {
                return NotFound();
            }
            return View(cliente);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Nombre,Apellido,NIF,CIF,Direccion,Telefono,Email")] Cliente cliente)
        {
            if (id != cliente.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                // Validar que al menos NIF o CIF esté presente
                if (string.IsNullOrEmpty(cliente.NIF) && string.IsNullOrEmpty(cliente.CIF))
                {
                    ModelState.AddModelError("", "Debe proporcionar al menos un NIF o CIF.");
                    return View(cliente);
                }

                try
                {
                    _context.Update(cliente);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Cliente actualizado exitosamente.";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ClienteExists(cliente.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                catch (DbUpdateException)
                {
                    ModelState.AddModelError("", "Ya existe un cliente con ese email, NIF o CIF.");
                    return View(cliente);
                }
                return RedirectToAction(nameof(Index));
            }
            return View(cliente);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cliente = await _context.Clientes
                .FirstOrDefaultAsync(m => m.Id == id);
            if (cliente == null)
            {
                return NotFound();
            }

            return View(cliente);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var cliente = await _context.Clientes.FindAsync(id);
            if (cliente != null)
            {
                try
                {
                    _context.Clientes.Remove(cliente);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Cliente eliminado exitosamente.";
                }
                catch (DbUpdateException)
                {
                    TempData["ErrorMessage"] = "No se puede eliminar el cliente porque tiene facturas asociadas.";
                    return RedirectToAction(nameof(Index));
                }
            }

            return RedirectToAction(nameof(Index));
        }

        private bool ClienteExists(int id)
        {
            return _context.Clientes.Any(e => e.Id == id);
        }
    }
}