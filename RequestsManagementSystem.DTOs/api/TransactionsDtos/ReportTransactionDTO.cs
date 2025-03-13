using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RequestsManagementSystem.DTOs.api.TransactionsDtos
{
    class ReportTransactionDTO
    {
        public string Title { get; set; }
        public string TotalLeaves { get; set; }
        public string UsedLeaves { get; set; }
        public string AdditionalLeaves { get; set; }
        public List<TransactionForReportDTO> Transactions { get; set; }
    }
}
