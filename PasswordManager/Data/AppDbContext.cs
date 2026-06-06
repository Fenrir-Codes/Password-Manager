using Microsoft.EntityFrameworkCore;
using PasswordManager.Models;
using System;
using System.IO;

namespace PasswordManager.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<PasswordEntry> Passwords { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // Lekérjük a futó PasswordManager.exe pontos mappáját
            string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;

            // Összekötjük a mappa útvonalát az adatbázis nevével
            string dbPath = Path.Combine(baseDirectory, "pmdb.db");

            // Átadjuk az SQLite-nak
            optionsBuilder.UseSqlite($"Data Source={dbPath}");
        }
    }
}