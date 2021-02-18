using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SodaMachine
{
    class SodaMachine
    {
        //Member Variables (Has A)
        private List<Coin> _register;
        private List<Can> _inventory;

        //Constructor (Spawner)
        public SodaMachine()
        {
            _register = new List<Coin>();
            _inventory = new List<Can>();
            FillInventory();
            FillRegister();
        }

        //Member Methods (Can Do)

        //A method to fill the sodamachines register with coin objects.
        public void FillRegister()
        {
            Quarter quarter = new Quarter();
            Dime dime = new Dime();
            Nickel nickel = new Nickel();
            Penny penny = new Penny();

            AdministratorDepositCoinsIntoRegister(quarter, 20);
            AdministratorDepositCoinsIntoRegister(dime, 10);
            AdministratorDepositCoinsIntoRegister(nickel, 20);
            AdministratorDepositCoinsIntoRegister(penny, 50);
        }
        //A method to fill the sodamachines inventory with soda can objects.
        public void FillInventory()
        {
            Cola cola = new Cola();
            RootBeer rootBeer = new RootBeer();
            OrangeSoda orangeSoda = new OrangeSoda();

            AdministratorDepositCansIntoInventory(cola, 10);
            AdministratorDepositCansIntoInventory(rootBeer, 10);
            AdministratorDepositCansIntoInventory(orangeSoda, 1);
        }
        //Method to be called to start a transaction.
        //Takes in a customer which can be passed freely to which ever method needs it.
        public void BeginTransaction(Customer customer)
        {
            bool willProceed = UserInterface.DisplayWelcomeInstructions(_inventory);
            if (willProceed)
            {
                Transaction(customer);
            }
        }
        
        //This is the main transaction logic think of it like "runGame".  This is where the user will be prompted for the desired soda.
        //grab the desired soda from the inventory.
        //get payment from the user.
        //pass payment to the calculate transaction method to finish up the transaction based on the results.
        private void Transaction(Customer customer)
        {
            string customerCanSelection = "";
            bool useCard;
            Can customerCanSelected = null;
            List<Coin> paymentFromCustomer = new List<Coin>();

            customerCanSelection = UserInterface.SodaSelection(_inventory);
            customerCanSelected = GetSodaFromInventory(customerCanSelection);
            useCard = UserInterface.ContinuePrompt("Would you like to use a Credit Card for payment? (y/n)");
            
            if (useCard)
            {
                CalculateTransaction(customer.Wallet.creditCard, customerCanSelected, customer);
            }
            else
            {
                paymentFromCustomer = customer.GatherCoinsFromWallet(customerCanSelected);
                CalculateTransaction(paymentFromCustomer, customerCanSelected, customer);
                UserInterface.DisplayTotalValueOfCoins(customer.Wallet.Coins);
            }
            UserInterface.OutputText($"You currently have {customer.Backpack.cans.Count} cans in your Backpack");
        }
        //Gets a soda from the inventory based on the name of the soda.
        private Can GetSodaFromInventory(string nameOfSoda)
        {
            Can matchingCan = null; //validation is in the Transaction Method already

            foreach (Can can in _inventory)
            {
                if (can.Name == nameOfSoda)
                {
                    matchingCan = can;
                }
            }
            return matchingCan;
        }

        //This is the main method for calculating the result of the transaction.
        //It takes in the payment from the customer, the soda object they selected, and the customer who is purchasing the soda.
        private void CalculateTransaction(Card card, Can chosenSoda, Customer customer)
        {
            if (_inventory.Contains(chosenSoda) == false)
            {
                UserInterface.OutputText($"The Vending Machine doesn't have any more {chosenSoda.Name} in stock - TRANSACTION CANCELED.\nSorry for the Inconvenience!");
            }
            else if (Math.Round(card.MoneyInAccount,2) < Math.Round(chosenSoda.Price,2))
            {
                UserInterface.OutputText($"Card Declined - TRANSACTION CANCELED.\nSorry for the Inconvenience!");
            }
            else
            {
                card.WithdrawMoney(chosenSoda.Price);
                UserInterface.EndMessage(chosenSoda.Name, card);
                customer.AddCanToBackpack(chosenSoda);
                _inventory.Remove(chosenSoda);
            }
        }
        private void CalculateTransaction(List<Coin> payment, Can chosenSoda, Customer customer)
        {
            double paymentTotalValue = TotalCoinValue(payment);
            double changeTotalValue = 0;
            List<Coin> changeGathered = new List<Coin>();

            DepositCoinsIntoRegister(payment);
            
            if (paymentTotalValue >= chosenSoda.Price && _inventory.Contains(chosenSoda) == false)
            {
                UserInterface.OutputText($"The Vending Machine doesn't have any more {chosenSoda.Name} in stock - TRANSACTION CANCELED.\nSorry for the Inconvenience! Take your payment of {paymentTotalValue} back from below.");
                changeGathered = GatherChange(paymentTotalValue);
                customer.AddCoinsIntoWallet(changeGathered);
            }
            else if (paymentTotalValue < chosenSoda.Price)
            {
                //If the payment does not meet the cost of the soda: dispense payment back to the customer.
                UserInterface.OutputText($"The payment does not meet the cost of the soda - TRANSACTION CANCELED. \nTake your payment of {paymentTotalValue} back from below.");
                changeGathered = GatherChange(paymentTotalValue);
                customer.AddCoinsIntoWallet(changeGathered);
            }
            else if (Math.Round(paymentTotalValue,2) == Math.Round(chosenSoda.Price,2))
            {
                //If the payment is exact to the cost of the soda:  Despense soda.
                CompleteTransaction(chosenSoda, customer, changeTotalValue);
            }
            else
            {
                changeTotalValue = DetermineChange(paymentTotalValue, chosenSoda.Price);
                changeGathered = GatherChange(changeTotalValue);

                if (changeGathered == null)
                {
                    //If the payment is greater than the cost of the soda, but the machine does not have ample change: Despense payment back to the customer.
                    UserInterface.OutputText($"The Vending Machine doesn't have enough change to give back - TRANSACTION CANCELED.\nSorry for the Inconvenience! Take your payment of {paymentTotalValue} back from below.");
                    changeGathered = GatherChange(paymentTotalValue);
                    customer.AddCoinsIntoWallet(changeGathered);
                }
                else 
                {
                    //If the payment is greater than the price of the soda, and if the sodamachine has enough change to return: Despense soda, and change to the customer.
                    CompleteTransaction(chosenSoda, customer, changeTotalValue);
                    customer.AddCoinsIntoWallet(changeGathered);
                }
            }
        }
        //Takes in the value of the amount of change needed.
        //Attempts to gather all the required coins from the sodamachine's register to make change.
        //Returns the list of coins as change to despense.
        //If the change cannot be made, return null.
        private List<Coin> GatherChange(double changeValue)
        {
            List<Coin> changeGathered = new List<Coin>();
            double changeNeeded = changeValue;
            
            changeNeeded = GetCoinsFromRegisterToMatchChangeNeeded(changeGathered, changeNeeded, "Quarter", 0.25);
            changeNeeded = GetCoinsFromRegisterToMatchChangeNeeded(changeGathered, changeNeeded, "Dime", 0.10);
            changeNeeded = GetCoinsFromRegisterToMatchChangeNeeded(changeGathered, changeNeeded, "Nickel", 0.05);
            changeNeeded = GetCoinsFromRegisterToMatchChangeNeeded(changeGathered, changeNeeded, "Penny", 0.01);

            if (Math.Round(TotalCoinValue(changeGathered),2) == Math.Round(changeValue,2))
            {
                return changeGathered;
            }
            else
            {
                DepositCoinsIntoRegister(changeGathered);
                return null;
            }
        }
        public double GetCoinsFromRegisterToMatchChangeNeeded(List<Coin> changeGathered, double changeNeeded, string coinName, double coinValue)
        {
            Coin coinGathered = new Coin();

            while (RegisterHasCoin(coinName) && Math.Round(changeNeeded, 2) >= coinValue)
            {
                coinGathered = GetCoinFromRegister(coinName);
                changeGathered.Add(coinGathered);
                changeNeeded -= coinValue;
            }
            return changeNeeded;
        }
        //Reusable method to check if the register has a coin of that name.
        //If it does have one, return true.  Else, false.
        private bool RegisterHasCoin(string name)
        {
            foreach (Coin coin in _register)
            {
                if (coin.Name == name)
                {
                    return true;
                }
            }
            return false;
        }
        //Reusable method to return a coin from the register.
        //Returns null if no coin can be found of that name.
        private Coin GetCoinFromRegister(string name)
        {
            Coin coinToGrab = new Coin();

            foreach (Coin coin in _register)
            {
                if (coin.Name == name)
                {
                    coinToGrab = coin;
                    _register.Remove(coin);
                    return coinToGrab;
                }
            }
            return null;
        }
        //Takes in the total payment amount and the price of can to return the change amount.
        private double DetermineChange(double totalPayment, double canPrice)
        {
            return (totalPayment - canPrice);
        }
        //Takes in a list of coins to returnt he total value of the coins as a double.
        private double TotalCoinValue(List<Coin> payment)
        {
            double totalCoinValue = 0;

            foreach (Coin coin in payment)
            {
                totalCoinValue += coin.Value;
            }

            return Math.Round(totalCoinValue,2);
        }
        //Puts a list of coins into the soda machines register.
        private void DepositCoinsIntoRegister(List<Coin> coins)
        {
            foreach (Coin coin in coins)
            {
                _register.Add(coin);
            }
        }
        private void AdministratorDepositCoinsIntoRegister(Coin coin, int numberOfCoinsToDeposit)
        {
            for (int i = 0; i < numberOfCoinsToDeposit; i++)
            {
                _register.Add(coin);
            }
           
        }
        private void AdministratorDepositCansIntoInventory(Can can, int numberOfCansToDeposit)
        {
            for (int i = 0; i < numberOfCansToDeposit; i++)
            {
                _inventory.Add(can);
            }
        }
        private void CompleteTransaction(Can can, Customer customer, double changeGivenBack)
        {
            UserInterface.EndMessage(can.Name, changeGivenBack);
            customer.AddCanToBackpack(can);
            _inventory.Remove(can);
        }
    }
}
