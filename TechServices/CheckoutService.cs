using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using TechData;
using TechData.Interfaces;
using TechData.Models;

namespace TechServices
{
    public class CheckoutService : ICheckoutService
    {
        private readonly TechContext _context;

        public CheckoutService(TechContext context)
        {
            _context = context;
        }


        public void CheckoutItem(int id, string userMail)
        {
            if (IsCheckedOut(id)) return;

            var item = _context.LibraryAssets
                .Include(a => a.Status)
                .First(a => a.Id == id);

            _context.Update(item);

            item.Status = _context.Statuses
                .FirstOrDefault(a => a.Name == "Împrumutat");

            var now = DateTime.Now;

            //extragem patronul cu email-ul dat

            var patron = _context.Patrons
                 .Include(card => card.LibraryCard)
                .First(pa => pa.Email == userMail);

            var libraryCard = _context.LibraryCards
                .Include(c => c.Checkouts)
                .FirstOrDefault(a => a.Id == patron.LibraryCard.Id);

            var checkout = new Checkout
            {
                LibraryAsset = item,
                LibraryCard = libraryCard,
                Since = now,
                Until = GetDefaultCheckoutTime(now)
            };

            _context.Add(checkout);

            var checkoutHistory = new CheckoutHistory
            {
                CheckedOut = now,
                LibraryAsset = item,
                LibraryCard = libraryCard
            };

            _context.Add(checkoutHistory);
            _context.SaveChanges();
        }

        public void MarkLost(int id)
        {
            var item = _context.LibraryAssets
                .First(a => a.Id == id);

            _context.Update(item);

            item.Status = _context.Statuses.FirstOrDefault(a => a.Name == "Pierdut");

            _context.SaveChanges();
        }

        public void MarkFound(int id)
        {
            var item = _context.LibraryAssets
                .First(a => a.Id == id);

            _context.Update(item);
            item.Status = _context.Statuses.FirstOrDefault(a => a.Name == "Valabil");
            var now = DateTime.Now;

            // remove any existing checkouts on the item
            var checkout = _context.Checkouts
                .FirstOrDefault(a => a.LibraryAsset.Id == id);
            if (checkout != null) _context.Remove(checkout);

            // close any existing checkout history
            var history = _context.CheckoutHistories
                .FirstOrDefault(h =>
                    h.LibraryAsset.Id == id
                    && h.CheckedIn == null);
            if (history != null)
            {
                _context.Update(history);
                history.CheckedIn = now;
            }

            _context.SaveChanges();
        }

        public void PlaceHold(int assetId, string userMail)
        {
            var now = DateTime.Now;

            var asset = _context.LibraryAssets
                .Include(a => a.Status)
                .First(a => a.Id == assetId);

            //extragem patron-ul
            var patron = _context.Patrons
                .Include(pa => pa.LibraryCard)
                .First(pa => pa.Email == userMail);

            var card = _context.LibraryCards
                .First(a => a.Id == patron.LibraryCard.Id);

            _context.Update(asset);

            if (asset.Status.Name == "Valabil")
                asset.Status = _context.Statuses.FirstOrDefault(a => a.Name == "On Hold");

            var hold = new Hold
            {
                HoldPlaced = now,
                LibraryAsset = asset,
                LibraryCard = card
            };

            _context.Add(hold);
            _context.SaveChanges();
        }

        public void CheckInItem(int id)
        {
            var now = DateTime.Now;

            var item = _context.LibraryAssets
                .First(a => a.Id == id);

            _context.Update(item);

            // remove any existing checkouts on the item
            var checkout = _context.Checkouts
                .Include(c => c.LibraryAsset)
                .Include(c => c.LibraryCard)
                .FirstOrDefault(a => a.LibraryAsset.Id == id);

            if (checkout != null) _context.Remove(checkout);

            // close any existing checkout history
            var history = _context.CheckoutHistories
                .Include(h => h.LibraryAsset)
                .Include(h => h.LibraryCard)
                .FirstOrDefault(h =>
                    h.LibraryAsset.Id == id
                    && h.CheckedIn == null);
            if (history != null)
            {
                _context.Update(history);
                history.CheckedIn = now;

                //get reservation time
                var checkOutTime = (DateTime.Now - history.CheckedOut).Days;

                //Apply fees : 2 Lei pentru fiecare zi intarziata ... scump, dar patesti
                if (checkOutTime > 1)
                {
                    history.LibraryCard.Fees += 2.00F * checkOutTime;
                }


            }

            // look for current holds
            var currentHolds = _context.Holds
                .Include(a => a.LibraryAsset)
                .Include(a => a.LibraryCard)
                .Where(a => a.LibraryAsset.Id == id);

            // if there are current holds, check out the item to the earliest
            if (currentHolds.Any())
            {
                CheckoutToEarliestHold(id, currentHolds);
                return;
            }

            // otherwise, set item status to available
            item.Status = _context.Statuses.FirstOrDefault(a => a.Name == "Valabil");

            _context.SaveChanges();
        }

