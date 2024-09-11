using Microsoft.EntityFrameworkCore;

namespace Homework_02._09._2024_RelationshipsBetweenEntities
{
    class Program
    {
        static void Main(string[] args)
        {
            using (ApplicationContext db = new ApplicationContext())
            {
                db.Database.EnsureDeleted();
                db.Database.EnsureCreated();

                var users = new List<User>
            {
                new User { FirstName = "John", LastName = "Doe", Settings = new UserSettings { Preference = "Dark", Theme = "Modern" } },
                new User { FirstName = "Jane", LastName = "Smith", Settings = new UserSettings { Preference = "Light", Theme = "Classic" } },
                new User { FirstName = "Bob", LastName = "Johnson", Settings = new UserSettings { Preference = "Dark", Theme = "Minimalist" } }
            };

                db.Users.AddRange(users);
                db.SaveChanges();

                var user = db.Users
                             .Include(u => u.Settings)
                             .FirstOrDefault(u => u.UserId == 2);

                if (user != null)
                {
                    Console.WriteLine("User: \t\t" + user.FirstName + " " + user.LastName);
                    Console.WriteLine("Preference: \t" + " " + user.Settings.Preference);
                    Console.WriteLine("Theme: \t\t" + " " + user.Settings.Theme);
                }

                var userToDelete = db.Users
                                     .Include(u => u.Settings)
                                     .FirstOrDefault(u => u.UserId == 3);

                if (userToDelete != null)
                {
                    db.Users.Remove(userToDelete);
                    db.SaveChanges();
                }
            }
        }
    }

    public class User
    {
        public int UserId { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public UserSettings? Settings { get; set; }
    }

    public class UserSettings
    {
        public int UserSettingsId { get; set; }
        public string? Preference { get; set; }
        public string? Theme { get; set; }
        public int UserId { get; set; }
        public User? User { get; set; }
    }

    public class ApplicationContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<UserSettings> UserSettings { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(@"Server=DESKTOP-M496S5I;Database=Ralationships;Trusted_Connection=True; TrustServerCertificate=True");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .HasOne(u => u.Settings)
                .WithOne(us => us.User)
                .HasForeignKey<UserSettings>(us => us.UserId);

            base.OnModelCreating(modelBuilder);
        }
    }

}
