# Book_Management_System
The Book Management Application is a full-stack web application that allows users to perform CRUD (Create, Read, Update, Delete) operations on a book collection. It provides a clean and intuitive interface for managing book inventory with features like searching, filtering, and real-time updates.


# Book Management Application

A full-stack application for managing a collection of books, featuring a .NET Web API backend and an Angular frontend.

## Project Structure

- **BookAPI**: ASP.NET Core Web API built with .NET 10.0.
- **frontend**: Angular 21 application presenting the user interface.

## Features

- **Add New Books**: Easy-to-use form to add book details (Title, Author, ISBN, Publication Date).
- **Edit Books**: Update existing book information via a modal interface.
- **Delete Books**: Secure deletion with a confirmation modal to prevent accidental data loss.
- **Responsive Design**: Clean and modern UI built with Bootstrap 5.

## Prerequisites

Before running the application, ensure you have the following installed:

- [.NET 10.0 SDK](https://dotnet.microsoft.com/download)
- [Node.js](https://nodejs.org/) (LTS version recommended)
- [npm](https://www.npmjs.com/) (comes with Node.js)

## Installation & Running

### 1. Backend (BookAPI)

1. Navigate to the `BookAPI/BookAPI` directory:
   ```bash
   cd BookAPI/BookAPI
   ```
2. Restore dependencies and run the application:
   ```bash
   dotnet run
   ```
   The API will typically be available at `http://localhost:5000` or `https://localhost:5001`.

### 2. Frontend

1. Navigate to the `frontend` directory:
   ```bash
   cd frontend
   ```
2. Install the necessary dependencies:
   ```bash
   npm install
   ```
3. Start the Angular development server:
   ```bash
   npm start
   ```
4. Access the application in your browser at `http://localhost:4200/`.

## Workflow

1. Start the **BookAPI** backend server first to ensure the frontend can connect to the data source.
2. Start the **frontend** development server.
3. Open `http://localhost:4200/` in your browser.
4. Interact with the book list to add, edit, or delete books.

## Tech Stack

- **Backend**: C#, ASP.NET Core, .NET 10.0
- **Frontend**: TypeScript, Angular 21, Bootstrap 5, RxJS
- **Data Sharing**: DTO (Data Transfer Object) patterns for clean API communication.
