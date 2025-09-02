using System.ComponentModel.DataAnnotations;

namespace Facturacion.Models.Entities
{
    public class Categoria
    {
        public int Id { get; set; }
        
        [Required(ErrorMessage = "El nombre de la categoría es obligatorio")]
        [StringLength(100, ErrorMessage = "El nombre no puede exceder los 100 caracteres")]
        public string Nombre { get; set; } = string.Empty;
        
        [StringLength(500, ErrorMessage = "La descripción no puede exceder los 500 caracteres")]
        public string? Descripcion { get; set; }
        
        public virtual ICollection<Producto> Productos { get; set; } = new List<Producto>();
    }
}