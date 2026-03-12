import { Component, OnInit, ChangeDetectorRef, NgZone } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { BookService } from '../../services/book.service';
import { Book, CreateBookDto } from '../../models/book.model';

@Component({
  selector: 'app-book-list',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './book-list.component.html',
  styleUrls: ['./book-list.component.css']
})
export class BookListComponent implements OnInit {
  books: Book[] = [];
  showForm = false;
  editingBook: Book | null = null;
  loading = false;
  errorMessage = '';
  successMessage = '';
  confirmDeleteId: number | null = null;
  deleteBookTitle = '';

  // Form data
  formData: CreateBookDto = {
    title: '',
    author: '',
    isbn: '',
    publicationDate: new Date()
  };

  constructor(
    private bookService: BookService,
    private cdr: ChangeDetectorRef,
    private ngZone: NgZone
  ) {}

  ngOnInit(): void {
    this.loadBooks();
  }

  loadBooks(): void {
    this.loading = true;
    this.cdr.detectChanges();
    this.bookService.getAllBooks().subscribe({
      next: (response: any) => {
        this.ngZone.run(() => {
          // Handle both standard array response and ApiResponse format
          const data = response.data !== undefined ? response.data : response;
          this.books = Array.isArray(data) ? data : [];
          this.loading = false;
          this.cdr.detectChanges();
        });
      },
      error: (error) => {
        this.ngZone.run(() => {
          this.errorMessage = 'Failed to load books';
          this.loading = false;
          this.cdr.detectChanges();
          console.error(error);
        });
      }
    });
  }

  openAddForm(): void {
    this.showForm = true;
    this.editingBook = null;
    this.formData = {
      title: '',
      author: '',
      isbn: '',
      publicationDate: new Date()
    };
    this.cdr.detectChanges();
  }

  openEditForm(book: Book): void {
    this.showForm = true;
    this.editingBook = book;
    this.formData = {
      title: book.title,
      author: book.author,
      isbn: book.isbn,
      publicationDate: new Date(book.publicationDate)
    };
    this.cdr.detectChanges();
  }

  closeForm(): void {
    this.showForm = false;
    this.editingBook = null;
    this.errorMessage = '';
    this.cdr.detectChanges();
  }

  saveBook(): void {
    if (!this.formData.title || !this.formData.author) {
      this.errorMessage = 'Title and Author are required';
      return;
    }

    this.loading = true;
    this.cdr.detectChanges();

    if (this.editingBook) {
      // Update existing book
      this.bookService.updateBook(this.editingBook.id, this.formData).subscribe({
        next: () => {
          this.ngZone.run(() => {
            this.successMessage = 'Book updated successfully';
            this.closeForm();
            this.loadBooks();
            this.cdr.detectChanges();
            setTimeout(() => { this.successMessage = ''; this.cdr.detectChanges(); }, 3000);
          });
        },
        error: (error) => {
          this.ngZone.run(() => {
            this.errorMessage = 'Failed to update book';
            this.loading = false;
            this.cdr.detectChanges();
            console.error(error);
          });
        }
      });
    } else {
      // Create new book
      this.bookService.createBook(this.formData).subscribe({
        next: () => {
          this.ngZone.run(() => {
            this.successMessage = 'Book created successfully';
            this.closeForm();
            this.loadBooks();
            this.cdr.detectChanges();
            setTimeout(() => { this.successMessage = ''; this.cdr.detectChanges(); }, 3000);
          });
        },
        error: (error) => {
          this.ngZone.run(() => {
            this.errorMessage = 'Failed to create book';
            this.loading = false;
            this.cdr.detectChanges();
            console.error(error);
          });
        }
      });
    }
  }

  requestDelete(id: number, title: string): void {
    this.confirmDeleteId = id;
    this.deleteBookTitle = title;
    this.cdr.detectChanges();
  }

  cancelDelete(): void {
    this.confirmDeleteId = null;
    this.cdr.detectChanges();
  }

  confirmDelete(id: number): void {
    this.confirmDeleteId = null;
    this.loading = true;
    this.cdr.detectChanges();
    this.bookService.deleteBook(id).subscribe({
      next: () => {
        this.ngZone.run(() => {
          this.successMessage = 'Book deleted successfully';
          this.loadBooks();
          this.cdr.detectChanges();
          setTimeout(() => { this.successMessage = ''; this.cdr.detectChanges(); }, 3000);
        });
      },
      error: (error) => {
        this.ngZone.run(() => {
          this.errorMessage = 'Failed to delete book';
          this.loading = false;
          this.cdr.detectChanges();
          console.error(error);
        });
      }
    });
  }

  formatDate(date: Date): string {
    return new Date(date).toLocaleDateString();
  }
}