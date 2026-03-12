using Microsoft.AspNetCore.Mvc;
using BookAPI.DTOs;
using BookAPI.Services;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace BookAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class BooksController : ControllerBase
    {
        private readonly IBookService _bookService;
        private readonly ILogger<BooksController> _logger;

        public BooksController(IBookService bookService, ILogger<BooksController> logger)
        {
            _bookService = bookService;
            _logger = logger;
        }

        // GET: api/books
        [HttpGet]
        public async Task<ActionResult<ApiResponse<object>>> GetBooks()
        {
            try
            {
                var books = await _bookService.GetAllAsync();
                var response = books.Select(b => new BookResponseDto
                {
                    Id = b.Id,
                    Title = b.Title,
                    Author = b.Author,
                    ISBN = b.ISBN,
                    PublicationDate = b.PublicationDate,
                    CreatedAt = b.CreatedAt,
                    UpdatedAt = b.UpdatedAt
                }).ToList();

                return Ok(ApiResponse<object>.SuccessResponse(
                    response, 
                    $"Successfully retrieved {response.Count} books"
                ));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all books");
                return StatusCode(500, ApiResponse<object>.ErrorResponse(
                    "An error occurred while retrieving books",
                    ex.Message
                ));
            }
        }

        // GET: api/books/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<object>>> GetBook(int id)
        {
            try
            {
                var book = await _bookService.GetByIdAsync(id);
                
                if (book == null)
                {
                    return NotFound(ApiResponse<object>.ErrorResponse(
                        $"Book with ID {id} not found",
                        "Book not found"
                    ));
                }

                var response = new BookResponseDto
                {
                    Id = book.Id,
                    Title = book.Title,
                    Author = book.Author,
                    ISBN = book.ISBN,
                    PublicationDate = book.PublicationDate,
                    CreatedAt = book.CreatedAt,
                    UpdatedAt = book.UpdatedAt
                };

                return Ok(ApiResponse<object>.SuccessResponse(response, "Book retrieved successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting book with ID {id}");
                return StatusCode(500, ApiResponse<object>.ErrorResponse(
                    "An error occurred while retrieving the book",
                    ex.Message
                ));
            }
        }

        // POST: api/books
        [HttpPost]
        public async Task<ActionResult<ApiResponse<object>>> CreateBook([FromBody] CreateBookDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList();

                    return BadRequest(ApiResponse<object>.ErrorResponse(
                        "Validation failed",
                        errors
                    ));
                }

                var book = await _bookService.CreateAsync(dto);
                
                var response = new BookResponseDto
                {
                    Id = book.Id,
                    Title = book.Title,
                    Author = book.Author,
                    ISBN = book.ISBN,
                    PublicationDate = book.PublicationDate,
                    CreatedAt = book.CreatedAt,
                    UpdatedAt = book.UpdatedAt
                };

                return CreatedAtAction(
                    nameof(GetBook), 
                    new { id = book.Id }, 
                    ApiResponse<object>.SuccessResponse(response, "Book created successfully")
                );
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ApiResponse<object>.ErrorResponse("Invalid input", ex.Message));
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ApiResponse<object>.ErrorResponse("Duplicate ISBN", ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating book");
                return StatusCode(500, ApiResponse<object>.ErrorResponse(
                    "An error occurred while creating the book",
                    ex.Message
                ));
            }
        }

        // PUT: api/books/5
        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponse<object>>> UpdateBook(int id, [FromBody] UpdateBookDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList();

                    return BadRequest(ApiResponse<object>.ErrorResponse(
                        "Validation failed",
                        errors
                    ));
                }

                var updated = await _bookService.UpdateAsync(id, dto);
                
                if (!updated)
                {
                    return NotFound(ApiResponse<object>.ErrorResponse(
                        $"Book with ID {id} not found",
                        "Book not found"
                    ));
                }

                var book = await _bookService.GetByIdAsync(id);
                var response = new BookResponseDto
                {
                    Id = book!.Id,
                    Title = book.Title,
                    Author = book.Author,
                    ISBN = book.ISBN,
                    PublicationDate = book.PublicationDate,
                    CreatedAt = book.CreatedAt,
                    UpdatedAt = book.UpdatedAt
                };

                return Ok(ApiResponse<object>.SuccessResponse(response, "Book updated successfully"));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ApiResponse<object>.ErrorResponse("Invalid input", ex.Message));
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ApiResponse<object>.ErrorResponse("Duplicate ISBN", ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating book with ID {id}");
                return StatusCode(500, ApiResponse<object>.ErrorResponse(
                    "An error occurred while updating the book",
                    ex.Message
                ));
            }
        }

        // DELETE: api/books/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponse<object>>> DeleteBook(int id)
        {
            try
            {
                var deleted = await _bookService.DeleteAsync(id);
                
                if (!deleted)
                {
                    return NotFound(ApiResponse<object>.ErrorResponse(
                        $"Book with ID {id} not found",
                        "Book not found"
                    ));
                }

                return Ok(ApiResponse<object>.SuccessResponse(null, "Book deleted successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting book with ID {id}");
                return StatusCode(500, ApiResponse<object>.ErrorResponse(
                    "An error occurred while deleting the book",
                    ex.Message
                ));
            }
        }

        // GET: api/books/search?term=gatsby
        [HttpGet("search")]
        public async Task<ActionResult<ApiResponse<object>>> SearchBooks([FromQuery] string term)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(term))
                {
                    return BadRequest(ApiResponse<object>.ErrorResponse(
                        "Search term is required",
                        "Invalid search parameter"
                    ));
                }

                var books = await _bookService.GetAllAsync();
                var searchTerm = term.ToLowerInvariant();
                
                var results = books.Where(b => 
                    b.Title.ToLowerInvariant().Contains(searchTerm) ||
                    b.Author.ToLowerInvariant().Contains(searchTerm) ||
                    (b.ISBN != null && b.ISBN.Contains(searchTerm))
                ).Select(b => new BookResponseDto
                {
                    Id = b.Id,
                    Title = b.Title,
                    Author = b.Author,
                    ISBN = b.ISBN,
                    PublicationDate = b.PublicationDate
                }).ToList();

                return Ok(ApiResponse<object>.SuccessResponse(
                    results,
                    $"Found {results.Count} books matching '{term}'"
                ));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error searching books with term '{term}'");
                return StatusCode(500, ApiResponse<object>.ErrorResponse(
                    "An error occurred while searching books",
                    ex.Message
                ));
            }
        }
    }
}