using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SodaMachine
{
    class Customer
    {
        //Member Variables (Has A)
        public Wallet Wallet;
        public Backpack Backpack;

        //Constructor (Spawner)
        public Customer()
        {
            Wallet = new Wallet();
            Backpack = new Backpack();
        }
        //Member Methods (Can Do)

        //This method will be the main logic for a customer to retrieve coins form their wallet.
        //Takes in the selected can for price reference
        //Will need to get user input for coins they would like to add.
        //When all is said and done this method will return a list of coin objects that the customer will use a payment for their soda.
        public List<Coin> GatherCoinsFromWallet(Can selectedCan)
        {
            bool keepGatheringCoins = true;
            string coinChosenName;
            Coin coinChosen;
            List<Coin> coinsToPayWith = new List<Coin>();

            while (keepGatheringCoins)
            {
                coinChosenName = UserInterface.CoinSelection(selectedCan, Wallet.Coins);
                coinChosen = GetCoinFromWallet(coinChosenName);
                coinsToPayWith.Add(coinChosen);

                Console.WriteLine("\nTo spend on the machine:");
                UserInterface.DisplayCost(selectedCan);
                UserInterface.DisplayTotalValueOfCoins(coinsToPayWith);
                keepGatheringCoins = UserInterface.ContinuePrompt("Do you want to keep adding coins to spend in the Soda Machine? (y/n)");
            }
            return coinsToPayWith;
        }
        //Returns a coin object from the wallet based on the name passed into it.
        //Returns null if no coin can be found
        public Coin GetCoinFromWallet(string coinName)
        {
            Coin coin = null;

            for (int i = 0; i < Wallet.Coins.Count ; i++)
            {
                if (Wallet.Coins[i].Name == coinName)
                {
                    coin = Wallet.Coins[i];
                    Wallet.Coins.RemoveAt(i);
                    return coin;
                }
            }
            return coin;
        }
        //Takes in a list of coin objects to add into the customers wallet.
        public void AddCoinsIntoWallet(List<Coin> coinsToAdd)
        {
            foreach (Coin coin in coinsToAdd)
            {
                Wallet.Coins.Add(coin);
            }
        }
        //Takes in a can object to add to the customers backpack.
        public void AddCanToBackpack(Can purchasedCan)
        {
            Backpack.cans.Add(purchasedCan);
        }
    }
}
