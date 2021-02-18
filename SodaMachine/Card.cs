using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SodaMachine
{
    class Card
    {//Member Variables (Has A)
        protected double moneyInAccount;
        //Constructor (Spawner)
        public Card()
        {
            DepositMoney();
        }
        //Member Methods (Can Do)
        //Fills wallet with starting money
        private void DepositMoney()
        {
            moneyInAccount = 4;
        }
    }
}
