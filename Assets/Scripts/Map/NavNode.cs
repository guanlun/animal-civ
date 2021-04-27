public class NavNode
{
    public int remainingMoves;
    public Hex fromHex;

    public NavNode(int remainingMoves, Hex fromHex = null)
    {
        this.remainingMoves = remainingMoves;
        this.fromHex = fromHex;
    }
}