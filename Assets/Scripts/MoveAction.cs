public class MoveAction : Action
{
    public Hex destHex;

    public MoveAction(Hex destHex)
    {
        this.destHex = destHex;
    }

    public override void ToggleTargetState(bool toggleOn)
    {
        this.destHex.SetAdjacent(toggleOn);
    }

    public override void Execute()
    {
        throw new System.NotImplementedException();
    }

    public override float GetResultScore()
    {
        throw new System.NotImplementedException();
    }
}