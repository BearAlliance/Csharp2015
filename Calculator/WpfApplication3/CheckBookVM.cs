using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using Checkbook;
using Checkbook.Model;

namespace Checkbook
{
    public class CheckBookVM : BaseVM
    {
        public CheckBookVM()
        {
        }

        string maxPayer;
        CbDb _Db = new CbDb();

        private ObservableCollection<Transaction> _Transactions;
        public ObservableCollection<Transaction> Transactions
        {
            get { return _Transactions; }
            set { _Transactions = value; OnPropertyChanged(); OnPropertyChanged("Transations");
            findMaxPayer();
            }
        }

        public void findMaxPayer()
        {
            

             // Group by payee
                var payees =
                from t in Transactions
                group t by t.Payee;

            // Find the largest single transaction
            // And set it to maxPayer
            double maxPay = 0;
            foreach (var i in Transactions) 
            {
                if (i.Amount > maxPay) {
                    maxPay = i.Amount;
                    maxPayer = i.Payee.ToString();
                }
            }
            var turnRed = false;

            // If the selected transaction contains the maxPayer as Payee
            if (maxPayer == SelectedTransaction.Payee.ToString()) {
                 // Fire an event that triggeres the background to change
                turnRed = true;
            }
            else {
                turnRed = false;
            }
        }

       // private ObservableCollection<Transaction> _Accounts;
        //public ObservableCollection<Transaction> Accounts
        //{
           // get { return _Accounts; }
           // set { _Accounts = value; OnPropertyChanged(); OnPropertyChanged("Accounts"); }
       // }

        public ObservableCollection<Account> Accounts
        {
            get { return _Db.Accounts.Local; }
        }

        //public IEnumerable<Transaction> Transactions
        //{
         //   get { return _Db.Transactions.Local; }
        //}

        public DelegateCommand Save
        {
            get
            {
                return new DelegateCommand
                {
                    ExecuteFunction = _ => _Db.SaveChanges(),
                    CanExecuteFunction = _ => _Db.ChangeTracker.HasChanges()
                };
            }
        }

        public DelegateCommand NewTransaction
        {
            get
            {
                return new DelegateCommand
                {
                    ExecuteFunction = _ =>
                    {
                        Transactions.Add(new Transaction { Date = DateTime.Now, Account=CurrentAccount });
                    }
                };
            }
        }


        public DelegateCommand CreateAccount
        {
            get
            {
                return new DelegateCommand
                {
                    ExecuteFunction = _ =>
                    {
                        _Db.Accounts.Add(new Account { Name = "NewAcct" });
                        _Db.Accounts.ToList();
                    }
                };
            }
        }

        public DelegateCommand DeleteAccount
        {
            get
            {
                return new DelegateCommand
                {
                    ExecuteFunction = _ =>
                    {
                        if (CurrentAccount != null)
                        {
                            _Db.Accounts.Remove(CurrentAccount);
                        }
                        _Db.Accounts.ToList();
                    }
                };
            }
        }


        public DelegateCommand DeleteTransaction
        {
            get
            {
                return new DelegateCommand
                {
                    ExecuteFunction = _ =>
                    {
                        if (SelectedTransaction != null)
                        {
                            _Db.Transactions.Remove(SelectedTransaction);
                        }
                        _Db.Transactions.ToList();
                    }
                };
            }
        }

        public Account CurrentAccount { get; set; }
        public float Total { get; set; }

        public Transaction SelectedTransaction { get; set; }



        public void Fill()
        {
            _Transactions = _Db.Transactions.Local;

            _Db.Accounts.Add(new Account {Name="Checking" });
            _Db.Accounts.Add(new Account { Name = "Savings" });
            _Db.Accounts.ToList();
            _Db.Transactions.ToList();
        }
    }
}