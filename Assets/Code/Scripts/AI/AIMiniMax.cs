using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AIMiniMax : IAI
{
    private Heuristic _heuristic;
    private int _searchDepth;
    private GameState _gameState;

    public AIMiniMax(Heuristic heuristic, int searchDepth, GameState gameState)
    {
        if (searchDepth < 1)
        {
            throw new ArgumentException("SearchDepth cannot be smaller than 1.");
        }
        _gameState = gameState;
        _heuristic = heuristic;
        _searchDepth = searchDepth;
    }

    public Position GetMove(BoardState boardState, Player player)
    {
        // Illegal position.
        Position bestMove = new Position(-1, -1);
        double bestScore = double.MinValue;

        List<Position> boardPositions = boardState.LegalMoves.Keys.ToList();
        foreach (Position boardPosition in boardPositions)
        {
            BoardState boardWithMove = boardState.CloneBoard();
            boardWithMove.CurrentPlayer = player;
            if (boardWithMove.MakeMove(boardPosition, out MoveInfo moveInfo))
            {
                double score = getBoardValuation(boardWithMove, boardWithMove.CurrentPlayer, _searchDepth - 1);
                if (score >= bestScore)
                {
                    bestScore = score;
                    bestMove = boardPosition;
                }
            }
        }

        return bestMove;
    }

    double getBoardValuation(BoardState boardState, Player optimisingPlayer, int currentSearchDepth)
    {
        if (currentSearchDepth == 0)
        {
            return _heuristic.GetScore(boardState, optimisingPlayer);
        }

        bool isMax = _gameState.BoardState.CurrentPlayer == optimisingPlayer;

        // Illegal position.
        Position bestMove = new Position(-1, -1);
        double bestScore = isMax ? double.MinValue : double.MaxValue;

        if (_gameState.GameOver)
        {
            int optimisingPlayerScore = boardState.DiscCount[optimisingPlayer];
            int opponentScore = boardState.DiscCount[optimisingPlayer.Opponent()];
            if (optimisingPlayerScore > opponentScore)
            {
                return double.MaxValue;
            }

            if (opponentScore > optimisingPlayerScore)
            {
                return double.MinValue;
            }
            return 0;
        }

        List<Position> boardPositions = boardState.LegalMoves.Keys.ToList();
        foreach (Position boardPosition in boardPositions)
        {
            BoardState boardWithMove = boardState.CloneBoard();
            boardWithMove.CurrentPlayer = optimisingPlayer;

            if (boardWithMove.MakeMove(boardPosition, out MoveInfo moveInfo))
            {
                double score = getBoardValuation(boardWithMove, boardWithMove.CurrentPlayer, currentSearchDepth - 1);
                if (isMax)
                {
                    if (score >= bestScore)
                    {
                        bestScore = score;
                        bestMove = boardPosition;
                    }
                }
                else
                {
                    if (score <= bestScore)
                    {
                        bestScore = score;
                        bestMove = boardPosition;
                    }
                }
            }
        }
        return bestScore;
    }
}
