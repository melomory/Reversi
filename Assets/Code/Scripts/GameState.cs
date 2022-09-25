using System;
using System.Linq;
using System.Collections.Generic;

public class GameState
{
    public BoardState BoardState { get; set; }

    private Player _currentPlayer;
    public Player CurrentPlayer
    {
        get => _currentPlayer;
        set 
        { 
            _currentPlayer = value;
            BoardState.CurrentPlayer = _currentPlayer;
        }
    }

    public bool GameOver { get; set; }

    public Player Winner { get; set; }

    public bool PlayerIsAI { get; set; } = false;

    public bool PlayerAIMoved { get; set; } = false;

    public GameState()
    {
        BoardState = new BoardState(this);
    }
}
