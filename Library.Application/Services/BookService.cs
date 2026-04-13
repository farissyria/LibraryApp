using AutoMapper;
using Library.Application.DTOs;
using Library.Core.Entities;
using Library.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Reflection.Metadata.BlobBuilder;

namespace Library.Application.Services
{
    public class BookService : IBookService
    {
        private readonly IBookRepository _bookRepository;
        private readonly IMapper _mapper;
        public BookService(IBookRepository bookRepository, IMapper mapper)
        {
            _bookRepository= bookRepository;
            _mapper= mapper;
        }
        public async Task<bool> BorrowBookAsync(int bookId)
        {
            // Decrease available copies by 1 when borrowing
            return await _bookRepository.UpdateAvailableCopiesAsync(bookId, -1);

        }

        public async Task<BookDto> CreateBookAsync(CreateBookDto createDto)
        {
           var book=  _mapper.Map<Book>(createDto);
            // Set initial available copies equal to total copies
            book.AvailableCopies = createDto.TotalCopies;
            book.CreatedAt = DateTime.UtcNow;
            book.IsActive = true;
            var created=await _bookRepository.AddAsync(book);
            return _mapper.Map<BookDto>(created);
        }

        public async Task DeleteBookAsync(int id)
        {
            var book=await _bookRepository.GetById(id);
            if(book ==null)
                throw new KeyNotFoundException($"Book with id {id} not found");
            book.IsActive=false;
            await _bookRepository.UpdateAsync(book);
        }

        public async Task<IEnumerable<BookDto>> GetAllBooksAsync()
        {
            var books=await _bookRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<BookDto>>(books); 
        }

        public async Task<IEnumerable<BookDto>> GetAvailableBooksAsync()
        {
            var books = await _bookRepository.GetAvailableBooksAsync();
            return  _mapper.Map<IEnumerable<BookDto>>(books);
        }

        public async Task<BookDto?> GetBookByIdAsync(int id)
        {
            var book = await _bookRepository.GetById(id);
            return _mapper.Map<BookDto>(book);
        }

        public async Task<IEnumerable<BookDto>> GetBooksByAuthorAsync(string author)
        {
           var book=await _bookRepository.GetByAuthorAsync(author);
            return _mapper.Map<IEnumerable<BookDto>>(book);
        }

        public async Task<bool> ReturnBookAsync(int bookId)
        {
            // Increase available copies by 1 when returning
            return await _bookRepository.UpdateAvailableCopiesAsync(bookId, 1);

        }

        public async Task<IEnumerable<BookDto>> SearchBooksAsync(string searchTerm)
        {
            var book = await _bookRepository.SearchAsync(searchTerm);
            return _mapper.Map<IEnumerable<BookDto>>(book);
        }

        public async Task UpdateBookAsync(int id, UpdateBookDto updateDto)
        {
            var book = await _bookRepository.GetById(id);
            if (book == null)
                throw new KeyNotFoundException($"Book with id {id} not found");

            // Update book properties
            _mapper.Map(updateDto, book);

            // Recalculate available copies if total copies changed
            if (book.TotalCopies != updateDto.TotalCopies)
            {
                var difference = updateDto.TotalCopies - book.TotalCopies;
                book.AvailableCopies += difference;
                book.TotalCopies = updateDto.TotalCopies;
            }

            book.UpdatedAt = DateTime.UtcNow;
            await _bookRepository.UpdateAsync(book);

        }
    }
}
