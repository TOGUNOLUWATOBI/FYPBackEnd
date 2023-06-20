using FYPBackEnd.Data.Models.UVerify.Request;
using FYPBackEnd.Data.Models.UVerify.Requests;
using FYPBackEnd.Data.ReturnedResponse;
using System.Threading.Tasks;

namespace FYPBackEnd.Services.Interfaces
{
    public interface IUVerify
    {
        Task<ApiResponse> VerifyBVN(BvnVerificationRequestModel model);
        Task<ApiResponse> VerifyNin(NinVerificationRequestModel model);
        Task<ApiResponse> VerifyPassport(PassportVerificationRequestModel model);
        Task<ApiResponse> VerifyDriversLicense(DriverLicesnseVerificationRequestModel model);
    }
}
