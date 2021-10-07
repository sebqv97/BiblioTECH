using TechData.Models;

namespace TechData.Interfaces
{
    public interface IBookService
    {

        Book Get(int id);

        void Edit(Book editedBook);
    }
}
