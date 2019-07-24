using System;
using System.Threading.Tasks;
using DurableFunctionsDemo.Models;

namespace DurableFunctionsDemo.Service.Unreliable
{
    public class DataService : IDataService
    {
        public Task SaveReport(ExpenseReport expenseReport)
        {
            throw new Exception("Demo error");
        }
    }
}