using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SodaMachine
{
    class Wallet
    {
        //Member Variables (Has A)
        public List<Coin> Coins;
        //Constructor (Spawner)
        public Wallet()
        {
            Coins = new List<Coin>();
            FillRegister();
        }
        //Member Methods (Can Do)
        //Fills wallet with starting money
        private void FillRegister()
        {
          
        }
        private void InitialDepositOfCoinsIntoWallet(Coin coin, int numberOfCoinsToDeposit)
        {
            for (int i = 0; i < numberOfCoinsToDeposit; i++)
            {
                Coins.Add(coin);
            }

        }
    }
}
