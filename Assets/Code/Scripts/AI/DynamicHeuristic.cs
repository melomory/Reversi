//public class DynamicHeuristic : IHeuristic
//{
//    private double[,] _boardValuation;
//    private double[] _scoreWeights;

//    private readonly double[] defaultTileValuation = { 20, -3, 11, 8, -7, -4, 1, 2, 2, -3 };
//    private readonly double[] defaultScoreWeights = { 10, 801.724, 382.026, 78.922, 74.396, 10 };
   

//    public DynamicHeuristic(double[] mirroredTileValuation = null, double[] scoreWeights = null)
//    {
//        if (mirroredTileValuation is null)
//        {
//            mirroredTileValuation = defaultTileValuation;
//        }

//        _boardValuation = getBoardValuation(mirroredTileValuation);

//        if (scoreWeights is null)
//        {
//            _scoreWeights = defaultScoreWeights;
//        }
//        _scoreWeights = scoreWeights;
//    }

//    //public void WriteArray(int[,] array)
//    //{
//    //    for (int y = 0; y < array.GetLength(0); ++y)
//    //    {
//    //        for (int x = 0; x < array.GetLength(1); ++x)
//    //        {
//    //            Console.Write(array[x, y] + " ");
//    //        }
//    //        Console.WriteLine();
//    //    }
//    //}


//    public double GetScore(BoardState boardState, Player player)
//    {
//        var opponent = player.Opponent();
//        BoardSquareState playerTile = ReversiBoard.getTile(playerColor);
//        BoardSquareState opponentTile = ReversiBoard.getTile(opponentColor);

//        List<Position> boardLocations = boardState.LegalMoves;  // all positions

//        int my_tiles = 0, opp_tiles = 0, my_front_tiles = 0, opp_front_tiles = 0;
//        double p = 0, c = 0, l = 0, m = 0, f = 0, d = 0;

//        // Piece difference, frontier disks and disk squares
//        foreach (Position location in boardLocations)
//        {
//            if (board.GetBoardState(location) == playerTile)
//            {
//                d += boardValuation[location.X, location.Y];
//                my_tiles++;
//            }
//            else if (board.GetBoardState(location) == opponentTile)
//            {
//                d -= boardValuation[location.X, location.Y];
//                opp_tiles++;
//            }
//            if (board.GetBoardState(location) != BoardSquareState.Empty)
//            {
//                foreach (Point direction in board.directions)
//                {
//                    Point adj_space = location; adj_space.Offset(direction);
//                    if (!ReversiBoard.OutOfBounds(adj_space) && board.GetBoardState(adj_space) == BoardSquareState.Empty)
//                    {
//                        if (board.GetBoardState(location) == playerTile) my_front_tiles++;
//                        else opp_front_tiles++;
//                        break;
//                    }
//                }
//            }
//        }
//        if (my_tiles > opp_tiles)
//            p = (100.0 * my_tiles) / (my_tiles + opp_tiles);
//        else if (my_tiles < opp_tiles)
//            p = -(100.0 * opp_tiles) / (my_tiles + opp_tiles);
//        else p = 0;

//        if (my_front_tiles > opp_front_tiles)
//            f = -(100.0 * my_front_tiles) / (my_front_tiles + opp_front_tiles);
//        else if (my_front_tiles < opp_front_tiles)
//            f = (100.0 * opp_front_tiles) / (my_front_tiles + opp_front_tiles);
//        else f = 0;

//        // Corner occupancy
//        int playerCorner = 0, opponentCorner = 0;
//        if (board.GetBoardState(new Point(0, 0)) == playerTile) ++playerCorner;
//        else if (board.GetBoardState(new Point(0, 0)) == opponentTile) ++opponentCorner;
//        if (board.GetBoardState(new Point(7, 0)) == playerTile) ++playerCorner;
//        else if (board.GetBoardState(new Point(7, 0)) == opponentTile) ++opponentCorner;
//        if (board.GetBoardState(new Point(0, 7)) == playerTile) ++playerCorner;
//        else if (board.GetBoardState(new Point(0, 7)) == opponentTile) ++opponentCorner;
//        if (board.GetBoardState(new Point(7, 7)) == playerTile) ++playerCorner;
//        else if (board.GetBoardState(new Point(7, 7)) == opponentTile) ++opponentCorner;
//        c = 25 * (playerCorner - opponentCorner);

