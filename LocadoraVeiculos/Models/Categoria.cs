using System.ComponentModel.DataAnnotations;

namespace LocadoraVeiculos.Models
{
    public class Categoria
    {
        [Key]
        public int IdCategoria { get; set; }

        [Required]
        [MaxLength(50)]
        public string Nome { get; set; } = string.Empty;

        [MaxLength(200)]
        public string? Descricao { get; set; }

        public ICollection<Veiculo> Veiculos { get; set; }
            = new List<Veiculo>();
    }
}