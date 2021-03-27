using System.Collections.Generic;

public class Faction
{
    public bool isPlayerFaction = false;
    public List<Unit> units = new List<Unit>();

    public List<Buidling> buildings = new List<Buidling>();

    public List<Hex> exploredHexes = new List<Hex>();
    public Faction(bool isPlayerFaction = false)
    {
        this.isPlayerFaction = isPlayerFaction;
    }

    public void AddUnit(Unit unit)
    {
        this.units.Add(unit);
        unit.SetFaction(this);
    }

    public void AddBuilding(Buidling buidling)
    {
        this.buildings.Add(buidling);
        buidling.SetFaction(this);
    }

    public void StartTurn()
    {
        foreach (Unit unit in this.units)
        {

        }
    }
}