using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BudgetTracker.Models
{
    public class Bank
    {
        public int BankId { get; set; }
        public string Location { get; set; }
        public string Name { get; set; }
        public Bank(int bankId, string location, string name)
        {
            BankId = bankId;
            Location = location;
            Name = name;
        }
    }
}
