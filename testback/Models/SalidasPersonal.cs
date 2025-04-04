using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace testback.Models
{
    public class SalidasPersonal
    {
        [Key]
        public required int Id { get; set; }
        [ForeignKey("Empleado")]
        public required int EmpleadoId { get; set; }
        public required DateTime FechaHoraSalida { get; set; }
        public string? Comentarios { get; set; }
    }
}
