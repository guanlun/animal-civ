public class MoveAction : Action
{
    public Hex destHex;

    public NavNode navNode;

    public MoveAction(Unit unit, Hex destHex, NavNode navNode)
    {
        this.unit = unit;
        this.destHex = destHex;
        this.navNode = navNode;
    }

    public override void ToggleTargetState(bool toggleOn)
    {
        this.destHex.SetMoveable(toggleOn);
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