using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Noticia.EntityFrameworkCore.MemoryJoin.UnitTests.Models;

public class JoinableTableModel
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    
    public string? Test { get; set; }
    
    public string? StringValue { get; set; }
}