using Microsoft.AspNetCore.Http;

namespace eBooks.Data.Services
{
    public interface IProfilePictureService
    {
        Task<string> SaveAsync(IFormFile file);
        Task DeleteAsync(string? imagePath);
    }
}