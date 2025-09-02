using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Facturacion.Models.Entities
{
    public class FacturaDetalle
    {
        public int Id { get; set; }
        
        [Required(ErrorMessage = "La factura es obligatoria")]
        public int FacturaId { get; set; }
        
        [Required(ErrorMessage = "El producto es obligatorio")]
        public int ProductoId { get; set; }
        
        [Required(ErrorMessage = "La cantidad es obligatoria")]
        [Range(1, int.MaxValue, ErrorMessage = "La cantidad debe ser mayor que 0")]
        public int Cantidad { get; set; }
        
        [Required(ErrorMessage = "El precio unitario es obligatorio")]
        [Range(0.01, double.MaxValue, ErrorMessage = "El precio unitario debe ser mayor que 0")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal PrecioUnitario { get; set; }
        
        [Column(TypeName = "decimal(18,2)")]
        public decimal Subtotal { get; set; }
        
        public virtual Factura Factura { get; set; } = null!;
        
        public virtual Producto Producto { get; set; } = null!;
    }
}