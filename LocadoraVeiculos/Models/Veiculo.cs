using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LocadoraVeiculos.Models
{
    public class Veiculo
    {
        [Key]
        public int IdVeiculo { get; set; }

        [Required]
        [MaxLength(100)]
        public string Modelo { get; set; } = string.Empty;

        [Required]
        public int AnoFabricacao { get; set; }

        [Required]
        public int Quilometragem { get; set; }

        [Required]
        [MaxLength(10)]
        public string Placa { get; set; } = string.Empty;

        [ForeignKey("Fabricante")]
        public int IdFabricante { get; set; }

        public Fabricante? Fabricante { get; set; }

        [ForeignKey("Categoria")]
        public int IdCategoria { get; set; }

        public Categoria? Categoria { get; set; }

        public ICollection<Aluguel> Alugueis { get; set; }
            = new List<Aluguel>();
    }
}