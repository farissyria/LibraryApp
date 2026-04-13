using Library.Core.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.Infrastructure.Data
{
    public class LibraryDbContext : IdentityDbContext<User>
    {
        public LibraryDbContext(DbContextOptions<LibraryDbContext> options) : base(options)
        {

        }
        public DbSet<Book> Books { get; set; }
        protected override void OnModelCreating(ModelBuilder mb)
        {
            base.OnModelCreating(mb);
            mb.Entity<Book>(entity =>
            {

                // ISBN must be unique
                entity.HasIndex(b => b.ISBN).IsUnique();

                // Add index for faster searching
                entity.HasIndex(b => b.Author);
                entity.HasIndex(b => b.Title);

                // Configure decimal precision (though no decimals here)
                // Configure default values
                entity.Property(b => b.AvailableCopies).HasDefaultValue(0);
                entity.Property(b => b.TotalCopies).HasDefaultValue(0);
                entity.Property(b => b.IsActive).HasDefaultValue(true);

            });
        }
            //seeding data
            private void SeedData(ModelBuilder mb)
            {
            mb.Entity<Book>().HasData(
                
                new Book 
                { 
                Id = 1,
                    Title = "Clean Code",
                    Author = "Robert C. Martin",
                    ISBN = "9780132350884",
                    PublicationYear = 2008,
                    TotalCopies = 5,
                    AvailableCopies = 3,
                    Description = "A handbook of agile software craftsmanship"
                },
                new Book
                {
                    Id = 2,
                    Title = "Design Patterns",
                    Author = "Erich Gamma",
                    ISBN = "9780201633610",
                    PublicationYear = 1994,
                    TotalCopies = 3,
                    AvailableCopies = 2,
                    Description = "Elements of reusable object-oriented software"
                }
            );

             }
                
            
        
    }
}
