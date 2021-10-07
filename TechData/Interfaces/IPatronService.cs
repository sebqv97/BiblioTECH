using System.Collections.Generic;
using TechData.Models;

namespace TechData.Interfaces
{
    public interface IPatronService
    {
        int GetPatronIdFromCurrentUser(string Mail);
        Patron Get(int id);
        void Add(Patron newBook, int branchId);
        IEnumerable<CheckoutHistory> GetCheckoutHistory(int patronId);
        IEnumerable<Hold> GetHolds(int patronId);
        IEnumerable<Checkout> GetCheckouts(int id);
        IEnumerable<Patron> GetAll();
    }
}
