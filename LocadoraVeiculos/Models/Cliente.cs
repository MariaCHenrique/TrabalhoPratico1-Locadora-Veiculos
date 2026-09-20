using System.ComponentModel.DataAnnotations;

namespace LocadoraVeiculos.Models
{
    public class Cliente
    {
        [Key]
        public int IdCliente { get; set; }

        [Required]
        [MaxLength(150)]
        public string Nome { get; set; } = string.Empty;

        [Required]
        [MaxLength(11)]
        public string CPF { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [MaxLength(150)]
        public string Email { get; set; } = string.Empty;

        [MaxLength(20)]
        public string? Telefone { get; set; }

        public ICollection<Aluguel> Alugueis { get; set; }
            = new List<Aluguel>();
    }
}