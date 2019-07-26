using DurableFunctionsDemo.Service;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

[assembly: FunctionsStartup(typeof(DurableFunctionsDemo.Startup))]
namespace DurableFunctionsDemo
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddSingleton<IExpenseValidatorFactory, ExpenseValidatorFactory>();

            builder.Services.AddTransient<IDataService, DataService>();
            builder.Services.AddTransient<INotificationService, NotificationService>();
        }
    }
}