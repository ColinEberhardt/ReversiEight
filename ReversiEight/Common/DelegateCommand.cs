using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ReversiEight.Common
{
  public class DelegateCommand : ICommand
  {
    private readonly Action _executeMethod;

    #region Constructors

    public DelegateCommand(Action executeMethod)
    {
      _executeMethod = executeMethod;
    }

    #endregion Constructors

    #region ICommand Members

    public event EventHandler CanExecuteChanged;

    public bool CanExecute(object parameter)
    {
      return true;
    }

    public void Execute(object parameter)
    {
      _executeMethod();
    }

    #endregion ICommand Members

    #region Public Methods


    public void RaiseCanExecuteChanged()
    {
      OnCanExecuteChanged(EventArgs.Empty);
    }

    #endregion Public Methods

    #region Protected Methods

    protected virtual void OnCanExecuteChanged(EventArgs e)
    {
      var handler = CanExecuteChanged;
      if (handler != null)
      {
        handler(this, e);
      }
    }

    #endregion Protected Methods
  }
}
