using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


public class Obra
    {
        public int Id { get; set; }
        [Required]
        public required string NombreObra { get; set; }
        [Required]
        public required string ClienteObra { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public required decimal CostoObra { get; set; }
        [Required]
        public required string Estado { get; set; } = "Activo";
        [Required]
        public required string Ubicacion { get; set; }
        public int? ResponsableId { get; set; }
        [NotMapped]
        public object? Responsable => null;
        public string? ResponsableSecundario { get; set; }
    }
