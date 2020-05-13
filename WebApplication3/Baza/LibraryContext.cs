using Microsoft.EntityFrameworkCore;
using MySql.Data.EntityFrameworkCore.Extensions;
using System.Security.Policy;
using WebApplication3.Models;

namespace WebApplication3.Baza
{
    public class LibraryContext : DbContext
    {
        public DbSet<Book> books { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseMySQL("server=localhost; port=3306; database=new_schema; user=root; password=1q2w3e4rPoi");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Book>(entity =>
            {
                entity.HasKey(e => e.BooksID);
                entity.Property(e => e.Title).IsRequired();
                entity.Property(e => e.Author).IsRequired();
                entity.Property(e => e.Genre).IsRequired();
                entity.Property(e => e.Available).IsRequired();
                entity.Property(e => e.Edition).IsRequired();
            });
        }
    }
}