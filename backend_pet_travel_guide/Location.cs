using System.ComponentModel.DataAnnotations.Schema;

[Table("locations")]
public class Location
{
    public int id { get; set; }
    public string? filename { get; set; }
    public string? ispetfriendly { get; set; }
    public string? country { get; set; }
    
    [Column("county")]
    public string? county { get; set; }
    public string? city { get; set; }
    public string? name { get; set; }
    
    [Column("category")]
    public string? Category { get; set; }
    public decimal latitude { get; set; }
    public decimal longitude { get; set; }
    public decimal rating { get; set; }
    public int userratingstotal { get; set; }
    public int onestar { get; set; }
    public int twostar { get; set; }
    public int threestar { get; set; }
    public int fourstar { get; set; }
    public int fivestar { get; set; }
    public string? street { get; set; }
    public string? streetnumber { get; set; }
    public string? postalcode { get; set; }
    public string? phone { get; set; }
    public string? website { get; set; }
}