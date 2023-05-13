using FYPBackEnd.Data.Models.FlutterWave;
using FYPBackEnd.Data.ReturnedResponse;
using System.Threading.Tasks;

namespace FYPBackEnd.Services.Interfaces
{
    public interface IFlutterWave
    {
        Task<ApiResponse> CreateVirtualStaticAccount(CreateVIrtualRequestModel model);
        Task<ApiResponse> GetVirtualStaticAccount(string orderRef);
        Task<ApiResponse> InitiateTransfer(InitiateTransferRequestModel model);
        Task<ApiResponse> GetBillCategories();
        Task<ApiResponse> PayBill(PayBillRequestModel model);
        Task<ApiResponse> ValidateBillPayment(ValidateBillRequestModel model);
        Task<ApiResponse> GetAllBanks();
        Task<ApiResponse> AccountNameVerification(AccountNameVerificationModel model);
        Task<ApiResponse> CreatePayoutSubaccount(CreatePaymentSubaccountRequestModel model);
        Task<ApiResponse> GetPaymentSubaccountBalance(string thirdpartyReference);
        Task<ApiResponse> GetPaymentSubaccount(string thirdpartyReference);
    }
}
