using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

[Table("utilizatori")]
public class Utilizator
{
    [Key]
    [Column("id")]
    public int IdUser { get; set; }

    [Column("firstname")]
    public string? FirstName { get; set; }

    [Column("lastname")]
    public string? LastName { get; set; }
}
