using Microsoft.EntityFrameworkCore;
using MusicIdentifierAPI.Domain;

namespace MusicIdentifierAPI
{
    public class DatabaseContext : DbContext
    {
        public DatabaseContext(DbContextOptions options) : base(options)
        {

        }

        public DatabaseContext()
        {

        }

        public DbSet<User> User { get; set; }
        public DbSet<Song> Song { get; set; }
        public DbSet<SongPart> SongPart { get; set; }
        public DbSet<Playlist> Playlist { get; set; }

        protected override void OnModelCreating(ModelBuilder model)
        {

        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql("Host=localhost;Database=MusicIdentifierDB;Username=postgres;Password=admin");
        }
    }
}
