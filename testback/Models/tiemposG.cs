using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class tiemposg
{
    [Key]
    public int Id { get; set; }

    [ForeignKey("Empleado")]
    public int EmpleadoId { get; set; }

    public DateTime? FechaHoraEntrada { get; set; }
    public DateTime? FechaHoraSalida { get; set; }
    public string? Comentarios { get; set; }
    public string? PermisosEspeciales { get; set; }

    public virtual Empleado Empleado { get; set; }
}
