using System.ComponentModel.DataAnnotations;

namespace MyPortfolio.Models
{
    public class Stock
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string? Symbol { get; set; }

        [Required]
        public string? CompanyName { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1")]
        public int Quantity { get; set; }

        [DataType(DataType.Currency)]
        public decimal BuyPrice { get; set; }

        [DataType(DataType.Date)]
        public DateTime BuyDate { get; set; }

        // Optional properties
        [DataType(DataType.Currency)]
        public decimal? CurrentPrice { get; set; }

        //[DataType(DataType.MultilineText)]
        public string? Notes { get; set; }

        [Required]
        public string Username { get; set; }
    }
}
