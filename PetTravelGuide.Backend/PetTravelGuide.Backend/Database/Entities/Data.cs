using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

[Table("data")]
public class Data
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)] // Add this attribute
    [Key]
    [Column("id")]
    public int IdData { get; set; }

    [ForeignKey("User")]
    [Column("user_id")]
    public int UserID { get; set; }

    [ForeignKey("Item")]
    [Column("location_id")]
    public int LocationID { get; set; }

    [Column("rating")]
    public double Rating { get; set; }

    public Utilizator? Utilizator { get; set; }
    public Item? Item { get; set; }
}
