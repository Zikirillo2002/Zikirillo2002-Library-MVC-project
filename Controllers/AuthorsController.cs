using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Library.Data;
using Library.Models;
using Microsoft.Data.SqlClient;

namespace Library.Controllers
{
    public class AuthorsController : Controller
    {
        private readonly LibraryDbContext _context;

        public AuthorsController(LibraryDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string sortOrder, string? searchString)
        {
            ViewData["CurrentSort"] = sortOrder;
            ViewData["IdSort"] = sortOrder == "id_asc" ? "id_desc" : "id_asc";
            ViewData["NameSort"] = sortOrder == "name_asc" ? "name_desc" : "name_asc";
            ViewData["DateofBirthSort"] = sortOrder == "birthDate_asc" ? "birthDate_desc" : "birthDate_asc";
            ViewData["EmailSort"] = sortOrder == "email_asc" ? "email_desc" : "email_asc";

            var authors = _context.Authors.AsQueryable();

            if (searchString != null)
            {
                authors = authors
                .Where(x => x.FullName.ToLower().Contains(searchString.ToLower()) ||
                x.Email.ToLower().Contains(searchString.ToLower()))
                .AsQueryable();
            }

            authors = sortOrder switch
            {
                "id_asc" => authors.OrderBy(x => x.Id),
                "id_desc" => authors.OrderByDescending(x => x.Id),
                "name_asc" => authors.OrderBy(x => x.FullName),
                "name_desc" => authors.OrderByDescending(x => x.FullName),
                "birthDate_asc" => authors.OrderBy(x => x.BirthDate),
                "birthDate_desc" => authors.OrderByDescending(x => x.BirthDate),
                "email_asc" => authors.OrderBy(x => x.Email),
                "email_desc" => authors.OrderByDescending(x => x.Email),
                _ => authors.OrderBy(x => x.Id)
            };

            return View(authors);
        }

        // GET: Authors/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Authors == null)
            {
                return NotFound();
            }

            var author = await _context.Authors
                .FirstOrDefaultAsync(m => m.Id == id);
            if (author == null)
            {
                return NotFound();
            }

            return View(author);
        }

        // GET: Authors/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Authors/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,FullName,BirthDate,Email,PhoneNumber")] Author author)
        {
            if (author != null)
            {
                _context.Authors.Add(author);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(author);
        }

        // GET: Authors/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Authors == null)
            {
                return NotFound();
            }

            var author = await _context.Authors.FindAsync(id);
            if (author == null)
            {
                return NotFound();
            }
            return View(author);
        }

        // POST: Authors/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,FullName,BirthDate,Email,PhoneNumber")] Author author)
        {
            if (id != author.Id)
            {
                return NotFound();
            }

            if (author != null)
            {
                try
                {
                    _context.Update(author);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AuthorExists(author.Id))
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
            return View(author);
        }

        // GET: Authors/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Authors == null)
            {
                return NotFound();
            }

            var author = await _context.Authors
                .FirstOrDefaultAsync(m => m.Id == id);
            if (author == null)
            {
                return NotFound();
            }

            return View(author);
        }

        // POST: Authors/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Authors == null)
            {
                return Problem("Entity set 'LibraryDbContext.Authors'  is null.");
            }
            var author = await _context.Authors.FindAsync(id);
            if (author != null)
            {
                _context.Authors.Remove(author);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AuthorExists(int id)
        {
          return (_context.Authors?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
