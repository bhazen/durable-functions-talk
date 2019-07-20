using DurableFunctionsDemo.Service.Basic;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

[assembly: FunctionsStartup(typeof(DurableFunctionsDemo.Startup))]
namespace DurableFunctionsDemo
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddTransient<IBasicExpenseValidationProvider, BasicExpenseValidationProvider>();
        }
    }
}