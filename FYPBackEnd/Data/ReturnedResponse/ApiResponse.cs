namespace FYPBackEnd.Data.ReturnedResponse
{
    public class ApiResponse
    {
        public int Code { get; set; }

        public string Message { get; set; }

        public object Data { get; set; }

        public string Status { get; set; }
        public string StatusCode { get; set; }

    }
}
