using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace testback.Models
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum EstadoSolicitud
    {
        Pendiente, 
        Aprobada,   
        Rechazada,
        Comprado
    }
    [Table("solicitud")]
    public class Solicitud
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }
        [Required]
        [Column("inventarioId")]
        public int InventarioId { get; set; }
        [ForeignKey(nameof(InventarioId))]
        public Inventario? Inventario { get; set; }
        [Required]
        [Column("solicitante")]
        public string Solicitante { get; set; } = string.Empty;
        [Required]
        [Column("obra")]
        public string Obra { get; set; } = string.Empty;
        [Required]
        [Column("estado")]
        public EstadoSolicitud Estado { get; set; } = EstadoSolicitud.Pendiente;
        [Required]
        [Column("cantidad")]
        public int Cantidad { get; set; }
        [Required]
        [Column("fechaSolicitud")]
        public DateTime FechaSolicitud { get; set; }
        [Column("observaciones")]
        public string? Observaciones { get; set; }
    }
}
