using FYPBackEnd.Data.ReturnedResponse;
using System.Threading.Tasks;

namespace FYPBackEnd.Services.Interfaces
{
    public interface IOtpService
    {
        Task<string> GenerateOtpCodeAsync();

        Task<bool> VerifyOtpCodeValidityAsync(string code);

        Task<bool> ReSendOtpCode(string email);
        
    }
}
