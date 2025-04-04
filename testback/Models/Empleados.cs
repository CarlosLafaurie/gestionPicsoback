using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
public class Empleado
{
    public int Id { get; set; }
    [Required]
    public required string Cedula { get; set; }
    [Required]
    public required string NombreCompleto { get; set; }
    [Required]
    public required string Cargo { get; set; }
    [Required]
    public required string Obra { get; set; }
    [Required]
    public required string Responsable { get; set; }
    public string? ResponsableSecundario { get; set; }
    [Column(TypeName = "decimal(18,2)")]
    public required decimal Salario { get; set; }
    [Required]
    public required string Estado { get; set; } = "Activo";
}
