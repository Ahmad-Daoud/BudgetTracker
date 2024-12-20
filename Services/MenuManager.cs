using BudgetTracker.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
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
                        await ModifyTransaction();
                        break;
                    case "4":
                        Console.Clear();
                        await RemoveTransaction();
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
            var transactions = await transactionProcessor.GetTransactions(1);
            Console.Clear();
            Console.WriteLine("Here are the transactions in the database : \n");
            foreach (var transaction in transactions)
            {
                string catName = await transactionProcessor.GetCategoryNameById(transaction.CategoryId);
                Console.WriteLine($"{transaction.Id} - Transaction Amount: {transaction.Amount} {transaction.Date.ToString("d")} , Category : {catName}");
            }

            Console.WriteLine("Press any key to quit");
            Console.ReadKey();
            Console.Clear();
        }
        public async Task AddTransaction()
        {
            Console.WriteLine("Add a transaction! \n Choose your transaction amount : ");
            decimal amount;
            while (true)
            {
                var resp = Console.ReadLine();
                // Check if it's all digits. It also might contain a full stop / comma
                if (resp != null && (resp.All(char.IsDigit) || resp.Contains(".") || resp.Contains(",")))
                {
                    // Convert response to a digit
                    try
                    {
                        amount = Convert.ToDecimal(resp);
                        break;
                    }
                    catch (Exception e)
                    {
                        Console.Clear();
                        Console.WriteLine("Invalid input. Please try again");
                    }
                }
                else
                {
                    Console.Clear();
                    Console.WriteLine("Invalid input. Please try again");
                }
            }
            Console.WriteLine("Choose id of one of your banks : ");
            var banks = await transactionProcessor.GetUserBank();
            foreach (var bank in banks)
            {
                Console.WriteLine($"{bank.BankId} - Name: {bank.Name} \nBank Location: {bank.Location}");
            }
            string? bankId;
            while (true)
            {
                bankId = Console.ReadLine();
                if (bankId != null && banks.Any(bank => bank.BankId == int.Parse(bankId)))
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
            if (chosenCategory != null)
            {
                int categoryId = await transactionProcessor.GetCategoryIdOrCreate(chosenCategory);
                // We give the transaction an id of 1 but the id will be generated by the database when sent
                var transaction = new Models.Transaction(1, 1, amount, DateTime.Now, categoryId, Convert.ToInt32(bankId));
                Console.WriteLine("Adding transaction...");
                await transactionProcessor.sendTransaction(transaction);
            }
            else
            {
                throw new ArgumentNullException(nameof(chosenCategory));
            }
        }
        public async Task ModifyTransaction()
        {
            string? input;
            bool quit = false;
            while (true)
            {
                Console.WriteLine("Please enter the transaction id you wish to modify or q to quit");
                input = Console.ReadLine();
                if (input == "q")
                {
                    quit = true;
                    break;
                }
                else if (input != null && input != "" && input.All(char.IsDigit))
                {
                    if (await transactionProcessor.TransactionExists(Convert.ToInt32(input)) == true)
                    {
                        break;
                    }
                    else
                    {
                        Console.Clear();
                        Console.WriteLine("Transaction not found. Please try again");
                    }
                }
                else
                {
                    Console.Clear();
                    Console.WriteLine("Invalid input. Please try again");
                }
            }
            if (quit != true && input != null)
            {
                Console.Clear();
                int id = Convert.ToInt32(input);
                bool run = true;
                while (run)
                {
                    Console.WriteLine("What would you like to modify ? \n 1 - Amount \n 2 - Category");
                    var userInput = Console.ReadLine();
                    if(userInput == null || !userInput.All(char.IsDigit))
                    {
                        Console.Clear();
                        continue;
                    }
                    run = false;
                    switch (userInput)
                    {                            
                        case "1":
                            await ModifyAmount(id);
                            break;
                        case "2":
                            await ModifyCategory(id);
                            break;
                    }
                }
            }
            else if (quit != true)
            {
                throw new ArgumentNullException(nameof(input));
            }
            //Console.Clear();
        }
        public async Task RemoveTransaction()
        {
            string? input;
            bool quit = false;
            while (true)
            {
                Console.WriteLine("Please enter the transaction id you wish to remove or q to quit");
                input = Console.ReadLine();
                if (input == "q" )
                {
                    quit = true;
                    break;
                }
                else if(input != null && input != "" && input.All(char.IsDigit))
                {
                    if(await transactionProcessor.TransactionExists(Convert.ToInt32(input)) == true)
                    {
                        break;
                    }
                    else
                    {
                        Console.Clear();
                        Console.WriteLine("Transaction not found. Please try again");
                    }
                }
                else
                {
                    Console.Clear();
                    Console.WriteLine("Invalid input. Please try again");
                }
            }
            if (quit != true && input != null)
            {
                bool response = await transactionProcessor.RemoveTransaction(Convert.ToInt32(input));
                if (response)
                {
                    Console.Clear();
                    Console.WriteLine("Transaction removed successfully!");
                }
                else
                {
                    Console.WriteLine("Transaction not found");
                    await RemoveTransaction();
                }
            }
            else if (quit != true)
            {
                throw new ArgumentNullException(nameof(input));
            }
            Console.Clear();
        }
        public async Task ModifyAmount(int id)
        {
            Console.Clear();
            decimal amount;
            while (true)
            {
                Console.WriteLine("Enter the new amount : ");
                var resp = Console.ReadLine();
                // Check if it's all digits. It also might contain a full stop / comma
                // TODO : fix to become resp.All(char.IsDigit || "." || ",") and make sure it contains only one comma/period 
                if (resp != null && (resp.All(char.IsDigit) || resp.Contains(".") || resp.Contains(",")))
                {
                    // Convert response to a digit
                    try
                    {
                        amount = Convert.ToDecimal(resp);
                        break;
                    }
                    catch (Exception e)
                    {
                        Console.Clear();
                        Console.WriteLine("Invalid input. Please try again");
                    }
                }
                else
                {
                    Console.Clear();
                    Console.WriteLine("Invalid input. Please try again");
                }
            }
            //Models.Transaction transaction = transactionProcessor.GetTransaction(id);
            Models.Transaction transaction = await transactionProcessor.GetTransaction(id);
            transaction.setAmount(amount);
            await transactionProcessor.ModifyTransaction(transaction, id);
            Console.Clear();
        }
        public async Task ModifyCategory(int id)
        {
            Console.Clear();
            int catId = 0;
            while (true)
            {
                Console.WriteLine("Enter the new category name : ");
                var resp = Console.ReadLine();
                if (resp != null)
                {
                    // Get category if it exists, create it if it doesn't
                    catId = transactionProcessor.GetCategoryIdOrCreate(resp).Result;
                    break;
                }
                else
                {
                    Console.Clear();
                    Console.WriteLine("Invalid input. Please try again");
                }
            }
            Models.Transaction transaction = await transactionProcessor.GetTransaction(id);
            transaction.SetCategory(catId);
            await transactionProcessor.ModifyTransaction(transaction, id);
            Console.ReadLine();
        }
    }
}
