using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace testback.Models.Dtos
{
    public class SubirDocumentoDto
    {
        [Required]
        public string NombreEmpleado { get; set; } = null!;
        public string? Comentarios { get; set; }
        [Required]
        public DateTime FechaHoraEntrada { get; set; }
        [Required]
        public IFormFile Archivo { get; set; }
    }
}