        public IEnumerable<CheckoutHistory> GetCheckoutHistory(int id)
        {
            return _context.CheckoutHistories
                .Include(a => a.LibraryAsset)
                .Include(a => a.LibraryCard)
                .Where(a => a.LibraryAsset.Id == id);
        }

        // Remove useless method and replace with finding latest CheckoutHistory if needed 
        public Checkout GetLatestCheckout(int id)
        {
            return _context.Checkouts
                .Where(c => c.LibraryAsset.Id == id)
                .OrderByDescending(c => c.Since)
                .FirstOrDefault();
        }



        public bool IsCheckedOut(int id)
        {
            var isCheckedOut = _context.Checkouts.Any(a => a.LibraryAsset.Id == id);

            return isCheckedOut;
        }

        public string GetCurrentHoldPatron(int id)
        {
            var hold = _context.Holds
                .Include(a => a.LibraryAsset)
                .Include(a => a.LibraryCard)
                .Where(v => v.Id == id);

            var cardId = hold
                .Include(a => a.LibraryCard)
                .Select(a => a.LibraryCard.Id)
                .FirstOrDefault();

            var patron = _context.Patrons
                .Include(p => p.LibraryCard)
                .First(p => p.LibraryCard.Id == cardId);

            return patron.FirstName + " " + patron.LastName;
        }

        public string GetCurrentHoldPlaced(int id)
        {
            var hold = _context.Holds
                .Include(a => a.LibraryAsset)
                .Include(a => a.LibraryCard)
                .Where(v => v.Id == id);

            return hold.Select(a => a.HoldPlaced)
                .FirstOrDefault().ToString(CultureInfo.InvariantCulture);
        }

        public IEnumerable<Hold> GetCurrentHolds(int id)
        {
            return _context.Holds
                .Include(h => h.LibraryAsset)
                .Where(a => a.LibraryAsset.Id == id);
        }

        public string GetCurrentPatron(int id)
        {
            var checkout = _context.Checkouts
                .Include(a => a.LibraryAsset)
                .Include(a => a.LibraryCard)
                .FirstOrDefault(a => a.LibraryAsset.Id == id);

            if (checkout == null) return "Nu este împrumutat.";

            var cardId = checkout.LibraryCard.Id;

            var patron = _context.Patrons
                .Include(p => p.LibraryCard)
                .First(c => c.LibraryCard.Id == cardId);

            return patron.FirstName + " " + patron.LastName;
        }

        private void CheckoutToEarliestHold(int assetId, IEnumerable<Hold> currentHolds)
        {
            var earliestHold = currentHolds.OrderBy(a => a.HoldPlaced).FirstOrDefault();
            if (earliestHold == null) return;
            var card = earliestHold.LibraryCard;
            var patron = _context.Patrons
                .First(pa => pa.LibraryCard == card);
            _context.Remove(earliestHold);
            _context.SaveChanges();

            CheckoutItem(assetId, patron.Email);
        }

        private DateTime GetDefaultCheckoutTime(DateTime now)
        {
            return now.AddDays(1);
        }

        public bool IsCheckedOutByMe(int id, string myMail)
        {
            if (IsCheckedOut(id))
            {
                var patron = _context.Patrons
                    .Include(pa => pa.LibraryCard)
                    .First(pa => pa.Email == myMail);
                if (GetLatestCheckout(id).LibraryCard.Id == patron.LibraryCard.Id)
                {
                    return true;
                }
            }
            return false;
        }

        public bool IsHoldedByMe(int id, string myMail)
        {
            if (IsCheckedOut(id))
            {
                var patron = _context.Patrons
                    .Include(pa => pa.LibraryCard)
                    .First(pa => pa.Email == myMail);
                var holds = GetCurrentHolds(id)
                    .FirstOrDefault(hol => hol.LibraryCard == patron.LibraryCard);

                if (holds == null)
                {
                    return false;
                }
                else
                {
                    return true;
                }

            }
            return false;
        }

        public void CancelHold(int assetId, string userMail)
        {
            var asset = _context.LibraryAssets
                 .First(ass => ass.Id == assetId);
            var patron = _context.Patrons
                .Include(pa => pa.LibraryCard)
                .First(pa => pa.Email == userMail);
            var holds = _context.Holds
                .First(ass => ass.LibraryCard == patron.LibraryCard && ass.LibraryAsset == asset);

            _context.Remove(holds);
            _context.SaveChanges();
        }

        public float userFees(string myMail)
        {
            var libraryCard = _context.Patrons
                .Include(pa => pa.LibraryCard)
                .FirstOrDefault(pa => pa.Email == myMail).LibraryCard;
            return libraryCard.Fees;
        }
    }
}
