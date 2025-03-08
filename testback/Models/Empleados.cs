using System.ComponentModel.DataAnnotations.Schema;

public class Empleado
{
    public int Id { get; set; }
    public string Cedula { get; set; }
    public string NombreCompleto { get; set; }
    public string Cargo { get; set; }
    public string Obra { get; set; }
    public string Responsable { get; set; }
}
