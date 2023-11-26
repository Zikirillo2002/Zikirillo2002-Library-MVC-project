using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Library.Data;
using Library.Models;
using Library.ViewModel;
using Microsoft.IdentityModel.Tokens;

namespace Library.Controllers
{
    public class BooksController : Controller
    {
        private readonly LibraryDbContext _context;

        public BooksController(LibraryDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string sortOrder)
        {
            ViewData["CurrentSort"] = sortOrder;
            ViewData["IdSort"] = sortOrder == "id_asc" ? "id_desc" : "id_asc";
            ViewData["NameSort"] = sortOrder == "name_asc" ? "name_desc" : "name_asc";
            ViewData["PriceSort"] = sortOrder == "price_asc" ? "price_desc" : "price_asc";
            ViewData["CategoryNameSort"] = sortOrder == "category_asc" ? "category_desc" : "category_asc";

            var libraryDbContext = _context.Books
                .Include(b => b.Category)
                .AsQueryable();

            libraryDbContext = sortOrder switch
            {
                "id_asc" => libraryDbContext.OrderBy(x => x.Id),
                "id_desc" => libraryDbContext.OrderByDescending(x => x.Id),
                "name_asc" => libraryDbContext.OrderBy(x => x.Name),
                "name_desc" => libraryDbContext.OrderByDescending(x => x.Name),
                "price_asc" => libraryDbContext.OrderBy(x => x.Price),
                "price_desc" => libraryDbContext.OrderByDescending(x => x.Price),
                "category_asc" => libraryDbContext.OrderBy(x => x.Category.Name),
                "category_desc" => libraryDbContext.OrderByDescending(x => x.Category.Name),
                _ => libraryDbContext.OrderBy(x => x.Id)
            };

            var categories = await _context.Categories.Select(x => new SelectListItem()
            {
                Text = x.Name,
                Value = x.Id.ToString(),
            }).ToListAsync();

            var booksViewModel = new BookViewModel()
            {
                Books = await libraryDbContext.ToListAsync(),
                Categories = categories
            };

            return View(booksViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Index(string? searchString, string category)
        {
            var bookList = _context.Books.ToList();

            if (searchString == null && category == "All")
            {
                var category1 = await _context.Categories.Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.Name
                }).ToListAsync();

                var bookVM1 = new BookViewModel()
                {
                    Books = bookList,
                    Categories = category1
                };

                return View(bookVM1);
            }

            if (searchString == null)
            {
                int categoryId1 = int.Parse(category);

                var books1 = await _context.Books
                .Include(x => x.Category)
                .Where(e => e.CategoryId == categoryId1)
                .ToListAsync();

                var category1 = await _context.Categories.Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.Name
                }).ToListAsync();

                var bookVM1 = new BookViewModel()
                {
                    Books = books1,
                    Categories = category1
                };

                return View(bookVM1);
            }

            if (category == "All")
            {
                var searchBooks = await _context.Books
                .Include(x => x.Category)
                .Where(s => s.Name.ToLower().Contains(searchString.ToLower()))
                .ToListAsync();

                var category1 = await _context.Categories.Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.Name
                }).ToListAsync();

                var bookVM1 = new BookViewModel()
                {
                    Books = searchBooks,
                    Categories = category1
                };

                return View(bookVM1);
            }

            int categoryId = int.Parse(category);

            var books = await _context.Books
                .Include(x => x.Category)
                .Where(s => s.Name.ToLower().Contains(searchString.ToLower()))
                .Where(e => e.CategoryId == categoryId)
                .ToListAsync();

            var categories = await _context.Categories.Select(x => new SelectListItem
            {
                Value = x.Id.ToString(),
                Text = x.Name
            }).ToListAsync();

            if (books.IsNullOrEmpty())
            {
                books = _context.Books.ToList();
            }

            var booktVM = new BookViewModel()
            {
                Books = books,
                Categories = categories
            };

            return View(booktVM);

        }



        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Books == null)
            {
                return NotFound();
            }

            var book = await _context.Books
                .Include(b => b.Category)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (book == null)
            {
                return NotFound();
            }

            return View(book);
        }

        // GET: Books/Create
        public IActionResult Create()
        {
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name");
            return View();
        }

        // POST: Books/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Description,Price,CategoryId")] Book book)
        {
            if (book != null)
            {
                _context.Books.Add(book);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", book.CategoryId);
            return View(book);
        }

        // GET: Books/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Books == null)
            {
                return NotFound();
            }

            var book = await _context.Books.FindAsync(id);
            if (book == null)
            {
                return NotFound();
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", book.CategoryId);
            return View(book);
        }

        // POST: Books/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description,Price,CategoryId")] Book book)
        {
            if (id != book.Id)
            {
                return NotFound();
            }

            if (book != null)
            {
                try
                {
                    _context.Update(book);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BookExists(book.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", book.CategoryId);
            return View(book);
        }

        // GET: Books/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Books == null)
            {
                return NotFound();
            }

            var book = await _context.Books
                .Include(b => b.Category)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (book == null)
            {
                return NotFound();
            }

            return View(book);
        }

        // POST: Books/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Books == null)
            {
                return Problem("Entity set 'LibraryDbContext.Books'  is null.");
            }
            var book = await _context.Books.FindAsync(id);
            if (book != null)
            {
                _context.Books.Remove(book);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BookExists(int id)
        {
          return (_context.Books?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