//        // Corner closeness
//        int playerCornerClose = 0, opponentCornerClose = 0;
//        if (board.GetBoardState(new Point(0, 0)) == BoardSquareState.Empty)
//        {
//            if (board.GetBoardState(new Point(0, 1)) == playerTile) playerCornerClose++;
//            else if (board.GetBoardState(new Point(0, 1)) == opponentTile) opponentCornerClose++;
//            if (board.GetBoardState(new Point(1, 1)) == playerTile) playerCornerClose++;
//            else if (board.GetBoardState(new Point(1, 1)) == opponentTile) opponentCornerClose++;
//            if (board.GetBoardState(new Point(1, 0)) == playerTile) playerCornerClose++;
//            else if (board.GetBoardState(new Point(1, 0)) == opponentTile) opponentCornerClose++;
//        }
//        if (board.GetBoardState(new Point(0, 7)) == BoardSquareState.Empty)
//        {
//            if (board.GetBoardState(new Point(0, 6)) == playerTile) playerCornerClose++;
//            else if (board.GetBoardState(new Point(0, 6)) == opponentTile) opponentCornerClose++;
//            if (board.GetBoardState(new Point(1, 6)) == playerTile) playerCornerClose++;
//            else if (board.GetBoardState(new Point(1, 6)) == opponentTile) opponentCornerClose++;
//            if (board.GetBoardState(new Point(1, 7)) == playerTile) playerCornerClose++;
//            else if (board.GetBoardState(new Point(1, 7)) == opponentTile) opponentCornerClose++;
//        }
//        if (board.GetBoardState(new Point(7, 0)) == BoardSquareState.Empty)
//        {
//            if (board.GetBoardState(new Point(7, 1)) == playerTile) playerCornerClose++;
//            else if (board.GetBoardState(new Point(7, 1)) == opponentTile) opponentCornerClose++;
//            if (board.GetBoardState(new Point(6, 1)) == playerTile) playerCornerClose++;
//            else if (board.GetBoardState(new Point(6, 1)) == opponentTile) opponentCornerClose++;
//            if (board.GetBoardState(new Point(6, 0)) == playerTile) playerCornerClose++;
//            else if (board.GetBoardState(new Point(6, 0)) == opponentTile) opponentCornerClose++;
//        }
//        if (board.GetBoardState(new Point(7, 7)) == BoardSquareState.Empty)
//        {
//            if (board.GetBoardState(new Point(6, 7)) == playerTile) playerCornerClose++;
//            else if (board.GetBoardState(new Point(6, 7)) == opponentTile) opponentCornerClose++;
//            if (board.GetBoardState(new Point(6, 6)) == playerTile) playerCornerClose++;
//            else if (board.GetBoardState(new Point(6, 6)) == opponentTile) opponentCornerClose++;
//            if (board.GetBoardState(new Point(7, 6)) == playerTile) playerCornerClose++;
//            else if (board.GetBoardState(new Point(7, 6)) == opponentTile) opponentCornerClose++;
//        }
//        l = -12.5 * (playerCornerClose - opponentCornerClose);

//        // Mobility
//        int playerValidMoves = board.NumLegalMoves(playerColor);
//        int opponentValidMoves = board.NumLegalMoves(opponentColor);
//        if (playerValidMoves > opponentValidMoves)
//            m = (100.0 * playerValidMoves) / (playerValidMoves + opponentValidMoves);
//        else if (playerValidMoves < opponentValidMoves)
//            m = -(100.0 * opponentValidMoves) / (playerValidMoves + opponentValidMoves);
//        else m = 0;

//        // final weighted score
//        double score = (scoreWeights[0] * p) + (scoreWeights[1] * c) + (scoreWeights[2] * l)
//            + (scoreWeights[3] * m) + (scoreWeights[4] * f) + (scoreWeights[5] * d);
//        return score;
//    }

//    private double[,] getBoardValuation(double[] mirroredTileValuations)
//    {
//        double[,] boardValuation = new double[ReversiBoard.boardSize, ReversiBoard.boardSize];
//        int mirroredTileValuationsIndex = 0;
//        int halfSize = ReversiBoard.boardSize / 2;

//        if (mirroredTileValuations.Length != ((halfSize + 1) * halfSize) / 2)
//        {
//            Console.WriteLine(((ReversiBoard.boardSize + 1) * ReversiBoard.boardSize) / 2);
//            throw new ArgumentException("Not the right amount of valuations to fill the board.");
//        }

//        // Fill in one quarter of the board
//        for (int x = 0; x < 4; ++x)
//        {
//            for (int y = x; y < 4; ++y)
//            {
//                boardValuation[x, y] = mirroredTileValuations[mirroredTileValuationsIndex];
//                boardValuation[y, x] = mirroredTileValuations[mirroredTileValuationsIndex];
//                ++mirroredTileValuationsIndex;
//            }
//        }
//        // Fill in the rest of the board
//        for (int x = 0; x < 4; ++x)
//        {
//            for (int y = 0; y < 4; ++y)
//            {
//                boardValuation[7 - x, y] = boardValuation[x, y];
//                boardValuation[x, 7 - y] = boardValuation[x, y];
//                boardValuation[7 - x, 7 - y] = boardValuation[x, y];
//            }
//        }
//        return boardValuation;
//    }
//}