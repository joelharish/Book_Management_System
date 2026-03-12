using System;
using System.ComponentModel.DataAnnotations;

namespace BookAPI.Models
{
    public class Book
    {
        public int Id { get; set; }
        
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
        
        // Audit fields
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}