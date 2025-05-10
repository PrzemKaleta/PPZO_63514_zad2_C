using System;
using System.Collections.Generic;

public class Transaction
{
    public string TransactionType { get; }
    public int Amount { get; }
    public DateTime Timestamp { get; }
    public string Description { get; }

    public Transaction(string transactionType, int amount, string description = "")
    {
        TransactionType = transactionType;
        Amount = amount;
        Timestamp = DateTime.Now;
        Description = description;
    }

    public override string ToString()
    {
        return $"{Timestamp:yyyy-MM-dd HH:mm:ss} - " +
               $"{char.ToUpper(TransactionType[0]) + TransactionType.Substring(1)}: {Amount} PLN - " +
               $"{Description}";
    }
}

public class Account
{
    public string AccountNumber { get; }
    public string OwnerName { get; }
    public int Balance { get; private set; }
    public List<Transaction> TransactionHistory { get; }

    public Account(string accountNumber, string ownerName, int initialBalance = 0)
    {
        AccountNumber = accountNumber;
        OwnerName = ownerName;
        Balance = initialBalance;
        TransactionHistory = new List<Transaction>();
    }

    internal void AdjustBalance(int amountAdjustment)
    {
        Balance += amountAdjustment;
    }

    public bool Deposit(int amount)
    {
        if (amount <= 0)
        {
            Console.WriteLine("Błąd: Kwota wpłaty musi być dodatnią liczbą całkowitą");
            return false;
        }
        AdjustBalance(amount);
        Transaction transactionObj = new Transaction("wpłata", amount, "Wpłata środków");
        TransactionHistory.Add(transactionObj);
        Console.WriteLine($"\nWpłacono {amount} PLN na konto o numerze {AccountNumber}");
        Console.WriteLine($"Nowe saldo: {Balance} PLN");
        return true;
    }

    public bool Withdraw(int amount)
    {
        if (amount <= 0)
        {
            Console.WriteLine("Błąd: Kwota wypłaty musi być dodatnią liczbą całkowitą");
            return false;
        }

        if (Balance >= amount)
        {
            AdjustBalance(-amount);
            Transaction transactionObj = new Transaction("wypłata", amount, "Wypłata środków");
            TransactionHistory.Add(transactionObj);
            Console.WriteLine($"\nWypłacono {amount} PLN z konta o numerze {AccountNumber}");
            Console.WriteLine($"Nowe saldo: {Balance} PLN");
            return true;
        }
        else
        {
            Console.WriteLine($"Błąd: Niewystarczające środki na koncie {AccountNumber}. " +
                              $"Saldo: {Balance} PLN, próba wypłaty: {amount} PLN");
            return false;
        }
    }

    public class Bank
    {
        public Dictionary<string, Account> Accounts { get; }
        private int _nextAccountNumberBase;

        public Bank()
        {
            Accounts = new Dictionary<string, Account>();
            _nextAccountNumberBase = 0;
        }

        public string GenerateAccountNumber()
        {
            _nextAccountNumberBase++;
            return _nextAccountNumberBase.ToString();
        }

        public Account? CreateAccount(string ownerName, int initialDeposit = 0)
        {
            string accountNumber = GenerateAccountNumber();
            if (initialDeposit < 0)
            {
                Console.WriteLine("Błąd przy tworzeniu konta: Saldo początkowe nie może być ujemne");
                return null;
            }

            if (string.IsNullOrEmpty(ownerName))
            {
                Console.WriteLine("Błąd przy tworzeniu konta: Nazwa właściciela nie może być pusta");
                return null;
            }

            Account newAccount = new Account(accountNumber, ownerName, initialDeposit);
            Accounts[accountNumber] = newAccount;

            if (initialDeposit > 0)
            {
                Transaction initialTransaction = new Transaction("wpłata", initialDeposit, "Wpłata początkowa");
                newAccount.TransactionHistory.Add(initialTransaction);
            }
            return newAccount;
        }

        public Account? GetAccount(string accountNumber)
        {
            if (string.IsNullOrEmpty(accountNumber)) return null;
            Accounts.TryGetValue(accountNumber, out Account? account);
            return account;
        }

