using FYPBackEnd.Data.Enums;

namespace FYPBackEnd.Data.ReturnedResponse
{
    public class ReturnedResponse
    {
        public static ApiResponse SuccessResponse(string message, object data)
        {
            var apiResp = new ApiResponse();
            apiResp.Data = data;
            apiResp.StatusCode = Status.Successful.ToString();
            apiResp.Code = 200;
            apiResp.Message = message;  
            return apiResp;
        }

        public static ApiResponse ErrorResponse(string message, object data)
        {
            var apiResp = new ApiResponse();
            apiResp.Data = data;
            apiResp.StatusCode = Status.UnSuccessful.ToString();
            apiResp.Code = 400;
            apiResp.Message = message;
            return apiResp;
        }
    }
}
