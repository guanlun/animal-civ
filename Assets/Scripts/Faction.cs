using System.Collections.Generic;

public class Faction
{
    public bool isPlayerFaction = false;
    public List<Unit> units = new List<Unit>();

    public List<Buidling> buildings = new List<Buidling>();

    public List<Hex> exploredHexes = new List<Hex>();

    private FactionResources resources;

    public Faction(bool isPlayerFaction = false)
    {
        this.isPlayerFaction = isPlayerFaction;
        this.resources = new FactionResources(10, 10, 10);
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
        foreach (Unit unit in this.units) {
            unit.ComputePossibleActions();
            unit.TakeBestAction();
        }
    }

    public void EndTurn()
    {
        foreach (Unit unit in this.units) {
            unit.ResetRemainingMoves();
        }

        foreach (Buidling buidling in this.buildings) {
            buidling.ComputeResources(this.resources);
        }
    }

    public FactionResources GetResources()
    {
        return this.resources;
    }
}