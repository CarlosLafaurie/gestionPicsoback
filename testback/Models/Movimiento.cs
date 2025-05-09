using System.ComponentModel.DataAnnotations;

namespace testback.Models
{
    public class Movimiento
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public int InventarioId { get; set; } 
        [Required]
        public string CodigoHerramienta { get; set; } = string.Empty;
        [Required]
        public string NombreHerramienta { get; set; } = string.Empty;
        [Required]
        public string Responsable { get; set; } = string.Empty;
        [Required]
        public string Obra { get; set; } = string.Empty;
        [Required]
        public DateTime FechaMovimiento { get; set; } = DateTime.UtcNow;
        [Required]
        public string TipoMovimiento { get; set; } = string.Empty;
        [Required]
        public string Estado { get; set; } = string.Empty;
        public string Comentario { get; set; } = string.Empty;
    }
}
