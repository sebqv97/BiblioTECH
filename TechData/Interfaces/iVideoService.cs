using TechData.Models;

namespace TechData.Interfaces
{
    public interface IVideoService
    {
        Video Get(int id);

        void Edit(Video editedVideo);
    }
}
