using Library.Application.DTOs;
using Library.Application.Services;
using Library.Core.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using static System.Reflection.Metadata.BlobBuilder;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Library.API.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class BooksController : ControllerBase
    {
        private readonly IBookService _bookService;
        private readonly ILogger<BooksController> _logger;
        public BooksController(IBookService bookService, ILogger<BooksController> logger)
        {
            _bookService = bookService;
            _logger = logger;
        }
        // GET: api/books - Get all books (public)
        [HttpGet]
        [AllowAnonymous] // Anyone can view books
        public async Task<ActionResult<IEnumerable<BookDto>>> GetAll()
        {
            try
            {
                var books = await _bookService.GetAllBooksAsync();
                return Ok(books);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all books");
                return StatusCode(500, "Internal server error");
            }

        }

        [HttpPost("{id}")]
        public async Task<ActionResult> GetBookByIdAsync(int id)
        {
            try
            {
                var books = await _bookService.ReturnBookAsync(id);
                if (!books)
                    return BadRequest("Book not found");
                return Ok(new { message = "Book returned successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting id");
                return StatusCode(500, "Internal server error");
            }

        }
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<BookDto>> CreateBook(CreateBookDto createBookDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var book = await _bookService.CreateBookAsync(createBookDto);
                return CreatedAtAction(nameof(GetBookByIdAsync), new { id = book.Id }, book);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating book");
                return StatusCode(500, "Internal server error");
            }

        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateBook(int id, UpdateBookDto updateBookDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                await _bookService.UpdateBookAsync(id, updateBookDto);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"error updating book id{id}", id);
                return StatusCode(500, "Internal server error");
            }

        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteBook(int id)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);
                await _bookService.DeleteBookAsync(id);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }

            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting book {id}", id);
                return StatusCode(500, "Internal Server Error");
            }
        }

        [HttpGet("Search Book")]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<BookDto>>> Search([FromQuery] string term)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(term))
                    return BadRequest("Search term cannot be empty");

                var books = await _bookService.SearchBooksAsync(term);
                return Ok(books);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching books with term {Term}", term);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("Search by Author")]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<BookDto>>> SearchByAuthor(string author)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(author))
                    return BadRequest("Search author cannot be empty");

                var books = await _bookService.GetBooksByAuthorAsync(author);
                return Ok(books);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching author with term {author}", author);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("available")]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<BookDto>>> GetAvailableBooks()
        {
            try
            {
                var books = await _bookService.GetAvailableBooksAsync();
                return Ok(books);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting available books");
                return StatusCode(500, "Internal server error");

            }
        }

        [HttpPost("{id}/borrow")]
        public async Task<IActionResult> Borrow(int id)
        {
            try
            {
                var result = await _bookService.BorrowBookAsync(id);
                if (!result)
                    return BadRequest("Book not available or not found");

                return Ok(new { message = "Book borrowed successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error borrowing book {Id}", id);
                return StatusCode(500, "Internal server error");
            }

        }
        
        [HttpPost("{id}/return")]
        public async Task<IActionResult> Return(int id)
        {
            try
            {
                var result = await _bookService.ReturnBookAsync(id);
                if (!result)
                    return BadRequest("Book not found");

                return Ok(new { message = "Book returned successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error returning book {Id}", id);
                return StatusCode(500, "Internal server error");
            }
        }
    }
}

    
