using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Facturacion.Models.Entities
{
    public class Factura
    {
        public int Id { get; set; }
        
        [Required(ErrorMessage = "El número de factura es obligatorio")]
        [StringLength(20, ErrorMessage = "El número de factura no puede exceder los 20 caracteres")]
        public string NumeroFactura { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "La fecha es obligatoria")]
        public DateTime Fecha { get; set; }
        
        [Required(ErrorMessage = "El cliente es obligatorio")]
        public int ClienteId { get; set; }
        
        [Column(TypeName = "decimal(18,2)")]
        public decimal Subtotal { get; set; }
        
        [Column(TypeName = "decimal(18,2)")]
        public decimal IRPF { get; set; } = 15.0m; // 15% por defecto
        
        [Column(TypeName = "decimal(18,2)")]
        [Range(0, 100, ErrorMessage = "El IVA debe estar entre 0 y 100")]
        public decimal IVA { get; set; } = 0.0m; // Opcional, por defecto 0
        
        [Column(TypeName = "decimal(18,2)")]
        public decimal Total { get; set; }
        
        [StringLength(1000, ErrorMessage = "Las observaciones no pueden exceder los 1000 caracteres")]
        public string? Observaciones { get; set; }
        
        [StringLength(100, ErrorMessage = "La forma de pago no puede exceder los 100 caracteres")]
        public string? FormaPago { get; set; }
        
        public virtual Cliente Cliente { get; set; } = null!;
        
        public virtual ICollection<FacturaDetalle> FacturaDetalles { get; set; } = new List<FacturaDetalle>();
    }
}