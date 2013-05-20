using System;
using System.ComponentModel;

namespace ReversiEight.Common
{
  /// <summary>
  /// A base view model, provides INotifyPropertyChanged 
  /// </summary>
  public class ViewModelBase : INotifyPropertyChanged
  {
    #region INotifyPropertyChanged Members

    public event PropertyChangedEventHandler PropertyChanged;

    protected void OnPropertyChanged(string property)
    {
      if (PropertyChanged != null)
      {
        PropertyChanged(this, new PropertyChangedEventArgs(property));
      }
    }

    #endregion

 

    /// <summary>
    /// Sets the given field raising a property changed event if teh given value differs.
    /// </summary>
    protected void SetField<T>(ref T field, T value, string propertyName)
    {
      if (Object.Equals(field, value))
        return;

      field = value;
      OnPropertyChanged(propertyName);
    }
  }
}
