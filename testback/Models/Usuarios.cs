using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

public class Usuario
{
    public int Id { get; set; }
    [Required]
    public required string Cedula { get; set; }
    [Required]
    public required string NombreCompleto { get; set; }
    [Required]
    public required string Cargo { get; set; }
    [Required]
    public required string Rol { get; set; }
    [Required]
    public required string ContrasenaHash { get; set; }
    [Required]
    [StringLength(10)]
    public required string Estado { get; set; } = "Activo";
}
