using System;

public class Heuristic : IHeuristic
{
    private readonly double[,] _boardHeuristic;

    public Heuristic()
    {
        _boardHeuristic = new double[BoardState.Rows, BoardState.Columns]
        {
            { 1000, -10, 10, 10, 10, 10, -10, 1000 },
            { -10, -10, 1, 1, 1, 1, -10, -10 },
            { 10, 1, 1, 1, 1, 1, 1, 10 },
            { 10, 1, 1, 1, 1, 1, 1, 10 },
            { 10, 1, 1, 1, 1, 1, 1, 10 },
            { 10, 1, 1, 1, 1, 1, 1, 10 },
            { -10, -10, 1, 1, 1, 1, -10, -10 },
            { 1000, -10, 10, 10, 10, 10, -10, 1000 }
        };
    }

    public double GetScore(BoardState boardState, Player player)
    {
        var opponent = player.Opponent();

        var playerDiskCount = 0;
        var opponentDiskCount = 0;

        var playerWeighted = 0.0;
        var opponentWeighted = 0.0;


        for (int row = 0; row < BoardState.Rows; row++)
        {
            for (int column = 0; column < BoardState.Columns; column++)
            {
                if (boardState.Board[row, column] == player)
                {
                    playerDiskCount++;
                    playerWeighted += _boardHeuristic[row, column];
                }
                else if (boardState.Board[row, column] == opponent)
                {
                    opponentDiskCount++;
                    opponentWeighted += _boardHeuristic[row, column];
                }
            }
        }

        var emptySpaceCount = BoardState.Rows * BoardState.Columns - playerDiskCount - opponentDiskCount;

        if (emptySpaceCount > 0)
        {
            return playerWeighted - opponentWeighted;
        }

        return Convert.ToDouble(playerDiskCount - opponentDiskCount);
    }
}
