using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace testback.Models
{
    public class IngresosPersonal
    {
        [Key]
        public required int Id { get; set; }
        [ForeignKey("Empleado")]
        public required int EmpleadoId { get; set; }
        public required DateTime FechaHoraEntrada { get; set; }
        public string? Comentarios { get; set; }
    }
}