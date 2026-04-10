using Library.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.Core.Interfaces
{
    public interface IBookRepositories:IRepository<Book>
    {
        // Search books by title or author
        Task<IEnumerable<Book>> SearchAsync(string searchTerm);

        // Get books by author name
        Task<IEnumerable<Book>> GetByAuthorAsync(string author);

        // Get available books (copies > 0)
        Task<IEnumerable<Book>> GetAvailableBooksAsync();

        // Update available copies when book is borrowed/returned
        Task<bool> UpdateAvailableCopiesAsync(int bookId, int changeAmount);

    }
}
