using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BookAPI.DTOs
{
    // For creating a new book
    public class CreateBookDto
    {
        [Required(ErrorMessage = "Title is required")]
        [StringLength(200, MinimumLength = 1, ErrorMessage = "Title must be between 1 and 200 characters")]
        public string Title { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Author is required")]
        [StringLength(100, ErrorMessage = "Author must not exceed 100 characters")]
        public string Author { get; set; } = string.Empty;
        
        [StringLength(20, ErrorMessage = "ISBN must not exceed 20 characters")]
        public string? ISBN { get; set; }
        
        [DataType(DataType.Date)]
        public DateTime PublicationDate { get; set; }
    }

    // For updating a book
    public class UpdateBookDto
    {
        [Required(ErrorMessage = "Title is required")]
        [StringLength(200, MinimumLength = 1, ErrorMessage = "Title must be between 1 and 200 characters")]
        public string Title { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Author is required")]
        [StringLength(100, ErrorMessage = "Author must not exceed 100 characters")]
        public string Author { get; set; } = string.Empty;
        
        [StringLength(20, ErrorMessage = "ISBN must not exceed 20 characters")]
        public string? ISBN { get; set; }
        
        [DataType(DataType.Date)]
        public DateTime PublicationDate { get; set; }
    }

    // For response
    public class BookResponseDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Author { get; set; } = string.Empty;
        public string? ISBN { get; set; }
        public DateTime PublicationDate { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    // API Response Wrapper
    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public T? Data { get; set; }
        public List<string> Errors { get; set; } = new List<string>();
        public DateTime Timestamp { get; set; }

        public ApiResponse()
        {
            Timestamp = DateTime.UtcNow;
        }

        public static ApiResponse<T> SuccessResponse(T data, string message = "Operation completed successfully")
        {
            return new ApiResponse<T>
            {
                Success = true,
                Message = message,
                Data = data,
                Errors = new List<string>(),
                Timestamp = DateTime.UtcNow
            };
        }

        public static ApiResponse<T> ErrorResponse(string message, string error)
        {
            return new ApiResponse<T>
            {
                Success = false,
                Message = message,
                Data = default,
                Errors = new List<string> { error },
                Timestamp = DateTime.UtcNow
            };
        }

        public static ApiResponse<T> ErrorResponse(string message, List<string> errors)
        {
            return new ApiResponse<T>
            {
                Success = false,
                Message = message,
                Data = default,
                Errors = errors ?? new List<string>(),
                Timestamp = DateTime.UtcNow
            };
        }

        public static ApiResponse<T> ErrorResponse(string message, Exception ex)
        {
            return new ApiResponse<T>
            {
                Success = false,
                Message = message,
                Data = default,
                Errors = new List<string> { ex.Message },
                Timestamp = DateTime.UtcNow
            };
        }
    }
}