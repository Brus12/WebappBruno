using Microsoft.EntityFrameworkCore;
using WebappBruno.Models;
namespace WebappBruno.Context
{
    public class brunoContext : DbContext
    {
        public brunoContext(DbContextOptions<brunoContext>options):base(options) 
        { 
        }

        public DbSet<Autor> autores { get; set; }
        public DbSet<Libro> libros { get; set; }
    }
}
