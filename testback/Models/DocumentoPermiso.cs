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
        public required DateTime FechaHoraEntrada { get; set; }
        [Required]
        public required string RutaDocumento { get; set; }
    }
}
