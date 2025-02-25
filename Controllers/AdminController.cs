﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IndyBooks.Models;
using IndyBooks.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IndyBooks.Controllers
{
    public class AdminController : Controller
    {
        private IndyBooksDataContext _db;
        public AdminController(IndyBooksDataContext db) { _db = db; }

        /***
         * READ       
         */
        [HttpGet]
        public IActionResult Index(long id)
        {


            IQueryable<Book> books = _db.Books.Include(a => a.Author);

            //filter books by the id (if passed an id as its Route Parameter),
            //     otherwise use the entire collection of Books, ordered by SKU.
            if (id == 0)
            { 
                return View("SearchResults", books);
            }

             books = books.Where(Book => Book.Id == id);
             return View("SearchResults", books);


        }
        /***
         * CREATE
         */
        [HttpGet]
        public IActionResult CreateBook()
        {
            IQueryable<Writer> writer = _db.Writers;
            //TODO: Build a new CreateBookViewModel with a complete set of Writers from the database
            CreateBookViewModel createBookVM = new CreateBookViewModel()
            {
               Writers = writer,
            };
            

            return View(createBookVM); //TODO: pass the ViewModel onto the CreateBook View
        }
        [HttpPost]
        public IActionResult CreateBook(CreateBookViewModel bookVM)
        {
            //TODO: Build the Writer object using the parameter
            if(bookVM.Name != null)
            {
                Writer writer = new Writer()
                {
                    Name = bookVM.Name,
                };

                _db.Writers.Add(writer);
                
                
            } else
            {
                _db
            }

            }
         

            //TODO: Build the Book using the parameter data and your newly created author.

            Book book = new Book()
            {
                Title = bookVM.Title,
                Price = bookVM.Price,
                SKU = bookVM.SKU,
                Author = writer,

            };

            //TODO: Add author and book to their DbSets; SaveChanges
            

            //TODO: Show the book by passing the Book's id (rather than 1) to the Index Action 
            return RedirectToAction("Index", new { id = 1 });
        }
        /***
         * UPDATE (reusing the CreateBook View ) 
         */
         //TODO: Write a method to take a book id, and load book and author info
         //      into the ViewModel for the CreateBook View
         [HttpGet]

        /***
         * DELETE
         */
        [HttpGet]
        public IActionResult DeleteBook(long id)
        {
            _db.Remove(new Book() { Id = id });
            _db.SaveChanges();
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Search() { return View(); }
        [HttpPost]
        public IActionResult Search(SearchViewModel search)
        {
            //Full Collection Search
            IQueryable<Book> foundBooks = _db.Books.Include(b=>b.Author); // start with entire collection

            //Partial Title Search
            if (search.Title != null)
            {
                foundBooks = foundBooks
                            .Where(b => b.Title.Contains(search.Title))
                            .OrderBy(b => b.Author.Name)
                            ;
            }

            //Author's Last Name Search
            if (search.AuthorName != null)
            {
                //Use the Name property of the Book's Author entity
                foundBooks = foundBooks
                            .Where(b => b.Author.Name.EndsWith(search.AuthorName))
                            ;
            }
            //Priced Between Search (min and max price entered)
            if (search.MinPrice > 0 && search.MaxPrice > 0)
            {
                foundBooks = foundBooks
                            .Where(b => b.Price >= search.MinPrice && b.Price <= search.MaxPrice)
                            ;
            }
            //Composite Search Results
            return View("SearchResults", foundBooks);
        }

    }
}
