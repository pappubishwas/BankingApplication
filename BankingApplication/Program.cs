using System;
using System.Collections.Generic;
using System.Linq;

public class User
{
    public string Username { get; set; }
    public string Password { get; set; }
    public bool IsAdmin { get; set; }
    public List<Account> Accounts { get; set; } = new List<Account>();

    public User(string username, string password,bool isAdmin = false)
    {
        Username = username;
        Password = password;
        IsAdmin = isAdmin;
    }
}

public class Account
{
    private static int AccountCounter = 1;
    public int AccountNumber { get; private set; }
    public string AccountHolderName { get; set; }
    public string AccountType { get; set; }
    public decimal Balance { get; private set; }
    public List<Transaction> Transactions { get; set; } = new List<Transaction>();

    public Account(string name, string type, decimal initialDeposit)
    {
        AccountNumber = AccountCounter++;
        AccountHolderName = name;
        AccountType = type;
        Balance = initialDeposit;
        Transactions.Add(new Transaction("IntialDeposit", initialDeposit));
    }

    public void Deposit(decimal amount)
    {
        Balance += amount;
        Transactions.Add(new Transaction("Deposit", amount));
    }

    public bool Withdraw(decimal amount)
    {
        if (Balance >= amount)
        {
            Balance -= amount;
            Transactions.Add(new Transaction("Withdrawal", amount));
            return true;
        }
        else
        {
            Console.WriteLine("Insufficient balance.");
            return false;
        }
    }

    public void AddMonthlyInterest(decimal rate)
    {
        if (AccountType.ToLower() == "savings")
        {
            decimal interest = Balance * rate;
            Balance += interest;
            Transactions.Add(new Transaction("Interest", interest));
        }
    }
}

public class Transaction
{
    public string TransactionId { get; }
    public string Type { get; }
    public decimal Amount { get; }
    public DateTime Date { get; }

    public Transaction(string type, decimal amount)
    {
        TransactionId = DateTime.Now.ToString("yyyyMMddHHmmssfff");
        Type = type;
        Amount = amount;
        Date = DateTime.Now;
    }
}

public class BankingApplication
{
    private List<User> users = new List<User>();
    private User currentUser = null;
    private const decimal InterestRate = 0.02m;
    public BankingApplication() { 
     users.Add(new User("admin","admin123",isAdmin:true));
    }

    public void RegisterUser()
    {
        if (currentUser != null) {
            Console.WriteLine("Please logout from current account");
            return;
        }
        Console.Write("Enter Username: ");
        string username = Console.ReadLine();
        Console.Write("Enter Password: ");
        string password = Console.ReadLine();
        if (users.Exists(user=> user.Username==username))
        {
            Console.Write("Username is already exits! Kindly give unique name");

        }
        else
        {
            users.Add(new User ( username,  password ));
            Console.WriteLine("Registration successful!");
        }
    }

    public bool Login()
    {
        if (currentUser != null) {
            Console.WriteLine("Please logout before try to login!");
            return false;
        }
        Console.Write("Enter Username: ");
        string username = Console.ReadLine();
        Console.Write("Enter Password: ");
        string password = Console.ReadLine();

        foreach (var user in users)
        {
            if (user.Username == username && user.Password == password)
            {
                currentUser = user;
                Console.WriteLine("Login successful!");
                return true;
            }
        }
        Console.WriteLine("Invalid credentials.");
        return false;
    }

    public void OpenAccount()
    {
        if (currentUser == null)
        {
            Console.WriteLine("Please login before Open an Account.");
            return;
        }

        Console.Write("Enter Account Holder Name: ");
        string name = Console.ReadLine();
        Console.Write("Enter Account Type (savings/checking): ");
        string type = Console.ReadLine();
        Console.Write("Enter Initial Deposit: ");
        decimal initialDeposit = Convert.ToDecimal(Console.ReadLine());
        Account newAccount=new Account(name, type, initialDeposit);
        currentUser.Accounts.Add(newAccount);
        Console.WriteLine("Account opened successfully!");
        Console.WriteLine($"Your Account number is:  {newAccount.AccountNumber}");
    }

