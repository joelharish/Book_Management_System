export interface Book {
  id: number;
  title: string;
  author: string;
  isbn: string;
  publicationDate: Date;
}

export interface CreateBookDto {
  title: string;
  author: string;
  isbn: string;
  publicationDate: Date;
}

export interface UpdateBookDto {
  title: string;
  author: string;
  isbn: string;
  publicationDate: Date;
}

export interface ApiResponse<T> {
  success: boolean;
  message: string;
  data: T;
  errors: string[];
}