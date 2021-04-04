public abstract class Action
{

    public abstract void ToggleTargetState(bool toggleOn);

    public abstract void Execute();

    public abstract float GetResultScore();
}