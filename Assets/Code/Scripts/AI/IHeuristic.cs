public interface IHeuristic
{
    public double GetScore(BoardState boardState, Player player);
}