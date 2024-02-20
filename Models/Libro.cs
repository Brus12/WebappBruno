using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebappBruno.Models
{
    [Table("libroes")]
    public class Libro
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]

        public int Id { get; set; }
        public string Titulo { get; set; }
        [DataType(DataType.Date)]
        public DateTime AnioPublicacion { get; set; }
        public string Foto { get; set; } = "";
        public virtual Autor Autor { get; set; }
    }
}
