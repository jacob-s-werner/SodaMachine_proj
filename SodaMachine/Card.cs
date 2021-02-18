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
        public double MoneyInAccount
        {
            get
            {
                return moneyInAccount;
            }
        }
        public Card()
        {
            DepositMoney();
        }
        //Member Methods (Can Do)
        //Fills card with starting money
        private void DepositMoney()
        {
            moneyInAccount = 4;
        }
        public void WithdrawMoney(double amountToWithdraw)
        {
            moneyInAccount -= amountToWithdraw;
        }
    }
}