        public class Program
        {
            public static void Main(string[] args)
            {
                Bank mainBank = new Bank();

                while (true)
                {
                    Console.WriteLine("\n--- MENU SYSTEMU BANKOWEGO ---");
                    Console.WriteLine("1. Utwórz nowe konto");
                    Console.WriteLine("2. Wpłać środki na konto");
                    Console.WriteLine("3. Wypłać środki z konta");
                    Console.WriteLine("0. Wyjdź z systemu");

                    string? userChoice = Console.ReadLine();

                    switch (userChoice)
                    {
                        case "1":
                            Console.WriteLine("\n--- Tworzenie nowego konta ---");
                            Console.Write("Podaj imię i nazwisko właściciela: ");
                            string? ownerNameInput = Console.ReadLine();
                            if (string.IsNullOrEmpty(ownerNameInput))
                            {
                                Console.WriteLine("Nazwa właściciela nie może być pusta");
                                break;
                            }

                            int initialBalanceValue = 0;
                            while (true)
                            {
                                Console.Write("Podaj kwotę początkowej wpłaty: ");
                                string? balanceInput = Console.ReadLine();
                                if (int.TryParse(balanceInput, out initialBalanceValue))
                                {
                                    if (initialBalanceValue < 0)
                                    {
                                        Console.WriteLine("Saldo początkowe nie może być ujemne. Spróbuj ponownie");
                                        continue;
                                    }
                                    break;
                                }
                                else
                                {
                                    Console.WriteLine("Nieprawidłowa kwota, wpisz liczbę całkowitą i spróbuj ponownie");
                                }
                            }

                            Account? newAccountObject = mainBank.CreateAccount(ownerNameInput, initialBalanceValue);
                            if (newAccountObject != null)
                            {
                                Console.WriteLine("\nKonto zostało pomyślnie utworzone");
                                Console.WriteLine($"  Numer konta: {newAccountObject.AccountNumber}");
                                Console.WriteLine($"  Właściciel: {newAccountObject.OwnerName}");
                                Console.WriteLine($"  Saldo: {newAccountObject.Balance} PLN");
                            }
                            break;

                        case "2":
                            Console.WriteLine("\n--- Wpłata środków ---");
                            Console.Write("Podaj numer konta, na które chcesz wpłacić środki: ");
                            string? accountNumberInputDeposit = Console.ReadLine();
                            if (string.IsNullOrEmpty(accountNumberInputDeposit)) { Console.WriteLine("Numer konta nie może być pusty."); break; }

                            Account? targetAccountDeposit = mainBank.GetAccount(accountNumberInputDeposit);
                            if (targetAccountDeposit != null)
                            {
                                int amountValueDeposit = 0;
                                while (true)
                                {
                                    Console.Write("Podaj kwotę do wpłaty (pełna kwota): ");
                                    string? amountInput = Console.ReadLine();
                                    if (int.TryParse(amountInput, out amountValueDeposit))
                                    {
                                        break;
                                    }
                                    else
                                    {
                                        Console.WriteLine("Nieprawidłowa kwota, wpisz liczbę całkowitą i spróbuj ponownie");
                                    }
                                }
                                targetAccountDeposit.Deposit(amountValueDeposit);
                            }
                            else
                            {
                                Console.WriteLine($"Konto o numerze {accountNumberInputDeposit} nie istnieje");
                            }
                            break;

                        case "3":
                            Console.WriteLine("\n--- Wypłata środków ---");
                            Console.Write("Podaj numer konta, z którego chcesz wypłacić środki: ");
                            string? accountNumberInputWithdraw = Console.ReadLine();
                            if (string.IsNullOrEmpty(accountNumberInputWithdraw)) { Console.WriteLine("Numer konta nie może być pusty."); break; }

                            Account? targetAccountWithdraw = mainBank.GetAccount(accountNumberInputWithdraw);
                            if (targetAccountWithdraw != null)
                            {
                                int amountValueWithdraw = 0;
                                while (true)
                                {
                                    Console.Write("Podaj kwotę do wypłaty (pełna kwota): ");
                                    string? amountInput = Console.ReadLine();
                                    if (int.TryParse(amountInput, out amountValueWithdraw))
                                    {
                                        break;
                                    }
                                    else
                                    {
                                        Console.WriteLine("Nieprawidłowa kwota. Wpisz liczbę całkowitą. Spróbuj ponownie");
                                    }
                                }
                                targetAccountWithdraw.Withdraw(amountValueWithdraw);
                            }
                            else
                            {
                                Console.WriteLine($"Konto o numerze {accountNumberInputWithdraw} nie istnieje");
                            }
                            break;

                        case "0":
                            Console.WriteLine("Koniec");
                            return;

                        default:
                            Console.WriteLine("Nieprawidłowa opcja, wybierz numer od 0 do 7");
                            break;
                    }
                }
            }
        }
    }
}