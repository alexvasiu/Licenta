using System;
using System.IO;
using EntityFrameworkCore.Triggers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MusicIdentifierAPI.Domain;

namespace MusicIdentifierAPI
{
    public class DatabaseContext : DbContextWithTriggers
    {
        public IConfiguration Configuration { get; }

        public DatabaseContext(DbContextOptions options, IConfiguration configuration) : base(options)
        {
            Configuration = configuration;
        }

        public DatabaseContext()
        {

        }

        public DbSet<User> User { get; set; }
        public DbSet<Song> Song { get; set; }
        public DbSet<SongPart> SongPart { get; set; }
        public DbSet<Playlist> Playlist { get; set; }
        public DbSet<SongPlaylist> SongPlaylist { get; set; }
        protected override void OnModelCreating(ModelBuilder model)
        {
            model.Entity<SongPart>()
                .HasIndex(x => x.Points);
            model.Entity<SongPart>()
                .HasIndex(x => new {x.Points, x.Time});
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (Configuration != null)
            {
                var appSettingsSection = Configuration.GetSection("AppSettings");
                var appSettings = appSettingsSection.Get<AppSettings>();
                optionsBuilder.UseNpgsql(appSettings.ConnectionString);
            }
            else
            {
                var currentDirectory = Directory.GetCurrentDirectory();
                var builder = new ConfigurationBuilder().SetBasePath(
                        currentDirectory.Contains("MusicIdentifierAPI") ? currentDirectory
                            : Path.Combine(currentDirectory, @"..\..\..\..\MusicIdentifierAPI"))
                    .AddJsonFile("appsettings.json");
                var configuration = builder.Build();
                var appSettingsSection = configuration.GetSection("AppSettings");
                var appSettings = appSettingsSection.Get<AppSettings>();
                optionsBuilder.UseNpgsql(appSettings.ConnectionString);
            }
        }
    }
}
