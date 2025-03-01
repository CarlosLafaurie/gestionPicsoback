using System.ComponentModel.DataAnnotations.Schema;

public class Empleado
{
    public int Id { get; set; }
    public string Cedula { get; set; }
    public string NombreCompleto { get; set; }
    public string Cargo { get; set; }
    public string Obra { get; set; }
    public string Responsable { get; set; }

    [Column("fecha_hora_entrada")]
    public DateTime? FechaHoraEntrada { get; set; }

    [Column("fecha_hora_salida")]
    public DateTime? FechaHoraSalida { get; set; }

    public string? Comentarios { get; set; }

    [Column("permisos_especiales")]
    public string? PermisosEspeciales { get; set; }
}
