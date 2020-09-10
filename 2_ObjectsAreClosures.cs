using System;
using System.Collections.Generic;
using FluentAssertions;
using Xunit;

namespace Closures
{
    public class BankAccountExample
    {
        public class BankAccount
        {
            private decimal _balance = 0;
            private List<string> _creditCards = new List<string>();

            public void Withdraw(decimal amount) => _balance -= amount;

            public void Deposit(decimal amount) => _balance += amount;

            public decimal GetBalance() => _balance;

            public void AddCreditCard(string cardNumber) => _creditCards.Add(cardNumber);

            public List<string> GetCreditCards() => _creditCards;
        }

        public Func<string, object[], object>  MakeBankAccount1()
        {
            decimal balance = 0;
            var creditCards = new List<string>();

            return (string method, object[] arguments) =>
                method switch 
                {
                    "Withdraw" =>       DoAction(() => balance -= (int)arguments[0]),
                    "Deposit" =>        DoAction(() => balance += (int)arguments[0]),
                    "GetBalance" =>     balance,
                    "AddCreditCard" =>  DoAction(() => creditCards.Add(arguments[0] as string)),
                    "GetCreditCards" => creditCards,

                    _ => throw new Exception($"Unknown method: {method}")
                };

            object DoAction(Action action) { action(); return null; }
        }

        public Dictionary<string, Func<object[], object>>  MakeBankAccount2()
        {
            decimal balance = 0;
            var creditCards = new List<string>();

            return new Dictionary<string, Func<object[], object>>
            {
                {"Withdraw",       (object[] args) => { balance -= (int)args[0]; return null; }},
                {"Deposit",        (object[] args) => { balance += (int)args[0]; return null; }},
                {"GetBalance",     (object[] args) => { return balance; }},
                {"AddCreditCard",  (object[] args) => { creditCards.Add(args[0] as string); return null; }},
                {"GetCreditCards", (object[] args) => { return creditCards; }}
            };
        }

        [Fact]
        public void DemoMakeBankAccount1()
        {
            {
                var account = new BankAccount();

                account.GetBalance().Should().Be(0);

                account.Deposit(100);
                account.Withdraw(50);

                account.GetBalance().Should().Be(50);
            }

            {
                var account = MakeBankAccount1();

                account("GetBalance", new object[] {}).Should().Be(0);

                account("Deposit", new object[] { 100 });
                account("Withdraw", new object[] { 50 });

                account("GetBalance", new object[] {}).Should().Be(50);
            }

            {
                var account = MakeBankAccount2();

                account["GetBalance"](new object[] {}).Should().Be(0);

                account["Deposit"](new object[] { 100 });
                account["Withdraw"](new object[] { 50 });

                account["GetBalance"](new object[] {}).Should().Be(50);
            }
        }
    }
}
