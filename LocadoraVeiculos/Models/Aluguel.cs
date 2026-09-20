using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LocadoraVeiculos.Models
{
    public class Aluguel
    {
        [Key]
        public int IdAluguel { get; set; }

        [ForeignKey("Cliente")]
        public int IdCliente { get; set; }

        public Cliente? Cliente { get; set; }

        [ForeignKey("Veiculo")]
        public int IdVeiculo { get; set; }

        public Veiculo? Veiculo { get; set; }

        [Required]
        public DateTime DataInicio { get; set; }

        [Required]
        public DateTime DataFim { get; set; }

        public DateTime? DataDevolucao { get; set; }

        [Required]
        public int QuilometragemInicial { get; set; }

        public int? QuilometragemFinal { get; set; }

        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal ValorDiaria { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal? ValorTotal { get; set; }
    }
}