public class Navigability
{
    public bool isNavigable;

    public int movementCost;

    public Navigability(int movementCost, bool isNavigable = true)
    {
        this.movementCost = movementCost;
        this.isNavigable = isNavigable;
    }
    public static Navigability plainNavigability = new Navigability(1);
    public static Navigability hillNavigability = new Navigability(2);
    public static Navigability forestNavigability = new Navigability(2);
    public static Navigability mountainNavigability = new Navigability(100, false);
    public static Navigability waterNavigability = new Navigability(100, false);
}
