using Library.Core.Entities;
using Library.Core.Interfaces;
using Library.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.Infrastructure.Repositories
{
    public class BookRepository:Repository<Book>,IBookRepository
    {
        public BookRepository(LibraryDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Book>> GetAvailableBooksAsync()
        {
            return await _dbSet
                .Where(b => b.AvailableCopies > 0 && b.IsActive)
                .ToListAsync();

        }

        public async Task<IEnumerable<Book>> GetByAuthorAsync(string author)
        {
            return await _dbSet
                 .Where(b => b.Author.Contains(author) && b.IsActive)
                 .ToListAsync();
        }

        public async Task<IEnumerable<Book>> SearchAsync(string searchTerm)
        {
            return await _dbSet
                .Where(s=>s.Title.Contains(searchTerm) ||
                           s.Author.Contains(searchTerm))
                .Where(s => s.IsActive).ToListAsync();
        }

        public async Task<bool> UpdateAvailableCopiesAsync(int bookId, int changeAmount)
        {
            var book = await _dbSet.FindAsync(bookId);
            if (book == null)
                return false;

            // Update available copies (positive for borrow, negative for return)
            book.AvailableCopies += changeAmount;
            book.UpdatedAt = DateTime.UtcNow;

            await UpdateAsync(book);
            return true;

        }
    }
}
