﻿using Microsoft.EntityFrameworkCore;

namespace WebApplication1.Entities
{
    public class RestaurantDbContext : DbContext
    {
        private string _connectionString = "Server=(LocalDB)\\MSSQLLocalDB;Database=RestaurantDb;Integrated Security=true;";
        public DbSet<Restaurant> Restaurants { get; set; }
        public DbSet<Address> Address { get; set; }
        public DbSet<Dish> Dishes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Restaurant>()
                .Property(r => r.Name)
                .IsRequired()
                .HasMaxLength(25);
            modelBuilder.Entity<Dish>()
                .Property(d => d.Name)
                .IsRequired();
            modelBuilder.Entity<Address>()
                .Property(p => p.City)
                .IsRequired()
                .HasMaxLength(50);
            modelBuilder.Entity<Address>()
                .Property(p => p.Street)
                .IsRequired()
                .HasMaxLength(50);
                


        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(_connectionString);
        }
    }
}
