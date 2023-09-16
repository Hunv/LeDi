﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using LeDi.Shared2.DatabaseModel;

namespace LeDi.Server2.DatabaseModel
{
    public class LeDiDbContext : IdentityDbContext
    {
        public DbSet<TblMatch>? TblMatches { get; set; }
        public DbSet<TblMatchEvent>? TblMatchEvents { get; set; }
        public DbSet<TblMatchPenalty>? TblMatchPenalties { get; set; }
        public DbSet<TblMatchReferee>? TblMatchReferees { get; set; }
        public DbSet<TblSetting> TblSettings { get; set; }
        public DbSet<TblDevice>? TblDevice { get; set; }
        public DbSet<TblDeviceSetting>? TblDeviceSettings { get; set; }
        public DbSet<TblDeviceCommand>? TblDeviceCommands { get; set; }
        public DbSet<TblPlayer>? TblPlayers { get; set; }
        public DbSet<TblPlayer2Match>? TblPlayer2Matches { get; set; }
        public DbSet<TblDevice2Match>? TblDevice2Matches { get; set; }
        public DbSet<TblDevice2Match>? TblDevice2Tournaments { get; set; }
        public DbSet<TblTemplatePenaltyItem> TblTemplatePenaltyItems { get; set; }
        public DbSet<TblTemplatePenaltyText> TblTemplatePenaltyTexts { get; set; }
        public DbSet<TblTournament> TblTournaments { get; set; }
        public DbSet<TblUserRole> TblUserRoles { get; set; }
        public DbSet<TblTemplate> TblTemplates { get; set; }

        public LeDiDbContext(DbContextOptions<LeDiDbContext> options)
            : base(options)
        {
        }

        public LeDiDbContext() : base() { }

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
            modelBuilder.Entity<TblPlayer2Match>().ToTable("Player2Matches");
            modelBuilder.Entity<TblPlayer2Match>(entity =>
            {
                entity.HasKey(e => new { e.PlayerId, e.MatchId });
                entity.HasOne(e => e.Player)
                    .WithMany(e => e.MatchList);
            });


            // Devices to Matches
            modelBuilder.Entity<TblDevice2Match>().ToTable("Device2Matches");
            modelBuilder.Entity<TblDevice2Match>(entity =>
            {
                entity.HasKey(e => new { e.DeviceId, e.MatchId });
                entity.HasOne(e => e.Device)
                    .WithMany(e => e.MatchList);
            });

            // Devices to Tournaments (for the default config for matches)
            modelBuilder.Entity<TblDevice2Tournament>().ToTable("Device2Tournaments");
            modelBuilder.Entity<TblDevice2Tournament>(entity =>
            {
                entity.HasKey(e => new { e.DeviceId, e.TournamentId });
                entity.HasOne(e => e.Device)
                    .WithMany(e => e.TournamentList);
            });


            // Matchevents to matches
            //modelBuilder.Entity<Match>(entity =>
            //{
            //    entity.HasKey(e => e.Id);
            //    entity.HasMany(e => e.MatchEvents);
            //});

            // Primary key for DeviceSettings
            modelBuilder.Entity<TblDeviceSetting>(entity =>
            {
                entity.HasKey(new string[] { "SettingName", "DeviceId" });
            });

            base.OnModelCreating(modelBuilder);
        }

    }
}
