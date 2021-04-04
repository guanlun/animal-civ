public class AttackAction : Action
{
    public Unit targetUnit;

    public AttackAction(Unit targetUnit)
    {
        this.targetUnit = targetUnit;
    }

    public override void ToggleTargetState(bool toggleOn)
    {
        this.targetUnit.SetAttackTargetIndicatorActive(toggleOn);
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