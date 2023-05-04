using FYPBackEnd.Data.Models.Okra;
using FYPBackEnd.Data.ReturnedResponse;
using System.Threading.Tasks;

namespace FYPBackEnd.Services.Interfaces
{
    public interface IOkra
    {
        Task<ApiResponse> GetBvnDetailsById(GetBvnByIdRequestModel model);
        Task<ApiResponse> GetBvnDetailsByBvn(GetBvnByBvnRequestModel model);
    }
}
