using BudgetTracker.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace BudgetTracker.Services
{
    public class MenuManager
    {
        public bool Run { get; private set; } = true;
        public TransactionProcessor transactionProcessor { get; set; } = new TransactionProcessor();
        public async Task RunMenu()
        {
            Console.WriteLine("Welcome to budget tracker!");
            // Displays a menu text
            while (Run)
            {
                Console.WriteLine(" 1 - View transactions. \n 2- Add a transaction \n 3- Modify a transaction \n 4- Remove transaction \n 0- Quit");
                string? value = Console.ReadLine();
                switch (value)
                {
                    case "1":
                        Console.Clear();
                        await ViewTransactions();
                        break;
                    case "2":
                        Console.Clear();
                        await AddTransaction();
                        break;
                    case "3":
                        Console.Clear();
                        ModifyTransaction();
                        break;
                    case "4":
                        Console.Clear();
                        RemoveTransaction();
                        break;
                    case "0":
                        Run = false;
                        break;
                    default:
                        Console.Clear();
                        Console.WriteLine("Unknown Entry. Please try again");
                        break;
                }
            }
        }
        public async Task ViewTransactions()
        {
            var transactions = await transactionProcessor.GetTransactions();
            Console.Clear();

            foreach (var transaction in transactions)
            {
                Console.WriteLine($"Transaction Amount: {transaction.Amount} \nTransaction Date: {transaction.Date}");
            }

            Console.WriteLine("Press any key to quit");
            Console.ReadKey();
        }
        public async Task AddTransaction()
        {
            Console.WriteLine("Add a transaction! \n Choose your transaction amount : ");
            decimal amount = Convert.ToDecimal(Console.ReadLine());
            Console.WriteLine("Choose id of one of your banks : ");
            var banks = await transactionProcessor.GetUserBank();
            foreach (var bank in banks)
            {
                Console.WriteLine($"{bank.BankId} - Name: {bank.Name} \nBank Location: {bank.Location}");
            }
            string? input;
            while (true)
            {
                input = Console.ReadLine();
                if (input != null && banks.Any(bank => bank.BankId == int.Parse(input)))
                {
                    break;
                }
                else
                {
                    Console.WriteLine("Invalid bank id. Please try again");
                }
            }
            Console.WriteLine("Choose a category or create your own : ");
            var categories = await transactionProcessor.GetCategories();
            foreach (Category category in categories)
            {
                Console.WriteLine($"{category.Name}\n");
            }
            string? chosenCategory = Console.ReadLine();
            if(chosenCategory != null)
            {
                int categoryId = await transactionProcessor.GetCategoryIdOrCreate(chosenCategory);
                var transaction = new Models.Transaction(1, amount, DateTime.Now, categoryId, Convert.ToInt32(input));
                Console.WriteLine("Adding transaction...");
                await transactionProcessor.sendTransaction(transaction);
            }
            else
            {
                throw new ArgumentNullException(nameof(chosenCategory));
            }
        }
        public void ModifyTransaction()
        {
            Console.WriteLine("Transaction modification function");
        }
        public void RemoveTransaction()
        {
            Console.WriteLine("Transaction removal");
        }
    }
}