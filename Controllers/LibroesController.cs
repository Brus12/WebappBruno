using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.DotNet.Scaffolding.Shared.Messaging;
using Microsoft.EntityFrameworkCore;
using WebappBruno.Context;
using WebappBruno.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using WebappBruno.Helpers;

namespace WebappBruno.Controllers
{
    public class LibroesController : Controller
    {
        private readonly brunoContext _context;
        private readonly AzureStorageConfig _config;
        
        public LibroesController(brunoContext context, IOptions<AzureStorageConfig> config)
        {
            _context = context;
            _config = config.Value;
        }

        // GET: Libroes
        public async Task<IActionResult> Index()
        {
              return _context.libros != null ? 
                          View(await _context.libros.Include(e=> e.Autor).ToListAsync()) :
                          Problem("Entity set 'brunoContext.libros'  is null.");
        }

        // GET: Libroes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.libros == null)
            {
                return NotFound();
            }

            var libro = await _context.libros
                .Include(e => e.Autor)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (libro == null)
            {
                return NotFound();
            }

            return View(libro);
        }

        // GET: Libroes/Create
        public async Task<IActionResult> Create()
        {
            var autores = await _context.autores.ToListAsync();
            ViewBag.Autor = new SelectList(autores, "Id", "Nombre");
            return View();
        }

        // POST: Libroes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Titulo,AnioPublicacion,Foto,Autor")] Libro libro, IFormFile foto)
        {
            if ("Titulo,AnioPublicacion".Split(',').All (campo => ModelState.ContainsKey(campo)))
            {
                if (foto == null)
                {
                    libro.Foto = StorageHelper.URL_Imagen_default;
                }
                else
                {
                    string extension = foto.FileName.Split(",")[0];
                    string Titulo = $"{Guid.NewGuid()}.{extension}";
                    libro.Foto = await StorageHelper.SubirArchivo(foto.OpenReadStream(), Titulo, _config);
                }
                _context.Set<Libro>().Add(libro);
                _context.Entry(libro.Autor).State = EntityState.Unchanged;
                await _context.SaveChangesAsync();
                                           
                return RedirectToAction(nameof(Index));
               
            }
                        
            
            return View(libro);
        }

        // GET: Libroes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.libros == null)
            {
                return NotFound();
            }

            var libro = await _context.libros.FindAsync(id);
            if (libro == null)
            {
                return NotFound();
            }
            var autores = await _context.autores.ToListAsync();
            ViewBag.Autor = new SelectList(autores, "Id", "Nombre");
            return View(libro);
        }

        // POST: Libroes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Titulo,AnioPublicacion,Foto,Autor")] Libro libro)
        {
            if (id != libro.Id)
            {
                return NotFound();
            }

            // Busca el libro original en la base de datos
            var libroOriginal = await _context.libros.AsNoTracking().FirstOrDefaultAsync(l => l.Id == id);

            if (libroOriginal == null)
            {
                return NotFound();
            }

            // Copia el autor del libro original al libro editado
            libro.Autor = libroOriginal.Autor;


            if (ModelState.IsValid)
            {
                try
                {                    
                    _context.Update(libro);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LibroExists(libro.Id))
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
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors);
                foreach (var error in errors)
                {
                    Console.WriteLine(error.ErrorMessage);
                }
            }
            return View(libro);
        }

        // GET: Libroes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.libros == null)
            {
                return NotFound();
            }

            var libro = await _context.libros
                .Include(e=> e.Autor)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (libro == null)
            {
                return NotFound();
            }

            return View(libro);
        }

        // POST: Libroes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.libros == null)
            {
                return Problem("Entity set 'brunoContext.libros'  is null.");
            }
            var libro = await _context.libros.FindAsync(id);
            if (libro != null)
            {
                _context.libros.Remove(libro);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool LibroExists(int id)
        {
          return (_context.libros?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
