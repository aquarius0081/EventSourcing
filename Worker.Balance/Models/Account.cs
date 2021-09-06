using System.ComponentModel.DataAnnotations;

namespace Worker.Balance.Models
{
    public class Account
    {
        [Key]
        public int AccountId { get; set; }

        [Required]
        public string AccountName { get; set; }

        [Required]
        public decimal Balance { get; set; }
    }
}