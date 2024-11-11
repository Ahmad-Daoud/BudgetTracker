using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BudgetTracker.Models
{
    public class Transaction
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }
        public int CategoryId { get; set; }
        public int BankId { get; set; }
        public Transaction(int id , int userId, decimal transactionAmount, DateTime transactionDate,int categoryId, int bankId)
        {
            Id = id;
            UserId = userId;
            Amount = transactionAmount;
            Date = transactionDate;
            CategoryId = categoryId;
            BankId = bankId;
        }
        public void SetCategory(int categoryId)
        {
            this.CategoryId = categoryId;
        }
        public void setAmount(decimal amount)
        {
            this.Amount = amount;
        }
        public void SetDate(DateTime date)
        {
            this.Date = date;
        }
        public void SetBank(int bankId)
        {
            this.BankId = bankId;
        }
    }
}
