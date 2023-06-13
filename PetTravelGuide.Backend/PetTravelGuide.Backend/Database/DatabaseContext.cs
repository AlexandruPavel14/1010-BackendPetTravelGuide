using Microsoft.EntityFrameworkCore;
using PetTravelGuide.Backend.Database.Entities;


namespace PetTravelGuide.Backend.Database;

public class DatabaseContext : DbContext
{
    public DatabaseContext() { }

    public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options) { }
    public virtual DbSet<Location> Locations { get; set; } = null!;
    public virtual DbSet<Role> Roles { get; set; } = null!;
    public virtual DbSet<User> Users { get; set; } = null!;
    public virtual DbSet<Pet> Pets { get; set; } = null!;
    public virtual DbSet<Item> Items { get; set; } = null!;
    public virtual DbSet<Utilizator> Utilizatori { get; set; } = null!;
    public virtual DbSet<Data> Data { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Role>(entity =>
        {
            entity.ToTable("roles", "authentication");

            entity.Property(e => e.Id)
                .HasColumnName("id")
                .UseIdentityAlwaysColumn();

            entity.Property(e => e.Name)
                .HasMaxLength(256)
                .HasColumnName("name");

            entity.Property(e => e.NormalizedName)
                .HasMaxLength(256)
                .HasColumnName("normalized_name");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("users", "authentication");

            entity.HasIndex(e => e.Email, "users_email_index")
                .IsUnique();

            entity.Property(e => e.Id)
                .HasColumnName("id")
                .UseIdentityAlwaysColumn();

            entity.Property(e => e.Email)
                .HasMaxLength(256)
                .HasColumnName("email");

            // entity.Property(e => e.EmailConfirmed)
            //     .HasColumnName("email_confirmed");

            entity.Property(e => e.FirstName)
                .HasMaxLength(32)
                .HasColumnName("first_name");

            entity.Property(e => e.LastName)
                .HasMaxLength(32)
                .HasColumnName("last_name");

            entity.Property(e => e.Gender)
                .HasMaxLength(1)
                .HasColumnName("gender");
            
            entity.Property(e => e.PasswordHash)
                .HasMaxLength(128)
                .HasColumnName("password_hash"); 

            entity.Property(e => e.PhoneNumber)
                .HasMaxLength(16)
                .HasColumnName("phone_number");            
            
            // entity.Property(e => e.LockoutEnabled)
            //     .HasColumnName("lockout_enabled");

            // entity.Property(e => e.LockoutEnd)
            //     .HasColumnType("timestamp without time zone")
            //     .HasColumnName("lockout_end");
            
            // entity.Property(e => e.AccessFailedCount)
            //     .HasColumnName("access_failed_count");            

            // entity.Property(e => e.PictureUrl)
            //     .HasMaxLength(1024)
            //     .HasColumnName("picture_url");
            
            entity.Property(e => e.CreatedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at")
                .HasDefaultValueSql("timezone('Europe/Bucharest'::text, now())");

            entity.Property(e => e.ModifiedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("modified_at");

            entity.Property(e => e.Active)
                .IsRequired()
                .HasColumnName("active")
                .HasDefaultValueSql("true");
            
            entity.HasMany(d => d.Roles)
                .WithMany(p => p.Users)
                .UsingEntity<Dictionary<string, object>>(
                    "UsersRole",
                    l => l.HasOne<Role>().WithMany().HasForeignKey("RoleId").HasConstraintName("user_roles_roles_role_id_fkey"),
                    r => r.HasOne<User>().WithMany().HasForeignKey("UserId").HasConstraintName("user_roles_users_user_id"),
                    j =>
                    {
                        j.HasKey("UserId", "RoleId").HasName("users_roles_pkey");
                        j.ToTable("users_roles", "authentication");
                        j.IndexerProperty<long>("UserId").HasColumnName("user_id");
                        j.IndexerProperty<short>("RoleId").HasColumnName("role_id");
                    });
        });


        modelBuilder.Entity<Pet>(entity =>
        {
            entity.ToTable("pets", "public");
            
            entity.Property(e => e.Id)
                .HasColumnName("id")
                .UseIdentityAlwaysColumn();

            entity.Property(e => e.UserId)
                .HasColumnName("user_id")
                .IsRequired();

            entity.Property(e => e.Name)
                .HasColumnName("name")
                .HasMaxLength(128)
                .IsRequired();

            entity.Property(e => e.Species)
                .HasColumnName("species")
                .HasMaxLength(128)
                .IsRequired();

            entity.Property(e => e.Age)
                .HasColumnName("age")
                .IsRequired();
            
            entity.Property(e => e.CreatedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at")
                .HasDefaultValueSql("timezone('Europe/Bucharest'::text, now())");

            entity.HasOne<User>(e => e.User)
                .WithMany(e => e.Pets)
                .HasForeignKey(e => e.UserId);
        });

        modelBuilder.Entity<Item>(entity =>
        {
            entity.ToTable("item");

            entity.Property(e => e.Id)
                .HasColumnName("id");

            entity.Property(e => e.Country)
                .HasColumnName("country");

            entity.Property(e => e.County)
                .HasColumnName("county");

            entity.Property(e => e.City)
                .HasColumnName("city");

            entity.Property(e => e.Name)
                .HasColumnName("name");

            entity.Property(e => e.Latitude)
                .HasColumnName("latitude");

            entity.Property(e => e.Longitude)
                .HasColumnName("longitude");

            entity.Property(e => e.Rating)
                .HasColumnName("rating");

            entity.Property(e => e.UserRatingsTotal)
                .HasColumnName("userratingstotal");

            entity.Property(e => e.OneStar)
                .HasColumnName("onestar");

            entity.Property(e => e.TwoStar)
                .HasColumnName("twostar");

            entity.Property(e => e.ThreeStar)
                .HasColumnName("threestar");

            entity.Property(e => e.FourStar)
                .HasColumnName("fourstar");

            entity.Property(e => e.FiveStar)
                .HasColumnName("fivestar");

            entity.Property(e => e.Mountains)
                .HasColumnName("mountains");

            entity.Property(e => e.Hills)
                .HasColumnName("hills");

            entity.Property(e => e.Plains)
                .HasColumnName("plains");

            entity.Property(e => e.Plateaus)
                .HasColumnName("plateaus");

            entity.Property(e => e.Valleys)
                .HasColumnName("valleys");

            entity.Property(e => e.GlacialFields)
                .HasColumnName("glacialfields");

            entity.Property(e => e.Deltas)
                .HasColumnName("deltas");

            entity.Property(e => e.Canyons)
                .HasColumnName("canyons");

            entity.Property(e => e.Beaches)
                .HasColumnName("beaches");

            entity.Property(e => e.NatureSpots)
                .HasColumnName("naturespots");

            entity.Property(e => e.StunningViews)
                .HasColumnName("stunningviews");

            entity.Property(e => e.Lakes)
                .HasColumnName("lakes");

            entity.Property(e => e.Parks)
                .HasColumnName("parks");

            entity.Property(e => e.IconicCities)
                .HasColumnName("iconiccities");

            entity.Property(e => e.Farms)
                .HasColumnName("farms");

            entity.Property(e => e.Castles)
                .HasColumnName("castles");

            entity.Property(e => e.HistoricalHomes)
                .HasColumnName("historicalhomes");

            entity.Property(e => e.BoatRides)
                .HasColumnName("boatrides");

            entity.Property(e => e.LakefrontAreas)
                .HasColumnName("lakefrontareas");

            entity.Property(e => e.SwimmingAreas)
                .HasColumnName("swimmingareas");

            entity.Property(e => e.Caves)
                .HasColumnName("caves");

            entity.Property(e => e.Playgrounds)
                .HasColumnName("playgrounds");

            // Configuration for Item entity...
        });


        modelBuilder.Entity<Utilizator>(entity =>
        {
            entity.ToTable("utilizatori");

            entity.Property(e => e.IdUser)
                .HasColumnName("id");

            entity.Property(e => e.FirstName)
                .HasColumnName("firstname");

            entity.Property(e => e.LastName)
                .HasColumnName("lastname");

            // Configuration for Utilizator entity...
        });


            modelBuilder.Entity<Data>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("data");

                entity.Property(e => e.IdData)
                    .HasColumnName("id")
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.UserID)
                    .HasColumnName("user_id");

                entity.Property(e => e.LocationID)
                    .HasColumnName("location_id");

                entity.Property(e => e.Rating)
                    .HasColumnName("rating");

                entity.HasOne(d => d.Utilizator)
                    .WithMany()
                    .HasForeignKey(d => d.UserID)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_data_utilizatori_user_id");

                entity.HasOne(d => d.Item)
                    .WithMany()
                    .HasForeignKey(d => d.LocationID)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_data_item_location_id");

                // Configuration for Data entity...
            });
        }
    }
