using System.Collections.Generic;
using TechData.Models;

namespace TechData.Interfaces
{
    public interface ICheckoutService
    {
        IEnumerable<CheckoutHistory> GetCheckoutHistory(int id);
        IEnumerable<Hold> GetCurrentHolds(int id);


        Checkout GetLatestCheckout(int id);


        void PlaceHold(int assetId, string userMail);
        void CancelHold(int assetId, string userMail);
        void CheckoutItem(int id, string userMail);
        void CheckInItem(int id);
        void MarkLost(int id);
        void MarkFound(int id);


        bool IsCheckedOut(int id);
        bool IsCheckedOutByMe(int id, string myMail);
        bool IsHoldedByMe(int id, string myMail);

        string GetCurrentHoldPatron(int id);
        string GetCurrentHoldPlaced(int id);
        string GetCurrentPatron(int id);

        float userFees(string myMail);




    }
}
