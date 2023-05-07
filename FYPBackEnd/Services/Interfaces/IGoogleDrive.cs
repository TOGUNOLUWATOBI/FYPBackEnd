using FYPBackEnd.Data.ReturnedResponse;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace FYPBackEnd.Services.Interfaces
{
    public interface IGoogleDrive
    {
        Task<ApiResponse> UploadFileWithMetaData(IFormFile file);
    }
}
