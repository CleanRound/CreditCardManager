public class CreditCard
{
    public string CardNumber { get; private set; }
    public string OwnerName { get; private set; }
    public DateTime Validity { get; private set; }
    public string PIN { get; private set; }
    public decimal CreditLimit { get; private set; }
    public decimal Balance { get; private set; }
    public decimal CreditUsed { get; private set; }

    public event Action<decimal> Refilled;
    public event Action<decimal> FundsSpent;
    public event Action CreditFundsUsed;
    public event Action<decimal> LimitReached;
    public event Action<string> PINChanged;

    public CreditCard(string cardNumber, string ownerName, DateTime validity, string pin, decimal creditLimit, decimal initialBalance)
    {
        CardNumber = cardNumber;
        OwnerName = ownerName;
        Validity = validity;
        PIN = pin;
        CreditLimit = creditLimit;
        Balance = initialBalance;
        CreditUsed = 0;
    }

    public void Refill(decimal amount)
    {
        Balance += amount;
        Refilled?.Invoke(amount);
    }

    public bool SpendFunds(decimal amount)
    {
        if (Balance >= amount)
        {
            Balance -= amount;
            FundsSpent?.Invoke(amount);
            return true;
        }
        else if (Balance + (CreditLimit - CreditUsed) >= amount)
        {
            decimal creditNeeded = amount - Balance;
            Balance = 0;
            CreditUsed += creditNeeded;
            CreditFundsUsed?.Invoke();
            FundsSpent?.Invoke(amount);
            return true;
        }
        else
        {
            LimitReached?.Invoke(amount);
            return false;
        }
    }

    public void ChangePIN(string newPin)
    {
        PIN = newPin;
        PINChanged?.Invoke(newPin);
    }
}

public class Program
{
    public static void Main()
    {
        CreditCard card = new CreditCard("1234-5678-9123-4567", "John Doe", new DateTime(2026, 12, 31), "1234", 5000m, 1000m);

        card.Refilled += amount => Console.WriteLine($"Refilled: {amount:C}");
        card.FundsSpent += amount => Console.WriteLine($"Spent: {amount:C}");
        card.CreditFundsUsed += () => Console.WriteLine("Credit funds started to be used.");
        card.LimitReached += amount => Console.WriteLine($"Attempted to spend {amount:C}, but limit reached.");
        card.PINChanged += newPin => Console.WriteLine($"PIN changed to: {newPin}");

        card.Refill(200m);
        card.SpendFunds(300m);
        card.SpendFunds(1000m);
        card.SpendFunds(6000m);
        card.ChangePIN("4321");
    }
}
