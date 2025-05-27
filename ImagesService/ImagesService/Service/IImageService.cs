using System.Threading.Tasks;

namespace ImagesService.Service
{
    public interface IImageService
    {
        Task SaveImageAsync(string filePath, byte[] imageData);
        Task<byte[]> LoadImageAsync(string filePath);
        void DeleteImage(string filePath);
    }
}
