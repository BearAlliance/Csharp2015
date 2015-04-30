using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Checkbook
{
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
    }

    public class DelegateCommand : ICommand
    {

        public Predicate<object> CanExecuteFunction { get; set; }
        public Action<object> ExecuteFunction { get; set; }

        public bool CanExecute(object parameter)
        {
            if (CanExecuteFunction != null)
                return CanExecuteFunction(parameter);
            else
                return true;
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }
        public void OnCanExecuteChanged()
        {
            CommandManager.InvalidateRequerySuggested();
        }

        public void Execute(object parameter)
        {
            if (ExecuteFunction != null) ExecuteFunction(parameter);
        }
    }
    public class BaseVM : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string propertyName = null)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
