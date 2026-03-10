using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace tsp.Models;

[Table("Files")]
public class VFile
{
    [Key]
    public int FileId { get; set; }

    public string? FileName { get; set; }

    public byte[]? FileData { get; set; }
}