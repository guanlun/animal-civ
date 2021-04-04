public class MoveAction : Action
{
    public Hex destHex;

    public MoveAction(Unit unit, Hex destHex)
    {
        this.unit = unit;
        this.destHex = destHex;
    }

    public override void ToggleTargetState(bool toggleOn)
    {
        this.destHex.SetAdjacent(toggleOn);
    }

    public override void Execute()
    {
        this.unit.MoveToHex(this.destHex);
    }

    public override float GetResultScore()
    {
        return 0.5f;
    }
}