using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace testback.Models
{
    [Table("documentopermiso")]
    public class DocumentoPermiso
    {
        public int Id { get; set; }
        [Required]
        public required string NombreEmpleado { get; set; }
        public string? Comentarios { get; set; }
        [Required]
        public required DateTime FechaInicio { get; set; }
        [Required]
        public required DateTime FechaFin { get; set; }
        [Required]
        public required byte[] Archivo { get; set; }
        [Required]
        public required string NombreArchivo { get; set; }
    }
}