using BudgetTracker.Services;

namespace BudetTracker
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            MenuManager menuManager = new MenuManager();
            await menuManager.RunMenu();
        }
    }
}