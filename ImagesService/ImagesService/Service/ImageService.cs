using System;
using System.IO;
using System.Threading.Tasks;

namespace ImagesService.Service
{
    public class ImageService
    {
        private readonly string _imageFolder = Path.Combine(Directory.GetCurrentDirectory(), "Images");

        // Méthode pour sauvegarder une image sur le disque  
        public async Task SaveImageAsync(string filePath, byte[] imageData)
        {
            if (string.IsNullOrWhiteSpace(filePath))
                throw new ArgumentException("Le chemin du fichier ne peut pas être vide.", nameof(filePath));

            if (imageData == null || imageData.Length == 0)
                throw new ArgumentException("Les données de l'image ne peuvent pas être nulles ou vides.", nameof(imageData));

            await File.WriteAllBytesAsync(filePath, imageData);
        }

        // Méthode pour charger une image depuis le disque  
        public async Task<byte[]> LoadImageAsync(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
                throw new ArgumentException("Le chemin du fichier ne peut pas être vide.", nameof(filePath));

            if (!File.Exists(filePath))
                throw new FileNotFoundException("Le fichier spécifié est introuvable.", filePath);

            return await File.ReadAllBytesAsync(filePath);
        }

        // Méthode pour supprimer une image du disque  
        public void DeleteImage(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
                throw new ArgumentException("Le chemin du fichier ne peut pas être vide.", nameof(filePath));

            if (File.Exists(filePath))
                File.Delete(filePath);
        }
    }
}
