using DurableFunctionsDemo.Models;
using System.Threading.Tasks;

namespace DurableFunctionsDemo.Service
{
    public interface IDataService
    {
        Task SaveReport(ExpenseReport expenseReport);
        Task SaveReportReliable(ExpenseReport expenseReport);
    }
}