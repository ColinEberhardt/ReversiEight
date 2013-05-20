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
  /// A view model that encapsulates the logic of the game of Reversi
  /// </summary>
  public class GameBoardViewModel : ViewModelBase
  {
    delegate void NavigationFunction(ref int row, ref int col);

    private static List<NavigationFunction> _navigationFunctions = new List<NavigationFunction>();

    static GameBoardViewModel()
    {
      _navigationFunctions.Add(delegate(ref int row, ref int col) { row++; });
      _navigationFunctions.Add(delegate(ref int row, ref int col) { row--; });
      _navigationFunctions.Add(delegate(ref int row, ref int col) { row++; col--; });
      _navigationFunctions.Add(delegate(ref int row, ref int col) { row++; col++; });
      _navigationFunctions.Add(delegate(ref int row, ref int col) { row--; col--; });
      _navigationFunctions.Add(delegate(ref int row, ref int col) { row--; col++; });
      _navigationFunctions.Add(delegate(ref int row, ref int col) { col++; });
      _navigationFunctions.Add(delegate(ref int row, ref int col) { col--; });
    }

    private List<GameBoardSquareViewModel> _squares;

    private BoardSquareState _nextMove = BoardSquareState.WHITE;

    private int _whiteScore = 0;

    private int _blackScore = 0;

    private bool _gameOver = false;

    public GameBoardViewModel(GameBoardViewModel copy)
    {
      _squares = copy._squares.Select(s => new GameBoardSquareViewModel(s.Row, s.Column, this)
      {
        State = s.State
      }).ToList();
      _nextMove = copy._nextMove;
    }

    public GameBoardViewModel()
    {
      _squares = new List<GameBoardSquareViewModel>();
      for (int col = 0; col < 8; col++)
      {
        for (int row = 0; row < 8; row++)
        {
          _squares.Add(new GameBoardSquareViewModel(row, col, this));
        }
      }

      InitialiseGame();
    }

    public bool GameOver
    {
      get { return _gameOver; }
      private set
      {
        SetField<bool>(ref _gameOver, value, "GameOver");
      }
    }

    public int BlackScore
    {
      get { return _blackScore; }
      private set
      {
        SetField<int>(ref _blackScore, value, "BlackScore");
      }
    }

    public int WhiteScore
    {
      get { return _whiteScore; }
      private set
      {
        SetField<int>(ref _whiteScore, value, "WhiteScore");
      }
    }

    public List<GameBoardSquareViewModel> Squares
    {
      get { return _squares; }
    }

    public BoardSquareState NextMove
    {
      get { return _nextMove; }
      private set
      {
        SetField<BoardSquareState>(ref _nextMove, value, "NextMove");
      }
    }

    public ICommand RestartGame
    {
      get
      {
        return new DelegateCommand(() => InitialiseGame());
      }
    }

    /// <summary>
    /// Makes the given move for the current player. Score are updated and play then moves
    /// to the next player.
    /// </summary>
    public void MakeMove(int row, int col)
    {
      // is this a valid move?
      if (!IsValidMove(row, col, NextMove))
        return;

      // set the square to its new state
      GetSquare(row, col).State = NextMove;

      // flip the opponents counters
      FlipOpponentsCounters(row, col, NextMove);
      
      // swap moves
      NextMove = InvertState(NextMove);

      // if this player cannot make a move, swap back again
      if (!CanPlayerMakeAMove(NextMove))
      {
        NextMove = InvertState(NextMove);
      }

      // check whether the game has finished
      GameOver = HasGameFinished();

      // update the scores
      BlackScore = _squares.Count(s => s.State == BoardSquareState.BLACK);
      WhiteScore = _squares.Count(s => s.State == BoardSquareState.WHITE);
    }

    /// <summary>
    /// Determines whether the given move is valid for the next turn
    /// </summary>
    public bool IsValidMove(int row, int col)
    {
      return IsValidMove(row, col, NextMove);
    }

    /// <summary>
    /// Determines whether the given move is valid
    /// </summary>
    public bool IsValidMove(int row, int col, BoardSquareState state)
    {
      // check the cell is empty
      if (GetSquare(row, col).State != BoardSquareState.EMPTY)
        return false;

      // if counters are surrounded in any direction, the move is valid
      return _navigationFunctions.Any(navFunction => MoveSurroundsCounters(row, col, navFunction, state));
    }

    /// <summary>
    /// Determines whether the given move 'surrounds' any of the opponents pieces.
    /// </summary>
    private bool MoveSurroundsCounters(int row, int column,
      NavigationFunction navigationFunction, BoardSquareState state)
    {
      int index = 1;

      var squares = NavigateBoard(navigationFunction, row, column);
      foreach(var square in squares)
      {
        BoardSquareState currentCellState = square.State;

        // the cell that is the immediate neighbour must be of the other colour
        if (index == 1)
        {
          if (currentCellState != InvertState(state))
          {
            return false;
          }
        }
        else
        {
          // if we have reached a cell of the same colour, this is a valid move
          if (currentCellState == state)
          {
            return true;
          }

          // if we have reached an empty cell - fail
          if (currentCellState == BoardSquareState.EMPTY)
          {
            return false;
          }
        }

        index++;
      }

      return false;
    }

    /// <summary>
    /// Flips all the opponents pieces that are surrounded by the given move.
    /// </summary>
    private void FlipOpponentsCounters(int row, int column, BoardSquareState state)
    {
      foreach (var navigationFunction in _navigationFunctions)
      {
        // are any pieces surrounded in this direction?
        if (!MoveSurroundsCounters(row, column, navigationFunction, state))
          continue;

        BoardSquareState opponentsState = InvertState(state);

        var squares = NavigateBoard(navigationFunction, row, column);
        foreach (var square in squares)
        {
          if (square.State == state)
            break;

          square.State = state;
        }
      }
    }

    private bool HasGameFinished()
    {
        return  !CanPlayerMakeAMove(BoardSquareState.BLACK) &&
                !CanPlayerMakeAMove(BoardSquareState.WHITE);
    }

    /// <summary>
    /// Determines whether there are any valid moves that the given player can make.
    /// </summary>
    private bool CanPlayerMakeAMove(BoardSquareState state)
    {
        // test all the board locations to see if a move can be made
        for (int row = 0; row < 8; row++)
        {
            for (int col = 0; col < 8; col++)
            {
                if (IsValidMove(row, col, state))
                {
                    return true;
                }
            }
        }
        return false;
    }

    private GameBoardSquareViewModel GetSquare(int row, int col)
    {
      return _squares.Single(s => s.Column == col && s.Row == row);
    }

    private BoardSquareState InvertState(BoardSquareState state)
    {
      return state == BoardSquareState.BLACK ? BoardSquareState.WHITE : BoardSquareState.BLACK;
    }

    /// <summary>
    /// A list of board squares that are yielded via the given navigation function.
    /// </summary>
    private IEnumerable<GameBoardSquareViewModel> NavigateBoard(NavigationFunction navigationFunction, int row, int column)
    {
      navigationFunction(ref column, ref row);
      while (column >= 0 && column <= 7 && row >= 0 && row <= 7)
      {
        yield return GetSquare(row, column);
        navigationFunction(ref column, ref row);
      }
    }

    /// <summary>
    /// Sets up the view model to the initial game state.
    /// </summary>
    private void InitialiseGame()
    {
      foreach (var square in _squares)
      {
        square.State = BoardSquareState.EMPTY;
      }

      GetSquare(3, 4).State = BoardSquareState.BLACK;
      GetSquare(4, 3).State = BoardSquareState.BLACK;
      GetSquare(4, 4).State = BoardSquareState.WHITE;
      GetSquare(3, 3).State = BoardSquareState.WHITE;

      NextMove = BoardSquareState.BLACK;

      WhiteScore = 0;
      BlackScore = 0;
    }
  }
}
