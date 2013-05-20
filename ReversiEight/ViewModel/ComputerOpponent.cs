using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;

namespace ReversiEight.ViewModel
{
  public class ComputerOpponent
  {
    private struct Move
    {
      public int Row;
      public int Column;
    }

    private int _maxDepth;

    private GameBoardViewModel _viewModel;

    private BoardSquareState _computerColor;

    public ComputerOpponent(GameBoardViewModel viewModel, BoardSquareState computerColor, int maxDepth)
    {
      _maxDepth = maxDepth;
      _computerColor = computerColor;
      _viewModel = viewModel;
      _viewModel.PropertyChanged += GameBoardViewModel_PropertyChanged;

      MakeMoveIfCorrectTurn();
    }

    private void GameBoardViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      if (e.PropertyName == "NextMove")
      {
        MakeMoveIfCorrectTurn();
      }
    }

    private void MakeMoveIfCorrectTurn()
    {
      if (_viewModel.NextMove == _computerColor)
      {
        // make a move after a short delay.
        var timer = new DispatcherTimer();
        timer.Interval = TimeSpan.FromSeconds(0.5);
        timer.Tick += (s, e2) =>
        {
          MakeNextMove();
          timer.Stop();
        };
        timer.Start();
      }
    }

    private void MakeNextMove()
    {
      Move bestMove = new Move()
      {
        Column = -1,
        Row = -1
      };
      int bestScore = int.MinValue;

      // check every valid move for this particular board, then select the one with the best 'score'
      List<Move> moves = ValidMovesForBoard(_viewModel);
      foreach (Move nextMove in moves)
      {
        // clone the current board and make this move
        GameBoardViewModel testBoard = new GameBoardViewModel(_viewModel);
        testBoard.MakeMove(nextMove.Row, nextMove.Column);

        // determine the score for this move
        int scoreForMove = ScoreForBoard(testBoard, 1);

        // pick the best
        if (scoreForMove > bestScore || bestScore == int.MinValue)
        {
          bestScore = scoreForMove;
          bestMove.Row = nextMove.Row;
          bestMove.Column = nextMove.Column;
        }
      }

      if (bestMove.Column != -1 && bestMove.Row != -1)
      {
        _viewModel.MakeMove(bestMove.Row, bestMove.Column);
      }
    }

    // Computes the score for the given board
    private int ScoreForBoard(GameBoardViewModel board, int depth)
    {
      // if we have reached the maximum search depth, then just compute the score of the current
      // board state
      if (depth >= _maxDepth)
      {
        return _computerColor == BoardSquareState.WHITE ?
                                  board.WhiteScore - board.BlackScore :
                                  board.BlackScore - board.WhiteScore;
      }

      int minMax = int.MinValue;

      // check every valid next move for this particular board
      List<Move> moves = ValidMovesForBoard(board);
      foreach (Move nextMove in moves)
      {
        // clone the current board and make the move
        GameBoardViewModel testBoard = new GameBoardViewModel(board);
        testBoard.MakeMove(nextMove.Row, nextMove.Column);

        // compute the score for this board
        int score = ScoreForBoard(testBoard, depth + 1);

        // pick the best score
        if (depth % 2 == 0)
        {
          if (score > minMax || minMax == int.MinValue)
          {
            minMax = score;
          }
        }
        else
        {
          if (score < minMax || minMax == int.MinValue)
          {
            minMax = score;
          }
        }
      }

      return minMax;
    }

    // returns an array of valid next moves for the given board
    private List<Move> ValidMovesForBoard(GameBoardViewModel viewModel)
    {
      List<Move> moves = new List<Move>();

      for (int row = 0; row < 8; row++)
      {
        for (int col = 0; col < 8; col++)
        {
          if (viewModel.IsValidMove(row, col))
          {
            moves.Add(new Move()
            {
              Row = row,
              Column = col
            });
          }
        }
      }

      return moves;
    }
  }
}
