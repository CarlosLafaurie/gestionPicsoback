using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace testback.Models
{
    [Table("solicitud_item")]
    public class SolicitudItem
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [Column("solicitudId")]
        public int SolicitudId { get; set; }

        [ForeignKey(nameof(SolicitudId))]
        public Solicitud? Solicitud { get; set; }

        [Required]
        [Column("inventarioId")]
        public int InventarioId { get; set; }

        [ForeignKey(nameof(InventarioId))]
        public Inventario? Inventario { get; set; }

        [Required]
        [Column("cantidad")]
        public int Cantidad { get; set; }
    }
}
