using System;
using System.ComponentModel.DataAnnotations;

namespace testback.Models
{
    public class Inventario
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Codigo { get; set; } = string.Empty;
        [Required]
        public string Herramienta { get; set; } = string.Empty;
        [Required]
        public string NumeroSerie { get; set; } = string.Empty;
        public string Marca { get; set; } = string.Empty;
        public DateTime? FechaUltimoMantenimiento { get; set; }
        public DateTime? FechaProximoMantenimiento { get; set; }
        public string EmpresaMantenimiento { get; set; } = string.Empty;
        public DateTime? FechaCompra { get; set; }
        public string Proveedor { get; set; } = string.Empty;
        public int Garantia { get; set; } = 0;
        public string Observaciones { get; set; } = string.Empty;
        [Required]
        public string Ubicacion { get; set; } = string.Empty;
        [Required]
        public string Responsable { get; set; } = string.Empty;
        [Required]
        public string Estado { get; set; } = "Activo";
        [Required]
        public int Cantidad { get; set; } = 0;
    }
}
