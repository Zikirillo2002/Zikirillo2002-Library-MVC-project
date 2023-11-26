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

namespace Library.Controllers
{
    public class PublicationsController : Controller
    {
        private readonly LibraryDbContext _context;

        public PublicationsController(LibraryDbContext context)
        {
            _context = context;
        }

        // GET: Publications
        public async Task<IActionResult> Index(string sortOrder, string? searchString)
        {
            ViewData["CurrentSort"] = sortOrder;
            ViewData["IdSort"] = sortOrder == "id_asc" ? "id_desc" : "id_asc";
            ViewData["BookNameSort"] = sortOrder == "bookName_asc" ? "bookName_desc" : "bookName_asc";
            ViewData["AuthorNameSort"] = sortOrder == "authorName_asc" ? "authorName_desc" : "authorName_asc";
            ViewData["DateSort"] = sortOrder == "date_asc" ? "date_desc" : "date_asc";

            var libraryDbContext = _context.Publications
                .Include(p => p.Author)
                .Include(p => p.Book)
                .AsQueryable();

            if (searchString != null)
            {
                libraryDbContext = libraryDbContext
                .Where(x => x.Book.Name.ToLower().Contains(searchString.ToLower()) ||
                x.Author.FullName.ToLower().Contains(searchString.ToLower()))
                .AsQueryable();
            }

            libraryDbContext = sortOrder switch
            {
                "id_asc" => libraryDbContext.OrderBy(x => x.Id),
                "id_desc" => libraryDbContext.OrderByDescending(x => x.Id),
                "bookName_asc" => libraryDbContext.OrderBy(x => x.Book.Name),
                "bookName_desc" => libraryDbContext.OrderByDescending(x => x.Book.Name),
                "authorName_asc" => libraryDbContext.OrderBy(x => x.Author.FullName),
                "authorName_desc" => libraryDbContext.OrderByDescending(x => x.Author.FullName),
                "date_asc" => libraryDbContext.OrderBy(x => x.PublishedDate),
                "date_desc" => libraryDbContext.OrderByDescending(x => x.PublishedDate),
                _ => libraryDbContext.OrderBy(x => x.Id)
            };

            return View(libraryDbContext);
        }

        // GET: Publications/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Publications == null)
            {
                return NotFound();
            }

            var publication = await _context.Publications
                .Include(p => p.Author)
                .Include(p => p.Book)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (publication == null)
            {
                return NotFound();
            }

            return View(publication);
        }

        // GET: Publications/Create
        public IActionResult Create()
        {
            ViewData["AuthorId"] = new SelectList(_context.Authors, "Id", "FullName");
            ViewData["BookId"] = new SelectList(_context.Books, "Id", "Name");
            return View();
        }

        // POST: Publications/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,BookId,AuthorId,PublishedDate")] Publication publication)
        {
            if (publication != null)
            {
                _context.Publications.Add(publication);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["AuthorId"] = new SelectList(_context.Authors, "Id", "FullName", publication.AuthorId);
            ViewData["BookId"] = new SelectList(_context.Books, "Id", "Name", publication.BookId);

            return View(publication);
        }

        // GET: Publications/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Publications == null)
            {
                return NotFound();
            }

            var publication = await _context.Publications.FindAsync(id);
            if (publication == null)
            {
                return NotFound();
            }
            ViewData["AuthorId"] = new SelectList(_context.Authors, "Id", "FullName", publication.AuthorId);
            ViewData["BookId"] = new SelectList(_context.Books, "Id", "Name", publication.BookId);
            return View(publication);
        }

        // POST: Publications/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,BookId,AuthorId,PublishedDate")] Publication publication)
        {
            if (id != publication.Id)
            {
                return NotFound();
            }

            if (publication != null)
            {
                try
                {
                    _context.Update(publication);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PublicationExists(publication.Id))
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
            ViewData["AuthorId"] = new SelectList(_context.Authors, "Id", "FullName", publication.AuthorId);
            ViewData["BookId"] = new SelectList(_context.Books, "Id", "Name", publication.BookId);
            return View(publication);
        }

        // GET: Publications/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Publications == null)
            {
                return NotFound();
            }

            var publication = await _context.Publications
                .Include(p => p.Author)
                .Include(p => p.Book)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (publication == null)
            {
                return NotFound();
            }

            return View(publication);
        }

        // POST: Publications/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Publications == null)
            {
                return Problem("Entity set 'LibraryDbContext.Publications'  is null.");
            }
            var publication = await _context.Publications.FindAsync(id);
            if (publication != null)
            {
                _context.Publications.Remove(publication);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PublicationExists(int id)
        {
          return (_context.Publications?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