    public void ProcessTransaction()
    {
        if (currentUser == null)
        {
            Console.WriteLine("Please Login Before transaction!");
            return;
        }

        
        try
        {
            Console.Write("Enter Account Number: ");
            int accNumber = Convert.ToInt32(Console.ReadLine());
            var account = currentUser.Accounts.Find(a => a.AccountNumber == accNumber);
            if (account == null)
            {
                Console.WriteLine("Account not found.");
                return;
            }

            Console.Write("Enter Transaction Type (deposit/withdraw): ");
            string type = Console.ReadLine();
            Console.Write("Enter Amount: ");
            decimal amount = Convert.ToDecimal(Console.ReadLine());

            if (type.ToLower() == "deposit")
            {
                account.Deposit(amount);
                Console.WriteLine("Deposit successful!");
            }
            else if (type.ToLower() == "withdraw")
            {
                if (account.Withdraw(amount))
                    Console.WriteLine("Withdrawal successful!");
            }
            else
            {
                Console.WriteLine("Invalid transaction type.");
            }
        }
        catch(FormatException)
        {
            Console.WriteLine("Account number should be integer");
            return;
        }

    }

    public void GenerateStatement()
    {
        if (currentUser == null)
        {
            Console.WriteLine("Please Login!");
            return;
        }

        try
        {
            Console.Write("Enter Account Number: ");
            int accNumber = Convert.ToInt32(Console.ReadLine());

            var account = currentUser.Accounts.Find(a => a.AccountNumber == accNumber);
            if (account == null)
            {
                Console.WriteLine("Account not found.");
                return;
            }

            Console.WriteLine("\nTransaction Statement:");
            Console.WriteLine($"Account Holder Name: {account.AccountHolderName}");
            Console.WriteLine($"Account Number: {account.AccountNumber}");
            Console.WriteLine($"Account Types : {account.AccountType}");
            Console.WriteLine($"Account Balance  : {account.Balance}");
            foreach (var transaction in account.Transactions)
            {
                Console.WriteLine($"ID: {transaction.TransactionId}, Date: {transaction.Date}, Type: {transaction.Type}, Amount: {transaction.Amount}");
            }
        } catch (FormatException) {
            Console.WriteLine("Account number should be integer");
            return;
        }
    }

    public void CheckBalance()
    {
        if (currentUser == null)
        {
            Console.WriteLine("Please Login!");
            return;
        }

        try
        {
            Console.Write("Enter Account Number: ");
            int accNumber = Convert.ToInt32(Console.ReadLine());

            var account = currentUser.Accounts.Find(a => a.AccountNumber == accNumber);
            if (account == null)
            {
                Console.WriteLine("Account not found.");
                return;
            }

            Console.WriteLine($"Account Holder Name: {account.AccountHolderName}");
            Console.WriteLine($"Account Number: {account.AccountNumber}");
            Console.WriteLine($"Account Types : {account.AccountType}");
            Console.WriteLine($"Current Balance: {account.Balance}");
        }catch(FormatException) {
            Console.WriteLine("Account number should be integer");
            return;
        }
    }

    public void AddMonthlyInterest()
    {
        if (currentUser == null || !currentUser.IsAdmin)
        {
            Console.WriteLine("Access denied! Only admin can add interest!");
            return;
        }

        foreach (var user in users)
        {
            foreach(var account in user.Accounts)
            {
                account.AddMonthlyInterest(InterestRate);
            }
        }
        Console.WriteLine("Monthly interest added to all savings accounts.");
    }

    public void MainMenu()
    {
        while (true)
        {
            Console.WriteLine("\nBanking Application Menu:");
            Console.WriteLine("1. Register");
            Console.WriteLine("2. Login");
            Console.WriteLine("3. Open Account");
            Console.WriteLine("4. Deposit/Withdraw");
            Console.WriteLine("5. Generate Statement");
            Console.WriteLine("6. Check Balance");
            Console.WriteLine("7. Add Monthly Interest");
            Console.WriteLine("8. Logout");
            Console.WriteLine("9. Exit");
            Console.Write("Choose an option: ");

            try
            {
                int choice = Convert.ToInt32(Console.ReadLine());

                switch (choice)
                {
                    case 1:
                        RegisterUser();
                        break;
                    case 2:
                        Login();
                        break;
                    case 3:
                        OpenAccount();
                        break;
                    case 4:
                        ProcessTransaction();
                        break;
                    case 5:
                        GenerateStatement();
                        break;
                    case 6:
                        CheckBalance();
                        break;
                    case 7:
                        AddMonthlyInterest();
                        break;
                    case 8:
                        if (currentUser == null)
                        {
                            Console.WriteLine("You are already logged out");
                            break;
                        }
                        currentUser = null;
                        Console.WriteLine("Logged out.");
                        break;
                    case 9:
                        return;
                    default:
                        currentUser = null;
                        Console.WriteLine("Invalid option. Please enter a number between 1 and 9.");
                        break;
                }
            }
            catch (FormatException)
            {
                Console.WriteLine("Invalid input. Please enter a valid integer option.");
            }
        }
    }

}

public class Program
{
    public static void Main()
    {
        BankingApplication app = new BankingApplication();
        app.MainMenu();
    }
}
