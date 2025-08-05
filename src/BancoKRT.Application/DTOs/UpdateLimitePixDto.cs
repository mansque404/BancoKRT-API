using System.ComponentModel.DataAnnotations;

namespace BancoKRT.Application.DTOs
{
    public class UpdateLimitePixDto
    {
        [Required]
        [Range(0, double.MaxValue, ErrorMessage = "O limite PIX não pode ser negativo.")]
        public decimal NovoLimitePix { get; set; }
    }
}
