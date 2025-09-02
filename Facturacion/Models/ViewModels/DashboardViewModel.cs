using Facturacion.Models.Entities;

namespace Facturacion.Models.ViewModels
{
    public class DashboardViewModel
    {
        public int TotalFacturas { get; set; }
        public decimal TotalFacturado { get; set; }
        public int TotalClientes { get; set; }
        public int TotalProductos { get; set; }
        public decimal FacturaPromedio { get; set; }
        public List<Factura> UltimasFacturas { get; set; } = new List<Factura>();
    }
}