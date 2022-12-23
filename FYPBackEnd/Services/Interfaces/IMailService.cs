using FYPBackEnd.Data.Entities;
using FYPBackEnd.Data.Models.RequestModel;
using FYPBackEnd.Data.ReturnedResponse;
using Microsoft.AspNetCore.Builder;
using System.Threading.Tasks;

namespace FYPBackEnd.Services.Interfaces
{
    public interface IMailService
    {
        Task<ApiResponse> SendGenericEmailAsync(MailRequestModel model);
        Task<ApiResponse> SendVerificationEmailAsync(ApplicationUser user);
    }
}
