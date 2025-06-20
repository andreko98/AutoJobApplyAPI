﻿using AutoJobApplyDatabase.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace AutoJobApplyDatabase.Context
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Application> Applications { get; set; }
        public DbSet<Job> Jobs { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<EmailLog> EmailLogs { get; set; }
        public DbSet<ExternalApiKey> ExternalApiKeys { get; set; }
        public DbSet<EmailCredential> EmailCredentials { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new ApplicationMap());
            modelBuilder.ApplyConfiguration(new JobMap());
            modelBuilder.ApplyConfiguration(new UserMap());
            modelBuilder.ApplyConfiguration(new EmailLogMap());
            modelBuilder.ApplyConfiguration(new ExternalApiKeyMap());
            modelBuilder.ApplyConfiguration(new EmailCredentialsMap());
        }
    }
}