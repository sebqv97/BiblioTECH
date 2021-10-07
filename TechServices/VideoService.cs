using Microsoft.EntityFrameworkCore;
using System.Linq;
using TechData;
using TechData.Interfaces;
using TechData.Models;

namespace TechServices
{
    public class VideoService : IVideoService
    {
        private readonly TechContext _context;

        public VideoService(TechContext context)
        {
            _context = context;
        }



        public Video Get(int id)
        {
            return _context.Videos.FirstOrDefault(v => v.Id == id);
        }

        public void Edit(Video editedVideo)
        {
            _context.Entry(editedVideo).State = EntityState.Modified;
            _context.SaveChanges();
        }
    }
}
