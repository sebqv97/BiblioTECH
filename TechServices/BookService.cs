using Microsoft.EntityFrameworkCore;
using System.Linq;
using TechData;
using TechData.Interfaces;
using TechData.Models;

namespace TechServices
{
    public class BookService : IBookService
    {
        private readonly TechContext _context;

        public BookService(TechContext context)
        {
            _context = context;
        }


        public void Edit(Book editedBook)
        {
            _context.Entry(editedBook).State = EntityState.Modified;
            _context.SaveChanges();
        }

        public Book Get(int id)
        {
            return _context.Books.FirstOrDefault(book => book.Id == id);
        }

    }
}
