using System.ComponentModel.DataAnnotations.Schema;

public class Usuario
{
    public int Id { get; set; }
    public string Cedula { get; set; }
    public string NombreCompleto { get; set; }
    public string Cargo { get; set; }
    public string Obra { get; set; }
    public string Contrasena { get; set; }
}

