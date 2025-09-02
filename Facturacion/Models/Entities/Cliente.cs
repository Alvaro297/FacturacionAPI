using System.ComponentModel.DataAnnotations;

namespace Facturacion.Models.Entities
{
    public class Cliente
    {
        public int Id { get; set; }
        
        [Required(ErrorMessage = "El nombre es obligatorio")]
        [StringLength(100, ErrorMessage = "El nombre no puede exceder los 100 caracteres")]
        public string Nombre { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "El apellido es obligatorio")]
        [StringLength(100, ErrorMessage = "El apellido no puede exceder los 100 caracteres")]
        public string Apellido { get; set; } = string.Empty;
        
        [StringLength(9, MinimumLength = 9, ErrorMessage = "El NIF debe tener 9 caracteres")]
        public string? NIF { get; set; }
        
        [StringLength(9, MinimumLength = 9, ErrorMessage = "El CIF debe tener 9 caracteres")]
        public string? CIF { get; set; }
        
        [Required(ErrorMessage = "La dirección es obligatoria")]
        [StringLength(200, ErrorMessage = "La dirección no puede exceder los 200 caracteres")]
        public string Direccion { get; set; } = string.Empty;
        
        [Phone(ErrorMessage = "El formato del teléfono no es válido")]
        [StringLength(15, ErrorMessage = "El teléfono no puede exceder los 15 caracteres")]
        public string? Telefono { get; set; }
        
        [Required(ErrorMessage = "El email es obligatorio")]
        [EmailAddress(ErrorMessage = "El formato del email no es válido")]
        [StringLength(100, ErrorMessage = "El email no puede exceder los 100 caracteres")]
        public string Email { get; set; } = string.Empty;
        
        public virtual ICollection<Factura> Facturas { get; set; } = new List<Factura>();
    }
}