public abstract class Action
{
    public Unit unit;

    public abstract void ToggleTargetState(bool toggleOn);

    public abstract void Execute();

    public abstract float GetResultScore();
}