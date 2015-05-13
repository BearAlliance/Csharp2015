using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Checkbook.Model
{
    public class Transaction : BaseVM
    {
        public int Id { get; set; }


        /*
        public IEnumerable<Transaction> SimilarTransactions {
            get {
                return from t in VM.Transactions
                       where t.Payee == this.Payee
                       select t;
            }
        }
        */
        private DateTime _Date;
        public DateTime Date
        {
            get { return _Date; }
            set { _Date = value; OnPropertyChanged(); }
        }

        private string _Payee;
        public string Payee
        {
            get { return _Payee; }
            set { _Payee = value; OnPropertyChanged(); }
        }

        public int AccountId { get; set; }

        private Account _Account;
        public virtual Account Account
        {
            get { return _Account; }
            set { _Account = value; OnPropertyChanged(); }
        }

        private double _Amount;
        public double Amount
        {
            get { return _Amount; }
            set { _Amount = value; OnPropertyChanged(); }
        }

        private string _Tag;
        public string Tag
        {
            get { return _Tag; }
            set { _Tag = value; OnPropertyChanged(); }
        }


    }

    public class Account
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Institution { get; set; }
        public bool Business { get; set; }

        public virtual IList<Transaction> Transactions { get; set; }
    }

    public class CheckBookVM : BaseVM
    {
        public CheckBookVM()
        {
        }

        CbDb _Db = new CbDb();

        private int _RowsPerPage = 5;
        private int _CurrentPage = 1;
        public int CurrentPage
        {
            get { return _CurrentPage; }
            set { _CurrentPage = value; OnPropertyChanged(); OnPropertyChanged("CurrentTransactions"); }
        }

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

        public IEnumerable<Transaction> CurrentTransactions
        {
            get { return Transactions.Skip((_CurrentPage - 1) * _RowsPerPage).Take(_RowsPerPage); }
        }

        public DelegateCommand MoveNext
        {
            get
            {
                return new DelegateCommand
                {
                    ExecuteFunction = _ => CurrentPage++,
                    CanExecuteFunction = _ => CurrentPage * _RowsPerPage < Transactions.Count
                };
            }
        }

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
                        CurrentPage = Transactions.Count / _RowsPerPage + 1;
                    }
                };
            }
        }

        public void Fill()
        {
            Transactions = _Db.Transactions.Local;
            _Db.Accounts.ToList();
            _Db.Transactions.ToList();
            //new ObservableCollection<Transaction>();
        }
    }
}