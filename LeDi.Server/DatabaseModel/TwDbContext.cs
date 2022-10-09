using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LeDi.Server.DatabaseModel
{
    public class TwDbContext : DbContext
    {
        public DbSet<Match>? Matches { get; set; }
        public DbSet<MatchEvent>? MatchEvents { get; set; }
        public DbSet<Setting>? Settings { get; set; }
        public DbSet<Device>? Device { get; set; }
        public DbSet<DeviceSetting>? DeviceSettings { get; set; }
        public DbSet<DeviceCommand>? DeviceCommands { get; set; }
        public DbSet<Player>? Players { get; set; }
        public DbSet<Player2Match>? Player2Matches { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Filename=LeDi.db", options =>
            {
                options.MigrationsAssembly(Assembly.GetExecutingAssembly().FullName);
            });
            base.OnConfiguring(optionsBuilder);
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Map table names

            // Players to Matches
            modelBuilder.Entity<Player2Match>().ToTable("Player2Matches");
            modelBuilder.Entity<Player2Match>(entity =>
            {
                entity.HasKey(e => new { e.PlayerId, e.MatchId });
                entity.HasOne(e => e.Player)
                    .WithMany(e => e.MatchList);
            });

            // Matchevents to matches
            //modelBuilder.Entity<Match>(entity =>
            //{
            //    entity.HasKey(e => e.Id);
            //    entity.HasMany(e => e.MatchEvents);
            //});

            // Primary key for DeviceSettings
            modelBuilder.Entity<DeviceSetting>(entity =>
            {
                entity.HasKey(new string[] { "SettingName", "DeviceId" });
            });

            //base.OnModelCreating(modelBuilder);
        }

    }
}
