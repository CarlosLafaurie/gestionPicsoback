using System.ComponentModel.DataAnnotations.Schema;

namespace testback.Models
{
    public class Obra
    {
        public int Id { get; set; }

        [Column("nombre_obra")]
        public string NombreObra { get; set; }

        public string Responsable { get; set; }

        [Column("cliente_obra")]
        public string ClienteObra { get; set; }
    }
}

