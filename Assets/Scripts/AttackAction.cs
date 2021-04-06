public class AttackAction : Action
{
    public Unit targetUnit;

    public AttackAction(Unit unit, Unit targetUnit)
    {
        this.unit = unit;
        this.targetUnit = targetUnit;
    }

    public override void ToggleTargetState(bool toggleOn)
    {
        this.targetUnit.SetAttackTargetIndicatorActive(toggleOn);
    }

    public override void Execute()
    {
        this.unit.AttackTarget(this.targetUnit);
    }

    public override float GetResultScore()
    {
        return 0.2f;
    }
}