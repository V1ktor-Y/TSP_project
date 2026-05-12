using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace tsp.Models;

[Table("Accounts")]
public class Account
{
    [Key]
    public int AccountId { get; set; }

    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    public string PasswordHash { get; set; } = string.Empty;

    public ICollection<VFile> Files { get; set; } = new List<VFile>();
}
