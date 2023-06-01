using FYPBackEnd.Data.Entities;
using FYPBackEnd.Data.Models.RequestModel;
using FYPBackEnd.Data.ReturnedResponse;
using System.Threading.Tasks;

namespace FYPBackEnd.Services.Interfaces
{
    public interface IAccountService
    {
        Task<ApiResponse> AddPanicModePin(AddPanicModePinModel model, string userId);
        Task<ApiResponse> GenerateAccountNumber(string userId);
        Task<ApiResponse> InitiateTransfer(TransferRequestModel model, string userId);
        Task<ApiResponse> BuyAirtimeData(BuyAirtimeRequestModel model, string userId);
        Task<ApiResponse> CheckTransactionPanicPin(CheckTransactionPinModel model, string userId);
        Task<ApiResponse> ChangeTransactionPin(ChangeTransactionPinModel model, string userId);
        Task<ApiResponse> AddTransactionPin(AddTransactionPinModel model, string userId);
        Task<ApiResponse> ChangePanicModePin(ChangePanicModePinModel model, string userId);
        Task<ApiResponse> CheckTransactionFee(int Amount);
        Task<ApiResponse> GetDataBundleByProviders(string serviceProvider);
        Task<ApiResponse> validateAccountDetails(VerifyAccountUserRequestModel model);
        Task<ApiResponse> GetAllBanksWithCode();
        Task<ApiResponse> PopulateBankTable();
    }
}
