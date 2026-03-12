using BookAPI.DTOs;
using BookAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookAPI.Services
{
    public interface IBookService
    {
        Task<List<Book>> GetAllAsync();
        Task<Book?> GetByIdAsync(int id);
        Task<Book> CreateAsync(CreateBookDto dto);
        Task<bool> UpdateAsync(int id, UpdateBookDto dto);
        Task<bool> DeleteAsync(int id);
        Task<bool> IsbnExistsAsync(string isbn, int? excludeId = null);
    }

    public class BookService : IBookService
    {
        private readonly List<Book> _books;
        private int _nextId;
        private readonly object _lock = new object();
        private readonly ILogger<BookService> _logger;

        public BookService(ILogger<BookService> logger)
        {
            _logger = logger;
            _books = new List<Book>();
            _nextId = 1;
            InitializeSampleData();
        }

        private void InitializeSampleData()
        {
            lock (_lock)
            {
                var sampleBooks = new[]
                {
                    new Book
                    {
                        Id = _nextId++,
                        Title = "The Great Gatsby",
                        Author = "F. Scott Fitzgerald",
                        ISBN = "978-0-7432-7356-5",
                        PublicationDate = new DateTime(1925, 4, 10),
                        CreatedAt = DateTime.UtcNow
                    },
                    new Book
                    {
                        Id = _nextId++,
                        Title = "To Kill a Mockingbird",
                        Author = "Harper Lee",
                        ISBN = "978-0-06-112008-4",
                        PublicationDate = new DateTime(1960, 7, 11),
                        CreatedAt = DateTime.UtcNow
                    },
                    new Book
                    {
                        Id = _nextId++,
                        Title = "1984",
                        Author = "George Orwell",
                        ISBN = "978-0-452-28423-4",
                        PublicationDate = new DateTime(1949, 6, 8),
                        CreatedAt = DateTime.UtcNow
                    },
                    new Book
                    {
                        Id = _nextId++,
                        Title = "Pride and Prejudice",
                        Author = "Jane Austen",
                        ISBN = "978-0-14-143951-8",
                        PublicationDate = new DateTime(1813, 1, 28),
                        CreatedAt = DateTime.UtcNow
                    },
                    new Book
                    {
                        Id = _nextId++,
                        Title = "The Catcher in the Rye",
                        Author = "J.D. Salinger",
                        ISBN = "978-0-316-76948-0",
                        PublicationDate = new DateTime(1951, 7, 16),
                        CreatedAt = DateTime.UtcNow
                    }
                };

                _books.AddRange(sampleBooks);
                _logger.LogInformation($"Initialized {_books.Count} sample books");
            }
        }

        public Task<List<Book>> GetAllAsync()
        {
            lock (_lock)
            {
                return Task.FromResult(_books.OrderBy(b => b.Id).ToList());
            }
        }

        public Task<Book?> GetByIdAsync(int id)
        {
            lock (_lock)
            {
                var book = _books.FirstOrDefault(b => b.Id == id);
                return Task.FromResult(book);
            }
        }

        public async Task<bool> IsbnExistsAsync(string isbn, int? excludeId = null)
        {
            if (string.IsNullOrWhiteSpace(isbn))
                return false;

            lock (_lock)
            {
                return _books.Any(b => 
                    b.ISBN == isbn.Trim() && 
                    (!excludeId.HasValue || b.Id != excludeId.Value));
            }
        }

        public async Task<Book> CreateAsync(CreateBookDto dto)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto));

            if (string.IsNullOrWhiteSpace(dto.Title))
                throw new ArgumentException("Title is required", nameof(dto.Title));

            if (string.IsNullOrWhiteSpace(dto.Author))
                throw new ArgumentException("Author is required", nameof(dto.Author));

            lock (_lock)
            {
                // Check for duplicate ISBN if provided
                if (!string.IsNullOrWhiteSpace(dto.ISBN))
                {
                    var existingBook = _books.FirstOrDefault(b => b.ISBN == dto.ISBN.Trim());
                    if (existingBook != null)
                    {
                        throw new InvalidOperationException($"A book with ISBN {dto.ISBN} already exists.");
                    }
                }

                var book = new Book
                {
                    Id = _nextId++,
                    Title = dto.Title.Trim(),
                    Author = dto.Author.Trim(),
                    ISBN = dto.ISBN?.Trim(),
                    PublicationDate = dto.PublicationDate,
                    CreatedAt = DateTime.UtcNow
                };

                _books.Add(book);
                _logger.LogInformation($"Book created: ID={book.Id}, Title={book.Title}");
                
                return book;
            }
        }

        public async Task<bool> UpdateAsync(int id, UpdateBookDto dto)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto));

            if (string.IsNullOrWhiteSpace(dto.Title))
                throw new ArgumentException("Title is required", nameof(dto.Title));

            if (string.IsNullOrWhiteSpace(dto.Author))
                throw new ArgumentException("Author is required", nameof(dto.Author));

            lock (_lock)
            {
                var book = _books.FirstOrDefault(b => b.Id == id);
                if (book == null) return false;

                // Check for duplicate ISBN if changed
                if (!string.IsNullOrWhiteSpace(dto.ISBN) && dto.ISBN.Trim() != book.ISBN)
                {
                    var existingBook = _books.FirstOrDefault(b => 
                        b.ISBN == dto.ISBN.Trim() && b.Id != id);
                    
                    if (existingBook != null)
                    {
                        throw new InvalidOperationException($"A book with ISBN {dto.ISBN} already exists.");
                    }
                }

                book.Title = dto.Title.Trim();
                book.Author = dto.Author.Trim();
                book.ISBN = dto.ISBN?.Trim();
                book.PublicationDate = dto.PublicationDate;
                book.UpdatedAt = DateTime.UtcNow;

                _logger.LogInformation($"Book updated: ID={book.Id}");
                return true;
            }
        }

        public async Task<bool> DeleteAsync(int id)
        {
            lock (_lock)
            {
                var book = _books.FirstOrDefault(b => b.Id == id);
                if (book == null) return false;

                var removed = _books.Remove(book);
                if (removed)
                {
                    _logger.LogInformation($"Book deleted: ID={id}");
                }
                
                return removed;
            }
        }
    }
}