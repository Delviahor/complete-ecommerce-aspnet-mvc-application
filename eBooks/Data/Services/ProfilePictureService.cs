using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;

namespace eBooks.Data.Services
{
    public class ProfilePictureService : IProfilePictureService
    {
        private readonly IWebHostEnvironment _environment;

        private const long MaxFileSize = 2 * 1024 * 1024; // 2 MB

        public ProfilePictureService(IWebHostEnvironment environment)
        {
            _environment = environment;
        }

        // Guarda una imagen después de validar su formato y tamaño.
        public async Task<string> SaveAsync(IFormFile file)
        {
            if (file == null || file.Length == 0)
                throw new InvalidOperationException("No image was selected.");

            if (file.Length > MaxFileSize)
                throw new InvalidOperationException("The image cannot exceed 2 MB.");

            // Detectamos el formato mediante su firma binaria.
            var extension = await GetImageExtensionAsync(file);

            var fileName = $"profile-{Guid.NewGuid():N}{extension}";

            var uploadsFolder = Path.Combine(
                _environment.WebRootPath,
                "images",
                "profiles");

            Directory.CreateDirectory(uploadsFolder);

            var filePath = Path.Combine(uploadsFolder, fileName);

            // CreateNew evita sobrescribir un archivo existente.
            await using (var stream = new FileStream(
                filePath,
                FileMode.CreateNew,
                FileAccess.Write,
                FileShare.None))
            {
                await file.CopyToAsync(stream);
            }

            // Ruta relativa que se almacenará en la base de datos.
            return $"/images/profiles/{fileName}";
        }

        // Elimina una imagen anterior almacenada por este servicio.
        public Task DeleteAsync(string? imagePath)
        {
            if (string.IsNullOrWhiteSpace(imagePath))
                return Task.CompletedTask;

            // Solo permitimos nombres de archivo de nuestra carpeta.
            var fileName = Path.GetFileName(imagePath);

            if (!fileName.StartsWith(
                "profile-",
                StringComparison.OrdinalIgnoreCase))
            {
                return Task.CompletedTask;
            }

            var uploadsFolder = Path.Combine(
                _environment.WebRootPath,
                "images",
                "profiles");

            var filePath = Path.Combine(uploadsFolder, fileName);

            if (File.Exists(filePath))
                File.Delete(filePath);

            return Task.CompletedTask;
        }

        // Identifica JPEG, PNG o WebP mediante sus bytes iniciales.
        private static async Task<string> GetImageExtensionAsync(
            IFormFile file)
        {
            var header = new byte[12];

            await using var stream = file.OpenReadStream();

            var bytesRead = await stream.ReadAsync(
                header.AsMemory(0, header.Length));

            // JPEG
            if (bytesRead >= 3 &&
                header[0] == 0xFF &&
                header[1] == 0xD8 &&
                header[2] == 0xFF)
            {
                return ".jpg";
            }

            // PNG
            if (bytesRead >= 8 &&
                header[0] == 0x89 &&
                header[1] == 0x50 &&
                header[2] == 0x4E &&
                header[3] == 0x47 &&
                header[4] == 0x0D &&
                header[5] == 0x0A &&
                header[6] == 0x1A &&
                header[7] == 0x0A)
            {
                return ".png";
            }

            // WebP
            if (bytesRead >= 12 &&
                header[0] == 0x52 &&
                header[1] == 0x49 &&
                header[2] == 0x46 &&
                header[3] == 0x46 &&
                header[8] == 0x57 &&
                header[9] == 0x45 &&
                header[10] == 0x42 &&
                header[11] == 0x50)
            {
                return ".webp";
            }

            throw new InvalidOperationException(
                "Only JPEG, PNG and WebP images are supported.");
        }
    }
}
