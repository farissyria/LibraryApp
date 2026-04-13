using Library.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.Application.Services
{
    public interface IBookService
    {
        Task<IEnumerable<BookDto>> GetAllBooksAsync();
        Task<BookDto?> GetBookByIdAsync(int id);
        Task<BookDto> CreateBookAsync(CreateBookDto createDto);
        Task UpdateBookAsync(int id, UpdateBookDto updateDto);
        Task DeleteBookAsync(int id);
        Task<IEnumerable<BookDto>> SearchBooksAsync(string searchTerm);
        Task<IEnumerable<BookDto>> GetBooksByAuthorAsync(string author);
        Task<IEnumerable<BookDto>> GetAvailableBooksAsync();
        Task<bool> BorrowBookAsync(int bookId);
        Task<bool> ReturnBookAsync(int bookId);

    }
}
