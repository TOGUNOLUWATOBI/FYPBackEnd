using System.Collections.Generic;

namespace FYPBackEnd.Data.Models.DTO
{
    public class DashboardModel
    {
        public string Name { get; set; }
        public decimal Balance { get; set; }
        public List<TransactionDto> Transactions { get; set; }
    }
}
