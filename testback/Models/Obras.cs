using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace testback.Models
{
    public class Obra
    {
        public int Id { get; set; }
        [Required]
        public required string NombreObra { get; set; }
        [Required]
        public required string Responsable { get; set; }
        public string? ResponsableSecundario { get; set; }
        [Required]
        public required string ClienteObra { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public required decimal CostoObra { get; set; }
        [Required]
        public required string Estado { get; set; } = "Activo";
        [Required]
        public required string Ubicacion { get; set; }
    }
}
