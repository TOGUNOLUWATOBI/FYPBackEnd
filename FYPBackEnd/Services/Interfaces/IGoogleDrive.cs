using FYPBackEnd.Data.ReturnedResponse;
using System.Threading.Tasks;

namespace FYPBackEnd.Services.Interfaces
{
    public interface IGoogleDrive
    {
        Task<ApiResponse> UploadFileWithMetaData();
    }
}
