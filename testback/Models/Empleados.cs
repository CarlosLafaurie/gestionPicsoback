using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Empleado
{
    public int Id { get; set; }
    [Required]
    public string Cedula { get; set; }
    [Required]
    public string NombreCompleto { get; set; }
    [Required]
    public string Cargo { get; set; }
    [Required]
    public string Obra { get; set; }
    [Required]
    public string Responsable { get; set; }
    public string? ResponsableSecundario { get; set; }
    [Column(TypeName = "decimal(18,2)")]
    public decimal Salario { get; set; }
    [Required]
    public string Estado { get; set; } = "Activo";
    public string? Telefono { get; set; }
    public string? NumeroCuenta { get; set; }
}
