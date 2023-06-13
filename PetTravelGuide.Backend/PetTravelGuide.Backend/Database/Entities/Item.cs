using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

[Table("item")]
public class Item
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("country")]
    public string? Country { get; set; }

    [Column("county")]
    public string? County { get; set; }

    [Column("city")]
    public string? City { get; set; }

    [Column("name")]
    public string? Name { get; set; }

    [Column("latitude")]
    public double Latitude { get; set; }

    [Column("longitude")]
    public double Longitude { get; set; }

    [Column("rating")]
    public double Rating { get; set; }

    [Column("userratingstotal")]
    public int UserRatingsTotal { get; set; }

    [Column("onestar")]
    public int OneStar { get; set; }

    [Column("twostar")]
    public int TwoStar { get; set; }

    [Column("threestar")]
    public int ThreeStar { get; set; }

    [Column("fourstar")]
    public int FourStar { get; set; }

    [Column("fivestar")]
    public int FiveStar { get; set; }

    [Column("mountains")]
    public int Mountains { get; set; }

    [Column("hills")]
    public int Hills { get; set; }

    [Column("plains")]
    public int Plains { get; set; }

    [Column("plateaus")]
    public int Plateaus { get; set; }

    [Column("valleys")]
    public int Valleys { get; set; }

    [Column("glacialfields")]
    public int GlacialFields { get; set; }

    [Column("deltas")]
    public int Deltas { get; set; }

    [Column("canyons")]
    public int Canyons { get; set; }

    [Column("beaches")]
    public int Beaches { get; set; }

    [Column("naturespots")]
    public int NatureSpots { get; set; }

    [Column("stunningviews")]
    public int StunningViews { get; set; }

    [Column("lakes")]
    public int Lakes { get; set; }

    [Column("parks")]
    public int Parks { get; set; }

    [Column("iconiccities")]
    public int IconicCities { get; set; }

    [Column("farms")]
    public int Farms { get; set; }

    [Column("castles")]
    public int Castles { get; set; }

    [Column("historicalhomes")]
    public int HistoricalHomes { get; set; }

    [Column("boatrides")]
    public int BoatRides { get; set; }

    [Column("lakefrontareas")]
    public int LakefrontAreas { get; set; }

    [Column("swimmingareas")]
    public int SwimmingAreas { get; set; }

    [Column("caves")]
    public int Caves { get; set; }

    [Column("playgrounds")]
    public int Playgrounds { get; set; }
}
