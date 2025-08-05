using System.ComponentModel.DataAnnotations;

namespace BancoKRT.Application.DTOs
{
    public class ProcessarTransacaoDto
    {
        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "O valor da transação deve ser positivo.")]
        public decimal Valor { get; set; }
    }
}
