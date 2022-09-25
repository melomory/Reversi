using System.Collections.Generic;
using System.Linq;

public class BoardState 
{
    private GameState _gameState = null;

    public const int Rows = 8;

    public const int Columns = 8;

    public Player[,] Board { get; set; }

    private Player _currentPlayer;

    public Player CurrentPlayer
    {
        get => _currentPlayer;
        set 
        { 
            _currentPlayer = value;
            LegalMoves = FindLegalMoves(_currentPlayer);
        }
    }

    public Dictionary<Player, int> DiscCount { get; set; }

    public Dictionary<Position, List<Position>> LegalMoves { get; private set; }

    public BoardState()
    {
    }

    public BoardState(GameState gameState)
    {
        Board = new Player[Rows, Columns];
        Board[3, 3] = Player.White;
        Board[3, 4] = Player.Black;
        Board[4, 3] = Player.Black;
        Board[4, 4] = Player.White;

        DiscCount = new Dictionary<Player, int>()
        {
            { Player.Black, 2 },
            { Player.White, 2 }
        };

        LegalMoves = FindLegalMoves(CurrentPlayer);
        _gameState = gameState;
    }

    public BoardState CloneBoard()
    {
        var boardState = new BoardState();
        boardState.Board = new Player[Rows, Columns];
        boardState.DiscCount = new Dictionary<Player, int>(DiscCount);
        boardState.LegalMoves = new Dictionary<Position, List<Position>>(LegalMoves);
        for (int row = 0; row < Rows; row++)
        {
            for (int column = 0; column < Columns; column++)
            {
                boardState.Board[row, column] = Board[row, column];
            }
        }
        return boardState;
    }

    public bool MakeMove(Position position, out MoveInfo moveInfo)
    {
        if (!LegalMoves.ContainsKey(position))
        {
            moveInfo = null;
            return false;
        }

        var movePlayer = CurrentPlayer;
        var outflanked = LegalMoves[position];
        Board[position.Row, position.Column] = movePlayer;
        FlipDiscs(outflanked);
        UpdateDiscCounts(movePlayer, outflanked.Count);
        PassTurn();

        moveInfo = new MoveInfo { Player = movePlayer, Position = position, Outflanked = outflanked };
        return true;
    }

    public IEnumerable<Position> OccupiedPositions()
    {
        for (int row = 0; row < Rows; row++)
        {
            for (int column = 0; column < Columns; column++)
            {
                if (Board[row, column] != Player.None)
                {
                    yield return new Position(row, column);
                }
            }
        }
    }

    private void FlipDiscs(List<Position> positions)
    {
        foreach (var position in positions)
        {
            Board[position.Row, position.Column] = Board[position.Row, position.Column].Opponent();
        }
    }

    private void UpdateDiscCounts(Player movePlayer, int outflankedCount)
    {
        DiscCount[movePlayer] += outflankedCount + 1;
        DiscCount[movePlayer.Opponent()] -= outflankedCount;
    }

    private void ChangePlayer()
    {
        if (_gameState != null)
        {
            _gameState.PlayerIsAI = !_gameState.PlayerIsAI;
            _gameState.CurrentPlayer = _gameState.CurrentPlayer.Opponent();
        }
        else
        {
            CurrentPlayer = CurrentPlayer.Opponent();
        }
        LegalMoves = FindLegalMoves(CurrentPlayer);
    }

    private Player FindWinner()
    {
        if (DiscCount[Player.Black] > DiscCount[Player.White])
        {
            return Player.Black;
        }
        if (DiscCount[Player.White] > DiscCount[Player.Black])
        {
            return Player.White;
        }

        return Player.None;
    }

    private void PassTurn()
    {
        ChangePlayer();

        if (LegalMoves.Count > 0)
        {
            return;
        }

        ChangePlayer();
        if (_gameState != null && _gameState.PlayerIsAI)
        {
            _gameState.PlayerAIMoved = false;
        }
        if (_gameState?.BoardState.LegalMoves.Count == 0)
        {
            CurrentPlayer = Player.None;
            _gameState.GameOver = true;
            _gameState.Winner = FindWinner();
        }
    }

    private bool IsInsideBoard(int rows, int columns)
    {
        return rows >= 0 && rows < Rows && columns >= 0 && columns < Columns;
    }

    private List<Position> OutflankedInDirecion(Position position, Player player, int rowDelta, int columnDelta)
    {
        var outflanked = new List<Position>();
        int row = position.Row + rowDelta;
        int column = position.Column + columnDelta;

        while (IsInsideBoard(row, column) && Board[row, column] != Player.None)
        {
            if (Board[row, column] == player.Opponent())
            {
                outflanked.Add(new Position(row, column));
                row += rowDelta;
                column += columnDelta;
            }
            else // if (Board[row, column] == player)
            {
                return outflanked;
            }
        }

        return new List<Position>();
    }

    private List<Position> Outflanked(Position position, Player player)
    {
        var outflanked = new List<Position>();

        for (int rowDelta = -1; rowDelta <= 1; rowDelta++)
        {
            for (int columnDelta = -1; columnDelta <= 1; columnDelta++)
            {
                if (rowDelta == 0 && columnDelta == 0)
                {
                    continue;
                }

                outflanked.AddRange(OutflankedInDirecion(position, player, rowDelta, columnDelta));
            }
        }
        return outflanked;
    }

    private bool IsMoveLegal(Player player, Position position, out List<Position> outflanked)
    {
        if (Board[position.Row, position.Column] != Player.None)
        {
            outflanked = null;
            return false;
        }

        outflanked = Outflanked(position, player);
        return outflanked.Any();
    }

    private Dictionary<Position, List<Position>> FindLegalMoves(Player player)
    {
        var legalMoves = new Dictionary<Position, List<Position>>();

        for (int row = 0; row < Rows; row++)
        {
            for (int column = 0; column < Columns; column++)
            {
                var position = new Position(row, column);

                if (IsMoveLegal(player, position, out List<Position> outflanked))
                {
                    legalMoves[position] = outflanked;
                }
            }
        }

        return legalMoves;
    }
}
