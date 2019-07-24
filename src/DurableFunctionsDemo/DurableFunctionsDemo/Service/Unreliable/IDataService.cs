using DurableFunctionsDemo.Models;
using System.Threading.Tasks;

namespace DurableFunctionsDemo.Service.Unreliable
{
    public interface IDataService
    {
        Task SaveReport(ExpenseReport expenseReport);
    }
}