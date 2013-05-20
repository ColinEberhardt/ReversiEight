using ReversiEight.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ReversiEight.ViewModel
{
  /// <summary>
  /// Represents an individual square on the reversi board
  /// </summary>
  public class GameBoardSquareViewModel : ViewModelBase
  {
    private BoardSquareState _state;

    private GameBoardViewModel _parent;

    public GameBoardSquareViewModel(int row, int col, GameBoardViewModel parent)
    {
      Column = col;
      Row = row;
      _parent = parent;
    }

    public int Column { get; private set; }

    public int Row { get; private set; }

    public ICommand CellTapped
    {
      get
      {
        return new DelegateCommand(() =>
        {
          _parent.MakeMove(Row, Column);
        });
      }
    }

    public BoardSquareState State 
    {
      get { return _state; }
      set { SetField<BoardSquareState>(ref _state, value, "State"); } 
    }
  }
}
