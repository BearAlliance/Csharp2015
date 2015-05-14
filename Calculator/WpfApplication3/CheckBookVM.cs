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

        CbDb _Db = new CbDb();

        private ObservableCollection<Transaction> _Transactions;
        public ObservableCollection<Transaction> Transactions
        {
            get { return _Transactions; }
            set { _Transactions = value; OnPropertyChanged(); OnPropertyChanged("Accounts"); }
        }

        public IEnumerable<Account> Accounts
        {
            get { return _Db.Accounts.Local; }
        }

        public String NewAcct{get; set;}

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
                        Transactions.Add(new Transaction { });
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
                        _Db.Accounts.Add(new Account {Name= NewAcct });
                    }
                };
            }
        }

        public float Total { get; set; }



        public void Fill()
        {
            _Transactions = _Db.Transactions.Local;

            _Db.Accounts.Add(new Account {Name="Checking" });
            _Db.Accounts.Add(new Account { Name = "Savings" });
            _Db.Accounts.ToList();
            _Db.Transactions.ToList();
            //new ObservableCollection<Transaction>();
        }
    }
}