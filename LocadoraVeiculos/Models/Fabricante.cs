using System.ComponentModel.DataAnnotations;

namespace LocadoraVeiculos.Models
{
    public class Fabricante
    {
        [Key]
        public int IdFabricante { get; set; }

        [Required]
        [MaxLength(100)]
        public string Nome { get; set; } = string.Empty;

        public ICollection<Veiculo> Veiculos { get; set; }
            = new List<Veiculo>();
    }
}